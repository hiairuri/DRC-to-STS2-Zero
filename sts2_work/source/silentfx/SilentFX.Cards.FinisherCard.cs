// namespace: SilentFX.Cards
// type: FinisherCard (class)
// source lines: 57086-57557
	internal sealed class FinisherCard
	{
		internal const float Wide = 300f;

		internal const float Tall = 422f;

		private const float Unit = 100f;

		private static float _edgeOn = 0.3f;

		private static float _lean = 0.3f;

		private NCard _copy;

		private CardModel _shown;

		private Node2D _plain;

		private Node2D _lights;

		private Node2D _side;

		private Node2D _front;

		private Sprite2D _tip;

		private Sprite2D _glint;

		public Node2D Rig { get; private set; }

		public bool Alive
		{
			get
			{
				try
				{
					if (!FxKit.Alive((GodotObject)(object)Rig) || !((Node)Rig).IsInsideTree())
					{
						return false;
					}
					if (_copy == null)
					{
						return FxKit.Alive((GodotObject)(object)_plain);
					}
					return FxKit.Alive((GodotObject)(object)_copy) && _copy.Model == _shown;
				}
				catch
				{
					return false;
				}
			}
		}

		internal static void ReadTuning()
		{
			_edgeOn = Mathf.Clamp(Tuning.F("finisher.edge_on", 0.3f), 0.08f, 0.9f);
			_lean = Mathf.Clamp(Tuning.F("finisher.lean", 0.3f), 0f, 0.8f);
		}

		public static FinisherCard Make(CardModel card, Node cards, Node lights)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Expected Obj, but got Unknown
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)cards) || !cards.IsInsideTree())
			{
				return null;
			}
			FinisherCard finisherCard = new FinisherCard();
			try
			{
				Node2D val = new Node2D
				{
					Name = StringName.op_Implicit("SilentFX_FinisherCard")
				};
				cards.AddChild((Node)(object)val, false, (InternalMode)0);
				finisherCard.Rig = val;
				if (card != null)
				{
					NCard val2 = CardStandIn.Make(card, (Node)(object)val);
					if (FxKit.Alive((GodotObject)(object)val2))
					{
						finisherCard._copy = val2;
						finisherCard._shown = card;
						((Control)val2).Position = Vector2.Zero;
						((Control)val2).Rotation = 0f;
						((Control)val2).Scale = Vector2.One;
						((CanvasItem)val2).Modulate = new Color(1f, 1f, 1f, 0f);
					}
				}
				if (finisherCard._copy == null)
				{
					finisherCard._plain = MakePlain();
					((CanvasItem)finisherCard._plain).Modulate = new Color(1f, 1f, 1f, 0f);
					((Node)val).AddChild((Node)(object)finisherCard._plain, false, (InternalMode)0);
				}
				if (FxKit.Alive((GodotObject)(object)lights) && lights.IsInsideTree())
				{
					finisherCard.MakeLights(lights);
				}
				return finisherCard;
			}
			catch (Exception ex)
			{
				DevLog.Warn("FinisherCard.Make: " + ex.Message);
				finisherCard.Free();
				return null;
			}
		}

		private static Node2D MakePlain()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Expected Obj, but got Unknown
			Node2D val = new Node2D
			{
				Name = StringName.op_Implicit("Plain")
			};
			((Node)val).AddChild((Node)(object)FxKit.Shape(Props.Box(-150f, -211f, 150f, 211f), new Color(0.13f, 0.15f, 0.21f, 1f)), false, (InternalMode)0);
			((Node)val).AddChild((Node)(object)FxKit.Shape(Props.Box(-138f, -199f, 138f, 199f), new Color(0.8f, 0.84f, 0.92f, 1f)), false, (InternalMode)0);
			((Node)val).AddChild((Node)(object)FxKit.Shape(Props.Box(-112f, -168f, 112f, -8f), new Color(0.3f, 0.36f, 0.5f, 1f)), false, (InternalMode)0);
			return val;
		}

		private static Curve Taper(float from, float to)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Expected Obj, but got Unknown
			Curve val = new Curve();
			val.AddPoint(new Vector2(0f, from), 0f, 0f, (TangentMode)0, (TangentMode)0);
			val.AddPoint(new Vector2(1f, to), 0f, 0f, (TangentMode)0, (TangentMode)0);
			return val;
		}

		private void MakeLights(Node parent)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected Obj, but got Unknown
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Expected Obj, but got Unknown
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Expected Obj, but got Unknown
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			_lights = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_FinisherEdge"),
				Visible = false
			};
			parent.AddChild((Node)(object)_lights, false, (InternalMode)0);
			Color hone = FinisherFx.Hone;
			_side = new Node2D();
			((Node)_lights).AddChild((Node)(object)_side, false, (InternalMode)0);
			Sprite2D val = FxKit.Glow(new Color(hone.R, hone.G, hone.B, 0.62f), new Vector2(130f, 15f));
			((Node2D)val).Position = new Vector2(46f, 0f);
			((Node)_side).AddChild((Node)(object)val, false, (InternalMode)0);
			Line2D val2 = Props.Line(2.6f, new Color(1f, 0.99f, 0.95f, 1f), true, Vector2.Zero, new Vector2(100f, 0f));
			val2.WidthCurve = Taper(1f, 0.16f);
			((Node)_side).AddChild((Node)(object)val2, false, (InternalMode)0);
			_front = new Node2D();
			((Node)_lights).AddChild((Node)(object)_front, false, (InternalMode)0);
			Sprite2D val3 = FxKit.Glow(new Color(hone.R, hone.G, hone.B, 0.8f), new Vector2(140f, 20f));
			((Node2D)val3).Position = new Vector2(50f, 0f);
			((Node)_front).AddChild((Node)(object)val3, false, (InternalMode)0);
			Line2D val4 = Props.Line(3.2f, new Color(1f, 1f, 1f, 1f), true, Vector2.Zero, new Vector2(100f, 0f));
			val4.WidthCurve = Taper(1f, 0.45f);
			((Node)_front).AddChild((Node)(object)val4, false, (InternalMode)0);
			_tip = FxKit.Glow(new Color(1f, 1f, 1f, 0f), new Vector2(64f, 64f));
			((Node)_lights).AddChild((Node)(object)_tip, false, (InternalMode)0);
			_glint = FxKit.Glow(new Color(1f, 1f, 1f, 0f), new Vector2(64f, 64f));
			((CanvasItem)_glint).Visible = false;
			((Node)_lights).AddChild((Node)(object)_glint, false, (InternalMode)0);
		}

		private static void Lay(Node2D line, Vector2 from, Vector2 to, float thick, float host)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)line))
			{
				Vector2 val = to - from;
				line.GlobalPosition = from;
				line.GlobalRotation = ((val.LengthSquared() > 0.01f) ? val.Angle() : 0f);
				line.Scale = new Vector2(Mathf.Max(0.01f, val.Length() / (100f * host)), Mathf.Max(0.01f, thick));
			}
		}

		internal static Vector2 PointOffset(float turn, float way, float stretch)
		{
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			float num = ((way < 0f) ? (-1f) : 1f);
			float num2 = Mathf.Clamp(Mathf.Lerp(1f, _edgeOn, turn), 0.05f, 1.1f);
			float num3 = Mathf.Min(1f, num2);
			float num4 = _lean * Mathf.Sqrt(Mathf.Max(0f, 1f - num3 * num3)) * num;
			return new Vector2(num2 * 300f * 0.5f * num, (0f - num4) * 300f * 0.5f * num - Mathf.Max(0.02f, stretch) * 422f * 0.5f);
		}

		public void Put(Vector2 at, float rotation, float size, float turn, float way, float stretch, float light, float alpha, float hone, float glint)
		{
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_045d: Unknown result type (might be due to invalid IL or missing references)
			//IL_045f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0461: Unknown result type (might be due to invalid IL or missing references)
			//IL_0466: Unknown result type (might be due to invalid IL or missing references)
			//IL_046e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0470: Unknown result type (might be due to invalid IL or missing references)
			//IL_0474: Unknown result type (might be due to invalid IL or missing references)
			//IL_0479: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)Rig) || !((Node)Rig).IsInsideTree())
			{
				return;
			}
			float num = Mathf.Max(0.02f, size);
			float num2 = Mathf.Clamp(alpha, 0f, 1f);
			float num3 = ((way < 0f) ? (-1f) : 1f);
			float num4 = Mathf.Clamp(Mathf.Lerp(1f, _edgeOn, turn), 0.05f, 1.1f);
			float num5 = Mathf.Min(1f, num4);
			float num6 = _lean * Mathf.Sqrt(Mathf.Max(0f, 1f - num5 * num5)) * num3;
			Vector2 val = new Vector2(num * num4, (0f - num) * num6);
			Vector2 val2 = val.Rotated(rotation);
			val = new Vector2(0f, num * Mathf.Max(0.02f, stretch));
			Vector2 val3 = val.Rotated(rotation);
			Rig.GlobalTransform = new Transform2D(val2, val3, at);
			Color modulate = new Color(light, light, light, num2);
			if (_copy != null)
			{
				if (FxKit.Alive((GodotObject)(object)_copy) && _copy.Model == _shown)
				{
					Vector2 val4;
					if (!FxKit.Alive((GodotObject)(object)_copy.Body))
					{
						val4 = Vector2.One;
					}
					else
					{
						val = _copy.Body.Scale;
						val4 = val.Abs();
					}
					Vector2 val5 = val4;
					((Control)_copy).Scale = new Vector2(1f / Mathf.Max(0.001f, val5.X), 1f / Mathf.Max(0.001f, val5.Y));
					((CanvasItem)_copy).Modulate = modulate;
				}
			}
			else if (FxKit.Alive((GodotObject)(object)_plain))
			{
				((CanvasItem)_plain).Modulate = modulate;
			}
			if (!FxKit.Alive((GodotObject)(object)_lights))
			{
				return;
			}
			float num7 = Mathf.Clamp(hone, 0f, 1.6f) * num2;
			bool flag = num7 > 0.01f;
			if (((CanvasItem)_lights).Visible != flag)
			{
				((CanvasItem)_lights).Visible = flag;
			}
			if (!flag || !((Node)_lights).IsInsideTree())
			{
				return;
			}
			Transform2D globalTransform = ((CanvasItem)_lights).GetGlobalTransform();
			val = globalTransform.Scale;
			float num8 = Mathf.Clamp(val.Abs().X, 0.05f, 20f);
			Vector2 val6 = at + val2 * (150f * num3) - val3 * 211f;
			Vector2 val7 = at + val2 * (150f * num3) + val3 * 211f;
			Vector2 to = at - val2 * (150f * num3) - val3 * 211f;
			float num9 = Mathf.Clamp(num / 0.34f, 0.7f, 2.2f) / num8;
			Lay(_side, val6, val7, num9, num8);
			Lay(_front, val6, to, num9, num8);
			if (FxKit.Alive((GodotObject)(object)_side))
			{
				((CanvasItem)_side).Modulate = new Color(1f, 1f, 1f, Mathf.Min(1f, num7));
			}
			if (FxKit.Alive((GodotObject)(object)_front))
			{
				((CanvasItem)_front).Modulate = new Color(1f, 1f, 1f, Mathf.Min(1f, num7 * 1.2f));
			}
			if (FxKit.Alive((GodotObject)(object)_tip))
			{
				float num10 = (20f + 18f * Mathf.Max(0f, num7 - 0.7f)) * num9;
				((Node2D)_tip).GlobalPosition = val6;
				((Node2D)_tip).Scale = new Vector2(num10, num10) / 64f;
				((CanvasItem)_tip).Modulate = new Color(1f, 0.98f, 0.92f, Mathf.Min(1f, num7 * 0.85f));
			}
			if (FxKit.Alive((GodotObject)(object)_glint))
			{
				bool flag2 = glint >= 0f && glint <= 1f;
				if (((CanvasItem)_glint).Visible != flag2)
				{
					((CanvasItem)_glint).Visible = flag2;
				}
				if (flag2)
				{
					Vector2 val8 = val6 - val7;
					((Node2D)_glint).GlobalPosition = val7 + val8 * glint;
					((Node2D)_glint).GlobalRotation = ((val8.LengthSquared() > 0.01f) ? val8.Angle() : 0f);
					((Node2D)_glint).Scale = new Vector2(58f * num9, 10f * num9) / 64f;
					((CanvasItem)_glint).Modulate = new Color(1f, 1f, 0.94f, Mathf.Sin((float)Math.PI * glint) * num2);
				}
			}
		}

		public void Free()
		{
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (FxKit.Alive((GodotObject)(object)_lights))
				{
					((Node)_lights).QueueFree();
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("FinisherCard.Free (lights): " + ex.Message);
			}
			_lights = null;
			_side = null;
			_front = null;
			_tip = null;
			_glint = null;
			NCard copy = _copy;
			CardModel shown = _shown;
			_copy = null;
			_shown = null;
			if (copy != null)
			{
				try
				{
					if (FxKit.Alive((GodotObject)(object)copy) && copy.Model == shown)
					{
						((CanvasItem)copy).Modulate = Colors.White;
					}
				}
				catch (Exception ex2)
				{
					DevLog.Warn("FinisherCard.Free (tint): " + ex2.Message);
				}
				CardStandIn.Free(copy);
			}
			Node2D rig = Rig;
			Rig = null;
			_plain = null;
			try
			{
				if (FxKit.Alive((GodotObject)(object)rig))
				{
					if (copy != null && GodotObject.IsInstanceValid((GodotObject)(object)copy) && (object)((Node)copy).GetParent() == rig)
					{
						((Node)rig).RemoveChild((Node)(object)copy);
					}
					((Node)rig).QueueFree();
				}
			}
			catch (Exception ex3)
			{
				DevLog.Warn("FinisherCard.Free (rig): " + ex3.Message);
			}
		}
	}
