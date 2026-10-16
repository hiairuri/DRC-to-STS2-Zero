// namespace: SilentFX.Cards
// type: BulletTimeFx (class)
// source lines: 48864-49075
	public sealed class BulletTimeFx : CardFx
	{
		internal static readonly Color Ice = new Color(0.74f, 0.9f, 1f, 1f);

		internal static Node2D MakeClock(float size, out ShaderMaterial mat, out Line2D minute, out Line2D hour)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			Node2D val = FxKit.Sigil(size, Ice, out mat);
			if (mat != null)
			{
				mat.SetShaderParameter(StringName.op_Implicit("radius"), Variant.op_Implicit(0.8f));
				mat.SetShaderParameter(StringName.op_Implicit("width"), Variant.op_Implicit(0.018f));
				mat.SetShaderParameter(StringName.op_Implicit("ticks"), Variant.op_Implicit(12f));
				mat.SetShaderParameter(StringName.op_Implicit("tick_len"), Variant.op_Implicit(0.12f));
				mat.SetShaderParameter(StringName.op_Implicit("tick_width"), Variant.op_Implicit(0.016f));
				mat.SetShaderParameter(StringName.op_Implicit("inner"), Variant.op_Implicit(0.06f));
				mat.SetShaderParameter(StringName.op_Implicit("inner_width"), Variant.op_Implicit(0.03f));
				mat.SetShaderParameter(StringName.op_Implicit("glow"), Variant.op_Implicit(0.3f));
				mat.SetShaderParameter(StringName.op_Implicit("fill"), Variant.op_Implicit(0.04f));
				mat.SetShaderParameter(StringName.op_Implicit("alpha"), Variant.op_Implicit(0f));
			}
			minute = Hand(size * 0.33f, 4f);
			hour = Hand(size * 0.21f, 6.5f);
			((Node)val).AddChild((Node)(object)minute, false, (InternalMode)0);
			((Node)val).AddChild((Node)(object)hour, false, (InternalMode)0);
			return val;
		}

		public override void OnPlay(CardFxContext c)
		{
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node frontParent = c.FrontParent;
				if (!FxKit.Alive((GodotObject)(object)frontParent))
				{
					return;
				}
				if (!(c.Held is BulletTimeHeld))
				{
					float size = Tuning.F("bullet.clock_size", 330f);
					Node2D clock = MakeClock(size, out var cm, out var minute, out var hour);
					frontParent.AddChild((Node)(object)clock, false, (InternalMode)0);
					FxKit.FreeAfter((Node)(object)clock, Mathf.Max(0f, Tuning.F("bullet.clock_time", 1.5f)) + 0.1f);
					clock.GlobalPosition = c.Chest;
					FxKit.Drive((Node)(object)clock, Tuning.F("bullet.clock_time", 1.5f), (float t) =>
					{
						//IL_00af: Unknown result type (might be due to invalid IL or missing references)
						//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
						//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
						if (FxKit.Alive((GodotObject)(object)clock))
						{
							float num = Mathf.Min(1f, t / 0.08f);
							float num2 = 1f - Mathf.SmoothStep(0.72f, 1f, t);
							float num3 = num * num2;
							float num4 = Mathf.Min(1f, t / 0.6f);
							float num5 = 1f - Mathf.Pow(1f - num4, 3f);
							((Node2D)minute).Rotation = num5 * ((float)Math.PI * 2f) * 2.25f;
							((Node2D)hour).Rotation = num5 * ((float)Math.PI * 2f) * 0.6f - 1f;
							((CanvasItem)minute).Modulate = new Color(1f, 1f, 1f, num3);
							((CanvasItem)hour).Modulate = new Color(1f, 1f, 1f, num3);
							if (FxKit.Alive((GodotObject)(object)cm))
							{
								cm.SetShaderParameter(StringName.op_Implicit("alpha"), Variant.op_Implicit(0.9f * num3));
							}
						}
					}, (TransitionType)0, (EaseType)2).TweenCallback(Callable.From((Action)(() =>
					{
						if (FxKit.Alive((GodotObject)(object)clock))
						{
							((Node)clock).QueueFree();
						}
					})));
				}
				Ring(frontParent, c.Chest, 0f, 0f, 0.9f, 7f);
				Ring(frontParent, c.Chest, 0.16f, 60f, 1.1f, 5f);
				FxKit.Burst(frontParent, c.Chest, new Color(0.8f, 0.92f, 1f, 0.9f), Tuning.I("bullet.dust", 46), 380f, 900f, 1.7, 0.08f, 0.22f, Vector2.Zero, 180f, null, 30f, 1100f);
				if (!c.Remote && TimeFreeze.Begin(c))
				{
					return;
				}
				ColorRect veil = FxKit.Veil(new Color(0.3f, 0.52f, 0.95f, 0f));
				frontParent.AddChild((Node)(object)veil, false, (InternalMode)0);
				FxKit.FreeAfter((Node)(object)veil, Mathf.Max(0f, Tuning.F("bullet.hold", 1f)) + 0.82f);
				Tween val = ((Node)veil).CreateTween();
				val.TweenProperty((GodotObject)(object)veil, NodePath.op_Implicit("color:a"), Variant.op_Implicit(Tuning.F("bullet.tint", 0.18f)), 0.12);
				val.TweenInterval((double)Tuning.F("bullet.hold", 1f));
				val.TweenProperty((GodotObject)(object)veil, NodePath.op_Implicit("color:a"), Variant.op_Implicit(0f), 0.6);
				val.TweenCallback(Callable.From((Action)(() =>
				{
					if (FxKit.Alive((GodotObject)(object)veil))
					{
						((Node)veil).QueueFree();
					}
				})));
			}
			catch (Exception ex)
			{
				DevLog.Warn("BulletTimeFx.OnPlay: " + ex);
			}
		}

		private static Line2D Hand(float length, float width)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Expected Obj, but got Unknown
			Line2D val = new Line2D
			{
				Width = width,
				DefaultColor = new Color(Ice.R, Ice.G, Ice.B, 0.95f),
				BeginCapMode = (LineCapMode)2,
				EndCapMode = (LineCapMode)2,
				Material = (Material)(object)FxKit.Additive,
				Antialiased = true
			};
			val.AddPoint(Vector2.Zero, -1);
			val.AddPoint(new Vector2(0f, 0f - length), -1);
			return val;
		}

		private static void Ring(Node parent, Vector2 at, float delay, float ticks, float time, float toScale)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			Node2D ring = FxKit.Sigil(300f, Ice, out var mat);
			parent.AddChild((Node)(object)ring, false, (InternalMode)0);
			FxKit.FreeAfter((Node)(object)ring, Mathf.Max(0f, delay) + Mathf.Max(0f, time) + 0.1f);
			ring.GlobalPosition = at;
			if (mat != null)
			{
				mat.SetShaderParameter(StringName.op_Implicit("radius"), Variant.op_Implicit(0.84f));
				mat.SetShaderParameter(StringName.op_Implicit("width"), Variant.op_Implicit(0.022f));
				mat.SetShaderParameter(StringName.op_Implicit("ticks"), Variant.op_Implicit(ticks));
				mat.SetShaderParameter(StringName.op_Implicit("tick_len"), Variant.op_Implicit(0.05f));
				mat.SetShaderParameter(StringName.op_Implicit("tick_width"), Variant.op_Implicit(0.006f));
				mat.SetShaderParameter(StringName.op_Implicit("glow"), Variant.op_Implicit(0.35f));
				mat.SetShaderParameter(StringName.op_Implicit("alpha"), Variant.op_Implicit(0f));
			}
			Action<float> action = (float t) =>
			{
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)ring))
				{
					float num = 0.25f + toScale * t;
					ring.Scale = new Vector2(num, num);
					if (FxKit.Alive((GodotObject)(object)mat))
					{
						mat.SetShaderParameter(StringName.op_Implicit("alpha"), Variant.op_Implicit(0.8f * Mathf.Pow(1f - t, 1.5f)));
					}
				}
			};
			Tween val = ((Node)ring).CreateTween();
			val.TweenMethod(Callable.From<float>(action), Variant.op_Implicit(0f), Variant.op_Implicit(1f), (double)time).SetDelay((double)delay).SetTrans((TransitionType)4)
				.SetEase((EaseType)1);
			val.TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)ring))
				{
					((Node)ring).QueueFree();
				}
			})));
		}

		static BulletTimeFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		}
	}
