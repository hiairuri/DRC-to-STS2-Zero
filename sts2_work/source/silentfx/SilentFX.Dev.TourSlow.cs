// namespace: SilentFX.Dev
// type: TourSlow (class)
// source lines: 29327-29415
	internal static class TourSlow
	{
		private static bool _on;

		private static bool _ticking;

		private static double _usual = 1.0;

		private static double _tail = -1.0;

		private static double _real;

		internal static bool On => _on;

		internal static void Begin(float scale)
		{
			if (scale < 0f)
			{
				scale = Tuning.F("tour.slow", 0.2f);
			}
			scale = Mathf.Clamp(scale, 0.05f, 1f);
			if (!(scale > 0.999f))
			{
				if (!_on)
				{
					_usual = Engine.TimeScale;
				}
				_on = true;
				_tail = -1.0;
				_real = 0.0;
				Engine.TimeScale = scale;
				if (!_ticking)
				{
					_ticking = true;
					Ticker.Add(Update);
				}
				DevLog.Info($"Tour: slow motion on ({scale:0.00}x) - a card was played");
			}
		}

		internal static void Release()
		{
			if (_on && _tail < 0.0)
			{
				_tail = Mathf.Clamp(Tuning.F("tour.slow_tail", 0.5f), 0f, 5f);
			}
		}

		internal static void End()
		{
			if (_on)
			{
				_on = false;
				_tail = -1.0;
				Engine.TimeScale = _usual;
				DevLog.Info("Tour: slow motion off");
			}
		}

		private static void Update(double realDt)
		{
			try
			{
				if (!_on)
				{
					return;
				}
				_real += realDt;
				if (_tail >= 0.0)
				{
					_tail -= realDt * Engine.TimeScale;
					if (_tail <= 0.0)
					{
						End();
					}
				}
				if (_on && (_real > 30.0 || !TourController.Active))
				{
					End();
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Tour: slow motion failed: " + ex);
				_on = false;
				Engine.TimeScale = _usual;
			}
		}
	}
