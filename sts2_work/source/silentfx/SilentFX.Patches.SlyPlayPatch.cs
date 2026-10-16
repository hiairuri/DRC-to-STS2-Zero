// namespace: SilentFX.Patches
// type: SlyPlayPatch (class)
// source lines: 22118-22138
	public static class SlyPlayPatch
	{
		public static void Prefix(CardModel card, AutoPlayType type)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Invalid comparison between Unknown and I4
			try
			{
				if ((int)type == 2 && MasterPlannerFx.IsTagged(card))
				{
					Player owner = card.Owner;
					MasterPlannerFx.PaidOff((owner != null) ? owner.Creature : null, card);
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("SlyPlayPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(SneakyPower), "AfterCardPlayed")]
