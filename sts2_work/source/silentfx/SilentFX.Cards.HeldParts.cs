// namespace: SilentFX.Cards
// type: HeldParts (class)
// source lines: 83698-83777
	internal static class HeldParts
	{
		public static Node2D AimRing(Node parent, float size, Color tint, out ShaderMaterial mat)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			Node2D val = FxKit.Sigil(size, tint, out mat);
			parent.AddChild((Node)(object)val, false, (InternalMode)0);
			if (mat != null)
			{
				mat.SetShaderParameter(StringName.op_Implicit("radius"), Variant.op_Implicit(0.8f));
				mat.SetShaderParameter(StringName.op_Implicit("width"), Variant.op_Implicit(0.035f));
				mat.SetShaderParameter(StringName.op_Implicit("dashes"), Variant.op_Implicit(8f));
				mat.SetShaderParameter(StringName.op_Implicit("glow"), Variant.op_Implicit(0.3f));
				mat.SetShaderParameter(StringName.op_Implicit("alpha"), Variant.op_Implicit(0f));
			}
			return val;
		}

		public static GpuParticles2D Drips(Color color, int amount, float halfWidth, float halfHeight)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected Obj, but got Unknown
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Expected Obj, but got Unknown
			ParticleProcessMaterial val = new ParticleProcessMaterial();
			val.EmissionShape = (EmissionShapeEnum)3;
			val.EmissionBoxExtents = new Vector3(halfWidth, halfHeight, 0f);
			val.Direction = new Vector3(0f, 1f, 0f);
			val.Spread = 10f;
			val.InitialVelocityMin = 10f;
			val.InitialVelocityMax = 50f;
			val.Gravity = new Vector3(0f, 640f, 0f);
			val.ScaleMin = 0.07f;
			val.ScaleMax = 0.16f;
			val.Color = color;
			val.ColorRamp = (Texture2D)(object)FxKit.Ramp(new float[3] { 0f, 0.7f, 1f }, new Color[3]
			{
				new Color(1f, 1f, 1f, 1f),
				new Color(1f, 1f, 1f, 0.8f),
				new Color(1f, 1f, 1f, 0f)
			});
			val.ParticleFlagDisableZ = true;
			ParticleProcessMaterial processMaterial = val;
			return new GpuParticles2D
			{
				Amount = amount,
				Lifetime = 0.6,
				Randomness = 0.6f,
				LocalCoords = false,
				Texture = FxKit.SoftDot,
				ProcessMaterial = (Material)(object)processMaterial,
				Material = (Material)(object)FxKit.Additive,
				VisibilityRect = new Rect2(-600f, -600f, 1200f, 1600f)
			};
		}
	}
