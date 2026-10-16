// namespace: SilentFX.Patches
// type: StrangleNotePatch (class)
// source lines: 22967-23004
	public static class StrangleNotePatch
	{
		public static void Prefix(StranglePower __instance, CardPlay cardPlay)
		{
			try
			{
				Creature applier = ((PowerModel)__instance).Applier;
				Player val = ((applier != null) ? applier.Player : null);
				if (val == null)
				{
					return;
				}
				object obj;
				if (cardPlay == null)
				{
					obj = null;
				}
				else
				{
					CardModel card = cardPlay.Card;
					obj = ((card != null) ? card.Owner : null);
				}
				if (obj == val)
				{
					StrangleSeen.Note(__instance, cardPlay.Card);
					if (MultiplayerFx.Active)
					{
						PowerFxManager.NoteStrangleCardPlayed(__instance, cardPlay.Card);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("StrangleNotePatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(StranglePower), "AfterCardPlayed")]
