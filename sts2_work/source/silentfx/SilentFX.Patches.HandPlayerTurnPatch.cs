// namespace: SilentFX.Patches
// type: HandPlayerTurnPatch (class)
// source lines: 20981-21013
	public static class HandPlayerTurnPatch
	{
		public static void Prefix(ICombatState combatState, CombatSide side)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Invalid comparison between Unknown and I4
			try
			{
				if ((int)side == 1)
				{
					Player player = null;
					try
					{
						player = LocalContext.GetMe(combatState);
					}
					catch (Exception ex)
					{
						DevLog.Warn("HandPlayerTurnPatch (local player): " + ex);
					}
					HandFlow.RaisePlayerTurnBegins(player);
				}
			}
			catch (Exception ex2)
			{
				DevLog.Error("HandPlayerTurnPatch: " + ex2);
			}
		}
	}
	[HarmonyPatch(typeof(Hook), "BeforeFlush", new Type[]
	{
		typeof(ICombatState),
		typeof(Player)
	})]
