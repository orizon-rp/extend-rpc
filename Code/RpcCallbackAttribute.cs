namespace Extend.Callbacks;

/// <summary>
/// Attribute to decorate methods that are called when a remote procedure call (RPC) is received.
/// </summary>
/// <param name="timeout">The maximum time to wait for the response./></param>
[AttributeUsage( AttributeTargets.Method )]
[CodeGenerator( CodeGeneratorFlags.WrapMethod | CodeGeneratorFlags.Instance | CodeGeneratorFlags.Static,
	"Extend.Callbacks.RpcCallbackHandler.OnRpc" )]
public sealed class RpcCallbackAttribute( int timeout = 5 ) : Attribute
{
	public int Timeout => timeout;
}
