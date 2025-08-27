using System.Text.Json.Serialization;

namespace Extend.Callbacks;

/// <summary>
/// Represents a message exchanged between a client and a server during an RPC (Remote Procedure Call).
/// </summary>
public readonly struct RpcMessage
{
	/// <summary>
	/// The unique identifier of the message.
	/// </summary>
	public Guid Id { get; init; }

	/// <summary>
	/// The name of the method to be invoked on the server.
	/// </summary>
	public string Method { get; init; }

	/// <summary>
	/// The timestamp when the message was created.
	/// </summary>
	public DateTime Timestamp { get; init; }

	/// <summary>
	/// The identifier of the sender (client) of the message.
	/// </summary>
	public Guid Sender { get; private init; }
	
	/// <summary>
	/// The name of the return type of the method to be invoked on the server.
	/// </summary>
	public string MethodReturnTypeName { get; private init; }

	/// <summary>
	/// Gets the connection associated with the sender of the message.
	/// </summary>
	[JsonIgnore]
	public Connection Connection => Connection.Find( Sender );

	/// <summary>
	/// Creates a new instance of <see cref="RpcMessage"/> with the given method name and sender identifier.
	/// </summary>
	/// <param name="method">The name of the method to be invoked on the server.</param>
	/// <param name="methodReturnTypeName">The name of the method to be invoked on the server.</param>
	/// <param name="sender">The identifier of the sender (client) of the message.</param>
	/// <returns>A new instance of <see cref="RpcMessage"/>.</returns>
	public static RpcMessage Create( string method, string methodReturnTypeName, Guid sender ) => new()
	{
		Id = Guid.NewGuid(), Method = method, MethodReturnTypeName = methodReturnTypeName, Timestamp = DateTime.UtcNow, Sender = sender
	};
}
