// namespace: SilentFX.Core
// type: HandPace (class)
// source lines: 39508-39664
	public static class HandPace
	{
		public enum Mode
		{
			Off,
			Fast,
			Slow
		}

		private const string RitsuAssembly = "STS2-RitsuLib";

		private const double ProbeSeconds = 6.0;

		private const string FoundText = "HandPace: RitsuLib settings are available at Settings > Mod设置（RitsuLib）> SilentFX";

		private static bool _inited;

		private static double _probeTime;

		private static double _probeGap;

		private static readonly Action<double> ProbeTick = Probe;

		public static Mode Current
		{
			get
			{
				if (!PlayerSettings.Enabled)
				{
					return Mode.Off;
				}
				return Preference;
			}
		}

		internal static Mode Preference => FromNumber(PlayerSettings.GetHandPacePreference());

		public static bool On => Current != Mode.Off;

		public static float Scale
		{
			get
			{
				if (Current != Mode.Fast)
				{
					return 1f;
				}
				float num = Tuning.F("hand.fast_scale", 0.6f);
				if (!float.IsFinite(num))
				{
					return 0.6f;
				}
				return Math.Clamp(num, 0.1f, 1f);
			}
		}

		public static void Set(Mode mode)
		{
			try
			{
				mode = FromNumber((int)mode);
				if (Preference != mode)
				{
					PlayerSettings.SetHandPacePreference((int)mode);
					DevLog.Info("HandPace: " + Name(mode) + " (chosen by the player)");
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandPace.Set failed: " + ex.Message);
			}
		}

		public static void Init()
		{
			if (_inited)
			{
				return;
			}
			_inited = true;
			try
			{
				DevLog.Info("HandPace: preference " + Name(Preference) + "; effective " + Name(Current));
				if (RitsuLibLoaded())
				{
					DevLog.Info("HandPace: RitsuLib settings are available at Settings > Mod设置（RitsuLib）> SilentFX");
				}
				else
				{
					Ticker.Add(ProbeTick);
				}
			}
			catch (Exception ex)
			{
				DevLog.Info("HandPace: settings page unavailable (" + ex.Message + ")");
			}
		}

		internal static Mode FromNumber(int value)
		{
			if (value > 0)
			{
				if (value != 1)
				{
					return Mode.Slow;
				}
				return Mode.Fast;
			}
			return Mode.Off;
		}

		internal static string Name(Mode mode)
		{
			return mode switch
			{
				Mode.Fast => "fast", 
				Mode.Off => "off", 
				_ => "slow", 
			};
		}

		private static void Probe(double realDt)
		{
			_probeTime += realDt;
			_probeGap += realDt;
			bool flag = _probeTime >= 6.0;
			if (!(_probeGap < 0.5) || flag)
			{
				_probeGap = 0.0;
				bool flag2 = RitsuLibLoaded();
				if (flag2 || flag)
				{
					Ticker.Remove(ProbeTick);
					DevLog.Info(flag2 ? "HandPace: RitsuLib settings are available at Settings > Mod设置（RitsuLib）> SilentFX" : "HandPace: RitsuLib is not loaded; settings stay available in user_settings.ini");
				}
			}
		}

		private static bool RitsuLibLoaded()
		{
			try
			{
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				for (int i = 0; i < assemblies.Length; i++)
				{
					if (string.Equals(assemblies[i].GetName().Name, "STS2-RitsuLib", StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
			}
			catch
			{
			}
			return false;
		}
	}
