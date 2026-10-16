// namespace: SilentFX.Patches
// type: ExposeAppliedWindowPatch (class)
// source lines: 20106-20164
	public static class ExposeAppliedWindowPatch
	{
		private const int ByExpose = 1;

		private const int ByMalaise = 2;

		public static bool Prepare()
		{
			try
			{
				return ExposeHooks.FindIncreased() != null;
			}
			catch
			{
				return false;
			}
		}

		public static MethodBase TargetMethod()
		{
			return ExposeHooks.FindIncreased();
		}

		public static void Prefix(NCreature __instance, object[] __args, out int __state)
		{
			__state = 0;
			try
			{
				PowerModel power = ExposeHooks.PowerIn(__args);
				ExposeWipe.EnterApplied(__instance, power);
				__state |= 1;
				StatusSteps.EnterApplied(__instance, power);
				__state |= 2;
			}
			catch (Exception ex)
			{
				DevLog.Error("ExposeAppliedWindowPatch: " + ex);
			}
		}

		public static void Finalizer(int __state)
		{
			try
			{
				if ((__state & 1) != 0)
				{
					ExposeWipe.LeaveApplied();
				}
				if ((__state & 2) != 0)
				{
					StatusSteps.LeaveApplied();
				}
			}
			catch
			{
			}
		}
	}
	[HarmonyPatch]
