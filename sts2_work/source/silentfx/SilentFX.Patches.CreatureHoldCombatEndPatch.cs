// namespace: SilentFX.Patches
// type: CreatureHoldCombatEndPatch (class)
// source lines: 22362-22393
	internal static class CreatureHoldCombatEndPatch
	{
		private static void Prefix()
		{
			RemoteFxFocus.Reset();
			try
			{
				CreatureHold.Reset();
			}
			catch (Exception ex)
			{
				DevLog.Warn("CreatureHoldCombatEndPatch: " + ex);
			}
			try
			{
				BurstFx.Abandon();
			}
			catch (Exception ex2)
			{
				DevLog.Warn("CreatureHoldCombatEndPatch (Burst): " + ex2);
			}
			try
			{
				ExposeWipe.Reset();
			}
			catch (Exception ex3)
			{
				DevLog.Warn("CreatureHoldCombatEndPatch (Expose): " + ex3);
			}
		}
	}
	[HarmonyPatch(typeof(TimeFreeze), "SlowEnemies")]
