// namespace: SilentFX.Patches
// type: StatusChangeHoldPatch (class)
// source lines: 23231-23267
	public static class StatusChangeHoldPatch
	{
		public static bool Prepare()
		{
			try
			{
				return StatusStepHooks.FindChanged() != null;
			}
			catch
			{
				return false;
			}
		}

		public static MethodBase TargetMethod()
		{
			return StatusStepHooks.FindChanged();
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
				DevLog.Error("StatusChangeHoldPatch: " + ex);
				return true;
			}
		}
	}
	[HarmonyPatch]
