// namespace: SilentFX.Patches
// type: HuntExtraRewardPatch (class)
// source lines: 22510-22527
	internal static class HuntExtraRewardPatch
	{
		private static void Postfix(Player player, Reward reward)
		{
			try
			{
				if (PlayerSettings.Enabled && reward is CardReward)
				{
					TheHuntFx.OnExtraReward(player);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("HuntExtraRewardPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(Hook), "AfterCardPlayed")]
