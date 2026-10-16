// namespace: SilentFX.Cards
// type: BlurFx (class)
// source lines: 46849-46906
	public sealed class BlurFx : CardFx
	{
		internal static readonly Color Smear = new Color(0.62f, 0.82f, 1f, 0.42f);

		public override void OnPlay(CardFxContext c)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Smears(c.FrontParent, c.Chest, c.Body, 1f);
			}
			catch (Exception ex)
			{
				DevLog.Warn("BlueSkills.OnPlay: " + ex);
			}
		}

		internal static void Smears(Node parent, Vector2 chest, Vector2 body, float strength)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)parent))
			{
				Color color = new Color(Smear.R, Smear.G, Smear.B, Smear.A * strength);
				for (int i = -1; i <= 1; i++)
				{
					Vector2 val = chest + new Vector2(0f, (float)i * body.Y * 0.26f);
					float num = body.X * (1.25f - 0.2f * (float)Mathf.Abs(i));
					Brush.Cut(parent, val, val + new Vector2(num, 0f), body.Y * 0.3f, color, 0.03f * (float)(i + 1), 0.06f, 0.05f, 0.3f);
					Brush.Cut(parent, val, val - new Vector2(num, 0f), body.Y * 0.3f, color, 0.03f * (float)(i + 1), 0.06f, 0.05f, 0.3f);
				}
			}
		}

		static BlurFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		}
	}
