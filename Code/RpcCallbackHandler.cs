using System.Threading.Tasks;

namespace Extend.Callbacks;

public static class RpcCallbackHandler
{
	public static async Task<T?> OnRpc<T>( WrappedMethod<Task<T>> m, params object[] args )
	{
		if ( Networking.IsHost )
			return await m.Resume();

		var attribute = m.GetAttribute<RpcCallbackAttribute>();
		var timeout = TimeSpan.FromSeconds( attribute.Timeout );
		
		return await RpcClient.Send<T>( m.MethodIdentity, typeof(T), timeout, args );
	}
}
