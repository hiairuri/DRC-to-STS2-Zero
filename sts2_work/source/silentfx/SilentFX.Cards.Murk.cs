// namespace: SilentFX.Cards
// type: Murk (class)
// source lines: 90016-90150
	internal sealed class Murk
	{
		public readonly Node2D Root;

		private readonly Sprite2D _glow;

		private readonly Sprite2D _soft;

		private readonly Sprite2D _core;

		private readonly float _light;

		private Murk(Node2D root, Sprite2D glow, Sprite2D soft, Sprite2D core, float light)
		{
			Root = root;
			_glow = glow;
			_soft = soft;
			_core = core;
			_light = light;
		}

		public static Murk Make(Node parent, float light = 0.5f)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected Obj, but got Unknown
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)parent))
			{
				return null;
			}
			Node2D val = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_ShadowMurk")
			};
			parent.AddChild((Node)(object)val, false, (InternalMode)0);
			if (!((Node)val).IsInsideTree())
			{
				((Node)val).QueueFree();
				return null;
			}
			Sprite2D val2 = Shade.Aura(new Vector2(64f, 64f), 0f);
			Sprite2D val3 = Shade.Blob(new Vector2(64f, 64f), 0f);
			Sprite2D val4 = Shade.Blob(new Vector2(64f, 64f), 0f);
			((Node)val).AddChild((Node)(object)val2, false, (InternalMode)0);
			((Node)val).AddChild((Node)(object)val3, false, (InternalMode)0);
			((Node)val).AddChild((Node)(object)val4, false, (InternalMode)0);
			return new Murk(val, val2, val3, val4, light);
		}

		public void Lay(Vector2 a, Vector2 b, float thick, float alpha)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)Root) && FxKit.Alive((GodotObject)(object)_glow) && FxKit.Alive((GodotObject)(object)_soft) && FxKit.Alive((GodotObject)(object)_core))
			{
				Vector2 val = b - a;
				float num = Mathf.Max(8f, val.Length() / FogKit.ScaleOf(((Node)Root).GetParent()));
				float num2 = Mathf.Max(2f, thick);
				float num3 = Mathf.Clamp(alpha, 0f, 1f);
				Root.GlobalPosition = (a + b) * 0.5f;
				Root.GlobalRotation = ((val.LengthSquared() > 0.01f) ? val.Angle() : 0f);
				((Node2D)_glow).Scale = new Vector2(num * 1.5f, num2 * 2.4f) / 64f;
				((Node2D)_soft).Scale = new Vector2(num * 1.36f, num2 * 1.7f) / 64f;
				((Node2D)_core).Scale = new Vector2(num * 1.16f, num2) / 64f;
				((CanvasItem)_glow).Modulate = Shade.Glowing(_light * num3);
				((CanvasItem)_soft).Modulate = Shade.Inked(0.78f * num3);
				((CanvasItem)_core).Modulate = Shade.Inked(num3);
			}
		}

		public void Free()
		{
			if (FxKit.Alive((GodotObject)(object)Root))
			{
				((Node)Root).QueueFree();
			}
		}

		public static void Stroke(Node parent, Vector2 from, Vector2 to, float thick, float inTime = 0.06f, float hold = 0.04f, float fade = 0.2f, float light = 0.5f)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			Murk murk = Make(parent, light);
			if (murk != null)
			{
				float total = Mathf.Max(0.03f, inTime + hold + fade);
				FxKit.FreeAfter((Node)(object)murk.Root, total + 0.1f);
				murk.Lay(from, from.Lerp(to, 0.12f), thick, 0f);
				FxKit.Drive((Node)(object)murk.Root, total, (float u) =>
				{
					//IL_0060: Unknown result type (might be due to invalid IL or missing references)
					//IL_0086: Unknown result type (might be due to invalid IL or missing references)
					//IL_008b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0093: Unknown result type (might be due to invalid IL or missing references)
					//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
					//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
					//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
					//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
					float num = u * total;
					float num2 = Mathf.Clamp(num / Mathf.Max(0.001f, inTime), 0f, 1f);
					float num3 = Mathf.Clamp((num - inTime - hold) / Mathf.Max(0.001f, fade), 0f, 1f);
					Vector2 b = from.Lerp(to, 0.12f + 0.88f * (1f - (1f - num2) * (1f - num2)));
					Vector2 a = from.Lerp(to, 0.7f * num3 * num3);
					murk.Lay(a, b, thick * (1f - 0.6f * num3), 1f - num3);
				}, (TransitionType)0, (EaseType)2).TweenCallback(Callable.From((Action)(() =>
				{
					murk.Free();
				})));
			}
		}
	}
