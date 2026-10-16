// namespace: SilentFX.Patches
// type: CardFinishedHookPatch (class)
// source lines: 22242-22269
	public static class CardFinishedHookPatch
	{
		public static void Prefix(CardPlay cardPlay)
		{
			RemoteFxFocus.AfterCardPlayed(cardPlay);
			try
			{
				ShivFx.NoteCardFinished(cardPlay);
				AssassinateFx.NoteCardFinished((cardPlay != null) ? cardPlay.Card : null);
				FinisherFx.NoteCardFinished((cardPlay != null) ? cardPlay.Card : null);
				SnakebiteFx.NoteCardFinished((cardPlay != null) ? cardPlay.Card : null);
				MalaiseFx.NoteCardFinished((cardPlay != null) ? cardPlay.Card : null);
				PinpointFocus.NoteCardFinished((cardPlay != null) ? cardPlay.Card : null);
				HeldFxManager.OnCardFinished((cardPlay != null) ? cardPlay.Card : null);
				Showcase.NoteCardFinished(cardPlay);
				TourController.NoteCardFinished();
				if (cardPlay != null)
				{
					BurstFx.NoteFinished(cardPlay.Card, Convert.ToInt32(cardPlay.PlayIndex), Convert.ToInt32(cardPlay.PlayCount));
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("CardFinishedHookPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(Hook), "BeforeSideTurnStart")]
