// namespace: SilentFX.Cards
// type: EscapePlanFx (class)
// source lines: 46907-46936
	public sealed class EscapePlanFx : CardFx
	{
		public override void OnPlay(CardFxContext c)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node frontParent = c.FrontParent;
				if (FxKit.Alive((GodotObject)(object)frontParent))
				{
					float num = Mathf.Clamp(c.Body.Y / 220f, 0.7f, 1.4f);
					FxKit.Flash(frontParent, c.Feet + new Vector2(0f, -12f), new Color(1f, 0.96f, 0.86f, 0.7f), 60f * num, 1.6f, 0.12);
					FxKit.Burst(frontParent, c.Feet + new Vector2(0f, -10f), new Color(0.8f, 0.82f, 0.86f, 0.5f), 16, 40f * num, 170f * num, 0.75, 1f * num, 2.2f * num, new Vector2(0f, -150f), 70f, new Vector2(0f, -1f), c.Body.X * 0.3f, 90f, additive: false, 0.9f);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("BlueSkills.OnPlay: " + ex);
			}
		}
	}
