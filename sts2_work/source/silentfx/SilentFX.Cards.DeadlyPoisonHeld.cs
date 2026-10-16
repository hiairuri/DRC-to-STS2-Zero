// namespace: SilentFX.Cards
// type: DeadlyPoisonHeld (class)
// source lines: 83778-83919
	public sealed class DeadlyPoisonHeld : HeldSession, IHeldOrigin
	{
		private static readonly StringName PAlpha = StringName.op_Implicit("alpha");

		private static readonly StringName PSpin = StringName.op_Implicit("spin");

		private Node2D _orb;

		private Sprite2D _glow;

		private Sprite2D _film;

		private Node2D _mark;

		private ShaderMaterial _markMat;

		private Vector2 _markAt;

		public Vector2 Origin
		{
			get
			{
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				if (!FxKit.Alive((GodotObject)(object)_orb))
				{
					return C.WeaponHand;
				}
				return _orb.GlobalPosition;
			}
		}

		protected override void Build()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected Obj, but got Unknown
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Expected Obj, but got Unknown
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			Color venom = PoisonFx.Venom;
			_orb = new Node2D();
			((Node)Front).AddChild((Node)(object)_orb, false, (InternalMode)0);
			_glow = FxKit.Glow(new Color(venom.R, venom.G, venom.B, 0.9f), new Vector2(70f, 70f));
			((Node)_orb).AddChild((Node)(object)_glow, false, (InternalMode)0);
			((Node)_orb).AddChild((Node)(object)FxKit.Glow(new Color(0.9f, 1f, 0.8f, 0.9f), new Vector2(26f, 26f)), false, (InternalMode)0);
			_film = new Sprite2D
			{
				Texture = FxKit.Bubble,
				Material = (Material)(object)FxKit.Additive,
				Modulate = new Color(venom.R, venom.G, venom.B, 0.9f),
				Scale = new Vector2(44f, 44f) / 64f
			};
			((Node)_orb).AddChild((Node)(object)_film, false, (InternalMode)0);
			GpuParticles2D val = HeldParts.Drips(new Color(venom.R, venom.G, venom.B, 0.9f), 6, 8f, 4f);
			((Node2D)val).Position = new Vector2(0f, 14f);
			((Node)_orb).AddChild((Node)(object)val, false, (InternalMode)0);
			GpuParticles2D val2 = FxKit.Bubbles((Node)(object)_orb, Vector2.Zero, new Color(venom.R, venom.G, venom.B, 0.8f), 5, new Vector2(10f, 4f), 20f, 60f, 0.7, 0.05f, 0.12f, oneShot: false);
			if (FxKit.Alive((GodotObject)(object)val2))
			{
				((Node2D)val2).Position = Vector2.Zero;
			}
			if (!C.Remote)
			{
				_mark = HeldParts.AimRing((Node)(object)Front, 150f, venom, out _markMat);
			}
			_markAt = C.Chest;
		}

		protected override void Tick(float dt)
		{
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)_orb))
			{
				return;
			}
			float num = 0.85f + 0.1f * Mathf.Sin(Time * 5f) + 0.4f * Arm;
			_orb.GlobalPosition = C.WeaponHand + new Vector2(6f * C.Facing, -18f - 10f * Arm + Mathf.Sin(Time * 2.6f) * 3f);
			_orb.Scale = new Vector2(num, num) * Mathf.Min(1f, Time / 0.18f);
			if (FxKit.Alive((GodotObject)(object)_film))
			{
				((Node2D)_film).Rotation = Time * 1.3f;
			}
			if (C.HasTarget)
			{
				_markAt = C.TargetChest;
			}
			if (FxKit.Alive((GodotObject)(object)_mark))
			{
				_mark.GlobalPosition = _markAt;
				float num2 = Mathf.Lerp(1.7f, 1f, Arm);
				_mark.Scale = new Vector2(num2, num2);
				if (FxKit.Alive((GodotObject)(object)_markMat))
				{
					_markMat.SetShaderParameter(PAlpha, Variant.op_Implicit(0.75f * Arm));
					_markMat.SetShaderParameter(PSpin, Variant.op_Implicit(Time * 1.6f));
				}
			}
		}
	}
