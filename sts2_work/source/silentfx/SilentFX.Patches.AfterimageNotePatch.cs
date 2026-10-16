// namespace: SilentFX.Patches
// type: AfterimageNotePatch (class)
// source lines: 22837-22868
	public static class AfterimageNotePatch
	{
		public static void Prefix(AfterimagePower __instance, CardPlay cardPlay)
		{
			try
			{
				CardModel val = ((cardPlay != null) ? cardPlay.Card : null);
				if (MultiplayerFx.Active && !MultiplayerFx.IsLocal(((PowerModel)__instance).Owner))
				{
					object obj;
					if (val == null)
					{
						obj = null;
					}
					else
					{
						Player owner = val.Owner;
						obj = ((owner != null) ? owner.Creature : null);
					}
					if (obj == ((PowerModel)__instance).Owner)
					{
						AfterimageRemoteSeen.Note(__instance, val);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("AfterimageNotePatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(AfterimagePower), "AfterCardPlayed")]
