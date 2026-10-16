// namespace: SilentFX.Core
// type: FrameDiagnostics (class)
// source lines: 35578-35687
	internal static class FrameDiagnostics
	{
		private sealed class Sample
		{
			internal string Name;

			internal int Calls;

			internal double Total;

			internal double Peak;
		}

		private static readonly Dictionary<MethodInfo, Sample> Samples = new Dictionary<MethodInfo, Sample>();

		private static double _elapsed;

		private static double _wallPeak;

		private static int _frames;

		private static int _over33;

		private static int _over100;

		private static int _gc0;

		private static int _gc1;

		private static int _gc2;

		private static long _allocatedStart;

		internal static bool Enabled { get; private set; }

		internal static void Frame(double wallDt)
		{
			int num;
			if (Tuning.I("perf.trace", 0) != 0 && GameRefs.CombatRoom != null)
			{
				SceneTree tree = Ticker.Tree;
				num = ((tree == null || !tree.Paused) ? 1 : 0);
			}
			else
			{
				num = 0;
			}
			bool flag = (byte)num != 0;
			if (flag != Enabled)
			{
				Enabled = flag;
				Reset();
			}
			if (!flag)
			{
				return;
			}
			_elapsed += wallDt;
			_frames++;
			_wallPeak = Math.Max(_wallPeak, wallDt * 1000.0);
			if (wallDt > 1.0 / 30.0)
			{
				_over33++;
			}
			if (wallDt > 0.1)
			{
				_over100++;
			}
			if (!(_elapsed < 5.0))
			{
				List<Sample> list = new List<Sample>(Samples.Values);
				list.Sort((Sample a, Sample b) => b.Total.CompareTo(a.Total));
				DevLog.Info($"Perf (CPU only): {_frames} frames/{_elapsed:F1}s, wall peak {_wallPeak:F1} ms, >33ms={_over33}, >100ms={_over100}, main-thread allocation={(double)(GC.GetAllocatedBytesForCurrentThread() - _allocatedStart) / 1048576.0:F2} MiB, GC={GC.CollectionCount(0) - _gc0}/{GC.CollectionCount(1) - _gc1}/{GC.CollectionCount(2) - _gc2}");
				for (int num2 = 0; num2 < Math.Min(5, list.Count); num2++)
				{
					Sample sample = list[num2];
					DevLog.Info($"Perf callback: {sample.Name}, calls={sample.Calls}, CPU total={sample.Total:F2} ms, peak={sample.Peak:F2} ms");
				}
				Reset();
			}
		}

		internal static void Callback(Action<double> callback, long start)
		{
			double totalMilliseconds = Stopwatch.GetElapsedTime(start).TotalMilliseconds;
			MethodInfo method = callback.Method;
			if (!Samples.TryGetValue(method, out var value))
			{
				value = new Sample
				{
					Name = method.DeclaringType?.FullName + "." + method.Name
				};
				Samples.Add(method, value);
			}
			value.Calls++;
			value.Total += totalMilliseconds;
			value.Peak = Math.Max(value.Peak, totalMilliseconds);
		}

		private static void Reset()
		{
			Samples.Clear();
			_elapsed = (_wallPeak = 0.0);
			_frames = (_over33 = (_over100 = 0));
			_allocatedStart = GC.GetAllocatedBytesForCurrentThread();
			_gc0 = GC.CollectionCount(0);
			_gc1 = GC.CollectionCount(1);
			_gc2 = GC.CollectionCount(2);
		}
	}
