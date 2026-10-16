// namespace: SilentFX.Core
// type: AssetWarmup (class)
// source lines: 31976-32146
	internal static class AssetWarmup
	{
		private sealed record Work(string Name, Func<bool> Step, Action Cancel, Func<bool> CanRun);

		private static readonly Queue<Work> Pending = new Queue<Work>();

		private static readonly HashSet<string> Names = new HashSet<string>(StringComparer.Ordinal);

		private static bool _ticking;

		private static bool _settingsHooked;

		private static double _quiet;

		private static int _completed;

		private static int _failed;

		private static double _cpuMs;

		private static double _worstMs;

		internal static bool Enabled => true;

		internal static void Add(string name, Action action)
		{
			Add(name, () =>
			{
				action();
				return true;
			});
		}

		internal static void Add(string name, Func<bool> step, Action cancel = null, Func<bool> canRun = null)
		{
			if (!_settingsHooked)
			{
				PlayerSettings.Changed += SettingsChanged;
				_settingsHooked = true;
			}
			if (Names.Add(name))
			{
				Pending.Enqueue(new Work(name, step, cancel, canRun));
				if (!_ticking)
				{
					_ticking = true;
					_quiet = 0.0;
					_completed = (_failed = 0);
					_cpuMs = (_worstMs = 0.0);
					Ticker.Add(Update);
				}
			}
		}

		private static void SettingsChanged()
		{
			if (!PlayerSettings.PrewarmEnabled || !PlayerSettings.ShaderWarmupEnabled)
			{
				ShaderWarmup.Pause();
			}
		}

		private static void Update(double dt)
		{
			if (!PlayerSettings.PrewarmEnabled)
			{
				ShaderWarmup.Pause();
				return;
			}
			if (GameRefs.CombatRoom != null || Ticker.Tree == null || Ticker.Tree.Paused)
			{
				_quiet = 0.0;
				ShaderWarmup.Pause();
				return;
			}
			_quiet += dt;
			if (_quiet < 0.5)
			{
				return;
			}
			if (Pending.Count == 0)
			{
				Stop();
				return;
			}
			Work work = null;
			int count = Pending.Count;
			while (count-- > 0)
			{
				Work work2 = Pending.Dequeue();
				bool flag;
				try
				{
					flag = work2.CanRun == null || work2.CanRun();
				}
				catch (Exception ex)
				{
					_failed++;
					Cancel(work2);
					DevLog.Warn("Prewarm availability check failed: " + work2.Name + " - " + ex.Message);
					Names.Remove(work2.Name);
					continue;
				}
				if (flag)
				{
					work = work2;
					break;
				}
				Pending.Enqueue(work2);
			}
			if (work == null)
			{
				if (Pending.Count == 0)
				{
					Stop();
				}
				return;
			}
			bool flag2 = true;
			long timestamp = Stopwatch.GetTimestamp();
			try
			{
				flag2 = work.Step();
			}
			catch (Exception ex2)
			{
				_failed++;
				Cancel(work);
				DevLog.Warn("Prewarm failed: " + work.Name + " - " + ex2.Message);
			}
			double totalMilliseconds = Stopwatch.GetElapsedTime(timestamp).TotalMilliseconds;
			_cpuMs += totalMilliseconds;
			_worstMs = Math.Max(_worstMs, totalMilliseconds);
			if (totalMilliseconds >= 8.0)
			{
				DevLog.Info($"Prewarm slow unit: {work.Name} {totalMilliseconds:F1} ms (outside combat)");
			}
			if (flag2)
			{
				Names.Remove(work.Name);
				_completed++;
			}
			else
			{
				Pending.Enqueue(work);
			}
			if (Pending.Count == 0)
			{
				Stop();
			}
		}

		private static void Stop()
		{
			Ticker.Remove(Update);
			_ticking = false;
			DevLog.Info($"Prewarm queue drained: {_completed} jobs, {_failed} failed, CPU {_cpuMs:F1} ms, slowest unit {_worstMs:F1} ms; GPU timing requires in-game profiling");
		}

		private static void Cancel(Work work)
		{
			try
			{
				work.Cancel?.Invoke();
			}
			catch (Exception ex)
			{
				DevLog.Warn("Prewarm cleanup: " + work.Name + " - " + ex.Message);
			}
		}
	}
