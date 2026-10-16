// namespace: SilentFX.Cards
// type: AcrobaticsFx (class)
// source lines: 46748-46790
	public sealed class AcrobaticsFx : CardFx
	{
		private static readonly Color Air = new Color(0.66f, 0.96f, 1f, 0.6f);

		public override void OnPlay(CardFxContext c)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node frontParent = c.FrontParent;
				if (!FxKit.Alive((GodotObject)(object)frontParent))
				{
					return;
				}
				Vector2 feet = c.Feet;
				float radius = c.Body.X * 0.62f;
				float height = c.Body.Y * 1.45f;
				for (int i = 0; i < 3; i++)
				{
					float phase = (float)i * ((float)Math.PI * 2f) / 3f;
					Brush.Follow(frontParent, (float t) =>
					{
						//IL_0006: Unknown result type (might be due to invalid IL or missing references)
						//IL_006c: Unknown result type (might be due to invalid IL or missing references)
						//IL_0071: Unknown result type (might be due to invalid IL or missing references)
						return feet + new Vector2(Mathf.Cos(t * 7.5f + phase) * radius * (1f - 0.35f * t), (0f - t) * height + Mathf.Sin(t * 7.5f + phase) * radius * 0.22f);
					}, 0.22f, 15f, Air, 0.34f, 0.04f * (float)i);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("BlueSkills.OnPlay: " + ex);
			}
		}

		static AcrobaticsFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		}
	}
