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
	/// <param name="methodName">The name of the RPC method</param>
	/// <param name="methodReturnType">The type of the method return type</param>
	/// <param name="timeout">The maximum time to wait in seconds for the response./></param>
	/// <param name="args">Arguments for the RPC method</param>
	/// <returns>The result of the RPC call</returns>
	/// <exception cref="ArgumentException">If <paramref name="methodName"/> is null or empty</exception>
	/// <exception cref="InvalidOperationException">If the maximum concurrent RPC operations limit is reached</exception>
	/// <exception cref="Exception">If the RPC call fails</exception>
	public static async Task<TResult?> Send<TResult>( string methodName, Type methodReturnType, TimeSpan timeout,
		params object[] args )
	{
		if ( string.IsNullOrEmpty( methodName ) )
			throw new ArgumentException( "Method name cannot be null or empty", nameof(methodName) );

		var request = RpcMessage.Create( methodName, methodReturnType.FullName ?? methodReturnType.Name,
			Connection.Local.Id );

		try
		{
			RpcServer.SendRpc( request, args );
			return await RpcCallbackSystem.Current.WaitForResponse<TResult>( request.Id, timeout );
		}
		catch ( Exception e )
		{
			Log.Warning( $"RPC call failed for {methodName}: {e.Message}" );
			return default!;
		}
	}

	/// <summary>
	/// Internal callback for when the server sends a successful RPC response
	/// </summary>
	/// <param name="response">The original RPC message</param>
	/// <param name="result">The result of the RPC call</param>
	/// <param name="methodName">The name of the RPC method</param>
	[Sandbox.Rpc.Broadcast( NetFlags.Reliable | NetFlags.HostOnly )]
	internal static void OnRpcResponse( RpcMessage response, object? result, string methodName )
	{
		// Log.Info( $"RPC Response received for {methodName}: {result}" );
		RpcCallbackSystem.Current.CompleteResponse( response, result, methodName );
	}

	/// <summary>
	/// Internal callback for when the server sends an error RPC response
	/// </summary>
	/// <param name="response">The original RPC message</param>
	/// <param name="errorMessage">The error message from the server</param>
	/// <param name="methodName">The name of the RPC method</param>
	[Sandbox.Rpc.Broadcast( NetFlags.Reliable | NetFlags.HostOnly )]
	internal static void OnRpcError( RpcMessage response, RpcError errorMessage, string methodName )
	{
		Log.Warning( $"RPC Error received for {methodName}: {errorMessage}" );
		RpcCallbackSystem.Current.CompleteWithError( response, errorMessage, methodName );
	}
}
