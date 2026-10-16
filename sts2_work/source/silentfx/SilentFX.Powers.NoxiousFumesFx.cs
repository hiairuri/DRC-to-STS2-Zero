// namespace: SilentFX.Powers
// type: NoxiousFumesFx (class)
// source lines: 9418-10115
	public sealed class NoxiousFumesFx : PowerFx
	{
		private sealed class Bank
		{
			public float Side;

			public Node2D Back;

			public Node2D Front;

			public ColorRect BackQuad;

			public ColorRect FrontQuad;

			public ShaderMaterial MatBack;

			public ShaderMaterial MatFront;

			public GpuParticles2D Motes;

			public float Near;

			public float Far;

			public float Floor;

			public bool Placed;

			public float Show;

			public float Scroll;

			public float Surge = -1f;
		}

		private static readonly StringName PIntensity = StringName.op_Implicit("intensity");

		private static readonly StringName PThick = StringName.op_Implicit("thick");

		private static readonly StringName PRise = StringName.op_Implicit("rise");

		private static readonly StringName PFloorY = StringName.op_Implicit("floor_y");

		private static readonly StringName PNearSoft = StringName.op_Implicit("near_soft");

		private static readonly StringName PFarSoft = StringName.op_Implicit("far_soft");

		private static readonly StringName PScroll = StringName.op_Implicit("scroll");

		private static readonly StringName PChurn = StringName.op_Implicit("churn");

		private static readonly StringName PPulse = StringName.op_Implicit("pulse");

		private static readonly StringName PPulseAt = StringName.op_Implicit("pulse_at");

		private readonly List<FxTarget> _targetScratch = new List<FxTarget>();

		private readonly Bank[] _banks = new Bank[2];

		private float _time;

		private float _wait;

		private float _fade;

		private float _level;

		private int _seenAmount = int.MinValue;

		private bool _noShader;

		private bool _stopping;

		private bool _dead;

		public override bool IsAlive => !_dead;

		public static float SheetTravel => Mathf.Max(0.2f, Tuning.F("fumes.sheet_travel", 0.8f));

		public static float SheetSettle => Mathf.Max(0.2f, Tuning.F("fumes.sheet_settle", 0.75f));

		private static float SurgeTime => Mathf.Max(0.2f, Tuning.F("fumes.surge_time", 0.8f));

		public static float RiseFor(Vector2 body)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			return Tuning.F("fumes.fog_rise", 250f) * Mathf.Clamp(body.Y / 244f, 0.6f, 1.3f);
		}

		public static float SeedOf(float side)
		{
			if (!(side < 0f))
			{
				return 3.7f;
			}
			return 17.2f;
		}

		private static float LevelOf(int amount)
		{
			float num = Mathf.Max(2f, Tuning.F("fumes.fog_levels", 6f));
			return Mathf.Clamp(((float)amount - 1f) / (num - 1f), 0f, 1f);
		}

		public static bool RegionOf(List<FxTarget> targets, float side, Vector2 chest, Vector2 feet, Vector2 body, out float near, out float far, out float floor)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			near = 0f;
			far = 0f;
			floor = feet.Y;
			if (targets == null)
			{
				return false;
			}
			float num = ((side < 0f) ? (-1f) : 1f);
			bool flag = false;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = feet.Y;
			foreach (FxTarget target in targets)
			{
				if (target != null && target.Side < 0f == num < 0f)
				{
					float num5 = Mathf.Max(target.Width, 80f) * 0.5f;
					float num6 = (target.Feet.X - chest.X) * num;
					if (!flag || num6 - num5 < num2)
					{
						num2 = num6 - num5;
					}
					if (!flag || num6 + num5 > num3)
					{
						num3 = num6 + num5;
					}
					num4 = Mathf.Max(num4, target.Feet.Y);
					flag = true;
				}
			}
			if (!flag)
			{
				return false;
			}
			float num7 = Mathf.Max(body.X * Tuning.F("fumes.fog_clear", 0.8f), num2 - Tuning.F("fumes.fog_lead", 190f));
			float num8 = num3 + Tuning.F("fumes.fog_past", 240f);
			if (Tuning.F("fumes.fog_to_edge", 1f) >= 0.5f)
			{
				Vector2 val = FxKit.ViewSize();
				float num9 = ((num > 0f) ? (val.X - chest.X) : chest.X) + 140f;
				num8 = Mathf.Max(num8, num9);
			}
			num8 = Mathf.Max(num8, num7 + 320f);
			near = chest.X + num * num7;
			far = chest.X + num * num8;
			floor = num4;
			return true;
		}

		public override void Start(bool animateIn)
		{
			try
			{
				_seenAmount = Amount;
				_level = LevelOf(Amount);
				bool flag = animateIn && Tuning.F("intro.on", 1f) >= 0.5f;
				_wait = (flag ? (SheetTravel * 0.8f) : 0f);
				_fade = (animateIn ? 0f : 1f);
				Step(0f);
			}
			catch (Exception ex)
			{
				DevLog.Warn("NoxiousFumesFx.Start: " + ex);
				Dispose();
			}
		}

		public override void Update(double dt)
		{
			try
			{
				if (_dead)
				{
					return;
				}
				if (!Anchor.Valid())
				{
					Dispose();
					return;
				}
				Step((float)dt);
				if (_stopping && _fade < 0.02f)
				{
					Dispose();
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("NoxiousFumesFx.Update: " + ex);
				Dispose();
			}
		}

		public override void OnAmountChanged(int amount)
		{
			try
			{
				bool num = _seenAmount != int.MinValue && amount > _seenAmount;
				_seenAmount = amount;
				if (num && !_dead && !_stopping)
				{
					Surge();
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("NoxiousFumesFx.OnAmountChanged: " + ex);
				Dispose();
			}
		}

		public override void Trigger(Vector2? at = null)
		{
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			if (_dead || _stopping)
			{
				return;
			}
			Surge();
			Node front = Anchor.FrontParent;
			if (!FxKit.Alive((GodotObject)(object)front))
			{
				return;
			}
			float surgeTime = SurgeTime;
			int num;
			if (MultiplayerFx.Active)
			{
				object owner = Anchor.Owner;
				Creature val = (Creature)((owner is Creature) ? owner : null);
				if (val != null && val.IsPlayer)
				{
					num = ((!MultiplayerFx.IsLocal(val)) ? 1 : 0);
					goto IL_0079;
				}
			}
			num = 0;
			goto IL_0079;
			IL_0079:
			bool remoteFade = (byte)num != 0;
			Anchor.SafeTargets(_targetScratch);
			foreach (FxTarget item in _targetScratch)
			{
				if (item == null)
				{
					continue;
				}
				Bank bank = _banks[(item.Side < 0f) ? 1u : 0u];
				float num2 = 0f;
				if (bank != null && bank.Placed)
				{
					float num3 = Mathf.Abs(bank.Far - bank.Near);
					float num4 = Mathf.Abs(item.Feet.X - bank.Near);
					num2 = surgeTime * Mathf.Clamp((num4 + 200f) / (num3 + 400f), 0f, 1f);
				}
				Vector2 feet = item.Feet;
				float width = Mathf.Max(item.Width, 130f);
				FxKit.After(front, num2, () =>
				{
					//IL_0049: Unknown result type (might be due to invalid IL or missing references)
					using (RemoteFxFade.Capture(remoteFade, front, Anchor.BackParent))
					{
						Erupt(feet, width);
					}
				});
			}
		}

		public override void Stop(bool animateOut)
		{
			if (_dead)
			{
				return;
			}
			if (!animateOut)
			{
				Dispose();
				return;
			}
			_stopping = true;
			Bank[] banks = _banks;
			foreach (Bank bank in banks)
			{
				if (bank != null && FxKit.Alive((GodotObject)(object)bank.Motes))
				{
					bank.Motes.Emitting = false;
				}
			}
		}

		private void Surge()
		{
			Bank[] banks = _banks;
			foreach (Bank bank in banks)
			{
				if (bank != null && bank.Placed)
				{
					bank.Surge = 0f;
				}
			}
		}

		private void Step(float dt)
		{
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			_time += dt;
			float target;
			if (_stopping)
			{
				target = 0f;
			}
			else
			{
				target = ((_time >= _wait) ? 1f : 0f);
			}
			_fade = FxKit.Damp(_fade, target, _stopping ? 3.4f : 2.8f, dt);
			_level = FxKit.Damp(_level, LevelOf(Amount), 2.4f, dt);
			if (_noShader || !FxKit.Alive((GodotObject)(object)Anchor.BackParent) || !FxKit.Alive((GodotObject)(object)Anchor.FrontParent))
			{
				return;
			}
			if (!_stopping)
			{
				Anchor.SafeTargets(_targetScratch);
			}
			List<FxTarget> targets = (_stopping ? null : _targetScratch);
			Vector2 chest = Anchor.Chest();
			Vector2 feet = Anchor.Feet();
			Vector2 body = Anchor.SafeBodySize();
			for (int i = 0; i < 2; i++)
			{
				float side = ((i == 0) ? 1f : (-1f));
				bool flag = RegionOf(targets, side, chest, feet, body, out var near, out var far, out var floor);
				Bank bank = _banks[i];
				if (bank == null)
				{
					if (!flag)
					{
						continue;
					}
					bank = MakeBank(side);
					if (bank == null)
					{
						_noShader = true;
						break;
					}
					bank.Show = ((_time <= 0.05f) ? 1f : 0f);
					_banks[i] = bank;
				}
				if (flag)
				{
					if (!bank.Placed)
					{
						bank.Near = near;
						bank.Far = far;
						bank.Floor = floor;
						bank.Placed = true;
					}
					else
					{
						bank.Near = FxKit.Damp(bank.Near, near, 3.5f, dt);
						bank.Far = FxKit.Damp(bank.Far, far, 3.5f, dt);
						bank.Floor = FxKit.Damp(bank.Floor, floor, 3.5f, dt);
					}
				}
				bank.Show = FxKit.Damp(bank.Show, flag ? 1f : 0f, flag ? 4f : 1.8f, dt);
				if (bank.Placed)
				{
					Draw(bank, dt, body);
				}
			}
		}

		private Bank MakeBank(float side)
		{
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			Bank bank = new Bank
			{
				Side = side
			};
			_banks[(side < 0f) ? 1u : 0u] = bank;
			bank.Back = FogKit.Quad(Anchor.BackParent, "SilentFX_NoxiousBack", out bank.BackQuad, out bank.MatBack);
			bank.Front = FogKit.Quad(Anchor.FrontParent, "SilentFX_NoxiousFront", out bank.FrontQuad, out bank.MatFront);
			if (bank.Back == null && bank.Front == null)
			{
				return null;
			}
			float num = SeedOf(side);
			if (bank.MatBack != null)
			{
				bank.MatBack.SetShaderParameter(StringName.op_Implicit("layer"), Variant.op_Implicit(0f));
				bank.MatBack.SetShaderParameter(StringName.op_Implicit("dir"), Variant.op_Implicit(side));
				bank.MatBack.SetShaderParameter(StringName.op_Implicit("seed"), Variant.op_Implicit(num));
			}
			if (bank.MatFront != null)
			{
				bank.MatFront.SetShaderParameter(StringName.op_Implicit("layer"), Variant.op_Implicit(1f));
				bank.MatFront.SetShaderParameter(StringName.op_Implicit("dir"), Variant.op_Implicit(side));
				bank.MatFront.SetShaderParameter(StringName.op_Implicit("seed"), Variant.op_Implicit(num + 7.3f));
			}
			if (FxKit.Alive((GodotObject)(object)bank.Front))
			{
				bank.Motes = BuildMotes(side);
				((Node)bank.Front).AddChild((Node)(object)bank.Motes, false, (InternalMode)0);
			}
			return bank;
		}

		private void Draw(Bank bank, float dt, Vector2 body)
		{
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_034b: Unknown result type (might be due to invalid IL or missing references)
			float num = Mathf.Pow(Mathf.Clamp(_level, 0f, 1f), 0.8f);
			float num2 = Mathf.Clamp(_fade, 0f, 1f) * Mathf.Clamp(bank.Show, 0f, 1f);
			float num3 = RiseFor(body);
			float rise = num3 * Mathf.Lerp(Tuning.F("fumes.fog_low", 0.6f), 1f, num);
			float num4 = Mathf.Lerp(Tuning.F("fumes.fog_min", 0.18f), Tuning.F("fumes.fog_max", 0.7f), num);
			float num5 = Mathf.Lerp(Tuning.F("fumes.veil_min", 0.04f), Tuning.F("fumes.veil_max", 0.26f), num);
			float thick = Mathf.Lerp(0.85f, 1.2f, num);
			float num6 = Mathf.Max(64f, Mathf.Abs(bank.Far - bank.Near));
			float num7 = 0f;
			float surgeAt = 0f;
			if (bank.Surge >= 0f)
			{
				bank.Surge += dt / SurgeTime;
				if (bank.Surge >= 1f)
				{
					bank.Surge = -1f;
				}
				else
				{
					num7 = Mathf.Sqrt(Mathf.Max(0f, Mathf.Sin((float)Math.PI * bank.Surge)));
					surgeAt = -200f + (num6 + 400f) * bank.Surge;
				}
			}
			bank.Scroll += Tuning.F("fumes.fog_drift", 46f) * (1f + 1.6f * num7) * dt;
			float num8 = bank.Floor - num3 * 1.75f - 50f;
			Vector2 at = new Vector2(Mathf.Min(bank.Near, bank.Far), num8);
			Vector2 size = new Vector2(num6, num3 * 1.75f + 160f);
			bool visible = num2 > 0.004f;
			Lay(bank.Back, bank.BackQuad, bank.MatBack, at, size, visible);
			Lay(bank.Front, bank.FrontQuad, bank.MatFront, at, size, visible);
			float floorY = bank.Floor - num8;
			Feed(bank.MatBack, bank, num4 * num2, thick, rise, floorY, num7, surgeAt);
			Feed(bank.MatFront, bank, num5 * num2, thick, rise, floorY, num7, surgeAt);
			if (FxKit.Alive((GodotObject)(object)bank.Motes) && FxKit.Alive((GodotObject)(object)bank.Front))
			{
				float num9 = FogKit.ScaleOf(((Node)bank.Front).GetParent());
				((Node2D)bank.Motes).Position = new Vector2(num6 * 0.5f, bank.Floor - num8 - 20f) / num9;
				((CanvasItem)bank.Motes).Modulate = new Color(1f, 1f, 1f, num2);
				bank.Motes.AmountRatio = Mathf.Clamp(0.15f + 0.65f * num + 0.35f * num7, 0f, 1f);
				Material processMaterial = bank.Motes.ProcessMaterial;
				ParticleProcessMaterial val = (ParticleProcessMaterial)(object)((processMaterial is ParticleProcessMaterial) ? processMaterial : null);
				if (val != null)
				{
					val.EmissionBoxExtents = new Vector3(Mathf.Max(60f, num6 * 0.4f) / num9, 16f, 0f);
				}
			}
		}

		private static void Lay(Node2D holder, ColorRect quad, ShaderMaterial mat, Vector2 at, Vector2 size, bool visible)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)holder))
			{
				((CanvasItem)holder).Visible = visible;
				if (visible)
				{
					FogKit.Place(holder, quad, mat, at, size);
				}
			}
		}

		private void Feed(ShaderMaterial mat, Bank bank, float alpha, float thick, float rise, float floorY, float surge, float surgeAt)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)mat))
			{
				mat.SetShaderParameter(PIntensity, Variant.op_Implicit(Mathf.Clamp(alpha, 0f, 1f)));
				mat.SetShaderParameter(PThick, Variant.op_Implicit(thick));
				mat.SetShaderParameter(PRise, Variant.op_Implicit(rise));
				mat.SetShaderParameter(PFloorY, Variant.op_Implicit(floorY));
				mat.SetShaderParameter(PNearSoft, Variant.op_Implicit(Tuning.F("fumes.fog_soft", 260f)));
				mat.SetShaderParameter(PFarSoft, Variant.op_Implicit(120f));
				mat.SetShaderParameter(PScroll, Variant.op_Implicit(bank.Scroll));
				mat.SetShaderParameter(PChurn, Variant.op_Implicit(_time));
				mat.SetShaderParameter(PPulse, Variant.op_Implicit(surge));
				mat.SetShaderParameter(PPulseAt, Variant.op_Implicit(surgeAt));
			}
		}

		private static GpuParticles2D BuildMotes(float side)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected Obj, but got Unknown
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Expected Obj, but got Unknown
			ParticleProcessMaterial val = new ParticleProcessMaterial();
			val.EmissionShape = (EmissionShapeEnum)3;
			val.EmissionBoxExtents = new Vector3(300f, 16f, 0f);
			val.Direction = new Vector3(side * 0.45f, -1f, 0f);
			val.Spread = 16f;
			val.InitialVelocityMin = 14f;
			val.InitialVelocityMax = 46f;
			val.Gravity = new Vector3(side * 14f, -24f, 0f);
			val.ScaleMin = 0.1f;
			val.ScaleMax = 0.28f;
			val.Color = new Color(0.6f, 1f, 0.36f, 0.8f);
			val.ColorRamp = (Texture2D)(object)FxKit.Ramp(new float[4] { 0f, 0.18f, 0.7f, 1f }, new Color[4]
			{
				new Color(1f, 1f, 1f, 0f),
				new Color(1f, 1f, 1f, 1f),
				new Color(1f, 1f, 1f, 0.7f),
				new Color(1f, 1f, 1f, 0f)
			});
			val.ParticleFlagDisableZ = true;
			ParticleProcessMaterial processMaterial = val;
			return new GpuParticles2D
			{
				Name = StringName.op_Implicit("Motes"),
				Amount = Tuning.I("fumes.fog_motes", 26),
				Lifetime = 2.8,
				Preprocess = 2.0,
				Randomness = 0.6f,
				Texture = FxKit.SoftDot,
				ProcessMaterial = (Material)(object)processMaterial,
				Material = (Material)(object)FxKit.Additive,
				VisibilityRect = new Rect2(-2400f, -900f, 4800f, 1400f)
			};
		}

		private void Dispose()
		{
			if (_dead)
			{
				return;
			}
			_dead = true;
			for (int i = 0; i < _banks.Length; i++)
			{
				Bank bank = _banks[i];
				if (bank == null)
				{
					continue;
				}
				try
				{
					if (FxKit.Alive((GodotObject)(object)bank.Back))
					{
						((Node)bank.Back).QueueFree();
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("Noxious Fumes back cleanup: " + ex);
				}
				try
				{
					if (FxKit.Alive((GodotObject)(object)bank.Front))
					{
						((Node)bank.Front).QueueFree();
					}
				}
				catch (Exception ex2)
				{
					DevLog.Warn("Noxious Fumes front cleanup: " + ex2);
				}
				_banks[i] = null;
			}
		}

		private void Erupt(Vector2 feet, float width)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			if (_dead || _stopping)
			{
				return;
			}
			Node frontParent = Anchor.FrontParent;
			Node backParent = Anchor.BackParent;
			if (FxKit.Alive((GodotObject)(object)frontParent))
			{
				Color venom = PoisonFx.Venom;
				float num = Tuning.F("noxious.burst_size", 1f);
				if (FxKit.Alive((GodotObject)(object)backParent))
				{
					FxKit.RingPulse(backParent, feet, width * 2.1f * num, venom, 0f, 0.5f, 0.25f, 1.2f, 0.3f, 0f, 0.04f, 0.95f);
				}
				FxKit.Flash(frontParent, feet + new Vector2(0f, -18f), new Color(venom.R, venom.G, venom.B, 0.55f), width * 0.9f * num, 1.7f, 0.34);
				FxKit.Burst(frontParent, feet + new Vector2(0f, -10f), new Color(0.24f, 0.66f, 0.2f, Tuning.F("noxious.burst_alpha", 0.3f)), Tuning.I("noxious.burst_gas", 14), 90f, 300f, 1.0, 0.9f * num, 2.1f * num, new Vector2(0f, -70f), 38f, new Vector2(0f, -1f), width * 0.3f, 90f, additive: false, 0.9f);
				FxKit.Bubbles(frontParent, feet + new Vector2(0f, -10f), new Color(venom.R, venom.G, venom.B, 0.9f), Tuning.I("noxious.burst_bubbles", 18), new Vector2(width * 0.32f, 6f), 140f, 420f, 0.9, 0.1f, 0.32f);
				FxKit.Burst(frontParent, feet + new Vector2(0f, -10f), new Color(0.7f, 1f, 0.42f, 0.95f), 14, 200f, 520f, 0.6, 0.07f, 0.18f, new Vector2(0f, 760f), 30f, new Vector2(0f, -1f), width * 0.25f);
			}
		}
	}
