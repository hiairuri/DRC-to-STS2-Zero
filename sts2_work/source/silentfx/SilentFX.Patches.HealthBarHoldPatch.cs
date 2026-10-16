// namespace: SilentFX.Patches
// type: HealthBarHoldPatch (class)
// source lines: 20236-20259
	public static class HealthBarHoldPatch
	{
		public static bool Prefix(NHealthBar __instance)
		{
			if (!PlayerSettings.Enabled)
			{
				return true;
			}
			try
			{
				return !TimeFreeze.HoldBar(__instance) && !ExposeWipe.HoldsBar(__instance);
			}
			catch (Exception ex)
			{
				DevLog.Error("HealthBarHoldPatch: " + ex);
				return true;
			}
		}
	}
	[HarmonyPatch(typeof(NDamageNumVfx), "Create", new Type[]
	{
		typeof(Vector2),
		typeof(int)
	})]
