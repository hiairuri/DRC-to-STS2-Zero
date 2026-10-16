// namespace: SilentFX.Cards
// type: FadeFx (class)
// source lines: 47671-47698
	public sealed class FadeFx : CardFx
	{
		public override void OnPlay(CardFxContext c)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node frontParent = c.FrontParent;
				if (FxKit.Alive((GodotObject)(object)frontParent))
				{
					Vector2 at = (c.HasTarget ? c.TargetChest : c.Chest);
					float num = (c.HasTarget ? c.TargetWidth : c.Body.X);
					Brush.Motes(frontParent, at, new Color(0.7f, 1f, 0.78f, 0.8f), 14, num * 0.35f, 170f, 0.8);
					FxKit.Flash(frontParent, at, new Color(0.7f, 1f, 0.78f, 0.35f), num * 0.9f, 1.4f);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("BlueSkills.OnPlay: " + ex);
			}
		}
	}
