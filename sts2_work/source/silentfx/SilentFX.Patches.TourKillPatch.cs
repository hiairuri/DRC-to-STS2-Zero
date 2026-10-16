// namespace: SilentFX.Patches
// type: TourKillPatch (class)
// source lines: 23510-23534
	public static class TourKillPatch
	{
		public static bool Prefix(Creature creature, ref Task __result)
		{
			try
			{
				if (!TourController.Active || !TourController.Protect(creature))
				{
					return true;
				}
				if (creature.CurrentHp < 1)
				{
					creature.SetCurrentHpInternal(1m);
				}
				DevLog.Info("Tour: direct kill prevented for player/last enemy");
				__result = Task.CompletedTask;
				return false;
			}
			catch (Exception ex)
			{
				DevLog.Warn("TourKillPatch: " + ex);
				return true;
			}
		}
	}
