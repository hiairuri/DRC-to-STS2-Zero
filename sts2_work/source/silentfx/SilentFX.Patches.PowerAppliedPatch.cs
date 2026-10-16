// namespace: SilentFX.Patches
// type: PowerAppliedPatch (class)
// source lines: 22673-22687
	public static class PowerAppliedPatch
	{
		public static void Postfix(Creature __instance, PowerModel power)
		{
			try
			{
				PowerFxManager.OnApplied(__instance, power);
			}
			catch (Exception ex)
			{
				DevLog.Error("PowerAppliedPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(Creature), "RemovePowerInternal")]
