// namespace: SilentFX.Patches
// type: IntangibleTriggerPatch (class)
// source lines: 23083-23097
	public static class IntangibleTriggerPatch
	{
		public static void Prefix(IntangiblePower __instance)
		{
			try
			{
				PowerFxManager.Trigger((PowerModel)(object)__instance);
			}
			catch (Exception ex)
			{
				DevLog.Error("IntangibleTriggerPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(CombatHistory), "BlockGained")]
