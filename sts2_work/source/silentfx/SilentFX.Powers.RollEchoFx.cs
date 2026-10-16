// namespace: SilentFX.Powers
// type: RollEchoFx (class)
// source lines: 2262-2405
	public sealed class RollEchoFx : BasicPowerFx
	{
		private Sprite2D _print;

		private Sprite2D _core;

		private float _away = -1f;

		private float _wait;

		private float _shown;

		private float _lit;

		protected override string NodeName => "RollEcho";

		protected override bool UseFront => false;

		protected override float FadeRate => 7f;

		protected override void Build(bool animateIn)
		{
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			_away = 0f - Anchor.Side();
			float wait;
			if (!animateIn)
			{
				wait = 0f;
			}
			else
			{
				object owner = Anchor.Owner;
				wait = DodgeAndRollFx.UntilLanding((Creature)((owner is Creature) ? owner : null));
			}
			_wait = wait;
			_shown = (animateIn ? 0f : 1f);
			Color trace = DodgeAndRollFx.Trace;
			_print = FxKit.Glow(new Color(trace.R, trace.G, trace.B, 0f), new Vector2(64f, 64f));
			_core = FxKit.Glow(new Color(1f, 1f, 1f, 0f), new Vector2(64f, 64f));
			((Node)Back).AddChild((Node)(object)_print, false, (InternalMode)0);
			((Node)Back).AddChild((Node)(object)_core, false, (InternalMode)0);
		}

		protected override void Tick(float dt)
		{
			if (_wait > 0f)
			{
				_wait -= dt;
				if (_wait <= 0f)
				{
					_lit = Mathf.Max(_lit, 0.55f);
				}
			}
			else
			{
				_shown = FxKit.Damp(_shown, 1f, 10f, dt);
			}
			_lit = FxKit.Damp(_lit, 0f, 4.5f, dt);
		}

		protected override void Layout()
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)_print) && FxKit.Alive((GodotObject)(object)_core))
			{
				Vector2 val = Anchor.SafeBodySize();
				Vector2 globalPosition = DodgeAndRollFx.PrintPoint(Anchor.Feet(), val, _away);
				Color trace = DodgeAndRollFx.Trace;
				float num = Tuning.F("roll.print_alpha", 0.2f);
				float num2 = 0.5f + 0.5f * Mathf.Sin(Time * 1.7f);
				float num3 = A * _shown;
				float num4 = val.X * (0.8f + 0.3f * _lit);
				((Node2D)_print).GlobalPosition = globalPosition;
				((Node2D)_print).Scale = new Vector2(num4, val.X * (0.17f + 0.08f * _lit)) / 64f;
				((CanvasItem)_print).Modulate = new Color(trace.R, trace.G, trace.B, Mathf.Clamp(num3 * (num * (0.75f + 0.25f * num2) + 0.65f * _lit), 0f, 1f));
				((Node2D)_core).GlobalPosition = globalPosition;
				((Node2D)_core).Scale = new Vector2(num4 * 0.5f, val.X * 0.05f) / 64f;
				((CanvasItem)_core).Modulate = new Color(1f, 1f, 1f, Mathf.Clamp(num3 * (num * 0.55f + 0.7f * _lit), 0f, 1f));
			}
		}

		protected override void OnStacked()
		{
			float wait = _wait;
			object owner = Anchor.Owner;
			_wait = Mathf.Max(wait, DodgeAndRollFx.UntilLanding((Creature)((owner is Creature) ? owner : null)));
			if (_wait <= 0f)
			{
				_lit = Mathf.Max(_lit, 0.55f);
			}
		}

		protected override void OnTrigger(Vector2? at)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			if (Anchor.Valid())
			{
				_wait = 0f;
				_shown = 1f;
				_lit = 1f;
				Vector2 body = Anchor.SafeBodySize();
				Vector2 feet = Anchor.Feet();
				DodgeAndRollFx.Echo(Anchor.FrontParent, DodgeAndRollFx.PrintPoint(feet, body, _away), feet, Anchor.Chest(), body);
			}
		}
	}
