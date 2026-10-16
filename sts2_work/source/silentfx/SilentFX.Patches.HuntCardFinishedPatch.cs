// namespace: SilentFX.Patches
// type: HuntCardFinishedPatch (class)
// source lines: 22528-22545
	internal static class HuntCardFinishedPatch
	{
		private static void Prefix(CardPlay cardPlay)
		{
			try
			{
				if (PlayerSettings.Enabled)
				{
					TheHuntFx.OnCardFinished((cardPlay != null) ? cardPlay.Card : null);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("HuntCardFinishedPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(NCombatRoom), "_ExitTree")]
