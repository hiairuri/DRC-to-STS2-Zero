// namespace: SilentFX.Core
// type: Ticker (class)
// source lines: 45214-45364
	public static class Ticker
	{
		private static readonly List<Action<double>> Subs = new List<Action<double>>();

		private static readonly List<Action<double>> LateSubs = new List<Action<double>>();

		private static readonly HashSet<Action<double>> Members = new HashSet<Action<double>>();

		private static readonly HashSet<Action<double>> LateMembers = new HashSet<Action<double>>();

		private static ulong _lastUsec;

		private static double _lateDt;

		private static bool _latePending;

		private static bool _initialized;

		public static SceneTree Tree { get; private set; }

		public static bool Init()
		{
			if (_initialized)
			{
				return true;
			}
			MainLoop mainLoop = Engine.GetMainLoop();
			Tree = (SceneTree)(object)((mainLoop is SceneTree) ? mainLoop : null);
			if (Tree == null)
			{
				DevLog.Error("Ticker: main loop is not a SceneTree");
				return false;
			}
			_lastUsec = Time.GetTicksUsec();
			Tree.ProcessFrame += OnFrame;
			RenderingServer.FramePreDraw += OnLateFrame;
			_initialized = true;
			return true;
		}

		public static void Add(Action<double> cb)
		{
			if (LateMembers.Remove(cb))
			{
				LateSubs.Remove(cb);
			}
			if (Members.Add(cb))
			{
				Subs.Add(cb);
			}
		}

		public static void AddLate(Action<double> cb)
		{
			if (Members.Remove(cb))
			{
				Subs.Remove(cb);
			}
			if (LateMembers.Add(cb))
			{
				LateSubs.Add(cb);
			}
		}

		public static void Remove(Action<double> cb)
		{
			if (Members.Remove(cb))
			{
				Subs.Remove(cb);
			}
			if (LateMembers.Remove(cb))
			{
				LateSubs.Remove(cb);
			}
		}

		private static void OnFrame()
		{
			RemoteFxFade.Restore();
			ulong ticksUsec = Time.GetTicksUsec();
			double num = (double)(ticksUsec - _lastUsec) / 1000000.0;
			_lastUsec = ticksUsec;
			FrameDiagnostics.Frame(num);
			double timeScale = Engine.TimeScale;
			double num2 = ((timeScale > 0.001 && double.IsFinite(timeScale)) ? (((Node)Tree.Root).GetProcessDeltaTime() / timeScale) : num);
			if (!double.IsFinite(num2) || num2 < 0.0)
			{
				num2 = num;
			}
			num2 = Math.Clamp(num2, 0.0, 0.25);
			_lateDt = Math.Min(_lateDt + num2, 0.25);
			_latePending = true;
			Dispatch(Subs, Members, num2);
		}

		private static void OnLateFrame()
		{
			if (!_latePending)
			{
				RemoteFxFade.BeforeDraw();
				return;
			}
			double lateDt = _lateDt;
			_lateDt = 0.0;
			_latePending = false;
			Dispatch(LateSubs, LateMembers, lateDt);
			RemoteFxFade.BeforeDraw();
		}

		private static void Dispatch(List<Action<double>> subs, HashSet<Action<double>> members, double dt)
		{
			int count = subs.Count;
			if (count == 0)
			{
				return;
			}
			Action<double>[] array = ArrayPool<Action<double>>.Shared.Rent(count);
			try
			{
				subs.CopyTo(array);
				for (int num = count - 1; num >= 0; num--)
				{
					Action<double> action = array[num];
					if (members.Contains(action))
					{
						long num2 = (FrameDiagnostics.Enabled ? Stopwatch.GetTimestamp() : 0);
						try
						{
							action(dt);
						}
						catch (Exception ex)
						{
							DevLog.Error("Ticker callback failed and was removed: " + ex);
							Remove(action);
						}
						finally
						{
							if (num2 != 0L)
							{
								FrameDiagnostics.Callback(action, num2);
							}
						}
					}
				}
			}
			finally
			{
				ArrayPool<Action<double>>.Shared.Return(array, clearArray: true);
			}
		}
	}
