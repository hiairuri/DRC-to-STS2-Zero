// namespace: SilentFX.Cards
// type: PounceFx (class)
// source lines: 84553-84951
	public sealed class PounceFx : CardFx
	{
		private sealed class Mark
		{
			public Sprite2D Wash;

			public Sprite2D Point;

			public Node2D Ring;

			public ShaderMaterial RingMat;
		}

		private static readonly StringName PAlpha = StringName.op_Implicit("alpha");

		private const string Key = "pounce.free";

		private const float HalfW = 150f;

		private const float HalfH = 211f;

		private const float RingSize = 88f;

		private static readonly Vector2 CostAt = new Vector2(-124f, -184f);

		private static readonly Color Bright = new Color(1f, 0.86f, 0.55f, 1f);

		private static readonly List<HandCards.Entry> Cards = new List<HandCards.Entry>();

		public static void Reset()
		{
			try
			{
				CardDeco.RemoveAll("pounce.free", 0f);
			}
			catch (Exception ex)
			{
				DevLog.Warn("PounceFx.Reset: " + ex.Message);
			}
			Cards.Clear();
		}

		public override void OnPlay(CardFxContext c)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node frontParent = c.FrontParent;
				if (c.HasTarget && FxKit.Alive((GodotObject)(object)frontParent))
				{
					Vector2 from = c.Chest;
					Vector2 to = c.TargetChest + new Vector2(0f, (0f - c.TargetHeight) * 0.2f);
					float num = Mathf.Max(170f, c.TargetHeight * 0.9f);
					if (c.TargetWidth > c.TargetHeight * 1.35f)
					{
						num = Mathf.Max(num, from.DistanceTo(to) * 0.34f);
					}
					Vector2 ctrl = (from + to) * 0.5f + new Vector2(0f, 0f - num);
					Brush.Follow(frontParent, (float t) =>
					{
						//IL_0001: Unknown result type (might be due to invalid IL or missing references)
						//IL_0007: Unknown result type (might be due to invalid IL or missing references)
						//IL_000d: Unknown result type (might be due to invalid IL or missing references)
						//IL_0013: Unknown result type (might be due to invalid IL or missing references)
						return FxKit.Bezier(from, ctrl, to, t);
					}, 0.34f, 24f, Brush.Amber, 0.15f);
					Brush.Dust(frontParent, c.Feet, 0f - c.TargetSide, 7);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("PounceFx.OnPlay: " + ex);
			}
		}

		public override void OnHit(CardFxContext c, Vector2 at, bool killed)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node frontParent = c.FrontParent;
				if (FxKit.Alive((GodotObject)(object)frontParent))
				{
					float num = Strokes.Scale(c);
					float targetSide = c.TargetSide;
					Vector2 val = at + new Vector2((0f - targetSide) * 70f * num, (0f - c.TargetHeight) * 0.55f);
					Vector2 to = at + new Vector2(targetSide * 40f * num, c.TargetHeight * 0.22f);
					Brush.Cut(frontParent, val, to, 34f * num, Brush.Amber, 0f, 0.035f, 0.03f, 0.18f);
					FxKit.Flash(frontParent, at, new Color(1f, 0.8f, 0.45f, 0.75f), 120f * num, 1.5f, 0.16);
					Brush.Dust(frontParent, c.TargetFeet, 0f, 9, 1.1f);
					PointAtSkills(c, at);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("PounceFx.OnHit: " + ex);
			}
		}

		private static void PointAtSkills(CardFxContext c, Vector2 from)
		{
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Invalid comparison between Unknown and I4
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (c.Remote || Tuning.F("pounce.point", 1f) < 0.5f)
				{
					return;
				}
				Node val = (FxKit.Alive((GodotObject)(object)c.UiParent) ? c.UiParent : GameRefs.UiLayer);
				if (!FxKit.Alive((GodotObject)(object)val) || !val.IsInsideTree())
				{
					return;
				}
				HandCards.Fill(Cards);
				int num = 0;
				foreach (HandCards.Entry card2 in Cards)
				{
					CardModel card = card2.Card;
					NCard node = card2.Node;
					if (card != null && card != c.Card && (int)card.Type == 2 && FxKit.Alive((GodotObject)(object)node) && ((Node)node).IsInsideTree())
					{
						Send(val, from, card, node, 0.03f + 0.04f * (float)num);
						num++;
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("PounceFx.PointAtSkills: " + ex.Message);
			}
			Cards.Clear();
		}

		private static void Send(Node layer, Vector2 from, CardModel card, NCard node, float delay)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			Vector2 aim = Aim(node, from);
			Vector2 ctrl = new Vector2((from.X + aim.X) * 0.5f, Mathf.Min(from.Y, aim.Y) - 140f);
			float time = Mathf.Clamp(Tuning.F("pounce.point_time", 0.24f), 0.08f, 0.6f);
			Brush.Follow(layer, (float t) =>
			{
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_004b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)node) && ((Node)node).IsInsideTree() && node.Model == card)
				{
					aim = Aim(node, aim);
				}
				return FxKit.Bezier(from, ctrl, aim, t);
			}, 0.28f, 15f, Brush.Amber, time, delay, () =>
			{
				Land(card);
			});
		}

		private static Vector2 Aim(NCard node, Vector2 fallback)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			CardStandIn.Pose(node, out var centre, out var rotation, out var scale);
			if (centre == Vector2.Zero)
			{
				return fallback;
			}
			Vector2 val = centre;
			Vector2 val2 = CostAt * scale;
			return val + val2.Rotated(rotation);
		}

		private static void Land(CardModel card)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Invalid comparison between Unknown and I4
			try
			{
				if (card != null)
				{
					CardPile pile = card.Pile;
					if (pile != null && (int)pile.Type == 2)
					{
						CardDeco.Attach(card, "pounce.free", Build, Tick);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("PounceFx.Land: " + ex.Message);
			}
		}

		private static void Build(CardDeco.Handle h)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (h != null && FxKit.Alive((GodotObject)(object)h.Root))
				{
					Vector2 position = CostOf(h);
					Mark mark = new Mark
					{
						Wash = FxKit.Glow(new Color(Brush.Amber.R, Brush.Amber.G, Brush.Amber.B, 0f), new Vector2(430f, 560f)),
						Point = FxKit.Glow(new Color(Bright.R, Bright.G, Bright.B, 0f), new Vector2(64f, 64f))
					};
					((Node)h.Root).AddChild((Node)(object)mark.Wash, false, (InternalMode)0);
					((Node2D)mark.Point).Position = position;
					((Node)h.Root).AddChild((Node)(object)mark.Point, false, (InternalMode)0);
					mark.Ring = FxKit.Sigil(88f, new Color(1f, 0.74f, 0.34f, 1f), out var mat);
					mark.RingMat = mat;
					mark.Ring.Position = position;
					((Node)h.Root).AddChild((Node)(object)mark.Ring, false, (InternalMode)0);
					if (mat != null)
					{
						mat.SetShaderParameter(StringName.op_Implicit("radius"), Variant.op_Implicit(0.82f));
						mat.SetShaderParameter(StringName.op_Implicit("width"), Variant.op_Implicit(0.075f));
						mat.SetShaderParameter(StringName.op_Implicit("dashes"), Variant.op_Implicit(0f));
						mat.SetShaderParameter(StringName.op_Implicit("glow"), Variant.op_Implicit(0.4f));
						mat.SetShaderParameter(PAlpha, Variant.op_Implicit(0f));
					}
					h.State = mark;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("PounceFx.Build: " + ex.Message);
			}
		}

		private static Vector2 CostOf(CardDeco.Handle h)
		{
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				NCard host = h.Host;
				if (FxKit.Alive((GodotObject)(object)host) && ((Node)host).IsInsideTree() && ((Node)h.Root).IsInsideTree())
				{
					Control nodeOrNull = ((Node)host).GetNodeOrNull<Control>(NodePath.op_Implicit("%EnergyIcon"));
					if (FxKit.Alive((GodotObject)(object)nodeOrNull) && ((Node)nodeOrNull).IsInsideTree() && ((CanvasItem)nodeOrNull).IsVisibleInTree())
					{
						Vector2 val = ((CanvasItem)nodeOrNull).GetGlobalTransform() * (nodeOrNull.Size * 0.5f);
						Transform2D globalTransform = ((CanvasItem)h.Root).GetGlobalTransform();
						Vector2 val2 = globalTransform.AffineInverse() * val;
						if (Mathf.Abs(val2.X) < 190f && Mathf.Abs(val2.Y) < 251f)
						{
							return val2;
						}
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("PounceFx.CostOf: " + ex.Message);
			}
			return CostAt;
		}

		private static void Tick(CardDeco.Handle h, float dt)
		{
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (!(h?.State is Mark mark))
				{
					h?.Remove(0f);
					return;
				}
				float num = Mathf.Clamp(Tuning.F("pounce.mark_time", 0.6f), 0.25f, 2f);
				float time = h.Time;
				if (time >= num)
				{
					h.Remove(0.12f);
					return;
				}
				float num2 = time / num;
				float num3 = Mathf.Clamp((num2 - 0.62f) / 0.38f, 0f, 1f);
				float num4 = Mathf.Min(1f, time / 0.05f) * (1f - num3 * num3);
				float num5 = Mathf.Clamp(time / 0.14f, 0f, 1f);
				float num6 = 1f - (1f - num5) * (1f - num5);
				float num7 = Mathf.Clamp((time - 0.14f) / 0.1f, 0f, 1f);
				float num8 = Mathf.Lerp(2.6f, 0.86f, num6) + 0.14f * num7;
				if (FxKit.Alive((GodotObject)(object)mark.Ring))
				{
					mark.Ring.Scale = new Vector2(num8, num8);
				}
				if (mark.RingMat != null && FxKit.Alive((GodotObject)(object)mark.RingMat))
				{
					mark.RingMat.SetShaderParameter(PAlpha, Variant.op_Implicit(0.95f * num4));
				}
				float num9 = Mathf.Clamp(1f - Mathf.Abs(time - 0.14f) / 0.14f, 0f, 1f);
				if (FxKit.Alive((GodotObject)(object)mark.Point))
				{
					float num10 = 70f + 70f * num9;
					((Node2D)mark.Point).Scale = new Vector2(num10, num10) / 64f;
					((CanvasItem)mark.Point).Modulate = new Color(Bright.R, Bright.G, Bright.B, 0.25f * num4 + 0.65f * num9);
				}
				if (FxKit.Alive((GodotObject)(object)mark.Wash))
				{
					float num11 = Mathf.Min(1f, time / 0.06f) * (1f - num2) * (1f - num2);
					((CanvasItem)mark.Wash).Modulate = new Color(Brush.Amber.R, Brush.Amber.G, Brush.Amber.B, 0.55f * num11);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("PounceFx.Tick: " + ex.Message);
				h?.Remove(0f);
			}
		}

		static PounceFx()
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		}
	}
