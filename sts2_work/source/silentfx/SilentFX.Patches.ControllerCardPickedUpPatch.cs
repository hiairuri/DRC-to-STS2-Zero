// namespace: SilentFX.Patches
// type: ControllerCardPickedUpPatch (class)
// source lines: 21291-21305
	public static class ControllerCardPickedUpPatch
	{
		public static void Prefix(NControllerCardPlay __instance)
		{
			try
			{
				HeldHooks.Begin((NCardPlay)(object)__instance, null);
			}
			catch (Exception ex)
			{
				DevLog.Error("ControllerCardPickedUpPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(NCardPlay), "OnCreatureHover")]
