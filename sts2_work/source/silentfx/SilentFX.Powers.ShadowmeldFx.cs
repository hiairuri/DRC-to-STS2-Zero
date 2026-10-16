// namespace: SilentFX.Powers
// type: ShadowmeldFx (class)
// source lines: 12375-12571
	public sealed class ShadowmeldFx : BasicPowerFx
	{
		private static readonly StringName PHollow = StringName.op_Implicit("hollow");

		private ShadeMist _behind;

		private ShadeMist _veil;

		private Sprite2D _pool;

		private ColorRect _dusk;

		private ColorRect _duskOver;

		private float _duskIn;

		private float _fall;

		private float _rise;

		private float _sincePulse = 10f;

		protected override string NodeName => "Shadowmeld";

		protected override float KickDecay => 4f;

		protected override void Build(bool animateIn)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			_dusk = FxKit.Veil(new Color(0.02f, 0f, 0.05f, 0f));
			((Node)Back).AddChild((Node)(object)_dusk, false, (InternalMode)0);
			_duskOver = FxKit.Veil(new Color(0.02f, 0f, 0.05f, 0f));
			((Node)Front).AddChild((Node)(object)_duskOver, false, (InternalMode)0);
			_duskIn = (animateIn ? 0f : 1f);
			_fall = (animateIn ? 1f : 0f);
			_pool = Shade.Blob(new Vector2(64f, 64f), 0f);
			((Node)Back).AddChild((Node)(object)_pool, false, (InternalMode)0);
			_behind = new ShadeMist(0, 2.1f);
			_behind.Set("foot", 0.7f);
			_behind.Set("bulge", 0.42f);
			_behind.Set("taper", 1.7f);
			_behind.Set("wisp", 1.25f);
			_behind.Set("glow", 0.55f);
			_behind.Set(PHollow, 0.15f);
			((Node)Back).AddChild((Node)(object)_behind.Root, false, (InternalMode)0);
			_veil = new ShadeMist(0, 7.4f);
			_veil.Set("foot", 0.75f);
			_veil.Set("bulge", 0.4f);
			_veil.Set("taper", 1.6f);
			_veil.Set("wisp", 1.4f);
			_veil.Set("glow", 0.5f);
			_veil.Set("speed", 1.25f);
			((Node)Front).AddChild((Node)(object)_veil.Root, false, (InternalMode)0);
			_rise = (animateIn ? 0f : 1f);
		}

		protected override void OnStacked()
		{
			OnTrigger(null);
		}

		protected override void Tick(float dt)
		{
			_rise = Mathf.Min(1f, _rise + dt / Tuning.F("shadowmeld.rise", 0.3f));
			_sincePulse += dt;
			_duskIn = Mathf.Min(1f, _duskIn + dt / 0.18f);
			_fall = FxKit.Damp(_fall, 0f, 3.5f, dt);
		}

		private float Pulse()
		{
			float num = Tuning.F("shadowmeld.pulse_time", 0.42f);
			if (_sincePulse < 0.06f)
			{
				return _sincePulse / 0.06f;
			}
			float num2 = 1f - (_sincePulse - 0.06f) / num;
			if (!(num2 <= 0f))
			{
				return num2 * num2;
			}
			return 0f;
		}

		protected override void Layout()
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)Front) && FxKit.Alive((GodotObject)(object)Back))
			{
				Vector2 val = Anchor.SafeBodySize();
				Vector2 val2 = Anchor.Feet() + new Vector2(0f, 5f - val.Y * Tuning.F("shadow.ground", 0.07f));
				float num = Anchor.Side();
				float num2 = Intro.BackOut(_rise);
				float num3 = Pulse();
				float num4 = Tuning.F("shadowmeld.pulse", 1f);
				float num5 = A * Mathf.Min(1f, _rise * 2.5f);
				float num6 = 1f + 0.22f * num3 * num4;
				float num7 = Mathf.Max(8f, val.Y * 1.42f * num2 * (1f + 0.14f * num3 * num4));
				float num8 = Mathf.Max(8f, val.X * 1.9f * num6 * (0.6f + 0.4f * Mathf.Min(1f, _rise * 2f)));
				float num9 = Mathf.Clamp(Tuning.F("shadowmeld.alpha", 0.62f), 0f, 1f);
				float num10 = (Stopping ? A : Mathf.Max(A, _duskIn)) * (1f + 0.6f * _fall + 0.25f * num3 * Mathf.Clamp(num4, 0f, 2f));
				if (FxKit.Alive((GodotObject)(object)_dusk))
				{
					_dusk.Color = new Color(0.02f, 0f, 0.05f, Mathf.Clamp(Tuning.F("shadowmeld.dusk", 0.14f) * num10, 0f, 0.6f));
				}
				if (FxKit.Alive((GodotObject)(object)_duskOver))
				{
					_duskOver.Color = new Color(0.02f, 0f, 0.05f, Mathf.Clamp(Tuning.F("shadowmeld.dusk_front", 0.07f) * num10, 0f, 0.4f));
				}
				if (_behind != null && FxKit.Alive((GodotObject)(object)_behind.Root))
				{
					_behind.Root.GlobalPosition = val2 + new Vector2((0f - num) * val.X * 0.05f, 0f);
					_behind.Size(num8 * 1.08f, num7 * 1.04f);
					_behind.Intensity = 0.88f * num5;
					_behind.Pulse = num3;
				}
				if (_veil != null && FxKit.Alive((GodotObject)(object)_veil.Root))
				{
					_veil.Root.GlobalPosition = val2 + new Vector2(num * val.X * 0.05f, 3f);
					_veil.Size(num8, num7);
					_veil.Set(PHollow, Mathf.Lerp(Tuning.F("shadowmeld.hollow", 0.62f), 0.15f, num3));
					_veil.Intensity = Mathf.Clamp(num9 + 0.35f * num3, 0f, 1f) * num5;
					_veil.Pulse = num3;
				}
				if (FxKit.Alive((GodotObject)(object)_pool))
				{
					((Node2D)_pool).GlobalPosition = val2;
					((Node2D)_pool).Scale = new Vector2(val.X * 2f * num6, Mathf.Clamp(val.Y * 0.09f, 10f, 26f) * 3.6f) / 64f;
					((CanvasItem)_pool).Modulate = Shade.Inked(0.8f * num5);
				}
			}
		}

		protected override void OnTrigger(Vector2? at)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			_sincePulse = 0f;
			Node frontParent = Anchor.FrontParent;
			if (FxKit.Alive((GodotObject)(object)frontParent))
			{
				Vector2 val = Anchor.SafeBodySize();
				Vector2 val2 = Anchor.Chest();
				float num = Tuning.F("shadowmeld.pulse", 1f);
				if (!(num <= 0.01f))
				{
					Shade.Puff(frontParent, val2, val.Y * 1.25f * num, 2.1f, 0.52f, 0.95f, 1f);
					Shade.Puff(Anchor.BackParent, val2 + new Vector2(0f, val.Y * 0.1f), val.Y * 1.7f * num, 1.7f, 0.6f, 0.7f, 0.8f);
					FxKit.Flash(frontParent, val2, Shade.Glowing(0.42f), val.Y * 0.9f * num, 2f);
					Shade.Motes(frontParent, val2, 16, 420f * num, null, 180f, val.X * 0.22f, 0.5);
					Shade.Sparks(frontParent, val2, 8, 380f * num, null, 180f, val.X * 0.2f);
				}
			}
		}
	}
