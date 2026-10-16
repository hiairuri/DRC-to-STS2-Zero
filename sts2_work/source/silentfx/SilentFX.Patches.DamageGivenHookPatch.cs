// namespace: SilentFX.Patches
// type: DamageGivenHookPatch (class)
// source lines: 21949-22020
	public static class DamageGivenHookPatch
	{
		public static void Prefix(Creature dealer, DamageResult results, ValueProp props, Creature target, CardModel cardSource)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (results == null || target == null)
				{
					return;
				}
				bool flag = ValuePropExtensions.IsPoweredAttack(props) && ShadowEcho.Active(dealer);
				if (PlayerSettings.Enabled && cardSource is Shiv)
				{
					ShivFx.OnShivHit(cardSource, target);
				}
				else if (cardSource != null)
				{
					CardFxManager.OnDamage(cardSource, target, results);
				}
				if (dealer != null && ValuePropExtensions.IsPoweredAttack(props) && target.HasPower<WeakPower>())
				{
					if (dealer.IsPlayer)
					{
						if (!MultiplayerFx.Active || MultiplayerFx.IsLocal(dealer) || cardSource != null)
						{
							PowerFxManager.TriggerFor<TrackingPower>(dealer, target);
						}
					}
					else if (MultiplayerFx.Active && cardSource != null)
					{
						TriggerRemotePetTracking(dealer, target);
					}
				}
				if (PlayerSettings.Enabled & flag)
				{
					ShadowEcho.OnHit(cardSource, dealer, target, handled: false, results.TotalDamage);
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("DamageGivenHookPatch: " + ex);
			}
		}

		private static void TriggerRemotePetTracking(Creature dealer, Creature target)
		{
			NCombatRoom combatRoom = GameRefs.CombatRoom;
			if (combatRoom == null)
			{
				return;
			}
			foreach (NCreature creatureNode in combatRoom.CreatureNodes)
			{
				Creature val = (FxKit.Alive((GodotObject)(object)creatureNode) ? creatureNode.Entity : null);
				if (val == null || !val.IsPlayer || MultiplayerFx.IsLocal(val))
				{
					continue;
				}
				foreach (Creature pet in val.Pets)
				{
					if (pet == dealer)
					{
						PowerFxManager.TriggerFor<TrackingPower>(val, target);
						break;
					}
				}
			}
		}
	}
	[HarmonyPatch(typeof(Hook), "BeforeFlush")]
