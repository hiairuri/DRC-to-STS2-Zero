// namespace: SilentFX.Patches
// type: LocalAimHoverPatch (class)
// source lines: 19546-19553
	internal static class LocalAimHoverPatch
	{
		private static void Postfix(NCardPlay __instance, NCreature creature)
		{
			AimLocalTarget.SetTarget(__instance, (creature != null) ? creature.Entity : null);
		}
	}
	[HarmonyPatch(typeof(NCardPlay), "OnCreatureUnhover")]
