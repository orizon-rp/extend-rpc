// namespace Extend.Callbacks;
//
// public sealed class RpcMonitor : Component
// {
// 	private readonly Dictionary<string, RpcMethodStats> _methodStats = new();
//
// 	protected override void OnAwake()
// 	{
// 		base.OnAwake();
//
// 		RpcManager.OnOperationCompleted += OnOperationCompleted;
// 		RpcManager.OnOperationFailed += OnOperationFailed;
// 		RpcManager.OnOperationTimeout += OnOperationTimeout;
// 	}
//
// 	private void OnOperationCompleted( RpcMessage message, TimeSpan duration )
// 	{
// 		UpdateMethodStats( message.Method, true, duration );
// 	}
//
// 	private void OnOperationFailed( RpcMessage message, RpcError exception )
// 	{
// 		UpdateMethodStats( message.Method, false, TimeSpan.Zero );
// 	}
//
// 	private void OnOperationTimeout( RpcMessage message )
// 	{
// 		UpdateMethodStats( message.Method, false, TimeSpan.Zero, true );
// 	}
//
// 	private void UpdateMethodStats( string method, bool success, TimeSpan duration, bool timeout = false )
// 	{
// 		if ( !_methodStats.ContainsKey( method ) )
// 			_methodStats[method] = new RpcMethodStats { MethodName = method };
//
// 		var stats = _methodStats[method];
// 		stats.TotalCalls++;
//
// 		if ( success )
// 		{
// 			stats.SuccessfulCalls++;
// 			stats.TotalDuration += duration;
// 			stats.AverageDuration = TimeSpan.FromTicks( stats.TotalDuration.Ticks / stats.SuccessfulCalls );
//
// 			if ( duration > stats.MaxDuration ) stats.MaxDuration = duration;
// 			if ( stats.MinDuration == TimeSpan.Zero || duration < stats.MinDuration ) stats.MinDuration = duration;
// 		}
// 		else if ( timeout )
// 		{
// 			stats.TimeoutCalls++;
// 		}
// 		else
// 		{
// 			stats.FailedCalls++;
// 		}
//
// 		stats.SuccessRate = (float)stats.SuccessfulCalls / stats.TotalCalls;
// 	}
//
// 	[ConCmd( "rpc_detailed_stats" )]
// 	private static void ShowDetailedStats()
// 	{
// 		var monitor = Game.ActiveScene.Components.Get<RpcMonitor>();
//
// 		if ( monitor is null )
// 		{
// 			Log.Warning( "RpcMonitor component not found" );
// 			return;
// 		}
//
// 		Log.Info( "=== Detailed RPC Statistics ===" );
//
// 		foreach ( var stats in monitor._methodStats.Values.OrderByDescending( s => s.TotalCalls ) )
// 		{
// 			Log.Info( $"{stats.MethodName}:" );
// 			Log.Info(
// 				$"  Total: {stats.TotalCalls}, Success: {stats.SuccessfulCalls}, Failed: {stats.FailedCalls}, Timeout: {stats.TimeoutCalls}" );
// 			Log.Info( $"  Success Rate: {stats.SuccessRate:P2}" );
//
// 			if ( stats.SuccessfulCalls > 0 )
// 			{
// 				Log.Info(
// 					$"  Duration - Avg: {stats.AverageDuration.TotalMilliseconds:F1}ms, Min: {stats.MinDuration.TotalMilliseconds:F1}ms, Max: {stats.MaxDuration.TotalMilliseconds:F1}ms" );
// 			}
// 		}
// 	}
// }
