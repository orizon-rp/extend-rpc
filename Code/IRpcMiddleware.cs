using System.Threading.Tasks;

namespace Extend.Callbacks;

/// <summary>
/// Interface representing an RPC middleware.
/// </summary>
public interface IRpcMiddleware
{
	/// <summary>
	/// Called before executing an RPC method.
	/// </summary>
	/// <param name="message">The RPC message.</param>
	/// <param name="args">The arguments passed to the RPC method.</param>
	/// <param name="sender">The connection that sent the RPC message.</param>
	/// <returns>A task that will complete with a boolean indicating whether the RPC method should be executed.</returns>
	Task<bool> OnBeforeExecute( RpcMessage message, object[] args, Connection sender );

	/// <summary>
	/// Called after executing an RPC method.
	/// </summary>
	/// <param name="message">The RPC message.</param>
	/// <param name="result">The result returned by the RPC method.</param>
	/// <param name="exception">The exception thrown by the RPC method, if any.</param>
	/// <param name="duration">The duration of the RPC method execution.</param>
	/// <returns>A task that will complete when the middleware is done processing.</returns>
	Task OnAfterExecute( RpcMessage message, object result, Exception exception, TimeSpan duration );
}
