// namespace: SilentFX.Patches
// type: CardDiscardedHookPatch (class)
// source lines: 22099-22117
	public static class CardDiscardedHookPatch
	{
		public static void Prefix(CardModel card)
		{
			try
			{
				if (MasterPlannerFx.IsTagged(card))
				{
					Player owner = card.Owner;
					MasterPlannerFx.Discarded((owner != null) ? owner.Creature : null, card);
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("CardDiscardedHookPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(CardCmd), "AutoPlay")]
