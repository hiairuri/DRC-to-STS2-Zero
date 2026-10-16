// namespace: SilentFX.Patches
// type: AimHostServicePatch (class)
// source lines: 19494-19511
	internal static class AimHostServicePatch
	{
		private static bool Prepare()
		{
			return AimCompatibility.HostServiceTarget != null;
		}

		private static MethodBase TargetMethod()
		{
			return AimCompatibility.HostServiceTarget;
		}

		private static void Postfix(NetHostGameService __instance)
		{
			AimSync.Attach((INetGameService)(object)__instance);
		}
	}
	[HarmonyPatch]
