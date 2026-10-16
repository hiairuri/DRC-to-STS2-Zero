// namespace: SilentFX.Cards
// type: StormOfSteelFx (class)
// source lines: 105275-105520
	public sealed class StormOfSteelFx : CardFx
	{
		private const float Split = 0.64f;

		private const float TrailSpan = 0.11f;

		public override void OnPlay(CardFxContext c)
		{
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Expected Obj, but got Unknown
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node frontParent = c.FrontParent;
				if (!FxKit.Alive((GodotObject)(object)frontParent))
				{
					return;
				}
				Gust(c);
				Sfx.Play("storm_gust");
				HeldSession held = c.Held;
				IShivSource held2 = held as IShivSource;
				if (held2 != null)
				{
					if (c.Card == null)
					{
						FxKit.After(frontParent, 0.35, () =>
						{
							ShivFx.PreviewTake(c, held2, ShivStyle.Storm, 16);
						});
					}
					return;
				}
				int n = (c.Remote ? SnapshotHandCount(c.Card, 16) : Mathf.Clamp(Tuning.I("storm.blades", 10), 4, 16));
				Node2D holder = new Node2D
				{
					Name = StringName.op_Implicit("SilentFX_StormOfSteel")
				};
				frontParent.AddChild((Node)(object)holder, false, (InternalMode)0);
				FxKit.FreeAfter((Node)(object)holder, Mathf.Max(0f, Tuning.F("storm.time", 1.05f)) + 0.28f);
				Node2D[] blades = new Node2D[n];
				Line2D[] ribbons = new Line2D[n];
				List<Vector2>[] trails = new List<Vector2>[n];
				List<float>[] trailAt = new List<float>[n];
				for (int num = 0; num < n; num++)
				{
					ribbons[num] = FxKit.Ribbon(7f, new Color(0.75f, 0.9f, 1f, 0.6f));
					((Node)holder).AddChild((Node)(object)ribbons[num], false, (InternalMode)0);
					trails[num] = new List<Vector2>();
					trailAt[num] = new List<float>();
				}
				for (int num2 = 0; num2 < n; num2++)
				{
					blades[num2] = FxKit.Blade(44f, 6f, new Color(0.94f, 0.97f, 1f, 1f), new Color(0.6f, 0.67f, 0.78f, 1f), new Color(0.34f, 0.27f, 0.22f, 1f), new Color(0.55f, 0.85f, 1f, 0.24f));
					((Node)holder).AddChild((Node)(object)blades[num2], false, (InternalMode)0);
					blades[num2].GlobalPosition = c.Chest;
				}
				Vector2 chest = c.Chest;
				Vector2 hand = (c.Remote ? c.WeaponHand : c.Hand);
				float reach = c.Body.X * 0.55f + 50f;
				float lift = c.Body.Y * 0.85f;
				float drop = c.Body.Y * 0.2f;
				float turns = Tuning.F("storm.turns", 9.5f);
				Tween val = FxKit.Drive((Node)(object)holder, Tuning.F("storm.time", 1.05f), (float t) =>
				{
					//IL_006d: Unknown result type (might be due to invalid IL or missing references)
					//IL_009a: Unknown result type (might be due to invalid IL or missing references)
					//IL_009f: Unknown result type (might be due to invalid IL or missing references)
					//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
					//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
					//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
					//IL_018f: Unknown result type (might be due to invalid IL or missing references)
					//IL_0169: Unknown result type (might be due to invalid IL or missing references)
					//IL_010a: Unknown result type (might be due to invalid IL or missing references)
					//IL_00db: Unknown result type (might be due to invalid IL or missing references)
					//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
					//IL_0102: Unknown result type (might be due to invalid IL or missing references)
					//IL_010f: Unknown result type (might be due to invalid IL or missing references)
					//IL_0113: Unknown result type (might be due to invalid IL or missing references)
					//IL_011a: Unknown result type (might be due to invalid IL or missing references)
					//IL_011f: Unknown result type (might be due to invalid IL or missing references)
					//IL_0123: Unknown result type (might be due to invalid IL or missing references)
					//IL_0125: Unknown result type (might be due to invalid IL or missing references)
					//IL_0127: Unknown result type (might be due to invalid IL or missing references)
					//IL_012c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0206: Unknown result type (might be due to invalid IL or missing references)
					//IL_020b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0210: Unknown result type (might be due to invalid IL or missing references)
					if (FxKit.Alive((GodotObject)(object)holder))
					{
						float num3 = Mathf.Clamp(t / 0.64f, 0f, 1f);
						float num4 = 1f - (1f - num3) * (1f - num3);
						for (int i = 0; i < n; i++)
						{
							float num5 = (float)Math.PI * 2f * (float)i / (float)n + num3 * turns;
							float num6 = 30f + reach * num4;
							Vector2 val2 = chest + new Vector2(Mathf.Cos(num5) * num6, Mathf.Sin(num5) * num6 * 0.3f + drop - lift * num4);
							Vector2 val3 = val2;
							float num7 = num5 + (float)Math.PI;
							if (t > 0.64f)
							{
								float num8 = (t - 0.64f) / 0.36f;
								Vector2 val4 = (c.Remote ? hand : (hand + new Vector2(((float)i - (float)(n - 1) * 0.5f) * 34f, 0f)));
								val3 = val2.Lerp(val4, num8 * num8);
								float num9 = num7;
								Vector2 val5 = val4 - val2;
								num7 = Mathf.LerpAngle(num9, val5.Angle() + (float)Math.PI / 2f, Mathf.Min(1f, num8 * 3f));
							}
							Node2D val6 = blades[i];
							if (FxKit.Alive((GodotObject)(object)val6))
							{
								val6.GlobalPosition = val3;
								val6.Rotation = num7;
							}
							List<Vector2> list = trails[i];
							List<float> list2 = trailAt[i];
							list.Add(val3);
							list2.Add(t);
							while (list.Count > 2 && (t - list2[0] > 0.11f || list.Count > 90))
							{
								list.RemoveAt(0);
								list2.RemoveAt(0);
							}
							if (FxKit.Alive((GodotObject)(object)ribbons[i]))
							{
								Vector2[] array = new Vector2[list.Count];
								for (int j = 0; j < list.Count; j++)
								{
									array[j] = holder.ToLocal(list[j]);
								}
								ribbons[i].Points = array;
							}
						}
					}
				}, (TransitionType)0, (EaseType)2);
				val.TweenProperty((GodotObject)(object)holder, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.18);
				val.TweenCallback(Callable.From((Action)(() =>
				{
					if (FxKit.Alive((GodotObject)(object)holder))
					{
						((Node)holder).QueueFree();
					}
				})));
			}
			catch (Exception ex)
			{
				DevLog.Warn("StormOfSteelFx.OnPlay: " + ex);
			}
		}

		internal static int SnapshotHandCount(CardModel card, int cap)
		{
			if (((card != null) ? card.Owner : null) == null)
			{
				return 0;
			}
			try
			{
				CardPile pile = PileTypeExtensions.GetPile((PileType)2, card.Owner);
				int num = 0;
				if (pile != null)
				{
					foreach (CardModel card2 in pile.Cards)
					{
						if (card2 != card)
						{
							num++;
						}
					}
				}
				return Mathf.Clamp(num, 0, cap);
			}
			catch (Exception ex)
			{
				DevLog.Warn("Storm of Steel: could not read the caster's model hand: " + ex.Message);
				return 0;
			}
		}

		private static void Gust(CardFxContext c)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			Node frontParent = c.FrontParent;
			Vector2 chest = c.Chest;
			Node2D gust = FxKit.Sigil(c.Body.X * 2f, new Color(0.8f, 0.92f, 1f, 1f), out var mat);
			frontParent.AddChild((Node)(object)gust, false, (InternalMode)0);
			FxKit.FreeAfter((Node)(object)gust, 0.699999988079071);
			gust.GlobalPosition = c.Feet;
			if (mat != null)
			{
				mat.SetShaderParameter(StringName.op_Implicit("radius"), Variant.op_Implicit(0.82f));
				mat.SetShaderParameter(StringName.op_Implicit("width"), Variant.op_Implicit(0.035f));
				mat.SetShaderParameter(StringName.op_Implicit("dashes"), Variant.op_Implicit(9f));
				mat.SetShaderParameter(StringName.op_Implicit("glow"), Variant.op_Implicit(0.4f));
				mat.SetShaderParameter(StringName.op_Implicit("alpha"), Variant.op_Implicit(0f));
			}
			FxKit.Drive((Node)(object)gust, 0.6f, (float t) =>
			{
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0080: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)gust))
				{
					float num = 0.4f + 1.2f * t;
					gust.Scale = new Vector2(num, num * 0.3f);
					if (FxKit.Alive((GodotObject)(object)mat))
					{
						mat.SetShaderParameter(StringName.op_Implicit("alpha"), Variant.op_Implicit(0.75f * (1f - t)));
						mat.SetShaderParameter(StringName.op_Implicit("spin"), Variant.op_Implicit(t * 5f));
					}
				}
			}, (TransitionType)4, (EaseType)1).TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)gust))
				{
					((Node)gust).QueueFree();
				}
			})));
			FxKit.Burst(frontParent, chest, new Color(0.85f, 0.93f, 1f, 0.9f), 22, 150f, 420f, 0.5, 0.08f, 0.2f, Vector2.Zero);
		}
	}
