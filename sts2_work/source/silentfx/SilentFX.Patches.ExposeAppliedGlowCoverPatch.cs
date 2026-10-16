// namespace: SilentFX.Patches
// type: ExposeAppliedGlowCoverPatch (class)
// source lines: 20165-20210
	public static class ExposeAppliedGlowCoverPatch
	{
		private static List<MethodBase> Targets()
		{
			List<MethodBase> list = ExposeHooks.Creates("NPowerAppliedDebuffVfx");
			list.AddRange(ExposeHooks.Creates("NPowerAppliedBuffVfx"));
			return list;
		}

		public static bool Prepare()
		{
			try
			{
				return Targets().Count > 0;
			}
			catch
			{
				return false;
			}
		}

		public static IEnumerable<MethodBase> TargetMethods()
		{
			return Targets();
		}

		public static void Postfix(object __result)
		{
			if (!PlayerSettings.Enabled)
			{
				return;
			}
			try
			{
				if (__result != null && (ExposeWipe.Covering || StatusSteps.Covering))
				{
					ExposeHooks.Hide(__result);
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("ExposeAppliedGlowCoverPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(NCreature), "StartDeathAnim")]
