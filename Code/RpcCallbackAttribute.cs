namespace Extend.Callbacks;

/// <summary>
/// Attribute to decorate methods that are called when a remote procedure call (RPC) is received.
/// </summary>
/// <param name="timeout">The maximum time to wait for the response. Defaults to <see cref="RpcCallbackSystem.DefaultTimeout"/></param>
[AttributeUsage( AttributeTargets.Method )]
[CodeGenerator( CodeGeneratorFlags.WrapMethod | CodeGeneratorFlags.Instance | CodeGeneratorFlags.Static,
	"Extend.Rpc.RpcCallbackHandler.OnRpc" )]
public sealed class RpcCallbackAttribute( int timeout = 5 ) : Attribute
{
	public int Timeout => timeout;
}
