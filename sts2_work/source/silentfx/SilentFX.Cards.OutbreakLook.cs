// namespace: SilentFX.Cards
// type: OutbreakLook (class)
// source lines: 81235-81349
	internal static class OutbreakLook
	{
		public const int Segments = 20;

		public const float HeldReach = 0.34f;

		public static Vector2 Point(Vector2 from, Vector2 to, float u, float seed, float time)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = to - from;
			Vector2 val2;
			if (!(val.LengthSquared() > 1f))
			{
				val2 = Vector2.Up;
			}
			else
			{
				Vector2 val3 = new Vector2(0f - val.Y, val.X);
				val2 = val3.Normalized();
			}
			Vector2 val4 = val2;
			float num = Mathf.Sin(u * 17f + seed + time * 2.2f) * 7f * Mathf.Sin((float)Math.PI * Mathf.Min(1f, u * 1.1f));
			return from + val * u + val4 * (num * 0.5f) + new Vector2(0f, num * 0.35f);
		}

		public static Line2D Line(Node parent, float width, float alpha)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Expected Obj, but got Unknown
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Expected Obj, but got Unknown
			Color venom = PoisonFx.Venom;
			Color acid = PoisonFx.Acid;
			Gradient val = new Gradient();
			val.Offsets = new float[3] { 0f, 0.15f, 1f };
			val.Colors = new Color[3]
			{
				new Color(venom.R, venom.G, venom.B, 0f),
				new Color(venom.R, venom.G, venom.B, 0.9f * alpha),
				new Color(acid.R, acid.G, acid.B, alpha)
			};
			Line2D val2 = new Line2D
			{
				Width = width,
				Gradient = val,
				JointMode = (LineJointMode)2,
				EndCapMode = (LineCapMode)2,
				Material = (Material)(object)FxKit.Additive,
				Antialiased = true
			};
			parent.AddChild((Node)(object)val2, false, (InternalMode)0);
			return val2;
		}

		public static void Lay(Line2D line, Node2D space, Vector2[] points, Vector2 from, Vector2 to, float grow, float seed, float time)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)line) && FxKit.Alive((GodotObject)(object)space))
			{
				for (int i = 0; i <= 20; i++)
				{
					points[i] = space.ToLocal(Point(from, to, grow * (float)i / 20f, seed, time));
				}
				line.Points = points;
			}
		}
	}
