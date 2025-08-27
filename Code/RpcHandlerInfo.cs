namespace Extend.Callbacks;

/// <summary>
/// Represents metadata about an RPC handler.
/// </summary>
public sealed class RpcHandlerInfo
{
    /// <summary>
    /// The instance of the object that contains the RPC method.
    /// </summary>
    public required object Instance { get; init; }

    /// <summary>
    /// Information about the method that is the RPC handler.
    /// </summary>
    public required MethodDescription Method { get; init; }

    /// <summary>
    /// Attributes applied to the RPC method.
    /// </summary>
    public required List<Attribute> Attributes { get; init; }

    /// <summary>
    /// The date and time the handler was registered.
    /// </summary>
    public required DateTime RegisteredAt { get; init; }
}
