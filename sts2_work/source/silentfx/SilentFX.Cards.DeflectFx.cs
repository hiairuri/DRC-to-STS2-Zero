// namespace: SilentFX.Cards
// type: DeflectFx (class)
// source lines: 48112-48318
	public sealed class DeflectFx : CardFx
	{
		private static readonly Color Shot = new Color(1f, 0.86f, 0.62f, 0.95f);

		public override void OnPlay(CardFxContext c)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node frontParent = c.FrontParent;
				if (FxKit.Alive((GodotObject)(object)frontParent))
				{
					float facing = c.Facing;
					Vector2 a = c.Chest + new Vector2(facing * c.Body.X * 0.5f, (0f - c.Body.Y) * 0.52f);
					Vector2 val = c.Chest + new Vector2(facing * c.Body.X * 1.05f, 0f);
					Vector2 val2 = c.Chest + new Vector2(facing * c.Body.X * 0.5f, c.Body.Y * 0.52f);
					float num = Mathf.Max(0f, Tuning.F("deflect.gap", 0.08f));
					Brush.Arc(frontParent, a, val, val2, 24f, Brush.Steel, 0f, 0.05f, 0.11f + num, 0.2f);
					float num2 = c.Body.X * Tuning.F("deflect.reach", 2.6f);
					Vector2 val3 = FxKit.Bezier(a, val, val2, 0.3f);
					Vector2 val4 = new Vector2(0f - facing, -0.1f);
					Glance(frontParent, val3 - val4.Normalized() * num2, val3, c.Chest + new Vector2((0f - facing) * c.Body.X * 0.9f, (0f - c.Body.Y) * 1.75f), c.Body.X, 0.03f);
					Vector2 val5 = FxKit.Bezier(a, val, val2, 0.7f);
					val4 = new Vector2(0f - facing, 0.1f);
					Glance(frontParent, val5 - val4.Normalized() * num2, val5, new Vector2(c.Chest.X - facing * c.Body.X * 0.55f, c.Feet.Y + c.Body.Y * 0.4f), c.Body.X, 0.03f + num);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("BlueSkills.OnPlay: " + ex);
			}
		}

		private static void Glance(Node parent, Vector2 from, Vector2 hit, Vector2 wide, float unit, float delay)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			FxKit.After(parent, delay, () =>
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				Streak(parent, from, hit, unit, 11f, Shot, 0.1f, spent: false, () =>
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0020: Unknown result type (might be due to invalid IL or missing references)
					//IL_003e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0044: Unknown result type (might be due to invalid IL or missing references)
					//IL_0049: Unknown result type (might be due to invalid IL or missing references)
					//IL_004e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0070: Unknown result type (might be due to invalid IL or missing references)
					//IL_0067: Unknown result type (might be due to invalid IL or missing references)
					//IL_0075: Unknown result type (might be due to invalid IL or missing references)
					//IL_007d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0082: Unknown result type (might be due to invalid IL or missing references)
					//IL_008d: Unknown result type (might be due to invalid IL or missing references)
					//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
					//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
					//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
					FxKit.Flash(parent, hit, new Color(0.92f, 0.97f, 1f, 0.85f), 44f, 2f, 0.14);
					Vector2 val = wide - hit;
					Vector2 value = (Vector2)((val.LengthSquared() > 0.01f) ? val.Normalized() : new Vector2(0f, -1f));
					Brush.Sparks(parent, hit, Brush.Steel, 5, 420f, value, 26f);
					Streak(parent, hit, wide, unit * 0.8f, 8f, new Color(Shot.R, Shot.G, Shot.B, 0.8f), 0.22f, spent: true, null);
				});
			});
		}

		private static void Streak(Node parent, Vector2 from, Vector2 to, float length, float thick, Color color, float time, bool spent, Action onArrive)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Expected Obj, but got Unknown
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)parent))
			{
				return;
			}
			Node2D root = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_DeflectShot")
			};
			parent.AddChild((Node)(object)root, false, (InternalMode)0);
			if (!((Node)root).IsInsideTree())
			{
				((Node)root).QueueFree();
				return;
			}
			Sprite2D outer = FxKit.Glow(new Color(color.R, color.G, color.B, 0f), new Vector2(64f, 64f));
			Sprite2D core = FxKit.Glow(new Color(1f, 1f, 1f, 0f), new Vector2(64f, 64f));
			((Node)root).AddChild((Node)(object)outer, false, (InternalMode)0);
			((Node)root).AddChild((Node)(object)core, false, (InternalMode)0);
			float distance = from.DistanceTo(to);
			Vector2 dir = (Vector2)((distance > 0.5f) ? ((to - from) / distance) : new Vector2(1f, 0f));
			root.GlobalPosition = from;
			root.GlobalRotation = dir.Angle();
			FxKit.Drive((Node)(object)root, Mathf.Max(0.02f, time), (float u) =>
			{
				//IL_0102: Unknown result type (might be due to invalid IL or missing references)
				//IL_0108: Unknown result type (might be due to invalid IL or missing references)
				//IL_0116: Unknown result type (might be due to invalid IL or missing references)
				//IL_011b: Unknown result type (might be due to invalid IL or missing references)
				//IL_013d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0147: Unknown result type (might be due to invalid IL or missing references)
				//IL_016f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0179: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)root) && FxKit.Alive((GodotObject)(object)outer) && FxKit.Alive((GodotObject)(object)core))
				{
					float num = (spent ? (1f - (1f - u) * (1f - u)) : (u * (0.5f + 0.5f * u)));
					float num2 = distance * num;
					float num3 = Mathf.Clamp(num2, 8f, Mathf.Max(8f, length)) * (spent ? (1f - 0.5f * u) : 1f);
					float num4 = thick * (spent ? (1f - 0.6f * u) : 1f);
					float num5 = (spent ? (Mathf.Min(1f, u * 12f) * (1f - u) * (1f - u)) : Mathf.Min(1f, u * 8f));
					root.GlobalPosition = from + dir * (num2 - num3 * 0.5f);
					((Node2D)outer).Scale = new Vector2(num3 * 1.3f, Mathf.Max(1f, num4)) / 64f;
					((Node2D)core).Scale = new Vector2(num3 * 1.12f, Mathf.Max(0.5f, num4 * 0.36f)) / 64f;
					((CanvasItem)outer).Modulate = new Color(color.R, color.G, color.B, color.A * num5);
					((CanvasItem)core).Modulate = new Color(1f, 1f, 1f, Mathf.Min(1f, color.A * 1.1f) * num5);
				}
			}, (TransitionType)0, (EaseType)2).TweenCallback(Callable.From((Action)(() =>
			{
				try
				{
					onArrive?.Invoke();
				}
				catch (Exception ex)
				{
					DevLog.Warn("Deflect shot failed: " + ex.Message);
				}
				if (FxKit.Alive((GodotObject)(object)root))
				{
					((Node)root).QueueFree();
				}
			})));
		}

		static DeflectFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		}
	}
