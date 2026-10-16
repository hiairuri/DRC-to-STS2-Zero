// namespace: SilentFX.Patches
// type: MouseCardPickedUpPatch (class)
// source lines: 21276-21290
	public static class MouseCardPickedUpPatch
	{
		public static void Prefix(NMouseCardPlay __instance)
		{
			try
			{
				HeldHooks.Begin((NCardPlay)(object)__instance, () => HeldHooks.OverPlayArea(__instance));
			}
			catch (Exception ex)
			{
				DevLog.Error("MouseCardPickedUpPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(NControllerCardPlay), "Start")]
