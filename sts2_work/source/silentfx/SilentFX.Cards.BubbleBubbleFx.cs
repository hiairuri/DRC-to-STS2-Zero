// namespace: SilentFX.Cards
// type: BubbleBubbleFx (class)
// source lines: 48692-48863
	public sealed class BubbleBubbleFx : CardFx
	{
		public override void OnPlay(CardFxContext c)
		{
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Expected Obj, but got Unknown
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_039a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node frontParent = c.FrontParent;
				if (!c.HasTarget || !FxKit.Alive((GodotObject)(object)frontParent))
				{
					return;
				}
				bool flag = c.TargetPoison > 0;
				if (flag)
				{
					Sfx.Play("bubble_boil");
				}
				float num = Mathf.Max(c.TargetWidth, 130f);
				float num2 = ((c.TargetWidth > c.TargetHeight * 1.35f) ? Mathf.Max(1f, c.TargetHeight) : Mathf.Max(120f, Mathf.Abs(c.TargetFeet.Y - c.TargetChest.Y) * 2f));
				Vector2 targetFeet = c.TargetFeet;
				Color venom = PoisonFx.Venom;
				if (FxKit.Alive((GodotObject)(object)c.BackParent))
				{
					Sprite2D pool = new Sprite2D
					{
						Texture = FxKit.SoftDot,
						Modulate = new Color(0.07f, 0.3f, 0.1f, 0f),
						Scale = new Vector2(num * 1.7f, num * 0.46f) / 64f
					};
					c.BackParent.AddChild((Node)(object)pool, false, (InternalMode)0);
					FxKit.FreeAfter((Node)(object)pool, (flag ? 0.75f : 0.3f) + 0.67f);
					((Node2D)pool).GlobalPosition = targetFeet;
					Tween val = ((Node)pool).CreateTween();
					val.TweenProperty((GodotObject)(object)pool, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(flag ? 0.85f : 0.4f), 0.12);
					val.TweenInterval(flag ? 0.75 : 0.3);
					val.TweenProperty((GodotObject)(object)pool, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.45);
					val.TweenCallback(Callable.From((Action)(() =>
					{
						if (FxKit.Alive((GodotObject)(object)pool))
						{
							((Node)pool).QueueFree();
						}
					})));
					FxKit.RingPulse(c.BackParent, targetFeet, num * 1.8f, venom, 0f, 0.5f, 0.5f, 1.05f, 0.28f, 0f, 0.03f, flag ? 0.9f : 0.4f);
				}
				FxKit.Bubbles(frontParent, targetFeet + new Vector2(0f, -10f), new Color(venom.R, venom.G, venom.B, 0.9f), flag ? Tuning.I("bubble.stream", 36) : 8, new Vector2(num * 0.42f, 6f), flag ? 120f : 60f, flag ? 340f : 150f, 1.0, 0.12f, flag ? 0.4f : 0.24f, oneShot: true, 0.25f);
				if (flag)
				{
					int num3 = Tuning.I("bubble.big", 7);
					for (int num4 = 0; num4 < num3; num4++)
					{
						float num5 = (FxKit.Hash(num4 * 7 + 3) - 0.5f) * num * 0.8f;
						float rise = num2 * (0.3f + 0.7f * FxKit.Hash(num4 * 5 + 11));
						float size = 34f + 44f * FxKit.Hash(num4 * 3 + 1);
						float delay = 0.05f + (float)num4 * 0.085f;
						float time = 0.32f + 0.3f * FxKit.Hash(num4 + 21);
						Color col = ((num4 % 3 == 0) ? PoisonFx.Acid : venom);
						BigBubble(frontParent, targetFeet + new Vector2(num5, -8f), rise, size, delay, time, col);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("BubbleBubbleFx.OnPlay: " + ex);
			}
		}

		private static void BigBubble(Node parent, Vector2 start, float rise, float size, float delay, float time, Color col)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Expected Obj, but got Unknown
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			Sprite2D bubble = new Sprite2D
			{
				Texture = FxKit.Bubble,
				Material = (Material)(object)FxKit.Additive,
				Modulate = new Color(col.R, col.G, col.B, 0f),
				Scale = new Vector2(size, size) * 0.35f / 64f
			};
			parent.AddChild((Node)(object)bubble, false, (InternalMode)0);
			FxKit.FreeAfter((Node)(object)bubble, delay + time + 0.25f);
			((Node2D)bubble).GlobalPosition = start;
			Vector2 top = start + new Vector2((FxKit.Hash((int)size) - 0.5f) * 40f, 0f - rise);
			Tween val = ((Node)bubble).CreateTween();
			val.TweenInterval((double)delay);
			val.TweenProperty((GodotObject)(object)bubble, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0.95f), 0.05);
			val.SetParallel(true);
			val.TweenProperty((GodotObject)(object)bubble, NodePath.op_Implicit("global_position"), Variant.op_Implicit(top), (double)time).SetTrans((TransitionType)4).SetEase((EaseType)1);
			val.TweenProperty((GodotObject)(object)bubble, NodePath.op_Implicit("scale"), Variant.op_Implicit(new Vector2(size, size) / 64f), (double)time);
			val.SetParallel(false);
			val.TweenCallback(Callable.From((Action)(() =>
			{
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_006d: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)parent))
				{
					FxKit.Burst(parent, top, new Color(col.R, col.G, col.B, 0.95f), 7, 90f, 220f, 0.4, 0.07f, 0.16f, new Vector2(0f, 420f), 180f, null, size * 0.3f);
				}
			})));
			val.SetParallel(true);
			val.TweenProperty((GodotObject)(object)bubble, NodePath.op_Implicit("scale"), Variant.op_Implicit(new Vector2(size, size) * 1.6f / 64f), 0.1);
			val.TweenProperty((GodotObject)(object)bubble, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.1);
			val.SetParallel(false);
			val.TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)bubble))
				{
					((Node)bubble).QueueFree();
				}
			})));
		}
	}
