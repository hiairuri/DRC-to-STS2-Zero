// namespace: SilentFX.Powers
// type: ShadowPresence (class)
// source lines: 12572-12947
	internal sealed class ShadowPresence
	{
		private static readonly StringName PIntensity = StringName.op_Implicit("intensity");

		private static readonly StringName PPulse = StringName.op_Implicit("pulse");

		private static readonly StringName PLean = StringName.op_Implicit("lean");

		private static readonly Color Cold = new Color(0.36f, 0.32f, 1f, 1f);

		private readonly bool _full;

		private Sprite2D _ground;

		private Sprite2D _rim;

		private Node2D _pool;

		private ShaderMaterial _poolMat;

		private ShadeMist _mass;

		private ShadeMist _left;

		private ShadeMist _right;

		private ShadeMist _lap;

		private GpuParticles2D _motes;

		private GpuParticles2D _glints;

		public ShadowPresence(bool full)
		{
			_full = full;
		}

		private static ShadeMist Column(float seed, float foot, float bulge, float taper, float wisp, float glow, float speed, float hollow = 0f)
		{
			ShadeMist shadeMist = new ShadeMist(0, seed);
			shadeMist.Set("foot", foot);
			shadeMist.Set("bulge", bulge);
			shadeMist.Set("taper", taper);
			shadeMist.Set("wisp", wisp);
			shadeMist.Set("glow", glow);
			shadeMist.Set("speed", speed);
			if (hollow > 0f)
			{
				shadeMist.Set("hollow", hollow);
			}
			return shadeMist;
		}

		private static GpuParticles2D Inward(Color color, int amount, float radius, float scaleMin, float scaleMax, bool additive)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Expected Obj, but got Unknown
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Expected Obj, but got Unknown
			ParticleProcessMaterial processMaterial = new ParticleProcessMaterial
			{
				EmissionShape = (EmissionShapeEnum)6,
				EmissionRingAxis = new Vector3(0f, 0f, 1f),
				EmissionRingHeight = 0f,
				EmissionRingRadius = radius,
				EmissionRingInnerRadius = radius * 0.72f,
				Direction = new Vector3(0f, -1f, 0f),
				Spread = 180f,
				InitialVelocityMin = 0f,
				InitialVelocityMax = 12f,
				RadialVelocityMin = (0f - radius) * 0.95f / 1.1f,
				RadialVelocityMax = (0f - radius) * 0.7f / 1.1f,
				TangentialAccelMin = 20f,
				TangentialAccelMax = 70f,
				Gravity = new Vector3(0f, 0f, 0f),
				ScaleMin = scaleMin,
				ScaleMax = scaleMax,
				Color = color,
				ColorRamp = (Texture2D)(object)FxKit.FadeInOut(0.25f),
				ParticleFlagDisableZ = true
			};
			GpuParticles2D val = new GpuParticles2D
			{
				Amount = Math.Max(1, amount),
				Lifetime = 1.100000023841858,
				OneShot = false,
				Explosiveness = 0f,
				Randomness = 0.6f,
				Texture = FxKit.SoftDot,
				ProcessMaterial = (Material)(object)processMaterial,
				LocalCoords = true,
				VisibilityRect = new Rect2(-900f, -900f, 1800f, 1800f),
				Modulate = new Color(1f, 1f, 1f, 0f)
			};
			if (additive)
			{
				((CanvasItem)val).Material = (Material)(object)FxKit.Additive;
			}
			return val;
		}

		public void Build(Node2D back, Node2D front, Vector2 body)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Expected Obj, but got Unknown
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)back))
			{
				_ground = Shade.Blob(new Vector2(64f, 64f), 0f);
				((Node)back).AddChild((Node)(object)_ground, false, (InternalMode)0);
				_rim = FxKit.Glow(new Color(Cold.R, Cold.G, Cold.B, 0f), new Vector2(64f, 64f));
				((Node)back).AddChild((Node)(object)_rim, false, (InternalMode)0);
				_pool = new Node2D
				{
					Name = StringName.op_Implicit("SilentFX_ShadowPuddle")
				};
				((Node)back).AddChild((Node)(object)_pool, false, (InternalMode)0);
				_poolMat = ShaderLib.NewMaterial("shadow_pool");
				if (_poolMat != null)
				{
					_poolMat.SetShaderParameter(PIntensity, Variant.op_Implicit(0f));
					_poolMat.SetShaderParameter(StringName.op_Implicit("seed"), Variant.op_Implicit(3.7f));
					_poolMat.SetShaderParameter(StringName.op_Implicit("calm"), Variant.op_Implicit(Mathf.Clamp(Tuning.F(_full ? "shadow.twin_calm" : "shadow.pool_calm", _full ? 0.35f : 0.6f), 0f, 1f)));
					ColorRect val = FxKit.Quad(new Vector2(100f, 100f), (Material)(object)_poolMat);
					((Control)val).Position = new Vector2(-50f, -50f);
					((Node)_pool).AddChild((Node)(object)val, false, (InternalMode)0);
				}
				_mass = Column(7.2f, 0.95f, 0.35f, 1.5f, 1.5f, 0.5f, _full ? 1.3f : 1f);
				((Node)back).AddChild((Node)(object)_mass.Root, false, (InternalMode)0);
				if (_full)
				{
					_left = Column(1.9f, 0.85f, 0.22f, 0.95f, 1.6f, 0.6f, 1.8f);
					_right = Column(3.3f, 0.85f, 0.22f, 0.95f, 1.6f, 0.6f, 1.5f);
					((Node)back).AddChild((Node)(object)_left.Root, false, (InternalMode)0);
					((Node)back).AddChild((Node)(object)_right.Root, false, (InternalMode)0);
				}
			}
			if (FxKit.Alive((GodotObject)(object)front))
			{
				_lap = Column(4.4f, 0.95f, 0.2f, 1.1f, 1.5f, 0.45f, _full ? 1.2f : 0.9f, 0.7f);
				((Node)front).AddChild((Node)(object)_lap.Root, false, (InternalMode)0);
				float num = body.X * (_full ? 0.8f : 0.7f);
				_motes = Inward(Shade.Inked(0.9f), _full ? 9 : 5, num, 0.09f, 0.22f, additive: false);
				((Node2D)_motes).Scale = new Vector2(1f, 0.6f);
				((Node)front).AddChild((Node)(object)_motes, false, (InternalMode)0);
				if (_full)
				{
					_glints = Inward(new Color(Cold.R, Cold.G, Cold.B, 0.9f), 4, num * 0.9f, 0.05f, 0.11f, additive: true);
					((Node2D)_glints).Scale = new Vector2(1f, 0.6f);
					((Node)front).AddChild((Node)(object)_glints, false, (InternalMode)0);
				}
			}
		}

		private static void Place(ShadeMist mist, Vector2 at, float width, float height, float lean, float intensity, float pulse)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			if (mist != null && FxKit.Alive((GodotObject)(object)mist.Root))
			{
				mist.Root.GlobalPosition = at;
				mist.Size(width, Mathf.Max(4f, height));
				mist.Set(PLean, Mathf.Clamp(lean, -1.4f, 1.4f));
				mist.Intensity = intensity;
				mist.Pulse = pulse;
			}
		}

		public void Layout(FxAnchor anchor, float time, float alpha, float flood, float rise, float surge, float lunge, float aim, float kick)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0519: Unknown result type (might be due to invalid IL or missing references)
			//IL_0530: Unknown result type (might be due to invalid IL or missing references)
			//IL_0536: Unknown result type (might be due to invalid IL or missing references)
			//IL_0542: Unknown result type (might be due to invalid IL or missing references)
			//IL_0547: Unknown result type (might be due to invalid IL or missing references)
			//IL_055c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0591: Unknown result type (might be due to invalid IL or missing references)
			//IL_0597: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0622: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = anchor.SafeBodySize();
			float num = anchor.Side();
			Vector2 val2 = anchor.Feet() + new Vector2(0f, 2f - val.Y * Tuning.F("shadow.ground", 0.07f));
			float num2 = Mathf.Clamp(Tuning.F("shadow.foot", 1.15f), 0.4f, 3f);
			float num3 = Mathf.Sin(time * 0.9f) + 0.25f * Mathf.Sin(time * 1.9f + 0.7f);
			float num4 = 1f + 0.035f * num3 + 0.06f * kick + 0.16f * surge + 0.06f * lunge;
			float num5 = Mathf.Clamp(flood * 3f, 0f, 1f);
			float num6 = Mathf.Clamp(alpha, 0f, 1f) * num5;
			float num7 = Mathf.Max(2f, val.X * num2 * flood * num4 * (_full ? Mathf.Clamp(Tuning.F("shadow.twin_pool", 1.9f), 0.6f, 3f) : 1f));
			float num8 = Mathf.Max(2f, Mathf.Clamp(num7 * 0.24f, 14f, 90f));
			Vector2 globalPosition = val2 + new Vector2((0f - num) * val.X * 0.05f, 0f);
			if (FxKit.Alive((GodotObject)(object)_ground))
			{
				((Node2D)_ground).GlobalPosition = globalPosition;
				((Node2D)_ground).Scale = new Vector2(num7 * 1.7f, num8 * 2.1f) / 64f;
				((CanvasItem)_ground).Modulate = Shade.Inked(0.82f * num6);
			}
			if (FxKit.Alive((GodotObject)(object)_rim))
			{
				float num9 = (_full ? 0.26f : 0.14f) + 0.04f * num3 + 0.25f * surge + 0.25f * lunge + 0.15f * kick;
				((Node2D)_rim).GlobalPosition = globalPosition;
				((Node2D)_rim).Scale = new Vector2(num7 * 1.3f, num8 * 2f) / 64f;
				((CanvasItem)_rim).Modulate = new Color(Cold.R, Cold.G, Cold.B, Mathf.Clamp(num9, 0f, 0.9f) * num6);
			}
			if (FxKit.Alive((GodotObject)(object)_pool))
			{
				_pool.GlobalPosition = globalPosition;
				_pool.Scale = new Vector2(num7 * 1.4f, num8 * 1.5f) / 100f;
				if (FxKit.Alive((GodotObject)(object)_poolMat))
				{
					_poolMat.SetShaderParameter(PIntensity, Variant.op_Implicit(num6 * (_full ? 1f : 0.9f)));
					_poolMat.SetShaderParameter(PPulse, Variant.op_Implicit(Mathf.Clamp(0.5f * kick + surge + lunge, 0f, 1f)));
				}
			}
			float num10 = Mathf.Max(0f, rise);
			float num11 = val.Y * Mathf.Clamp(_full ? Tuning.F("shadow.mist_high", 0.5f) : Tuning.F("shadow.mist_low", 0.26f), 0.05f, 3f);
			float num12 = Mathf.Max(8f, num11 * (0.1f + 0.9f * num10) * (1f + 0.05f * num3) * (1f + 0.25f * lunge + 0.45f * surge));
			float num13 = Mathf.Max(8f, val.X * Mathf.Clamp(Tuning.F("shadow.mist_wide", 1.25f), 0.4f, 4f) * (_full ? 1.12f : 1f) * (1f + 0.08f * lunge + 0.15f * surge));
			float num14 = 1.1f * Tuning.F("shadow.twin_lunge", 0.75f) * lunge * aim;
			float num15 = Mathf.Clamp(alpha, 0f, 1f) * Mathf.Clamp(Tuning.F("shadow.twin_alpha", 0.92f), 0.1f, 1f) * num5 * (_full ? 1f : 0.85f);
			float num16 = Mathf.Clamp(Mathf.Max(lunge, surge), 0f, 1f) * 0.7f;
			Place(_mass, val2, num13, num12 * (_full ? 0.7f : 1f), num14 * 0.5f, num15, num16);
			Tongue(_left, -1f, 1.15f, val2, num13, num12, num14, num15, num16, time);
			Tongue(_right, 1f, 0.85f, val2, num13, num12, num14, num15, num16, time);
			Place(_lap, val2 + new Vector2(0f, val.Y * 0.02f), num13 * 0.9f, Mathf.Min(num12 * 0.7f, val.Y * 0.2f) * Mathf.Min(1f, num10), num14 * 0.3f, num15 * 0.75f, num16);
			Vector2 globalPosition2 = val2 + new Vector2(0f, (0f - val.Y) * 0.04f);
			if (FxKit.Alive((GodotObject)(object)_motes))
			{
				((Node2D)_motes).GlobalPosition = globalPosition2;
				((CanvasItem)_motes).Modulate = new Color(1f, 1f, 1f, num15);
			}
			if (FxKit.Alive((GodotObject)(object)_glints))
			{
				((Node2D)_glints).GlobalPosition = globalPosition2;
				((CanvasItem)_glints).Modulate = new Color(1f, 1f, 1f, num15 * 0.7f);
			}
		}

		private static void Tongue(ShadeMist mist, float which, float pace, Vector2 root, float width, float height, float thrown, float veil, float flare, float time)
		{
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			float num = 0.78f + 0.2f * Mathf.Sin(time * pace + which * 1.3f) + 0.08f * Mathf.Sin(time * pace * 2.7f + which * 2.6f);
			float num2 = 0.12f * Mathf.Sin(time * 1.3f + which * 1.9f);
			Place(mist, root + new Vector2(which * width * 0.24f, 0f), width * 0.55f, height * num, which * (0.4f + num2) + thrown, veil * 0.9f, flare);
		}

		public void Erupt(FxAnchor anchor, float strength)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = anchor.SafeBodySize();
			Vector2 at = anchor.Feet() + new Vector2(0f, 2f - val.Y * Tuning.F("shadow.ground", 0.07f));
			float num = Mathf.Clamp(Tuning.F("shadow.foot", 1.15f), 0.4f, 3f);
			float num2 = val.X * num * strength;
			Node backParent = anchor.BackParent;
			if (FxKit.Alive((GodotObject)(object)backParent))
			{
				FxKit.RingPulse(backParent, at, num2 * 2.1f, Shade.Rimmed(0.85f), 0f, 0.38f, 0.12f, 1f, 0.24f, 0f, 0.045f, 0.65f);
				Shade.Puff(backParent, at, num2 * 0.9f, 1.8f, 0.42f, 0.8f, 0.9f);
			}
			Node frontParent = anchor.FrontParent;
			if (FxKit.Alive((GodotObject)(object)frontParent))
			{
				Shade.Motes(frontParent, at, 8, 320f, new Vector2(0f, -1f), 70f, val.X * 0.28f);
				if (_full)
				{
					Shade.Sparks(frontParent, at, 4, 300f, new Vector2(0f, -1f), 75f, val.X * 0.22f);
				}
			}
		}

		static ShadowPresence()
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		}
	}
