// namespace: SilentFX.Patches
// type: StatusFlashHoldPatch (class)
// source lines: 23268-23304
	public static class StatusFlashHoldPatch
	{
		public static bool Prepare()
		{
			try
			{
				return StatusStepHooks.FindFlash() != null;
			}
			catch
			{
				return false;
			}
		}

		public static MethodBase TargetMethod()
		{
			return StatusStepHooks.FindFlash();
		}

		public static bool Prefix(NPower __instance)
		{
			if (!PlayerSettings.Enabled)
			{
				return true;
			}
			try
			{
				return !StatusSteps.HoldsAmount(__instance);
			}
			catch (Exception ex)
			{
				DevLog.Error("StatusFlashHoldPatch: " + ex);
				return true;
			}
		}
	}
	[HarmonyPatch(typeof(Hook), "AfterCombatEnd")]
