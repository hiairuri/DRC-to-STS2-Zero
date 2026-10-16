// namespace: SilentFX.Patches
// type: ExposeIconHoldPatch (class)
// source lines: 19984-20025
	public static class ExposeIconHoldPatch
	{
		public static bool Prepare()
		{
			try
			{
				return ExposeHooks.FindRemoval() != null;
			}
			catch
			{
				return false;
			}
		}

		public static MethodBase TargetMethod()
		{
			return ExposeHooks.FindRemoval();
		}

		public static bool Prefix(NPowerContainer __instance, object[] __args)
		{
			if (!PlayerSettings.Enabled)
			{
				return true;
			}
			try
			{
				PowerModel power = ExposeHooks.PowerIn(__args);
				if (ExposeWipe.HoldRemoval(__instance, power))
				{
					return false;
				}
				return !StatusSteps.HoldRemoval(__instance, power);
			}
			catch (Exception ex)
			{
				DevLog.Error("ExposeIconHoldPatch: " + ex);
				return true;
			}
		}
	}
	[HarmonyPatch]
