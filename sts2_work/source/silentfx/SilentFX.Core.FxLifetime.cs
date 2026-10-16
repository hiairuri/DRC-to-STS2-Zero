// namespace: SilentFX.Core
// type: FxLifetime (class)
// source lines: 36786-36924
	internal static class FxLifetime
	{
		private sealed class Lease
		{
			public Node Node;

			public Node Room;

			public double Remaining;

			public Action Cleanup;
		}

		private static readonly List<Lease> Live = new List<Lease>();

		private static bool _ticking;

		public static void Watch(Node node, double seconds, Action cleanup = null)
		{
			try
			{
				if (!FxKit.Alive((GodotObject)(object)node))
				{
					cleanup?.Invoke();
					return;
				}
				if (!_ticking)
				{
					Ticker.Add(Update);
					_ticking = true;
				}
				Live.Add(new Lease
				{
					Node = node,
					Room = (Node)(object)GameRefs.CombatRoom,
					Remaining = (double.IsFinite(seconds) ? Math.Max(0.0, seconds) : 0.0),
					Cleanup = cleanup
				});
			}
			catch (Exception ex)
			{
				DevLog.Warn("FxLifetime.Watch: " + ex);
			}
		}

		public static void Reset()
		{
			while (Live.Count > 0)
			{
				int index = Live.Count - 1;
				Lease lease = Live[index];
				Live.RemoveAt(index);
				try
				{
					lease.Cleanup?.Invoke();
				}
				catch (Exception ex)
				{
					DevLog.Warn("FxLifetime reset cleanup: " + ex);
				}
				try
				{
					if (FxKit.Alive((GodotObject)(object)lease.Node))
					{
						lease.Node.QueueFree();
					}
				}
				catch (Exception ex2)
				{
					DevLog.Warn("FxLifetime reset free: " + ex2);
				}
			}
			StopIfEmpty();
		}

		private static void StopIfEmpty()
		{
			if (Live.Count == 0)
			{
				Ticker.Remove(Update);
				_ticking = false;
			}
		}

		private static void Update(double realDt)
		{
			if (Live.Count == 0)
			{
				StopIfEmpty();
				return;
			}
			double num = realDt * (double)Mathf.Clamp((float)Engine.TimeScale, 0f, 4f);
			NCombatRoom combatRoom = GameRefs.CombatRoom;
			for (int num2 = Live.Count - 1; num2 >= 0; num2--)
			{
				Lease lease = Live[num2];
				try
				{
					if (lease.Room == null || (object)lease.Room == combatRoom)
					{
						if (FxKit.Alive((GodotObject)(object)lease.Node) && lease.Node.IsInsideTree() && TimeFreeze.HoldsVisual(lease.Node))
						{
							continue;
						}
						lease.Remaining -= num;
						if (lease.Remaining > 0.0 && FxKit.Alive((GodotObject)(object)lease.Node) && lease.Node.IsInsideTree())
						{
							continue;
						}
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("FxLifetime.Update: " + ex);
				}
				Live.RemoveAt(num2);
				try
				{
					lease.Cleanup?.Invoke();
				}
				catch (Exception ex2)
				{
					DevLog.Warn("FxLifetime cleanup: " + ex2);
				}
				try
				{
					if (FxKit.Alive((GodotObject)(object)lease.Node))
					{
						lease.Node.QueueFree();
					}
				}
				catch (Exception ex3)
				{
					DevLog.Warn("FxLifetime free: " + ex3);
				}
			}
			StopIfEmpty();
		}
	}
