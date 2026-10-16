// namespace: SilentFX.Patches
// type: MouseAimSessionPatch (class)
// source lines: 19530-19537
	internal static class MouseAimSessionPatch
	{
		private static void Prefix(NMouseCardPlay __instance)
		{
			AimLocalTarget.Begin((NCardPlay)(object)__instance);
		}
	}
	[HarmonyPatch(typeof(NControllerCardPlay), "Start")]
