// namespace: SilentFX.Patches
// type: AimHostHandshakePatch (class)
// source lines: 19476-19493
	internal static class AimHostHandshakePatch
	{
		private static bool Prepare()
		{
			return AimCompatibility.HostHandshakeTarget != null;
		}

		private static MethodBase TargetMethod()
		{
			return AimCompatibility.HostHandshakeTarget;
		}

		private static void Postfix(NetClientGameService __instance, ulong __0, object __1)
		{
			AimCompatibility.RememberHost(__instance, __0, __1);
		}
	}
	[HarmonyPatch]
