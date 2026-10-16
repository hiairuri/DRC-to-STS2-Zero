// namespace: SilentFX.Patches
// type: ThornsTriggerPatch (class)
// source lines: 22053-22072
	public static class ThornsTriggerPatch
	{
		public static void Prefix(ThornsPower __instance, Creature target, ValueProp props, Creature dealer, CardModel cardSource)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				bool flag = MultiplayerFx.Active && !MultiplayerFx.IsLocal(((PowerModel)__instance).Owner) && cardSource is Omnislice;
				if (dealer != null && target == ((PowerModel)__instance).Owner && (ValuePropExtensions.IsPoweredAttack(props) | flag))
				{
					PowerFxManager.Trigger((PowerModel)(object)__instance, dealer);
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("ThornsTriggerPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(MasterPlannerPower), "AfterCardPlayed")]
