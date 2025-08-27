namespace Extend.Callbacks;

/// <summary>
/// Utility class for working with remote procedure call (RPC) related functionality.
/// </summary>
public static class RpcUtility
{
	/// <summary>
	/// Enumerates all the RPC callback methods in all the components of the active scene.
	/// </summary>
	public static IEnumerable<(Component component, MethodDescription Method, List<Attribute> attribute)>
		GetRpcCallbackMethods()
	{
		var components = Game.ActiveScene.Components.GetAll( FindMode.EverythingInSelfAndChildren );

		foreach ( var component in components )
		{
			var type = Game.TypeLibrary.GetType( component.GetType() );

			foreach ( var method in type.Methods )
			{
				var attribute = method.GetCustomAttribute<RpcCallbackAttribute>();
				if ( attribute is null ) continue;

				yield return (component, method, method.Attributes.ToList());
			}
		}
	}
}
