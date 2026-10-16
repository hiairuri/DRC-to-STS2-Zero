// namespace: SilentFX.Patches
// type: StrangleTriggerPatch (class)
// source lines: 23005-23029
	public static class StrangleTriggerPatch
	{
		public static void Prefix(StranglePower __instance, CardPlay cardPlay)
		{
			try
			{
				if (StrangleSeen.Take(__instance, (cardPlay != null) ? cardPlay.Card : null) && (MultiplayerFx.Active || MultiplayerFx.CurrentInvocation == null))
				{
					if (MultiplayerFx.Active)
					{
						PowerFxManager.TriggerStrangle(__instance, cardPlay.Card);
					}
					else
					{
						PowerFxManager.Trigger((PowerModel)(object)__instance);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("StrangleTriggerPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(SpeedsterPower), "AfterCardDrawn")]
