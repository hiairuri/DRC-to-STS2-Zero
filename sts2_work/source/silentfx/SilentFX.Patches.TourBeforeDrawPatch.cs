// namespace: SilentFX.Patches
// type: TourBeforeDrawPatch (class)
// source lines: 23593-23614
	public static class TourBeforeDrawPatch
	{
		public static void Postfix(ICombatState combatState, Player player, ref Task __result)
		{
			try
			{
				if (TourController.Active)
				{
					CombatManager instance = CombatManager.Instance;
					if (instance != null && !instance.IsOverOrEnding && ((player != null) ? player.PlayerCombatState : null) != null)
					{
						__result = TourTurnPlan.BeforeDrawCompleted(__result, combatState, player);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("TourBeforeDrawPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(NCardFlyPowerVfx), "_Ready")]
