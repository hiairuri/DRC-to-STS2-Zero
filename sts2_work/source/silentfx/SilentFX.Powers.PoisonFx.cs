// namespace: SilentFX.Powers
// type: PoisonFx (class)
// source lines: 11393-11706
	public sealed class PoisonFx : BasicPowerFx
	{
		public static readonly Color Venom = new Color(0.5f, 0.98f, 0.32f, 1f);

		public static readonly Color Acid = new Color(0.82f, 1f, 0.26f, 1f);

		private static int _pendingPulses = 1;

		private GpuParticles2D _bubbles;

		private GpuParticles2D _mist;

		private float _mistAlpha;

		private int _lastAmount = -1;

		protected override string NodeName => "Poison";

		protected override bool UseBack => false;

		protected override float KickDecay => 4f;

		private static float FullAt => Mathf.Max(1f, Tuning.F("poison.full_at", 64f));

		public static void OnPoisonTriggered(PoisonPower power)
		{
			Creature val = ((power != null) ? ((PowerModel)power).Owner : null);
			if (val == null || val.IsPlayer)
			{
				return;
			}
			int num = 0;
			Creature val2 = null;
			ICombatState combatState = val.CombatState;
			if (combatState != null)
			{
				foreach (Creature item in combatState.GetOpponentsOf(val))
				{
					if (item == null || !item.IsAlive)
					{
						continue;
					}
					int powerAmount = item.GetPowerAmount<AccelerantPower>();
					if (powerAmount > 0)
					{
						num += powerAmount;
						if (val2 == null)
						{
							val2 = item;
						}
					}
				}
			}
			_pendingPulses = Math.Max(1, Math.Min(((PowerModel)power).Amount, 1 + num));
			int pendingPulses = _pendingPulses;
			PowerFxManager.Trigger((PowerModel)(object)power);
			_pendingPulses = 1;
			if (val2 != null)
			{
				AccelerantFx.OnPoisonBites(val, pendingPulses);
			}
		}

		protected override void Build(bool animateIn)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			int num = Tuning.I("poison.bubbles", 20);
			Vector2 val = Anchor.SafeBodySize();
			if (num > 0)
			{
				_bubbles = FxKit.Bubbles((Node)(object)Front, Anchor.Chest(), new Color(Venom.R, Venom.G, Venom.B, Tuning.F("poison.bubble_alpha", 0.75f)), num, new Vector2(val.X * 0.3f, val.Y * 0.28f), 22f, 62f, 1.5, 0.1f, 0.24f, oneShot: false);
			}
			_mist = MakeMist(val);
			((Node)Front).AddChild((Node)(object)_mist, false, (InternalMode)0);
			_mistAlpha = (((float)Amount > FullAt) ? 1f : 0f);
			_lastAmount = Amount;
			ApplyAmount();
			if (animateIn)
			{
				Puff(1f);
			}
		}

		private void ApplyAmount()
		{
			if (FxKit.Alive((GodotObject)(object)_bubbles))
			{
				_bubbles.AmountRatio = Mathf.Clamp((float)Amount / FullAt, 0.14f, 1f);
			}
		}

		private static GpuParticles2D MakeMist(Vector2 body)
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
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Expected Obj, but got Unknown
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Expected Obj, but got Unknown
			ParticleProcessMaterial processMaterial = new ParticleProcessMaterial
			{
				EmissionShape = (EmissionShapeEnum)3,
				EmissionBoxExtents = new Vector3(body.X * 0.36f, body.Y * 0.3f, 0f),
				Direction = new Vector3(0f, -1f, 0f),
				Spread = 35f,
				InitialVelocityMin = 12f,
				InitialVelocityMax = 40f,
				Gravity = new Vector3(0f, -16f, 0f),
				ScaleMin = 1.3f,
				ScaleMax = 2.6f,
				Color = new Color(Venom.R, Venom.G, Venom.B, Tuning.F("poison.mist_alpha", 0.1f)),
				ColorRamp = (Texture2D)(object)FxKit.FadeInOut(0.3f),
				ParticleFlagDisableZ = true
			};
			return new GpuParticles2D
			{
				Name = StringName.op_Implicit("Mist"),
				Amount = Math.Max(1, Tuning.I("poison.mist", 12)),
				Lifetime = 2.2,
				Randomness = 0.6f,
				Texture = FxKit.SoftDot,
				ProcessMaterial = (Material)(object)processMaterial,
				Material = (Material)(object)FxKit.Additive,
				VisibilityRect = new Rect2(-600f, -800f, 1200f, 1200f)
			};
		}

		protected override void Tick(float dt)
		{
			_mistAlpha = FxKit.Damp(_mistAlpha, ((float)Amount > FullAt) ? 1f : 0f, 2.5f, dt);
		}

		public override void OnAmountChanged(int amount)
		{
			if (!Dead)
			{
				if (_lastAmount >= 0 && amount > _lastAmount && !Stopping)
				{
					Puff(0.6f);
				}
				_lastAmount = amount;
				ApplyAmount();
			}
		}

		private void Puff(float strength)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			Node frontParent = Anchor.FrontParent;
			if (FxKit.Alive((GodotObject)(object)frontParent))
			{
				Vector2 val = Anchor.SafeBodySize();
				Vector2 val2 = Anchor.Chest();
				Kick = Mathf.Max(Kick, strength);
				FxKit.Bubbles(frontParent, val2, new Color(Venom.R, Venom.G, Venom.B, 0.9f), 6 + (int)(10f * strength), new Vector2(val.X * 0.26f, val.Y * 0.2f), 70f, 190f, 0.75, 0.12f, 0.3f);
				FxKit.Flash(frontParent, val2, new Color(Venom.R, Venom.G, Venom.B, 0.3f * strength), Mathf.Clamp(val.X, 120f, 320f) * 0.9f, 1.6f, 0.35);
			}
		}

		protected override void OnTrigger(Vector2? at)
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			int num = Mathf.Clamp(_pendingPulses, 1, 6);
			Node parent = Anchor.FrontParent;
			if (!FxKit.Alive((GodotObject)(object)parent))
			{
				return;
			}
			Vector2 chest = Anchor.Chest();
			float width = Mathf.Clamp(Anchor.SafeBodySize().X, 120f, 380f);
			float num2 = Tuning.F("poison.pulse_gap", 0.2f);
			for (int i = 0; i < num; i++)
			{
				int index = i;
				FxKit.After(parent, (float)i * num2, () =>
				{
					//IL_0011: Unknown result type (might be due to invalid IL or missing references)
					Bite(parent, chest, width, index);
				});
			}
		}

		public static void Bite(Node parent, Vector2 chest, float width, int index)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)parent))
			{
				Color val = ((index == 0) ? Venom : Acid);
				float num = Tuning.F("poison.bite_scale", 1f);
				FxKit.RingPulse(parent, chest, width * 1.5f * num, val, 0f, 0.42f, 0.35f, 1.25f);
				FxKit.Flash(parent, chest, new Color(val.R, val.G, val.B, 0.55f), width * 0.7f * num, 1.9f, 0.28);
				FxKit.Bubbles(parent, chest, new Color(val.R, val.G, val.B, 0.95f), Tuning.I("poison.bite_bubbles", 14), new Vector2(width * 0.24f, width * 0.26f), 90f, 250f, 0.8, 0.12f * num, 0.34f * num);
				FxKit.Burst(parent, chest, new Color(val.R * 0.8f, val.G, val.B * 0.8f, 0.9f), 9, 30f, 150f, 0.7, 0.1f * num, 0.2f * num, new Vector2(0f, 620f), 180f, null, width * 0.22f);
			}
		}

		protected override void Layout()
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)Front))
			{
				return;
			}
			((CanvasItem)Front).Modulate = new Color(1f, 1f, 1f, A);
			if (FxKit.Alive((GodotObject)(object)_bubbles))
			{
				Vector2 val = Anchor.SafeBodySize();
				((Node2D)_bubbles).GlobalPosition = Anchor.Chest() + new Vector2(0f, val.Y * 0.08f);
				float num = 1f + 0.8f * Kick;
				((CanvasItem)_bubbles).Modulate = new Color(num, num, num, 1f);
			}
			if (FxKit.Alive((GodotObject)(object)_mist))
			{
				((Node2D)_mist).GlobalPosition = Anchor.Chest();
				bool flag = !Stopping && (float)Amount > FullAt;
				if (_mist.Emitting != flag)
				{
					_mist.Emitting = flag;
				}
				((CanvasItem)_mist).Modulate = new Color(1f, 1f, 1f, _mistAlpha);
			}
		}

		protected override void OnStopping()
		{
			if (FxKit.Alive((GodotObject)(object)_bubbles))
			{
				_bubbles.Emitting = false;
			}
		}

		static PoisonFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		}
	}
