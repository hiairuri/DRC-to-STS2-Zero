// namespace: SilentFX.Patches
// type: HandDecoFreedPatch (class)
// source lines: 21111-21125
	public static class HandDecoFreedPatch
	{
		public static void Prefix(NCard __instance)
		{
			try
			{
				CardDeco.DetachHost(__instance);
			}
			catch (Exception ex)
			{
				DevLog.Error("HandDecoFreedPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(NPlayerHand), "SelectCards")]
