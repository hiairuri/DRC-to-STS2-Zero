// namespace: SilentFX.Cards
// type: PoisonedStabFx (class)
// source lines: 83588-83693
	public sealed class PoisonedStabFx : CardFx
	{
		public override void OnHit(CardFxContext c, Vector2 at, bool killed)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Expected Obj, but got Unknown
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node frontParent = c.FrontParent;
				if (!FxKit.Alive((GodotObject)(object)frontParent))
				{
					return;
				}
				Color venom = PoisonFx.Venom;
				Vector2 val = at - c.Chest;
				val = (Vector2)((val.LengthSquared() > 1f) ? val.Normalized() : new Vector2(c.TargetSide, 0f));
				float num = Tuning.F("stab.length", 150f) * Mathf.Max(1f, c.TargetWidth / 260f);
				Node2D holder = new Node2D
				{
					Name = StringName.op_Implicit("SilentFX_PoisonedStab")
				};
				frontParent.AddChild((Node)(object)holder, false, (InternalMode)0);
				FxKit.FreeAfter((Node)(object)holder, 0.3700000047683716);
				holder.GlobalPosition = at;
				Vector2 val2 = -val * num * 0.62f + new Vector2(0f, -10f);
				Vector2 to = val * num * 0.48f + new Vector2(0f, 8f);
				((Node)holder).AddChild((Node)(object)FxKit.Crescent(val2, to, 6f * c.TargetSide, 15f, new Color(venom.R, venom.G, venom.B, 0.85f)), false, (InternalMode)0);
				((Node)holder).AddChild((Node)(object)FxKit.Crescent(val2, to, 6f * c.TargetSide, 4f, new Color(0.92f, 1f, 0.86f, 0.95f)), false, (InternalMode)0);
				Tween val3 = ((Node)holder).CreateTween();
				val3.TweenInterval(0.05);
				val3.TweenProperty((GodotObject)(object)holder, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.22);
				val3.TweenCallback(Callable.From((Action)(() =>
				{
					if (FxKit.Alive((GodotObject)(object)holder))
					{
						((Node)holder).QueueFree();
					}
				})));
				FxKit.Burst(frontParent, at + val * 20f, new Color(venom.R, venom.G, venom.B, 0.95f), Tuning.I("stab.spray", 18), 200f, 520f, 0.6, 0.1f, 0.28f, new Vector2(0f, 760f), 34f, val + new Vector2(0f, -0.25f), 8f);
				FxKit.Burst(frontParent, at, new Color(venom.R * 0.8f, venom.G, venom.B * 0.8f, 0.9f), 7, 20f, 110f, 0.7, 0.09f, 0.18f, new Vector2(0f, 640f), 180f, null, 16f);
				FxKit.Flash(frontParent, at, new Color(venom.R, venom.G, venom.B, 0.7f), 80f, 2.6f, 0.28);
			}
			catch (Exception ex)
			{
				DevLog.Warn("PoisonedStabFx.OnHit: " + ex);
			}
		}
	}
