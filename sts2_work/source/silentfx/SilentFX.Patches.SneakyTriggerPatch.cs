// namespace: SilentFX.Patches
// type: SneakyTriggerPatch (class)
// source lines: 22139-22170
	public static class SneakyTriggerPatch
	{
		public static void Prefix(SneakyPower __instance, CardPlay cardPlay)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Invalid comparison between Unknown and I4
			try
			{
				CardModel val = ((cardPlay != null) ? cardPlay.Card : null);
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
				Creature val2 = (Creature)obj;
				if (val != null && (int)val.Type == 1 && val2 != null && val2 != ((PowerModel)__instance).Owner)
				{
					PowerFxManager.Trigger((PowerModel)(object)__instance);
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("SneakyTriggerPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(ToolsOfTheTradePower), "AfterPlayerTurnStart")]
