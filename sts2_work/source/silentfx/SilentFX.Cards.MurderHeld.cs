// namespace: SilentFX.Cards
// type: MurderHeld (class)
// source lines: 80860-81021
	public sealed class MurderHeld : HeldSession
	{
		private ColorRect _veil;

		private Node2D _mark;

		private Sprite2D _glow;

		private Vector2 _at;

		private float _show;

		protected override void Build()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Expected Obj, but got Unknown
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			_veil = FxKit.Veil(new Color(0.1f, 0f, 0.02f, 0f));
			((Node)Back).AddChild((Node)(object)_veil, false, (InternalMode)0);
			Color blood = MurderFx.Blood;
			float num = Tuning.F("murder.mark_halfwidth", 44f);
			float num2 = Tuning.F("murder.mark_height", 24f);
			_mark = new Node2D();
			((Node)Front).AddChild((Node)(object)_mark, false, (InternalMode)0);
			_glow = FxKit.Glow(new Color(blood.R, blood.G, blood.B, 0.55f), new Vector2(num * 3.4f, num2 * 4.2f));
			((Node2D)_glow).Position = new Vector2(0f, (0f - num2) * 0.55f);
			((Node)_mark).AddChild((Node)(object)_glow, false, (InternalMode)0);
			Polygon2D val = FxKit.Shape(new Vector2[3]
			{
				new Vector2(0f - num, 0f - num2),
				new Vector2(num, 0f - num2),
				new Vector2(0f, 0f)
			}, new Color(blood.R, blood.G, blood.B, 0.92f));
			((CanvasItem)val).Material = (Material)(object)FxKit.Additive;
			((Node)_mark).AddChild((Node)(object)val, false, (InternalMode)0);
			Polygon2D val2 = FxKit.Shape(new Vector2[3]
			{
				new Vector2((0f - num) * 0.62f, (0f - num2) * 0.86f),
				new Vector2(num * 0.62f, (0f - num2) * 0.86f),
				new Vector2(0f, (0f - num2) * 0.24f)
			}, new Color(1f, 0.8f, 0.78f, 0.95f));
			((CanvasItem)val2).Material = (Material)(object)FxKit.Additive;
			((Node)_mark).AddChild((Node)(object)val2, false, (InternalMode)0);
			((CanvasItem)_mark).Modulate = new Color(1f, 1f, 1f, 0f);
			_at = C.Chest;
		}

		private Vector2 OverHead()
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			return new Vector2(C.TargetChest.X, C.TargetFeet.Y - C.TargetHeight - Tuning.F("murder.mark_gap", 78f));
		}

		protected override void Tick(float dt)
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)_veil))
			{
				_veil.Color = new Color(0.1f, 0f, 0.02f, Tuning.F("murder.held_dark", 0.4f) * (0.8f + 0.2f * Lift));
			}
			bool flag = C.HasTarget && !Playing;
			if (flag && !Hovering)
			{
				MurderFx.Dim(Mathf.Max(0.05f, Sfx.Knob("mood.murder.in", 0.6f)));
			}
			if (!Hovering && !Playing)
			{
				Sfx.Hold("murder_drone", 0.6f + 0.4f * _show);
			}
			if (C.HasTarget)
			{
				_at = OverHead();
			}
			_show = FxKit.Damp(_show, flag ? 1f : 0f, 12f, dt);
			if (FxKit.Alive((GodotObject)(object)_mark))
			{
				_mark.GlobalPosition = _at + new Vector2(0f, -26f * (1f - _show) + Mathf.Sin(Time * 4.2f) * 4f);
				float num = Mathf.Lerp(1.5f, 1f, _show);
				_mark.Scale = new Vector2(num, num);
				((CanvasItem)_mark).Modulate = new Color(1f, 1f, 1f, _show);
				if (FxKit.Alive((GodotObject)(object)_glow))
				{
					((CanvasItem)_glow).Modulate = new Color(MurderFx.Blood.R, MurderFx.Blood.G, MurderFx.Blood.B, 0.4f + 0.18f * Mathf.Sin(Time * 6f));
				}
			}
		}

		protected override void OnPlay(CardFxContext play)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
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
				((CanvasItem)mark).Modulate = new Color(2.2f, 2.2f, 2.2f, 1f);
				Tween val = ((Node)mark).CreateTween();
				val.SetParallel(true);
				val.TweenProperty((GodotObject)(object)mark, NodePath.op_Implicit("scale"), Variant.op_Implicit(new Vector2(1.7f, 1.7f)), 0.24).SetTrans((TransitionType)4).SetEase((EaseType)1);
				val.TweenProperty((GodotObject)(object)mark, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.24).SetTrans((TransitionType)4).SetEase((EaseType)0);
				FxKit.FreeAfter((Node)(object)mark, 0.3);
				if (play.HasTarget)
				{
					Brush.Dart(play.FrontParent, globalPosition, new Vector2(globalPosition.X, play.TargetChest.Y + play.TargetHeight * 0.2f), 180f, 14f, new Color(MurderFx.Blood.R, MurderFx.Blood.G, MurderFx.Blood.B, 0.9f), 0.11f);
				}
			}
			Leave(30f);
		}
	}
