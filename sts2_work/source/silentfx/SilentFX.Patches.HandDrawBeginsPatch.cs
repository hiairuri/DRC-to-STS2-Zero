// namespace: SilentFX.Patches
// type: HandDrawBeginsPatch (class)
// source lines: 20531-20545
	public static class HandDrawBeginsPatch
	{
		public static void Prefix(Player player)
		{
			try
			{
				HandFlow.RaiseHandDrawBegins(player);
			}
			catch (Exception ex)
			{
				DevLog.Error("HandDrawBeginsPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(Hook), "AfterCardDrawn")]
