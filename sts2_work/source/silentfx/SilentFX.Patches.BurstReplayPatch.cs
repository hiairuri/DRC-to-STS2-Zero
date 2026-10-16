// namespace: SilentFX.Patches
// type: BurstReplayPatch (class)
// source lines: 22227-22241
	public static class BurstReplayPatch
	{
		public static void Postfix(NCard __instance)
		{
			try
			{
				BurstFx.OnReplay(__instance);
			}
			catch (Exception ex)
			{
				DevLog.Error("BurstReplayPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(Hook), "AfterCardPlayed")]
