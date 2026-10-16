// namespace: SilentFX.Powers
// type: ConcoctFx (class)
// source lines: 4469-4644
	public sealed class ConcoctFx : BasicPowerFx
	{
		private Node2D _flask;

		private Sprite2D _glow;

		private GpuParticles2D _fizz;

		protected override string NodeName => "Concoct";

		protected override bool UseBack => false;

		protected override void Build(bool animateIn)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected Obj, but got Unknown
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_038a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			_flask = new Node2D();
			((Node)Front).AddChild((Node)(object)_flask, false, (InternalMode)0);
			_glow = FxKit.Glow(new Color(PoisonFx.Venom.R, PoisonFx.Venom.G, PoisonFx.Venom.B, 0.35f), new Vector2(96f, 96f));
			((Node2D)_glow).Position = new Vector2(0f, 4f);
			((Node)_flask).AddChild((Node)(object)_glow, false, (InternalMode)0);
			Vector2[] array = new Vector2[14];
			for (int i = 0; i < array.Length; i++)
			{
				float num = (float)Math.PI * 2f * (float)i / (float)array.Length;
				array[i] = new Vector2(Mathf.Cos(num) * 17f, Mathf.Sin(num) * 17f + 4f);
			}
			Vector2[] array2 = new Vector2[9];
			for (int j = 0; j < array2.Length; j++)
			{
				float num2 = (float)Math.PI * (-0.08f + 1.16f * (float)j / (float)(array2.Length - 1));
				array2[j] = new Vector2(Mathf.Cos(num2) * 14.5f, Mathf.Sin(num2) * 14.5f + 4f);
			}
			((Node)_flask).AddChild((Node)(object)FxKit.Shape(array, new Color(0.8f, 1f, 0.86f, 0.26f)), false, (InternalMode)0);
			((Node)_flask).AddChild((Node)(object)FxKit.Shape(array2, new Color(PoisonFx.Venom.R * 0.75f, PoisonFx.Venom.G * 0.92f, PoisonFx.Venom.B * 0.75f, 0.95f)), false, (InternalMode)0);
			((Node)_flask).AddChild((Node)(object)FxKit.Shape(new Vector2[4]
			{
				new Vector2(-5.5f, -26f),
				new Vector2(5.5f, -26f),
				new Vector2(6.5f, -11f),
				new Vector2(-6.5f, -11f)
			}, new Color(0.8f, 1f, 0.86f, 0.3f)), false, (InternalMode)0);
			((Node)_flask).AddChild((Node)(object)FxKit.Shape(new Vector2[4]
			{
				new Vector2(-8f, -29f),
				new Vector2(8f, -29f),
				new Vector2(8f, -25f),
				new Vector2(-8f, -25f)
			}, new Color(0.86f, 1f, 0.9f, 0.75f)), false, (InternalMode)0);
			((Node)_flask).AddChild((Node)(object)FxKit.Shape(new Vector2[4]
			{
				new Vector2(-11f, -4f),
				new Vector2(-7f, -8f),
				new Vector2(-6f, 2f),
				new Vector2(-10f, 6f)
			}, new Color(1f, 1f, 1f, 0.55f)), false, (InternalMode)0);
			_fizz = FxKit.Bubbles((Node)(object)_flask, Vector2.Zero, new Color(PoisonFx.Venom.R, PoisonFx.Venom.G, PoisonFx.Venom.B, 0.9f), Tuning.I("concoct.fizz", 7), new Vector2(3f, 2f), 26f, 58f, 0.9, 0.07f, 0.15f, oneShot: false);
			if (FxKit.Alive((GodotObject)(object)_fizz))
			{
				((Node2D)_fizz).Position = new Vector2(0f, -30f);
			}
		}

		protected override void OnTrigger(Vector2? at)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			Node frontParent = Anchor.FrontParent;
			if (at.HasValue && FxKit.Alive((GodotObject)(object)frontParent))
			{
				Color venom = PoisonFx.Venom;
				FxKit.Burst(frontParent, at.Value, new Color(venom.R, venom.G, venom.B, 0.95f), Tuning.I("concoct.splash", 18), 110f, 310f, 0.75, 0.14f, 0.34f, new Vector2(0f, 520f), 150f, new Vector2(0f, -1f), 14f);
				FxKit.Bubbles(frontParent, at.Value, new Color(venom.R, venom.G, venom.B, 0.9f), 8, new Vector2(30f, 20f), 80f, 200f, 0.7, 0.12f, 0.28f);
				FxKit.Flash(frontParent, at.Value, new Color(venom.R, venom.G, venom.B, 0.7f), 80f, 3.2f, 0.35);
			}
		}

		protected override void Layout()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)_flask))
			{
				Vector2 val = Anchor.SafeBodySize();
				Vector2 val2 = Anchor.Chest() + new Vector2(val.X * Tuning.F("concoct.x", -0.42f) * Anchor.Side(), val.Y * Tuning.F("concoct.y", -0.3f));
				_flask.GlobalPosition = val2 + new Vector2(0f, Mathf.Sin(Time * 2.1f) * 6f - 26f * Kick);
				_flask.Rotation = (Mathf.Sin(Time * 1.5f) * 0.14f + 0.9f * Kick) * Anchor.Side();
				float num = Tuning.F("concoct.scale", 1.25f) * (1f + 0.35f * Kick);
				_flask.Scale = new Vector2(num * Anchor.Side(), num);
				((CanvasItem)_flask).Modulate = new Color(1f, 1f, 1f, A);
				if (FxKit.Alive((GodotObject)(object)_glow))
				{
					float num2 = 0.3f + 0.1f * Mathf.Sin(Time * 3f) + 0.5f * Kick;
					((CanvasItem)_glow).Modulate = new Color(PoisonFx.Venom.R, PoisonFx.Venom.G, PoisonFx.Venom.B, num2);
				}
			}
		}

		protected override void OnStopping()
		{
			if (FxKit.Alive((GodotObject)(object)_fizz))
			{
				_fizz.Emitting = false;
			}
		}
	}
