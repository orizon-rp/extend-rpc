using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Extend.Callbacks;

internal sealed class RpcCallbackSystem : GameObjectSystem<RpcCallbackSystem>
{
	public readonly ConcurrentDictionary<string, RpcHandlerInfo> Handlers = new();
	public readonly ConcurrentDictionary<Guid, RpcPendingOperation> Operations = new();

	public static event Action<RpcHandlerInfo>? OnHandlerRegistered;
	public static event Action<RpcMessage, TimeSpan>? OnOperationCompleted;
	public static event Action<RpcMessage, RpcError>? OnOperationFailed;
	public static event Action<RpcMessage>? OnOperationTimeout;

	public RpcCallbackSystem( Scene scene ) : base( scene )
	{
		RefreshHandlers();
	}

	private void RefreshHandlers()
	{
		Handlers.Clear();

		var methods = RpcUtility.GetRpcCallbackMethods();

		foreach ( var (component, method, attributes) in methods )
		{
			if ( attributes.FirstOrDefault( a => a is RpcCallbackAttribute ) is not RpcCallbackAttribute )
				throw new Exception( $"Invalid RPC callback attribute: {attributes}" );

			var handlerInfo = new RpcHandlerInfo
			{
				Instance = component, Method = method, Attributes = attributes, RegisteredAt = DateTime.UtcNow
			};

			Log.Info( $"Registering RPC handler: {method.Name}" );

			if ( !Handlers.TryAdd( method.Name, handlerInfo ) )
			{
				Log.Warning(
					$"Failed to register RPC handler: {method.Name}. A handler with the same name already exists" );
				continue;
			}

			OnHandlerRegistered?.Invoke( handlerInfo );
		}

		Log.Info( $"Registered {Handlers.Count} RPC handlers" );
	}

	public async Task<TResult?> WaitForResponse<TResult>( Guid id, TimeSpan timeout )
	{
		var startTime = DateTime.UtcNow;

		using var cts = new CancellationTokenSource( timeout );

		try
		{
			while ( !cts.Token.IsCancellationRequested )
			{
				if ( Operations.TryGetValue( id, out var operation ) )
				{
					Operations.TryRemove( id, out _ );

					var duration = DateTime.UtcNow - startTime;
					var message = new RpcMessage { Id = id, Method = operation.MethodName };

					if ( operation.Exception is not null )
					{
						Log.Error( operation.Exception );

						OnOperationFailed?.Invoke( message, operation.Exception );

						using ( Sandbox.Rpc.FilterInclude( Sandbox.Rpc.Caller ) )
							RpcClient.OnRpcError( message,
								new RpcError.HandlerNotFound { Message = operation.Exception.ToString() },
								message.Method );

						return default;
					}

					OnOperationCompleted?.Invoke( message, duration );
					return (TResult)operation.Result!;
				}

				await Task.Delay( 10, cts.Token );
			}

			var timeoutMessage = new RpcMessage { Id = id, Method = "TIMEOUT" };

			Log.Error( $"RPC operation {id} timed out after {timeout}" );
			OnOperationTimeout?.Invoke( timeoutMessage );

			using ( Sandbox.Rpc.FilterInclude( Sandbox.Rpc.Caller ) )
				RpcClient.OnRpcError( timeoutMessage,
					new RpcError.Timeout { Message = $"RPC operation {id} timed out after {timeout}" },
					string.Empty );

			return default;
		}
		catch ( OperationCanceledException )
		{
			var timeoutMessage = new RpcMessage { Id = id, Method = "TIMEOUT" };

			Log.Error( $"RPC operation {id} timed out after {timeout}" );
			OnOperationTimeout?.Invoke( timeoutMessage );

			using ( Sandbox.Rpc.FilterInclude( Sandbox.Rpc.Caller ) )
				RpcClient.OnRpcError( timeoutMessage,
					new RpcError.Timeout { Message = $"RPC operation {id} timed out after {timeout}" },
					string.Empty );

			return default;
		}
	}

	public void CompleteResponse( RpcMessage response, object? result, string methodName = "" )
	{
		var operation = new RpcPendingOperation
		{
			Result = result, CompletedAt = DateTime.UtcNow, MethodName = methodName
		};

		Operations.TryAdd( response.Id, operation );
	}

	public void CompleteWithError( RpcMessage response, RpcError exception, string methodName = "" )
	{
		var operation = new RpcPendingOperation
		{
			Exception = exception, CompletedAt = DateTime.UtcNow, MethodName = methodName
		};

		Operations.TryAdd( response.Id, operation );
	}
}
