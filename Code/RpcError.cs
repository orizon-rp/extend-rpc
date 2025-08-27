namespace Extend.Callbacks;

/// <summary>
/// Represents an error that occurred during a remote procedure call (RPC).
/// </summary>
public abstract record RpcError
{
	/// <summary>
	/// Represents an exception that occurred during a remote procedure call.
	/// </summary>
	public record Exception : RpcError
	{
		/// <summary>
		/// The error message.
		/// </summary>
		public required string Message { get; init; }
	}

	/// <summary>
	/// Indicates that the RPC operation timed out.
	/// </summary>
	public record Timeout : Exception;

	/// <summary>
	/// Indicates that the RPC handler was not found.
	/// </summary>
	public record HandlerNotFound : Exception;

	/// <summary>
	/// Indicates that the arguments passed to the RPC method were invalid.
	/// </summary>
	public record InvalidArguments : Exception;
}
