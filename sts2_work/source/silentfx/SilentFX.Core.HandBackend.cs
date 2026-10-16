// namespace: SilentFX.Core
// type: HandBackend (class)
// source lines: 33934-34005
	public static class HandBackend
	{
		private static bool _registered;

		public static void Init()
		{
			try
			{
				if (!_registered)
				{
					CardRide.Init();
					CardDeco.Init();
					HandHold.Init();
					Ticker.Add(UpdateDraws);
					_registered = true;
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("HandBackend.Init: " + ex);
			}
		}

		private static void UpdateDraws(double realDt)
		{
			try
			{
				HandFlow.UpdateRecentDraws();
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandBackend (draws): " + ex);
			}
		}

		public static void Reset()
		{
			try
			{
				CardRide.Reset();
			}
			catch (Exception ex)
			{
				DevLog.Error("HandBackend.Reset (rides): " + ex);
			}
			try
			{
				CardDeco.Reset();
			}
			catch (Exception ex2)
			{
				DevLog.Error("HandBackend.Reset (decos): " + ex2);
			}
			try
			{
				HandHold.Reset();
			}
			catch (Exception ex3)
			{
				DevLog.Error("HandBackend.Reset (hold): " + ex3);
			}
			try
			{
				CardStandIn.Reset();
			}
			catch (Exception ex4)
			{
				DevLog.Error("HandBackend.Reset (stand-ins): " + ex4);
			}
			HandFlow.ResetDrawState();
		}
	}
