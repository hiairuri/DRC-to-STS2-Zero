// namespace: SilentFX.Patches
// type: TourDamagePatch (class)
// source lines: 23470-23491
	public static class TourDamagePatch
	{
		public static void Prefix(Creature __instance, ref decimal amount)
		{
			try
			{
				if (TourController.Active && TourController.Protect(__instance))
				{
					decimal num = Math.Max(0, __instance.CurrentHp - 1);
					if (amount > num)
					{
						DevLog.Info($"Tour: lethal damage capped {amount} -> {num}");
						amount = num;
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("TourDamagePatch: " + ex);
			}
		}
	}
