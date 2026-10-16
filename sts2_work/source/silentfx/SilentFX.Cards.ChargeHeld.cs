// namespace: SilentFX.Cards
// type: ChargeHeld (class)
// source lines: 83920-84085
	public abstract class ChargeHeld : HeldSession, IHeldOrigin
	{
		private Node2D _orb;

		private Sprite2D _halo;

		private Sprite2D _core;

		private Line2D _aim;

		protected virtual float Size => 78f;

		protected virtual float Stretch => 1f;

		protected virtual bool Drips => false;

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
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected Obj, but got Unknown
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Expected Obj, but got Unknown
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Expected Obj, but got Unknown
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			Color venom = PoisonFx.Venom;
			Gradient val = new Gradient();
			val.Offsets = new float[3] { 0f, 0.25f, 1f };
			val.Colors = new Color[3]
			{
				new Color(venom.R, venom.G, venom.B, 0f),
				new Color(venom.R, venom.G, venom.B, 0.55f),
				new Color(venom.R, venom.G, venom.B, 0.1f)
			};
			if (!C.Remote)
			{
				_aim = new Line2D
				{
					Width = 5f,
					Gradient = val,
					Material = (Material)(object)FxKit.Additive,
					Antialiased = true
				};
				((Node)Front).AddChild((Node)(object)_aim, false, (InternalMode)0);
			}
			_orb = new Node2D();
			((Node)Front).AddChild((Node)(object)_orb, false, (InternalMode)0);
			_halo = FxKit.Glow(new Color(venom.R, venom.G, venom.B, 0.85f), new Vector2(Size, Size));
			((Node)_orb).AddChild((Node)(object)_halo, false, (InternalMode)0);
			_core = FxKit.Glow(new Color(0.9f, 1f, 0.82f, 0.95f), new Vector2(Size * 0.36f, Size * 0.36f));
			((Node)_orb).AddChild((Node)(object)_core, false, (InternalMode)0);
			if (Drips)
			{
				GpuParticles2D val2 = HeldParts.Drips(new Color(venom.R, venom.G, venom.B, 0.95f), 7, 8f, 4f);
				((Node2D)val2).Position = new Vector2(0f, 10f);
				((Node)_orb).AddChild((Node)(object)val2, false, (InternalMode)0);
			}
		}

		protected override void Tick(float dt)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)_orb))
			{
				return;
			}
			Vector2 val = C.WeaponHand + new Vector2(6f * C.Facing, -14f);
			Vector2 val2 = new Vector2(C.Facing, -0.1f);
			if (C.HasTarget)
			{
				Vector2 val3 = C.TargetChest - val;
				if (val3.LengthSquared() > 1f)
				{
					Vector2 val4 = val2.Lerp(val3.Normalized(), Arm);
					val2 = val4.Normalized();
				}
			}
			float num = (0.8f + 0.1f * Mathf.Sin(Time * 5.5f) + 0.45f * Arm) * Mathf.Min(1f, Time / 0.18f);
			_orb.GlobalPosition = val + new Vector2(0f, Mathf.Sin(Time * 2.6f) * 3f);
			_orb.Rotation = val2.Angle();
			_orb.Scale = new Vector2(num * Stretch, num / Mathf.Sqrt(Stretch));
			if (FxKit.Alive((GodotObject)(object)_aim))
			{
				((CanvasItem)_aim).Visible = C.HasTarget;
				if (C.HasTarget)
				{
					_aim.Points = new Vector2[2]
					{
						Front.ToLocal(_orb.GlobalPosition),
						Front.ToLocal(C.TargetChest)
					};
					((CanvasItem)_aim).Modulate = new Color(1f, 1f, 1f, 0.55f * Arm * (0.75f + 0.25f * Mathf.Sin(Time * 9f)));
				}
			}
		}
	}
