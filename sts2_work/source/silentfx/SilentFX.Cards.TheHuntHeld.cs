// namespace: SilentFX.Cards
// type: TheHuntHeld (class)
// source lines: 113102-113383
	public sealed class TheHuntHeld : HeldSession
	{
		private Node2D _mark;

		private Sprite2D _glow;

		private readonly Line2D[] _corner = new Line2D[4];

		private Line2D _chevron;

		private Node2D _blade;

		private float _show;

		private float _gold;

		private float _check;

		private bool _fatal;

		private float _size = 110f;

		protected override void Build()
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Expected Obj, but got Unknown
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			_blade = FxKit.Blade(52f, 5f, new Color(0.92f, 0.97f, 1f, 1f), new Color(0.62f, 0.7f, 0.8f, 1f), new Color(0.78f, 0.6f, 0.22f, 1f), new Color(TheHuntFx.Gold.R, TheHuntFx.Gold.G, TheHuntFx.Gold.B, 0.28f));
			((Node)Front).AddChild((Node)(object)_blade, false, (InternalMode)0);
			_mark = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_HuntMark")
			};
			((Node)Front).AddChild((Node)(object)_mark, false, (InternalMode)0);
			_glow = FxKit.Glow(new Color(TheHuntFx.Gold.R, TheHuntFx.Gold.G, TheHuntFx.Gold.B, 0f), new Vector2(64f, 64f));
			((Node)_mark).AddChild((Node)(object)_glow, false, (InternalMode)0);
			for (int i = 0; i < 4; i++)
			{
				_corner[i] = Props.Line(5.5f, Colors.White, true);
				((Node)_mark).AddChild((Node)(object)_corner[i], false, (InternalMode)0);
			}
			_chevron = Props.Line(5.5f, Colors.White, true);
			((Node)_mark).AddChild((Node)(object)_chevron, false, (InternalMode)0);
			((CanvasItem)_mark).Modulate = new Color(1f, 1f, 1f, 0f);
		}

		public override void OnTargetChanged()
		{
			_check = 0f;
		}

		protected override void Tick(float dt)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)_blade))
			{
				Vector2 val = C.WeaponHand + new Vector2(8f * C.Facing, -6f);
				Vector2 val2 = (Vector2)(C.HasTarget ? (C.TargetChest - val) : new Vector2(C.Facing, -0.25f));
				if (val2.LengthSquared() < 1f)
				{
					val2 = new Vector2(C.Facing, 0f);
				}
				_blade.GlobalPosition = val + new Vector2(Mathf.Sin(Time * 31f) * 0.8f * _gold, Mathf.Sin(Time * 2.2f) * 2.5f);
				_blade.Rotation = Mathf.LerpAngle(_blade.Rotation, val2.Angle() + (float)Math.PI / 2f, Mathf.Clamp(dt * 14f, 0f, 1f));
				float num = (0.85f + 0.25f * Arm) * Mathf.Clamp(C.Body.Y / 244f, 0.6f, 1.3f);
				_blade.Scale = new Vector2(num, num);
				((CanvasItem)_blade).Modulate = new Color(1f, 1f, 1f, Lift);
			}
			if (C.HasTarget && !Playing)
			{
				_check -= dt;
				if (_check <= 0f)
				{
					_check = 0.2f;
					TheHuntFx.WouldKill(C.Card, C.Target, out _fatal);
				}
			}
			bool flag = C.HasTarget && !Playing;
			_show = FxKit.Damp(_show, flag ? 1f : 0f, 13f, dt);
			_gold = FxKit.Damp(_gold, (_fatal && C.HasTarget) ? 1f : 0f, 12f, dt);
			if (FxKit.Alive((GodotObject)(object)_mark))
			{
				if (C.HasTarget)
				{
					_size = Mathf.Clamp(Mathf.Max(C.TargetWidth, C.TargetHeight) * Tuning.F("hunt.mark", 0.5f), 70f, 190f);
				}
				Place(_mark, _show, _gold, 0f);
			}
		}

		private void Place(Node2D mark, float show, float gold, float shut)
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)mark))
			{
				return;
			}
			if (C.HasTarget && !Playing)
			{
				mark.GlobalPosition = C.TargetChest;
			}
			((CanvasItem)mark).Modulate = new Color(1f, 1f, 1f, show);
			float num = gold * 0.05f * Mathf.Sin(Time * 7.5f);
			float num2 = _size * (Mathf.Lerp(1.1f, 0.9f, gold) + num + 0.7f * (1f - show)) * (1f - 0.42f * shut);
			float num3 = _size * 0.36f;
			mark.Rotation = (1f - show) * -0.6f;
			Color val = TheHuntFx.Pale.Lerp(TheHuntFx.Gold, gold);
			val = new Color(val.R, val.G, val.B, Mathf.Lerp(0.7f, 1f, gold));
			float num4 = num2 * 1.22f;
			for (int i = 0; i < 4; i++)
			{
				if (FxKit.Alive((GodotObject)(object)_corner[i]))
				{
					Vector2 val2 = i switch
					{
						2 => new Vector2(0f, 1f), 
						1 => new Vector2(1f, 0f), 
						0 => new Vector2(0f, -1f), 
						_ => new Vector2(-1f, 0f), 
					};
					Vector2 val3 = new Vector2(0f - val2.Y, val2.X);
					Vector2 val4 = val2 * num4;
					_corner[i].Points = new Vector2[3]
					{
						val4 - val2 * (num3 * 0.72f) + val3 * (num3 * 0.72f),
						val4,
						val4 - val2 * (num3 * 0.72f) - val3 * (num3 * 0.72f)
					};
					_corner[i].DefaultColor = val;
					_corner[i].Width = Mathf.Lerp(4.5f, 6.5f, gold);
				}
			}
			float num5 = 0f - num4 - 22f - (1f - show) * 40f + gold * Mathf.Sin(Time * 5f) * 4f;
			if (FxKit.Alive((GodotObject)(object)_chevron))
			{
				_chevron.Points = new Vector2[3]
				{
					new Vector2(-18f, num5 - 16f),
					new Vector2(0f, num5),
					new Vector2(18f, num5 - 16f)
				};
				_chevron.DefaultColor = val;
				_chevron.Width = Mathf.Lerp(4.5f, 6.5f, gold);
			}
			if (FxKit.Alive((GodotObject)(object)_glow))
			{
				((Node2D)_glow).Scale = new Vector2(_size * 3.2f, _size * 3.2f) / 64f;
				((CanvasItem)_glow).Modulate = new Color(TheHuntFx.Gold.R, TheHuntFx.Gold.G, TheHuntFx.Gold.B, gold * (0.2f + 0.08f * Mathf.Sin(Time * 7.5f)));
			}
		}

		protected override void OnPlay(CardFxContext play)
		{
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			Node2D mark = _mark;
			_mark = null;
			if (FxKit.Alive((GodotObject)(object)mark) && FxKit.Alive((GodotObject)(object)play.FrontParent) && _show > 0.2f)
			{
				Vector2 globalPosition = mark.GlobalPosition;
				Node parent = ((Node)mark).GetParent();
				if (parent != null)
				{
					parent.RemoveChild((Node)(object)mark);
				}
				play.FrontParent.AddChild((Node)(object)mark, false, (InternalMode)0);
				mark.GlobalPosition = globalPosition;
				bool gold = _fatal;
				float from = _gold;
				Tween val = ((Node)mark).CreateTween();
				val.TweenMethod(Callable.From<float>((Action<float>)((float t) =>
				{
					//IL_00df: Unknown result type (might be due to invalid IL or missing references)
					//IL_010a: Unknown result type (might be due to invalid IL or missing references)
					//IL_007c: Unknown result type (might be due to invalid IL or missing references)
					if (FxKit.Alive((GodotObject)(object)mark))
					{
						if (gold)
						{
							Place(mark, 1f, 1f, t);
							((CanvasItem)mark).Modulate = new Color(1f + 1.2f * t, 1f + 1.2f * t, 1f + 0.6f * t, 1f - t * t);
						}
						else
						{
							Place(mark, 1f - 0.5f * t, from, 0f);
							mark.Scale = new Vector2(1f + 0.35f * t, 1f + 0.35f * t);
							((CanvasItem)mark).Modulate = new Color(1f, 1f, 1f, 1f - t);
						}
					}
				})), Variant.op_Implicit(0f), Variant.op_Implicit(1f), (double)(gold ? 0.16f : 0.22f)).SetTrans((TransitionType)4).SetEase((EaseType)1);
				val.TweenCallback(Callable.From((Action)(() =>
				{
					if (FxKit.Alive((GodotObject)(object)mark))
					{
						((Node)mark).QueueFree();
					}
				})));
			}
			Leave(30f);
		}

		protected override void OnEnd()
		{
			_mark = null;
		}
	}
