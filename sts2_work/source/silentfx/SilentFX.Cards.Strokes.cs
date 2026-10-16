// namespace: SilentFX.Cards
// type: Strokes (class)
// source lines: 46358-46374
	internal static class Strokes
	{
		public static float Scale(CardFxContext c)
		{
			return Mathf.Max(Mathf.Max(c.TargetWidth, c.TargetHeight) / 260f, 1f);
		}

		public static float Across(CardFxContext c, float share = 0.9f)
		{
			return Mathf.Max(c.TargetWidth * share, 150f);
		}

		public static float Vertical(CardFxContext c, float reach)
		{
			return reach * Mathf.Min(1f, c.TargetHeight * 1.35f / Mathf.Max(1f, c.TargetWidth));
		}
	}
