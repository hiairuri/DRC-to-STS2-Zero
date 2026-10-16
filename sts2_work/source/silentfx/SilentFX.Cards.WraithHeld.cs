// namespace: SilentFX.Cards
// type: WraithHeld (class)
// source lines: 117505-117632
	public sealed class WraithHeld : HeldSession
	{
		private static readonly StringName PIntensity = StringName.op_Implicit("intensity");

		private static readonly StringName PReach = StringName.op_Implicit("reach");

		private static readonly StringName PClock = StringName.op_Implicit("clock");

		private static readonly StringName PAspect = StringName.op_Implicit("aspect");

		private Node2D _root;

		private ShaderMaterial _haze;

		private WraithFigure _figure;

		private float _form;

		private float _wake;

		protected override void Build()
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected Obj, but got Unknown
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			if (!(Tuning.F("wraith.held.on", 1f) < 0.5f))
			{
				_root = new Node2D
				{
					Name = StringName.op_Implicit("SilentFX_WraithHeld")
				};
				((Node)Back).AddChild((Node)(object)_root, false, (InternalMode)0);
				_root.GlobalTransform = Transform2D.Identity;
				Vector2 view = ((C.View.X > 100f && C.View.Y > 100f) ? C.View : FxKit.ViewSize());
				WraithFormIntroFx.Seen(C.BackParent, view, out var lo, out var hi);
				Vector2 val = hi - lo;
				_haze = ShaderLib.NewMaterial("wraith_haze");
				if (_haze != null)
				{
					_haze.SetShaderParameter(PIntensity, Variant.op_Implicit(0f));
					_haze.SetShaderParameter(PAspect, Variant.op_Implicit(val.X / Mathf.Max(1f, val.Y)));
					ColorRect val2 = FxKit.Quad(val, (Material)(object)_haze);
					((Control)val2).Position = lo;
					((Node)_root).AddChild((Node)(object)val2, false, (InternalMode)0);
				}
				WraithFigure.Place(C, lo, hi, out var at, out var r);
				_figure = WraithFigure.Make((Node)(object)_root, at, r, lo, hi);
			}
		}

		protected override void Tick(float dt)
		{
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			bool flag = !Hovering && Armed && !Leaving;
			if (!Hovering && !Leaving && !Released)
			{
				Sfx.Hold("wraith_held", Mathf.Clamp(Lift, 0f, 1f) * (0.55f + 0.45f * _form));
			}
			_form = Mathf.Clamp(_form + dt / 0.85f * (flag ? 1f : (-1.5f)), 0f, 1f);
			_wake = Mathf.MoveToward(_wake, Leaving ? 0f : 1f, dt / 0.45f);
			float num = _form * _form * (3f - 2f * _form);
			if (FxKit.Alive((GodotObject)(object)_haze))
			{
				float num2 = Mathf.Clamp(Tuning.F("wraith.held.haze", 1f), 0f, 1.5f);
				_haze.SetShaderParameter(PIntensity, Variant.op_Implicit(num2 * _wake * (0.72f + 0.14f * Lift + 0.14f * num)));
				_haze.SetShaderParameter(PReach, Variant.op_Implicit(0.19f + 0.06f * Lift + 0.07f * num));
				_haze.SetShaderParameter(PClock, Variant.op_Implicit(Time));
			}
			WraithFigure figure = _figure;
			if (figure != null)
			{
				if (!figure.Alive)
				{
					_figure = null;
					return;
				}
				figure.Wake = _wake;
				figure.Reach = Mathf.Lerp(0.8f, 1.1f, Lift);
				figure.Form = _form;
				figure.Clock = Time;
				figure.Step();
			}
		}

		internal WraithFigure Take(out float form)
		{
			form = _form;
			WraithFigure figure = _figure;
			_figure = null;
			if (figure == null || !figure.Alive)
			{
				return null;
			}
			return figure;
		}

		protected override void OnPlay(CardFxContext play)
		{
			Leave(5f);
		}

		protected override void OnEnd()
		{
			_figure = null;
		}
	}
