// namespace: SilentFX.Cards
// type: ShivHeld (class)
// source lines: 99211-100286
	public sealed class ShivHeld : HeldSession, IShivSource
	{
		private sealed class Blade
		{
			public Node2D Node;

			public Vector2 Pos;

			public float Rot;

			public bool Placed;

			public bool Taken;

			public float Beat = -1f;

			public bool Borrowed;
		}

		private sealed class FanBlade
		{
			public Node2D Node;

			public float Rot;

			public float Show;

			public bool Going;
		}

		private static readonly StringName PAlpha = StringName.op_Implicit("alpha");

		private static readonly StringName PSpin = StringName.op_Implicit("spin");

		private static readonly Color Steel = new Color(0.75f, 0.92f, 1f, 1f);

		private static readonly Color Pale = new Color(1f, 0.92f, 0.7f, 1f);

		private static readonly Color InkGlow = new Color(0.55f, 0.34f, 0.95f, 1f);

		private readonly ShivStyle _style;

		private readonly int _previewCount;

		private Blade[] _blades = Array.Empty<Blade>();

		private float _spin;

		private float _beat;

		private bool _lunged;

		private CardModel _borrowedFor;

		private bool _fan;

		private float _open;

		private Vector2 _hilt;

		private readonly List<FanBlade> _extra = new List<FanBlade>();

		private readonly List<FxTarget> _fanTargets = new List<FxTarget>();

		private readonly List<float> _fanAngles = new List<float>();

		private Node2D _ring;

		private ShaderMaterial _ringMat;

		private Sprite2D _aura;

		private Node2D _cloak;

		private Line2D _cloth;

		private Line2D _clothEdge;

		private float _cloakOpen;

		private float _cloakHem = 0.55f;

		private PhantomBladesFx.RemoteLoan _remoteLoan;

		private float _playingFor;

		public ShivHeld()
			: this(ShivStyle.Plain, 3)
		{
		}

		public ShivHeld(ShivStyle style, int previewCount)
		{
			_style = style;
			_previewCount = previewCount;
		}

		private int CountFor(CardModel card)
		{
			if (_style == ShivStyle.Return)
			{
				return 1;
			}
			if (_style == ShivStyle.Symphony)
			{
				return 2;
			}
			if (card == null)
			{
				return _previewCount;
			}
			if (_style == ShivStyle.Storm)
			{
				try
				{
					CardPile pile = PileTypeExtensions.GetPile((PileType)2, card.Owner);
					return Mathf.Clamp(((pile == null) ? 1 : pile.Cards.Count) - 1, 0, 12);
				}
				catch
				{
					return 4;
				}
			}
			return Mathf.Clamp(ShivFx.CountFor(card, _previewCount), 1, 8);
		}

		protected override void Build()
		{
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			_fan = _style == ShivStyle.Return && C.Card is Shiv && ShivFan.Active(C.Card);
			if (_style == ShivStyle.Return && C.Card is Shiv && FxKit.Alive((GodotObject)(object)C.FrontParent) && (C.Remote ? PhantomBladesFx.TryLendRemote(C.Card, out var blade, out _remoteLoan) : PhantomBladesFx.TryLend(C.Card, out blade)))
			{
				Vector2 globalPosition = blade.GlobalPosition;
				float globalRotation = blade.GlobalRotation;
				Node parent = ((Node)blade).GetParent();
				if (parent != null)
				{
					parent.RemoveChild((Node)(object)blade);
				}
				C.FrontParent.AddChild((Node)(object)blade, false, (InternalMode)0);
				blade.GlobalPosition = globalPosition;
				blade.GlobalRotation = globalRotation;
				_blades = new Blade[1]
				{
					new Blade
					{
						Node = blade,
						Pos = globalPosition,
						Rot = globalRotation,
						Placed = true,
						Borrowed = true
					}
				};
				_borrowedFor = C.Card;
				return;
			}
			int num = ((_style != ShivStyle.Cloak) ? CountFor(C.Card) : 0);
			int num2;
			if (_style != ShivStyle.Ink)
			{
				CardModel card = C.Card;
				num2 = ((((card != null) ? card.Enchantment : null) is Inky) ? 1 : 0);
			}
			else
			{
				num2 = 1;
			}
			bool flag = (byte)num2 != 0;
			_blades = new Blade[num];
			for (int i = 0; i < num; i++)
			{
				Node2D val = ShivFx.MakeShiv(flag);
				((CanvasItem)val).Visible = false;
				((Node)Front).AddChild((Node)(object)val, false, (InternalMode)0);
				if (flag)
				{
					((Node)val).AddChild((Node)(object)InkDrip(), false, (InternalMode)0);
				}
				_blades[i] = new Blade
				{
					Node = val
				};
			}
			Vector2 body = C.Body;
			switch (_style)
			{
			case ShivStyle.Dance:
				_ring = FloorRing((Node)(object)Back, body.X * 1.7f, Steel, 9f, out _ringMat);
				_ring.GlobalPosition = C.Feet;
				break;
			case ShivStyle.Storm:
				_ring = FloorRing((Node)(object)Back, body.X * 2.3f + 120f, Steel, 14f, out _ringMat);
				_ring.GlobalPosition = StormCentre();
				break;
			case ShivStyle.Cloak:
				_cloak = ShivFx.MakeCloak(C, out _cloth, out _clothEdge);
				((Node)Front).AddChild((Node)(object)_cloak, false, (InternalMode)0);
				break;
			case ShivStyle.Ink:
				_aura = FxKit.Glow(new Color(InkGlow.R, InkGlow.G, InkGlow.B, 0.3f), new Vector2(230f, 190f));
				((Node)Back).AddChild((Node)(object)_aura, false, (InternalMode)0);
				((Node2D)_aura).GlobalPosition = InkCentre() + new Vector2(0f, -20f);
				break;
			}
		}

		private static Node2D FloorRing(Node parent, float size, Color tint, float dashes, out ShaderMaterial mat)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			Node2D val = FxKit.Sigil(size, tint, out mat);
			parent.AddChild((Node)(object)val, false, (InternalMode)0);
			val.Scale = new Vector2(1f, 0.3f);
			if (mat != null)
			{
				mat.SetShaderParameter(StringName.op_Implicit("radius"), Variant.op_Implicit(0.82f));
				mat.SetShaderParameter(StringName.op_Implicit("width"), Variant.op_Implicit(0.022f));
				mat.SetShaderParameter(StringName.op_Implicit("dashes"), Variant.op_Implicit(dashes));
				mat.SetShaderParameter(StringName.op_Implicit("glow"), Variant.op_Implicit(0.35f));
				mat.SetShaderParameter(PAlpha, Variant.op_Implicit(0f));
			}
			return val;
		}

		private static GpuParticles2D InkDrip()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected Obj, but got Unknown
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Expected Obj, but got Unknown
			ParticleProcessMaterial val = new ParticleProcessMaterial();
			val.EmissionShape = (EmissionShapeEnum)3;
			val.EmissionBoxExtents = new Vector3(4f, 18f, 0f);
			val.Direction = new Vector3(0f, 1f, 0f);
			val.Spread = 12f;
			val.InitialVelocityMin = 10f;
			val.InitialVelocityMax = 50f;
			val.Gravity = new Vector3(0f, 620f, 0f);
			val.ScaleMin = 0.1f;
			val.ScaleMax = 0.22f;
			val.Color = new Color(0.2f, 0.1f, 0.42f, 0.95f);
			val.ColorRamp = (Texture2D)(object)FxKit.Ramp(new float[3] { 0f, 0.7f, 1f }, new Color[3]
			{
				new Color(1f, 1f, 1f, 1f),
				new Color(1f, 1f, 1f, 0.8f),
				new Color(1f, 1f, 1f, 0f)
			});
			val.ParticleFlagDisableZ = true;
			ParticleProcessMaterial processMaterial = val;
			return new GpuParticles2D
			{
				Amount = 7,
				Lifetime = 0.55,
				Randomness = 0.6f,
				LocalCoords = false,
				Texture = FxKit.SoftDot,
				ProcessMaterial = (Material)(object)processMaterial,
				Position = new Vector2(0f, -20f),
				VisibilityRect = new Rect2(-400f, -400f, 800f, 1200f)
			};
		}

		private Vector2 StormCentre()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			return C.Chest + new Vector2(0f, (0f - C.Body.Y) * 0.62f - 20f);
		}

		private Vector2 InkCentre()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			return C.WeaponHand + new Vector2(C.Body.X * 0.24f * C.Facing, -14f);
		}

		private static float Spread(int index)
		{
			if (index <= 0)
			{
				return 0f;
			}
			int num = (index + 1) / 2;
			return (index % 2 == 1) ? num : (-num);
		}

		private static float Facing(Vector2 dir)
		{
			return dir.Angle() + (float)Math.PI / 2f;
		}

		protected override void Tick(float dt)
		{
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0904: Unknown result type (might be due to invalid IL or missing references)
			//IL_0924: Unknown result type (might be due to invalid IL or missing references)
			//IL_0929: Unknown result type (might be due to invalid IL or missing references)
			//IL_092e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04da: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0503: Unknown result type (might be due to invalid IL or missing references)
			//IL_0515: Unknown result type (might be due to invalid IL or missing references)
			//IL_051a: Unknown result type (might be due to invalid IL or missing references)
			//IL_051f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0521: Unknown result type (might be due to invalid IL or missing references)
			//IL_072d: Unknown result type (might be due to invalid IL or missing references)
			//IL_075d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0762: Unknown result type (might be due to invalid IL or missing references)
			//IL_077f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07db: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0797: Unknown result type (might be due to invalid IL or missing references)
			//IL_079c: Unknown result type (might be due to invalid IL or missing references)
			//IL_079e: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0946: Unknown result type (might be due to invalid IL or missing references)
			//IL_0948: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0428: Unknown result type (might be due to invalid IL or missing references)
			//IL_042d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_0449: Unknown result type (might be due to invalid IL or missing references)
			//IL_054d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0557: Unknown result type (might be due to invalid IL or missing references)
			//IL_0586: Unknown result type (might be due to invalid IL or missing references)
			//IL_058b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0590: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_080c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0811: Unknown result type (might be due to invalid IL or missing references)
			//IL_0833: Unknown result type (might be due to invalid IL or missing references)
			//IL_0838: Unknown result type (might be due to invalid IL or missing references)
			//IL_0885: Unknown result type (might be due to invalid IL or missing references)
			//IL_088a: Unknown result type (might be due to invalid IL or missing references)
			//IL_088f: Unknown result type (might be due to invalid IL or missing references)
			//IL_08aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_08af: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_08bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0969: Unknown result type (might be due to invalid IL or missing references)
			//IL_097d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0982: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0608: Unknown result type (might be due to invalid IL or missing references)
			//IL_060d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0612: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a03: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a2e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a50: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e5: Unknown result type (might be due to invalid IL or missing references)
			if (Playing && !Leaving)
			{
				_playingFor += dt;
				if (_playingFor > Mathf.Max(3f, (float)_blades.Length * 0.5f + 1f))
				{
					Leave();
				}
			}
			if (_fan)
			{
				TickFan(dt);
				return;
			}
			int num = _blades.Length;
			float arm = Arm;
			float num2 = (Playing ? 1f : 0f);
			Vector2 body = C.Body;
			Vector2 chest = C.Chest;
			Vector2 weaponHand = C.WeaponHand;
			float wind = _style switch
			{
				ShivStyle.Dance => Mathf.Lerp(1.5f, 4.6f, arm) + 3.5f * num2, 
				ShivStyle.Storm => Mathf.Lerp(2.2f, 6.2f, arm) + 5f * num2, 
				_ => 1f, 
			};
			float num3 = wind;
			_spin += num3 * dt;
			if (_style == ShivStyle.Symphony && Time > 0.24f)
			{
				_beat += dt * Mathf.Lerp(1f, Mathf.Max(0.2f, Tuning.F("symphony.armed_tempo", 1.18f)), arm) / SymphonyTurn(out wind, out var _, out var _);
			}
			for (int i = 0; i < num; i++)
			{
				Blade blade = _blades[i];
				if (blade.Taken || !FxKit.Alive((GodotObject)(object)blade.Node))
				{
					continue;
				}
				float num4 = 1f;
				float num5 = 1f;
				bool flag = true;
				Vector2 val;
				float num6;
				switch (_style)
				{
				case ShivStyle.Dance:
				{
					Vector2 val7 = chest + new Vector2(0f, body.Y * 0.06f);
					float num16 = (body.X * 0.48f + 30f) * (1f - 0.12f * arm);
					float num17 = _spin + (float)Math.PI * 2f * (float)i / (float)num;
					val = val7 + new Vector2(Mathf.Cos(num17) * num16, Mathf.Sin(num17) * num16 * 0.32f + Mathf.Sin(Time * 3f + (float)i) * 4f);
					num6 = Facing(new Vector2(0f - Mathf.Sin(num17), Mathf.Cos(num17) * 0.32f));
					num4 = ((Mathf.Sin(num17) < 0f) ? 0.5f : 1f);
					flag = false;
					break;
				}
				case ShivStyle.Storm:
				{
					Vector2 val6 = StormCentre();
					float num13 = (body.X * 0.55f + 50f) * (1f - 0.18f * arm);
					float num14 = _spin + (float)Math.PI * 2f * (float)i / (float)num;
					float num15 = Mathf.Sin(Time * 1.3f) * 0.1f;
					val = val6 + new Vector2(Mathf.Cos(num14) * num13, Mathf.Sin(num14) * num13 * 0.3f + Mathf.Cos(num14) * num13 * num15);
					num6 = Facing(new Vector2(0f - Mathf.Sin(num14), Mathf.Cos(num14) * 0.3f));
					num4 = ((Mathf.Sin(num14) < 0f) ? 0.55f : 1f);
					flag = false;
					break;
				}
				case ShivStyle.Sleeves:
				{
					float num18 = ((i % 2 == 0) ? 1f : (-1f)) * C.Facing;
					int num19 = i / 2;
					val = chest + new Vector2(num18 * body.X * 0.2f, body.Y * 0.14f) + new Vector2(num18 * (6f * (float)num19 + 20f * arm), 6f + 10f * (float)num19 + 30f * arm + Mathf.Sin(Time * 2.4f + (float)i * 2f) * 2f);
					num6 = Facing(new Vector2(num18 * (0.1f + 0.25f * arm), 1f));
					num4 = 0.55f + 0.45f * arm;
					num5 = 0.55f + 0.45f * arm;
					break;
				}
				case ShivStyle.Fan:
				{
					float num20 = Mathf.Sin(Time * 2f) * 2.5f;
					float num21 = Mathf.DegToRad(-12f + num20 + Spread(i) * Mathf.Lerp(5f, 15f, arm));
					Vector2 val12 = new Vector2(Mathf.Cos(num21) * C.Facing, Mathf.Sin(num21));
					val = weaponHand + new Vector2(body.X * 0.3f * C.Facing, -6f) + val12 * Mathf.Lerp(24f, 56f, arm);
					num6 = Facing(val12);
					break;
				}
				case ShivStyle.Symphony:
				{
					float num8 = ((i == 0) ? (-1f) : 1f) * C.Facing;
					Vector2 val5 = chest + new Vector2(num8 * 46f, (0f - body.Y) * 0.7f + Mathf.Sin(Time * 1.7f + (float)i * 2.4f) * 1.5f);
					float num9 = SymphonyTurn(out var wind2, out var air2, out var settle2);
					float num10 = Mathf.Floor(_beat);
					float num11 = (((int)num10 % num == i) ? ((_beat - num10) * num9) : (-1f));
					float height = Tuning.F("symphony.hop_height", 46f) * (1f + 0.18f * arm);
					float num12 = HopHeight(num11, wind2, air2, settle2, height, out var squash, out var lift);
					val = val5 + new Vector2(0f, 0f - num12);
					num6 = num8 * 0.14f * (1f - lift * lift * (3f - 2f * lift));
					num5 = 1f - 0.16f * squash;
					flag = false;
					if (num11 >= wind2 + air2 && blade.Beat >= 0f && blade.Beat < wind2 + air2 && blade.Placed && !Leaving && !Playing)
					{
						FxKit.RingPulse(C.FrontParent, val5 + new Vector2(0f, 20f), 150f, new Color(Pale.R, Pale.G, Pale.B, 1f), 0f, 0.58f, 0.25f, 1.5f, 1f, 0f, 0.03f, 0.45f + 0.3f * arm);
					}
					blade.Beat = num11;
					break;
				}
				case ShivStyle.Return:
				{
					Vector2 val8 = weaponHand + new Vector2(10f * C.Facing, -14f + Mathf.Sin(Time * 2.3f) * 3f);
					Vector2 val9 = new Vector2(0.45f * C.Facing, -0.89f);
					if (C.HasTarget)
					{
						Vector2 val10 = C.TargetChest - weaponHand;
						if (val10.LengthSquared() > 1f)
						{
							Vector2 val11 = val9.Lerp(val10.Normalized(), arm);
							val9 = val11.Normalized();
						}
					}
					val = val8 - val9 * (16f * arm);
					num6 = Facing(val9);
					break;
				}
				case ShivStyle.Ink:
				{
					float num7 = Spread(i) - ((num % 2 == 0) ? 0.5f : 0f);
					Vector2 val2 = InkCentre();
					Vector2 val3 = val2 + new Vector2(num7 * 56f * C.Facing, -10f * Mathf.Abs(num7)) + new Vector2(Mathf.Sin(Time * 2.2f + (float)i * 2f) * 6f * C.Facing, Mathf.Cos(Time * 1.7f + (float)i) * 5f);
					Vector2 val4 = val2 + new Vector2(num7 * 38f * C.Facing, -26f);
					val = val3.Lerp(val4, arm);
					num6 = (Mathf.Sin(Time * 1.4f + (float)i * 1.7f) * 0.22f * (1f - arm) + num7 * 0.16f) * C.Facing;
					break;
				}
				default:
					val = weaponHand + new Vector2(Spread(i) * 20f * C.Facing, -30f);
					num6 = 0f;
					break;
				}
				if (!blade.Placed || !flag)
				{
					blade.Pos = val;
					blade.Rot = num6;
					blade.Placed = true;
				}
				else
				{
					blade.Pos = blade.Pos.Lerp(val, 1f - Mathf.Exp(-14f * dt));
					blade.Rot = Mathf.LerpAngle(blade.Rot, num6, 1f - Mathf.Exp(-14f * dt));
				}
				float num22 = (blade.Borrowed ? 1f : Mathf.Clamp((Time - (float)i * 0.04f) / 0.14f, 0f, 1f));
				((CanvasItem)blade.Node).Visible = num22 > 0.01f;
				blade.Node.GlobalPosition = blade.Pos;
				blade.Node.Rotation = blade.Rot;
				blade.Node.Scale = new Vector2(num22, num22 * num5);
				((CanvasItem)blade.Node).Modulate = new Color(1f, 1f, 1f, num4);
			}
			TickExtras(dt, arm);
		}

		private void TickExtras(float dt, float arm)
		{
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)_ring))
			{
				if (_style == ShivStyle.Storm)
				{
					_ring.GlobalPosition = StormCentre();
				}
				if (FxKit.Alive((GodotObject)(object)_ringMat))
				{
					_ringMat.SetShaderParameter(PAlpha, Variant.op_Implicit(0.22f + 0.5f * arm));
					_ringMat.SetShaderParameter(PSpin, Variant.op_Implicit(0f - _spin));
				}
			}
			if (FxKit.Alive((GodotObject)(object)_cloak))
			{
				_cloakOpen = -0.16f * arm + 0.04f * Mathf.Sin(Time * 1.9f);
				_cloakHem = 0.55f + 0.45f * arm;
				_cloak.GlobalPosition = ShivFx.CloakRoot(C);
				ShivFx.PoseCloak(_cloak, _cloth, C.Facing, _cloakOpen);
				if (FxKit.Alive((GodotObject)(object)_clothEdge))
				{
					((CanvasItem)_clothEdge).Modulate = new Color(1f, 1f, 1f, _cloakHem);
				}
			}
			if (FxKit.Alive((GodotObject)(object)_aura))
			{
				((Node2D)_aura).GlobalPosition = InkCentre() + new Vector2(0f, -20f);
				((CanvasItem)_aura).Modulate = new Color(InkGlow.R, InkGlow.G, InkGlow.B, 0.22f + 0.1f * Mathf.Sin(Time * 3f) + 0.25f * arm);
			}
		}

		private void TickFan(float dt)
		{
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0516: Unknown result type (might be due to invalid IL or missing references)
			//IL_0538: Unknown result type (might be due to invalid IL or missing references)
			Blade blade = ((_blades.Length != 0) ? _blades[0] : null);
			if (blade == null || blade.Taken || !FxKit.Alive((GodotObject)(object)blade.Node))
			{
				return;
			}
			float rate = 4f / Mathf.Max(0.03f, Tuning.F("fan.open_time", 0.16f));
			_open = FxKit.Damp(_open, Armed ? 1f : 0f, rate, dt);
			float num = Mathf.SmoothStep(0f, 0.5f, _open);
			float spread = Mathf.SmoothStep(0.35f, 1f, _open);
			Vector2 val = C.WeaponHand + new Vector2(10f * C.Facing, -14f + Mathf.Sin(Time * 2.3f) * 3f);
			Vector2 val2 = new Vector2(0.45f * C.Facing, -0.89f);
			float num2 = val2.Angle();
			CardModel card = C.Card;
			object owner;
			if (card == null)
			{
				owner = null;
			}
			else
			{
				Player owner2 = card.Owner;
				owner = ((owner2 != null) ? owner2.Creature : null);
			}
			ShivFan.FillEnemies((Creature)owner, _fanTargets);
			float middle = ShivFan.Spread(val, _fanTargets, _fanAngles, num2);
			float facing = Mathf.LerpAngle(num2, middle, num);
			_hilt = val - new Vector2(Mathf.Cos(middle), Mathf.Sin(middle)) * (Tuning.F("fan.draw_back", 12f) * num);
			int count = _fanTargets.Count;
			int num3 = Math.Max(0, count - 1);
			int i = 0;
			foreach (FanBlade item in _extra)
			{
				if (!item.Going)
				{
					i++;
				}
			}
			for (; i < num3; i++)
			{
				_extra.Add(MakeFanBlade(blade.Rot));
			}
			int num4 = _extra.Count - 1;
			while (num4 >= 0 && i > num3)
			{
				if (!_extra[num4].Going)
				{
					_extra[num4].Going = true;
					i--;
				}
				num4--;
			}
			int num5 = ((count > 0) ? ((count - 1) / 2) : (-1));
			float num6 = 1f - Mathf.Exp(-18f * dt);
			float num7 = AngleOf(num5) + (float)Math.PI / 2f;
			blade.Rot = ((!blade.Placed) ? num7 : Mathf.LerpAngle(blade.Rot, num7, num6));
			blade.Pos = (blade.Placed ? blade.Pos.Lerp(_hilt, num6) : _hilt);
			blade.Placed = true;
			ShivFan.NoteHeldHilt(C.Card, blade.Pos);
			float num8 = (blade.Borrowed ? 1f : Mathf.Clamp(Time / 0.14f, 0f, 1f));
			((CanvasItem)blade.Node).Visible = num8 > 0.01f;
			blade.Node.GlobalPosition = blade.Pos;
			blade.Node.Rotation = blade.Rot;
			blade.Node.Scale = new Vector2(num8, num8);
			((CanvasItem)blade.Node).Modulate = Colors.White;
			int num9 = 0;
			for (int num10 = _extra.Count - 1; num10 >= 0; num10--)
			{
				FanBlade fanBlade = _extra[num10];
				if (fanBlade.Going && fanBlade.Show < 0.02f)
				{
					if (FxKit.Alive((GodotObject)(object)fanBlade.Node))
					{
						((Node)fanBlade.Node).QueueFree();
					}
					_extra.RemoveAt(num10);
				}
			}
			foreach (FanBlade item2 in _extra)
			{
				if (!FxKit.Alive((GodotObject)(object)item2.Node))
				{
					continue;
				}
				item2.Show = FxKit.Damp(item2.Show, item2.Going ? 0f : 1f, 12f, dt);
				if (!item2.Going)
				{
					if (num9 == num5)
					{
						num9++;
					}
					item2.Rot = Mathf.LerpAngle(item2.Rot, AngleOf(num9) + (float)Math.PI / 2f, num6);
					num9++;
				}
				float num11 = item2.Show * Mathf.Clamp(spread * 3f, 0f, 1f);
				((CanvasItem)item2.Node).Visible = num11 > 0.01f;
				item2.Node.GlobalPosition = blade.Pos;
				item2.Node.Rotation = item2.Rot;
				item2.Node.Scale = new Vector2(num8, num8);
				((CanvasItem)item2.Node).Modulate = new Color(1f, 1f, 1f, num11);
			}
			float AngleOf(int slot)
			{
				if (slot < 0 || slot >= _fanAngles.Count)
				{
					return facing;
				}
				return facing + Mathf.Wrap(_fanAngles[slot] - middle, -(float)Math.PI, (float)Math.PI) * spread;
			}
		}

		private FanBlade MakeFanBlade(float rot)
		{
			CardModel card = C.Card;
			bool flag = ((card != null) ? card.Enchantment : null) is Inky;
			Node2D val = ((_borrowedFor != null) ? PhantomBladesFx.MakeSpectral(flag) : ShivFx.MakeShiv(flag));
			if (flag && _borrowedFor == null)
			{
				((Node)val).AddChild((Node)(object)InkDrip(), false, (InternalMode)0);
			}
			((CanvasItem)val).Visible = false;
			((Node)Front).AddChild((Node)(object)val, false, (InternalMode)0);
			return new FanBlade
			{
				Node = val,
				Rot = rot
			};
		}

		private void PlayFan()
		{
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			Blade blade = ((_blades.Length != 0) ? _blades[0] : null);
			if (_lunged || blade == null || blade.Taken || !FxKit.Alive((GodotObject)(object)blade.Node) || C.Card == null)
			{
				Leave(16f);
				return;
			}
			_lunged = true;
			if (C.Remote && _remoteLoan != null && !PhantomBladesFx.CommitRemoteThrow(_remoteLoan))
			{
				Leave(16f);
				return;
			}
			blade.Taken = true;
			int num = ((_fanTargets.Count > 0) ? ((_fanTargets.Count - 1) / 2) : (-1));
			List<(Node2D, object)> list = new List<(Node2D, object)> { (blade.Node, (num >= 0) ? _fanTargets[num].Key : null) };
			int num2 = 0;
			for (int i = 0; i < _extra.Count; i++)
			{
				FanBlade fanBlade = _extra[i];
				if (!fanBlade.Going && FxKit.Alive((GodotObject)(object)fanBlade.Node))
				{
					if (num2 == num)
					{
						num2++;
					}
					if (num2 < _fanTargets.Count && ((CanvasItem)fanBlade.Node).Visible && ((CanvasItem)fanBlade.Node).Modulate.A > 0.3f)
					{
						list.Add((fanBlade.Node, _fanTargets[num2].Key));
						_extra.RemoveAt(i);
						i--;
					}
					num2++;
				}
			}
			ShivFan.Launch(C.Card, list, blade.Pos, _borrowedFor != null);
			Leave(16f);
		}

		private static float SymphonyTurn(out float wind, out float air, out float settle)
		{
			wind = Mathf.Max(0.01f, Tuning.F("symphony.windup", 0.12f));
			air = Mathf.Max(0.05f, Tuning.F("symphony.hop_time", 0.56f));
			settle = Mathf.Max(0.01f, Tuning.F("symphony.settle", 0.26f));
			return wind + air + settle + Mathf.Max(0f, Tuning.F("symphony.pause", 0.05f));
		}

		private static float HopHeight(float t, float wind, float air, float settle, float height, out float squash, out float lift)
		{
			squash = 0f;
			lift = 0f;
			if (t < 0f)
			{
				return 0f;
			}
			float num = 4f * height / air;
			if (t < wind)
			{
				float num2 = t / wind;
				float num3 = num * wind * (num2 * num2 - num2 * num2 * num2);
				squash = Mathf.Clamp(num3 / Mathf.Max(0.5f, num * wind * 0.148f), 0f, 1f);
				return 0f - num3;
			}
			t -= wind;
			if (t < air)
			{
				float num4 = t / air;
				lift = 4f * num4 * (1f - num4);
				return height * lift;
			}
			t -= air;
			if (t < settle)
			{
				float num5 = t / settle;
				float num6 = num * 0.75f * settle;
				float num7 = num6 * num5 * (1f - num5) * (1f - num5);
				squash = Mathf.Clamp(num7 / Mathf.Max(0.5f, num6 * 0.148f), 0f, 1f);
				return 0f - num7;
			}
			return 0f;
		}

		public bool HandOverCloak(out float open, out float hem, out float alpha)
		{
			open = 0f;
			hem = 0.55f;
			alpha = 0f;
			if (_style != ShivStyle.Cloak || Finished || !FxKit.Alive((GodotObject)(object)_cloak) || !((CanvasItem)_cloak).Visible)
			{
				return false;
			}
			open = _cloakOpen;
			hem = _cloakHem;
			alpha = Mathf.Clamp(Fade, 0f, 1f);
			((CanvasItem)_cloak).Visible = false;
			return true;
		}

		public bool TryTake(out Node2D blade)
		{
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			blade = null;
			if (_style == ShivStyle.Return || Finished)
			{
				return false;
			}
			Blade[] blades = _blades;
			foreach (Blade blade2 in blades)
			{
				if (!blade2.Taken && FxKit.Alive((GodotObject)(object)blade2.Node))
				{
					blade2.Taken = true;
					Node2D node = blade2.Node;
					Vector2 globalPosition = node.GlobalPosition;
					float globalRotation = node.GlobalRotation;
					Node parent = ((Node)node).GetParent();
					if (parent != null)
					{
						parent.RemoveChild((Node)(object)node);
					}
					Node frontParent = C.FrontParent;
					if (!FxKit.Alive((GodotObject)(object)frontParent))
					{
						((Node)node).QueueFree();
						return false;
					}
					frontParent.AddChild((Node)(object)node, false, (InternalMode)0);
					FxKit.FreeAfter((Node)(object)node, 3.0);
					node.GlobalPosition = globalPosition;
					node.GlobalRotation = globalRotation;
					node.Scale = Vector2.One;
					((CanvasItem)node).Modulate = Colors.White;
					((CanvasItem)node).Visible = true;
					blade = node;
					return true;
				}
			}
			return false;
		}

		protected override void OnPlay(CardFxContext play)
		{
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			_playingFor = 0f;
			if (_fan)
			{
				PlayFan();
			}
			else
			{
				if (_style != ShivStyle.Return)
				{
					return;
				}
				if (_lunged || _blades.Length == 0 || !play.HasTarget)
				{
					Leave(16f);
					return;
				}
				_lunged = true;
				Blade blade = _blades[0];
				if (C.Remote && _remoteLoan != null && !PhantomBladesFx.CommitRemoteThrow(_remoteLoan))
				{
					Leave(16f);
					return;
				}
				blade.Taken = true;
				Node2D node = blade.Node;
				if (!FxKit.Alive((GodotObject)(object)node) || !FxKit.Alive((GodotObject)(object)play.FrontParent))
				{
					Leave(16f);
					return;
				}
				Vector2 from = node.GlobalPosition;
				float globalRotation = node.GlobalRotation;
				Node parent = ((Node)node).GetParent();
				if (parent != null)
				{
					parent.RemoveChild((Node)(object)node);
				}
				play.FrontParent.AddChild((Node)(object)node, false, (InternalMode)0);
				FxKit.FreeAfter((Node)(object)node, 0.20000000298023224);
				node.GlobalPosition = from;
				node.GlobalRotation = globalRotation;
				((CanvasItem)node).Modulate = Colors.White;
				Vector2 to = play.TargetChest;
				Line2D trail = FxKit.Ribbon(10f, new Color(Steel.R, Steel.G, Steel.B, 0.75f));
				play.FrontParent.AddChild((Node)(object)trail, false, (InternalMode)0);
				FxKit.FreeAfter((Node)(object)trail, 0.3799999952316284);
				FxKit.Drive((Node)(object)node, 0.1f, (float t) =>
				{
					//IL_0015: Unknown result type (might be due to invalid IL or missing references)
					//IL_001b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0020: Unknown result type (might be due to invalid IL or missing references)
					//IL_0027: Unknown result type (might be due to invalid IL or missing references)
					//IL_0046: Unknown result type (might be due to invalid IL or missing references)
					//IL_0047: Unknown result type (might be due to invalid IL or missing references)
					if (FxKit.Alive((GodotObject)(object)node))
					{
						Vector2 val2 = from.Lerp(to, t);
						node.GlobalPosition = val2;
						if (FxKit.Alive((GodotObject)(object)trail))
						{
							trail.AddPoint(((Node2D)trail).ToLocal(val2), -1);
						}
					}
				}, (TransitionType)4, (EaseType)0).TweenCallback(Callable.From((Action)(() =>
				{
					if (FxKit.Alive((GodotObject)(object)node))
					{
						((Node)node).QueueFree();
					}
				})));
				if (FxKit.Alive((GodotObject)(object)trail))
				{
					Tween val = ((Node)trail).CreateTween();
					val.TweenInterval(0.1);
					val.TweenProperty((GodotObject)(object)trail, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.18);
					val.TweenCallback(Callable.From((Action)(() =>
					{
						if (FxKit.Alive((GodotObject)(object)trail))
						{
							((Node)trail).QueueFree();
						}
					})));
				}
				Leave(16f);
			}
		}

		protected override void OnLeaving()
		{
			ReturnBorrowed();
		}

		protected override void OnEnd()
		{
			ReturnBorrowed();
		}

		private void ReturnBorrowed()
		{
			CardModel borrowedFor = _borrowedFor;
			_borrowedFor = null;
			if (borrowedFor == null || _blades.Length == 0)
			{
				return;
			}
			Blade blade = _blades[0];
			if (!blade.Taken && FxKit.Alive((GodotObject)(object)blade.Node))
			{
				blade.Taken = true;
				if (C.Remote)
				{
					PhantomBladesFx.GiveBackRemote(_remoteLoan);
				}
				else
				{
					PhantomBladesFx.GiveBack(borrowedFor, blade.Node);
				}
			}
		}

		static ShivHeld()
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		}
	}
