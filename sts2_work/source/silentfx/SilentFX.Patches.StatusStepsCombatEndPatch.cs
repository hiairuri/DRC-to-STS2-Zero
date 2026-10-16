// namespace: SilentFX.Patches
// type: StatusStepsCombatEndPatch (class)
// source lines: 23305-23327
	internal static class StatusStepsCombatEndPatch
	{
		private static void Prefix()
		{
			try
			{
				StatusSteps.Reset();
			}
			catch (Exception ex)
			{
				DevLog.Warn("StatusStepsCombatEndPatch: " + ex);
			}
			try
			{
				MalaiseFx.Reset();
			}
			catch (Exception ex2)
			{
				DevLog.Warn("StatusStepsCombatEndPatch (Malaise): " + ex2);
			}
		}
	}
	[HarmonyPatch(typeof(NCombatRoom), "_ExitTree")]
