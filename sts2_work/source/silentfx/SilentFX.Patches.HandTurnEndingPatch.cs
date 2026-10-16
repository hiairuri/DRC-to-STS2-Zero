// namespace: SilentFX.Patches
// type: HandTurnEndingPatch (class)
// source lines: 21014-21032
	public static class HandTurnEndingPatch
	{
		public static void Prefix(Player player)
		{
			try
			{
				HandFlow.RaiseTurnEnding(player);
			}
			catch (Exception ex)
			{
				DevLog.Error("HandTurnEndingPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(HandPosHelper), "GetPosition", new Type[]
	{
		typeof(int),
		typeof(int)
	})]
