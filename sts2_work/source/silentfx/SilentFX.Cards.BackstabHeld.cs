// namespace: SilentFX.Cards
// type: BackstabHeld (class)
// source lines: 46431-46493
	public sealed class BackstabHeld : HeldSession
	{
		private ShadeMist _mist;

		private Sprite2D _glint;

		private float _show;

		internal static Vector2 Behind(CardFxContext c)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			return c.TargetChest + new Vector2((c.TargetWidth * 0.5f + 30f) * c.TargetSide, (0f - c.TargetHeight) * 0.04f);
		}

		protected override void Build()
		{
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			_mist = new ShadeMist(1, 2.7f);
			_mist.Set("wisp", 1.5f);
			_mist.Set("glow", 0.3f);
			((Node)Back).AddChild((Node)(object)_mist.Root, false, (InternalMode)0);
			_glint = FxKit.Glow(new Color(0.88f, 0.94f, 1f, 0f), new Vector2(64f, 64f));
			((Node)Front).AddChild((Node)(object)_glint, false, (InternalMode)0);
		}

		protected override void Tick(float dt)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			bool flag = C.HasTarget && !Playing;
			_show = FxKit.Damp(_show, flag ? 1f : 0f, 9f, dt);
			Vector2 val = Behind(C);
			float num = 170f * Strokes.Scale(C);
			if (_mist != null && FxKit.Alive((GodotObject)(object)_mist.Root))
			{
				_mist.Root.GlobalPosition = val;
				_mist.Size(num * (0.6f + 0.4f * _show), num * (0.6f + 0.4f * _show));
				_mist.Intensity = 0.9f * _show;
			}
			if (FxKit.Alive((GodotObject)(object)_glint))
			{
				float num2 = 0.6f + 0.4f * Mathf.Sin(Time * 6f);
				((Node2D)_glint).GlobalPosition = val + new Vector2((0f - C.TargetSide) * num * 0.12f, 0f);
				((Node2D)_glint).Scale = new Vector2(34f + 10f * num2, 9f + 3f * num2) / 64f;
				((CanvasItem)_glint).Modulate = new Color(0.88f, 0.94f, 1f, (0.5f + 0.4f * num2) * _show);
			}
		}

		protected override void OnPlay(CardFxContext play)
		{
			Leave(12f);
		}
	}
