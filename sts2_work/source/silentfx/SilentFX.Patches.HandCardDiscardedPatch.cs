// namespace: SilentFX.Patches
// type: HandCardDiscardedPatch (class)
// source lines: 20933-20955
	public static class HandCardDiscardedPatch
	{
		public static void Prefix(PlayerChoiceContext choiceContext, CardModel card)
		{
			try
			{
				HandFlow.RaiseCardDiscarded(card, (choiceContext != null) ? choiceContext.LastInvolvedModel : null);
			}
			catch (Exception ex)
			{
				DevLog.Error("HandCardDiscardedPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(CardCmd), "AutoPlay", new Type[]
	{
		typeof(PlayerChoiceContext),
		typeof(CardModel),
		typeof(Creature),
		typeof(AutoPlayType),
		typeof(bool),
		typeof(bool)
	})]
