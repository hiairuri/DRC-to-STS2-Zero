// namespace: SilentFX.Patches
// type: ExposeAppliedCoverPatch (class)
// source lines: 20066-20105
	public static class ExposeAppliedCoverPatch
	{
		public static bool Prepare()
		{
			try
			{
				return ExposeHooks.Creates("NPowerAppliedVfx").Count > 0;
			}
			catch
			{
				return false;
			}
		}

		public static IEnumerable<MethodBase> TargetMethods()
		{
			return ExposeHooks.Creates("NPowerAppliedVfx");
		}

		public static void Postfix(object[] __args, object __result)
		{
			if (!PlayerSettings.Enabled)
			{
				return;
			}
			try
			{
				PowerModel power = ExposeHooks.PowerIn(__args);
				if (__result != null && (ExposeWipe.CoversApplied(power) || StatusSteps.CoversApplied(power)))
				{
					ExposeHooks.Hide(__result);
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("ExposeAppliedCoverPatch: " + ex);
			}
		}
	}
	[HarmonyPatch]
