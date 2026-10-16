// namespace: SilentFX.Powers
// type: SneakyFx (class)
// source lines: 13512-13729
	public sealed class SneakyFx : BasicPowerFx
	{
		private static readonly Color Smoke = new Color(0.36f, 0.22f, 0.6f, 1f);

		private static readonly Color Rim = new Color(0.56f, 0.4f, 0.92f, 1f);

		private GpuParticles2D _backSmoke;

		private GpuParticles2D _frontSmoke;

		protected override string NodeName => "Sneaky";

		protected override void Build(bool animateIn)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			Vector2 body = Anchor.SafeBodySize();
			_backSmoke = MakeSmoke(body, Tuning.I("sneaky.smoke", 16), Tuning.F("sneaky.alpha", 0.34f), 1f);
			((Node)Back).AddChild((Node)(object)_backSmoke, false, (InternalMode)0);
			_frontSmoke = MakeSmoke(body, Mathf.Max(1, Tuning.I("sneaky.smoke", 16) / 3), Tuning.F("sneaky.alpha", 0.34f) * 0.5f, 0.7f);
			((Node)Front).AddChild((Node)(object)_frontSmoke, false, (InternalMode)0);
		}

		private static GpuParticles2D MakeSmoke(Vector2 body, int amount, float alpha, float size)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Expected Obj, but got Unknown
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Expected Obj, but got Unknown
			ParticleProcessMaterial processMaterial = new ParticleProcessMaterial
			{
				EmissionShape = (EmissionShapeEnum)3,
				EmissionBoxExtents = new Vector3(body.X * 0.46f, body.Y * 0.3f, 0f),
				Direction = new Vector3(0f, -1f, 0f),
				Spread = 40f,
				InitialVelocityMin = 8f,
				InitialVelocityMax = 26f,
				Gravity = new Vector3(0f, -10f, 0f),
				ScaleMin = 1.6f * size,
				ScaleMax = 3.2f * size,
				Color = new Color(Smoke.R, Smoke.G, Smoke.B, alpha),
				ColorRamp = (Texture2D)(object)FxKit.FadeInOut(0.35f),
				ParticleFlagDisableZ = true
			};
			return new GpuParticles2D
			{
				Name = StringName.op_Implicit("Smoke"),
				Amount = Mathf.Max(1, amount),
				Lifetime = 2.6,
				Preprocess = 1.5,
				Randomness = 0.6f,
				Texture = FxKit.SoftDot,
				ProcessMaterial = (Material)(object)processMaterial,
				VisibilityRect = new Rect2(-600f, -700f, 1200f, 1100f)
			};
		}

		protected override void Layout()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = Anchor.SafeBodySize();
			Vector2 globalPosition = Anchor.Chest() + new Vector2(0f, val.Y * 0.1f);
			Color modulate = new Color(1f, 1f, 1f, A);
			if (FxKit.Alive((GodotObject)(object)_backSmoke))
			{
				((Node2D)_backSmoke).GlobalPosition = globalPosition;
				((CanvasItem)_backSmoke).Modulate = modulate;
			}
			if (FxKit.Alive((GodotObject)(object)_frontSmoke))
			{
				((Node2D)_frontSmoke).GlobalPosition = globalPosition;
				((CanvasItem)_frontSmoke).Modulate = modulate;
			}
		}

		protected override void OnTrigger(Vector2? at)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Expected Obj, but got Unknown
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Expected Obj, but got Unknown
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			Node frontParent = Anchor.FrontParent;
			if (!FxKit.Alive((GodotObject)(object)frontParent))
			{
				return;
			}
			Vector2 body = Anchor.SafeBodySize();
			Vector2 val = Anchor.Feet();
			float height = body.Y * 1.05f;
			float num = body.X * 1.35f;
			Node2D holder = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_SneakyPulse")
			};
			frontParent.AddChild((Node)(object)holder, false, (InternalMode)0);
			FxKit.FreeAfter((Node)(object)holder, Mathf.Max(0f, Tuning.F("sneaky.pulse_time", 0.55f)) + 0.1f);
			holder.GlobalPosition = val;
			Sprite2D band = new Sprite2D
			{
				Texture = FxKit.SoftDot,
				Modulate = new Color(Smoke.R, Smoke.G, Smoke.B, 0f),
				Scale = new Vector2(num, body.Y * 0.55f) / 64f
			};
			((Node)holder).AddChild((Node)(object)band, false, (InternalMode)0);
			Sprite2D edge = FxKit.Glow(new Color(Rim.R, Rim.G, Rim.B, 0f), new Vector2(num * 0.95f, 26f));
			((Node)holder).AddChild((Node)(object)edge, false, (InternalMode)0);
			FxKit.Drive((Node)(object)holder, Tuning.F("sneaky.pulse_time", 0.55f), (float t) =>
			{
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_010b: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)holder))
				{
					float num2 = (0f - height) * t;
					float num3 = Mathf.Sin((float)Math.PI * Mathf.Pow(t, 0.75f));
					if (FxKit.Alive((GodotObject)(object)band))
					{
						((Node2D)band).Position = new Vector2(0f, num2 + body.Y * 0.12f);
						((CanvasItem)band).Modulate = new Color(Smoke.R * 0.7f, Smoke.G * 0.7f, Smoke.B * 0.7f, 0.66f * num3);
					}
					if (FxKit.Alive((GodotObject)(object)edge))
					{
						((Node2D)edge).Position = new Vector2(0f, num2 - body.Y * 0.1f);
						((CanvasItem)edge).Modulate = new Color(Rim.R, Rim.G, Rim.B, 0.7f * num3);
					}
				}
			}, (TransitionType)4, (EaseType)1).TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)holder))
				{
					((Node)holder).QueueFree();
				}
			})));
			FxKit.Burst(frontParent, val + new Vector2(0f, (0f - body.Y) * 0.15f), new Color(Smoke.R, Smoke.G, Smoke.B, 0.55f), 12, 180f, 420f, 0.6, 0.5f, 1.2f, new Vector2(0f, -120f), 14f, new Vector2(0f, -1f), body.X * 0.4f, 300f, additive: false, 0.6f);
		}

		protected override void OnStopping()
		{
			if (FxKit.Alive((GodotObject)(object)_backSmoke))
			{
				_backSmoke.Emitting = false;
			}
			if (FxKit.Alive((GodotObject)(object)_frontSmoke))
			{
				_frontSmoke.Emitting = false;
			}
		}

		static SneakyFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		}
	}
