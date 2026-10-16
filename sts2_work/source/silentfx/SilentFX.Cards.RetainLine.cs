// namespace: SilentFX.Cards
// type: RetainLine (class)
// source lines: 88272-89128
	public sealed class RetainLine
	{
		private sealed class Tug
		{
			public CardModel Card;

			public NCard Node;

			public Node2D Behind;

			public Node2D Front;

			public readonly Line2D[] Halo = new Line2D[2];

			public readonly Line2D[] Core = new Line2D[2];

			public readonly Sprite2D[] Peg = new Sprite2D[2];

			public readonly Sprite2D[] Corner = new Sprite2D[2];

			public float T;

			public float Force = 1f;

			public float Again = 0.5f;

			public float AgainAt = 0.17f;

			public float Phase;

			public Vector2 Nudge;

			public float Turn;
		}

		private const float Side = 154f;

		private const float CornerX = 136f;

		private const float CornerY = 196f;

		private const float Life = 1.25f;

		private const float FadeOut = 0.14f;

		private readonly Color _color;

		private readonly Color _bright;

		private readonly Func<CardModel, bool> _holds;

		private readonly List<HandCards.Entry> _hand = new List<HandCards.Entry>();

		private readonly List<Tug> _tugs = new List<Tug>();

		private const float LineY = -150f;

		public bool Bound => _tugs.Count > 0;

		public RetainLine(Color color, Func<CardModel, bool> holds)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			_color = color;
			_bright = new Color(Mathf.Lerp(color.R, 1f, 0.4f), Mathf.Lerp(color.G, 1f, 0.4f), Mathf.Lerp(color.B, 1f, 0.4f), 1f);
			_holds = holds;
		}

		public static bool Holdable(CardModel card)
		{
			if (card == null)
			{
				return false;
			}
			try
			{
				if (card.ShouldRetainThisTurn || card.HasTurnEndInHandEffect)
				{
					return false;
				}
				return card.Keywords == null || !card.Keywords.Contains((CardKeyword)2);
			}
			catch
			{
				return false;
			}
		}

		private bool Holds(CardModel card)
		{
			try
			{
				return card != null && (_holds == null || _holds(card));
			}
			catch
			{
				return false;
			}
		}

		private static bool Seen(NCard node)
		{
			if (FxKit.Alive((GodotObject)(object)node) && ((Node)node).IsInsideTree())
			{
				return ((CanvasItem)node).IsVisibleInTree();
			}
			return false;
		}

		private static float BodyScale(NCard node)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)node.Body))
			{
				return 1f;
			}
			return node.Body.Scale.X;
		}

		private static Line2D MakeLine(float width, Color color, bool additive)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return Props.Line(width, color, additive);
		}

		private static float Chance(int index, int salt)
		{
			return FxKit.Hash(index * 7 + salt * 131 + 17);
		}

		public void Bind()
		{
			Release();
			HandCards.Fill(_hand);
			float num = Mathf.Max(0f, Tuning.F("plans.gap", 0.045f));
			int num2 = 0;
			foreach (HandCards.Entry item in _hand)
			{
				if (Holds(item.Card) && Seen(item.Node))
				{
					Tug tug = Make(item);
					if (tug != null)
					{
						float num3 = Mathf.Clamp(Tuning.F("plans.vary", 0.05f), 0f, 0.3f);
						tug.T = 0f - num * (float)num2;
						tug.Force = 1f + num3 * (Chance(num2, 2) * 2f - 1f);
						tug.Again = Mathf.Max(0f, Tuning.F("plans.again_force", 0.3f)) * (1f + 2f * num3 * (Chance(num2, 3) * 2f - 1f));
						tug.AgainAt = Tuning.F("plans.again", 0.17f) * (1f + num3 * (Chance(num2, 4) * 2f - 1f));
						tug.Phase = Chance(num2, 5) * ((float)Math.PI * 2f);
						num2++;
					}
				}
			}
		}

		public void Release()
		{
			foreach (Tug tug in _tugs)
			{
				Drop(tug);
			}
			_tugs.Clear();
		}

		public void Free()
		{
			Release();
		}

		private Tug Make(HandCards.Entry entry)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Expected Obj, but got Unknown
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Expected Obj, but got Unknown
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			Tug tug = new Tug
			{
				Card = entry.Card,
				Node = entry.Node
			};
			try
			{
				tug.Behind = new Node2D
				{
					Name = StringName.op_Implicit("SilentFX_RetainThreads"),
					ShowBehindParent = true,
					Visible = false
				};
				((Node)entry.Node).AddChild((Node)(object)tug.Behind, false, (InternalMode)0);
				tug.Front = new Node2D
				{
					Name = StringName.op_Implicit("SilentFX_RetainCorners"),
					Visible = false
				};
				((Node)entry.Node).AddChild((Node)(object)tug.Front, false, (InternalMode)0);
				_tugs.Add(tug);
				for (int i = 0; i < 2; i++)
				{
					tug.Halo[i] = MakeLine(10f, new Color(_color.R, _color.G, _color.B, 0.12f), additive: true);
					tug.Core[i] = MakeLine(3.2f, _bright, additive: true);
					tug.Peg[i] = FxKit.Glow(new Color(_color.R, _color.G, _color.B, 0f), new Vector2(44f, 44f));
					((Node)tug.Behind).AddChild((Node)(object)tug.Halo[i], false, (InternalMode)0);
					((Node)tug.Behind).AddChild((Node)(object)tug.Core[i], false, (InternalMode)0);
					((Node)tug.Behind).AddChild((Node)(object)tug.Peg[i], false, (InternalMode)0);
					tug.Corner[i] = FxKit.Glow(new Color(_color.R, _color.G, _color.B, 0f), new Vector2(96f, 96f));
					((Node2D)tug.Corner[i]).Position = new Vector2((i == 0) ? (-136f) : 136f, 196f);
					((Node)tug.Front).AddChild((Node)(object)tug.Corner[i], false, (InternalMode)0);
				}
				return tug;
			}
			catch (Exception ex)
			{
				DevLog.Warn("RetainLine (threads): " + ex.Message);
				Drop(tug);
				_tugs.Remove(tug);
				return null;
			}
		}

		private static void Settle(Tug tug)
		{
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)tug.Node) && tug.Node.Model == tug.Card)
			{
				if (tug.Nudge != Vector2.Zero)
				{
					NCard node = tug.Node;
					((Control)node).Position = ((Control)node).Position - tug.Nudge;
				}
				if (tug.Turn != 0f)
				{
					NCard node2 = tug.Node;
					((Control)node2).Rotation = ((Control)node2).Rotation - tug.Turn;
				}
			}
			tug.Nudge = Vector2.Zero;
			tug.Turn = 0f;
		}

		private static void Drop(Tug tug)
		{
			try
			{
				Settle(tug);
			}
			catch (Exception ex)
			{
				DevLog.Warn("RetainLine settle: " + ex);
			}
			try
			{
				if (FxKit.Alive((GodotObject)(object)tug.Behind))
				{
					((Node)tug.Behind).QueueFree();
				}
				if (FxKit.Alive((GodotObject)(object)tug.Front))
				{
					((Node)tug.Front).QueueFree();
				}
			}
			catch (Exception ex2)
			{
				DevLog.Warn("RetainLine cleanup: " + ex2);
			}
			tug.Behind = null;
			tug.Front = null;
		}

		private static float Kicked(float t, float stiff, float damp, float farthest)
		{
			if (t <= 0f)
			{
				return 0f;
			}
			float num = stiff * Mathf.Sqrt(1f - damp * damp);
			return Mathf.Exp((0f - damp) * stiff * t) * Mathf.Sin(num * t) / farthest;
		}

		public void Update(float dt)
		{
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			if (_tugs.Count == 0)
			{
				return;
			}
			float num = Mathf.Max(0f, Tuning.F("plans.lunge", 260f));
			float num2 = Mathf.Clamp(Tuning.F("plans.spring", 9.3f), 3f, 30f);
			float num3 = Mathf.Clamp(Tuning.F("plans.damp", 0.68f), 0.15f, 0.9f);
			float lean = Tuning.F("plans.lean", 0.1f);
			float num4 = num2 * Mathf.Sqrt(1f - num3 * num3);
			float num5 = Mathf.Atan2(Mathf.Sqrt(1f - num3 * num3), num3) / num4;
			float farthest = Mathf.Max(0.05f, Mathf.Exp((0f - num3) * num2 * num5) * Mathf.Sin(num4 * num5));
			for (int num6 = _tugs.Count - 1; num6 >= 0; num6--)
			{
				Tug tug = _tugs[num6];
				if (!FxKit.Alive((GodotObject)(object)tug.Behind) || !FxKit.Alive((GodotObject)(object)tug.Front) || !FxKit.Alive((GodotObject)(object)tug.Node) || !((Node)tug.Node).IsInsideTree() || tug.Node.Model != tug.Card)
				{
					if (FxKit.Alive((GodotObject)(object)tug.Node) && tug.Node.Model != tug.Card)
					{
						tug.Nudge = Vector2.Zero;
						tug.Turn = 0f;
					}
					Drop(tug);
					_tugs.RemoveAt(num6);
				}
				else
				{
					tug.T += dt;
					if (!(tug.T < 0f))
					{
						if (tug.T > 1.39f || num < 0.5f)
						{
							Drop(tug);
							_tugs.RemoveAt(num6);
						}
						else
						{
							try
							{
								Pull(tug, num, num2, num3, farthest, lean);
							}
							catch (Exception ex)
							{
								DevLog.Warn("RetainLine (pull): " + ex.Message);
								Drop(tug);
								_tugs.RemoveAt(num6);
							}
						}
					}
				}
			}
		}

		private void Pull(Tug tug, float reach, float stiff, float damp, float farthest, float lean)
		{
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0343: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_039a: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Unknown result type (might be due to invalid IL or missing references)
			//IL_042c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0431: Unknown result type (might be due to invalid IL or missing references)
			//IL_043c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0443: Unknown result type (might be due to invalid IL or missing references)
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0471: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0505: Unknown result type (might be due to invalid IL or missing references)
			//IL_0554: Unknown result type (might be due to invalid IL or missing references)
			//IL_0587: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_0363: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			NCard node = tug.Node;
			Node parent = ((Node)node).GetParent();
			CanvasItem val = (CanvasItem)(object)((parent is CanvasItem) ? parent : null);
			if (val == null)
			{
				return;
			}
			float t = tug.T;
			float num = Kicked(t, stiff, damp, farthest);
			float num2 = tug.Again * Kicked(t - tug.AgainAt, stiff, damp, farthest);
			float num3 = (num + num2) * tug.Force;
			float num4 = Mathf.Clamp(num3, 0f, 1.4f);
			float num5 = Mathf.Sin(t * 52f + tug.Phase) * num4 * num4;
			Transform2D globalTransform = ((CanvasItem)node).GetGlobalTransform();
			Transform2D val2 = val.GetGlobalTransform();
			Vector2 val3 = val2.BasisXform(tug.Nudge);
			Vector2 val4 = globalTransform.Origin - val3;
			Vector2 val5 = GameRefs.PilePoint((PileType)3) - val4;
			float num6 = Mathf.Clamp(Tuning.F("plans.aside", 0.35f), 0f, 2f);
			Vector2 val6 = (Vector2)((val5.LengthSquared() > 1f) ? val5.Normalized() : new Vector2(1f, 0f)) * num6 + new Vector2(0f, -1.15f);
			Vector2 val7 = val6.Normalized();
			Vector2 val8 = new Vector2(0f - val7.Y, val7.X);
			Vector2 val9 = val7 * (reach * num3) + val8 * (reach * 0.018f * num5);
			val2 = val.GetGlobalTransform();
			val2 = val2.AffineInverse();
			Vector2 val10 = val2.BasisXform(val9);
			float num7 = lean * ((val7.X >= 0f) ? 1f : (-1f)) * num3 + 0.012f * num5;
			((Control)node).Position = ((Control)node).Position + (val10 - tug.Nudge);
			((Control)node).Rotation = ((Control)node).Rotation + (num7 - tug.Turn);
			tug.Nudge = val10;
			tug.Turn = num7;
			globalTransform = ((CanvasItem)node).GetGlobalTransform();
			float num8 = BodyScale(node);
			tug.Behind.Scale = new Vector2(num8, num8);
			tug.Front.Scale = new Vector2(num8, num8);
			Transform2D val11 = new Transform2D(globalTransform.Rotation - num7, globalTransform.Scale, 0f, globalTransform.Origin - val9);
			float num9 = Mathf.Max(0.05f, globalTransform.Scale.X * num8);
			float num10 = Mathf.Max(20f, reach / num9);
			float num11 = Mathf.Clamp(t / 0.04f, 0f, 1f) * Mathf.Clamp((1.39f - t) / 0.14f, 0f, 1f);
			((CanvasItem)tug.Behind).Visible = true;
			((CanvasItem)tug.Front).Visible = true;
			for (int i = 0; i < 2; i++)
			{
				Vector2 val12 = new Vector2((i == 0) ? (-136f) : 136f, 196f);
				Vector2 val13 = tug.Behind.ToLocal(val11 * (val12 * num8));
				Vector2 val14 = val12 - val13;
				float num12 = val14.Length();
				float num13 = Mathf.Clamp(num12 / num10, 0f, 1.3f);
				float num14 = Mathf.Clamp(num13, 0f, 1f);
				Vector2 val15 = (val13 + val12) * 0.5f;
				if (num12 > 1f)
				{
					val15 += new Vector2(0f - val14.Y, val14.X) / num12 * (3.5f * num5);
				}
				Vector2[] points = new Vector2[3] { val13, val15, val12 };
				tug.Core[i].Points = points;
				tug.Halo[i].Points = points;
				tug.Core[i].Width = Mathf.Lerp(3.4f, 2f, num14);
				tug.Halo[i].Width = Mathf.Lerp(8f, 15f, num14);
				Color val16 = _color.Lerp(Colors.White, 0.25f + 0.45f * num14);
				tug.Core[i].DefaultColor = new Color(val16.R, val16.G, val16.B, num11 * Mathf.Clamp(0.22f + 0.78f * num14, 0f, 1f));
				tug.Halo[i].DefaultColor = new Color(_color.R, _color.G, _color.B, num11 * (0.08f + 0.36f * num14));
				((Node2D)tug.Peg[i]).Position = val13;
				((CanvasItem)tug.Peg[i]).Modulate = new Color(_color.R, _color.G, _color.B, num11 * 0.5f * num14);
				((CanvasItem)tug.Corner[i]).Modulate = new Color(_color.R, _color.G, _color.B, num11 * Mathf.Min(0.75f, 0.06f + 0.62f * num13));
				float num15 = 1f + 0.35f * num14;
				((Node2D)tug.Corner[i]).Scale = new Vector2(1.5f * num15, 1.5f * num15);
			}
		}

		public void Open()
		{
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Expected Obj, but got Unknown
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03de: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0407: Unknown result type (might be due to invalid IL or missing references)
			//IL_0416: Unknown result type (might be due to invalid IL or missing references)
			//IL_041b: Unknown result type (might be due to invalid IL or missing references)
			Node uiLayer = GameRefs.UiLayer;
			if (!FxKit.Alive((GodotObject)(object)uiLayer) || !uiLayer.IsInsideTree())
			{
				return;
			}
			List<Vector2> list = new List<Vector2>();
			HandCards.Fill(_hand);
			float num = 1f;
			foreach (HandCards.Entry item in _hand)
			{
				if (Seen(item.Node))
				{
					Transform2D globalTransform = ((CanvasItem)item.Node).GetGlobalTransform();
					float num2 = BodyScale(item.Node);
					num = globalTransform.Scale.X * num2;
					if (list.Count == 0)
					{
						list.Add(globalTransform * new Vector2(-154f * num2, -150f * num2));
					}
					list.Add(globalTransform * new Vector2(0f, -150f * num2));
				}
			}
			if (list.Count >= 2)
			{
				NCard node = _hand[_hand.Count - 1].Node;
				if (Seen(node))
				{
					list.Add(((CanvasItem)node).GetGlobalTransform() * new Vector2(154f * BodyScale(node), -150f * BodyScale(node)));
				}
			}
			else
			{
				Vector2 val = FxKit.ViewSize();
				list.Clear();
				list.Add(new Vector2(val.X * 0.28f, val.Y * 0.8f));
				list.Add(new Vector2(val.X * 0.5f, val.Y * 0.78f));
				list.Add(new Vector2(val.X * 0.72f, val.Y * 0.8f));
				num = 0.6f;
			}
			Node2D root = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_RetainQuill"),
				ZIndex = 5
			};
			uiLayer.AddChild((Node)(object)root, false, (InternalMode)0);
			Line2D halo = MakeLine(24f, new Color(_color.R, _color.G, _color.B, 0.3f), additive: true);
			Line2D core = MakeLine(7.5f, _bright, additive: false);
			((Node)root).AddChild((Node)(object)halo, false, (InternalMode)0);
			((Node)root).AddChild((Node)(object)core, false, (InternalMode)0);
			float l = 230f * Mathf.Clamp(num, 0.45f, 1f);
			Node2D quill = Props.Quill(l, new Color(_color.R * 0.9f, _color.G, _color.B * 0.92f, 1f), new Color(_color.R * 0.35f, _color.G * 0.72f, _color.B * 0.66f, 1f), new Color(0.1f, 0.32f, 0.28f, 1f), new Color(_color.R, _color.G, _color.B, 0.28f));
			quill.Rotation = 0.55f;
			((CanvasItem)quill).Modulate = new Color(1f, 1f, 1f, 0f);
			((Node)root).AddChild((Node)(object)quill, false, (InternalMode)0);
			Vector2[] local = new Vector2[list.Count];
			for (int i = 0; i < local.Length; i++)
			{
				local[i] = root.ToLocal(list[i]);
			}
			quill.Position = local[0] + new Vector2(18f, -40f);
			float num3 = Tuning.F("plans.write_time", 0.42f);
			Intro.Run((Node)(object)quill, 0f, 0.12f, (float v) =>
			{
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				quill.Position = local[0] + new Vector2(18f, -40f) * (1f - v);
				((CanvasItem)quill).Modulate = new Color(1f, 1f, 1f, v);
			}, (TransitionType)0, (EaseType)2);
			Intro.Run((Node)(object)root, 0.12f, num3, (float v) =>
			{
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				Vector2[] array = Intro.Partial(local, v);
				core.Points = array;
				halo.Points = array;
				quill.Position = array[^1];
				quill.Rotation = 0.55f + 0.1f * Mathf.Sin(v * 9f);
			}, (TransitionType)1, (EaseType)2);
			Intro.Run((Node)(object)quill, 0.12f + num3, 0.16f, (float v) =>
			{
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				quill.Position = local[local.Length - 1] + new Vector2(16f, -34f) * v;
				((CanvasItem)quill).Modulate = new Color(1f, 1f, 1f, 1f - v);
			}, (TransitionType)0, (EaseType)2);
			float num4 = 0.12f + num3 + 0.05f;
			float sink = 26f * Mathf.Clamp(num, 0.45f, 1f);
			Intro.Run((Node)(object)core, num4, 0.36f, (float v) =>
			{
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_008e: Unknown result type (might be due to invalid IL or missing references)
				Vector2 position = new Vector2(0f, sink * v * v);
				((Node2D)core).Position = position;
				((Node2D)halo).Position = position;
				core.Width = 7.5f * (1f - 0.6f * v);
				((CanvasItem)core).Modulate = new Color(1f, 1f, 1f, 1f - v);
				((CanvasItem)halo).Modulate = new Color(1f, 1f, 1f, 1f - v);
			}, (TransitionType)0, (EaseType)2);
			if (Tuning.F("plans.seep", 1f) > 0.5f)
			{
				int num5 = 0;
				foreach (HandCards.Entry item2 in _hand)
				{
					if (Seen(item2.Node))
					{
						try
						{
							Seep(item2.Node, num4 + 0.03f * (float)num5, num5);
						}
						catch (Exception ex)
						{
							DevLog.Warn("RetainLine (seep): " + ex.Message);
						}
						num5++;
					}
				}
			}
			Intro.At((Node)(object)root, 0.12f + num3, () =>
			{
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				FxKit.Burst((Node)(object)root, root.ToGlobal(local[local.Length - 1]), new Color(_color.R, _color.G, _color.B, 0.95f), 8, 80f, 230f, 0.3, 0.05f, 0.11f, new Vector2(0f, 240f));
			});
			FxKit.FreeAfter((Node)(object)root, 0.12f + num3 + 0.6f);
		}

		private static float SeepEnd(Node2D root, float x)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				float num = FxKit.ViewSize().Y - 10f;
				Vector2 val = root.ToGlobal(new Vector2(x, -150f));
				Vector2 val2 = root.ToGlobal(new Vector2(x, 196f));
				if (val2.Y <= num || val2.Y - val.Y < 1f)
				{
					return 196f;
				}
				return Mathf.Lerp(-150f, 196f, Mathf.Clamp((num - val.Y) / (val2.Y - val.Y), 0.25f, 1f));
			}
			catch
			{
				return 196f;
			}
		}

		private void Seep(NCard node, float delay, int index)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected Obj, but got Unknown
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Expected Obj, but got Unknown
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			Node2D root = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_RetainSeep")
			};
			((Node)node).AddChild((Node)(object)root, false, (InternalMode)0);
			if (!((Node)root).IsInsideTree())
			{
				((Node)root).QueueFree();
				return;
			}
			float num = BodyScale(node);
			root.Scale = new Vector2(num, num);
			float num2 = Mathf.Max(0.15f, Tuning.F("plans.seep_time", 0.5f)) * Mathf.Lerp(0.92f, 1.1f, Chance(index, 6));
			FxKit.FreeAfter((Node)(object)root, delay + num2 + 0.75f);
			for (int i = 0; i < 2; i++)
			{
				float x = ((i == 0) ? (-136f) : 136f);
				Line2D tail = MakeLine(4.4f, _bright, additive: true);
				Gradient val = new Gradient();
				val.Offsets = new float[2] { 0f, 1f };
				val.Colors = new Color[2]
				{
					new Color(_color.R, _color.G, _color.B, 0f),
					_bright
				};
				tail.Gradient = val;
				((CanvasItem)tail).Visible = false;
				Sprite2D head = FxKit.Glow(new Color(_color.R, _color.G, _color.B, 0f), new Vector2(34f, 34f));
				Sprite2D bloom = FxKit.Glow(new Color(_color.R, _color.G, _color.B, 0f), new Vector2(120f, 120f));
				((Node)root).AddChild((Node)(object)tail, false, (InternalMode)0);
				((Node)root).AddChild((Node)(object)head, false, (InternalMode)0);
				((Node)root).AddChild((Node)(object)bloom, false, (InternalMode)0);
				float num3 = 0.04f * (float)i + 0.03f * Chance(index, 7 + i);
				float sway = (Chance(index, 9 + i) - 0.5f) * 9f;
				float end = 196f;
				Intro.Run((Node)(object)root, delay + num3, num2, (float v) =>
				{
					//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
					//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
					//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
					//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
					//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
					//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
					//IL_0127: Unknown result type (might be due to invalid IL or missing references)
					//IL_0140: Unknown result type (might be due to invalid IL or missing references)
					//IL_0196: Unknown result type (might be due to invalid IL or missing references)
					if (FxKit.Alive((GodotObject)(object)tail) && FxKit.Alive((GodotObject)(object)head))
					{
						end = SeepEnd(root, x);
						float num4 = Mathf.Lerp(-150f, end, Mathf.Pow(v, 1.7f));
						float num5 = sway * Mathf.Sin(v * ((float)Math.PI * 2f)) * (1f - v);
						float num6 = Mathf.Min(74f, num4 - -150f);
						tail.Points = new Vector2[3]
						{
							new Vector2(x + num5 * 0.3f, num4 - num6),
							new Vector2(x + num5 * 0.7f, num4 - num6 * 0.5f),
							new Vector2(x + num5, num4)
						};
						((CanvasItem)tail).Visible = num6 > 1f;
						float num7 = Mathf.Clamp(v * 7f, 0f, 1f);
						((CanvasItem)tail).Modulate = new Color(1f, 1f, 1f, num7);
						((Node2D)head).Position = new Vector2(x + num5, num4);
						((CanvasItem)head).Modulate = new Color(_color.R, _color.G, _color.B, 0.85f * num7);
					}
				}, (TransitionType)0, (EaseType)2);
				Intro.Run((Node)(object)root, delay + num3 + num2, 0.36f, (float v) =>
				{
					//IL_0069: Unknown result type (might be due to invalid IL or missing references)
					//IL_006e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0089: Unknown result type (might be due to invalid IL or missing references)
					//IL_008e: Unknown result type (might be due to invalid IL or missing references)
					//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
					//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
					//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
					//IL_011c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0138: Unknown result type (might be due to invalid IL or missing references)
					//IL_01be: Unknown result type (might be due to invalid IL or missing references)
					//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
					if (FxKit.Alive((GodotObject)(object)tail) && FxKit.Alive((GodotObject)(object)head) && FxKit.Alive((GodotObject)(object)bloom))
					{
						float num4 = Mathf.Clamp(1f - v * 2.6f, 0f, 1f);
						float num5 = 74f * num4;
						tail.Points = new Vector2[3]
						{
							new Vector2(x, end - num5),
							new Vector2(x, end - num5 * 0.5f),
							new Vector2(x, end)
						};
						((CanvasItem)tail).Modulate = new Color(1f, 1f, 1f, num4);
						((CanvasItem)head).Modulate = new Color(_color.R, _color.G, _color.B, 0.85f * num4);
						((Node2D)bloom).Position = new Vector2(x, end);
						float num6 = Mathf.Sin((float)Math.PI * Mathf.Pow(v, 0.6f));
						((CanvasItem)bloom).Modulate = new Color(_color.R, _color.G, _color.B, Mathf.Clamp(Tuning.F("plans.seep_light", 0.55f), 0f, 1f) * num6);
						float num7 = Mathf.Lerp(0.6f, 1.25f, v);
						((Node2D)bloom).Scale = new Vector2(1.875f * num7, 1.875f * num7);
					}
				}, (TransitionType)0, (EaseType)2);
			}
		}
	}
