// namespace: SilentFX.Core
// type: FxClock (class)
// source lines: 35688-35714
	internal static class FxClock
	{
		private static double _milliseconds;

		public static ulong Milliseconds => (ulong)_milliseconds;

		static FxClock()
		{
			Ticker.Add(Update);
		}

		private static void Update(double realDt)
		{
			try
			{
				double num = realDt * (double)Mathf.Clamp((float)Engine.TimeScale, 0f, 4f);
				if (double.IsFinite(num) && num > 0.0)
				{
					_milliseconds += num * 1000.0;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("FxClock: " + ex.Message);
			}
		}
	}
