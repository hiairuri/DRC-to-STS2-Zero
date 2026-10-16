// namespace: SilentFX.Patches
// type: SkippedMotionPatch (class)
// source lines: 21444-21492
	public static class SkippedMotionPatch
	{
		public static bool Prefix(NCreature __instance, string trigger)
		{
			if (!PlayerSettings.Enabled)
			{
				return true;
			}
			try
			{
				if (trigger == "Shiv" && KnifeTrapFx.Throwing && Tuning.F("trap.keep_throw_anim", 0f) < 0.5f)
				{
					if (!MultiplayerFx.Active)
					{
						goto IL_0066;
					}
					if (MultiplayerFx.CurrentCard != null)
					{
						Player owner = MultiplayerFx.CurrentCard.Owner;
						if (((owner != null) ? owner.Creature : null) == ((__instance != null) ? __instance.Entity : null))
						{
							goto IL_0066;
						}
					}
				}
				if (trigger == "Attack" && AssassinateFx.Replaces((__instance != null) ? __instance.Entity : null))
				{
					return false;
				}
				if (trigger == "Attack" && FinisherFx.Replaces((__instance != null) ? __instance.Entity : null))
				{
					return false;
				}
				if (TimeFreeze.HoldMotion(__instance, trigger))
				{
					return false;
				}
				return true;
				IL_0066:
				return false;
			}
			catch (Exception ex)
			{
				DevLog.Error("SkippedMotionPatch: " + ex);
				return true;
			}
		}
	}
	[HarmonyPatch(typeof(NCardFlyVfx), "_Ready")]
