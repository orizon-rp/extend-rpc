using System.Threading.Tasks;

namespace Extend.Callbacks;

internal static class RpcCallbackHandler
{
	public static async Task<T?> OnRpc<T>( WrappedMethod<Task<T>> m, params object[] args )
	{
		if ( Sandbox.Networking.IsHost )
			return await m.Resume();

		var attribute = m.GetAttribute<RpcCallbackAttribute>();
		var timeout = TimeSpan.FromSeconds( attribute.Timeout );
		
		return await RpcClient.Send<T>( m.MethodName, typeof(T), timeout, args );
	}
}
