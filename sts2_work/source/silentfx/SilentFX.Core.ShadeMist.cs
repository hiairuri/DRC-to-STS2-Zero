// namespace: SilentFX.Core
// type: ShadeMist (class)
// source lines: 44735-44828
	public sealed class ShadeMist
	{
		private static readonly StringName PIntensity = StringName.op_Implicit("intensity");

		private static readonly StringName PPulse = StringName.op_Implicit("pulse");

		public const int Column = 0;

		public const int Blob = 1;

		public const int Bank = 2;

		private const float Base = 100f;

		private const float Ground = 0.94f;

		public readonly Node2D Root;

		private readonly ShaderMaterial _mat;

		private readonly bool _column;

		public float Intensity
		{
			set
			{
				Set(PIntensity, Mathf.Clamp(value, 0f, 1f));
			}
		}

		public float Pulse
		{
			set
			{
				Set(PPulse, Mathf.Clamp(value, 0f, 1f));
			}
		}

		public ShadeMist(int mode, float seed)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected Obj, but got Unknown
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			_column = mode == 0;
			Root = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_ShadeMist")
			};
			_mat = ShaderLib.NewMaterial("shade_mist");
			if (_mat != null)
			{
				_mat.SetShaderParameter(StringName.op_Implicit("mode"), Variant.op_Implicit((float)mode));
				_mat.SetShaderParameter(StringName.op_Implicit("seed"), Variant.op_Implicit(seed));
				_mat.SetShaderParameter(PIntensity, Variant.op_Implicit(0f));
				ColorRect val = FxKit.Quad(new Vector2(100f, 100f), (Material)(object)_mat);
				((Control)val).Position = (_column ? new Vector2(-50f, -94f) : new Vector2(-50f, -50f));
				((Node)Root).AddChild((Node)(object)val, false, (InternalMode)0);
			}
		}

		public void Size(float width, float height)
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)Root))
			{
				float num = (_column ? (height / 0.94f) : height);
				Root.Scale = new Vector2(Mathf.Max(1f, width) / 100f, Mathf.Max(1f, num) / 100f);
			}
		}

		public void Set(string name, float value)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)_mat))
			{
				_mat.SetShaderParameter(StringName.op_Implicit(name), Variant.op_Implicit(value));
			}
		}

		public void Set(StringName name, float value)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)_mat))
			{
				_mat.SetShaderParameter(name, Variant.op_Implicit(value));
			}
		}
	}
