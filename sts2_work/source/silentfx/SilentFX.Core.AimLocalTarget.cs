// namespace: SilentFX.Core
// type: AimLocalTarget (class)
// source lines: 31104-31287
	internal static class AimLocalTarget
	{
		internal readonly struct Snapshot
		{
			internal readonly CardModel Card;

			internal readonly Creature Target;

			internal readonly bool IsHeld;

			internal readonly bool IsTargeting;

			internal readonly bool InPlayZone;

			internal Snapshot(CardModel card, Creature target, bool isHeld, bool isTargeting, bool inPlayZone = false)
			{
				Card = card;
				Target = target;
				IsHeld = isHeld;
				IsTargeting = isTargeting;
				InPlayZone = inPlayZone;
			}
		}

		private sealed class Session
		{
			internal NCardPlay Play;

			internal CardModel Card;

			internal Creature Target;

			internal object Room;
		}

		private static Session _current;

		internal static Snapshot Read()
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Invalid comparison between Unknown and I4
			if (!MultiplayerFx.Active)
			{
				_current = null;
				return default;
			}
			Session current = _current;
			if (current != null && IsLive(current.Play) && current.Card != null && MultiplayerFx.IsLocal(current.Card))
			{
				CardPile pile = current.Card.Pile;
				if (pile != null && (int)pile.Type == 2)
				{
					Player owner = current.Card.Owner;
					if (owner != null)
					{
						Creature creature = owner.Creature;
						if (((creature != null) ? new bool?(creature.IsAlive) : ((bool?)null)) == true)
						{
							if (GameRefs.CombatRoom == null || current.Room != GameRefs.CombatRoom)
							{
								_current = null;
								return default;
							}
							bool flag;
							try
							{
								flag = NTargetManager.Instance != null && NTargetManager.Instance.IsInSelection;
							}
							catch
							{
								flag = false;
							}
							Creature val = (flag ? current.Target : null);
							if (val != null)
							{
								try
								{
									if (GameRefs.CombatRoom.GetCreatureNode(val) == null)
									{
										val = null;
									}
								}
								catch
								{
									val = null;
								}
							}
							bool inPlayZone;
							try
							{
								NCardPlay play = current.Play;
								NMouseCardPlay val2 = (NMouseCardPlay)(object)((play is NMouseCardPlay) ? play : null);
								inPlayZone = ((val2 != null) ? HeldHooks.OverPlayArea(val2) : (current.Play is NControllerCardPlay));
							}
							catch
							{
								inPlayZone = false;
							}
							return new Snapshot(current.Card, val, isHeld: true, flag, inPlayZone);
						}
					}
				}
			}
			return default;
		}

		internal static bool TryRead(out CardModel card, out Creature target, out bool isTargeting)
		{
			Snapshot snapshot = Read();
			card = snapshot.Card;
			target = snapshot.Target;
			isTargeting = snapshot.IsTargeting;
			return card != null;
		}

		internal static void Begin(NCardPlay play)
		{
			if (MultiplayerFx.Active && play != null)
			{
				CardModel val = null;
				try
				{
					NHandCardHolder holder = play.Holder;
					val = ((holder != null) ? ((NCardHolder)holder).CardModel : null);
				}
				catch
				{
				}
				if (val != null && MultiplayerFx.IsLocal(val))
				{
					_current = new Session
					{
						Play = play,
						Card = val,
						Room = GameRefs.CombatRoom
					};
				}
			}
		}

		internal static void SetTarget(NCardPlay play, Creature target)
		{
			Session current = _current;
			if (current != null && current.Play == play && MultiplayerFx.Active)
			{
				current.Target = target;
			}
		}

		internal static void ClearTarget(NCardPlay play)
		{
			Session current = _current;
			if (current != null && current.Play == play)
			{
				current.Target = null;
			}
		}

		internal static void End(NCardPlay play)
		{
			Session current = _current;
			if (current != null && current.Play == play)
			{
				_current = null;
			}
		}

		internal static void Reset()
		{
			_current = null;
		}

		private static bool IsLive(NCardPlay play)
		{
			try
			{
				return play != null && GodotObject.IsInstanceValid((GodotObject)(object)play) && ((Node)play).IsInsideTree();
			}
			catch
			{
				return false;
			}
		}
	}
