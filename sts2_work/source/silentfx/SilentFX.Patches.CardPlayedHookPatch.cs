// namespace: SilentFX.Patches
// type: CardPlayedHookPatch (class)
// source lines: 21902-21948
	public static class CardPlayedHookPatch
	{
		public static void Prefix(CardPlay cardPlay)
		{
			RemoteFxFocus.BeforeCardPlayed(cardPlay);
			try
			{
				TimeFreeze.OnCardPlayed(cardPlay);
				if (MultiplayerFx.Active)
				{
					CardModel obj = ((cardPlay != null) ? cardPlay.Card : null);
					ShadowStep val = (ShadowStep)(object)((obj is ShadowStep) ? obj : null);
					if (val != null)
					{
						Player owner = ((CardModel)val).Owner;
						ShadowEcho.Mark((owner != null) ? owner.Creature : null);
					}
				}
				CardModel obj2 = ((cardPlay != null) ? cardPlay.Card : null);
				Abrasive val2 = (Abrasive)(object)((obj2 is Abrasive) ? obj2 : null);
				if (val2 != null)
				{
					Player owner2 = ((CardModel)val2).Owner;
					ThornsFx.Mark((owner2 != null) ? owner2.Creature : null);
				}
				ShivFx.NoteCardPlayed(cardPlay);
				PowerFxManager.OnRemoteCardPlayed((cardPlay != null) ? cardPlay.Card : null, (cardPlay != null) ? cardPlay.Target : null);
				CardFxManager.OnCardPlayed(cardPlay);
				if (PlayerSettings.Enabled)
				{
					ShivFan.LaunchPending(cardPlay);
					MasterPlannerFx.OnCardPlayed(cardPlay);
				}
				Showcase.NoteCardStarted(cardPlay);
				TourController.NoteCardPlayed((cardPlay != null) ? cardPlay.Card : null);
				if (cardPlay != null)
				{
					BurstFx.NotePlay(cardPlay.Card, Convert.ToInt32(cardPlay.PlayIndex));
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("CardPlayedHookPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(Hook), "AfterDamageGiven")]
