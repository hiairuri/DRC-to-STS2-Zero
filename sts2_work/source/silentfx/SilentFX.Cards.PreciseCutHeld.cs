// namespace: SilentFX.Cards
// type: PreciseCutHeld (class)
// source lines: 87237-87676
	public sealed class PreciseCutHeld : HeldSession
	{
		private static readonly Color Cold = new Color(0.84f, 0.97f, 1f, 1f);

		private const float HalfW = 150f;

		private const float HalfH = 211f;

		private static readonly float CornerAngle = Mathf.Atan2(-211f, 150f);

		private const float Omega = 17f;

		private const float Zeta = 0.6f;

		private NCard _node;

		private float _applied;

		private float _turn;

		private float _speed;

		private bool _let;

		private Node2D _mark;

		private Line2D _jawA;

		private Line2D _jawB;

		private Line2D _stopNear;

		private Line2D _stopFar;

		private Line2D _sight;

		private Sprite2D _pip;

		private readonly Vector2[] _two = new Vector2[2];

		private float _show;

		private float _lock;

		private bool _locked;

		private Vector2 _at;

		private Vector2 _way = new Vector2(1f, 0f);

		private float _half = 120f;

		protected override void Build()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected Obj, but got Unknown
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			_mark = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_PreciseAim"),
				Visible = false
			};
			((Node)Front).AddChild((Node)(object)_mark, false, (InternalMode)0);
			_sight = Hair();
			_jawA = Hair();
			_jawB = Hair();
			_stopNear = Hair();
			_stopFar = Hair();
			_pip = FxKit.Glow(new Color(1f, 1f, 1f, 0f), new Vector2(64f, 64f));
			((Node)_mark).AddChild((Node)(object)_pip, false, (InternalMode)0);
		}

		private Line2D Hair()
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			Line2D val = Props.Line(1.5f, new Color(Cold.R, Cold.G, Cold.B, 1f), true, Vector2.Zero, Vector2.Zero);
			((CanvasItem)val).Modulate = new Color(1f, 1f, 1f, 0f);
			((Node)_mark).AddChild((Node)(object)val, false, (InternalMode)0);
			return val;
		}

		public override void OnTargetChanged()
		{
			if (C.HasTarget)
			{
				_lock = 0f;
				_locked = false;
			}
		}

		protected override void Tick(float dt)
		{
			try
			{
				TurnCard(dt);
			}
			catch (Exception ex)
			{
				DevLog.Warn("PreciseCutHeld (the card is left as it is): " + ex.Message);
				_let = true;
			}
			DrawMark(dt);
		}

		private void TurnCard(float dt)
		{
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			if (_let)
			{
				return;
			}
			CardModel card = C.Card;
			if (card == null)
			{
				return;
			}
			if (!FxKit.Alive((GodotObject)(object)_node) || _node.Model != card)
			{
				_node = HandCards.NodeOf(card);
				_applied = 0f;
			}
			if (!FxKit.Alive((GodotObject)(object)_node) || !((Node)_node).IsInsideTree())
			{
				_node = null;
				return;
			}
			float num = 0f;
			if (C.HasTarget && !Leaving && Tuning.F("precise.aim_turn", 1f) >= 0.5f)
			{
				Vector2 val = C.TargetChest - ((CanvasItem)_node).GetGlobalTransform().Origin;
				if (val.LengthSquared() > 400f)
				{
					Node parent = ((Node)_node).GetParent();
					CanvasItem val2 = (CanvasItem)(object)((parent is CanvasItem) ? parent : null);
					float num2;
					if (val2 == null)
					{
						num2 = 0f;
					}
					else
					{
						Transform2D globalTransform = val2.GetGlobalTransform();
						num2 = globalTransform.Rotation;
					}
					float num3 = num2;
					float num4 = ((Control)_node).Rotation - _applied;
					num = Mathf.Wrap(val.Angle() - CornerAngle - num3 - num4, -(float)Math.PI, (float)Math.PI);
				}
			}
			int num5 = Mathf.Clamp(Mathf.CeilToInt(dt / 0.008f), 1, 8);
			float num6 = dt / (float)num5;
			for (int i = 0; i < num5; i++)
			{
				float num7 = 289f * (num - _turn) - 20.400002f * _speed;
				_speed += num7 * num6;
				_turn += _speed * num6;
			}
			if (!float.IsFinite(_turn) || !float.IsFinite(_speed))
			{
				_turn = num;
				_speed = 0f;
			}
			NCard node = _node;
			((Control)node).Rotation = ((Control)node).Rotation + (_turn - _applied);
			_applied = _turn;
		}

		private void Unturn()
		{
			try
			{
				if (_applied != 0f && FxKit.Alive((GodotObject)(object)_node) && _node.Model == C.Card)
				{
					NCard node = _node;
					((Control)node).Rotation = ((Control)node).Rotation - _applied;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("PreciseCutHeld.Unturn: " + ex.Message);
			}
			_applied = 0f;
			_turn = 0f;
			_speed = 0f;
		}

		private bool Corner(out Vector2 corner, out Vector2 middle)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = (corner = (middle = C.Hand));
			if (!FxKit.Alive((GodotObject)(object)_node) || !((Node)_node).IsInsideTree())
			{
				return false;
			}
			CardStandIn.Pose(_node, out middle, out var rotation, out var scale);
			Vector2 val2 = middle;
			val = new Vector2(150f * scale.X, -211f * scale.Y);
			corner = val2 + val.Rotated(rotation);
			return true;
		}

		private void DrawMark(float dt)
		{
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0343: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0363: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0400: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_040e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0410: Unknown result type (might be due to invalid IL or missing references)
			//IL_0412: Unknown result type (might be due to invalid IL or missing references)
			//IL_0417: Unknown result type (might be due to invalid IL or missing references)
			//IL_041c: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			//IL_045d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0498: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0523: Unknown result type (might be due to invalid IL or missing references)
			//IL_054b: Unknown result type (might be due to invalid IL or missing references)
			bool flag = C.HasTarget && !Playing;
			_show = FxKit.Damp(_show, flag ? 1f : 0f, Playing ? 20f : 12f, dt);
			_lock = FxKit.Damp(_lock, C.HasTarget ? 1f : 0f, 13f, dt);
			if (!FxKit.Alive((GodotObject)(object)_mark))
			{
				return;
			}
			if (_show < 0.004f)
			{
				((CanvasItem)_mark).Visible = false;
				return;
			}
			((CanvasItem)_mark).Visible = true;
			bool flag2 = Corner(out var corner, out var middle);
			if (C.HasTarget)
			{
				_at = C.TargetChest;
				Vector2 val = _at - middle;
				if (val.LengthSquared() > 1f)
				{
					_way = val.Normalized();
				}
				float num = Mathf.Max(80f, C.TargetWidth);
				float num2 = Mathf.Max(80f, C.TargetHeight);
				_half = Mathf.Clamp(0.54f * Mathf.Sqrt(num * num * _way.X * _way.X + num2 * num2 * _way.Y * _way.Y), 60f, 440f);
			}
			Transform2D globalTransform = ((CanvasItem)_mark).GetGlobalTransform();
			Vector2 val2 = globalTransform.Scale;
			float num3 = Mathf.Clamp(val2.Abs().X, 0.05f, 20f);
			float num4 = Mathf.Sqrt(Strokes.Scale(C));
			float num5 = _lock * _lock * (3f - 2f * _lock);
			float num6 = Mathf.Max(8f, Tuning.F("precise.aim_open", 34f));
			float num7 = Mathf.Clamp(Tuning.F("precise.aim_gap", 7f), 2f, num6);
			float num8 = (Mathf.Lerp(num6, num7, num5) + 0.8f * num5 * Mathf.Sin(Time * 6f)) * num4;
			if (Playing)
			{
				num8 *= Mathf.Clamp(_show, 0f, 1f);
			}
			float num9 = 1.5f * Mathf.Clamp(Tuning.F("precise.aim_thick", 1f), 0.4f, 4f) / num3;
			float alpha = 0.92f * _show;
			Vector2 val3 = new Vector2(0f - _way.Y, _way.X);
			Vector2 val4 = _at - _way * _half;
			Vector2 val5 = _at + _way * _half;
			Vector2 val6 = val3 * (num8 * 0.5f);
			float num10 = num8 * 0.5f + 7f * num4;
			Set(_jawA, val4 + val6, val5 + val6, num9, alpha);
			Set(_jawB, val4 - val6, val5 - val6, num9, alpha);
			Set(_stopNear, val4 + val3 * num10, val4 - val3 * num10, num9, alpha);
			Set(_stopFar, val5 + val3 * num10, val5 - val3 * num10, num9, alpha);
			Vector2 val7 = corner + _way * (14f * num4);
			Vector2 val8 = val4 - _way * (8f * num4);
			float num11;
			if (flag2)
			{
				val2 = val8 - val7;
				if (val2.Dot(_way) > 30f)
				{
					num11 = Mathf.Clamp(Tuning.F("precise.aim_line", 0.26f), 0f, 1f);
					goto IL_0452;
				}
			}
			num11 = 0f;
			goto IL_0452;
			IL_0452:
			float num12 = num11;
			Set(_sight, val7, val8, num9 * 0.8f, num12 * _show * num5);
			if (FxKit.Alive((GodotObject)(object)_pip))
			{
				float num13 = 11f * num4 / num3;
				((Node2D)_pip).GlobalPosition = _at;
				((Node2D)_pip).Scale = new Vector2(num13, num13) / 64f;
				((CanvasItem)_pip).Modulate = new Color(1f, 1f, 1f, _show * num5);
			}
			if (!_locked && C.HasTarget && !Playing && _lock > 0.86f)
			{
				_locked = true;
				FxKit.Flash((Node)(object)Front, _at, new Color(Cold.R, Cold.G, Cold.B, 0.5f), 36f * num4, 1.5f, 0.14);
				PreciseCutChains.Twinkle(C.Card);
			}
		}

		private void Set(Line2D line, Vector2 from, Vector2 to, float width, float alpha)
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)line))
			{
				bool flag = alpha > 0.004f;
				if (((CanvasItem)line).Visible != flag)
				{
					((CanvasItem)line).Visible = flag;
				}
				if (flag)
				{
					_two[0] = _mark.ToLocal(from);
					_two[1] = _mark.ToLocal(to);
					line.Points = _two;
					line.Width = Mathf.Max(0.5f, width);
					((CanvasItem)line).Modulate = new Color(1f, 1f, 1f, Mathf.Clamp(alpha, 0f, 1f));
				}
			}
		}

		protected override void OnPlay(CardFxContext play)
		{
			_let = true;
			Unturn();
			Leave(12f);
		}

		protected override void OnEnd()
		{
			Unturn();
		}

		public PreciseCutHeld()
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		}

		static PreciseCutHeld()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		}
	}
