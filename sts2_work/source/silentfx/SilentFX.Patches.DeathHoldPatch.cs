// namespace: SilentFX.Patches
// type: DeathHoldPatch (class)
// source lines: 20211-20235
	public static class DeathHoldPatch
	{
		public static bool Prefix(NCreature __instance, bool shouldRemove, ref float __result)
		{
			if (!PlayerSettings.Enabled)
			{
				return true;
			}
			try
			{
				if (!TimeFreeze.HoldDeath(__instance, shouldRemove))
				{
					return true;
				}
				__result = 0.6f;
				return false;
			}
			catch (Exception ex)
			{
				DevLog.Error("DeathHoldPatch: " + ex);
				return true;
			}
		}
	}
	[HarmonyPatch(typeof(NHealthBar), "RefreshValues")]
