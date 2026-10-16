// namespace: SilentFX.Cards
// type: HandTrickWipe (class)
// source lines: 67148-68193
	public static class HandTrickWipe
	{
		private sealed class Mark
		{
			public CardModel Card;

			public Vector2 Centre;

			public float Rotation;

			public Vector2 Scale = Vector2.One;

			public bool Seen;

			public bool Gilded;

			public Mark()
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			}
		}

		private sealed class Run
		{
			public CardModel Card;

			public NCard Real;

			public NCard Copy;

			public Sprite2D Aura;

			public CardFlight.Plan Plan;

			public int Phase;

			public float Time;

			public float Length;

			public float Clock;

			public float NoChoice;

			public CardRide.Pose From;

			public CardRide.Pose Now;

			public Vector2 Spot;

			public float Side;

			public float Tilt;

			public Vector2 Small;

			public Vector2 Pile;

			public bool Flying;

			public float Gone;

			public float Claim;

			public float Shown = 1f;

			public Vector2 Last;

			public readonly List<Mark> Marks = new List<Mark>();

			public readonly HashSet<CardModel> SlyBefore = new HashSet<CardModel>(ReferenceEqualityComparer.Instance);
		}

		private const int Aside = 0;

		private const int Waiting = 1;

		private const int Wiping = 2;

		private const int Over = 3;

		private const float Swoop = 0.36f;

		private const float Pass = 0.34f;

		private const float Reach = 0.9f;

		private const float Lean = 0.1f;

		private static readonly Color Gold = new Color(0.98f, 0.84f, 0.45f, 1f);

		private static readonly List<Run> Runs = new List<Run>();

		private static readonly List<Run> Scratch = new List<Run>();

		private static readonly List<HandCards.Entry> Hand = new List<HandCards.Entry>();

		private static bool _ticking;

		public static void Reset()
		{
			try
			{
				Run[] array = Runs.ToArray();
				for (int i = 0; i < array.Length; i++)
				{
					End(array[i]);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandTrickWipe.Reset: " + ex);
			}
			Runs.Clear();
			Scratch.Clear();
			Hand.Clear();
		}

		internal static bool Holds(CardModel card)
		{
			if (card == null)
			{
				return false;
			}
			foreach (Run run in Runs)
			{
				if (run.Card == card && run.Phase != 3)
				{
					return true;
				}
			}
			return false;
		}

		private static float Smooth(float t)
		{
			return t * t * (3f - 2f * t);
		}

		private static float BackOut(float t)
		{
			float num = t - 1f;
			return 1f + 2.3f * num * num * num + 1.3f * num * num;
		}

		private static float Pace()
		{
			float scale = HandPace.Scale;
			if (!float.IsFinite(scale))
			{
				return 1f;
			}
			return Mathf.Clamp(scale, 0.25f, 1.5f);
		}

		private static bool Shows(NCard node, CardModel card)
		{
			if (FxKit.Alive((GodotObject)(object)node) && ((Node)node).IsInsideTree())
			{
				return node.Model == card;
			}
			return false;
		}

		private static bool Peeking()
		{
			try
			{
				NPlayerHand instance = NPlayerHand.Instance;
				if (!FxKit.Alive((GodotObject)(object)instance) || !((Node)instance).IsInsideTree() || !instance.IsInCardSelection)
				{
					return false;
				}
				NPeekButton peekButton = instance.PeekButton;
				return FxKit.Alive((GodotObject)(object)peekButton) && peekButton.IsPeeking;
			}
			catch
			{
				return false;
			}
		}

		public static bool Begin(CardModel card)
		{
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0385: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Invalid comparison between Unknown and I4
			if (card == null || !HandPace.On || Tuning.F("handtrick.wipe", 1f) < 0.5f)
			{
				return false;
			}
			if (TimeFreeze.Active)
			{
				return false;
			}
			foreach (Run run2 in Runs)
			{
				if (run2.Card == card && (run2.Phase == 0 || run2.Phase == 1))
				{
					return true;
				}
			}
			for (int num = Runs.Count - 1; num >= 0; num--)
			{
				if (Runs[num].Card == card)
				{
					End(Runs[num]);
				}
			}
			CardPile pile = card.Pile;
			if (pile == null || (int)pile.Type != 5)
			{
				return false;
			}
			Node uiLayer = GameRefs.UiLayer;
			if (!FxKit.Alive((GodotObject)(object)uiLayer) || !uiLayer.IsInsideTree())
			{
				return false;
			}
			NCard val;
			try
			{
				val = NCard.FindOnTable(card, (PileType?)(PileType)5);
			}
			catch
			{
				val = null;
			}
			if (!Shows(val, card))
			{
				return false;
			}
			Run run = null;
			NCard val2 = null;
			try
			{
				CardRide.Finish(card);
				CardStandIn.Pose(val, out var centre, out var rotation, out var scale);
				val2 = CardStandIn.Make(card, uiLayer);
				if (!FxKit.Alive((GodotObject)(object)val2))
				{
					return false;
				}
				Vector2 val3 = FxKit.ViewSize();
				Vector2 val4 = GameRefs.PilePoint((PileType)3);
				float num2 = ((val4.X >= val3.X * 0.5f) ? 1f : (-1f));
				float num3 = Mathf.Clamp(Tuning.F("handtrick.aside_scale", 0.62f), 0.3f, 1f);
				CardRide.Pose pose = new CardRide.Pose
				{
					Position = centre,
					Rotation = rotation,
					Scale = scale,
					Alpha = 1f
				};
				Run mine = new Run
				{
					Card = card,
					Real = val,
					Copy = val2,
					Phase = 0,
					Length = Mathf.Clamp(Tuning.F("handtrick.aside_time", 0.24f), 0.08f, 0.8f) * Pace(),
					From = pose,
					Now = pose,
					Last = centre,
					Side = num2,
					Spot = new Vector2(val3.X * (0.5f - num2 * Mathf.Clamp(Tuning.F("handtrick.aside_x", 0.16f), 0f, 0.45f)), val3.Y * Mathf.Clamp(Tuning.F("handtrick.aside_y", 0.45f), 0.1f, 0.9f)),
					Tilt = (0f - num2) * Mathf.Clamp(Tuning.F("handtrick.aside_tilt", 0.14f), 0f, 0.6f),
					Small = new Vector2(num3, num3),
					Pile = val4
				};
				mine.Plan = new CardFlight.Plan
				{
					HideCard = true,
					HideTrail = true,
					Started = (NCard node, Vector2 start, Vector2 target) =>
					{
						//IL_0007: Unknown result type (might be due to invalid IL or missing references)
						FlightBegan(mine, node, target);
					}
				};
				run = mine;
				Runs.Add(run);
				HandCards.Fill(Hand);
				foreach (HandCards.Entry item in Hand)
				{
					if (IsSly(item.Card))
					{
						run.SlyBefore.Add(item.Card);
					}
				}
				CardStandIn.Place(val2, centre, rotation, scale);
				((CanvasItem)val2).Modulate = Colors.White;
				((CanvasItem)val2).ZIndex = 10;
				MakeAura(run, uiLayer);
				CardStandIn.Hide(val);
				Renew(run);
				if (!_ticking)
				{
					_ticking = true;
					Ticker.AddLate(Update);
				}
				return true;
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandTrickWipe.Begin: " + ex);
				if (run != null)
				{
					End(run);
				}
				else
				{
					CardStandIn.Free(val2);
					CardStandIn.Show(val);
				}
				return false;
			}
		}

		private static bool IsSly(CardModel card)
		{
			try
			{
				return card != null && card.IsSlyThisTurn;
			}
			catch
			{
				return false;
			}
		}

		private static void MakeAura(Run run, Node layer)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Sprite2D val = FxKit.Glow(new Color(Gold.R, Gold.G, Gold.B, 0f), new Vector2(64f, 64f));
				layer.AddChild((Node)(object)val, false, (InternalMode)0);
				if (!((Node)val).IsInsideTree())
				{
					((Node)val).QueueFree();
					return;
				}
				layer.MoveChild((Node)(object)val, ((Node)run.Copy).GetIndex(false));
				((CanvasItem)val).ZIndex = 10;
				run.Aura = val;
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandTrickWipe (aura): " + ex.Message);
			}
		}

		private static void Renew(Run run)
		{
			run.Claim = 1f;
			CardFlight.Claim(run.Card, run.Plan);
		}

		private static void FlightBegan(Run run, NCard node, Vector2 to)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (!Runs.Contains(run))
				{
					if (FxKit.Alive((GodotObject)(object)node))
					{
						((CanvasItem)node).Visible = true;
						((CanvasItem)node).Modulate = Colors.White;
					}
				}
				else
				{
					run.Flying = true;
					run.Pile = to;
					CardStandIn.Show(node);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandTrickWipe (flight): " + ex);
			}
		}

		internal static bool Chosen(AbstractModel source, IReadOnlyList<CardModel> cards)
		{
			try
			{
				if (source == null || cards == null)
				{
					return false;
				}
				foreach (Run run in Runs)
				{
					if ((object)run.Card != source || (run.Phase != 0 && run.Phase != 1))
					{
						continue;
					}
					foreach (CardModel card in cards)
					{
						if (card != null && !Marked(run, card))
						{
							run.Marks.Add(new Mark
							{
								Card = card
							});
						}
					}
					return true;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandTrickWipe.Chosen: " + ex);
			}
			return false;
		}

		private static bool Marked(Run run, CardModel card)
		{
			foreach (Mark mark in run.Marks)
			{
				if (mark.Card == card)
				{
					return true;
				}
			}
			return false;
		}

		private static void Update(double realDt)
		{
			if (Runs.Count == 0)
			{
				return;
			}
			try
			{
				float real = (float)realDt;
				float dt = (float)(realDt * (double)Mathf.Clamp((float)Engine.TimeScale, 0f, 4f));
				Scratch.Clear();
				Scratch.AddRange(Runs);
				foreach (Run item in Scratch)
				{
					if (Runs.Contains(item))
					{
						try
						{
							Step(item, real, dt);
						}
						catch (Exception ex)
						{
							DevLog.Warn("HandTrickWipe (frame): " + ex);
							End(item);
						}
					}
				}
				Scratch.Clear();
			}
			catch (Exception ex2)
			{
				DevLog.Warn("HandTrickWipe.Update: " + ex2);
				Reset();
			}
		}

		private static void Step(Run run, float real, float dt)
		{
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Invalid comparison between Unknown and I4
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Invalid comparison between Unknown and I4
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Invalid comparison between Unknown and I4
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Invalid comparison between Unknown and I4
			CardModel card = run.Card;
			run.Clock += dt;
			if (run.Phase == 3)
			{
				run.Time += dt;
				if (run.Flying)
				{
					Runs.Remove(run);
				}
				else if (!Shows(run.Real, card) || run.Time > 1.2f)
				{
					End(run);
				}
				return;
			}
			if (!Shows(run.Copy, card))
			{
				End(run);
				return;
			}
			CardPile pile = card.Pile;
			PileType? val = ((pile != null) ? new PileType?(pile.Type) : ((PileType?)null));
			run.NoChoice = ((run.Phase == 1 && !HandCards.Selecting) ? (run.NoChoice + dt) : 0f);
			if (run.NoChoice > 6f || GameRefs.CombatRoom == null)
			{
				End(run);
				return;
			}
			if (run.Phase != 2)
			{
				if ((run.Claim -= dt) <= 0f)
				{
					Renew(run);
				}
				if ((int)val.GetValueOrDefault() == 5)
				{
					if (!Shows(run.Real, card))
					{
						End(run);
						return;
					}
					if (((CanvasItem)run.Real).Modulate.A > 0.01f)
					{
						CardStandIn.Hide(run.Real);
					}
				}
				else if ((int)val.GetValueOrDefault() == 3 || (run.Flying && (int)val.GetValueOrDefault() == 1))
				{
					StartWipe(run);
				}
				else if ((int)val.GetValueOrDefault() != 1 || (run.Gone += dt) > 0.6f)
				{
					End(run);
					return;
				}
			}
			CardRide.Pose pose = run.Now;
			if (run.Phase == 0)
			{
				run.Time += dt;
				float num = Mathf.Clamp(run.Time / Mathf.Max(0.05f, run.Length), 0f, 1f);
				float num2 = Smooth(num);
				pose.Position = run.From.Position.Lerp(run.Spot, BackOut(num));
				pose.Rotation = Mathf.LerpAngle(run.From.Rotation, run.Tilt, num2);
				pose.Scale = run.From.Scale.Lerp(run.Small, num2);
				pose.Alpha = 1f;
				if (num >= 1f)
				{
					run.Phase = 1;
					run.Time = 0f;
				}
			}
			else if (run.Phase == 1)
			{
				run.Time += dt;
				float time = run.Time;
				pose.Position = run.Spot + new Vector2(0f, 7f * Mathf.Sin(time * 2.4f));
				pose.Rotation = run.Tilt + 0.035f * Mathf.Sin(time * 1.7f);
				pose.Scale = run.Small;
				pose.Alpha = 1f;
			}
			else
			{
				run.Time += dt;
				float num3 = Mathf.Clamp(run.Time / Mathf.Max(0.05f, run.Length), 0f, 1f);
				pose = WipePose(run, num3);
				if (num3 >= 1f)
				{
					Arrived(run);
					return;
				}
			}
			run.Shown = FxKit.Damp(run.Shown, (run.Phase != 2 && Peeking()) ? 0f : 1f, 14f, real);
			run.Now = pose;
			CardStandIn.Place(run.Copy, pose.Position, pose.Rotation, pose.Scale);
			((CanvasItem)run.Copy).Modulate = new Color(1f, 1f, 1f, Mathf.Clamp(pose.Alpha * run.Shown, 0f, 1f));
			Light(run, pose, dt);
			run.Last = pose.Position;
		}

		private static void Light(Run run, CardRide.Pose pose, float dt)
		{
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			Sprite2D aura = run.Aura;
			if (!FxKit.Alive((GodotObject)(object)aura))
			{
				return;
			}
			float num = Mathf.Clamp(pose.Alpha * run.Shown, 0f, 1f);
			if (run.Phase != 2)
			{
				float num2 = 0.78f + 0.22f * Mathf.Sin(run.Clock * 3.1f);
				float num3 = Mathf.Clamp(Tuning.F("handtrick.halo", 0.34f), 0f, 1f) * num2 * Mathf.Min(1f, run.Clock / 0.25f);
				((Node2D)aura).GlobalPosition = pose.Position;
				((Node2D)aura).GlobalRotation = pose.Rotation;
				((Node2D)aura).Scale = new Vector2(300f * pose.Scale.X * 1.55f, 422f * pose.Scale.Y * 1.35f) / 64f;
				((CanvasItem)aura).Modulate = new Color(Gold.R, Gold.G, Gold.B, num3 * num);
				return;
			}
			Vector2 val = pose.Position - run.Last;
			float num4 = val.Length();
			if (dt > 0.0005f && num4 > 0.5f)
			{
				Vector2 val2 = val / num4;
				float num5 = Mathf.Clamp(num4 / dt * 0.085f, 40f, 320f);
				((Node2D)aura).GlobalPosition = pose.Position - val2 * (num5 * 0.45f);
				((Node2D)aura).GlobalRotation = val2.Angle();
				((Node2D)aura).Scale = new Vector2(num5 * 1.4f, 422f * pose.Scale.Y * 0.8f) / 64f;
			}
			((CanvasItem)aura).Modulate = new Color(Gold.R, Gold.G, Gold.B, Mathf.Clamp(Tuning.F("handtrick.streak", 0.5f), 0f, 1f) * num);
		}

		private static void Look(Mark mark)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Invalid comparison between Unknown and I4
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (mark.Card == null)
				{
					return;
				}
				CardPile pile = mark.Card.Pile;
				if (pile == null || (int)pile.Type != 2)
				{
					return;
				}
				NCard val = HandCards.NodeOf(mark.Card);
				if (Shows(val, mark.Card) && ((CanvasItem)val).IsVisibleInTree())
				{
					CardStandIn.Pose(val, out var centre, out var rotation, out var scale);
					if (!(scale.X < 0.15f) && !(scale.Y < 0.15f))
					{
						mark.Centre = centre;
						mark.Rotation = rotation;
						mark.Scale = scale;
						mark.Seen = true;
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandTrickWipe (look): " + ex.Message);
			}
		}

		private static void StartWipe(Run run)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			run.Phase = 2;
			run.Time = 0f;
			run.From = run.Now;
			if (!run.Flying)
			{
				run.Pile = GameRefs.PilePoint((PileType)3);
			}
			if (run.Marks.Count == 0)
			{
				HandCards.Fill(Hand);
				foreach (HandCards.Entry item in Hand)
				{
					if (item.Card != null && !run.SlyBefore.Contains(item.Card) && IsSly(item.Card))
					{
						run.Marks.Add(new Mark
						{
							Card = item.Card
						});
					}
				}
			}
			for (int num = run.Marks.Count - 1; num >= 0; num--)
			{
				Mark mark = run.Marks[num];
				Look(mark);
				if (!mark.Seen)
				{
					SlyGold.Gild(mark.Card, run.Side, 0f);
					run.Marks.RemoveAt(num);
				}
			}
			float side = run.Side;
			run.Marks.Sort((Mark a, Mark b) => (a.Centre.X * side).CompareTo(b.Centre.X * side));
			float num2 = Pace();
			if (run.Marks.Count == 0)
			{
				run.Length = Mathf.Clamp(Tuning.F("handtrick.leave_time", 0.36f), 0.12f, 1.2f) * num2;
			}
			else
			{
				run.Length = Mathf.Clamp(Tuning.F("handtrick.wipe_time", 0.62f), 0.25f, 1.5f) * num2 * ((run.Marks.Count > 1) ? 1.25f : 1f);
			}
		}

		private static CardRide.Pose WipePose(Run run, float t)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_046e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_04af: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0500: Unknown result type (might be due to invalid IL or missing references)
			//IL_050a: Unknown result type (might be due to invalid IL or missing references)
			//IL_050f: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03da: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_040e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0415: Unknown result type (might be due to invalid IL or missing references)
			//IL_041a: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_0583: Unknown result type (might be due to invalid IL or missing references)
			//IL_058a: Unknown result type (might be due to invalid IL or missing references)
			//IL_058f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0594: Unknown result type (might be due to invalid IL or missing references)
			//IL_0598: Unknown result type (might be due to invalid IL or missing references)
			CardRide.Pose pose = run.From;
			Vector2 pile = run.Pile;
			float side = run.Side;
			CardRide.Pose result = new CardRide.Pose
			{
				Alpha = 1f
			};
			foreach (Mark mark3 in run.Marks)
			{
				Look(mark3);
			}
			int count = run.Marks.Count;
			if (count == 0)
			{
				float num = t * t * (1.7f - 0.7f * t);
				Vector2 b = pose.Position.Lerp(pile, 0.35f) + new Vector2(0f, -110f);
				result.Position = FxKit.Bezier(pose.Position, b, pile, num);
				result.Rotation = pose.Rotation + side * 1.3f * num;
				result.Scale = pose.Scale.Lerp(pose.Scale * 0.25f, num * num);
				result.Alpha = ((t < 0.75f) ? 1f : (1f - (t - 0.75f) / 0.25f));
				return result;
			}
			Mark mark = run.Marks[0];
			Mark mark2 = run.Marks[count - 1];
			Vector2 val = new Vector2(side, 0f);
			Vector2 val2 = val.Rotated(mark.Rotation);
			val = new Vector2(side, 0f);
			Vector2 val3 = val.Rotated(mark2.Rotation);
			Vector2 val4 = mark.Centre - val2 * (300f * mark.Scale.X * 0.9f);
			Vector2 val5 = mark2.Centre + val3 * (300f * mark2.Scale.X * 0.9f);
			Vector2 val6 = val5 - val4;
			float num2 = val6.Length();
			Vector2 val7 = ((num2 > 1f) ? (val6 / num2) : val2);
			float num3 = num2 / Mathf.Max(0.02f, 0.34f * run.Length);
			float num4 = side * 0.1f;
			Vector2 val8 = mark.Scale * 1.03f;
			if (t < 0.36f)
			{
				float num5 = t / 0.36f;
				float t2 = Mathf.Pow(num5, 1.6f);
				float num6 = pose.Position.DistanceTo(val4);
				float num7 = Mathf.Clamp(num3 * 0.36f * run.Length / 3.2f, 30f, Mathf.Max(30f, num6 * 0.6f));
				result.Position = FxKit.Bezier(pose.Position, val4 - val7 * num7, val4, t2);
				if (num6 > 1f)
				{
					ref Vector2 position = ref result.Position;
					position -= (val4 - pose.Position) / num6 * (16f * Mathf.Sin((float)Math.PI * Mathf.Clamp(num5 / 0.35f, 0f, 1f)) * (1f - num5));
				}
				float num8 = Smooth(num5);
				result.Rotation = Mathf.LerpAngle(pose.Rotation, mark.Rotation + num4, num8);
				result.Scale = pose.Scale.Lerp(val8, num8);
			}
			else if (t < 0.70000005f)
			{
				float num9 = (t - 0.36f) / 0.34f;
				result.Position = val4.Lerp(val5, num9);
				result.Rotation = Mathf.LerpAngle(mark.Rotation, mark2.Rotation, num9) + num4;
				Vector2 val9 = mark.Scale.Lerp(mark2.Scale, num9) * 1.03f;
				result.Scale = new Vector2(val9.X * (1f + 0.06f * Mathf.Sin((float)Math.PI * num9)), val9.Y);
			}
			else
			{
				float num10 = Mathf.Max(0.05f, 0.29999998f);
				float num11 = Mathf.Clamp((t - 0.36f - 0.34f) / num10, 0f, 1f);
				float num12 = 0.5f * num11 + 0.5f * num11 * num11;
				float num13 = val5.DistanceTo(pile);
				float num14 = Mathf.Clamp(num3 * num10 * run.Length, 30f, Mathf.Max(30f, num13 * 0.7f));
				result.Position = FxKit.Bezier(val5, val5 + val7 * num14, pile, num12);
				result.Rotation = mark2.Rotation + num4 + side * 1.2f * num12;
				val = mark2.Scale * 1.03f;
				result.Scale = val.Lerp(mark2.Scale * 0.22f, num11 * num11);
				result.Alpha = ((num11 < 0.7f) ? 1f : (1f - (num11 - 0.7f) / 0.3f));
			}
			if (t >= 0.324f)
			{
				foreach (Mark mark4 in run.Marks)
				{
					if (!mark4.Gilded)
					{
						float num15 = 300f * mark4.Scale.X;
						val = result.Position - mark4.Centre;
						if (!(val.Dot(val7) < (0f - num15) * 0.9f - 4f))
						{
							mark4.Gilded = true;
							SlyGold.Gild(mark4.Card, side, Mathf.Clamp(1.8f * num15 / Mathf.Max(60f, num3), 0.05f, 0.5f));
						}
					}
				}
			}
			return result;
		}

		private static void Arrived(Run run)
		{
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				foreach (Mark mark in run.Marks)
				{
					if (!mark.Gilded)
					{
						SlyGold.Gild(mark.Card, run.Side, 0f);
					}
				}
				Node uiLayer = GameRefs.UiLayer;
				if (FxKit.Alive((GodotObject)(object)uiLayer) && uiLayer.IsInsideTree())
				{
					FxKit.Flash(uiLayer, run.Pile, new Color(Gold.R, Gold.G, Gold.B, 0.45f), 70f, 1.8f, 0.14);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandTrickWipe (arrival): " + ex.Message);
			}
			FreeCopy(run);
			if (run.Flying)
			{
				Runs.Remove(run);
				return;
			}
			run.Phase = 3;
			run.Time = 0f;
		}

		private static void FreeCopy(Run run)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (FxKit.Alive((GodotObject)(object)run.Copy) && run.Copy.Model == run.Card)
				{
					((CanvasItem)run.Copy).Modulate = Colors.White;
					((CanvasItem)run.Copy).ZIndex = 0;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandTrickWipe (copy): " + ex.Message);
			}
			CardStandIn.Free(run.Copy);
			run.Copy = null;
			try
			{
				if (FxKit.Alive((GodotObject)(object)run.Aura))
				{
					((Node)run.Aura).QueueFree();
				}
			}
			catch (Exception ex2)
			{
				DevLog.Warn("HandTrickWipe (aura): " + ex2.Message);
			}
			run.Aura = null;
		}

		private static void End(Run run)
		{
			if (run == null)
			{
				return;
			}
			Runs.Remove(run);
			FreeCopy(run);
			if (run.Flying)
			{
				return;
			}
			try
			{
				CardFlight.Release(run.Card);
				CardStandIn.Show(run.Real);
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandTrickWipe (restore): " + ex);
			}
		}

		static HandTrickWipe()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		}
	}
