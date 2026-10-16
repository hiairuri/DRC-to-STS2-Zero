// namespace: SilentFX.Cards
// type: RicochetRun (class)
// source lines: 89155-90015
	internal sealed class RicochetRun
	{
		private sealed class Mirror
		{
			public bool Top;

			public float Left;

			public float Right;

			public float Edge;

			public float Sag;

			public Sprite2D[] Outer;

			public Sprite2D[] Core;

			public float[] Along;

			public float[] Fat;

			public float[] Heat;

			public float Step;

			public Vector2 At(float x)
			{
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				float num = Mathf.Max(1f, (Right - Left) * 0.5f);
				float num2 = Mathf.Clamp((x - (Left + Right) * 0.5f) / num, -1f, 1f);
				float num3 = Sag * (1f - num2 * num2);
				return new Vector2(Mathf.Clamp(x, Left, Right), Top ? (Edge - num3) : (Edge + num3));
			}

			public void Strike(float x, float reach)
			{
				if (Heat == null)
				{
					return;
				}
				for (int i = 0; i < Heat.Length; i++)
				{
					float num = (Along[i] - x) / Mathf.Max(1f, reach);
					float num2 = Mathf.Exp((0f - num) * num);
					if (num2 > Heat[i])
					{
						Heat[i] = num2;
					}
				}
			}
		}

		private static readonly Color Ray = new Color(1f, 1f, 1f, 0.95f);

		private static readonly Color Glass = new Color(0.8f, 0.9f, 1f, 1f);

		private static readonly object PreviewKey = new object();

		private static readonly Dictionary<object, RicochetRun> Live = new Dictionary<object, RicochetRun>(ReferenceEqualityComparer.Instance);

		private static readonly List<RicochetRun> All = new List<RicochetRun>();

		private static readonly List<RicochetRun> Scratch = new List<RicochetRun>();

		private static bool _ticking;

		private static bool _listening;

		private object _key;

		private bool _multiplayer;

		private CardModel _card;

		private Creature _owner;

		private Node _room;

		private AttackInfo _attack;

		private Node _parent;

		private Node2D _holder;

		private Mirror _top;

		private Mirror _bottom;

		private float _scale = 1f;

		private float _side = 1f;

		private Vector2 _hand;

		private ulong _beganMs;

		private ulong _lastHitMs;

		private ulong _arriveMs;

		private ulong _endedMs;

		private float _appear;

		private float _fade = 1f;

		private bool _ending;

		private bool _in;

		private bool _onTop;

		private Vector2 _at;

		private Vector2 _dir;

		private int _hits;

		private int _planned;

		private int _march;

		public static void Begin(CardFxContext c)
		{
			if (c == null || !FxKit.Alive((GodotObject)(object)c.FrontParent))
			{
				return;
			}
			Listen();
			object key = c.Card ?? PreviewKey;
			bool flag = MultiplayerFx.Active && c.Card != null;
			if (flag)
			{
				if (MultiplayerFx.CurrentInvocation == null || MultiplayerFx.CurrentCard != c.Card)
				{
					return;
				}
				Player owner = c.Card.Owner;
				if (((owner != null) ? owner.Creature : null) == null || !FxKit.Alive((GodotObject)(object)GameRefs.CombatRoom))
				{
					return;
				}
				key = MultiplayerFx.CurrentInvocation;
			}
			if (Live.TryGetValue(key, out var value))
			{
				value.Finish(atOnce: true);
				Live.Remove(key);
			}
			RicochetRun ricochetRun = new RicochetRun
			{
				_multiplayer = flag,
				_card = c.Card,
				_owner = (flag ? c.Card.Owner.Creature : null),
				_room = (Node)(object)(flag ? GameRefs.CombatRoom : null)
			};
			if ((!flag || (ricochetRun._owner.IsAlive && ricochetRun.OwnerPresent())) && ricochetRun.Open(c, key))
			{
				Live[key] = ricochetRun;
				All.Add(ricochetRun);
				if (!_ticking)
				{
					Ticker.Add(Tick);
					_ticking = true;
				}
			}
		}

		public static void Hit(CardFxContext c)
		{
			if (c == null || !c.HasTarget)
			{
				return;
			}
			object key = c.Card ?? PreviewKey;
			if (!MultiplayerFx.Active && AttackFeed.CurrentDamage(c.Card)?.Invocation != null)
			{
				return;
			}
			AttackInfo attackInfo = null;
			if (MultiplayerFx.Active && c.Card != null)
			{
				attackInfo = AttackFeed.CurrentDamage(c.Card);
				if (attackInfo == null || attackInfo.Invocation == null || attackInfo.Invocation != MultiplayerFx.CurrentInvocation || MultiplayerFx.CurrentCard != c.Card)
				{
					return;
				}
				Creature attacker = attackInfo.Attacker;
				Player owner = c.Card.Owner;
				if (attacker != ((owner != null) ? owner.Creature : null))
				{
					return;
				}
				key = attackInfo.Invocation;
				if (!Live.TryGetValue(key, out var value) || value._ending || !value.Valid() || (value._attack != null && value._attack != attackInfo))
				{
					return;
				}
			}
			if (!Live.TryGetValue(key, out var value2) || value2._ending)
			{
				Begin(c);
				if (!Live.TryGetValue(key, out value2))
				{
					return;
				}
				value2._appear = 1f;
			}
			if (attackInfo != null)
			{
				value2._attack = attackInfo;
				value2._planned = attackInfo.PlannedHits;
			}
			value2.Fold(c);
		}

		private static void Listen()
		{
			if (_listening)
			{
				return;
			}
			_listening = true;
			try
			{
				AttackFeed.Began += (AttackInfo info) =>
				{
					RicochetRun value2;
					if (info != null && info.Invocation != null)
					{
						if (Live.TryGetValue(info.Invocation, out var value) && value.Valid() && value._card == info.Card && value._owner == info.Attacker && (value._attack == null || value._attack == info))
						{
							value._attack = info;
							value._planned = info.PlannedHits;
						}
					}
					else if (info?.Card != null && Live.TryGetValue(info.Card, out value2))
					{
						value2._planned = info.PlannedHits;
					}
				};
				AttackFeed.Ended += (AttackInfo info) =>
				{
					RicochetRun value2;
					if (info != null && info.Invocation != null)
					{
						if (Live.TryGetValue(info.Invocation, out var value) && value._attack == info)
						{
							if (value.Valid())
							{
								value.Finish(atOnce: false);
							}
							else
							{
								value.Drop();
							}
						}
					}
					else if (info?.Card != null && Live.TryGetValue(info.Card, out value2))
					{
						value2.Finish(atOnce: false);
					}
				};
			}
			catch (Exception ex)
			{
				DevLog.Warn("RicochetRun.Listen: " + ex.Message);
			}
		}

		private bool Open(CardFxContext c, object key)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Expected Obj, but got Unknown
			_key = key;
			_parent = c.FrontParent;
			_hand = c.WeaponHand;
			_beganMs = FxClock.Milliseconds;
			float left = float.MaxValue;
			float right = float.MinValue;
			float head = float.MaxValue;
			float feet = float.MinValue;
			float widest = 0f;
			int count = 0;
			float middle = 0f;
			foreach (FxTarget enemy in c.Enemies)
			{
				if (enemy != null)
				{
					Take(enemy.Feet, enemy.Width, enemy.Height);
				}
			}
			if (c.HasTarget)
			{
				Take(c.TargetFeet, c.TargetWidth, c.TargetHeight);
			}
			if (count == 0)
			{
				return false;
			}
			middle /= count;
			_side = ((middle < c.Chest.X) ? (-1f) : 1f);
			_scale = Mathf.Clamp(Mathf.Max(widest, feet - head) / 260f, 1f, 2.4f);
			float num = Mathf.Clamp(widest * 0.34f, 44f, 120f);
			left -= num;
			right += num;
			float num2 = feet - head;
			float edge = Mathf.Max(34f, head - Mathf.Clamp(num2 * 0.1f, 16f, 44f) * Tuning.F("ricochet.gap", 1f));
			float edge2 = Mathf.Min(c.View.Y - 30f, feet + Mathf.Clamp(num2 * 0.07f, 14f, 30f) * Tuning.F("ricochet.gap", 1f));
			float num3 = Mathf.Clamp((right - left) * 0.045f, 9f, 36f) * Tuning.F("ricochet.curve", 1f);
			_holder = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_Ricochet")
			};
			_parent.AddChild((Node)(object)_holder, false, (InternalMode)0);
			if (!((Node)_holder).IsInsideTree())
			{
				((Node)_holder).QueueFree();
				_holder = null;
				return false;
			}
			_top = Build(top: true, left, right, edge, num3);
			_bottom = Build(top: false, left, right, edge2, num3 * 0.8f);
			Draw(0f);
			return true;
			void Take(Vector2 foot, float width, float height)
			{
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
				width = Mathf.Max(60f, width);
				height = Mathf.Max(80f, height);
				left = Mathf.Min(left, foot.X - width * 0.5f);
				right = Mathf.Max(right, foot.X + width * 0.5f);
				head = Mathf.Min(head, foot.Y - height);
				feet = Mathf.Max(feet, foot.Y);
				widest = Mathf.Max(widest, width);
				middle += foot.X;
				count++;
			}
		}

		private Mirror Build(bool top, float left, float right, float edge, float sag)
		{
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			Mirror mirror = new Mirror
			{
				Top = top,
				Left = left,
				Right = right,
				Edge = edge,
				Sag = sag
			};
			int num = Mathf.Clamp(Mathf.RoundToInt((right - left) / 30f), 9, 48);
			mirror.Outer = new Sprite2D[num];
			mirror.Core = new Sprite2D[num];
			mirror.Along = new float[num];
			mirror.Fat = new float[num];
			mirror.Heat = new float[num];
			mirror.Step = (right - left) / (float)num;
			for (int i = 0; i < num; i++)
			{
				float num2 = ((float)i + 0.5f) / (float)num;
				float num3 = Mathf.Lerp(left, right, num2);
				Vector2 globalPosition = mirror.At(num3);
				Vector2 val = mirror.At(num3 + 4f) - mirror.At(num3 - 4f);
				float globalRotation = ((val.LengthSquared() > 0.001f) ? val.Angle() : 0f);
				mirror.Along[i] = num3;
				mirror.Fat[i] = Mathf.Pow(Mathf.Sin((float)Math.PI * num2), 0.7f);
				mirror.Outer[i] = FxKit.Glow(new Color(Glass.R, Glass.G, Glass.B, 0f), new Vector2(64f, 64f));
				mirror.Core[i] = FxKit.Glow(new Color(1f, 1f, 1f, 0f), new Vector2(64f, 64f));
				((Node)_holder).AddChild((Node)(object)mirror.Outer[i], false, (InternalMode)0);
				((Node)_holder).AddChild((Node)(object)mirror.Core[i], false, (InternalMode)0);
				((Node2D)mirror.Outer[i]).GlobalPosition = globalPosition;
				((Node2D)mirror.Core[i]).GlobalPosition = globalPosition;
				((Node2D)mirror.Outer[i]).GlobalRotation = globalRotation;
				((Node2D)mirror.Core[i]).GlobalRotation = globalRotation;
			}
			return mirror;
		}

		private void Draw(float dt)
		{
			if (FxKit.Alive((GodotObject)(object)_holder))
			{
				float thick = Tuning.F("ricochet.mirror_thick", 7f) * _scale;
				float faint = Tuning.F("ricochet.mirror_alpha", 0.22f);
				float cool = Mathf.Exp((0f - dt) * Tuning.F("ricochet.cool", 4.5f));
				Paint(_top, thick, faint, cool);
				Paint(_bottom, thick, faint, cool);
			}
		}

		private void Paint(Mirror m, float thick, float faint, float cool)
		{
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			if (m?.Outer == null)
			{
				return;
			}
			int num = m.Outer.Length;
			for (int i = 0; i < num; i++)
			{
				if (FxKit.Alive((GodotObject)(object)m.Outer[i]) && FxKit.Alive((GodotObject)(object)m.Core[i]))
				{
					m.Heat[i] *= cool;
					float num2 = ((float)i + 0.5f) / (float)num;
					float num3 = ((_side > 0f) ? num2 : (1f - num2));
					float num4 = Mathf.Clamp((_appear * 1.25f - num3) * 4f, 0f, 1f);
					float num5 = m.Heat[i];
					float num6 = num4 * _fade;
					float num7 = thick * (0.35f + 0.65f * m.Fat[i]) * (1f + 1.3f * num5);
					((Node2D)m.Outer[i]).Scale = new Vector2(m.Step * 2.6f, Mathf.Max(1f, num7)) / 64f;
					((Node2D)m.Core[i]).Scale = new Vector2(m.Step * 2.2f, Mathf.Max(0.5f, num7 * 0.34f)) / 64f;
					((CanvasItem)m.Outer[i]).Modulate = new Color(Glass.R, Glass.G, Glass.B, Mathf.Clamp((faint * m.Fat[i] + 0.75f * num5) * num6, 0f, 1f));
					((CanvasItem)m.Core[i]).Modulate = new Color(1f, 1f, 1f, Mathf.Clamp((faint * 0.35f * m.Fat[i] + 0.95f * num5) * num6, 0f, 1f));
				}
			}
		}

		private void Fold(CardFxContext c)
		{
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_042b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0457: Unknown result type (might be due to invalid IL or missing references)
			//IL_0460: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0477: Unknown result type (might be due to invalid IL or missing references)
			//IL_047c: Unknown result type (might be due to invalid IL or missing references)
			//IL_048c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_0522: Unknown result type (might be due to invalid IL or missing references)
			//IL_0529: Unknown result type (might be due to invalid IL or missing references)
			//IL_052e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0535: Unknown result type (might be due to invalid IL or missing references)
			//IL_053a: Unknown result type (might be due to invalid IL or missing references)
			//IL_053c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0541: Unknown result type (might be due to invalid IL or missing references)
			//IL_0545: Unknown result type (might be due to invalid IL or missing references)
			//IL_054a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0555: Unknown result type (might be due to invalid IL or missing references)
			//IL_0558: Unknown result type (might be due to invalid IL or missing references)
			//IL_055f: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			if (!Valid() || !FxKit.Alive((GodotObject)(object)_holder) || !FxKit.Alive((GodotObject)(object)_parent))
			{
				return;
			}
			float num = Mathf.Max(0.01f, Tuning.F("ricochet.sweep", 0.035f));
			float hold = Tuning.F("ricochet.hold", 0.02f);
			float fade = Tuning.F("ricochet.fade", 0.17f);
			float thick = Tuning.F("ricochet.thick", 15f) * _scale;
			float thick2 = Tuning.F("ricochet.thin", 5f) * _scale;
			float num2 = Mathf.Max(60f, c.TargetWidth);
			float num3 = Mathf.Max(80f, c.TargetHeight);
			float x = c.TargetFeet.X;
			float num4 = c.TargetFeet.Y - num3 * 0.86f;
			float num5 = c.TargetFeet.Y - num3 * 0.08f;
			float num6 = 0f;
			if (!_in)
			{
				if (_planned <= 0)
				{
					_planned = Planned(c.Card);
				}
				_onTop = _planned <= 0 || _planned % 2 == 0;
				Mirror mirror = (_onTop ? _top : _bottom);
				_at = mirror.At(x - _side * num2 * 0.42f);
				_march = 0;
				Brush.Cut(_parent, _hand, _at, thick2, Ray, 0f, num, hold, fade);
				num6 = num;
				_in = true;
				Vector2 entry = _at;
				bool top = _onTop;
				FxKit.After((Node)(object)_holder, num6, () =>
				{
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					Glint(top ? _top : _bottom, entry, 0.7f);
				});
			}
			else
			{
				ulong milliseconds = FxClock.Milliseconds;
				if (_arriveMs > milliseconds)
				{
					num6 = Mathf.Min(Tuning.F("ricochet.queue", 0.2f), (float)(_arriveMs - milliseconds) / 1000f);
				}
			}
			Mirror far = (_onTop ? _bottom : _top);
			Vector2 at = _at;
			float num7 = far.At(x).Y - at.Y;
			if (Mathf.Abs(num7) < 1f)
			{
				return;
			}
			float num8 = (num4 - at.Y) / num7;
			float num9 = (num5 - at.Y) / num7;
			float num10 = Mathf.Clamp(Mathf.Min(num8, num9), 0.08f, 0.92f);
			float num11 = Mathf.Clamp(Mathf.Max(num8, num9), 0.08f, 0.92f);
			float num12 = Mathf.Clamp(num2 * 0.3f, 26f, 90f);
			float x2;
			float num15;
			if (Mathf.Abs(at.X - x) <= num2 * 0.5f + num12)
			{
				float num13 = num2 * Mathf.Clamp(0.84f / (float)Mathf.Max(1, (_planned > 0) ? _planned : 4), 0.16f, 0.3f);
				float num14 = num2 * 0.42f;
				if (Mathf.Abs(at.X - x) > num14 + 1f)
				{
					_march = ((at.X < x) ? 1 : (-1));
					x2 = x + (float)_march * num2 * 0.1f;
				}
				else
				{
					if (_march == 0)
					{
						_march = ((at.X < x) ? 1 : (-1));
					}
					if (Mathf.Abs(at.X + (float)_march * num13 - x) > num14 + 1f)
					{
						_march = -_march;
					}
					x2 = at.X + (float)_march * num13;
				}
				num15 = 0.38f + 0.24f * FxKit.Hash(_hits * 5 + 11);
			}
			else
			{
				_march = 0;
				float num16 = x - (float)Mathf.Sign(x - at.X) * num2 * 0.1f;
				float num17 = Mathf.Abs(num16 - at.X);
				num15 = Mathf.Clamp(Mathf.Max(0.5f, num17 / (num17 + num12)), num10, num11);
				x2 = at.X + (num16 - at.X) / num15;
			}
			Vector2 to = far.At(x2);
			float num18 = to.Y - at.Y;
			if (Mathf.Abs(num18) < 1f)
			{
				return;
			}
			float num19 = (num4 - at.Y) / num18;
			float num20 = (num5 - at.Y) / num18;
			float num21 = Mathf.Clamp(Mathf.Min(num19, num20) + 0.03f, 0.06f, 0.94f);
			float num22 = Mathf.Clamp(Mathf.Max(num19, num20) - 0.03f, 0.06f, 0.94f);
			num15 = ((num21 <= num22) ? Mathf.Clamp(num15, num21, num22) : ((num21 + num22) * 0.5f));
			Vector2 hit = at.Lerp(to, num15);
			Vector2 val = to - at;
			Vector2 dir = val.Normalized();
			Brush.Cut(_parent, at, to, thick, Ray, num6, num, hold, fade);
			float scale = _scale;
			Node parent = _parent;
			FxKit.After((Node)(object)_holder, num6 + num * num15, () =>
			{
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0077: Unknown result type (might be due to invalid IL or missing references)
				if (Valid() && FxKit.Alive((GodotObject)(object)parent))
				{
					FxKit.Flash(parent, hit, new Color(1f, 1f, 1f, 0.85f), 58f * scale, 1.7f, 0.12);
					Brush.Sparks(parent, hit, Ray, 5, 300f, dir, 38f);
				}
			});
			FxKit.After((Node)(object)_holder, num6 + num, () =>
			{
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				Glint(far, to, 1f);
			});
			_at = to;
			_dir = dir;
			_onTop = !_onTop;
			_hits++;
			_lastHitMs = FxClock.Milliseconds;
			_arriveMs = _lastHitMs + (ulong)((num6 + num) * 1000f);
		}

		private void Glint(Mirror m, Vector2 at, float strength)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			if (Valid() && m != null && (!_ending || !(_fade <= 0.01f)) && FxKit.Alive((GodotObject)(object)_parent))
			{
				m.Strike(at.X, 46f * _scale);
				FxKit.Flash(_parent, at, new Color(1f, 1f, 1f, 0.8f * strength), 30f * _scale, 1.6f, 0.1);
				Brush.Sparks(_parent, at, Glass, 3, 180f, new Vector2(0f, m.Top ? 1f : (-1f)), 60f);
			}
		}

		private static int Planned(CardModel card)
		{
			try
			{
				if (card == null)
				{
					return 0;
				}
				AttackInfo attackInfo = AttackFeed.Current(card);
				if (attackInfo != null && attackInfo.PlannedHits > 0)
				{
					return attackInfo.PlannedHits;
				}
				return ((DynamicVar)card.DynamicVars.Repeat).IntValue;
			}
			catch
			{
				return 0;
			}
		}

		private void Finish(bool atOnce)
		{
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			if (_ending)
			{
				return;
			}
			_ending = true;
			_endedMs = FxClock.Milliseconds;
			if (atOnce)
			{
				_arriveMs = 0uL;
				_fade = Mathf.Min(_fade, 0.5f);
			}
			else
			{
				if (!_in || !FxKit.Alive((GodotObject)(object)_holder) || !FxKit.Alive((GodotObject)(object)_parent))
				{
					return;
				}
				ulong milliseconds = FxClock.Milliseconds;
				float num = ((_arriveMs > milliseconds) ? ((float)(_arriveMs - milliseconds) / 1000f) : 0f) + Tuning.F("ricochet.leave_wait", 0.03f);
				Vector2 from = _at;
				Vector2 dir = _dir;
				bool up = dir.Y < 0f;
				float scale = _scale;
				Node parent = _parent;
				FxKit.After((Node)(object)_holder, num, () =>
				{
					//IL_0097: Unknown result type (might be due to invalid IL or missing references)
					//IL_009d: Unknown result type (might be due to invalid IL or missing references)
					//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
					//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
					//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
					//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
					//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
					//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
					//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
					//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
					//IL_0104: Unknown result type (might be due to invalid IL or missing references)
					//IL_0041: Unknown result type (might be due to invalid IL or missing references)
					//IL_0047: Unknown result type (might be due to invalid IL or missing references)
					//IL_004d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0061: Unknown result type (might be due to invalid IL or missing references)
					//IL_006c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0071: Unknown result type (might be due to invalid IL or missing references)
					//IL_0077: Unknown result type (might be due to invalid IL or missing references)
					if (Valid() && FxKit.Alive((GodotObject)(object)parent))
					{
						float thick = Tuning.F("ricochet.thin", 5f) * scale;
						if (up)
						{
							Brush.Cut(parent, from, from + dir * Tuning.F("ricochet.leave", 300f) * scale, thick, Ray, 0f, 0.05f, 0.01f, 0.2f);
						}
						else
						{
							Vector2 val = from + dir * 46f * scale;
							Brush.Cut(parent, from, val, thick, Ray, 0f, 0.025f, 0.01f, 0.14f);
							FxKit.Flash(parent, val, new Color(0.9f, 0.95f, 1f, 0.45f), 54f * scale, 1.8f, 0.18);
						}
					}
				});
			}
		}

		private static void Tick(double realDt)
		{
			try
			{
				float dt = (float)(realDt * (double)Mathf.Clamp((float)Engine.TimeScale, 0f, 4f));
				ulong milliseconds = FxClock.Milliseconds;
				Scratch.Clear();
				Scratch.AddRange(All);
				foreach (RicochetRun item in Scratch)
				{
					item.Step(dt, milliseconds);
				}
				Scratch.Clear();
				if (All.Count == 0 && _ticking)
				{
					Ticker.Remove(Tick);
					_ticking = false;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("RicochetRun.Tick: " + ex.Message);
			}
		}

		private void Drop()
		{
			All.Remove(this);
			if (Live.TryGetValue(_key, out var value) && value == this)
			{
				Live.Remove(_key);
			}
			if (FxKit.Alive((GodotObject)(object)_holder))
			{
				((Node)_holder).QueueFree();
			}
		}

		private bool Valid()
		{
			if (_multiplayer)
			{
				if (MultiplayerFx.Active && FxKit.Alive((GodotObject)(object)_room) && _room.IsInsideTree() && FxKit.Alive((GodotObject)(object)_holder) && ((Node)_holder).IsInsideTree() && FxKit.Alive((GodotObject)(object)_parent) && (object)_room == GameRefs.CombatRoom && _owner != null && _owner.IsAlive)
				{
					Creature owner = _owner;
					CardModel card = _card;
					object obj;
					if (card == null)
					{
						obj = null;
					}
					else
					{
						Player owner2 = card.Owner;
						obj = ((owner2 != null) ? owner2.Creature : null);
					}
					if (owner == obj)
					{
						return OwnerPresent();
					}
				}
				return false;
			}
			return true;
		}

		private bool OwnerPresent()
		{
			CombatManager instance = CombatManager.Instance;
			CombatState val = ((instance != null) ? instance.DebugOnlyGetState() : null);
			if (val != null)
			{
				Creature owner = _owner;
				if ((object)((owner != null) ? owner.CombatState : null) == val)
				{
					CardModel card = _card;
					Player obj = ((card != null) ? card.Owner : null);
					Creature owner2 = _owner;
					if (obj == ((owner2 != null) ? owner2.Player : null))
					{
						foreach (Player player in val.Players)
						{
							if (player == _owner.Player)
							{
								return true;
							}
						}
						return false;
					}
				}
			}
			return false;
		}

		private void Step(float dt, ulong now)
		{
			if (!Valid() || !FxKit.Alive((GodotObject)(object)_holder))
			{
				Drop();
				return;
			}
			if (!_ending)
			{
				_appear = Mathf.Min(1f, _appear + dt / Mathf.Max(0.02f, Tuning.F("ricochet.open", 0.11f)));
				ulong num = (ulong)(Tuning.F("ricochet.idle", 1.2f) * 1000f);
				ulong num2 = (ulong)(Tuning.F("ricochet.never", 4f) * 1000f);
				if ((_hits > 0) ? (now - _lastHitMs > num) : (now - _beganMs > num2))
				{
					Finish(atOnce: false);
				}
			}
			else
			{
				if (now >= _arriveMs)
				{
					_fade = Mathf.Max(0f, _fade - dt / Mathf.Max(0.02f, Tuning.F("ricochet.close", 0.22f)));
				}
				if (_fade <= 0f && now - _endedMs > 700)
				{
					Drop();
					return;
				}
			}
			Draw(dt);
		}

		static RicochetRun()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		}
	}
