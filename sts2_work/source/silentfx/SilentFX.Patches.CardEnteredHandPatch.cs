// namespace: SilentFX.Patches
// type: CardEnteredHandPatch (class)
// source lines: 20565-20589
	public static class CardEnteredHandPatch
	{
		public static void Postfix(NCard card)
		{
			try
			{
				if (FxKit.Alive((GodotObject)(object)card))
				{
					HandFlow.NoteDrawEntered(card);
					HandFlow.RaiseCardEnteredHand(card);
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("CardEnteredHandPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(CardPileCmd), "Draw", new Type[]
	{
		typeof(PlayerChoiceContext),
		typeof(decimal),
		typeof(Player),
		typeof(bool)
	})]
