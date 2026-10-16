// namespace: SilentFX.Patches
// type: HandCardDrawnPatch (class)
// source lines: 20546-20564
	public static class HandCardDrawnPatch
	{
		public static void Prefix(CardModel card, bool fromHandDraw)
		{
			try
			{
				HandFlow.RaiseCardDrawn(card, fromHandDraw);
			}
			catch (Exception ex)
			{
				DevLog.Error("HandCardDrawnPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(NPlayerHand), "Add", new Type[]
	{
		typeof(NCard),
		typeof(int)
	})]
