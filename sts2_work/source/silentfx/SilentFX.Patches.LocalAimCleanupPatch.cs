// namespace: SilentFX.Patches
// type: LocalAimCleanupPatch (class)
// source lines: 19562-19568
	internal static class LocalAimCleanupPatch
	{
		private static void Prefix(NCardPlay __instance)
		{
			AimLocalTarget.End(__instance);
		}
	}
