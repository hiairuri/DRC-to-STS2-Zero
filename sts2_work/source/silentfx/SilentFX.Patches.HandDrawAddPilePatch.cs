// namespace: SilentFX.Patches
// type: HandDrawAddPilePatch (class)
// source lines: 20798-20818
	public static class HandDrawAddPilePatch
	{
		public static void Prefix(CardModel card, CardPile newPile)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				HandFlow.NoteDrawAdd(card, (PileType)((newPile != null) ? ((int)newPile.Type) : 0));
			}
			catch (Exception ex)
			{
				DevLog.Error("HandDrawAddPilePatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(CardCmd), "DiscardAndDraw", new Type[]
	{
		typeof(PlayerChoiceContext),
		typeof(IEnumerable<CardModel>),
		typeof(int)
	})]
