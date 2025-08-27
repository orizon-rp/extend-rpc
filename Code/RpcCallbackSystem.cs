using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Extend.Callbacks;

internal sealed class RpcCallbackSystem : GameObjectSystem<RpcCallbackSystem>
{
	public readonly ConcurrentDictionary<int, RpcHandlerInfo> Handlers = new();
	public readonly ConcurrentDictionary<Guid, RpcPendingOperation> Operations = new();

	public static event Action<RpcHandlerInfo>? OnHandlerRegistered;
	public static event Action<RpcMessage, TimeSpan>? OnOperationCompleted;
	public static event Action<RpcMessage, RpcError>? OnOperationFailed;
	public static event Action<RpcMessage>? OnOperationTimeout;

	public RpcCallbackSystem( Scene scene ) : base( scene )
	{
		Listen( Stage.SceneLoaded, 0, RefreshHandlers, nameof(RpcCallbackSystem) );
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
			
			if ( !Handlers.TryAdd( method.Identity, handlerInfo ) )
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
					var message = new RpcMessage { Id = id, MethodIdent = operation.MethodIdent };

					if ( operation.Exception is not null )
					{
						Log.Error( operation.Exception );

						OnOperationFailed?.Invoke( message, operation.Exception );

						using ( Rpc.FilterInclude( Rpc.Caller ) )
							RpcClient.OnRpcError( message,
								new RpcError.HandlerNotFound { Message = operation.Exception.ToString() },
								message.MethodIdent );

						return default;
					}

					OnOperationCompleted?.Invoke( message, duration );
					return (TResult)operation.Result!;
				}

				await Task.Delay( 10, cts.Token );
			}

			var timeoutMessage = new RpcMessage { Id = id };

			Log.Error( $"RPC operation {id} timed out after {timeout}" );
			OnOperationTimeout?.Invoke( timeoutMessage );

			using ( Rpc.FilterInclude( Rpc.Caller ) )
				RpcClient.OnRpcError( timeoutMessage,
					new RpcError.Timeout { Message = $"RPC operation {id} timed out after {timeout}" }, -1 );

			return default;
		}
		catch ( OperationCanceledException )
		{
			var timeoutMessage = new RpcMessage { Id = id };

			Log.Error( $"RPC operation {id} timed out after {timeout}" );
			OnOperationTimeout?.Invoke( timeoutMessage );

			using ( Rpc.FilterInclude( Rpc.Caller ) )
				RpcClient.OnRpcError( timeoutMessage,
					new RpcError.Timeout { Message = $"RPC operation {id} timed out after {timeout}" }, -1 );

			return default;
		}
	}

	public void CompleteResponse( RpcMessage response, object? result, int methodIdent )
	{
		var operation = new RpcPendingOperation
		{
			Result = result, CompletedAt = DateTime.UtcNow, MethodIdent = methodIdent
		};

		Operations.TryAdd( response.Id, operation );
	}

	public void CompleteWithError( RpcMessage response, RpcError exception, int methodIdent )
	{
		var operation = new RpcPendingOperation
		{
			Exception = exception, CompletedAt = DateTime.UtcNow, MethodIdent = methodIdent
		};

		Operations.TryAdd( response.Id, operation );
	}
}
