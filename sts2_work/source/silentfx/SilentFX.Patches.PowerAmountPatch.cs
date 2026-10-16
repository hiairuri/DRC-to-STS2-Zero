// namespace: SilentFX.Patches
// type: PowerAmountPatch (class)
// source lines: 22708-22722
	public static class PowerAmountPatch
	{
		public static void Postfix(PowerModel __instance)
		{
			try
			{
				PowerFxManager.OnAmountChanged(__instance);
			}
			catch (Exception ex)
			{
				DevLog.Error("PowerAmountPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(NCombatRoom), "_ExitTree")]
