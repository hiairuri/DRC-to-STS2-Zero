// namespace: SilentFX.Patches
// type: HandDrawSizedPatch (class)
// source lines: 20516-20530
	public static class HandDrawSizedPatch
	{
		public static void Postfix(Player player, decimal __result)
		{
			try
			{
				HandFlow.RaiseHandDrawSized(player, (int)__result);
			}
			catch (Exception ex)
			{
				DevLog.Error("HandDrawSizedPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(Hook), "BeforeHandDraw")]
