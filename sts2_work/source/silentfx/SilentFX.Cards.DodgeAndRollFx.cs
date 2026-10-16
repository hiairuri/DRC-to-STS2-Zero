// namespace: SilentFX.Cards
// type: DodgeAndRollFx (class)
// source lines: 48319-48559
	public sealed class DodgeAndRollFx : CardFx
	{
		internal static readonly Color Trace = new Color(0.7f, 0.9f, 1f, 1f);

		private static ulong _landsAtMsec;

		public override void OnPlay(CardFxContext c)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node parent = c.FrontParent;
				if (!FxKit.Alive((GodotObject)(object)parent))
				{
					return;
				}
				float away = 0f - c.Facing;
				Vector2 feet = c.Feet;
				Vector2 body = c.Body;
				Vector2 print = PrintPoint(feet, body, away);
				float num = body.Y * 0.46f;
				float num2 = Mathf.Max(0.08f, Tuning.F("roll.time", 0.3f));
				ulong num3 = FxClock.Milliseconds + (ulong)(num2 * 1000f);
				if (!MultiplayerFx.Active || !c.Remote)
				{
					_landsAtMsec = num3;
				}
				if (MultiplayerFx.Active)
				{
					CardModel card = c.Card;
					object obj;
					if (card == null)
					{
						obj = null;
					}
					else
					{
						Player owner = card.Owner;
						obj = ((owner != null) ? owner.Creature : null);
					}
					Creature val = (Creature)obj;
					if (val != null)
					{
						MultiplayerFx.For(val).RollLandsAt = num3;
					}
				}
				Brush.Dust(parent, feet + new Vector2(0f, -6f), 0f - away, 4, 0.6f);
				Tumble(parent, feet + new Vector2(0f, (0f - num) * 0.42f), print + new Vector2(0f, (0f - num) * 0.22f), body.Y * Tuning.F("roll.hop", 0.3f), num, 1f, num2);
				FxKit.After(parent, num2, () =>
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_004c: Unknown result type (might be due to invalid IL or missing references)
					Brush.Dust(parent, print, away, 7, 0.85f);
					FxKit.Flash(parent, print, new Color(Trace.R, Trace.G, Trace.B, 0.3f), body.X * 0.55f, 1.5f, 0.18);
				});
			}
			catch (Exception ex)
			{
				DevLog.Warn("BlueSkills.OnPlay: " + ex);
			}
		}

		internal static Vector2 PrintPoint(Vector2 feet, Vector2 body, float away)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			return feet + new Vector2(away * body.X * Tuning.F("roll.reach", 1f), -3f);
		}

		internal static float UntilLanding(Creature owner = null)
		{
			ulong milliseconds = FxClock.Milliseconds;
			if (MultiplayerFx.Active && owner != null)
			{
				ulong rollLandsAt = MultiplayerFx.For(owner).RollLandsAt;
				if (rollLandsAt > milliseconds)
				{
					return Mathf.Min(1f, (float)(rollLandsAt - milliseconds) / 1000f);
				}
				return 0f;
			}
			if (_landsAtMsec <= milliseconds)
			{
				return 0f;
			}
			return Mathf.Min(1f, (float)(_landsAtMsec - milliseconds) / 1000f);
		}

		internal static void Tumble(Node parent, Vector2 from, Vector2 to, float hop, float size, float strength, float time)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Expected Obj, but got Unknown
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)parent))
			{
				return;
			}
			Node2D val = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_Roll")
			};
			parent.AddChild((Node)(object)val, false, (InternalMode)0);
			if (!((Node)val).IsInsideTree())
			{
				((Node)val).QueueFree();
				return;
			}
			Sprite2D[] smears = new Sprite2D[4];
			for (int i = 0; i < 4; i++)
			{
				smears[i] = FxKit.Glow(new Color(Trace.R, Trace.G, Trace.B, 0f), new Vector2(64f, 64f));
				((Node)val).AddChild((Node)(object)smears[i], false, (InternalMode)0);
			}
			Sprite2D heart = FxKit.Glow(new Color(1f, 1f, 1f, 0f), new Vector2(64f, 64f));
			((Node)val).AddChild((Node)(object)heart, false, (InternalMode)0);
			float turn = ((to.X < from.X) ? (-1f) : 1f) * (float)Math.PI * (float)Mathf.Max(1, Tuning.I("roll.half_turns", 3));
			Vector2 touch = from.Lerp(to, 0.84f);
			float span = 1.255f;
			float num = Mathf.Max(0.08f, time) * span;
			FxKit.Drive((Node)(object)val, num, (float u) =>
			{
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0135: Unknown result type (might be due to invalid IL or missing references)
				//IL_015b: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_01af: Unknown result type (might be due to invalid IL or missing references)
				//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
				for (int j = 0; j < 4; j++)
				{
					Sprite2D val2 = smears[j];
					if (FxKit.Alive((GodotObject)(object)val2))
					{
						float num2 = Mathf.Clamp(u * span - 0.085f * (float)j, 0f, 1f);
						float num3 = Mathf.Lerp(num2, 1f - (1f - num2) * (1f - num2), 0.55f);
						float num4 = Mathf.Clamp((num2 - 0.8f) / 0.2f, 0f, 1f);
						Vector2 globalPosition = At(num3);
						float num5 = Mathf.Min(1f, num2 * 9f) * (1f - num4 * num4) * (1f - 0.22f * (float)j);
						((Node2D)val2).GlobalPosition = globalPosition;
						((Node2D)val2).GlobalRotation = turn * num3;
						((Node2D)val2).Scale = new Vector2(size * (1.25f + 0.55f * num4), size * (0.66f - 0.36f * num4)) / 64f;
						((CanvasItem)val2).Modulate = new Color(Trace.R, Trace.G, Trace.B, 0.5f * strength * num5);
						if (j == 0 && FxKit.Alive((GodotObject)(object)heart))
						{
							((Node2D)heart).GlobalPosition = globalPosition;
							((Node2D)heart).GlobalRotation = turn * num3;
							((Node2D)heart).Scale = new Vector2(size * (0.62f + 0.3f * num4), size * (0.3f - 0.14f * num4)) / 64f;
							((CanvasItem)heart).Modulate = new Color(1f, 1f, 1f, 0.55f * strength * num5);
						}
					}
				}
			}, (TransitionType)0, (EaseType)2);
			FxKit.FreeAfter((Node)(object)val, (double)num + 0.1);
			Vector2 At(float t)
			{
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0061: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Unknown result type (might be due to invalid IL or missing references)
				//IL_008a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				if (t < 0.76f)
				{
					float num2 = t / 0.76f;
					return from.Lerp(touch, num2) + new Vector2(0f, (0f - hop) * Mathf.Sin((float)Math.PI * num2));
				}
				float num3 = (t - 0.76f) / 0.24f;
				return touch.Lerp(to, num3) + new Vector2(0f, (0f - hop) * 0.2f * Mathf.Sin((float)Math.PI * num3));
			}
		}

		internal static void Echo(Node parent, Vector2 print, Vector2 feet, Vector2 chest, Vector2 body)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)parent))
			{
				float num = body.Y * 0.36f;
				float num2 = Mathf.Max(0.08f, Tuning.F("roll.echo_time", 0.24f));
				FxKit.Flash(parent, print, new Color(Trace.R, Trace.G, Trace.B, 0.55f), body.X * 0.5f, 1.6f, 0.16);
				Tumble(parent, print + new Vector2(0f, (0f - num) * 0.3f), feet + new Vector2(0f, (0f - body.Y) * 0.3f), body.Y * 0.16f, num, 0.75f, num2);
				FxKit.After(parent, num2 * 0.9f, () =>
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_002f: Unknown result type (might be due to invalid IL or missing references)
					FxKit.Flash(parent, chest, new Color(Trace.R, Trace.G, Trace.B, 0.42f), body.X * 0.75f, 1.45f, 0.2);
				});
			}
		}

		static DodgeAndRollFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		}
	}
