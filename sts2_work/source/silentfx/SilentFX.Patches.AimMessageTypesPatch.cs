// namespace: SilentFX.Patches
// type: AimMessageTypesPatch (class)
// source lines: 19450-19457
	internal static class AimMessageTypesPatch
	{
		private static void Postfix()
		{
			AimCompatibility.NormalizeTypes();
		}
	}
	[HarmonyPatch]
