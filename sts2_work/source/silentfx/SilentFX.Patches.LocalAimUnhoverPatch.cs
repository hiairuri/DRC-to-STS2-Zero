// namespace: SilentFX.Patches
// type: LocalAimUnhoverPatch (class)
// source lines: 19554-19561
	internal static class LocalAimUnhoverPatch
	{
		private static void Postfix(NCardPlay __instance)
		{
			AimLocalTarget.ClearTarget(__instance);
		}
	}
	[HarmonyPatch(typeof(NCardPlay), "Cleanup")]
