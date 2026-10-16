// namespace: SilentFX.Cards
// type: ShivFan (class)
// source lines: 95654-96999
	public static class ShivFan
	{
		private struct Knife
		{
			public Vector2 From;

			public ulong Arrive;
		}

		private sealed class Play
		{
			public CardModel Card;

			public Creature Owner;

			public object Invocation;

			public object Room;

			public bool Remote;

			public bool Scoped;

			public readonly List<FxTarget> Enemies = new List<FxTarget>();

			public readonly Dictionary<Creature, Knife> Knives = new Dictionary<Creature, Knife>(ReferenceEqualityComparer.Instance);

			public bool Launched;

			public bool Trap;

			public bool Last;

			public bool Through;

			public bool VeilLaid;

			public int Calls;
		}

		private sealed class HeldHilt
		{
			public Vector2 Position;

			public ulong At;

			public Creature Owner;

			public object Room;
		}

		private static readonly List<Play> Plays = new List<Play>();

		private static readonly List<float> Angles = new List<float>();

		private static Vector2 _sortHilt;

		private static float _sortMiddle;

		private static readonly Comparison<FxTarget> ByAngle = (FxTarget a, FxTarget b) =>
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			int num = Off(_sortHilt, _sortMiddle, a.Chest).CompareTo(Off(_sortHilt, _sortMiddle, b.Chest));
			return (num == 0) ? a.Chest.X.CompareTo(b.Chest.X) : num;
		};

		private static readonly Color Steel = new Color(0.75f, 0.92f, 1f, 1f);

		private static readonly Color Spectral = new Color(0.62f, 0.76f, 1f, 1f);

		private static readonly Color InkGlow = new Color(0.55f, 0.34f, 0.95f, 1f);

		private static Vector2 _heldHilt;

		private static ulong _heldHiltAt;

		private static bool _heldHiltSet;

		private static readonly Dictionary<CardModel, HeldHilt> HeldHilts = new Dictionary<CardModel, HeldHilt>(ReferenceEqualityComparer.Instance);

		public static bool Enabled => Tuning.F("fan.enable", 1f) >= 0.5f;

		public static bool Active(Creature owner)
		{
			if (owner == null || !Enabled)
			{
				return false;
			}
			try
			{
				return owner.HasPower<FanOfKnivesPower>();
			}
			catch
			{
				return false;
			}
		}

		public static bool Active(CardModel card)
		{
			if (card is Shiv)
			{
				Player owner = card.Owner;
				return Active((owner != null) ? owner.Creature : null);
			}
			return false;
		}

		private static bool IsInk(CardModel card)
		{
			try
			{
				return ((card != null) ? card.Enchantment : null) is Inky;
			}
			catch
			{
				return false;
			}
		}

		private static Play Find(CardModel card)
		{
			if (card == null)
			{
				return null;
			}
			if (!MultiplayerFx.Active && MultiplayerFx.CurrentInvocation != null)
			{
				return null;
			}
			if (MultiplayerFx.Active && MultiplayerFx.CurrentInvocation == null)
			{
				return null;
			}
			for (int num = Plays.Count - 1; num >= 0; num--)
			{
				Play play = Plays[num];
				if (play.Card == card)
				{
					if (!MultiplayerFx.Active)
					{
						if (!play.Scoped)
						{
							return play;
						}
					}
					else if (play.Scoped)
					{
						Creature owner = play.Owner;
						Player owner2 = card.Owner;
						if (owner == ((owner2 != null) ? owner2.Creature : null) && play.Room == GameRefs.CombatRoom && play.Invocation == MultiplayerFx.CurrentInvocation)
						{
							return play;
						}
					}
				}
			}
			return null;
		}

		private static Play Open(CardModel card)
		{
			if (card == null || (MultiplayerFx.Active && MultiplayerFx.CurrentInvocation == null) || (!MultiplayerFx.Active && MultiplayerFx.CurrentInvocation != null))
			{
				return null;
			}
			if (MultiplayerFx.Active && card != MultiplayerFx.CurrentCard)
			{
				return null;
			}
			Play play = Find(card);
			if (MultiplayerFx.Active && play != null)
			{
				return play;
			}
			if (!MultiplayerFx.Active)
			{
				for (int num = Plays.Count - 1; num >= 0; num--)
				{
					if (!Plays[num].Scoped && Plays[num].Card == card)
					{
						Plays.RemoveAt(num);
					}
				}
				if (Plays.Count > 8)
				{
					Plays.RemoveAt(0);
				}
			}
			Play play2 = new Play
			{
				Card = card
			};
			Player owner = card.Owner;
			play2.Owner = ((owner != null) ? owner.Creature : null);
			play2.Invocation = (MultiplayerFx.Active ? MultiplayerFx.CurrentInvocation : null);
			play2.Room = GameRefs.CombatRoom;
			play2.Scoped = MultiplayerFx.Active;
			play2.Remote = MultiplayerFx.Active && !MultiplayerFx.IsLocal(card);
			Play play3 = play2;
			Plays.Add(play3);
			return play3;
		}

		public static bool Begin(CardPlay play)
		{
			CardModel val = ((play != null) ? play.Card : null);
			if (val == null || !Active(val))
			{
				return false;
			}
			if (MultiplayerFx.Active && !MultiplayerFx.IsLocal(val))
			{
				Player owner = val.Owner;
				if (((owner != null) ? owner.Creature : null) == null)
				{
					return false;
				}
			}
			return Open(val) != null;
		}

		public static void LaunchPending(CardPlay play)
		{
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			CardModel val = ((play != null) ? play.Card : null);
			Play play2 = Find(val);
			if (play2 == null || play2.Launched)
			{
				return;
			}
			NCombatRoom combatRoom = GameRefs.CombatRoom;
			using (RemoteFxFade.Capture(MultiplayerFx.Active && play2.Remote, (Node)((combatRoom != null) ? combatRoom.BackCombatVfxContainer : null), (Node)((combatRoom != null) ? combatRoom.CombatVfxContainer : null), GameRefs.Overlay, (Node)combatRoom))
			{
				try
				{
					if (KnifeTrapFx.TakeVolleyKnife(out var from, out var rot, out var index, out var last, out var hanging, out var pile))
					{
						play2.Last = last;
						LaunchFromPile(play2, from, rot, index, hanging, pile);
						return;
					}
					if (PhantomBladesFx.TryTakeForFan(val, out var blade))
					{
						Launch(val, new List<(Node2D, object)> { (blade, null) }, blade.GlobalPosition, spectral: true);
						return;
					}
					Player owner = val.Owner;
					CardFxContext cardFxContext = CardFxManager.BuildContext((owner != null) ? owner.Creature : null, null);
					if (cardFxContext != null)
					{
						Launch(val, null, cardFxContext.WeaponHand + new Vector2(10f * cardFxContext.Facing, -14f), spectral: false);
					}
				}
				catch (Exception ex)
				{
					DevLog.Error("ShivFan.LaunchPending: " + ex);
				}
			}
		}

		public static void Finish(CardModel card)
		{
			if (card == null)
			{
				return;
			}
			Play play = Find(card);
			if ((MultiplayerFx.Active && play == null) || (!MultiplayerFx.Active && MultiplayerFx.CurrentInvocation != null))
			{
				return;
			}
			bool flag = false;
			for (int num = Plays.Count - 1; num >= 0; num--)
			{
				if (Plays[num].Card == card && !(MultiplayerFx.Active ? (Plays[num] != play) : Plays[num].Scoped))
				{
					Plays.RemoveAt(num);
					flag = true;
				}
			}
			if (flag)
			{
				PhantomBladesFx.ForgetThrowOrigin(card);
			}
		}

		public static void Reset()
		{
			Plays.Clear();
			HeldHilts.Clear();
			_heldHiltSet = false;
		}

		public static bool HidesThrow(CardModel shiv)
		{
			if (shiv != null)
			{
				return Find(shiv) != null;
			}
			return false;
		}

		public static bool FromTrap(CardModel shiv)
		{
			return Find(shiv)?.Trap ?? false;
		}

		public static bool Bursts(CardModel shiv)
		{
			return Find(shiv)?.Through ?? false;
		}

		public static void RedirectThrow(Creature owner, ref Creature target)
		{
			Play play = Find(ShivFx.CurrentShiv());
			if (play == null)
			{
				return;
			}
			object obj;
			if (owner == null)
			{
				obj = null;
			}
			else
			{
				ICombatState combatState = owner.CombatState;
				obj = ((combatState != null) ? combatState.HittableEnemies : null);
			}
			IReadOnlyList<Creature> readOnlyList = (IReadOnlyList<Creature>)obj;
			if (readOnlyList != null && readOnlyList.Count != 0)
			{
				Creature val = readOnlyList[play.Calls++ % readOnlyList.Count];
				if (val != null)
				{
					target = val;
				}
			}
		}

		public static bool HitOf(CardModel shiv, Creature target, out Vector2? from, out bool fromTrap, out float wait)
		{
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			from = null;
			fromTrap = false;
			wait = 0f;
			Play play = Find(shiv);
			if (play == null)
			{
				return false;
			}
			fromTrap = play.Trap;
			Vector2 from2;
			if (target != null && play.Knives.TryGetValue(target, out var value))
			{
				from = value.From;
				ulong milliseconds = FxClock.Milliseconds;
				if (value.Arrive > milliseconds)
				{
					wait = Mathf.Min((float)(value.Arrive - milliseconds) / 1000f, Mathf.Max(0f, Tuning.F("fan.hit_wait", 0.35f)));
				}
			}
			else if (PhantomBladesFx.PeekThrowOrigin(shiv, out from2))
			{
				from = from2;
			}
			return true;
		}

		public static bool TakeVeil(CardModel shiv)
		{
			Play play = Find(shiv);
			if (play == null)
			{
				return true;
			}
			if (play.VeilLaid)
			{
				return false;
			}
			play.VeilLaid = true;
			return true;
		}

		public static void NoteHeldHilt(Vector2 at)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			if (MultiplayerFx.Active)
			{
				CardModel currentCard = MultiplayerFx.CurrentCard;
				if (currentCard != null)
				{
					NoteHeldHilt(currentCard, at);
				}
			}
			else if (MultiplayerFx.CurrentInvocation == null)
			{
				_heldHilt = at;
				_heldHiltAt = FxClock.Milliseconds;
				_heldHiltSet = true;
			}
		}

		public static void NoteHeldHilt(CardModel card, Vector2 at)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			if (!MultiplayerFx.Active)
			{
				NoteHeldHilt(at);
				return;
			}
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
			if (obj != null && GameRefs.CombatRoom != null)
			{
				HeldHilts[card] = new HeldHilt
				{
					Position = at,
					At = FxClock.Milliseconds,
					Owner = card.Owner.Creature,
					Room = GameRefs.CombatRoom
				};
			}
		}

		public static bool TryHeldHilt(out Vector2 at)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			if (MultiplayerFx.Active)
			{
				CardModel currentCard = MultiplayerFx.CurrentCard;
				if (currentCard != null && TryHeldHilt(currentCard, out at))
				{
					return true;
				}
				at = default;
				return false;
			}
			at = _heldHilt;
			if (MultiplayerFx.CurrentInvocation == null && _heldHiltSet)
			{
				return FxClock.Milliseconds - _heldHiltAt < 250;
			}
			return false;
		}

		public static bool TryHeldHilt(CardModel card, out Vector2 at)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			at = default;
			if (!MultiplayerFx.Active)
			{
				return TryHeldHilt(out at);
			}
			if (card == null || !HeldHilts.TryGetValue(card, out var value))
			{
				return false;
			}
			at = value.Position;
			Creature owner = value.Owner;
			Player owner2 = card.Owner;
			if (owner != ((owner2 != null) ? owner2.Creature : null) || value.Room != GameRefs.CombatRoom || FxClock.Milliseconds - value.At >= 250)
			{
				HeldHilts.Remove(card);
				return false;
			}
			return true;
		}

		public static void FillEnemies(Creature owner, List<FxTarget> into)
		{
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			try
			{
				NCombatRoom combatRoom = GameRefs.CombatRoom;
				ICombatState obj = ((owner != null) ? owner.CombatState : null);
				IReadOnlyList<Creature> readOnlyList = ((obj != null) ? obj.HittableEnemies : null);
				if (combatRoom != null && readOnlyList != null)
				{
					for (int i = 0; i < readOnlyList.Count; i++)
					{
						Creature val = readOnlyList[i];
						if (val == null || !val.IsAlive)
						{
							continue;
						}
						NCreature creatureNode = combatRoom.GetCreatureNode(val);
						if (FxKit.Alive((GodotObject)(object)creatureNode) && ((Node)creatureNode).IsInsideTree())
						{
							FxTarget fxTarget;
							if (num < into.Count)
							{
								fxTarget = into[num];
							}
							else
							{
								fxTarget = new FxTarget();
								into.Add(fxTarget);
							}
							fxTarget.Key = val;
							fxTarget.Chest = creatureNode.VfxSpawnPosition;
							fxTarget.Feet = GameRefs.BottomOf(creatureNode);
							num++;
						}
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("ShivFan: could not list the enemies: " + ex.Message);
			}
			if (num < into.Count)
			{
				into.RemoveRange(num, into.Count - num);
			}
		}

		public static float Spread(Vector2 hilt, List<FxTarget> targets, List<float> angles, float fallback)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			angles.Clear();
			int count = targets.Count;
			if (count == 0)
			{
				return fallback;
			}
			Vector2 val = Vector2.Zero;
			foreach (FxTarget target in targets)
			{
				Vector2 val2 = target.Chest - hilt;
				if (val2.LengthSquared() > 1f)
				{
					val += val2.Normalized();
				}
			}
			float num = ((val.LengthSquared() > 0.0001f) ? val.Angle() : fallback);
			_sortHilt = hilt;
			_sortMiddle = num;
			targets.Sort(ByAngle);
			foreach (FxTarget target2 in targets)
			{
				angles.Add(Off(hilt, num, target2.Chest));
			}
			float num2 = Mathf.DegToRad(Mathf.Clamp(Tuning.F("fan.spread", 11f), 0f, 60f));
			float num3 = (angles[0] + angles[count - 1]) * 0.5f;
			for (int i = 1; i < count; i++)
			{
				if (angles[i] < angles[i - 1] + num2)
				{
					angles[i] = angles[i - 1] + num2;
				}
			}
			float num4 = num3 - (angles[0] + angles[count - 1]) * 0.5f;
			for (int j = 0; j < count; j++)
			{
				angles[j] = num + angles[j] + num4;
			}
			return num;
		}

		private static float Off(Vector2 hilt, float middle, Vector2 chest)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = chest - hilt;
			return Mathf.Wrap(val.Angle() - middle, -(float)Math.PI, (float)Math.PI);
		}

		public static void Launch(CardModel shiv, List<(Node2D Node, object Key)> ready, Vector2 hilt, bool spectral)
		{
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			Play play = Find(shiv) ?? Open(shiv);
			if (play == null)
			{
				Discard(ready, 0);
				return;
			}
			object obj;
			if (shiv == null)
			{
				obj = null;
			}
			else
			{
				Player owner = shiv.Owner;
				obj = ((owner != null) ? owner.Creature : null);
			}
			Creature val = (Creature)obj;
			CardFxContext cardFxContext = (play.Launched ? null : CardFxManager.BuildContext(val, null));
			if (cardFxContext == null || !FxKit.Alive((GodotObject)(object)cardFxContext.FrontParent))
			{
				Discard(ready, 0);
				return;
			}
			play.Launched = true;
			if (play.Remote)
			{
				cardFxContext.Remote = true;
			}
			bool ink = IsInk(shiv);
			FillEnemies(val, play.Enemies);
			Spread(hilt, play.Enemies, Angles, (cardFxContext.Facing >= 0f) ? 0f : ((float)Math.PI));
			if (spectral)
			{
				PhantomBladesFx.RememberThrowOrigin(shiv, hilt);
			}
			Node frontParent = cardFxContext.FrontParent;
			bool active = TimeFreeze.Active;
			ulong milliseconds = FxClock.Milliseconds;
			float num = Mathf.Max(0.04f, Tuning.F("fan.fly_time", 0.12f));
			float num2 = Mathf.Max(0f, Tuning.F("fan.stagger", 0.03f));
			int num3 = ready?.Count ?? 0;
			bool[] array = new bool[num3];
			Color trail = TrailColor(spectral, ink, trap: false);
			for (int i = 0; i < play.Enemies.Count; i++)
			{
				object key = play.Enemies[i].Key;
				Creature val2 = (Creature)((key is Creature) ? key : null);
				Vector2 chest = play.Enemies[i].Chest;
				float num4 = (float)i * num2;
				if (val2 != null)
				{
					play.Knives[val2] = new Knife
					{
						From = hilt,
						Arrive = (active ? 0 : (milliseconds + (ulong)((num4 + num) * 1000f)))
					};
				}
				if (active)
				{
					TimeFreeze.FloatKnife(hilt, chest, ink);
					continue;
				}
				Node2D val3 = Pick(play, ready, array, val2);
				if (val3 == null)
				{
					val3 = MakeKnife(spectral, ink);
					frontParent.AddChild((Node)(object)val3, false, (InternalMode)0);
					val3.GlobalPosition = hilt;
					Node2D val4 = val3;
					float num5;
					if (i >= Angles.Count)
					{
						Vector2 val5 = chest - hilt;
						num5 = val5.Angle();
					}
					else
					{
						num5 = Angles[i];
					}
					val4.GlobalRotation = num5 + (float)Math.PI / 2f;
				}
				else
				{
					Adopt(val3, frontParent);
				}
				((CanvasItem)val3).Visible = true;
				val3.Scale = Vector2.One;
				((CanvasItem)val3).Modulate = Colors.White;
				Fly(frontParent, val3, chest, num4, num, trail, val2);
			}
			for (int j = 0; j < num3; j++)
			{
				if (!array[j])
				{
					Fade(ready[j].Node);
				}
			}
			if (PowerFxManager.LogTriggers)
			{
				DevLog.Info($"Shiv FX: Fan of Knives, {play.Enemies.Count} knife(s){(spectral ? ", spectral" : "")}{(active ? ", in stopped time" : "")}");
			}
		}

		private static void LaunchFromPile(Play p, Vector2 from, float turned, int index, bool hanging, Vector2 pile)
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0427: Unknown result type (might be due to invalid IL or missing references)
			//IL_042c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0406: Unknown result type (might be due to invalid IL or missing references)
			//IL_0443: Unknown result type (might be due to invalid IL or missing references)
			//IL_0448: Unknown result type (might be due to invalid IL or missing references)
			//IL_0465: Unknown result type (might be due to invalid IL or missing references)
			//IL_0471: Unknown result type (might be due to invalid IL or missing references)
			//IL_0477: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0481: Unknown result type (might be due to invalid IL or missing references)
			//IL_0494: Unknown result type (might be due to invalid IL or missing references)
			//IL_049a: Unknown result type (might be due to invalid IL or missing references)
			//IL_049f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
			CardModel card = p.Card;
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
			CardFxContext cardFxContext = CardFxManager.BuildContext(val, null);
			if (cardFxContext == null)
			{
				return;
			}
			bool flag = MultiplayerFx.Active && p.Remote;
			if (flag)
			{
				cardFxContext.Remote = true;
				pile = cardFxContext.WeaponHand + new Vector2(10f * cardFxContext.Facing, -14f);
				from = pile;
				hanging = false;
			}
			p.Launched = true;
			p.Trap = true;
			bool ink = IsInk(card);
			FillEnemies(val, p.Enemies);
			int count = p.Enemies.Count;
			if (count == 0)
			{
				return;
			}
			int num = -1;
			if (hanging)
			{
				float num2 = float.MaxValue;
				for (int i = 0; i < count; i++)
				{
					Vector2 val2 = p.Enemies[i].Chest - from;
					if (!(val2.LengthSquared() < 1f))
					{
						float num3 = Mathf.Abs(Mathf.Wrap(val2.Angle() - (turned - (float)Math.PI / 2f), -(float)Math.PI, (float)Math.PI));
						if (num3 < num2)
						{
							num2 = num3;
							num = i;
						}
					}
				}
				if (num < 0)
				{
					num = 0;
				}
			}
			float num4 = Mathf.Max(0.04f, Tuning.F("fan.fly_time", 0.12f));
			float num5 = Mathf.Max(0f, Tuning.F("fan.stagger", 0.03f)) * (float)Math.Max(1, count - 1);
			bool active = TimeFreeze.Active;
			ulong milliseconds = FxClock.Milliseconds;
			Vector2[] array = new Vector2[count];
			float[] array2 = new float[count];
			int num6 = 0;
			Vector2 val3 = Vector2.Zero;
			for (int j = 0; j < count; j++)
			{
				val3 += p.Enemies[j].Chest;
				float num7 = FxKit.Hash(index * 31 + j * 7 + 3) * ((float)Math.PI * 2f);
				float num8 = 6f + 14f * FxKit.Hash(index * 31 + j * 7 + 4);
				array[j] = ((j == num) ? from : (pile + new Vector2(Mathf.Cos(num7), Mathf.Sin(num7)) * num8));
				array2[j] = ((j == num) ? 0f : (0.015f + num5 * FxKit.Hash(index * 31 + j * 7 + 5)));
				if (array2[j] < array2[num6])
				{
					num6 = j;
				}
				object key = p.Enemies[j].Key;
				Creature val4 = (Creature)((key is Creature) ? key : null);
				if (val4 != null)
				{
					p.Knives[val4] = new Knife
					{
						From = array[j],
						Arrive = (active ? 0 : (milliseconds + (ulong)((array2[j] + num4) * 1000f)))
					};
				}
				if (active)
				{
					TimeFreeze.FloatKnife(array[j], p.Enemies[j].Chest, ink);
				}
			}
			val3 /= (float)count;
			if (active)
			{
				return;
			}
			Node val5;
			if (flag)
			{
				val5 = cardFxContext.FrontParent;
			}
			else
			{
				val5 = (FxKit.Alive((GodotObject)(object)cardFxContext.UiParent) ? cardFxContext.UiParent : cardFxContext.FrontParent);
			}
			if (!FxKit.Alive((GodotObject)(object)val5))
			{
				return;
			}
			Node frontParent = cardFxContext.FrontParent;
			p.Through = FxKit.Alive((GodotObject)(object)frontParent);
			bool last = p.Last;
			Vector2 val6 = val3 - pile;
			Vector2 val7;
			if (!(val6.LengthSquared() > 1f))
			{
				val7 = new Vector2(-0.5f, -0.87f);
			}
			else
			{
				val6 = val3 - pile;
				val7 = val6.Normalized();
			}
			Vector2 way = val7;
			if (!flag && count > ((num >= 0) ? 1 : 0))
			{
				Erupt(val5, pile, way, last);
			}
			if (flag)
			{
				Sfx.Play("trap_erupt", last ? 1.25f : 1f);
			}
			if (num >= 0)
			{
				FxKit.Flash(val5, from, new Color(KnifeTrapFx.Ember.R, KnifeTrapFx.Ember.G, KnifeTrapFx.Ember.B, 0.65f), 70f, 1.8f, 0.14);
			}
			Color trail = TrailColor(spectral: false, ink, trap: true);
			for (int k = 0; k < count; k++)
			{
				Vector2 chest = p.Enemies[k].Chest;
				Node2D val8 = ShivFx.MakeShiv(ink);
				val5.AddChild((Node)(object)val8, false, (InternalMode)0);
				val8.GlobalPosition = array[k];
				val6 = chest - array[k];
				float globalRotation;
				if (!(val6.LengthSquared() > 1f))
				{
					globalRotation = turned;
				}
				else
				{
					val6 = chest - array[k];
					globalRotation = val6.Angle() + (float)Math.PI / 2f;
				}
				val8.GlobalRotation = globalRotation;
				((CanvasItem)val8).Visible = false;
				float delay = array2[k];
				object key2 = p.Enemies[k].Key;
				Fly(val5, val8, chest, delay, num4, trail, (Creature)((key2 is Creature) ? key2 : null), frontParent, k == num6, last, flag);
			}
			if (PowerFxManager.LogTriggers)
			{
				DevLog.Info($"Shiv FX: Knife Trap under Fan of Knives, {count} knife(s){((num >= 0) ? " (one from a hanging blade)" : "")}{(last ? ", the last" : "")}");
			}
		}

		private static void Erupt(Node parent, Vector2 pile, Vector2 way, bool big)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			Color ember = KnifeTrapFx.Ember;
			Sfx.Play("trap_erupt", big ? 1.25f : 1f);
			FxKit.Flash(parent, pile, new Color(ember.R, ember.G, ember.B, 0.85f), big ? 260f : 150f, big ? 2.1f : 1.9f, big ? 0.26 : 0.18);
			FxKit.Flash(parent, pile, new Color(1f, 0.97f, 0.9f, 0.9f), big ? 100f : 60f, 2.2f, 0.12);
			FxKit.RingPulse(parent, pile, big ? 240f : 150f, ember, 0f, big ? 0.38f : 0.28f, 0.35f, 1.25f, 1f, 0f, 0.045f, 0.9f);
			FxKit.Burst(parent, pile, new Color(1f, 0.8f, 0.52f, 0.95f), big ? 28 : 14, 220f, big ? 900f : 700f, 0.35, 0.05f, 0.16f, new Vector2(0f, 600f), big ? 140f : 55f, way, 12f);
			KnifeTrapFx.Splinters(parent, pile, way, Math.Max(0, Tuning.I(big ? "fan.trap_final_spray" : "fan.trap_spray", big ? 8 : 3)), big ? 170f : 85f, 0.9f);
		}

		private static void Fly(Node parent, Node2D knife, Vector2 to, float delay, float time, Color trail, Creature target, Node blastIn = null, bool shake = false, bool big = false, bool sourceFade = false)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)parent) || !FxKit.Alive((GodotObject)(object)knife))
			{
				return;
			}
			bool through = blastIn != null;
			if (sourceFade)
			{
				((CanvasItem)knife).Modulate = new Color(1f, 1f, 1f, 0f);
			}
			float exit = (through ? Mathf.Clamp(Tuning.F("fan.trap_exit", 0.16f), 0.04f, 0.6f) : 0f);
			float total = time + exit;
			FxKit.FreeAfter((Node)(object)knife, delay + total + 1.2f);
			Line2D line = FxKit.Ribbon(10f, trail);
			parent.AddChild((Node)(object)line, false, (InternalMode)0);
			FxKit.FreeAfter((Node)(object)line, delay + total + 0.6f);
			Vector2 from = knife.GlobalPosition;
			float startRot = knife.GlobalRotation;
			Vector2 val = to - from;
			float num;
			if (!(val.LengthSquared() > 1f))
			{
				num = startRot;
			}
			else
			{
				val = to - from;
				num = val.Angle() + (float)Math.PI / 2f;
			}
			float flyRot = num;
			val = to - from;
			Vector2 val2;
			if (!(val.LengthSquared() > 1f))
			{
				val2 = new Vector2(1f, 0f);
			}
			else
			{
				val = to - from;
				val2 = val.Normalized();
			}
			Vector2 way = val2;
			float beyond = Mathf.Clamp(from.DistanceTo(to) * exit / Mathf.Max(0.01f, time), 180f, 420f);
			bool dropped = false;
			bool burst = false;
			Tween val3 = ((Node)knife).CreateTween();
			if (delay > 0.001f)
			{
				val3.TweenInterval((double)delay);
			}
			val3.TweenMethod(Callable.From<float>((Action<float>)((float t) =>
			{
				//IL_0087: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
				//IL_027b: Unknown result type (might be due to invalid IL or missing references)
				//IL_029a: Unknown result type (might be due to invalid IL or missing references)
				//IL_029b: Unknown result type (might be due to invalid IL or missing references)
				//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0203: Unknown result type (might be due to invalid IL or missing references)
				//IL_0224: Unknown result type (might be due to invalid IL or missing references)
				//IL_0229: Unknown result type (might be due to invalid IL or missing references)
				//IL_022e: Unknown result type (might be due to invalid IL or missing references)
				//IL_026b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0184: Unknown result type (might be due to invalid IL or missing references)
				//IL_018a: Unknown result type (might be due to invalid IL or missing references)
				if (dropped || !FxKit.Alive((GodotObject)(object)knife))
				{
					return;
				}
				try
				{
					if (!((CanvasItem)knife).Visible)
					{
						((CanvasItem)knife).Visible = true;
					}
					float num2 = t * total;
					float num3 = Mathf.Clamp(num2 / time, 0f, 1f);
					if (sourceFade && num2 < time)
					{
						((CanvasItem)knife).Modulate = new Color(1f, 1f, 1f, Mathf.SmoothStep(0f, 0.2f, num3));
					}
					if (num3 < 0.5f && target != null && !target.IsAlive)
					{
						dropped = true;
						Drop(knife);
					}
					else
					{
						Vector2 val5;
						if (!through || num2 < time)
						{
							val5 = from.Lerp(to, num3 * num3);
							knife.GlobalRotation = Mathf.LerpAngle(startRot, flyRot, Mathf.Min(1f, num3 * 4f));
						}
						else
						{
							if (!burst)
							{
								burst = true;
								int shake2 = (shake ? Tuning.I(big ? "trap.final_shake" : "trap.shake", (!big) ? 1 : 2) : 0);
								using (RemoteFxFade.Capture(sourceFade, blastIn))
								{
									KnifeTrapFx.Blast(blastIn, to, way, big, shake2);
									if (shake & big)
									{
										Sfx.Play("trap_final");
									}
								}
							}
							float num4 = Mathf.Clamp((num2 - time) / exit, 0f, 1f);
							val5 = to + way * (beyond * (1f - (1f - num4) * (1f - num4)));
							knife.GlobalRotation = flyRot;
							((CanvasItem)knife).Modulate = new Color(1f, 1f, 1f, 1f - Mathf.SmoothStep(0.45f, 1f, num4));
						}
						knife.GlobalPosition = val5;
						if (FxKit.Alive((GodotObject)(object)line))
						{
							line.AddPoint(((Node2D)line).ToLocal(val5), -1);
						}
					}
				}
				catch (Exception ex)
				{
					dropped = true;
					DevLog.Warn("ShivFan (flight): " + ex.Message);
				}
			})), Variant.op_Implicit(0f), Variant.op_Implicit(1f), (double)total);
			val3.TweenCallback(Callable.From((Action)(() =>
			{
				if (!dropped && FxKit.Alive((GodotObject)(object)knife))
				{
					((Node)knife).QueueFree();
				}
			})));
			if (!FxKit.Alive((GodotObject)(object)line))
			{
				return;
			}
			Tween val4 = ((Node)line).CreateTween();
			val4.TweenInterval((double)(delay + total * 0.8f));
			val4.TweenProperty((GodotObject)(object)line, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.18);
			val4.TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)line))
				{
					((Node)line).QueueFree();
				}
			})));
		}

		private static void Drop(Node2D knife)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)knife))
			{
				return;
			}
			Vector2 from = knife.GlobalPosition;
			float rot = knife.GlobalRotation;
			float spin = ((FxKit.Hash((int)(from.X * 0.37f + from.Y)) > 0.5f) ? 1f : (-1f));
			FxKit.FreeAfter((Node)(object)knife, 0.7);
			FxKit.Drive((Node)(object)knife, 0.4f, (float t) =>
			{
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_007c: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)knife))
				{
					knife.GlobalPosition = from + new Vector2(22f * spin * t, 110f * t * t);
					knife.GlobalRotation = rot + spin * 1.8f * t;
					((CanvasItem)knife).Modulate = new Color(1f, 1f, 1f, 1f - t);
				}
			}, (TransitionType)0, (EaseType)2).TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)knife))
				{
					((Node)knife).QueueFree();
				}
			})));
		}

		private static void Fade(Node2D blade)
		{
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)blade))
			{
				return;
			}
			FxKit.FreeAfter((Node)(object)blade, 0.4);
			Tween val = ((Node)blade).CreateTween();
			val.TweenProperty((GodotObject)(object)blade, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.15);
			val.TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)blade))
				{
					((Node)blade).QueueFree();
				}
			})));
		}

		private static void Discard(List<(Node2D Node, object Key)> ready, int from)
		{
			if (ready != null)
			{
				for (int i = from; i < ready.Count; i++)
				{
					Fade(ready[i].Node);
				}
			}
		}

		private static Node2D Pick(Play play, List<(Node2D Node, object Key)> ready, bool[] used, Creature enemy)
		{
			if (ready == null)
			{
				return null;
			}
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < ready.Count; j++)
				{
					if (!used[j] && FxKit.Alive((GodotObject)(object)ready[j].Node) && (i != 0 || (enemy != null && ready[j].Key == enemy)) && (i != 1 || ready[j].Key == null || !IsListed(play, ready[j].Key)))
					{
						used[j] = true;
						return ready[j].Node;
					}
				}
			}
			return null;
		}

		private static bool IsListed(Play current, object key)
		{
			foreach (Play play in Plays)
			{
				if ((MultiplayerFx.Active && play != current) || (!MultiplayerFx.Active && play.Scoped))
				{
					continue;
				}
				foreach (FxTarget enemy in play.Enemies)
				{
					if (enemy.Key == key)
					{
						return true;
					}
				}
			}
			return false;
		}

		private static Node2D MakeKnife(bool spectral, bool ink)
		{
			if (!spectral)
			{
				return ShivFx.MakeShiv(ink);
			}
			return PhantomBladesFx.MakeSpectral(ink);
		}

		private static Color TrailColor(bool spectral, bool ink, bool trap)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			Color val;
			if (trap)
			{
				val = KnifeTrapFx.Ember;
			}
			else if (spectral)
			{
				val = Spectral;
			}
			else
			{
				val = (ink ? InkGlow : Steel);
			}
			return new Color(val.R, val.G, val.B, 0.75f);
		}

		private static void Adopt(Node2D node, Node parent)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			if (((Node)node).GetParent() != parent)
			{
				Vector2 globalPosition = node.GlobalPosition;
				float globalRotation = node.GlobalRotation;
				Node parent2 = ((Node)node).GetParent();
				if (parent2 != null)
				{
					parent2.RemoveChild((Node)(object)node);
				}
				parent.AddChild((Node)(object)node, false, (InternalMode)0);
				node.GlobalPosition = globalPosition;
				node.GlobalRotation = globalRotation;
			}
		}

		static ShivFan()
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		}
	}
