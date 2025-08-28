namespace Extend.Callbacks;

public enum RpcErrorCode
{
	Unknown = 0,
	Timeout = 1,
	CallbackNotFound = 2
}

/// <summary>
/// Represents an error that occurred during an RPC (Remote Procedure Call) operation.
/// </summary>
public readonly struct RpcError
{
	public RpcErrorCode Code { get; init; }
	public string Message { get; init; }

	public RpcError()
	{
	}

	public RpcError( RpcErrorCode code, string message )
	{
		Code = code;
		Message = message;
	}

	public static RpcError Timeout( Guid id, int timeout ) => new(RpcErrorCode.Timeout, $"Timeout for operation {id} after {timeout} seconds");
	public static RpcError Unknown(int methodIdent, Exception ex) => new(RpcErrorCode.Unknown, $"Unknown error for method {methodIdent}: {ex}");

	public static RpcError CallbackNotFound( int methodIdent ) => new(RpcErrorCode.CallbackNotFound,
		$"Callback for method {methodIdent} not found");

	public override string ToString() => $"{Code}: {Message}";
}
