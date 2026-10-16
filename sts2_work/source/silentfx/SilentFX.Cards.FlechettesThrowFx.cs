// namespace: SilentFX.Cards
// type: FlechettesThrowFx (class)
// source lines: 60068-60174
	public sealed class FlechettesThrowFx : CardFx
	{
		private static readonly List<HandCards.Entry> Cards = new List<HandCards.Entry>();

		public override void OnPlay(CardFxContext c)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Invalid comparison between Unknown and I4
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Tally.Begin(c.Card, c.WeaponHand);
				if (c.Remote || c.Held is FlechettesOrbit)
				{
					return;
				}
				try
				{
					HandCards.Fill(Cards);
					foreach (HandCards.Entry card in Cards)
					{
						if (card.Card != null && card.Card != c.Card && (int)card.Card.Type == 2)
						{
							Pulse(card.Node, Brush.Steel, 0.3f);
						}
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("FlechettesThrowFx.OnPlay: " + ex.Message);
				}
				Cards.Clear();
			}
			catch (Exception ex2)
			{
				DevLog.Warn("FlechettesThrowFx.OnPlay: " + ex2);
			}
		}

		public override void OnHit(CardFxContext c, Vector2 at, bool killed)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (HeldFxManager.PlayingFor(c.Card) is FlechettesOrbit flechettesOrbit && flechettesOrbit.Shoot(at))
				{
					return;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("FlechettesThrowFx.OnHit: " + ex.Message);
			}
			Node parent = c.FrontParent;
			if (FxKit.Alive((GodotObject)(object)parent))
			{
				int num = Tally.Hit(c.Card, at, c.WeaponHand, out var _);
				float k = Strokes.Scale(c);
				Vector2 spot = at + new Vector2((FxKit.Hash(num * 5 + 1) - 0.5f) * 60f * k, (FxKit.Hash(num * 9 + 2) - 0.5f) * 80f * k);
				Brush.Dart(parent, c.WeaponHand, spot, 150f, 9f, Brush.Steel, 0.06f, 0f, () =>
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0020: Unknown result type (might be due to invalid IL or missing references)
					FxKit.Flash(parent, spot, new Color(0.9f, 0.96f, 1f, 0.85f), 34f * k, 1.7f, 0.12);
				});
			}
		}

		private static void Pulse(NCard node, Color color, float time)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)node) || !((Node)node).IsInsideTree())
			{
				return;
			}
			Sprite2D glow = FxKit.Glow(new Color(color.R, color.G, color.B, 0f), new Vector2(430f, 560f));
			((Node)glow).Name = StringName.op_Implicit("SilentFX_FlechPulse");
			((Node)node).AddChild((Node)(object)glow, false, (InternalMode)0);
			Tween val = ((Node)glow).CreateTween();
			val.TweenProperty((GodotObject)(object)glow, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0.75f), 0.05);
			val.TweenProperty((GodotObject)(object)glow, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), (double)time);
			val.TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)glow))
				{
					((Node)glow).QueueFree();
				}
			})));
		}
	}
