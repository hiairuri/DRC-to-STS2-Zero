// namespace: SilentFX.Patches
// type: TourHandLockPatch (class)
// source lines: 23558-23577
	public static class TourHandLockPatch
	{
		public static bool Prefix(ref bool __result)
		{
			try
			{
				if (!TourController.BlockingInput)
				{
					return true;
				}
				__result = false;
				return false;
			}
			catch (Exception ex)
			{
				DevLog.Warn("TourHandLockPatch: " + ex);
				return true;
			}
		}
	}
