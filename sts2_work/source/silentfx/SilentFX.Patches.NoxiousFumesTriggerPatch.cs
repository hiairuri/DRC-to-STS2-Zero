// namespace: SilentFX.Patches
// type: NoxiousFumesTriggerPatch (class)
// source lines: 22745-22762
	public static class NoxiousFumesTriggerPatch
	{
		public static void Prefix(NoxiousFumesPower __instance, IReadOnlyList<Creature> participants)
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
				DevLog.Error("NoxiousFumesTriggerPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(WraithFormPower), "AfterSideTurnStart")]
