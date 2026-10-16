// namespace: SilentFX.Powers
// type: BlurHazeFx (class)
// source lines: 2200-2261
	public sealed class BlurHazeFx : BasicPowerFx
	{
		private readonly Sprite2D[] _streaks = new Sprite2D[4];

		protected override string NodeName => "Blur";

		protected override bool UseFront => false;

		protected override void Build(bool animateIn)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < _streaks.Length; i++)
			{
				_streaks[i] = FxKit.Glow(new Color(BlurFx.Smear.R, BlurFx.Smear.G, BlurFx.Smear.B, 0f), new Vector2(64f, 64f));
				((Node)Back).AddChild((Node)(object)_streaks[i], false, (InternalMode)0);
			}
		}

		protected override void Layout()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = Anchor.SafeBodySize();
			Vector2 val2 = Anchor.Chest();
			for (int i = 0; i < _streaks.Length; i++)
			{
				Sprite2D val3 = _streaks[i];
				if (FxKit.Alive((GodotObject)(object)val3))
				{
					float num = ((i % 2 == 0) ? 1f : (-1f));
					float num2 = (float)(i / 2) - 0.5f;
					float num3 = 0.5f + 0.5f * Mathf.Sin(Time * (1.3f + 0.25f * (float)i) + (float)i * 1.7f);
					((Node2D)val3).GlobalPosition = val2 + new Vector2(num * val.X * (0.35f + 0.4f * num3), num2 * val.Y * 0.42f);
					((Node2D)val3).Scale = new Vector2(val.X * (1f + 0.5f * num3 + 0.6f * Kick), val.Y * 0.3f) / 64f;
					((CanvasItem)val3).Modulate = new Color(BlurFx.Smear.R, BlurFx.Smear.G, BlurFx.Smear.B, A * (0.13f + 0.09f * (1f - num3) + 0.22f * Kick));
				}
			}
		}

		protected override void OnTrigger(Vector2? at)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			if (Anchor.Valid())
			{
				BlurFx.Smears(Anchor.FrontParent, Anchor.Chest(), Anchor.SafeBodySize(), 0.8f);
			}
		}
	}
