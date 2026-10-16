// namespace: SilentFX.Patches
// type: HandDrawAddTypePatch (class)
// source lines: 20775-20797
	public static class HandDrawAddTypePatch
	{
		public static void Prefix(CardModel card, PileType newPileType)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				HandFlow.NoteDrawAdd(card, newPileType);
			}
			catch (Exception ex)
			{
				DevLog.Error("HandDrawAddTypePatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(CardPileCmd), "Add", new Type[]
	{
		typeof(CardModel),
		typeof(CardPile),
		typeof(CardPilePosition),
		typeof(AbstractModel),
		typeof(bool)
	})]
