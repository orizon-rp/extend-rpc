using System.Text.Json;
using System.Threading.Tasks;

namespace Extend.Callbacks;

/// <summary>
/// RPC server methods
/// </summary>
public static class RpcServer
{
	/// <summary>
	/// Sends an RPC message to the client.
	/// This method is tagged with the <see cref="Rpc.Host"/> attribute, which means it will be executed on the server and callable by clients.
	/// </summary>
	/// <param name="request">The RPC message to send.</param>
	/// <param name="args">The arguments to pass to the RPC method.</param>
	[Rpc.Host]
	public static async void SendRpc( RpcMessage request, params object[] args )
	{
		try
		{
			if ( !RpcCallbackSystem.Current.Handlers.TryGetValue( request.Method, out var handler ) )
			{
				var error = $"No handler registered for method {request.Method}";

				using ( Rpc.FilterInclude( Rpc.Caller ) )
					RpcClient.OnRpcError( request, new RpcError.HandlerNotFound { Message = error }, request.Method );

				return;
			}

			var taskResult = handler.Method.InvokeWithReturn<Task>( handler.Instance, args );
			await taskResult;

			var returnType = TypeLibrary.GetType( request.MethodReturnTypeName ).TargetType;
			var json = Json.Serialize( taskResult );
			var obj = Json.Deserialize<Dictionary<string, JsonElement>>( json );
			var finalResult = obj.GetValueOrDefault( "Result" ).Deserialize( returnType );

			using ( Rpc.FilterInclude( Rpc.Caller ) )
				RpcClient.OnRpcResponse( request, finalResult, request.Method );
		}
		catch ( Exception e )
		{
			Log.Error( $"RPC execution error for {request.Method}: {e}" );

			using ( Rpc.FilterInclude( Rpc.Caller ) )
				RpcClient.OnRpcError( request, new RpcError.Exception { Message = e.Message }, request.Method );
		}
	}
}
