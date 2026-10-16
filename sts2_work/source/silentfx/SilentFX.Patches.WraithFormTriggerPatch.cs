// namespace: SilentFX.Patches
// type: WraithFormTriggerPatch (class)
// source lines: 22763-22780
	public static class WraithFormTriggerPatch
	{
		public static void Prefix(WraithFormPower __instance, IReadOnlyList<Creature> participants)
		{
			try
			{
				if (participants != null && participants.Contains(((PowerModel)__instance).Owner))
				{
					PowerFxManager.Trigger((PowerModel)(object)__instance);
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("WraithFormTriggerPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(InfiniteBladesPower), "BeforeHandDraw")]
