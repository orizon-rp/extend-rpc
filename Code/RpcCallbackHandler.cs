using System.Threading;
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
		var cancellationToken = CancellationToken.None;

		if ( m.Object is Component component )
			cancellationToken = component.GameObject.EnabledToken;
		
		return await RpcClient.Send<T>( m.MethodIdentity, typeof(T), timeout, cancellationToken, args );
	}
}
