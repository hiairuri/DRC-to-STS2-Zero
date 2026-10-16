// namespace: SilentFX.Patches
// type: AimClientServicePatch (class)
// source lines: 19512-19529
	internal static class AimClientServicePatch
	{
		private static bool Prepare()
		{
			return AimCompatibility.ClientServiceTarget != null;
		}

		private static MethodBase TargetMethod()
		{
			return AimCompatibility.ClientServiceTarget;
		}

		private static void Postfix(NetClientGameService __instance)
		{
			AimSync.Attach((INetGameService)(object)__instance);
		}
	}
	[HarmonyPatch(typeof(NMouseCardPlay), "Start")]
