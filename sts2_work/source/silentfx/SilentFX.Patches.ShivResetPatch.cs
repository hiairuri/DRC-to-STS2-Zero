// namespace: SilentFX.Patches
// type: ShivResetPatch (class)
// source lines: 22316-22361
	public static class ShivResetPatch
	{
		public static void Prefix()
		{
			try
			{
				TimeFreeze.Reset();
				MultiplayerFx.Reset();
				ExposeWipe.Reset();
				CreatureHold.Reset();
				ShivFx.Reset();
				HeldFxManager.Reset();
				MasterPlannerFx.Reset();
				CardFlight.Reset();
				HandMoves.Reset();
				GambleFlow.Reset();
				PreciseCutChains.Reset();
				SlyGold.Reset();
				FlechettesOrbit.Reset();
				HandBackend.Reset();
				WraithFormIntroFx.Reset();
				WraithPlayFx.Reset();
				FinisherFx.Reset();
				Sfx.Reset();
				Mood.Reset();
				SkewerFx.Reset();
				PinpointFocus.Reset();
				EchoingSlashFx.Reset();
				PounceFx.Reset();
				PiercingWailFx.Reset();
				StrangleRope.Reset();
				MurderFx.Reset();
				FxLifetime.Reset();
				AssassinateFx.Reset();
				SnakebiteFx.Reset();
				SuppressFx.Reset();
				AfterimageFx.Reset();
				BurstFx.Reset();
			}
			catch (Exception ex)
			{
				DevLog.Error("ShivResetPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(Hook), "AfterCombatEnd")]
