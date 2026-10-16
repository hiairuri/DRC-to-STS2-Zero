// namespace: SilentFX.Patches
// type: ConcoctTriggerPatch (class)
// source lines: 22477-22509
	public static class ConcoctTriggerPatch
	{
		public static bool Prepare()
		{
			return GameCapabilities.ConcoctAfterDamageGiven != null;
		}

		public static MethodBase TargetMethod()
		{
			return GameCapabilities.ConcoctAfterDamageGiven;
		}

		public static void Prefix(PowerModel __instance, Creature dealer, DamageResult result, ValueProp props, Creature target)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (dealer != null && dealer == __instance.Owner && result != null && result.UnblockedDamage > 0 && ValuePropExtensions.IsPoweredAttack(props))
				{
					PowerFxManager.Trigger(__instance, target);
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("ConcoctTriggerPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(CombatRoom), "AddExtraReward", new Type[]
	{
		typeof(Player),
		typeof(Reward)
	})]
