// namespace: SilentFX.Patches
// type: HandDecoReturnedPatch (class)
// source lines: 21096-21110
	public static class HandDecoReturnedPatch
	{
		public static void Prefix(NCard __instance)
		{
			try
			{
				CardDeco.DetachHost(__instance);
			}
			catch (Exception ex)
			{
				DevLog.Error("HandDecoReturnedPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(NCard), "OnFreedToPool", new Type[] { })]
