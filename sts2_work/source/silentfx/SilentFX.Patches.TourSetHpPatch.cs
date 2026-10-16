// namespace: SilentFX.Patches
// type: TourSetHpPatch (class)
// source lines: 23492-23509
	public static class TourSetHpPatch
	{
		public static void Prefix(Creature __instance, ref decimal amount)
		{
			try
			{
				if (TourController.Active && amount < 1m && TourController.Protect(__instance))
				{
					DevLog.Info("Tour: direct HP change capped at 1");
					amount = 1m;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("TourSetHpPatch: " + ex);
			}
		}
	}
