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
	public int MethodIdent { get; init; }

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
	public int MethodReturnTypeIdent { get; private init; }

	/// <summary>
	/// The arguments of the method to be invoked on the server.
	/// </summary>
	public int[] GenericArguments { get; private init; }

	/// <summary>
	/// Gets the connection associated with the sender of the message.
	/// </summary>
	[JsonIgnore]
	public Connection Connection => Connection.Find( Sender );

	/// <summary>
	/// Creates a new instance of <see cref="RpcMessage"/> with the given method name and sender identifier.
	/// </summary>
	/// <param name="methodIdent">The method identity of the method to be invoked on the server.</param>
	/// <param name="methodReturnType">The type of the method to be invoked on the server.</param>
	/// <param name="sender">The identifier of the sender (client) of the message.</param>
	/// <returns>A new instance of <see cref="RpcMessage"/>.</returns>
	public static RpcMessage Create( int methodIdent, Type methodReturnType, Guid sender )
	{
		var methodReturnTypeIdent = TypeLibrary.GetTypeIdent( methodReturnType );
		
		return new RpcMessage
		{
			Id = Guid.NewGuid(),
			MethodIdent = methodIdent,
			MethodReturnTypeIdent = methodReturnTypeIdent,
			GenericArguments = [],
			Timestamp = DateTime.UtcNow,
			Sender = sender
		};
	}
}
