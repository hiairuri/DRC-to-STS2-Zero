// namespace: SilentFX.Patches
// type: TourEndTurnLockPatch (class)
// source lines: 23578-23592
	public static class TourEndTurnLockPatch
	{
		public static bool Prefix()
		{
			try
			{
				return !TourController.BlockingInput;
			}
			catch (Exception ex)
			{
				DevLog.Warn("TourEndTurnLockPatch: " + ex);
				return true;
			}
		}
	}
