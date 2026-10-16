// namespace: SilentFX.Cards
// type: MirageHeld (class)
// source lines: 84286-84444
	public sealed class MirageHeld : HeldSession
	{
		private static readonly StringName PFill = StringName.op_Implicit("fill");

		private static readonly StringName PChurn = StringName.op_Implicit("churn");

		private static readonly StringName PIntensity = StringName.op_Implicit("intensity");

		private readonly List<GpuParticles2D> _wisps = new List<GpuParticles2D>();

		private Node2D _mist;

		private ColorRect _mistQuad;

		private ShaderMaterial _mistMat;

		private float _power;

		private float _strength = 1f;

		public float Filled { get; private set; }

		protected override void Build()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Expected Obj, but got Unknown
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Expected Obj, but got Unknown
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			Color haze = MirageFx.Haze;
			_power = MirageFx.PowerOf(C);
			_strength = ((MirageFx.PoisonOn(C) > 0) ? 1f : 0.5f);
			MirageFx.Ellipse(C, _power, out var centre, out var size);
			foreach (FxTarget enemy in C.Enemies)
			{
				if (enemy.Poison > 0 && _wisps.Count < 6)
				{
					Vector2 val = centre - enemy.Chest;
					float num = Mathf.Max(1f, val.Length());
					Vector2 val2 = val / num + new Vector2(0f, -0.12f);
					Vector2 val3 = val2.Normalized();
					float num2 = Tuning.F("mirage.mist_speed", 620f);
					ParticleProcessMaterial processMaterial = new ParticleProcessMaterial
					{
						EmissionShape = (EmissionShapeEnum)3,
						EmissionBoxExtents = new Vector3(enemy.Width * 0.22f, 50f, 0f),
						Direction = new Vector3(val3.X, val3.Y, 0f),
						Spread = 7f,
						InitialVelocityMin = num2 * 0.75f,
						InitialVelocityMax = num2 * 1.15f,
						Gravity = new Vector3(val.X / num * 260f, 30f, 0f),
						ScaleMin = 1.1f,
						ScaleMax = 2.6f,
						Color = new Color(haze.R, haze.G, haze.B, Tuning.F("mirage.mist_alpha", 0.13f)),
						ColorRamp = (Texture2D)(object)FxKit.FadeInOut(0.25f),
						ParticleFlagDisableZ = true
					};
					GpuParticles2D val4 = new GpuParticles2D
					{
						Amount = 14,
						Lifetime = Mathf.Clamp(num / num2, 0.45f, 1.4f),
						Randomness = 0.5f,
						Texture = FxKit.SoftDot,
						ProcessMaterial = (Material)(object)processMaterial,
						Material = (Material)(object)FxKit.Additive,
						VisibilityRect = new Rect2(-2000f, -1200f, 4000f, 2400f)
					};
					((Node)Front).AddChild((Node)(object)val4, false, (InternalMode)0);
					((Node2D)val4).GlobalPosition = enemy.Chest;
					_wisps.Add(val4);
				}
			}
			_mist = MirageFx.MakeMist((Node)(object)Front, centre, size, out _mistQuad, out _mistMat);
		}

		protected override void Tick(float dt)
		{
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			foreach (GpuParticles2D wisp in _wisps)
			{
				if (FxKit.Alive((GodotObject)(object)wisp))
				{
					wisp.SpeedScale = 0.7f + 0.9f * Arm;
				}
			}
			if (!Leaving)
			{
				Filled = (0.26f + 0.24f * Arm) * Mathf.Min(1f, Time / 0.25f);
			}
			if (FxKit.Alive((GodotObject)(object)_mist) && FxKit.Alive((GodotObject)(object)_mistMat))
			{
				MirageFx.Ellipse(C, _power, out var centre, out var size);
				MirageFx.PlaceMist(_mist, _mistQuad, _mistMat, centre, size);
				_mistMat.SetShaderParameter(PFill, Variant.op_Implicit(Filled));
				_mistMat.SetShaderParameter(PChurn, Variant.op_Implicit(Time * 0.8f));
				_mistMat.SetShaderParameter(PIntensity, Variant.op_Implicit(Mathf.Clamp((0.3f + 0.26f * Arm) * _strength * Fade, 0f, 1f)));
			}
		}

		protected override void OnLeaving()
		{
			foreach (GpuParticles2D wisp in _wisps)
			{
				if (FxKit.Alive((GodotObject)(object)wisp))
				{
					wisp.Emitting = false;
				}
			}
		}
	}
