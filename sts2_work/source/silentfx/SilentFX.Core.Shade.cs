// namespace: SilentFX.Core
// type: Shade (class)
// source lines: 44572-44734
	public static class Shade
	{
		public static readonly Color Ink = new Color(0.035f, 0.012f, 0.075f, 1f);

		public static readonly Color Rim = new Color(0.64f, 0.38f, 1f, 1f);

		public static readonly Color Glow = new Color(0.42f, 0.22f, 0.8f, 1f);

		public static Color Inked(float alpha)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			return new Color(Ink.R, Ink.G, Ink.B, alpha);
		}

		public static Color Rimmed(float alpha)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			return new Color(Rim.R, Rim.G, Rim.B, alpha);
		}

		public static Color Glowing(float alpha)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			return new Color(Glow.R, Glow.G, Glow.B, alpha);
		}

		public static Sprite2D Aura(Vector2 size, float alpha)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Expected Obj, but got Unknown
			return new Sprite2D
			{
				Texture = FxKit.SoftDot,
				Material = (Material)(object)FxKit.Additive,
				Modulate = Glowing(alpha),
				Scale = size / 64f
			};
		}

		public static Sprite2D Blob(Vector2 size, float alpha)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Expected Obj, but got Unknown
			return new Sprite2D
			{
				Texture = FxKit.SoftDot,
				Modulate = Inked(alpha),
				Scale = size / 64f
			};
		}

		public static void Motes(Node parent, Vector2 at, int amount, float speed, Vector2? dir = null, float spread = 180f, float radius = 10f, double life = 0.45)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			FxKit.Burst(parent, at, Inked(0.9f), amount, speed * 0.35f, speed, life, 0.1f, 0.28f, new Vector2(0f, -70f), spread, dir, radius, 0f, additive: false);
		}

		public static void Sparks(Node parent, Vector2 at, int amount, float speed, Vector2? dir = null, float spread = 180f, float radius = 8f)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			FxKit.Burst(parent, at, Rimmed(0.95f), amount, speed * 0.4f, speed, 0.32, 0.05f, 0.13f, new Vector2(0f, 260f), spread, dir, radius);
		}

		public static void Puff(Node parent, Vector2 at, float size, float grow = 1.8f, float time = 0.45f, float strength = 0.9f, float glow = 0.6f)
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)parent) || size < 2f)
			{
				return;
			}
			ShadeMist mist = new ShadeMist(1, Mathf.PosMod(at.X * 0.0131f + at.Y * 0.0077f, 10f));
			parent.AddChild((Node)(object)mist.Root, false, (InternalMode)0);
			FxKit.FreeAfter((Node)(object)mist.Root, Mathf.Max(0.05f, time) + 0.1f);
			if (!((Node)mist.Root).IsInsideTree())
			{
				((Node)mist.Root).QueueFree();
				return;
			}
			mist.Root.GlobalPosition = at;
			mist.Set("glow", glow);
			mist.Set("wisp", 1.4f);
			mist.Size(size, size);
			FxKit.Drive((Node)(object)mist.Root, Mathf.Max(0.05f, time), (float u) =>
			{
				if (FxKit.Alive((GodotObject)(object)mist.Root))
				{
					float num = 1f - (1f - u) * (1f - u);
					float num2 = size * Mathf.Lerp(1f, grow, num);
					mist.Size(num2, num2);
					mist.Intensity = strength * ((u < 0.15f) ? (u / 0.15f) : (1f - (u - 0.15f) / 0.85f));
				}
			}, (TransitionType)0, (EaseType)2).TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)mist.Root))
				{
					((Node)mist.Root).QueueFree();
				}
			})));
		}

		public static Vector2[] Partial(Vector2[] path, float fraction)
		{
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			if (path == null || path.Length == 0)
			{
				return Array.Empty<Vector2>();
			}
			float num = Mathf.Clamp(fraction, 0f, 1f) * (float)(path.Length - 1);
			int num2 = (int)num;
			float num3 = num - (float)num2;
			bool flag = num2 < path.Length - 1 && num3 > 0.001f;
			Vector2[] array = new Vector2[Mathf.Max(2, num2 + 1 + (flag ? 1 : 0))];
			for (int i = 0; i <= num2; i++)
			{
				array[i] = path[i];
			}
			if (flag)
			{
				array[num2 + 1] = path[num2].Lerp(path[num2 + 1], num3);
			}
			else if (num2 == 0)
			{
				array[1] = path[0];
			}
			return array;
		}

		static Shade()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		}
	}
