// namespace: SilentFX.Patches
// type: ControllerAimSessionPatch (class)
// source lines: 19538-19545
	internal static class ControllerAimSessionPatch
	{
		private static void Prefix(NControllerCardPlay __instance)
		{
			AimLocalTarget.Begin((NCardPlay)(object)__instance);
		}
	}
	[HarmonyPatch(typeof(NCardPlay), "OnCreatureHover")]
