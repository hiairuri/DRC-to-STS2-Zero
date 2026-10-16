// namespace: SilentFX.Cards
// type: PinpointFx (class)
// source lines: 83232-83307
	public sealed class PinpointFx : CardFx
	{
		internal static readonly Color Needle = new Color(1f, 0.93f, 0.7f, 1f);

		public override void OnHit(CardFxContext c, Vector2 at, bool killed)
		{
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			if (!MultiplayerFx.Active || MultiplayerFx.IsLocal(c.Card))
			{
				PinpointFocus.Struck++;
			}
			try
			{
				Node frontParent = c.FrontParent;
				if (FxKit.Alive((GodotObject)(object)frontParent))
				{
					float num = Strokes.Scale(c);
					int num2 = PinpointFocus.Steps(c.Card);
					float num3 = (float)num2 / 3f;
					Vector2 weaponHand = c.WeaponHand;
					Vector2 val = at - weaponHand;
					Vector2 val2 = (Vector2)((val.LengthSquared() > 16f) ? val.Normalized() : new Vector2((c.TargetSide < 0f) ? (-1f) : 1f, 0f));
					Vector2 to = at + val2 * (c.TargetWidth * 0.5f + 260f);
					float thick = PinpointFocus.Share(64f, 22f, num3) * num;
					Brush.Cut(frontParent, weaponHand, to, thick, new Color(Needle.R, Needle.G, Needle.B, Mathf.Lerp(0.22f, 0.4f, num3)), 0f, 0.03f, 0.03f, 0.16f);
					Brush.Cut(frontParent, weaponHand, to, PinpointFocus.Share(16f, 6f, num3) * num, new Color(1f, 0.98f, 0.9f, Mathf.Lerp(0.7f, 1f, num3)), 0f, 0.025f, 0.07f, 0.18f);
					if (num2 >= 3)
					{
						Brush.Cut(frontParent, weaponHand, to, 2.6f * num, new Color(1f, 1f, 1f, 1f), 0f, 0.02f, 0.14f, 0.24f);
					}
					FxKit.Flash(frontParent, at, new Color(1f, 0.98f, 0.9f, 1f), PinpointFocus.Share(80f, 44f, num3) * num, 1.5f, 0.14);
					FxKit.RingPulse(frontParent, at, PinpointFocus.Share(190f, 96f, num3) * num, Needle, 0f, 0.22f, 0.25f, 1f, 1f, 0f, 0.03f, Mathf.Lerp(0.45f, 0.9f, num3));
					Brush.Sparks(frontParent, at, Needle, 4 + num2 * 2, 380f, val2, 26f);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("PinpointFx.OnHit: " + ex);
			}
		}

		static PinpointFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		}
	}
