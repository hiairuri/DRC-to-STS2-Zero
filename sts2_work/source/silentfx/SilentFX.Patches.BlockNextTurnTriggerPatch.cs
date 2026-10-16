// namespace: SilentFX.Patches
// type: BlockNextTurnTriggerPatch (class)
// source lines: 23065-23082
	public static class BlockNextTurnTriggerPatch
	{
		public static void Prefix(BlockNextTurnPower __instance, Creature creature)
		{
			try
			{
				if (creature != null && creature == ((PowerModel)__instance).Owner)
				{
					PowerFxManager.Trigger((PowerModel)(object)__instance);
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("BlockNextTurnTriggerPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(IntangiblePower), "AfterModifyingHpLostAfterOsty")]
