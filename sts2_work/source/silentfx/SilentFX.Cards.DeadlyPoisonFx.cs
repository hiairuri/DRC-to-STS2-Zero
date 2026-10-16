// namespace: SilentFX.Cards
// type: DeadlyPoisonFx (class)
// source lines: 49726-49870
	public sealed class DeadlyPoisonFx : CardFx
	{
		public override void OnPlay(CardFxContext c)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node frontParent = c.FrontParent;
				if (c.HasTarget && FxKit.Alive((GodotObject)(object)frontParent))
				{
					Vector2 val = ((c.Held is IHeldOrigin heldOrigin) ? heldOrigin.Origin : c.WeaponHand);
					Throw(frontParent, val, c.TargetChest, Tuning.F("deadly.time", 0.2f), 1f);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("DeadlyPoisonFx.OnPlay: " + ex);
			}
		}

		public static void Throw(Node parent, Vector2 from, Vector2 to, float time, float size)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Expected Obj, but got Unknown
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			Color venom = PoisonFx.Venom;
			Node2D holder = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_VenomBolt")
			};
			parent.AddChild((Node)(object)holder, false, (InternalMode)0);
			FxKit.FreeAfter((Node)(object)holder, Mathf.Max(0f, time) + 0.3f);
			Line2D trail = FxKit.Ribbon(14f * size, new Color(venom.R, venom.G, venom.B, 0.85f));
			((Node)holder).AddChild((Node)(object)trail, false, (InternalMode)0);
			Sprite2D head = FxKit.Glow(new Color(venom.R, venom.G, venom.B, 0.95f), new Vector2(54f, 54f) * size);
			((Node)holder).AddChild((Node)(object)head, false, (InternalMode)0);
			Sprite2D core = FxKit.Glow(new Color(0.9f, 1f, 0.8f, 0.9f), new Vector2(22f, 22f) * size);
			((Node)holder).AddChild((Node)(object)core, false, (InternalMode)0);
			((CanvasItem)head).Visible = false;
			((CanvasItem)core).Visible = false;
			Vector2 val = (from + to) * 0.5f;
			Vector2 val2 = to - from;
			Vector2 mid = val + new Vector2(0f, 0f - Mathf.Min(120f, val2.Length() * 0.16f));
			int drips = 0;
			Action<float> action = (float t) =>
			{
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0061: Unknown result type (might be due to invalid IL or missing references)
				//IL_006d: Unknown result type (might be due to invalid IL or missing references)
				//IL_007f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0080: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0134: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)holder) && FxKit.Alive((GodotObject)(object)head) && FxKit.Alive((GodotObject)(object)core))
				{
					Vector2 val4 = FxKit.Bezier(from, mid, to, t);
					Sprite2D val5 = head;
					bool visible = (((CanvasItem)core).Visible = true);
					((CanvasItem)val5).Visible = visible;
					((Node2D)head).GlobalPosition = val4;
					((Node2D)core).GlobalPosition = val4;
					trail.AddPoint(holder.ToLocal(val4), -1);
					if (t > 0.2f * (float)(drips + 1) && drips < 4 && FxKit.Alive((GodotObject)(object)parent))
					{
						drips++;
						FxKit.Burst(parent, val4, new Color(venom.R, venom.G, venom.B, 0.9f), 3, 20f, 80f, 0.5, 0.08f * size, 0.16f * size, new Vector2(0f, 700f), 180f, null, 6f);
					}
				}
			};
			Tween val3 = ((Node)holder).CreateTween();
			val3.TweenMethod(Callable.From<float>(action), Variant.op_Implicit(0f), Variant.op_Implicit(1f), (double)time).SetTrans((TransitionType)4).SetEase((EaseType)0);
			val3.TweenCallback(Callable.From((Action)(() =>
			{
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_0072: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
				//IL_011e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0149: Unknown result type (might be due to invalid IL or missing references)
				//IL_015b: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)head))
				{
					((CanvasItem)head).Visible = false;
				}
				if (FxKit.Alive((GodotObject)(object)core))
				{
					((CanvasItem)core).Visible = false;
				}
				if (FxKit.Alive((GodotObject)(object)parent))
				{
					FxKit.Burst(parent, to, new Color(venom.R, venom.G, venom.B, 0.95f), Tuning.I("deadly.splash", 18), 120f, 340f, 0.7, 0.12f * size, 0.32f * size, new Vector2(0f, 560f), 160f, new Vector2(0f, -1f), 12f);
					FxKit.Flash(parent, to, new Color(venom.R, venom.G, venom.B, 0.75f), 90f * size, 3f, 0.32);
					FxKit.RingPulse(parent, to, 220f * size, venom, 0f, 0.32f, 0.3f, 1f);
				}
			})));
			val3.TweenProperty((GodotObject)(object)holder, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.2);
			val3.TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)holder))
				{
					((Node)holder).QueueFree();
				}
			})));
		}
	}
