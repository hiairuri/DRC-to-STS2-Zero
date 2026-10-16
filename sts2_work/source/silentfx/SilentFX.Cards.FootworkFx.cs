// namespace: SilentFX.Cards
// type: FootworkFx (class)
// source lines: 47699-47913
	public sealed class FootworkFx : CardFx
	{
		private static readonly Color Step = new Color(0.74f, 1f, 0.84f, 0.85f);

		public override void OnPlay(CardFxContext c)
		{
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node parent = c.FrontParent;
				if (FxKit.Alive((GodotObject)(object)parent))
				{
					float facing = c.Facing;
					float num = Mathf.Max(0.03f, Tuning.F("footwork.step", 0.085f));
					float num2 = c.Body.X * Tuning.F("footwork.print", 0.42f);
					Vector2 val = c.Feet + new Vector2(0f, -4f);
					Node parent2 = (FxKit.Alive((GodotObject)(object)c.BackParent) ? c.BackParent : parent);
					Print(parent2, val + new Vector2((0f - facing) * c.Body.X * 0.44f, 5f), facing, -0.2f * facing, num2, 0f);
					Print(parent2, val + new Vector2(facing * c.Body.X * 0.42f, -7f), facing, 0.16f * facing, num2, num);
					Print(parent2, val + new Vector2((0f - facing) * c.Body.X * 0.04f, 1f), facing, 0f, num2 * 1.1f, num * 2f);
					Sway(parent, c.Feet, c.Body, facing, num * 2f + 0.16f);
					Vector2 chest = c.Chest;
					float around = c.Body.X * 0.3f;
					FxKit.After(parent, num * 2f, () =>
					{
						//IL_0007: Unknown result type (might be due to invalid IL or missing references)
						//IL_0020: Unknown result type (might be due to invalid IL or missing references)
						Brush.Motes(parent, chest, new Color(0.8f, 1f, 0.88f, 0.9f), 8, around, 150f, 0.5);
					});
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("BlueSkills.OnPlay: " + ex);
			}
		}

		private static void Print(Node parent, Vector2 at, float toe, float turn, float size, float delay)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Expected Obj, but got Unknown
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Expected Obj, but got Unknown
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)parent))
			{
				return;
			}
			if (delay > 0.001f)
			{
				FxKit.After(parent, delay, () =>
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					Print(parent, at, toe, turn, size, 0f);
				});
				return;
			}
			Node2D ground = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_FootworkPrint")
			};
			parent.AddChild((Node)(object)ground, false, (InternalMode)0);
			if (!((Node)ground).IsInsideTree())
			{
				((Node)ground).QueueFree();
				return;
			}
			ground.GlobalPosition = at;
			ground.Scale = new Vector2(1f, 0.55f);
			((CanvasItem)ground).Modulate = new Color(1f, 1f, 1f, 0f);
			Node2D foot = new Node2D
			{
				Rotation = turn
			};
			((Node)ground).AddChild((Node)(object)foot, false, (InternalMode)0);
			Sprite2D val = FxKit.Glow(Step, new Vector2(size * 0.92f, size * 0.5f));
			((Node2D)val).Position = new Vector2(toe * size * 0.14f, 0f);
			Sprite2D val2 = FxKit.Glow(Step, new Vector2(size * 0.44f, size * 0.4f));
			((Node2D)val2).Position = new Vector2((0f - toe) * size * 0.4f, 0f);
			Sprite2D val3 = FxKit.Glow(new Color(1f, 1f, 1f, 0.9f), new Vector2(size * 0.6f, size * 0.2f));
			((Node2D)val3).Position = new Vector2(toe * size * 0.14f, 0f);
			((Node)foot).AddChild((Node)(object)val, false, (InternalMode)0);
			((Node)foot).AddChild((Node)(object)val2, false, (InternalMode)0);
			((Node)foot).AddChild((Node)(object)val3, false, (InternalMode)0);
			float fade = Mathf.Max(0.08f, Tuning.F("footwork.fade", 0.26f));
			float total = 0.1f + fade;
			FxKit.Drive((Node)(object)ground, total, (float u) =>
			{
				//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)ground) && FxKit.Alive((GodotObject)(object)foot))
				{
					float num = u * total;
					float num2 = Mathf.Clamp(num / 0.05f, 0f, 1f);
					float num3 = Mathf.Clamp((num - 0.05f) / 0.05f, 0f, 1f);
					float num4 = Mathf.Clamp((num - 0.05f - 0.05f) / fade, 0f, 1f);
					float num5 = ((num2 < 1f) ? Mathf.Lerp(1.75f, 0.88f, num2 * num2) : (Mathf.Lerp(0.88f, 1f, num3) + 0.1f * num4));
					foot.Scale = new Vector2(num5, num5);
					((CanvasItem)ground).Modulate = new Color(1f, 1f, 1f, Mathf.Min(1f, num2 * 2.5f) * (1f - num4) * (1f - num4));
				}
			}, (TransitionType)0, (EaseType)2);
			FxKit.FreeAfter((Node)(object)ground, (double)total + 0.1);
			Brush.Wave(parent, at, size * 1.9f, new Color(Step.R, Step.G, Step.B, 0.65f), 0.24f, 0.05f, 0.3f, 0.25f);
			Brush.Motes(parent, at, Step, 3, size * 0.2f, 110f, 0.35);
		}

		private static void Sway(Node parent, Vector2 feet, Vector2 body, float facing, float time)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected Obj, but got Unknown
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)parent))
			{
				return;
			}
			Node2D val = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_FootworkSway")
			};
			parent.AddChild((Node)(object)val, false, (InternalMode)0);
			if (!((Node)val).IsInsideTree())
			{
				((Node)val).QueueFree();
				return;
			}
			Sprite2D[] ghosts = new Sprite2D[3];
			for (int i = 0; i < 3; i++)
			{
				ghosts[i] = FxKit.Glow(new Color(Step.R, Step.G, Step.B, 0f), new Vector2(64f, 64f));
				((Node)val).AddChild((Node)(object)ghosts[i], false, (InternalMode)0);
			}
			Vector2 mid = feet + new Vector2(0f, (0f - body.Y) * 0.52f);
			float reach = body.X * Tuning.F("footwork.sway", 0.7f);
			float glow = Tuning.F("footwork.ghost", 0.3f);
			time = Mathf.Max(0.1f, time);
			FxKit.Drive((Node)(object)val, time, (float u) =>
			{
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_013b: Unknown result type (might be due to invalid IL or missing references)
				for (int j = 0; j < 3; j++)
				{
					Sprite2D val2 = ghosts[j];
					if (FxKit.Alive((GodotObject)(object)val2))
					{
						float num = Mathf.Clamp(u * 1.16f - 0.08f * (float)j, 0f, 1f);
						float num2 = Lean(num);
						float num3 = Mathf.Abs(num2 - Lean(Mathf.Max(0f, num - 0.05f)));
						((Node2D)val2).GlobalPosition = mid + new Vector2((0f - facing) * reach * num2, 0f);
						((Node2D)val2).Scale = new Vector2(body.X * (0.58f + 1.1f * num3), body.Y * 1.08f) / 64f;
						float num4 = Mathf.Min(1f, num * 10f) * (1f - num) * (1f - 0.28f * (float)j);
						((CanvasItem)val2).Modulate = new Color(Step.R, Step.G, Step.B, glow * num4 * Mathf.Min(1f, 0.25f + Mathf.Abs(num2) * 2.2f));
					}
				}
			}, (TransitionType)0, (EaseType)2);
			FxKit.FreeAfter((Node)(object)val, (double)time + 0.1);
		}

		private static float Lean(float u)
		{
			return Mathf.Sin(7.225663f * u) * (1f - u);
		}

		static FootworkFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		}
	}
