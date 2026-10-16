// namespace: SilentFX.Patches
// type: StatusAmountHoldPatch (class)
// source lines: 23194-23230
	public static class StatusAmountHoldPatch
	{
		public static bool Prepare()
		{
			try
			{
				return StatusStepHooks.FindRefresh() != null;
			}
			catch
			{
				return false;
			}
		}

		public static MethodBase TargetMethod()
		{
			return StatusStepHooks.FindRefresh();
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
				DevLog.Error("StatusAmountHoldPatch: " + ex);
				return true;
			}
		}
	}
	[HarmonyPatch]
