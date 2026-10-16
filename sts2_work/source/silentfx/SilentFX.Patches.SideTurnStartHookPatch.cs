// namespace: SilentFX.Patches
// type: SideTurnStartHookPatch (class)
// source lines: 22270-22290
	public static class SideTurnStartHookPatch
	{
		public static void Prefix(CombatSide side)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Invalid comparison between Unknown and I4
			try
			{
				TimeFreeze.Thaw();
				if ((int)side == 1)
				{
					ShivFx.OnPlayerTurnStart();
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("SideTurnStartHookPatch: " + ex);
			}
		}
	}
	[HarmonyPatch]
