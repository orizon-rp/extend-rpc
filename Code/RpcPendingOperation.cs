namespace Extend.Callbacks;

/// <summary>
/// Represents a pending RPC operation.
/// </summary>
public sealed class RpcPendingOperation
{
	/// <summary>
	/// Gets the name of the RPC method.
	/// </summary>
	public required int MethodIdent { get; init; }
	
	/// <summary>
	/// Gets the result of the RPC operation.
	/// </summary>
	public object? Result { get; init; }

	/// <summary>
	/// Gets the exception thrown by the RPC operation, if any.
	/// </summary>
	public RpcError? Exception { get; init; }

	/// <summary>
	/// Gets the date and time at which the RPC operation completed.
	/// </summary>
	public required DateTime? CompletedAt { get; init; }
}
