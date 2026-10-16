// namespace: SilentFX.Cards
// type: ShadowmeldHeld (class)
// source lines: 90740-90813
	public sealed class ShadowmeldHeld : HeldSession
	{
		private Sprite2D _pool;

		private ShadeMist _climb;

		private ColorRect _dusk;

		protected override void Build()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			_dusk = FxKit.Veil(new Color(0.02f, 0f, 0.05f, 0f));
			((Node)Back).AddChild((Node)(object)_dusk, false, (InternalMode)0);
			_pool = Shade.Blob(new Vector2(64f, 64f), 0.8f);
			((Node)Back).AddChild((Node)(object)_pool, false, (InternalMode)0);
			_climb = new ShadeMist(0, 1.3f);
			_climb.Set("foot", 0.85f);
			_climb.Set("bulge", 0.3f);
			_climb.Set("taper", 1.2f);
			_climb.Set("wisp", 1.45f);
			_climb.Set("glow", 0.5f);
			_climb.Set("hollow", 0.45f);
			((Node)Front).AddChild((Node)(object)_climb.Root, false, (InternalMode)0);
		}

		protected override void Tick(float dt)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			Vector2 body = C.Body;
			Vector2 val = C.Feet + new Vector2(0f, (0f - body.Y) * Tuning.F("shadow.ground", 0.07f));
			float num = 1f + 0.06f * Mathf.Sin(Time * 2.4f);
			if (FxKit.Alive((GodotObject)(object)_dusk))
			{
				_dusk.Color = new Color(0.02f, 0f, 0.05f, Mathf.Clamp(Tuning.F("shadowmeld.dusk", 0.14f) * 0.5f * Arm, 0f, 0.4f));
			}
			if (FxKit.Alive((GodotObject)(object)_pool))
			{
				((Node2D)_pool).GlobalPosition = val + new Vector2(0f, 3f);
				((Node2D)_pool).Scale = new Vector2(body.X * (1.5f + 0.5f * Arm) * num, Mathf.Clamp(body.Y * 0.09f, 10f, 26f) * 3.4f) / 64f;
			}
			if (_climb != null && FxKit.Alive((GodotObject)(object)_climb.Root))
			{
				_climb.Root.GlobalPosition = val + new Vector2(0f, 5f);
				_climb.Size(body.X * (1.55f + 0.25f * Arm), Mathf.Max(8f, body.Y * (0.42f + 0.52f * Arm) * num));
				_climb.Intensity = 0.6f + 0.25f * Arm;
				_climb.Pulse = 0.25f * Arm;
			}
		}

		protected override void OnPlay(CardFxContext play)
		{
			Leave(14f);
		}
	}
