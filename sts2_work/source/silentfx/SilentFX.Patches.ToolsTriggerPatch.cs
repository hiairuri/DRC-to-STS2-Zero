// namespace: SilentFX.Patches
// type: ToolsTriggerPatch (class)
// source lines: 22171-22192
	public static class ToolsTriggerPatch
	{
		public static void Prefix(ToolsOfTheTradePower __instance, Player player)
		{
			try
			{
				if (player != null)
				{
					Creature owner = ((PowerModel)__instance).Owner;
					if (((owner != null) ? owner.Player : null) == player)
					{
						PowerFxManager.Trigger((PowerModel)(object)__instance);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("ToolsTriggerPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(BurstPower), "AfterModifyingCardPlayCount")]
