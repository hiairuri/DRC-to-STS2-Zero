// namespace: SilentFX.Patches
// type: HitSparkHoldPatch (class)
// source lines: 20284-20302
	public static class HitSparkHoldPatch
	{
		public static void Postfix(NHitSparkVfx __result, Creature target)
		{
			if (!PlayerSettings.Enabled)
			{
				return;
			}
			try
			{
				TimeFreeze.OnHitSpark(__result, target);
			}
			catch (Exception ex)
			{
				DevLog.Error("HitSparkHoldPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(VfxCmd), "PlayVfx")]
