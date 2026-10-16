// namespace: SilentFX.Patches
// type: PoisonTriggerPatch (class)
// source lines: 22424-22448
	public static class PoisonTriggerPatch
	{
		public static bool Prepare()
		{
			return GameCapabilities.PoisonTrigger != null;
		}

		public static MethodBase TargetMethod()
		{
			return GameCapabilities.PoisonTrigger;
		}

		public static void Prefix(PoisonPower __instance)
		{
			try
			{
				PoisonFx.OnPoisonTriggered(__instance);
			}
			catch (Exception ex)
			{
				DevLog.Error("PoisonTriggerPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(CorrosiveWavePower), "AfterCardDrawn")]
