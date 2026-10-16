// namespace: SilentFX.Powers
// type: WraithFormFx (class)
// source lines: 19276-19449
	public sealed class WraithFormFx : BasicPowerFx
	{
		private static readonly StringName PIntensity = StringName.op_Implicit("intensity");

		private static readonly StringName PPulse = StringName.op_Implicit("pulse");

		private ShaderMaterial _matBack;

		private ShaderMaterial _matFront;

		private float _since = 100f;

		private bool _waiting;

		private float _gate = 1f;

		protected override string NodeName => "WraithForm";

		protected override void Build(bool animateIn)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = Anchor.SafeBodySize();
			float num = val.X * Tuning.F("wraith.width_mul", 1.9f) + 140f;
			float num2 = val.Y * Tuning.F("wraith.height_mul", 1.5f) + 60f;
			_matBack = ShaderLib.NewMaterial("wraith_smoke");
			if (_matBack != null)
			{
				_matBack.SetShaderParameter(StringName.op_Implicit("layer"), Variant.op_Implicit(0f));
				_matBack.SetShaderParameter(StringName.op_Implicit("seed"), Variant.op_Implicit(0f));
				_matBack.SetShaderParameter(PIntensity, Variant.op_Implicit(0f));
				ColorRect val2 = FxKit.Quad(new Vector2(num, num2), (Material)(object)_matBack);
				((Control)val2).Position = new Vector2((0f - num) * 0.5f, 0f - (num2 - 30f));
				((Node)Back).AddChild((Node)(object)val2, false, (InternalMode)0);
			}
			_matFront = ShaderLib.NewMaterial("wraith_smoke");
			if (_matFront != null)
			{
				_matFront.SetShaderParameter(StringName.op_Implicit("layer"), Variant.op_Implicit(1f));
				_matFront.SetShaderParameter(StringName.op_Implicit("seed"), Variant.op_Implicit(11.7f));
				_matFront.SetShaderParameter(PIntensity, Variant.op_Implicit(0f));
				ColorRect val3 = FxKit.Quad(new Vector2(num, num2), (Material)(object)_matFront);
				((Control)val3).Position = new Vector2((0f - num) * 0.5f, 0f - (num2 - 30f));
				((Node)Front).AddChild((Node)(object)val3, false, (InternalMode)0);
			}
			if (animateIn)
			{
				_since = 0f;
			}
			if (Waits())
			{
				_waiting = true;
				_gate = 0f;
			}
		}

		private bool Waits()
		{
			int num = WraithFormIntroFx.PhaseFor(Anchor.Owner);
			if (num != 1)
			{
				return num == 2;
			}
			return true;
		}

		protected override void Tick(float dt)
		{
			if (!Stopping && Waits())
			{
				_waiting = true;
				_since = 0f;
				_gate = FxKit.Damp(_gate, 0f, 9f, dt);
				return;
			}
			if (_waiting)
			{
				_waiting = false;
				_since = 0f;
			}
			_gate = FxKit.Damp(_gate, 1f, 9f, dt);
			_since += dt;
		}

		private float Fog()
		{
			float num = Tuning.F("wraith.fog_in", 0.28f);
			float num2 = Tuning.F("wraith.fog_hold", 0.9f);
			float num3 = Tuning.F("wraith.fog_out", 1.1f);
			float num4 = Mathf.Clamp(Tuning.F("wraith.fog_rest", 0.34f), 0f, 1f) * (0.88f + 0.12f * Mathf.Sin(Time * 1.4f));
			if (_since < num)
			{
				return Mathf.Max(num4, _since / num);
			}
			if (_since < num + num2)
			{
				return 1f;
			}
			float num5 = 1f - (_since - num - num2) / num3;
			float num6 = ((num5 <= 0f) ? 0f : (num5 * num5 * (3f - 2f * num5)));
			return Mathf.Max(num4, num6);
		}

		protected override void OnTrigger(Vector2? at)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			_since = 0f;
			Node frontParent = Anchor.FrontParent;
			if (FxKit.Alive((GodotObject)(object)frontParent))
			{
				Vector2 val = Anchor.SafeBodySize();
				FxKit.Burst(frontParent, Anchor.Chest() + new Vector2(0f, (0f - val.Y) * 0.2f), new Color(0.56f, 1f, 0.64f, 0.85f), Tuning.I("wraith.motes_amount", 16), 20f, 90f, 1.1, 0.08f, 0.22f, new Vector2(0f, 260f), 40f, new Vector2(0f, 1f), val.X * 0.4f, 0f, additive: true, 0.25f);
			}
		}

		protected override void Layout()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			Vector2 globalPosition = Anchor.Feet();
			float fog = Fog() * A * Mathf.Clamp(_gate, 0f, 1f);
			float num = Mathf.Lerp(1.3f, 0.9f, Mathf.Clamp(_since / 0.9f, 0f, 1f));
			if (FxKit.Alive((GodotObject)(object)Back))
			{
				Back.GlobalPosition = globalPosition;
				Back.Scale = new Vector2(num, 1f);
			}
			if (FxKit.Alive((GodotObject)(object)Front))
			{
				Front.GlobalPosition = globalPosition;
				Front.Scale = new Vector2(num, 1f);
			}
			UpdateMaterial(_matBack, fog);
			UpdateMaterial(_matFront, fog);
		}

		private static void UpdateMaterial(ShaderMaterial mat, float fog)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)mat))
			{
				mat.SetShaderParameter(PIntensity, Variant.op_Implicit(fog));
				mat.SetShaderParameter(PPulse, Variant.op_Implicit(0f));
			}
		}
	}
}
namespace SilentFX.Patches
{
	[HarmonyPatch(typeof(MessageTypes), "Initialize")]
