// namespace: SilentFX.Core
// type: WraithAudioWarmup (class)
// source lines: 45557-45693
	internal static class WraithAudioWarmup
	{
		private const string JobName = "Wraith neutral music DSP";

		private static bool _initialized;

		private static bool _queued;

		private static bool _watching;

		internal static void Init()
		{
			if (!_initialized)
			{
				_initialized = true;
				PlayerSettings.Changed += SettingsChanged;
			}
			Schedule();
		}

		internal static void ScheduleAfterMoodReset()
		{
			Schedule();
		}

		private static void SettingsChanged()
		{
			if (!SettingsAllowWarmup())
			{
				FmodNative.CancelWarmupWait();
			}
			else
			{
				Schedule();
			}
		}

		private static void Schedule()
		{
			if (!_initialized)
			{
				_initialized = true;
				PlayerSettings.Changed += SettingsChanged;
			}
			if (!_queued && !FmodNative.Dead)
			{
				_queued = true;
				AssetWarmup.Add("Wraith neutral music DSP", Step, Cancel, CanRun);
				if (!_watching)
				{
					_watching = true;
					Ticker.Add(Guard);
				}
			}
		}

		private static bool SettingsAllowWarmup()
		{
			if (PlayerSettings.Enabled && PlayerSettings.PrewarmEnabled)
			{
				return PlayerSettings.MusicEnabled;
			}
			return false;
		}

		private static bool CanRun()
		{
			if (SettingsAllowWarmup() && GameRefs.CombatRoom == null && Ticker.Tree != null)
			{
				return !Ticker.Tree.Paused;
			}
			return false;
		}

		private static void Guard(double dt)
		{
			if (!_queued)
			{
				StopGuard();
			}
			else if (!CanRun())
			{
				FmodNative.CancelWarmupWait();
			}
		}

		private static bool Step()
		{
			if (!CanRun())
			{
				FmodNative.CancelWarmupWait();
				return false;
			}
			if (FmodNative.Attached)
			{
				Complete("neutral chain already attached");
				return true;
			}
			switch (FmodNative.WarmNeutral())
			{
			case FmodNative.WarmResult.Pending:
				return false;
			case FmodNative.WarmResult.Unavailable:
				Complete("unavailable; ordinary Mood cold-attach fallback remains enabled");
				return true;
			default:
				Complete("neutral DSP chain attached and kept bypassed");
				return true;
			}
		}

		private static void Complete(string message)
		{
			_queued = false;
			StopGuard();
			DevLog.Info("Wraith audio prewarm: " + message);
		}

		private static void Cancel()
		{
			FmodNative.CancelWarmupWait();
			_queued = false;
			StopGuard();
		}

		private static void StopGuard()
		{
			if (_watching)
			{
				Ticker.Remove(Guard);
				_watching = false;
			}
		}
	}
}
namespace SilentFX.Cards
{
