// namespace: SilentFX.Patches
// type: AfterimageTriggerPatch (class)
// source lines: 22869-22908
	public static class AfterimageTriggerPatch
	{
		public static void Prefix(AfterimagePower __instance, CardPlay cardPlay)
		{
			try
			{
				object obj;
				if (cardPlay == null)
				{
					obj = null;
				}
				else
				{
					CardModel card = cardPlay.Card;
					if (card == null)
					{
						obj = null;
					}
					else
					{
						Player owner = card.Owner;
						obj = ((owner != null) ? owner.Creature : null);
					}
				}
				if (obj == ((PowerModel)__instance).Owner)
				{
					bool flag = AfterimageRemoteSeen.Take(__instance, (cardPlay != null) ? cardPlay.Card : null);
					if ((!MultiplayerFx.Active || MultiplayerFx.IsLocal(((PowerModel)__instance).Owner)) | flag)
					{
						PowerFxManager.Trigger((PowerModel)(object)__instance);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("AfterimageTriggerPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(EnvenomPower), "AfterDamageGiven")]
