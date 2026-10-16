// namespace: SilentFX.Patches
// type: CombatRoomExitPatch (class)
// source lines: 22723-22744
	public static class CombatRoomExitPatch
	{
		public static void Prefix()
		{
			try
			{
				AfterimageRemoteSeen.Reset();
				PowerFxManager.ClearAll(animate: false);
				ThornsFx.Reset();
				ShadowEcho.Reset();
				ShadowStepHeld.Reset();
				ShadowPoolFx.Reset();
				CardStandIn.Reset();
				Tally.Reset();
			}
			catch (Exception ex)
			{
				DevLog.Error("CombatRoomExitPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(NoxiousFumesPower), "AfterSideTurnStart")]
