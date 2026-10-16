// namespace: SilentFX.Patches
// type: FlushHookPatch (class)
// source lines: 22021-22052
	public static class FlushHookPatch
	{
		public static void Prefix(Player player, out float __state)
		{
			__state = 0f;
			try
			{
				__state = TimeFreeze.Thaw();
				PowerFxManager.TriggerFor<WellLaidPlansPower>((player != null) ? player.Creature : null, (Creature)null);
			}
			catch (Exception ex)
			{
				DevLog.Error("FlushHookPatch: " + ex);
			}
		}

		public static void Postfix(ref Task __result, float __state)
		{
			try
			{
				if (__state > 0f)
				{
					__result = TimeFreeze.Linger(__result, __state);
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("FlushHookPatch (linger): " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(ThornsPower), "BeforeDamageReceived")]
