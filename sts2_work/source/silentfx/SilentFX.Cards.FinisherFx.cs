// namespace: SilentFX.Cards
// type: FinisherFx (class)
// source lines: 57558-58917
	public sealed class FinisherFx : CardFx
	{
		private sealed class Loose
		{
			public FinisherCard Rig;

			public ulong At;

			public Action Blow;
		}

		private sealed class RemotePlay
		{
			internal bool Ended;

			internal bool Striking;

			internal ulong Since;

			internal Node Room;
		}

		private sealed class Sink
		{
			public Creature Who;

			public Node2D Picture;

			public Vector2 Home;

			public Vector2 Wrote;

			public Vector2 Shift;

			public bool Over;
		}

		internal static readonly Color Steel = new Color(0.92f, 0.96f, 1f, 1f);

		internal static readonly Color Chill = new Color(0.6f, 0.8f, 1f, 1f);

		internal static readonly Color End = new Color(1f, 0.16f, 0.2f, 1f);

		internal static readonly Color Hone = new Color(1f, 0.8f, 0.42f, 1f);

		private static CardModel _ended;

		private static readonly ConditionalWeakTable<CardModel, RemotePlay> Plays = new ConditionalWeakTable<CardModel, RemotePlay>();

		private static CardModel _striking;

		private static ulong _strikingSince;

		private static Node _strikingRoom;

		private static readonly List<Loose> Darts = new List<Loose>();

		private static readonly List<CardModel> PlayedScratch = new List<CardModel>();

		private static readonly List<Sink> Sinks = new List<Sink>();

		private const float Thick = 0.3f;

		private const float Shock1 = 0.36f;

		private const float Shock2 = 0.62f;

		private const float SlamTime = 1.15f;

		private static RemotePlay CurrentPlay()
		{
			CardModel currentCard = MultiplayerFx.CurrentCard;
			if (currentCard == null || !Plays.TryGetValue(currentCard, out var value) || !value.Striking)
			{
				return null;
			}
			if ((object)value.Room != GameRefs.CombatRoom || FxClock.Milliseconds - value.Since > 30000)
			{
				value.Striking = false;
				return null;
			}
			return value;
		}

		private static bool Striking()
		{
			if (_striking == null)
			{
				return false;
			}
			if ((object)_strikingRoom != GameRefs.CombatRoom || FxClock.Milliseconds - _strikingSince > 30000)
			{
				_striking = null;
				_strikingRoom = null;
				return false;
			}
			return true;
		}

		public static bool Replaces(Creature creature)
		{
			if (MultiplayerFx.Active)
			{
				if (creature != null && CurrentPlay() != null && Tuning.F("finisher.keep_swing", 0f) < 0.5f)
				{
					Player owner = MultiplayerFx.CurrentCard.Owner;
					return ((owner != null) ? owner.Creature : null) == creature;
				}
				return false;
			}
			if (creature != null && Striking() && Tuning.F("finisher.keep_swing", 0f) < 0.5f)
			{
				Player owner2 = _striking.Owner;
				return ((owner2 != null) ? owner2.Creature : null) == creature;
			}
			return false;
		}

		public static bool CoversStab()
		{
			if (MultiplayerFx.Active ? (CurrentPlay() != null) : Striking())
			{
				return Tuning.F("finisher.keep_stab", 0f) < 0.5f;
			}
			return false;
		}

		public static void NoteCardFinished(CardModel card)
		{
			if (card != null && Plays.TryGetValue(card, out var value))
			{
				value.Striking = false;
			}
			if (card != null && card == _striking)
			{
				_striking = null;
			}
		}

		public static void Reset()
		{
			Plays.Clear();
			_ended = null;
			_striking = null;
			_strikingRoom = null;
			while (Darts.Count > 0)
			{
				FreeDart(Darts[Darts.Count - 1]);
			}
			PlayedScratch.Clear();
			Sink[] array = Sinks.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				LetGo(array[i]);
			}
			Sinks.Clear();
		}

		private static Color Tint(Color c, float alpha)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			return new Color(c.R, c.G, c.B, Mathf.Clamp(alpha, 0f, 1f));
		}

		internal static int Expected(CardModel card)
		{
			if (card == null)
			{
				return 1;
			}
			try
			{
				return Mathf.Max(1, TurnLog.Count(card.Owner, (CardType)1));
			}
			catch (Exception ex)
			{
				DevLog.Warn("FinisherFx.Expected: " + ex.Message);
				return 1;
			}
		}

		internal static float Across(CardFxContext c)
		{
			return Mathf.Clamp(c.TargetWidth * 0.8f, 150f, 700f);
		}

		public override void OnPlay(CardFxContext c)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			Tally.Begin(c.Card, c.Chest);
			if (MultiplayerFx.Active && c.Card != null)
			{
				RemotePlay value = Plays.GetValue(c.Card, (CardModel _) => new RemotePlay());
				value.Ended = false;
				value.Striking = true;
				value.Since = FxClock.Milliseconds;
				value.Room = (Node)(object)GameRefs.CombatRoom;
			}
			else
			{
				if (c.Card != null && _ended == c.Card)
				{
					_ended = null;
				}
				_striking = c.Card;
				_strikingSince = FxClock.Milliseconds;
				_strikingRoom = (Node)(object)GameRefs.CombatRoom;
			}
			Sweep();
		}

		public override void OnHit(CardFxContext c, Vector2 at, bool killed)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			Node frontParent = c.FrontParent;
			if (!FxKit.Alive((GodotObject)(object)frontParent))
			{
				return;
			}
			int num = Tally.Hit(c.Card, at, c.Chest, out var _);
			int num2 = Expected(c.Card);
			RemotePlay remotePlay = ((MultiplayerFx.Active && c.Card != null) ? Plays.GetValue(c.Card, (CardModel _) => new RemotePlay()) : null);
			int num3;
			if (remotePlay == null)
			{
				if (c.Card == null)
				{
					num3 = 0;
					goto IL_0099;
				}
				num3 = ((_ended == c.Card) ? 1 : 0);
			}
			else
			{
				num3 = (remotePlay.Ended ? 1 : 0);
			}
			if (num3 == 0)
			{
				goto IL_0099;
			}
			int num4 = 0;
			goto IL_00ab;
			IL_00ab:
			bool flag = (byte)num4 != 0;
			if (flag)
			{
				if (remotePlay != null)
				{
					remotePlay.Ended = true;
				}
				else
				{
					_ended = c.Card;
				}
			}
			bool flag2 = false;
			if (num3 == 0)
			{
				try
				{
					flag2 = ((HeldFxManager.PlayingFor(c.Card) as FinisherHeld) ?? (c.Held as FinisherHeld))?.Strike(c, at, num, flag) ?? Dart(c, at, num, flag, num2);
				}
				catch (Exception ex)
				{
					DevLog.Warn("FinisherFx: " + ex.Message);
				}
			}
			if (flag && !flag2)
			{
				Sfx.Play("finisher_last");
			}
			if (flag2)
			{
				return;
			}
			try
			{
				float k = Strokes.Scale(c);
				float num5 = Across(c) * 0.5f;
				float num6 = ((c.TargetSide < 0f) ? (-1f) : 1f);
				if (flag)
				{
					Vector2 feet = (c.HasTarget ? c.TargetFeet : (at + new Vector2(0f, 110f)));
					Finale(frontParent, c.BackParent, at, feet, num6, k, num5, num2, cross: true, c.Target);
				}
				else
				{
					float num7 = ((num % 2 == 0) ? (-0.62f) : 0.55f) + 0.12f * Mathf.Sin((float)num * 2.4f);
					Vector2 dir = new Vector2(num6 * Mathf.Cos(num7), Mathf.Sin(num7));
					Impact(frontParent, at, dir, k, num5 * 1.15f);
				}
				return;
			}
			catch (Exception ex2)
			{
				DevLog.Warn("FinisherFx (cut): " + ex2.Message);
				return;
			}
			IL_0099:
			num4 = ((killed || num >= num2 - 1) ? 1 : 0);
			goto IL_00ab;
		}

		private static void FreeDart(Loose dart)
		{
			Darts.Remove(dart);
			if (dart == null)
			{
				return;
			}
			Action blow = dart.Blow;
			dart.Blow = null;
			try
			{
				blow?.Invoke();
			}
			catch (Exception ex)
			{
				DevLog.Warn("FinisherFx (blow): " + ex.Message);
			}
			FinisherCard rig = dart.Rig;
			dart.Rig = null;
			try
			{
				rig?.Free();
			}
			catch (Exception ex2)
			{
				DevLog.Warn("FinisherFx (free): " + ex2.Message);
			}
		}

		private static void Sweep()
		{
			ulong ticksMsec = Time.GetTicksMsec();
			for (int num = Darts.Count - 1; num >= 0; num--)
			{
				if (num < Darts.Count)
				{
					Loose loose = Darts[num];
					if (loose == null || loose.Rig == null || !loose.Rig.Alive || ticksMsec - loose.At > 2500)
					{
						FreeDart(loose);
					}
				}
			}
		}

		private static bool Dart(CardFxContext c, Vector2 at, int index, bool last, int expected)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0403: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0408: Unknown result type (might be due to invalid IL or missing references)
			//IL_0516: Unknown result type (might be due to invalid IL or missing references)
			//IL_0599: Unknown result type (might be due to invalid IL or missing references)
			if (c?.Card == null || !c.HasTarget)
			{
				return false;
			}
			Node parent = c.FrontParent;
			if (!FxKit.Alive((GodotObject)(object)parent) || !parent.IsInsideTree())
			{
				return false;
			}
			if (Tuning.F("finisher.loose", 1f) < 0.5f)
			{
				return false;
			}
			Sweep();
			int num = Mathf.Clamp(Tuning.I("finisher.max", 9), 0, 12);
			if (num <= 0 || Darts.Count >= 6)
			{
				return false;
			}
			TurnLog.Played(c.Card.Owner, PlayedScratch, (CardType)1);
			int count = PlayedScratch.Count;
			if (count == 0)
			{
				return false;
			}
			if (!last && index < count - num)
			{
				return false;
			}
			CardModel val = PlayedScratch[Mathf.Clamp(last ? (count - 1) : index, 0, count - 1)];
			PlayedScratch.Clear();
			if (val == null)
			{
				return false;
			}
			FinisherCard.ReadTuning();
			FinisherCard rig = FinisherCard.Make(val, parent, parent);
			if (rig == null)
			{
				return false;
			}
			Loose dart = new Loose
			{
				Rig = rig,
				At = Time.GetTicksMsec()
			};
			Darts.Add(dart);
			try
			{
				float side = ((c.TargetSide < 0f) ? (-1f) : 1f);
				float k = Strokes.Scale(c);
				float half = Across(c) * 0.5f;
				float size = Mathf.Clamp(Tuning.F("finisher.card", 0.34f), 0.16f, 0.7f) * Mathf.Clamp(Mathf.Sqrt(Mathf.Max(c.TargetWidth, c.TargetHeight) / 260f), 0.9f, 1.3f) * (last ? Mathf.Clamp(Tuning.F("finisher.last_scale", 1.45f), 1f, 2.5f) : 1f);
				float stretch = Mathf.Clamp(Tuning.F("finisher.dart_stretch", 0.25f), 0f, 1f);
				float reach = 211f * size;
				Vector2 feet = c.TargetFeet;
				Node back = c.BackParent;
				Vector2 pin;
				Vector2 from;
				if (last)
				{
					pin = new Vector2(at.X, Mathf.Max(at.Y + 30f, feet.Y));
					from = new Vector2(at.X, at.Y - c.TargetHeight * 0.5f - 280f);
				}
				else
				{
					pin = at + new Vector2(FxKit.Hash(index * 7 + 5) - 0.5f, FxKit.Hash(index * 7 + 9) - 0.5f) * 36f;
					from = c.Chest + new Vector2((0f - side) * (c.Body.X * 0.3f + 40f), (0f - c.Body.Y) * (0.1f + 0.3f * FxKit.Hash(index * 5 + 2)));
				}
				Vector2 val2 = pin - from;
				Vector2 dir = (Vector2)((val2.LengthSquared() > 1f) ? val2.Normalized() : new Vector2(0f, 1f));
				float rotation = dir.Angle() + (float)Math.PI / 2f;
				float way = ((index % 2 == 0) ? 1f : (-1f));
				float fly = (last ? 0.13f : 0.1f);
				float sink = (last ? 0.16f : 0.06f);
				float total = fly + sink;
				bool struck = false;
				if (last)
				{
					Creature who = c.Target;
					dart.Blow = () =>
					{
						//IL_0026: Unknown result type (might be due to invalid IL or missing references)
						//IL_0031: Unknown result type (might be due to invalid IL or missing references)
						//IL_008e: Unknown result type (might be due to invalid IL or missing references)
						//IL_0099: Unknown result type (might be due to invalid IL or missing references)
						Finale(parent, back, at, feet, side, k, half, expected, cross: false, who);
						Shards(parent, at, dir, 12, 1.5f * k, heavy: true);
					};
					Sfx.PlayAt("finisher_last", fly);
				}
				rig.Put(from, rotation, size * 0.45f, 0f, way, 1f, 1f, 0f, 0f, -1f);
				FxKit.Drive((Node)(object)rig.Rig, total, (float u) =>
				{
					//IL_003d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0043: Unknown result type (might be due to invalid IL or missing references)
					//IL_0050: Unknown result type (might be due to invalid IL or missing references)
					//IL_0055: Unknown result type (might be due to invalid IL or missing references)
					//IL_005c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0061: Unknown result type (might be due to invalid IL or missing references)
					//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
					//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
					//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
					//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
					//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
					//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
					//IL_0111: Unknown result type (might be due to invalid IL or missing references)
					//IL_0117: Unknown result type (might be due to invalid IL or missing references)
					float num2 = u * total;
					float num5;
					Vector2 at2;
					float num6;
					float size2;
					float alpha;
					if (num2 < fly)
					{
						float num3 = num2 / fly;
						float num4 = num3 * num3;
						num5 = 1f + stretch * num4;
						at2 = from.Lerp(pin - dir * (reach * num5), num4);
						num6 = Mathf.Min(1.2f, num3 * 3.2f);
						size2 = size * Mathf.Lerp(0.45f, 1f, Mathf.Min(1f, num3 * 4f));
						alpha = Mathf.Min(1f, num3 * 6f);
					}
					else
					{
						if (!struck)
						{
							struck = true;
							try
							{
								if (last)
								{
									Action blow = dart.Blow;
									dart.Blow = null;
									blow?.Invoke();
								}
								else
								{
									CardCut(parent, pin, dir, k, half);
								}
							}
							catch (Exception ex2)
							{
								DevLog.Warn("FinisherFx (dart): " + ex2.Message);
							}
						}
						float num7 = Mathf.Clamp((num2 - fly) / sink, 0f, 1f);
						num5 = Mathf.Max(0.02f, (1f + stretch) * (last ? (1f - num7 * num7) : ((1f - num7) * (1f - num7))));
						at2 = pin - dir * (reach * num5);
						num6 = 1.2f;
						size2 = size;
						alpha = 1f - Mathf.SmoothStep(0.6f, 1f, num7);
					}
					rig.Put(at2, rotation, size2, num6, way, num5, 1f, alpha, Mathf.SmoothStep(0.3f, 0.9f, num6) * 1.1f, -1f);
				}, (TransitionType)0, (EaseType)2).TweenCallback(Callable.From((Action)(() =>
				{
					FreeDart(dart);
				})));
				return true;
			}
			catch (Exception ex)
			{
				DevLog.Warn("FinisherFx.Dart: " + ex.Message);
				dart.Blow = null;
				FreeDart(dart);
				return false;
			}
		}

		internal static void Impact(Node parent, Vector2 at, Vector2 dir, float k, float half)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)parent))
			{
				Vector2 val = at - dir * half;
				Vector2 to = at + dir * half;
				Brush.Cut(parent, val, to, 46f * k, Tint(End, 0.42f), 0f, 0.03f, 0.03f, 0.2f);
				Brush.Cut(parent, val, to, 20f * k, Tint(Steel, 0.95f), 0f, 0.03f, 0.04f, 0.18f);
				FxKit.Flash(parent, at, Tint(Steel, 0.75f), 70f * k, 1.9f, 0.12);
				Brush.Sparks(parent, at, Steel, 5, 320f, dir, 34f);
			}
		}

		internal static void CardCut(Node parent, Vector2 at, Vector2 dir, float k, float half)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)parent))
			{
				Vector2 val = at - dir * (half * 0.7f);
				Vector2 to = at + dir * (half * 0.9f);
				Brush.Cut(parent, val, to, 34f * k, Tint(End, 0.34f), 0f, 0.02f, 0.03f, 0.2f);
				Brush.Cut(parent, val, to, 20f * k, Tint(Hone, 0.72f), 0f, 0.02f, 0.04f, 0.18f);
				Brush.Cut(parent, val, to, 8f * k, new Color(1f, 1f, 1f, 1f), 0f, 0.02f, 0.05f, 0.16f);
				Hairline(parent, val, to, 2.4f * k, 0.24f);
				FxKit.Flash(parent, at, new Color(1f, 0.96f, 0.86f, 0.8f), 60f * k, 1.8f, 0.12);
				Brush.Sparks(parent, at, Hone, 5, 340f, dir, 30f);
				Shards(parent, at, dir, 6, k, heavy: false);
			}
		}

		private static void Hairline(Node parent, Vector2 from, Vector2 to, float width, float time)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Expected Obj, but got Unknown
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)parent))
			{
				return;
			}
			Line2D line = Props.Line(Mathf.Max(1f, width), new Color(1f, 1f, 1f, 1f), true, Vector2.Zero, Vector2.Zero);
			((Node)line).Name = StringName.op_Implicit("SilentFX_FinisherHair");
			parent.AddChild((Node)(object)line, false, (InternalMode)0);
			if (!((Node)line).IsInsideTree())
			{
				((Node)line).QueueFree();
				return;
			}
			line.Points = new Vector2[2]
			{
				((Node2D)line).ToLocal(from),
				((Node2D)line).ToLocal(to)
			};
			Curve val = new Curve();
			val.AddPoint(new Vector2(0f, 0.08f), 0f, 0f, (TangentMode)0, (TangentMode)0);
			val.AddPoint(new Vector2(0.5f, 1f), 0f, 0f, (TangentMode)0, (TangentMode)0);
			val.AddPoint(new Vector2(1f, 0.08f), 0f, 0f, (TangentMode)0, (TangentMode)0);
			line.WidthCurve = val;
			FxKit.Drive((Node)(object)line, time, (float u) =>
			{
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)line))
				{
					((CanvasItem)line).Modulate = new Color(1f, 1f, 1f, 1f - u * u);
				}
			}, (TransitionType)0, (EaseType)2);
			FxKit.FreeAfter((Node)(object)line, (double)time + 0.05);
		}

		internal static void Shards(Node parent, Vector2 at, Vector2 dir, int count, float size, bool heavy)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Expected Obj, but got Unknown
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)parent) || count <= 0)
			{
				return;
			}
			Node2D val = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_FinisherCardShards")
			};
			parent.AddChild((Node)(object)val, false, (InternalMode)0);
			if (!((Node)val).IsInsideTree())
			{
				((Node)val).QueueFree();
				return;
			}
			val.GlobalPosition = at;
			int num = (int)(Time.GetTicksMsec() & 0x3FF);
			Vector2 val2 = (Vector2)((dir.LengthSquared() > 0.01f) ? (-dir.Normalized()) : new Vector2(0f, -1f));
			Vector2 val3 = new Vector2(0f - val2.Y, val2.X);
			Polygon2D[] shards = new Polygon2D[count];
			Vector2[] ways = new Vector2[count];
			float[] turns = new float[count];
			float[] spins = new float[count];
			for (int i = 0; i < count; i++)
			{
				float num2 = FxKit.Hash(num + i * 7 + 1);
				float num3 = FxKit.Hash(num + i * 7 + 2);
				float num4 = FxKit.Hash(num + i * 7 + 3);
				float num5 = (14f + 22f * num2) * size;
				float num6 = num5 * (0.22f + 0.25f * num3);
				Color color = (Color)((i % 3 == 0) ? Tint(Hone, 0.95f) : new Color(1f, 0.97f, 0.9f, 0.95f));
				Polygon2D val4 = FxKit.Shape(new Vector2[4]
				{
					new Vector2((0f - num5) * 0.5f, 0f),
					new Vector2((0f - num5) * 0.1f, (0f - num6) * 0.5f),
					new Vector2(num5 * 0.5f, 0f),
					new Vector2(num5 * 0.05f, num6 * 0.5f)
				}, color);
				((CanvasItem)val4).Material = (Material)(object)FxKit.Additive;
				((Node)val).AddChild((Node)(object)val4, false, (InternalMode)0);
				Vector2 val5 = val2 * (0.35f + 0.65f * num2) + val3 * ((num4 - 0.5f) * 1.8f);
				Vector2 val6 = val5.Normalized();
				ways[i] = val6 * ((heavy ? 190f : 120f) * size * (0.6f + 0.8f * num3));
				turns[i] = val6.Angle();
				spins[i] = (num4 - 0.5f) * 14f;
				((Node2D)val4).Rotation = turns[i];
				shards[i] = val4;
			}
			float num7 = (heavy ? 0.46f : 0.34f);
			FxKit.Drive((Node)(object)val, num7, (float u) =>
			{
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
				float num8 = 1f - (1f - u) * (1f - u) * (1f - u);
				for (int j = 0; j < shards.Length; j++)
				{
					Polygon2D val7 = shards[j];
					if (FxKit.Alive((GodotObject)(object)val7))
					{
						((Node2D)val7).Position = ways[j] * num8 + new Vector2(0f, 90f * u * u);
						((Node2D)val7).Rotation = turns[j] + spins[j] * u;
						((CanvasItem)val7).Modulate = new Color(1f, 1f, 1f, (u < 0.4f) ? 1f : (1f - (u - 0.4f) / 0.6f));
					}
				}
			}, (TransitionType)0, (EaseType)2);
			FxKit.FreeAfter((Node)(object)val, (double)num7 + 0.1);
		}

		private static void Heavy(Node parent, Vector2 from, Vector2 to, float k, float delay)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			Brush.Cut(parent, from, to, 88f * k, Tint(End, 0.5f), delay, 0.03f, 0.06f, 0.3f);
			Brush.Cut(parent, from, to, 40f * k, Tint(Steel, 1f), delay, 0.03f, 0.07f, 0.24f);
		}

		private static float Jolt(float tau)
		{
			if (!(tau <= 0f))
			{
				return Mathf.Min(1f, tau / 0.03f) * Mathf.Exp((0f - tau) / 0.1f);
			}
			return 0f;
		}

		private static float Sunk(float t)
		{
			if (t <= 0f)
			{
				return 0f;
			}
			float num = Mathf.Min(1f, t / 0.035f);
			float num2 = ((t < 0.3f) ? 1f : Mathf.Exp((0f - (t - 0.3f)) / 0.22f));
			return num * num2 + 0.3f * Jolt(t - 0.36f) + 0.15f * Jolt(t - 0.62f);
		}

		private static float Bulged(float t)
		{
			if (t <= 0f)
			{
				return 0f;
			}
			float num = Mathf.Min(1f, t / 0.05f);
			float num2 = 1f + 0.22f * Mathf.Exp((0f - t) / 0.05f);
			float num3 = ((t < 0.36f) ? 1f : Mathf.Exp((0f - (t - 0.36f)) / 0.28f));
			return num * num2 * num3 + 0.4f * Jolt(t - 0.36f) + 0.22f * Jolt(t - 0.62f);
		}

		internal static void Finale(Node parent, Node back, Vector2 at, Vector2 feet, float side, float k, float half, int count, bool cross, Creature who = null)
		{
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)parent))
			{
				return;
			}
			int num = Mathf.Clamp(count, 1, 10);
			float num2 = Mathf.Clamp(Tuning.F("finisher.payoff", 1f), 0.3f, 2f) * (1f + 0.05f * (float)num);
			float num3 = k * num2;
			float dark = Mathf.Clamp(Tuning.F("finisher.blink", 0.42f), 0f, 0.9f);
			if (dark > 0.01f && FxKit.Alive((GodotObject)(object)back))
			{
				ColorRect veil = FxKit.Veil(new Color(0f, 0f, 0.01f, dark));
				back.AddChild((Node)(object)veil, false, (InternalMode)0);
				if (((Node)veil).IsInsideTree())
				{
					FxKit.Drive((Node)(object)veil, 0.38f, (float u) =>
					{
						//IL_0038: Unknown result type (might be due to invalid IL or missing references)
						if (FxKit.Alive((GodotObject)(object)veil))
						{
							veil.Color = new Color(0f, 0f, 0.01f, dark * (1f - u) * (1f - u));
						}
					}, (TransitionType)0, (EaseType)2);
					FxKit.FreeAfter((Node)(object)veil, 0.45);
				}
				else
				{
					((Node)veil).QueueFree();
				}
			}
			if (cross)
			{
				float num4 = half * 1.5f;
				Vector2 val = new Vector2(side, 1f);
				Vector2 val2 = val.Normalized();
				val = new Vector2(side, -1f);
				Vector2 val3 = val.Normalized();
				Heavy(parent, at - val2 * num4, at + val2 * num4, num3, 0f);
				Heavy(parent, at - val3 * num4, at + val3 * num4, num3, 0.05f);
				FxKit.Flash(parent, at, Tint(End, 0.85f), 130f * num3, 2.2f, 0.32);
			}
			try
			{
				Slam(parent, back, feet, half, k, num2, num, who);
			}
			catch (Exception ex)
			{
				DevLog.Warn("FinisherFx (slam): " + ex.Message);
			}
		}

		private static void Slam(Node parent, Node back, Vector2 feet, float half, float k, float gain, int n, Creature who)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Expected Obj, but got Unknown
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Expected Obj, but got Unknown
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Expected Obj, but got Unknown
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_040b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0417: Unknown result type (might be due to invalid IL or missing references)
			//IL_0440: Unknown result type (might be due to invalid IL or missing references)
			//IL_0479: Unknown result type (might be due to invalid IL or missing references)
			//IL_0485: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0507: Unknown result type (might be due to invalid IL or missing references)
			//IL_050c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0511: Unknown result type (might be due to invalid IL or missing references)
			//IL_0518: Unknown result type (might be due to invalid IL or missing references)
			//IL_0528: Unknown result type (might be due to invalid IL or missing references)
			//IL_052d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0532: Unknown result type (might be due to invalid IL or missing references)
			//IL_053e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0562: Unknown result type (might be due to invalid IL or missing references)
			//IL_0586: Unknown result type (might be due to invalid IL or missing references)
			//IL_058b: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05de: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
			Node2D holder = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_FinisherSlam")
			};
			parent.AddChild((Node)(object)holder, false, (InternalMode)0);
			if (!((Node)holder).IsInsideTree())
			{
				((Node)holder).QueueFree();
				return;
			}
			FxKit.FreeAfter((Node)(object)holder, 1.2999999523162842);
			float num = k * gain;
			float reach = Mathf.Clamp(half, 90f, 320f) * (0.8f + 0.04f * (float)n);
			float high = Mathf.Clamp(44f * num, 26f, 130f);
			float lipWide = reach * 0.95f;
			object obj = ((FxKit.Alive((GodotObject)(object)back) && back.IsInsideTree()) ? ((object)back) : ((object)holder));
			Sprite2D dent = new Sprite2D
			{
				Name = StringName.op_Implicit("SilentFX_FinisherDent"),
				Texture = FxKit.SoftDot,
				Modulate = new Color(0.02f, 0.015f, 0.03f, 0f)
			};
			((Node)obj).AddChild((Node)(object)dent, false, (InternalMode)0);
			FxKit.FreeAfter((Node)(object)dent, 1.2999999523162842);
			((Node2D)dent).GlobalPosition = feet + new Vector2(0f, 8f * k);
			((Node2D)dent).Scale = new Vector2(reach * 2.5f, 40f * num) / 64f;
			Sprite2D[] mounds = new Sprite2D[2];
			Sprite2D[] rims = new Sprite2D[2];
			for (int i = 0; i < 2; i++)
			{
				mounds[i] = new Sprite2D
				{
					Texture = FxKit.SoftDot,
					Modulate = new Color(0.3f, 0.26f, 0.23f, 0f)
				};
				((Node)holder).AddChild((Node)(object)mounds[i], false, (InternalMode)0);
				rims[i] = FxKit.Glow(new Color(1f, 0.84f, 0.66f, 0f), new Vector2(64f, 64f));
				((Node)holder).AddChild((Node)(object)rims[i], false, (InternalMode)0);
			}
			Sprite2D flash = FxKit.Glow(Tint(End, 0.85f), new Vector2(reach * 3f, 64f * num));
			((Node)holder).AddChild((Node)(object)flash, false, (InternalMode)0);
			((Node2D)flash).GlobalPosition = feet;
			Sprite2D hot = FxKit.Glow(new Color(1f, 0.95f, 0.86f, 0.95f), new Vector2(reach * 1.5f, 26f * num));
			((Node)holder).AddChild((Node)(object)hot, false, (InternalMode)0);
			((Node2D)hot).GlobalPosition = feet;
			Vector2 flashScale = ((Node2D)flash).Scale;
			Vector2 hotScale = ((Node2D)hot).Scale;
			Sink sink = Seize(who);
			float deep = Mathf.Clamp(Tuning.F("finisher.sink_px", 20f), 0f, 60f) * num;
			if (sink != null)
			{
				((Node)holder).TreeExiting += () =>
				{
					LetGo(sink);
				};
			}
			FxKit.Drive((Node)(object)holder, 1.15f, (float u) =>
			{
				//IL_0061: Unknown result type (might be due to invalid IL or missing references)
				//IL_0287: Unknown result type (might be due to invalid IL or missing references)
				//IL_0297: Unknown result type (might be due to invalid IL or missing references)
				//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_008f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
				//IL_030d: Unknown result type (might be due to invalid IL or missing references)
				//IL_033a: Unknown result type (might be due to invalid IL or missing references)
				//IL_00df: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
				//IL_011d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0127: Unknown result type (might be due to invalid IL or missing references)
				//IL_0162: Unknown result type (might be due to invalid IL or missing references)
				//IL_0364: Unknown result type (might be due to invalid IL or missing references)
				//IL_0188: Unknown result type (might be due to invalid IL or missing references)
				//IL_0197: Unknown result type (might be due to invalid IL or missing references)
				//IL_019c: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_01db: Unknown result type (might be due to invalid IL or missing references)
				//IL_0210: Unknown result type (might be due to invalid IL or missing references)
				float num3 = u * 1.15f;
				float num4 = Bulged(num3);
				float num5 = 1f - Mathf.SmoothStep(0.7f, 1.15f, num3);
				if (FxKit.Alive((GodotObject)(object)dent))
				{
					((CanvasItem)dent).Modulate = new Color(0.02f, 0.015f, 0.03f, 0.78f * Mathf.Min(1f, num3 / 0.03f) * num5);
				}
				float num6 = high * num4;
				for (int j = 0; j < 2; j++)
				{
					float num7 = ((j == 0) ? (-1f) : 1f);
					Vector2 val2 = feet + new Vector2(num7 * (reach * 1.05f + 12f * k * num4), 0f);
					if (FxKit.Alive((GodotObject)(object)mounds[j]))
					{
						((Node2D)mounds[j]).GlobalPosition = val2 + new Vector2(0f, (0f - num6) * 0.3f);
						((Node2D)mounds[j]).Scale = new Vector2(lipWide, Mathf.Max(2f, num6 * 1.7f)) / 64f;
						((CanvasItem)mounds[j]).Modulate = new Color(0.3f, 0.26f, 0.23f, 0.85f * Mathf.Min(1f, num4 * 1.6f) * num5);
					}
					if (FxKit.Alive((GodotObject)(object)rims[j]))
					{
						((Node2D)rims[j]).GlobalPosition = val2 + new Vector2(0f, (0f - num6) * 0.95f);
						((Node2D)rims[j]).Scale = new Vector2(lipWide * 0.75f, Mathf.Max(3f, 9f * k)) / 64f;
						((CanvasItem)rims[j]).Modulate = new Color(1f, 0.84f, 0.66f, Mathf.Min(1f, 0.75f * num4) * num5);
					}
				}
				float num8 = Mathf.Clamp(num3 / 0.22f, 0f, 1f);
				if (FxKit.Alive((GodotObject)(object)flash))
				{
					((Node2D)flash).Scale = new Vector2(flashScale.X * (1f + 0.35f * num8), flashScale.Y * (1f - 0.4f * num8));
					((CanvasItem)flash).Modulate = Tint(End, 0.85f * (1f - num8) * (1f - num8));
				}
				float num9 = Mathf.Clamp(num3 / 0.1f, 0f, 1f);
				if (FxKit.Alive((GodotObject)(object)hot))
				{
					((Node2D)hot).Scale = new Vector2(hotScale.X * (1f + 0.5f * num9), hotScale.Y);
					((CanvasItem)hot).Modulate = new Color(1f, 0.95f, 0.86f, 0.95f * (1f - num9));
				}
				if (sink != null)
				{
					Push(sink, new Vector2(0f, deep * Sunk(num3)));
				}
			}, (TransitionType)0, (EaseType)2).TweenCallback(Callable.From((Action)(() =>
			{
				if (sink != null)
				{
					LetGo(sink);
				}
				if (FxKit.Alive((GodotObject)(object)holder))
				{
					((Node)holder).QueueFree();
				}
			})));
			float num2 = Mathf.Max(330f * num, reach * 3f);
			Color tint = new Color(1f, 0.88f, 0.76f, 1f);
			FxKit.RingPulse(parent, feet, num2, new Color(End.R, End.G, End.B, 1f), 0f, 0.42f, 0.18f, 1f, 0.3f, 0f, 0.04f, 0.9f);
			FxKit.RingPulse(parent, feet, num2 * 0.9f, tint, 0.36f, 0.34f, 0.3f, 1f, 0.3f, 0f, 0.03f, 0.5f);
			FxKit.RingPulse(parent, feet, num2 * 0.8f, tint, 0.62f, 0.3f, 0.35f, 1f, 0.3f, 0f, 0.024f, 0.3f);
			Vector2 left = feet + new Vector2(0f - reach, 0f);
			Vector2 right = feet + new Vector2(reach, 0f);
			Brush.Dust(parent, left, -1f, 10, 1.1f * k);
			Brush.Dust(parent, right, 1f, 10, 1.1f * k);
			Node parent2 = parent;
			Vector2 at = left;
			Color end = End;
			Vector2 val = new Vector2(-0.45f, -1f);
			Brush.Sparks(parent2, at, end, 6, 420f, val.Normalized(), 46f);
			Node parent3 = parent;
			Vector2 at2 = right;
			Color end2 = End;
			val = new Vector2(0.45f, -1f);
			Brush.Sparks(parent3, at2, end2, 6, 420f, val.Normalized(), 46f);
			bool shake = Tuning.F("finisher.shake", 1f) >= 0.5f;
			if (shake)
			{
				GameRefs.Shake((n >= 4) ? 3 : 2);
			}
			FxKit.After((Node)(object)holder, 0.36000001430511475, () =>
			{
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)parent))
				{
					Brush.Dust(parent, left, -1f, 5, 0.9f * k);
					Brush.Dust(parent, right, 1f, 5, 0.9f * k);
					if (shake)
					{
						GameRefs.Shake(1);
					}
				}
			});
			FxKit.After((Node)(object)holder, 0.6200000047683716, () =>
			{
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)parent))
				{
					Brush.Dust(parent, left, -1f, 3, 0.8f * k);
					Brush.Dust(parent, right, 1f, 3, 0.8f * k);
				}
			});
		}

		private static Sink Seize(Creature who)
		{
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (who == null || !who.IsAlive || Tuning.F("finisher.sink_px", 20f) <= 0.01f)
				{
					return null;
				}
				NCombatRoom combatRoom = GameRefs.CombatRoom;
				NCreature val = ((combatRoom != null) ? combatRoom.GetCreatureNode(who) : null);
				if (!FxKit.Alive((GodotObject)(object)val) || !((Node)val).IsInsideTree())
				{
					return null;
				}
				Node2D body = val.Body;
				if (!FxKit.Alive((GodotObject)(object)body) || !((CanvasItem)body).Visible)
				{
					return null;
				}
				Sink[] array = Sinks.ToArray();
				foreach (Sink sink in array)
				{
					if (sink.Picture == body)
					{
						LetGo(sink);
					}
				}
				Sink sink2 = new Sink
				{
					Who = who,
					Picture = body,
					Home = body.Position,
					Wrote = body.Position
				};
				Sinks.Add(sink2);
				return sink2;
			}
			catch (Exception ex)
			{
				DevLog.Warn("FinisherFx (victim): " + ex.Message);
				return null;
			}
		}

		private static void Push(Sink sink, Vector2 screen)
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			if (sink == null || sink.Over)
			{
				return;
			}
			Node2D picture = sink.Picture;
			if (picture == null)
			{
				return;
			}
			if (!FxKit.Alive((GodotObject)(object)picture))
			{
				sink.Picture = null;
				return;
			}
			if (sink.Who != null && !sink.Who.IsAlive)
			{
				LetGo(sink);
				return;
			}
			Vector2 val = Vector2.Zero;
			if (screen != Vector2.Zero && ((Node)picture).IsInsideTree())
			{
				Node parent = ((Node)picture).GetParent();
				CanvasItem val2 = (CanvasItem)(object)((parent is CanvasItem) ? parent : null);
				if (val2 != null && FxKit.Alive((GodotObject)(object)val2))
				{
					Transform2D val3 = val2.GetGlobalTransform();
					val3 = val3.AffineInverse();
					val = val3.BasisXform(screen);
					if (!float.IsFinite(val.X) || !float.IsFinite(val.Y) || val.LengthSquared() > 160000f)
					{
						val = Vector2.Zero;
					}
				}
			}
			if (!(val == sink.Shift))
			{
				Vector2 position = picture.Position;
				if (position != sink.Wrote)
				{
					sink.Home = position - sink.Shift;
				}
				Vector2 wrote = (picture.Position = ((val == Vector2.Zero) ? sink.Home : (sink.Home + val)));
				sink.Wrote = wrote;
				sink.Shift = val;
			}
		}

		private static void LetGo(Sink sink)
		{
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			if (sink == null || sink.Over)
			{
				return;
			}
			sink.Over = true;
			Sinks.Remove(sink);
			try
			{
				Node2D picture = sink.Picture;
				if (FxKit.Alive((GodotObject)(object)picture) && sink.Shift != Vector2.Zero)
				{
					Vector2 position = picture.Position;
					picture.Position = ((position != sink.Wrote) ? (position - sink.Shift) : sink.Home);
				}
			}
			catch
			{
			}
			sink.Picture = null;
			sink.Shift = Vector2.Zero;
		}

		static FinisherFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		}
	}
