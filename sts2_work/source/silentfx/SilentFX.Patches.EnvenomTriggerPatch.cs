// namespace: SilentFX.Patches
// type: EnvenomTriggerPatch (class)
// source lines: 22909-22927
	public static class EnvenomTriggerPatch
	{
		public static void Prefix(EnvenomPower __instance, Creature dealer, DamageResult result, ValueProp props, Creature target)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (dealer != null && dealer == ((PowerModel)__instance).Owner && result != null && result.UnblockedDamage > 0 && ValuePropExtensions.IsPoweredAttack(props))
				{
					PowerFxManager.Trigger((PowerModel)(object)__instance, target);
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("EnvenomTriggerPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(BlurPower), "AfterPreventingBlockClear")]
