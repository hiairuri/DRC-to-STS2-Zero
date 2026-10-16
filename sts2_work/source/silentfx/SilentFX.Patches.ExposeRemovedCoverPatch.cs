// namespace: SilentFX.Patches
// type: ExposeRemovedCoverPatch (class)
// source lines: 20026-20065
	public static class ExposeRemovedCoverPatch
	{
		public static bool Prepare()
		{
			try
			{
				return ExposeHooks.Creates("NPowerRemovedVfx").Count > 0;
			}
			catch
			{
				return false;
			}
		}

		public static IEnumerable<MethodBase> TargetMethods()
		{
			return ExposeHooks.Creates("NPowerRemovedVfx");
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
				if (__result != null && (ExposeWipe.CoversRemoved(power) || StatusSteps.CoversRemoved(power)))
				{
					ExposeHooks.Hide(__result);
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("ExposeRemovedCoverPatch: " + ex);
			}
		}
	}
	[HarmonyPatch]
