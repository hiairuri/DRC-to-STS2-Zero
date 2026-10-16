// namespace: SilentFX.Powers
// type: ThornsFx (class)
// source lines: 14601-15291
	public sealed class ThornsFx : BasicPowerFx
	{
		private sealed class Spine
		{
			public Node2D Near;

			public Node2D Far;

			public float Angle;

			public float Order;
		}

		private sealed class Jab
		{
			public Node2D Node;

			public float Fan;

			public float Long;
		}

		private sealed class Shove
		{
			public Creature Who;

			public Node2D Body;

			public Vector2 Home;

			public Vector2 Wrote;

			public Vector2 Moved;

			public Vector2 At;

			public Vector2 Speed;
		}

		private const float ShoveSpring = 1000f;

		private const float ShoveDamp = 32f;

		private const float ShoveMax = 36f;

		private static readonly Color Light = new Color(1f, 0.74f, 0.36f, 1f);

		private static readonly Color Dark = new Color(0.88f, 0.42f, 0.12f, 1f);

		private static readonly Color Hot = new Color(1f, 0.52f, 0.16f, 1f);

		private static readonly HashSet<Creature> Abrasive = new HashSet<Creature>(ReferenceEqualityComparer.Instance);

		private readonly List<Spine> _spines = new List<Spine>();

		private readonly List<Jab> _jabs = new List<Jab>();

		private readonly List<Shove> _shoves = new List<Shove>();

		private float _ring = 10f;

		private float _sinceHit = 10f;

		private Vector2 _attack = new Vector2(1f, 0f);

		private Vector2 _centre;

		private Vector2 _body = new Vector2(200f, 300f);

		private float _unit = 1f;

		protected override string NodeName => "Thorns";

		public static void Mark(Creature owner)
		{
			if (owner != null)
			{
				Abrasive.Add(owner);
			}
		}

		public static bool FromAbrasive(Creature owner)
		{
			if (owner != null)
			{
				return Abrasive.Contains(owner);
			}
			return false;
		}

		public static void Reset()
		{
			Abrasive.Clear();
		}

		protected override void Build(bool animateIn)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			Color val = new Color(0f, 0f, 0f, 0f);
			float num = Tuning.F("thorns.halfwidth", 5f);
			int num2 = Mathf.Clamp(Tuning.I("thorns.ring_count", 12), 6, 24);
			float length = Tuning.F("thorns.ring_size", 92f);
			for (int i = 0; i < num2; i++)
			{
				Node2D val2 = FxKit.Blade(length, num, Light, Dark, val, val);
				Node2D val3 = FxKit.Blade(length, num, Light, Dark, val, val);
				((CanvasItem)val2).Visible = false;
				((CanvasItem)val3).Visible = false;
				((Node)Front).AddChild((Node)(object)val2, false, (InternalMode)0);
				((Node)Back).AddChild((Node)(object)val3, false, (InternalMode)0);
				_spines.Add(new Spine
				{
					Near = val2,
					Far = val3,
					Angle = (float)Math.PI * 2f * (float)i / (float)num2 + 1.2f,
					Order = (float)i / (float)num2
				});
			}
			float num3 = Tuning.F("thorns.jab_size", 74f);
			for (int j = -1; j <= 1; j++)
			{
				float num4 = ((j == 0) ? 1f : 0.7f);
				Node2D val4 = FxKit.Blade(num3 * num4, num * 1.15f, Light, Dark, val, val);
				((CanvasItem)val4).Visible = false;
				((Node)Front).AddChild((Node)(object)val4, false, (InternalMode)0);
				_jabs.Add(new Jab
				{
					Node = val4,
					Fan = j,
					Long = num4
				});
			}
			_ring = (animateIn ? 0f : 10f);
		}

		protected override void OnStacked()
		{
			_ring = 0f;
		}

		protected override void Tick(float dt)
		{
			_ring += dt;
			_sinceHit += dt;
			TickShoves(dt);
		}

		private void Measure()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			_body = Anchor.SafeBodySize();
			_centre = Anchor.Chest();
			_unit = Mathf.Clamp(Mathf.Min(_body.X, _body.Y) / 183f, 0.75f, 1.6f);
		}

		private static float Pop(float t, float outTime, float holdTime, float backTime)
		{
			if (t <= 0f)
			{
				return 0f;
			}
			if (t < outTime)
			{
				float num = t / outTime;
				return Mathf.Sin(num * (float)Math.PI * 0.5f) * (1f + 0.3f * Mathf.Sin(num * (float)Math.PI));
			}
			if (t < outTime + holdTime)
			{
				return 1f;
			}
			float num2 = 1f - (t - outTime - holdTime) / Mathf.Max(0.01f, backTime);
			if (!(num2 <= 0f))
			{
				return num2 * num2;
			}
			return 0f;
		}

		private float Thrust()
		{
			return Pop(_sinceHit, Tuning.F("thorns.hit_out", 0.05f), Tuning.F("thorns.hit_hold", 0.1f), Tuning.F("thorns.hit_back", 0.24f));
		}

		protected override void Layout()
		{
			if (FxKit.Alive((GodotObject)(object)Back) && FxKit.Alive((GodotObject)(object)Front))
			{
				Measure();
				LayoutRing();
				LayoutJabs();
			}
		}

		private void LayoutRing()
		{
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			float num = Tuning.F("thorns.ring_sweep", 0.1f);
			float num2 = Tuning.F("thorns.ring_out", 0.06f);
			float num3 = Tuning.F("thorns.ring_hold", 0.14f);
			float num4 = Tuning.F("thorns.ring_back", 0.22f);
			if (!(_ring < num + num2 + num3 + num4))
			{
				foreach (Spine spine in _spines)
				{
					if (FxKit.Alive((GodotObject)(object)spine.Near) && ((CanvasItem)spine.Near).Visible)
					{
						((CanvasItem)spine.Near).Visible = false;
					}
					if (FxKit.Alive((GodotObject)(object)spine.Far) && ((CanvasItem)spine.Far).Visible)
					{
						((CanvasItem)spine.Far).Visible = false;
					}
				}
				return;
			}
			float num5 = Mathf.Clamp(_body.X * 0.5f * Tuning.F("thorns.ring", 1.35f), 60f, 320f);
			float num6 = Mathf.Clamp(Tuning.F("thorns.ring_flat", 0.3f), 0.05f, 1f);
			float num7 = num5 * num6;
			Vector2 val = _centre + new Vector2(0f, _body.Y * Tuning.F("thorns.ring_y", 0.08f));
			float num8 = _ring * Tuning.F("thorns.ring_turn", 4.5f) * Anchor.Side();
			float num9 = Mathf.Clamp(Tuning.F("thorns.far_alpha", 0.55f), 0f, 1f);
			float num10 = (Stopping ? A : 1f);
			foreach (Spine spine2 in _spines)
			{
				if (FxKit.Alive((GodotObject)(object)spine2.Near) && FxKit.Alive((GodotObject)(object)spine2.Far))
				{
					float num11 = Pop(_ring - spine2.Order * num, num2, num3, num4);
					if (num11 <= 0.002f)
					{
						((CanvasItem)spine2.Near).Visible = false;
						((CanvasItem)spine2.Far).Visible = false;
						continue;
					}
					float num12 = spine2.Angle * Anchor.Side() + num8;
					float num13 = Mathf.Cos(num12);
					float num14 = Mathf.Sin(num12);
					bool flag = num14 >= 0f;
					Node2D val2 = (flag ? spine2.Near : spine2.Far);
					((CanvasItem)spine2.Near).Visible = flag;
					((CanvasItem)spine2.Far).Visible = !flag;
					Vector2 val3 = new Vector2(num13, num14 * num6);
					float num15 = val3.Length();
					Vector2 val4 = (Vector2)((num15 > 0.01f) ? (val3 / num15) : new Vector2(1f, 0f));
					float num16 = Mathf.Lerp(0.5f, 1f, Mathf.Clamp((num15 - num6) / Mathf.Max(0.01f, 1f - num6), 0f, 1f));
					float num17 = (1f + 0.14f * num14) * _unit;
					val2.GlobalPosition = val + new Vector2(num13 * num5, num14 * num7);
					val2.Rotation = val4.Angle() + (float)Math.PI / 2f;
					val2.Scale = new Vector2(num17 * Mathf.Min(1f, num11 * 1.6f), num17 * num16 * num11);
					float num18 = 1f + 0.35f * Mathf.Clamp(num11, 0f, 1f);
					((CanvasItem)val2).Modulate = new Color(num18, num18, num18, num10 * (flag ? 1f : num9) * Mathf.Min(1f, num11 * 2.5f));
				}
			}
		}

		private void LayoutJabs()
		{
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			float num = Thrust();
			if (num <= 0.002f)
			{
				foreach (Jab jab in _jabs)
				{
					if (FxKit.Alive((GodotObject)(object)jab.Node) && ((CanvasItem)jab.Node).Visible)
					{
						((CanvasItem)jab.Node).Visible = false;
					}
				}
				return;
			}
			float num2 = Tuning.F("thorns.jab_fan", 0.34f);
			float num3 = Tuning.F("thorns.hit_lunge", 22f) * _unit;
			Vector2 val = JabRoot();
			Vector2 val2 = new Vector2(0f - _attack.Y, _attack.X);
			float num4 = _attack.Angle();
			float num5 = Mathf.Clamp(num, 0f, 1f);
			foreach (Jab jab2 in _jabs)
			{
				if (FxKit.Alive((GodotObject)(object)jab2.Node))
				{
					float num6 = num4 + jab2.Fan * num2;
					Vector2 val3 = new Vector2(Mathf.Cos(num6), Mathf.Sin(num6));
					((CanvasItem)jab2.Node).Visible = true;
					jab2.Node.GlobalPosition = val + val2 * (jab2.Fan * 15f * _unit) + val3 * (num3 * num5);
					jab2.Node.Rotation = num6 + (float)Math.PI / 2f;
					jab2.Node.Scale = new Vector2(_unit * (1f + 0.3f * num5) * Mathf.Min(1f, num * 2f), _unit * num);
					float num7 = 1f + 0.5f * num5;
					((CanvasItem)jab2.Node).Modulate = new Color(num7, num7, num7, A * Mathf.Min(1f, num * 3f));
				}
			}
		}

		private Vector2 JabRoot()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			return _centre + new Vector2(_attack.X * _body.X * 0.28f, _attack.Y * _body.Y * 0.22f);
		}

		protected override void OnTrigger(Vector2? at)
		{
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			_sinceHit = 0f;
			Measure();
			Vector2 where = at ?? (_centre + new Vector2(_body.X * 2.2f * Anchor.Side(), 0f));
			Vector2 val = where - _centre;
			Vector2 val2 = (Vector2)((val.LengthSquared() > 400f) ? val.Normalized() : new Vector2(Anchor.Side(), 0f));
			val2 = new Vector2(val2.X, val2.Y * 0.45f);
			_attack = (Vector2)((val2.LengthSquared() > 0.0004f) ? val2.Normalized() : new Vector2(Anchor.Side(), 0f));
			Node parent = Anchor.FrontParent;
			if (!FxKit.Alive((GodotObject)(object)parent))
			{
				return;
			}
			float num = (Tuning.F("thorns.jab_size", 74f) + Tuning.F("thorns.hit_lunge", 22f)) * _unit;
			FxKit.Burst(parent, JabRoot() + _attack * num, new Color(1f, 0.72f, 0.36f, 0.95f), 4, 140f, 340f, 0.28, 0.05f, 0.12f, new Vector2(0f, 480f), 26f, _attack, 4f);
			if (!at.HasValue && Anchor.Owner != null)
			{
				return;
			}
			object key = null;
			float width = 200f;
			if (at.HasValue)
			{
				float num2 = 4900f;
				foreach (FxTarget item in Anchor.SafeTargets())
				{
					float num3 = item.Chest.DistanceSquaredTo(where);
					if (!(num3 >= num2))
					{
						num2 = num3;
						key = item.Key;
						width = item.Width;
					}
				}
			}
			Vector2 struck = _attack;
			int num4;
			if (MultiplayerFx.Active)
			{
				object owner = Anchor.Owner;
				Creature val3 = (Creature)((owner is Creature) ? owner : null);
				if (val3 != null && val3.IsPlayer)
				{
					num4 = ((!MultiplayerFx.IsLocal(val3)) ? 1 : 0);
					goto IL_02ab;
				}
			}
			num4 = 0;
			goto IL_02ab;
			IL_02ab:
			bool remoteFade = (byte)num4 != 0;
			FxKit.After(parent, Tuning.F("thorns.hit_out", 0.05f), () =>
			{
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				//IL_0067: Unknown result type (might be due to invalid IL or missing references)
				if (Dead || !FxKit.Alive((GodotObject)(object)parent) || !FxKit.Alive((GodotObject)(object)Front))
				{
					return;
				}
				using (RemoteFxFade.Capture(remoteFade, parent))
				{
					Answer(parent, where, key, width, struck);
				}
			});
		}

		private void Answer(Node parent, Vector2 where, object key, float width, Vector2 dir)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			float num = Tuning.F("thorns.hit_flash", 1.2f) * _unit;
			if (num > 0.01f)
			{
				Vector2 val = where - dir * Mathf.Min(width * 0.18f, 70f * _unit);
				FxKit.Flash(parent, val, new Color(Hot.R, Hot.G, Hot.B, 0.6f), 90f * num, 1.9f, 0.22);
				FxKit.Burst(parent, val, new Color(Hot.R, Hot.G, Hot.B, 0.95f), 12, 180f, 480f, 0.32, 0.05f, 0.13f, new Vector2(0f, 520f), 52f, dir, 6f);
			}
			Creature val2 = (Creature)((key is Creature) ? key : null);
			if (val2 != null)
			{
				Flinch(val2, dir);
			}
		}

		private void Flinch(Creature who, Vector2 dir)
		{
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				float num = Mathf.Clamp(Tuning.F("thorns.flinch", 1f), 0f, 3f);
				if (num <= 0.01f || who == null || !who.IsAlive)
				{
					return;
				}
				float num2 = Tuning.F("thorns.flinch_hold", 0.08f);
				if (num2 > 0.005f)
				{
					CreatureHold.Hold(who, Mathf.Min(num2, 0.4f));
				}
				NCombatRoom combatRoom = GameRefs.CombatRoom;
				NCreature val = ((combatRoom != null) ? combatRoom.GetCreatureNode(who) : null);
				if (!FxKit.Alive((GodotObject)(object)val) || !((Node)val).IsInsideTree())
				{
					return;
				}
				Node2D body = val.Body;
				if (!FxKit.Alive((GodotObject)(object)body))
				{
					return;
				}
				Shove shove = null;
				foreach (Shove shofe in _shoves)
				{
					if (shofe.Who == who && shofe.Body == body)
					{
						shove = shofe;
					}
				}
				if (shove == null)
				{
					shove = new Shove
					{
						Who = who,
						Body = body,
						Home = body.Position,
						Wrote = body.Position
					};
					_shoves.Add(shove);
				}
				Shove shove2 = shove;
				shove2.Speed += new Vector2((dir.X < 0f) ? (-1f) : 1f, 0f) * (Tuning.F("thorns.flinch_kick", 720f) * num * _unit);
			}
			catch (Exception ex)
			{
				DevLog.Warn("ThornsFx.Flinch: " + ex.Message);
			}
		}

		private void TickShoves(float dt)
		{
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			for (int num = _shoves.Count - 1; num >= 0; num--)
			{
				Shove shove = _shoves[num];
				if (!FxKit.Alive((GodotObject)(object)shove.Body))
				{
					_shoves.RemoveAt(num);
				}
				else
				{
					int num2 = Mathf.Clamp(Mathf.CeilToInt(dt / 0.006f), 1, 24);
					float num3 = dt / (float)num2;
					for (int i = 0; i < num2; i++)
					{
						shove.Speed += (-1000f * shove.At - 32f * shove.Speed) * num3;
						shove.At += shove.Speed * num3;
					}
					float num4 = shove.At.Length();
					if (!float.IsFinite(num4) || !float.IsFinite(shove.Speed.X) || !float.IsFinite(shove.Speed.Y) || (num4 < 0.3f && shove.Speed.LengthSquared() < 100f))
					{
						Nudge(shove, Vector2.Zero);
						_shoves.RemoveAt(num);
					}
					else
					{
						if (num4 > 36f)
						{
							shove.At *= 36f / num4;
						}
						Nudge(shove, shove.At);
					}
				}
			}
		}

		private static void Nudge(Shove shove, Vector2 offset)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)shove.Body))
			{
				return;
			}
			Vector2 val = Vector2.Zero;
			if (offset != Vector2.Zero)
			{
				val = offset;
				Node parent = ((Node)shove.Body).GetParent();
				CanvasItem val2 = (CanvasItem)(object)((parent is CanvasItem) ? parent : null);
				if (val2 != null && FxKit.Alive((GodotObject)(object)val2))
				{
					Transform2D val3 = val2.GetGlobalTransform();
					val3 = val3.AffineInverse();
					val = val3.BasisXform(offset);
				}
				if (!float.IsFinite(val.X) || !float.IsFinite(val.Y))
				{
					val = Vector2.Zero;
				}
			}
			Vector2 position = shove.Body.Position;
			if (position != shove.Wrote)
			{
				shove.Home = position - shove.Moved;
			}
			else if (val == shove.Moved)
			{
				return;
			}
			shove.Body.Position = shove.Home + val;
			shove.Wrote = shove.Body.Position;
			shove.Moved = val;
		}

		private void ReleaseShoves()
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			for (int num = _shoves.Count - 1; num >= 0; num--)
			{
				try
				{
					Nudge(_shoves[num], Vector2.Zero);
				}
				catch (Exception ex)
				{
					DevLog.Warn("ThornsFx (attacker): " + ex.Message);
				}
			}
			_shoves.Clear();
		}

		protected override void OnDispose()
		{
			ReleaseShoves();
		}

		public ThornsFx()
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		}

		static ThornsFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		}
	}
