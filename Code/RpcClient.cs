using System.Threading.Tasks;

namespace Extend.Callbacks;

/// <summary>
/// Client for making RPC calls to the server
/// </summary>
public static class RpcClient
{
	/// <summary>
	/// Sends an RPC call to the server and waits for the response with a timeout
	/// </summary>
	/// <typeparam name="TResult">The return type of the RPC call</typeparam>
	/// <param name="methodIdent">The method ident of the RPC method</param>
	/// <param name="methodReturnType">The type of the method return type</param>
	/// <param name="timeout">The maximum time to wait in seconds for the response./></param>
	/// <param name="args">Arguments for the RPC method</param>
	/// <returns>The result of the RPC call</returns>
	/// <exception cref="Exception">If the RPC call fails</exception>
	public static async Task<TResult?> Send<TResult>( int methodIdent, Type methodReturnType, TimeSpan timeout,
		params object[] args )
	{
		var request = RpcMessage.Create( methodIdent, methodReturnType.FullName ?? methodReturnType.Name,
			Connection.Local.Id );

		try
		{
			RpcServer.SendRpc( request, args );
			return await RpcCallbackSystem.Current.WaitForResponse<TResult>( request.Id, timeout );
		}
		catch ( Exception e )
		{
			var method = TypeLibrary.GetMemberByIdent( methodIdent ) as MethodDescription;
			Log.Warning( $"RPC call failed for {method!.Name}: {e.Message}" );
			
			return default!;
		}
	}

	/// <summary>
	/// Internal callback for when the server sends a successful RPC response
	/// </summary>
	/// <param name="response">The original RPC message</param>
	/// <param name="result">The result of the RPC call</param>
	/// <param name="methodIdent">The name of the RPC method</param>
	[Sandbox.Rpc.Broadcast( NetFlags.Reliable | NetFlags.HostOnly )]
	internal static void OnRpcResponse( RpcMessage response, object? result, int methodIdent )
	{
		// Log.Info( $"RPC Response received for {methodName}: {result}" );
		RpcCallbackSystem.Current.CompleteResponse( response, result, methodIdent );
	}

	/// <summary>
	/// Internal callback for when the server sends an error RPC response
	/// </summary>
	/// <param name="response">The original RPC message</param>
	/// <param name="errorMessage">The error message from the server</param>
	/// <param name="methodIdent">The method identity of the RPC method</param>
	[Sandbox.Rpc.Broadcast( NetFlags.Reliable | NetFlags.HostOnly )]
	internal static void OnRpcError( RpcMessage response, RpcError errorMessage, int methodIdent )
	{
		Log.Warning( $"RPC Error received for {methodIdent}: {errorMessage}" );
		RpcCallbackSystem.Current.CompleteWithError( response, errorMessage, methodIdent );
	}
}
