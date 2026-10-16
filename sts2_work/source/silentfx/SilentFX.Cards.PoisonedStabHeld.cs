// namespace: SilentFX.Cards
// type: PoisonedStabHeld (class)
// source lines: 84090-84217
	public sealed class PoisonedStabHeld : HeldSession, IHeldOrigin
	{
		private Node2D _spike;

		private Sprite2D _halo;

		private float _length;

		public Vector2 Origin
		{
			get
			{
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				if (!FxKit.Alive((GodotObject)(object)_spike))
				{
					return C.WeaponHand;
				}
				return _spike.GlobalPosition;
			}
		}

		protected override void Build()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Expected Obj, but got Unknown
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			Color venom = PoisonFx.Venom;
			_length = Tuning.F("stab.spike_length", 124f);
			float num = Tuning.F("stab.spike_halfwidth", 15f);
			_spike = new Node2D();
			((Node)Front).AddChild((Node)(object)_spike, false, (InternalMode)0);
			_halo = FxKit.Glow(new Color(venom.R, venom.G, venom.B, 0.4f), new Vector2(_length * 1.45f, num * 5.5f));
			((Node2D)_halo).Position = new Vector2(_length * 0.42f, 0f);
			((Node)_spike).AddChild((Node)(object)_halo, false, (InternalMode)0);
			Polygon2D val = FxKit.Shape(new Vector2[3]
			{
				new Vector2(0f, 0f - num),
				new Vector2(_length, 0f),
				new Vector2(0f, num)
			}, new Color(venom.R, venom.G, venom.B, 0.85f));
			((CanvasItem)val).Material = (Material)(object)FxKit.Additive;
			((Node)_spike).AddChild((Node)(object)val, false, (InternalMode)0);
			Polygon2D val2 = FxKit.Shape(new Vector2[3]
			{
				new Vector2(_length * 0.05f, (0f - num) * 0.42f),
				new Vector2(_length * 0.84f, 0f),
				new Vector2(_length * 0.05f, num * 0.42f)
			}, new Color(0.92f, 1f, 0.84f, 0.95f));
			((CanvasItem)val2).Material = (Material)(object)FxKit.Additive;
			((Node)_spike).AddChild((Node)(object)val2, false, (InternalMode)0);
		}

		protected override void Tick(float dt)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)_spike))
			{
				return;
			}
			Vector2 val = C.WeaponHand + new Vector2(6f * C.Facing, -10f);
			Vector2 val2 = new Vector2(C.Facing, -0.12f);
			Vector2 val3 = val2.Normalized();
			if (C.HasTarget)
			{
				Vector2 val4 = C.TargetChest - val;
				if (val4.LengthSquared() > 1f)
				{
					val2 = val3.Lerp(val4.Normalized(), Arm);
					val3 = val2.Normalized();
				}
			}
			float num = Mathf.Min(1f, Time / 0.16f);
			float num2 = 0.8f + 0.2f * Arm + 0.03f * Mathf.Sin(Time * 6f);
			_spike.GlobalPosition = val + new Vector2(0f, Mathf.Sin(Time * 2.6f) * 2f);
			_spike.Rotation = val3.Angle();
			_spike.Scale = new Vector2(num * num2, num * (1f - 0.12f * Arm));
			if (FxKit.Alive((GodotObject)(object)_halo))
			{
				((CanvasItem)_halo).Modulate = new Color(PoisonFx.Venom.R, PoisonFx.Venom.G, PoisonFx.Venom.B, 0.3f + 0.25f * Arm);
			}
		}
	}
