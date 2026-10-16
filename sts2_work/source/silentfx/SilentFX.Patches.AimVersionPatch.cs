// namespace: SilentFX.Patches
// type: AimVersionPatch (class)
// source lines: 19458-19475
	internal static class AimVersionPatch
	{
		private static bool Prepare()
		{
			return AimCompatibility.VersionTarget != null;
		}

		private static MethodBase TargetMethod()
		{
			return AimCompatibility.VersionTarget;
		}

		private static void Postfix(ref object __result)
		{
			AimCompatibility.Advertise(ref __result);
		}
	}
	[HarmonyPatch]
