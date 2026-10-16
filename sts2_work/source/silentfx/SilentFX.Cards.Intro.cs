// namespace: SilentFX.Cards
// type: Intro (class)
// source lines: 69142-69352
	public static class Intro
	{
		public static float Unit(CardFxContext c)
		{
			return Mathf.Clamp(c.Body.Y / 300f, 0.7f, 1.25f) * Tuning.F("intro.scale", 1.8f);
		}

		public static Node2D Stage(CardFxContext c, IntroSpot spot, float life)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Expected Obj, but got Unknown
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)c.FrontParent))
			{
				return null;
			}
			float num = Unit(c);
			Node2D stage = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_Intro")
			};
			c.FrontParent.AddChild((Node)(object)stage, false, (InternalMode)0);
			if (!((Node)stage).IsInsideTree())
			{
				((Node)stage).QueueFree();
				return null;
			}
			stage.GlobalPosition = ((spot == IntroSpot.OnBody) ? c.Chest : (c.Chest + new Vector2((c.Body.X * 0.3f + Tuning.F("intro.reach", 92f) * num) * c.Facing, (0f - c.Body.Y) * 0.1f)));
			stage.Scale = new Vector2(num * c.Facing, num);
			((CanvasItem)stage).Modulate = new Color(1f, 1f, 1f, 0f);
			FxKit.FreeAfter((Node)(object)stage, Mathf.Max(0.34f, life) + 0.1f);
			Tween val = ((Node)stage).CreateTween();
			val.TweenProperty((GodotObject)(object)stage, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.07);
			val.TweenInterval((double)Mathf.Max(0.05f, life - 0.22f - 0.07f));
			val.TweenProperty((GodotObject)(object)stage, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.2199999988079071);
			val.TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)stage))
				{
					((Node)stage).QueueFree();
				}
			})));
			return stage;
		}

		public static void Run(Node owner, float delay, float time, Action<float> step, TransitionType trans = (TransitionType)0L, EaseType ease = (EaseType)2L)
		{
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)owner) || step == null)
			{
				return;
			}
			if (!owner.IsInsideTree())
			{
				step(1f);
				return;
			}
			owner.CreateTween().TweenMethod(Callable.From<float>((Action<float>)((float v) =>
			{
				if (FxKit.Alive((GodotObject)(object)owner))
				{
					step(v);
				}
			})), Variant.op_Implicit(0f), Variant.op_Implicit(1f), (double)Mathf.Max(0.01f, time)).SetDelay((double)Mathf.Max(0f, delay))
				.SetTrans(trans)
				.SetEase(ease);
		}

		public static void At(Node owner, float delay, Action action)
		{
			FxKit.After(owner, delay, action);
		}

		public static void Pop(Node2D node, float delay, float time = 0.16f, float to = 1f)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			node.Scale = Vector2.Zero;
			Run((Node)(object)node, delay, time, (float v) =>
			{
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				node.Scale = new Vector2(to * v, to * v);
			}, (TransitionType)10, (EaseType)1);
		}

		public static float BackOut(float t)
		{
			t = Mathf.Clamp(t, 0f, 1f) - 1f;
			return 1f + 2.70158f * t * t * t + 1.70158f * t * t;
		}

		public static Sprite2D Puff(Node2D parent, Vector2 from, Vector2 to, Color color, float delay, float time, float fromSize, float toSize, float sway = 8f)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Expected Obj, but got Unknown
			Sprite2D puff = new Sprite2D
			{
				Texture = FxKit.SoftDot,
				Modulate = new Color(color.R, color.G, color.B, 0f),
				Position = from,
				Scale = new Vector2(fromSize, fromSize) / 64f
			};
			((Node)parent).AddChild((Node)(object)puff, false, (InternalMode)0);
			float phase = from.X * 0.37f + to.Y * 0.11f + toSize;
			Run((Node)(object)puff, delay, time, (float v) =>
			{
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0052: Unknown result type (might be due to invalid IL or missing references)
				//IL_0077: Unknown result type (might be due to invalid IL or missing references)
				//IL_0081: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
				float num = 1f - (1f - v) * (1f - v);
				((Node2D)puff).Position = from.Lerp(to, num) + new Vector2(Mathf.Sin(v * 6f + phase) * sway, 0f);
				float num2 = Mathf.Lerp(fromSize, toSize, num);
				((Node2D)puff).Scale = new Vector2(num2, num2) / 64f;
				float num3 = ((v < 0.15f) ? (v / 0.15f) : (1f - (v - 0.15f) / 0.85f));
				((CanvasItem)puff).Modulate = new Color(color.R, color.G, color.B, color.A * num3);
			}, (TransitionType)0, (EaseType)2);
			return puff;
		}

		public static void Flash(Node2D stage, Vector2 local, Color color, float size, float grow = 2f, double time = 0.25)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			FxKit.Flash((Node)(object)stage, stage.ToGlobal(local), color, size, grow, time);
		}

		public static void Sparks(Node2D stage, Vector2 local, Color color, int count, float velMin, float velMax, double life, Vector2? dir = null, float spread = 180f, float gravity = 420f)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			FxKit.Burst((Node)(object)stage, stage.ToGlobal(local), color, count, velMin, velMax, life, 0.05f, 0.12f, new Vector2(0f, gravity), spread, dir, 4f);
		}

		public static Vector2[] Partial(Vector2[] path, float fraction)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			if (path == null || path.Length < 2)
			{
				return path ?? Array.Empty<Vector2>();
			}
			float num = 0f;
			for (int i = 1; i < path.Length; i++)
			{
				num += path[i - 1].DistanceTo(path[i]);
			}
			float num2 = num * Mathf.Clamp(fraction, 0f, 1f);
			List<Vector2> list = new List<Vector2> { path[0] };
			for (int j = 1; j < path.Length; j++)
			{
				if (!(num2 > 0f))
				{
					break;
				}
				float num3 = path[j - 1].DistanceTo(path[j]);
				if (num3 <= num2)
				{
					list.Add(path[j]);
					num2 -= num3;
				}
				else
				{
					list.Add(path[j - 1].Lerp(path[j], (num3 > 0.001f) ? (num2 / num3) : 1f));
					num2 = 0f;
				}
			}
			if (list.Count < 2)
			{
				list.Add(path[0]);
			}
			return list.ToArray();
		}
	}
