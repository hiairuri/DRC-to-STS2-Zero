// namespace: SilentFX.Patches
// type: SpeedsterTriggerPatch (class)
// source lines: 23030-23064
	public static class SpeedsterTriggerPatch
	{
		public static void Prefix(SpeedsterPower __instance, CardModel card, bool fromHandDraw)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				object obj;
				if (card == null)
				{
					obj = null;
				}
				else
				{
					Player owner = card.Owner;
					obj = ((owner != null) ? owner.Creature : null);
				}
				Creature val = (Creature)obj;
				if (!fromHandDraw && val != null && val == ((PowerModel)__instance).Owner)
				{
					ICombatState combatState = val.CombatState;
					if (combatState != null && combatState.CurrentSide == val.Side)
					{
						PowerFxManager.Trigger((PowerModel)(object)__instance);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("SpeedsterTriggerPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(BlockNextTurnPower), "AfterBlockCleared")]
