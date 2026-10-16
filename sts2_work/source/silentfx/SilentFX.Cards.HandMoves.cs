// namespace: SilentFX.Cards
// type: HandMoves (class)
// source lines: 64050-67090
	public static class HandMoves
	{
		private sealed class Streak
		{
			public Sprite2D Glow;

			public Vector2 Last;

			public float LastT;

			public bool Made;
		}

		private sealed class Act
		{
			public CardModel Card;

			public NCard Real;

			public NCard Copy;

			public Node Home;

			public CardRide.Spec Spec;

			public CardRide.Pose From;

			public float Duration;

			public float Elapsed;
		}

		private sealed class Whirl
		{
			public Vector2 Chest;

			public Vector2 Radii;

			public Node Fight;

			public float Height;

			public float Time;

			public float Small;

			public int Ghosts;

			public bool Closed;

			public bool Made;

			public Line2D Tail;

			public Line2D Core;

			public readonly List<Vector2> Path = new List<Vector2>();

			public readonly List<float> Stamp = new List<float>();
		}

		private sealed class Mix
		{
			public HandFlow.DrawInfo Draw;

			public float Start;

			public float Gap;

			public float[] Leave;
		}

		private sealed class Ghost
		{
			public NHandCardHolder Holder;

			public float Time;

			public Vector2 At;

			public float Turn;

			public float LastT;

			public bool Placed;

			public float Pulse;

			public float Solid;

			public bool Made;

			public Sprite2D Glow;
		}

		private sealed class Hopper
		{
			public NCard Node;

			public CardModel Card;

			public Node Home;

			public Vector2 Moved;

			public float Turned;

			public float Out;

			public float Speed;

			public float Lean;

			public float Clock;

			public float Phase;

			public float Wait;

			public bool Wanted;
		}

		private sealed class Watch
		{
			public CardModel Card;

			public float For;
		}

		private sealed class Hidden
		{
			public NCard Node;

			public CardModel Card;

			public float For;
		}

		private sealed class Tie
		{
			public HandFlow.DrawInfo Draw;

			public readonly HashSet<CardModel> Cards = new HashSet<CardModel>(ReferenceEqualityComparer.Instance);

			public bool Ended;

			public float Age;

			public float Quiet;
		}

		private sealed class Stay
		{
			public NCard Node;

			public CardModel Card;

			public Node2D Root;

			public Sprite2D Sheen;

			public float T;

			public bool Duck;

			public float Down;

			public float Speed;

			public Vector2 Nudge;
		}

		private sealed class Under
		{
			public Sprite2D Glint;

			public float FromX;

			public float ToX;
		}

		private const float Land = 0.85f;

		private const float CircleIn = 0.275f;

		private const float CircleOut = 0.725f;

		private const float Depth = 0.14f;

		private static readonly Color Gold = new Color(0.98f, 0.84f, 0.45f, 1f);

		private static readonly Color Teal = new Color(0.42f, 1f, 0.88f, 1f);

		private static readonly Color Pale = new Color(0.9f, 0.98f, 1f, 1f);

		private static readonly Color Wraith = new Color(0.7f, 0.86f, 1f, 1f);

		private static readonly Type[] Discarders = new Type[6]
		{
			typeof(Survivor),
			typeof(Acrobatics),
			typeof(Prepared),
			typeof(CalculatedGamble),
			typeof(DaggerThrow),
			typeof(HiddenDaggers)
		};

		private static readonly List<Stay> Stays = new List<Stay>();

		private static readonly Color StayBlue = new Color(0.4f, 0.74f, 1f, 1f);

		private static readonly List<HandCards.Entry> Hand = new List<HandCards.Entry>();

		private static readonly List<Act> Acts = new List<Act>();

		private static readonly Dictionary<NCard, Hopper> Hoppers = new Dictionary<NCard, Hopper>(ReferenceEqualityComparer.Instance);

		private static readonly List<Hopper> HopScratch = new List<Hopper>();

		private static readonly Dictionary<CardModel, NCard> SlyFlying = new Dictionary<CardModel, NCard>(ReferenceEqualityComparer.Instance);

		private static readonly List<Watch> Watches = new List<Watch>();

		private static readonly List<Hidden> Vanished = new List<Hidden>();

		private static readonly List<Hidden> Covered = new List<Hidden>();

		private static readonly List<Tie> Ties = new List<Tie>();

		private static Vector2[] _cardShape;

		private static Mix _mix;

		private static float _mixGap = -1f;

		private static HandFlow.DrawInfo _ghostDraw;

		private static float _ghostFor;

		private static CardModel _carried;

		private static bool _carriedDiscards;

		private static bool _picking;

		private static float _pickLost;

		private static bool _discarding;

		private static float _discardFor;

		private static float _clock;

		private static bool _ready;

		private const float UnderDive = 0.24f;

		private const float UnderRise = 0.58f;

		public static void Init()
		{
			try
			{
				if (!_ready)
				{
					_ready = true;
					CardShape();
					HandFlow.DrawBegins += OnDrawBegins;
					HandFlow.DrawnCardEntered += OnDrawnCardEntered;
					HandFlow.DrawEnded += OnDrawEnded;
					HandFlow.DiscardBegins += OnDiscardBegins;
					HandFlow.DiscardEnded += OnDiscardEnded;
					HandFlow.SlyPlayBegins += OnSlyPlayBegins;
					HandFlow.PlayerTurnBegins += OnPlayerTurnBegins;
					HandFlow.TurnEnding += OnTurnEnding;
					HandFlow.HandDrawBegins += OnHandDrawBegins;
					HandFlow.SelectionBegan += OnSelectionBegan;
					HandFlow.SelectionEnded += OnSelectionEnded;
					Ticker.Add(Update);
					DevLog.Info("HandMoves ready");
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("HandMoves.Init: " + ex);
			}
		}

		public static void Reset()
		{
			try
			{
				_carried = null;
				_carriedDiscards = false;
				_picking = false;
				_discarding = false;
				SettleAll();
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandMoves.Reset (hop): " + ex);
			}
			try
			{
				_mix = null;
				EndAllActs();
			}
			catch (Exception ex2)
			{
				DevLog.Warn("HandMoves.Reset (acts): " + ex2);
			}
			try
			{
				_ghostDraw = null;
				foreach (Hidden item in Covered)
				{
					CardStandIn.Show(item.Node);
				}
				Covered.Clear();
			}
			catch (Exception ex3)
			{
				DevLog.Warn("HandMoves.Reset (ghost): " + ex3);
			}
			try
			{
				SlyFlying.Clear();
				Watches.Clear();
				foreach (Hidden item2 in Vanished)
				{
					CardStandIn.Show(item2.Node);
				}
				Vanished.Clear();
			}
			catch (Exception ex4)
			{
				DevLog.Warn("HandMoves.Reset (sly): " + ex4);
			}
			FreeTies();
		}

		private static Color With(Color c, float alpha)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			return new Color(c.R, c.G, c.B, alpha);
		}

		private static float Smooth(float t)
		{
			return t * t * (3f - 2f * t);
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

		private static float Seconds(string key, float def)
		{
			return Mathf.Clamp(Tuning.F(key, def), 0.08f, 1.5f) * Pace();
		}

		private static float Side(in CardRide.Pose a, in CardRide.Pose b)
		{
			if (!(b.Position.X >= a.Position.X))
			{
				return -1f;
			}
			return 1f;
		}

		private static bool Sly(CardModel card)
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

		private static bool Shows(NCard node, CardModel card)
		{
			if (FxKit.Alive((GodotObject)(object)node) && ((Node)node).IsInsideTree())
			{
				return node.Model == card;
			}
			return false;
		}

		private static Vector2[] CardShape()
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			if (_cardShape != null)
			{
				return _cardShape;
			}
			Vector2[] array = new Vector2[4]
			{
				new Vector2(126f, -187f),
				new Vector2(126f, 187f),
				new Vector2(-126f, 187f),
				new Vector2(-126f, -187f)
			};
			List<Vector2> list = new List<Vector2>();
			for (int i = 0; i < 4; i++)
			{
				float num = -(float)Math.PI / 2f + (float)i * (float)Math.PI * 0.5f;
				for (int j = 0; j <= 4; j++)
				{
					float num2 = num + (float)Math.PI / 2f * (float)j / 4f;
					list.Add(array[i] + new Vector2(Mathf.Cos(num2), Mathf.Sin(num2)) * 24f);
				}
			}
			_cardShape = list.ToArray();
			return _cardShape;
		}

		private static bool SlotOf(NHandCardHolder holder, out Vector2 centre, out float angle)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			centre = Vector2.Zero;
			angle = 0f;
			if (!FxKit.Alive((GodotObject)(object)holder) || !((Node)holder).IsInsideTree())
			{
				return false;
			}
			Node parent = ((Node)holder).GetParent();
			CanvasItem val = (CanvasItem)(object)((parent is CanvasItem) ? parent : null);
			if (val == null || !FxKit.Alive((GodotObject)(object)val))
			{
				return false;
			}
			Transform2D globalTransform = val.GetGlobalTransform();
			centre = globalTransform * holder.TargetPosition;
			angle = globalTransform.Rotation + Mathf.DegToRad(holder.TargetAngle);
			return true;
		}

		private static void Trail(Streak s, NCard copy, Vector2 at, float t, float seconds, Color color, float thick, float longest)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			if (!s.Made)
			{
				s.Made = true;
				s.Last = at;
				s.LastT = t;
				Node val = (FxKit.Alive((GodotObject)(object)copy) ? ((Node)copy).GetParent() : null);
				if (FxKit.Alive((GodotObject)(object)val))
				{
					Sprite2D val2 = FxKit.Glow(new Color(color.R, color.G, color.B, 0f), new Vector2(64f, 64f));
					val.AddChild((Node)(object)val2, false, (InternalMode)0);
					if (!((Node)val2).IsInsideTree())
					{
						((Node)val2).QueueFree();
						return;
					}
					val.MoveChild((Node)(object)val2, ((Node)copy).GetIndex(false));
					FxKit.FreeAfter((Node)(object)val2, seconds + 0.12f);
					s.Glow = val2;
				}
			}
			else if (FxKit.Alive((GodotObject)(object)s.Glow))
			{
				float num = (t - s.LastT) * seconds;
				Vector2 val3 = at - s.Last;
				float num2 = val3.Length();
				s.Last = at;
				s.LastT = t;
				float num3 = Mathf.Clamp(Mathf.Min(t * 6f, (1f - t) * 4f), 0f, 1f);
				((CanvasItem)s.Glow).Modulate = new Color(color.R, color.G, color.B, color.A * num3);
				if (!(num < 0.0005f) && !(num2 < 0.5f))
				{
					Vector2 val4 = val3 / num2;
					float num4 = Mathf.Clamp(num2 / num * 0.075f, 24f, longest);
					((Node2D)s.Glow).GlobalPosition = at - val4 * (num4 * 0.5f);
					((Node2D)s.Glow).GlobalRotation = val4.Angle();
					((Node2D)s.Glow).Scale = new Vector2(num4 * 1.3f, Mathf.Max(1f, thick)) / 64f;
				}
			}
		}

		private static bool Perform(NCard real, CardRide.Spec spec)
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Invalid comparison between Unknown and I4
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			Act act = null;
			NCard val = null;
			try
			{
				if (spec == null || !FxKit.Alive((GodotObject)(object)real) || !((Node)real).IsInsideTree() || real.Model == null)
				{
					return false;
				}
				CardModel model = real.Model;
				CardPile pile = model.Pile;
				if (pile == null || (int)pile.Type != 2 || HeldFxManager.Carried == model)
				{
					return false;
				}
				Node uiLayer = GameRefs.UiLayer;
				if (!FxKit.Alive((GodotObject)(object)uiLayer) || !uiLayer.IsInsideTree())
				{
					return false;
				}
				CardRide.Finish(model);
				EndActs(model);
				CardStandIn.Pose(real, out var centre, out var rotation, out var scale);
				CardRide.Pose pose = spec.From ?? new CardRide.Pose
				{
					Position = centre,
					Rotation = rotation,
					Scale = scale,
					Alpha = 1f
				};
				val = CardStandIn.Make(model, uiLayer);
				if (!FxKit.Alive((GodotObject)(object)val))
				{
					return false;
				}
				act = new Act
				{
					Card = model,
					Real = real,
					Copy = val,
					Home = ((Node)real).GetParent(),
					Spec = spec,
					From = pose,
					Duration = (float.IsFinite(spec.Duration) ? Mathf.Clamp(spec.Duration, 0.05f, 1.6f) : 0.3f)
				};
				Acts.Add(act);
				CardStandIn.Hide(real);
				CardStandIn.Place(val, pose.Position, pose.Rotation, pose.Scale);
				((CanvasItem)val).Modulate = new Color(1f, 1f, 1f, pose.Alpha);
				((CanvasItem)val).ZIndex = 10;
				spec.Started?.Invoke(val);
				return true;
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandMoves.Perform: " + ex);
				if (act != null)
				{
					EndAct(act);
				}
				else
				{
					CardStandIn.Free(val);
				}
				CardStandIn.Show(real);
				return false;
			}
		}

		private static bool Performing(CardModel card)
		{
			foreach (Act act in Acts)
			{
				if (act.Card == card)
				{
					return true;
				}
			}
			return false;
		}

		private static bool IsCopy(NCard node)
		{
			foreach (Act act in Acts)
			{
				if (act.Copy == node)
				{
					return true;
				}
			}
			return false;
		}

		private static void EndAct(Act act)
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			if (act == null)
			{
				return;
			}
			Acts.Remove(act);
			try
			{
				if (FxKit.Alive((GodotObject)(object)act.Copy) && act.Copy.Model == act.Card)
				{
					((CanvasItem)act.Copy).Modulate = Colors.White;
					((CanvasItem)act.Copy).ZIndex = 0;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandMoves (act, tint): " + ex.Message);
			}
			CardStandIn.Free(act.Copy);
			CardStandIn.Show(act.Real);
		}

		private static void EndActs(CardModel card)
		{
			if (card == null)
			{
				return;
			}
			Act[] array = Acts.ToArray();
			foreach (Act act in array)
			{
				if (act.Card == card)
				{
					EndAct(act);
				}
			}
		}

		private static void EndAllActs()
		{
			Act[] array = Acts.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				EndAct(array[i]);
			}
			Acts.Clear();
		}

		private static void StepActs(float dt)
		{
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Invalid comparison between Unknown and I4
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			int count = Acts.Count;
			if (count == 0)
			{
				return;
			}
			Act[] array = ArrayPool<Act>.Shared.Rent(count);
			try
			{
				Acts.CopyTo(array);
				for (int i = 0; i < count; i++)
				{
					Act act = array[i];
					try
					{
						if (Acts.Contains(act))
						{
							if (!Shows(act.Copy, act.Card) || !Shows(act.Real, act.Card))
							{
								goto IL_0095;
							}
							CardPile pile = act.Card.Pile;
							if (pile == null || (int)pile.Type != 2 || HeldFxManager.Carried == act.Card)
							{
								goto IL_0095;
							}
							Node parent = ((Node)act.Real).GetParent();
							if (parent == act.Home)
							{
								goto IL_00e2;
							}
							if (parent is NHandCardHolder && !(act.Home is NHandCardHolder))
							{
								act.Home = parent;
								goto IL_00e2;
							}
							EndAct(act);
						}
						goto end_IL_0031;
						IL_0095:
						EndAct(act);
						goto end_IL_0031;
						IL_00e2:
						act.Elapsed += dt;
						if (act.Elapsed > act.Duration + 1f)
						{
							EndAct(act);
							continue;
						}
						float num = Mathf.Clamp(act.Elapsed / act.Duration, 0f, 1f);
						CardStandIn.Pose(act.Real, out var centre, out var rotation, out var scale);
						CardRide.Pose to = new CardRide.Pose
						{
							Position = centre,
							Rotation = rotation,
							Scale = scale,
							Alpha = 1f
						};
						CardRide.Pose from = act.From;
						CardRide.Pose pose = new CardRide.Pose
						{
							Position = from.Position.Lerp(to.Position, num),
							Rotation = Mathf.LerpAngle(from.Rotation, to.Rotation, num),
							Scale = from.Scale.Lerp(to.Scale, num),
							Alpha = Mathf.Lerp(from.Alpha, to.Alpha, num)
						};
						act.Spec.Shape?.Invoke(ref pose, num, in from, in to);
						CardStandIn.Place(act.Copy, pose.Position, pose.Rotation, pose.Scale);
						((CanvasItem)act.Copy).Modulate = new Color(1f, 1f, 1f, pose.Alpha);
						act.Spec.Frame?.Invoke(act.Copy, pose, num);
						if (!(num < 1f))
						{
							act.Spec.Arrived?.Invoke(pose);
							EndAct(act);
						}
						end_IL_0031:;
					}
					catch (Exception ex)
					{
						DevLog.Warn("HandMoves (act): " + ex);
						EndAct(act);
					}
				}
			}
			finally
			{
				ArrayPool<Act>.Shared.Return(array, clearArray: true);
			}
		}

		private static void OnDrawBegins(HandFlow.DrawInfo info)
		{
			try
			{
				if (info != null && !info.FromHandDraw && HandPace.On && !(Tuning.F("moves.on", 1f) < 0.5f) && info.Cause is Prepared && Tuning.F("moves.prepared_prehide", 1f) > 0.5f)
				{
					_ghostDraw = info;
					_ghostFor = 0f;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandMoves.OnDrawBegins: " + ex);
			}
		}

		private static void OnDrawnCardEntered(NCard node, HandFlow.DrawInfo info)
		{
			CardModel val = null;
			try
			{
				if (info != null && !info.FromHandDraw && info.Cause != null && FxKit.Alive((GodotObject)(object)node) && node.Model != null)
				{
					val = node.Model;
					AbstractModel cause = info.Cause;
					if (GameCapabilities.ExpertiseRetainsDrawnCards && cause is Expertise && Tuning.F("moves.expertise_tie", 1f) > 0.5f)
					{
						Tie tie = TieOf(info, make: true);
						tie.Cards.Add(val);
						tie.Quiet = 0f;
					}
					if (!HandPace.On || Tuning.F("moves.on", 1f) < 0.5f)
					{
						Uncover(node);
					}
					else if (cause is Backflip)
					{
						RideBackflip(node);
					}
					else if (cause is EscapePlan)
					{
						RideEscape(node);
					}
					else if (GameCapabilities.ExpertiseRetainsDrawnCards && cause is Expertise)
					{
						RideUnder(node);
					}
					else if (cause is Acrobatics)
					{
						RideSpringIn(node, info);
					}
					else if (cause is Prepared)
					{
						RideGhost(node);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandMoves.OnDrawnCardEntered: " + ex);
				try
				{
					CardRide.Finish(val);
					EndActs(val);
					Uncover(node);
				}
				catch (Exception ex2)
				{
					DevLog.Warn("HandMoves (restore): " + ex2.Message);
				}
			}
		}

		private static float Hop(ref CardRide.Pose pose, float t, in CardRide.Pose a, in CardRide.Pose b, float height, float hang)
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			float num = Mathf.Clamp(t / 0.85f, 0f, 1f);
			float num2 = Mathf.Clamp(num + hang * Mathf.Sin((float)Math.PI * 2f * num) / ((float)Math.PI * 2f), 0f, 1f);
			float num3 = Mathf.Lerp(num2, Smooth(num2), 0.5f);
			pose.Position = a.Position.Lerp(b.Position, num3) + new Vector2(0f, (0f - height) * 4f * num2 * (1f - num2));
			pose.Rotation = Mathf.LerpAngle(a.Rotation, b.Rotation, num3);
			pose.Scale = a.Scale.Lerp(b.Scale, num3);
			pose.Alpha = 1f;
			if (t > 0.85f)
			{
				float num4 = Mathf.Sin((float)Math.PI * (t - 0.85f) / 0.14999998f);
				pose.Scale = new Vector2(pose.Scale.X * (1f + 0.06f * num4), pose.Scale.Y * (1f - 0.08f * num4));
			}
			return num;
		}

		private static void RideBackflip(NCard node)
		{
			CardRide.ToCard(node, new CardRide.Spec
			{
				Duration = Seconds("moves.backflip_time", 0.55f),
				Shape = BackflipShape,
				Arrived = LandFlash
			});
		}

		private static void BackflipShape(ref CardRide.Pose pose, float t, in CardRide.Pose a, in CardRide.Pose b)
		{
			float hang = Mathf.Clamp(Tuning.F("moves.backflip_hang", 0.45f), 0f, 0.6f);
			float num = Hop(ref pose, t, in a, in b, Tuning.F("moves.backflip_hop", 190f), hang);
			float num2 = Mathf.Clamp(Tuning.F("moves.backflip_snap", 1.5f), 1f, 3f);
			float num3 = 1f - Mathf.Pow(1f - num, num2);
			pose.Rotation += Side(in a, in b) * ((float)Math.PI * 2f) * (1f - num3);
		}

		private static void LandFlash(CardRide.Pose pose)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node uiLayer = GameRefs.UiLayer;
				if (FxKit.Alive((GodotObject)(object)uiLayer))
				{
					Vector2 position = pose.Position;
					Vector2 val = new Vector2(0f, 90f * pose.Scale.Y);
					Vector2 at = position + val.Rotated(pose.Rotation);
					FxKit.Flash(uiLayer, at, With(Pale, 0.5f), 150f * Mathf.Max(0.4f, pose.Scale.X), 1.8f, 0.16);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandMoves (landing): " + ex.Message);
			}
		}

		private static void RideSpringIn(NCard node, HandFlow.DrawInfo info)
		{
			float num = Seconds("moves.acro_air", 0.82f);
			float num2 = 0f;
			try
			{
				num2 = MixHold(node, info, num);
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandMoves (acrobatics, order): " + ex.Message);
				_mix = null;
				num2 = 0f;
			}
			float num3 = num - Mathf.Max(0f, num2 - Mathf.Clamp(Tuning.F("moves.acro_hold", 0.5f), 0f, 0.7f));
			float num4 = ((float.IsFinite(num3) && num3 >= 0.08f) ? (1.6f - num3) : 0f);
			num2 = Mathf.Min(num2, num4);
			if (!(num2 >= 0.03f))
			{
				Perform(node, new CardRide.Spec
				{
					Duration = num,
					Shape = SpringInShape,
					Arrived = LandFlash
				});
				return;
			}
			float wait = num2;
			float total = num2 + num3;
			bool tossed = false;
			Perform(node, new CardRide.Spec
			{
				Duration = total,
				Shape = delegate(ref CardRide.Pose pose, float t, in CardRide.Pose a, in CardRide.Pose b)
				{
					SpringLateShape(wait, total, ref pose, t, in a, in b);
				},
				Frame = (NCard copy, CardRide.Pose pose, float t) =>
				{
					if (!tossed && !(t * total < wait))
					{
						tossed = true;
						ToFront(copy);
					}
				},
				Arrived = LandFlash
			});
		}

		private static float MixHold(NCard node, HandFlow.DrawInfo info, float air)
		{
			if (info == null || Tuning.F("moves.acro_mix", 1f) < 0.5f)
			{
				return 0f;
			}
			if (info.Shuffled)
			{
				_mix = null;
				return 0f;
			}
			float num = Pace();
			float num2 = Mathf.Clamp(Tuning.F("moves.acro_hold", 0.5f), 0f, 0.7f) + Mathf.Min(0.14f, 0.18f * air);
			int num3 = info.Entered - 1;
			if (num3 <= 0)
			{
				_mix = null;
				if (num3 < 0 || !FxKit.Alive((GodotObject)(object)node))
				{
					return 0f;
				}
				object obj = info.Player;
				if (obj == null)
				{
					CardModel model = node.Model;
					obj = ((model != null) ? model.Owner : null);
				}
				Player val = (Player)obj;
				if (val == null)
				{
					return 0f;
				}
				CardPile pile = PileTypeExtensions.GetPile((PileType)1, val);
				IReadOnlyList<CardModel> readOnlyList = ((pile != null) ? pile.Cards : null);
				CardPile pile2 = PileTypeExtensions.GetPile((PileType)2, val);
				IReadOnlyList<CardModel> readOnlyList2 = ((pile2 != null) ? pile2.Cards : null);
				if (readOnlyList == null || readOnlyList2 == null)
				{
					return 0f;
				}
				int num4 = Mathf.Min(info.Requested, 1 + Mathf.Max(0, CardPile.MaxCardsInHand - readOnlyList2.Count));
				num4 = Mathf.Min(num4, 10);
				if (num4 < 2 || readOnlyList.Count < num4 - 1)
				{
					return 0f;
				}
				float num5 = ((_mixGap > 0f) ? _mixGap : Tuning.F("moves.acro_gap", 0.25f));
				num5 = (float.IsFinite(num5) ? Mathf.Clamp(num5, 0.05f, 0.35f) : 0.25f);
				float apart = Mathf.Clamp(Tuning.F("moves.acro_apart", 0.12f), 0.04f, 0.4f) * num;
				_mix = new Mix
				{
					Draw = info,
					Start = _clock,
					Gap = num5,
					Leave = MixTimes(num4, num5, apart, num2)
				};
			}
			Mix mix = _mix;
			if (mix == null || mix.Draw != info)
			{
				return 0f;
			}
			float num6 = _clock - mix.Start;
			if (num3 > 0)
			{
				if (!(num6 >= 0f) || num6 > (float)num3 * mix.Gap + 0.3f)
				{
					_mix = null;
					return 0f;
				}
				_mixGap = Mathf.Clamp(num6 / (float)num3, 0.05f, 0.35f);
			}
			if (mix.Leave == null || num3 >= mix.Leave.Length)
			{
				return 0f;
			}
			return Mathf.Clamp(mix.Leave[num3] - num6, 0f, num2);
		}

		private static float[] MixTimes(int count, float gap, float apart, float most)
		{
			if (count < 2)
			{
				return null;
			}
			int[] array = new int[count];
			float[] array2 = new float[count];
			for (int i = 0; i < 28; i++)
			{
				for (int j = 0; j < count; j++)
				{
					array[j] = j;
				}
				if (i < 24)
				{
					for (int num = count - 1; num > 0; num--)
					{
						int num2 = Mathf.Clamp((int)(GD.Randf() * (float)(num + 1)), 0, num);
						int num3 = array[num];
						array[num] = array[num2];
						array[num2] = num3;
					}
				}
				else
				{
					int num4 = Mathf.Clamp((int)(GD.Randf() * (float)(count - 1)), 0, count - 2);
					int num5 = array[num4];
					array[num4] = array[num4 + 1];
					array[num4 + 1] = num5;
				}
				bool flag = true;
				for (int k = 0; k < count; k++)
				{
					if (array[k] != k)
					{
						flag = false;
					}
				}
				if (flag)
				{
					continue;
				}
				float num6 = 0f;
				float num7 = 0f;
				for (int l = 0; l < count; l++)
				{
					int num8 = array[l];
					float num9 = (float)num8 * gap;
					if (l > 0)
					{
						num9 = Mathf.Max(num9, num6 + apart);
					}
					array2[num8] = num9;
					num7 = Mathf.Max(num7, num9 - (float)num8 * gap);
					num6 = num9;
				}
				if (num7 <= most)
				{
					return array2;
				}
			}
			return null;
		}

		private static void ToFront(NCard copy)
		{
			try
			{
				if (!FxKit.Alive((GodotObject)(object)copy))
				{
					return;
				}
				Node parent = ((Node)copy).GetParent();
				if (FxKit.Alive((GodotObject)(object)parent))
				{
					int num = parent.GetChildCount(false) - 1;
					if (num > 0 && ((Node)copy).GetIndex(false) != num)
					{
						parent.MoveChild((Node)(object)copy, num);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandMoves (acrobatics, front): " + ex.Message);
			}
		}

		private static void SpringLateShape(float wait, float total, ref CardRide.Pose pose, float t, in CardRide.Pose a, in CardRide.Pose b)
		{
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			float num = Mathf.Max(0.02f, Mathf.Min(0.1f, wait));
			float num2 = t * total;
			float num5;
			if (num2 < wait)
			{
				float num3 = Smooth(Mathf.Clamp(num2 / num, 0f, 1f));
				float num4 = Smooth(Mathf.Clamp((num2 - (wait - num)) / num, 0f, 1f));
				num5 = 0.3f * num3 + 0.7f * num4;
				pose.Position = a.Position;
				pose.Rotation = a.Rotation;
				pose.Scale = a.Scale;
				pose.Alpha = 1f;
			}
			else
			{
				float num6 = Mathf.Max(0.01f, total - wait);
				SpringInShape(ref pose, Mathf.Clamp((num2 - wait) / num6, 0f, 1f), in a, in b);
				num5 = 1f - Smooth(Mathf.Clamp((num2 - wait) / num, 0f, 1f));
			}
			pose.Scale = new Vector2(pose.Scale.X * (1f + 0.05f * num5), pose.Scale.Y * (1f - 0.09f * num5));
		}

		private static void SpringInShape(ref CardRide.Pose pose, float t, in CardRide.Pose a, in CardRide.Pose b)
		{
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			float num = Mathf.Clamp(t / 0.85f, 0f, 1f);
			float num2 = Mathf.Clamp(Tuning.F("moves.acro_hang", 0.38f), 0f, 0.6f);
			float num3 = Mathf.Clamp(num + num2 * Mathf.Sin((float)Math.PI * 2f * num) / ((float)Math.PI * 2f), 0f, 1f);
			float num4 = Mathf.Max(60f, Tuning.F("moves.acro_rise", 520f));
			num4 = Mathf.Min(num4, Mathf.Max(120f, Mathf.Min(a.Position.Y, b.Position.Y) - 250f));
			float num5 = Mathf.Pow(num3, 1.35f);
			pose.Position = a.Position.Lerp(b.Position, num5) + new Vector2(0f, (0f - num4) * 4f * num3 * (1f - num3));
			pose.Scale = a.Scale.Lerp(b.Scale, Smooth(num)) * (1f + 0.07f * Mathf.Sin((float)Math.PI * num3));
			pose.Alpha = 1f;
			float num6 = Mathf.Round(Mathf.Clamp(Tuning.F("moves.acro_turns", 1f), 0f, 3f));
			float num7 = Mathf.Lerp(num, Smooth(num), 0.4f);
			pose.Rotation = Mathf.LerpAngle(a.Rotation, b.Rotation, Smooth(num)) - Side(in a, in b) * ((float)Math.PI * 2f) * num6 * (1f - num7);
			if (t > 0.85f)
			{
				float num8 = Mathf.Sin((float)Math.PI * (t - 0.85f) / 0.14999998f);
				pose.Scale = new Vector2(pose.Scale.X * (1f + 0.07f * num8), pose.Scale.Y * (1f - 0.1f * num8));
			}
		}

		private static void RideUnder(NCard node)
		{
			Under under = new Under();
			float time = Seconds("moves.expertise_under_time", 0.66f);
			CardRide.ToCard(node, new CardRide.Spec
			{
				Duration = time,
				Shape = delegate(ref CardRide.Pose pose, float t, in CardRide.Pose a, in CardRide.Pose b)
				{
					under.FromX = a.Position.X;
					under.ToX = b.Position.X;
					UnderShape(ref pose, t, in a, in b);
				},
				Frame = (NCard copy, CardRide.Pose pose, float t) =>
				{
					UnderFrame(under, copy, t, time);
				}
			});
		}

		private static void UnderShape(ref CardRide.Pose pose, float t, in CardRide.Pose a, in CardRide.Pose b)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			float y = FxKit.ViewSize().Y;
			if (t < 0.58f)
			{
				float num = Mathf.Clamp(t / 0.24f, 0f, 1f);
				float num2 = Side(in a, in b);
				float num3 = y + 211f * a.Scale.Y + 14f;
				pose.Position = new Vector2(a.Position.X + num2 * 28f * num * num, Mathf.Lerp(a.Position.Y, num3, num * num));
				pose.Rotation = a.Rotation + num2 * 0.22f * num;
				pose.Scale = a.Scale;
				pose.Alpha = 1f;
			}
			else
			{
				float num4 = (t - 0.58f) / 0.42000002f;
				float num5 = y + 211f * b.Scale.Y + 14f;
				float num6 = 0.55f * Intro.BackOut(num4) + 0.45f * (1f - Mathf.Pow(1f - num4, 3f));
				pose.Position = new Vector2(b.Position.X, Mathf.Lerp(num5, b.Position.Y, num6));
				pose.Rotation = b.Rotation - Side(in a, in b) * 0.07f * (1f - Smooth(Mathf.Clamp(num4 / 0.7f, 0f, 1f)));
				pose.Scale = b.Scale;
				pose.Alpha = 1f;
			}
		}

		private static void UnderFrame(Under under, NCard copy, float t, float time)
		{
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (under.Glint == null)
				{
					Node parent = ((Node)copy).GetParent();
					if (!FxKit.Alive((GodotObject)(object)parent))
					{
						return;
					}
					under.Glint = FxKit.Glow(new Color(StayBlue.R, StayBlue.G, StayBlue.B, 0f), new Vector2(170f, 44f));
					((CanvasItem)under.Glint).ZIndex = 4;
					parent.AddChild((Node)(object)under.Glint, false, (InternalMode)0);
					FxKit.FreeAfter((Node)(object)under.Glint, time + 0.5f);
				}
				Sprite2D glint = under.Glint;
				if (FxKit.Alive((GodotObject)(object)glint))
				{
					float y = FxKit.ViewSize().Y;
					float num = Smooth(Mathf.Clamp((t - 0.14400001f) / 0.476f, 0f, 1f));
					((Node2D)glint).GlobalPosition = new Vector2(Mathf.Lerp(under.FromX, under.ToX, num), y - 5f);
					float num2 = Smooth(Mathf.Clamp((t - 0.12f) / 0.08f, 0f, 1f)) * (1f - Smooth(Mathf.Clamp((t - 0.58f) / 0.16f, 0f, 1f)));
					float num3 = Mathf.Sin((float)Math.PI * num);
					float num4 = 1f + 0.8f * Smooth(Mathf.Clamp((t - 0.58f + 0.06f) / 0.1f, 0f, 1f));
					((Node2D)glint).Scale = new Vector2((170f + 260f * num3) / 64f, 44f * num4 / 64f);
					((CanvasItem)glint).Modulate = new Color(StayBlue.R, StayBlue.G, StayBlue.B, Mathf.Clamp(Tuning.F("moves.expertise_glint", 0.7f), 0f, 1f) * num2);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandMoves (under): " + ex.Message);
			}
		}

		private static void RideGhost(NCard node)
		{
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			float num = Seconds("moves.prepared_time", 0.6f);
			Ghost ghost = new Ghost();
			Node parent = ((Node)node).GetParent();
			ghost.Holder = (NHandCardHolder)(object)((parent is NHandCardHolder) ? parent : null);
			ghost.Time = num;
			Ghost ghost2 = ghost;
			CardStandIn.Pose(node, out var centre, out var rotation, out var scale);
			if (SlotOf(ghost2.Holder, out var centre2, out var angle))
			{
				centre = centre2;
				rotation = angle;
			}
			bool flag = Perform(node, new CardRide.Spec
			{
				Duration = num,
				From = new CardRide.Pose
				{
					Position = centre,
					Rotation = rotation,
					Scale = scale,
					Alpha = 0f
				},
				Shape = delegate(ref CardRide.Pose pose, float t, in CardRide.Pose a, in CardRide.Pose b)
				{
					GhostShape(ghost2, ref pose, t, in b);
				},
				Frame = (NCard copy, CardRide.Pose pose, float t) =>
				{
					GhostFrame(ghost2, copy, pose, t);
				}
			});
			Uncover(node, !flag);
		}

		private static float Blink(float t, float at, float half)
		{
			float num = Mathf.Abs(t - at) / half;
			if (!(num >= 1f))
			{
				return Smooth(1f - num);
			}
			return 0f;
		}

		private static void GhostShape(Ghost ghost, ref CardRide.Pose pose, float t, in CardRide.Pose b)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = b.Position;
			float num = b.Rotation;
			if (SlotOf(ghost.Holder, out var centre, out var angle))
			{
				val = centre;
				num = angle;
			}
			float num2 = Mathf.Max(0f, t - ghost.LastT) * ghost.Time;
			ghost.LastT = t;
			if (!ghost.Placed)
			{
				ghost.Placed = true;
				ghost.At = val;
				ghost.Turn = num;
			}
			else
			{
				float num3 = 1f - Mathf.Exp(-14f * num2);
				ghost.At = ghost.At.Lerp(val, num3);
				ghost.Turn = Mathf.LerpAngle(ghost.Turn, num, num3);
			}
			float num4 = Smooth(Mathf.Clamp(t / 0.22f, 0f, 1f));
			ghost.Pulse = Mathf.Max(Blink(t, 0.36f, 0.085f), Blink(t, 0.57f, 0.085f));
			ghost.Solid = Smooth(Mathf.Clamp((t - 0.72f) / 0.28f, 0f, 1f));
			float num5 = Mathf.Clamp(Tuning.F("moves.prepared_alpha", 0.55f), 0.1f, 1f);
			float num6 = Mathf.Min(1f, num5 * num4 + 0.37f * ghost.Pulse);
			float num7 = Tuning.F("moves.prepared_rise", 14f) * (1f - Smooth(Mathf.Clamp(t / 0.55f, 0f, 1f)));
			pose.Position = ghost.At.Lerp(b.Position, ghost.Solid) + new Vector2(0f, num7);
			pose.Rotation = Mathf.LerpAngle(ghost.Turn, b.Rotation, ghost.Solid);
			pose.Scale = b.Scale * (1f + 0.03f * ghost.Pulse);
			pose.Alpha = Mathf.Lerp(num6, 1f, ghost.Solid);
		}

		private static void GhostFrame(Ghost ghost, NCard copy, CardRide.Pose pose, float t)
		{
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)copy))
			{
				return;
			}
			float num = 1f + 0.45f * ghost.Pulse * (1f - ghost.Solid);
			((CanvasItem)copy).Modulate = new Color(Mathf.Lerp(Wraith.R, 1f, ghost.Solid) * num, Mathf.Lerp(Wraith.G, 1f, ghost.Solid) * num, Mathf.Lerp(Wraith.B, 1f, ghost.Solid) * num, pose.Alpha);
			if (!ghost.Made)
			{
				ghost.Made = true;
				Node parent = ((Node)copy).GetParent();
				if (!FxKit.Alive((GodotObject)(object)parent))
				{
					return;
				}
				Sprite2D val = FxKit.Glow(new Color(Wraith.R, Wraith.G, Wraith.B, 0f), new Vector2(64f, 64f));
				parent.AddChild((Node)(object)val, false, (InternalMode)0);
				if (!((Node)val).IsInsideTree())
				{
					((Node)val).QueueFree();
					return;
				}
				parent.MoveChild((Node)(object)val, ((Node)copy).GetIndex(false));
				FxKit.FreeAfter((Node)(object)val, ghost.Time + 0.25f);
				ghost.Glow = val;
			}
			if (FxKit.Alive((GodotObject)(object)ghost.Glow))
			{
				float num2 = (0.16f + 0.5f * ghost.Pulse) * Mathf.Clamp(t / 0.15f, 0f, 1f) * (1f - ghost.Solid);
				((Node2D)ghost.Glow).GlobalPosition = pose.Position;
				((Node2D)ghost.Glow).GlobalRotation = pose.Rotation;
				((Node2D)ghost.Glow).Scale = new Vector2(440f * pose.Scale.X, 570f * pose.Scale.Y) * ((1f + 0.1f * ghost.Pulse) / 64f);
				((CanvasItem)ghost.Glow).Modulate = new Color(Wraith.R, Wraith.G, Wraith.B, num2);
			}
		}

		private static void UpdateCover(float real)
		{
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Invalid comparison between Unknown and I4
			if (_ghostDraw != null)
			{
				_ghostFor += real;
				Node val = ((_ghostFor < 4f) ? GameRefs.UiLayer : null);
				if (!FxKit.Alive((GodotObject)(object)val))
				{
					_ghostDraw = null;
				}
				else
				{
					int childCount = val.GetChildCount(false);
					for (int i = 0; i < childCount; i++)
					{
						Node child = val.GetChild(i, false);
						NCard val2 = (NCard)(object)((child is NCard) ? child : null);
						if (val2 == null || !FxKit.Alive((GodotObject)(object)val2))
						{
							continue;
						}
						CardModel model = val2.Model;
						if (model != null)
						{
							CardPile pile = model.Pile;
							if (pile != null && (int)pile.Type == 2 && (_ghostDraw.Player == null || model.Owner == _ghostDraw.Player) && !IsCopy(val2) && !IsCovered(val2) && !CardRide.Riding(model) && !Performing(model) && HeldFxManager.Carried != model)
							{
								Covered.Add(new Hidden
								{
									Node = val2,
									Card = model
								});
								CardStandIn.Hide(val2);
							}
						}
					}
				}
			}
			for (int num = Covered.Count - 1; num >= 0; num--)
			{
				Hidden hidden = Covered[num];
				hidden.For += real;
				if (!(hidden.For < 1.5f) || !Shows(hidden.Node, hidden.Card))
				{
					Covered.RemoveAt(num);
					if (!Performing(hidden.Card))
					{
						CardStandIn.Show(hidden.Node);
					}
				}
			}
		}

		private static bool IsCovered(NCard node)
		{
			foreach (Hidden item in Covered)
			{
				if (item.Node == node)
				{
					return true;
				}
			}
			return false;
		}

		private static void Uncover(NCard node, bool show = true)
		{
			if (node == null)
			{
				return;
			}
			for (int num = Covered.Count - 1; num >= 0; num--)
			{
				if (Covered[num].Node == node)
				{
					Covered.RemoveAt(num);
					if (show)
					{
						CardStandIn.Show(node);
					}
				}
			}
		}

		private static void RideEscape(NCard node)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Invalid comparison between Unknown and I4
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			CardModel model = node.Model;
			if ((int)model.Type != 2)
			{
				return;
			}
			Player owner = model.Owner;
			CardFxContext cardFxContext = CardFxManager.BuildContext((owner != null) ? owner.Creature : null, null);
			if (cardFxContext != null)
			{
				Whirl whirl = new Whirl
				{
					Chest = cardFxContext.Chest,
					Radii = new Vector2(Mathf.Clamp(cardFxContext.Body.X * 0.9f, 90f, 320f), Mathf.Clamp(cardFxContext.Body.Y * 0.45f, 70f, 260f)),
					Fight = cardFxContext.FrontParent,
					Height = cardFxContext.Body.Y,
					Time = Seconds("moves.escape_time", 0.8f),
					Small = Mathf.Clamp(Tuning.F("moves.escape_small", 0.6f), 0.3f, 1f)
				};
				CardRide.ToCard(node, new CardRide.Spec
				{
					Duration = whirl.Time,
					Shape = delegate(ref CardRide.Pose pose, float t, in CardRide.Pose a, in CardRide.Pose b)
					{
						WhirlShape(whirl, ref pose, t, in a, in b);
					},
					Frame = (NCard copy, CardRide.Pose pose, float t) =>
					{
						WhirlFrame(whirl, copy, pose, t);
					},
					Arrived = LandFlash
				});
			}
		}

		private static void WhirlShape(Whirl whirl, ref CardRide.Pose pose, float t, in CardRide.Pose a, in CardRide.Pose b)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = whirl.Chest + new Vector2(0f, whirl.Radii.Y);
			float num = Mathf.Max(whirl.Radii.X, 120f);
			Vector2 val2 = b.Scale * whirl.Small;
			float alpha = 1f;
			Vector2 position;
			Vector2 scale;
			float num4;
			if (t < 0.275f)
			{
				float num2 = t / 0.275f;
				position = FxKit.Bezier(t: Mathf.Lerp(num2, num2 * num2, 0.45f), a: a.Position, b: val + new Vector2((0f - num) * 1.1f, 0f), c: val);
				float num3 = Smooth(num2);
				scale = a.Scale.Lerp(val2 * 1.14f, num3);
				num4 = 0.3f * num3;
			}
			else if (t < 0.725f)
			{
				float num5 = (t - 0.275f) / 0.45000002f;
				float num6 = Mathf.Lerp(num5, Smooth(num5), 0.3f);
				float num7 = (float)Math.PI / 2f - (float)Math.PI * 2f * num6;
				float num8 = Mathf.Sin(num7);
				position = whirl.Chest + new Vector2(whirl.Radii.X * Mathf.Cos(num7), whirl.Radii.Y * num8);
				scale = val2 * (1f + 0.14f * num8);
				alpha = 1f - 0.42f * Mathf.Max(0f, 0f - num8);
				num4 = 0.3f * num8;
			}
			else
			{
				float num9 = (t - 0.725f) / 0.27499998f;
				float t2 = 1f - (1f - num9) * (1f - num9);
				position = FxKit.Bezier(val, val + new Vector2(num * 0.9f, 0f), b.Position, t2);
				float num10 = Smooth(num9);
				Vector2 val3 = val2 * 1.14f;
				scale = val3.Lerp(b.Scale, num10);
				num4 = 0.3f * (1f - num10);
			}
			pose.Position = position;
			pose.Scale = scale;
			pose.Alpha = alpha;
			pose.Rotation = Mathf.LerpAngle(a.Rotation, b.Rotation, Smooth(t)) + num4;
		}

		private static void WhirlFrame(Whirl whirl, NCard copy, CardRide.Pose pose, float t)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)copy))
			{
				return;
			}
			Node parent = ((Node)copy).GetParent();
			if (!FxKit.Alive((GodotObject)(object)parent))
			{
				return;
			}
			if (!whirl.Made)
			{
				whirl.Made = true;
				Line2D val = FxKit.Ribbon(Tuning.F("moves.escape_tail_width", 34f), With(Teal, 0.62f));
				Line2D val2 = FxKit.Ribbon(11f, new Color(0.86f, 1f, 0.97f, 0.95f));
				parent.AddChild((Node)(object)val, false, (InternalMode)0);
				parent.AddChild((Node)(object)val2, false, (InternalMode)0);
				if (!((Node)val).IsInsideTree() || !((Node)val2).IsInsideTree())
				{
					((Node)val).QueueFree();
					((Node)val2).QueueFree();
				}
				else
				{
					parent.MoveChild((Node)(object)val, ((Node)copy).GetIndex(false));
					parent.MoveChild((Node)(object)val2, ((Node)copy).GetIndex(false));
					FxKit.FreeAfter((Node)(object)val, whirl.Time + 0.3f);
					FxKit.FreeAfter((Node)(object)val2, whirl.Time + 0.3f);
					whirl.Tail = val;
					whirl.Core = val2;
				}
			}
			if (FxKit.Alive((GodotObject)(object)whirl.Tail) && FxKit.Alive((GodotObject)(object)whirl.Core))
			{
				float num = t * whirl.Time;
				float num2 = Mathf.Clamp(Tuning.F("moves.escape_tail", 0.11f), 0.03f, 0.4f) * Pace();
				whirl.Path.Add(pose.Position);
				whirl.Stamp.Add(num);
				while (whirl.Stamp.Count > 2 && num - whirl.Stamp[0] > num2)
				{
					whirl.Stamp.RemoveAt(0);
					whirl.Path.RemoveAt(0);
				}
				Vector2[] array = new Vector2[whirl.Path.Count];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = ((Node2D)whirl.Tail).ToLocal(whirl.Path[i]);
				}
				whirl.Tail.Points = array;
				whirl.Core.Points = array;
				float num3 = Mathf.Clamp(Mathf.Min(t * 10f, (1f - t) * 7f), 0f, 1f);
				((CanvasItem)whirl.Tail).Modulate = new Color(1f, 1f, 1f, num3);
				((CanvasItem)whirl.Core).Modulate = new Color(1f, 1f, 1f, num3);
			}
			int num4 = Mathf.Clamp((int)Tuning.F("moves.escape_ghosts", 4f), 0, 8);
			float num5 = (t - 0.275f) / 0.45000002f;
			if (whirl.Ghosts < num4 && num5 >= ((float)whirl.Ghosts + 0.5f) / (float)num4)
			{
				whirl.Ghosts++;
				if (num5 <= 1f)
				{
					Afterimage(parent, copy, pose);
				}
			}
			if (!whirl.Closed && t >= 0.725f)
			{
				whirl.Closed = true;
				if (FxKit.Alive((GodotObject)(object)whirl.Fight))
				{
					FxKit.RingPulse(whirl.Fight, whirl.Chest, Mathf.Max(220f, whirl.Height * 1.25f), Teal, 0f, 0.34f, 0.3f, 1.25f, 1f, 0f, 0.05f, 0.95f);
					FxKit.Flash(whirl.Fight, whirl.Chest, With(Teal, 0.65f), Mathf.Max(130f, whirl.Height * 0.55f), 1.8f, 0.2);
					Brush.Sparks(whirl.Fight, whirl.Chest, Teal, 7, 320f);
				}
			}
		}

		private static void Afterimage(Node parent, NCard copy, CardRide.Pose pose)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected Obj, but got Unknown
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Expected Obj, but got Unknown
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			Node2D ghost = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_Afterimage")
			};
			((Node)ghost).AddChild((Node)new Polygon2D
			{
				Polygon = CardShape(),
				Color = new Color(Teal.R, Teal.G, Teal.B, Tuning.F("moves.escape_ghost", 0.14f)),
				Material = (Material)(object)FxKit.Additive,
				Antialiased = true
			}, false, (InternalMode)0);
			Line2D val = Props.Line(8f, new Color(Teal.R, Teal.G, Teal.B, 0.85f), additive: true, CardShape());
			val.Closed = true;
			((Node)ghost).AddChild((Node)(object)val, false, (InternalMode)0);
			parent.AddChild((Node)(object)ghost, false, (InternalMode)0);
			if (!((Node)ghost).IsInsideTree())
			{
				((Node)ghost).QueueFree();
				return;
			}
			parent.MoveChild((Node)(object)ghost, ((Node)copy).GetIndex(false));
			ghost.GlobalPosition = pose.Position;
			ghost.GlobalRotation = pose.Rotation;
			ghost.GlobalScale = pose.Scale * 0.94f;
			Vector2 size = ghost.Scale;
			FxKit.Drive((Node)(object)ghost, 0.15f, (float u) =>
			{
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)ghost))
				{
					ghost.Scale = size * (1f - 0.35f * u);
					((CanvasItem)ghost).Modulate = new Color(1f, 1f, 1f, 1f - u);
				}
			}, (TransitionType)0, (EaseType)2);
			FxKit.FreeAfter((Node)(object)ghost, 0.19);
		}

		private static Tie TieOf(HandFlow.DrawInfo info, bool make)
		{
			foreach (Tie tie2 in Ties)
			{
				if (tie2.Draw == info)
				{
					return tie2;
				}
			}
			if (!make)
			{
				return null;
			}
			Tie tie = new Tie
			{
				Draw = info
			};
			Ties.Add(tie);
			return tie;
		}

		private static void OnDrawEnded(HandFlow.DrawInfo info)
		{
			try
			{
				if (info != null)
				{
					if (_ghostDraw == info)
					{
						_ghostDraw = null;
					}
					if (GameCapabilities.ExpertiseRetainsDrawnCards && !info.FromHandDraw && info.Cause is Expertise && !(Tuning.F("moves.expertise_tie", 1f) < 0.5f))
					{
						TieOf(info, make: true).Ended = true;
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandMoves.OnDrawEnded: " + ex);
			}
		}

		private static bool Keeps(Tie tie, CardModel card)
		{
			if (!GameCapabilities.ExpertiseRetainsDrawnCards)
			{
				return false;
			}
			try
			{
				return card != null && tie.Cards.Contains(card) && card.ShouldRetainThisTurn && (card.Keywords == null || !card.Keywords.Contains((CardKeyword)5));
			}
			catch
			{
				return false;
			}
		}

		private static void UpdateTies(float real, float dt)
		{
			if (!GameCapabilities.ExpertiseRetainsDrawnCards)
			{
				return;
			}
			for (int num = Ties.Count - 1; num >= 0; num--)
			{
				Tie tie = Ties[num];
				tie.Age += real;
				tie.Quiet += real;
				if ((!tie.Ended && tie.Age > 10f) || tie.Age > 600f)
				{
					Ties.RemoveAt(num);
				}
			}
			StepStays(dt);
		}

		private static void OnTurnEnding(Player player)
		{
			try
			{
				_picking = false;
				_discarding = false;
				SettleAll();
				if (Ties.Count == 0)
				{
					return;
				}
				if (Tuning.F("moves.expertise_tie", 1f) > 0.5f)
				{
					HandCards.Fill(Hand);
					bool flag = false;
					try
					{
						Creature val = ((player != null) ? player.Creature : null);
						flag = val != null && (val.HasPower<WellLaidPlansPower>() || val.HasPower<RetainHandPower>());
					}
					catch
					{
						flag = false;
					}
					bool flag2 = false;
					if (!flag)
					{
						foreach (HandCards.Entry item in Hand)
						{
							try
							{
								CardModel card = item.Card;
								if (card == null || card.ShouldRetainThisTurn || (card.Keywords != null && card.Keywords.Contains((CardKeyword)2)))
								{
									continue;
								}
								flag2 = true;
								break;
							}
							catch
							{
							}
						}
					}
					int num = 0;
					foreach (HandCards.Entry item2 in Hand)
					{
						if (!Shows(item2.Node, item2.Card) || !((CanvasItem)item2.Node).IsVisibleInTree())
						{
							continue;
						}
						bool flag3 = false;
						foreach (Tie tie in Ties)
						{
							if (Keeps(tie, item2.Card))
							{
								flag3 = true;
								break;
							}
						}
						if (flag3 && BeginStay(item2.Node, item2.Card, flag2, num))
						{
							num++;
						}
					}
					if (num > 0)
					{
						DevLog.Info($"Expertise: {num} card(s) stay - " + (flag2 ? "they duck under the discard" : "nothing is thrown away, so only the light"));
					}
				}
				Ties.Clear();
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandMoves.OnTurnEnding: " + ex);
				FreeTies();
			}
		}

		private static bool BeginStay(NCard node, CardModel card, bool duck, int index)
		{
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Expected Obj, but got Unknown
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			foreach (Stay stay2 in Stays)
			{
				if (stay2.Node == node)
				{
					return false;
				}
			}
			Stay stay = new Stay
			{
				Node = node,
				Card = card,
				Duck = duck,
				T = -0.03f * (float)index
			};
			stay.Root = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_StayLight")
			};
			((Node)node).AddChild((Node)(object)stay.Root, false, (InternalMode)0);
			Stays.Add(stay);
			Color val = StayBlue.Lerp(Colors.White, 0.35f);
			stay.Sheen = FxKit.Glow(new Color(val.R, val.G, val.B, 0f), new Vector2(84f, 620f));
			((Node2D)stay.Sheen).Rotation = 0.3f;
			((Node)stay.Root).AddChild((Node)(object)stay.Sheen, false, (InternalMode)0);
			return true;
		}

		private static void EndStay(Stay stay)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (stay.Nudge != Vector2.Zero && Shows(stay.Node, stay.Card))
				{
					NCard node = stay.Node;
					((Control)node).Position = ((Control)node).Position - stay.Nudge;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandMoves (stay): " + ex.Message);
			}
			stay.Nudge = Vector2.Zero;
			if (FxKit.Alive((GodotObject)(object)stay.Root))
			{
				((Node)stay.Root).QueueFree();
			}
			stay.Root = null;
		}

		private static void StepStays(float dt)
		{
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			if (Stays.Count == 0)
			{
				return;
			}
			float num = Mathf.Clamp(Tuning.F("moves.expertise_glow", 0.45f), 0f, 1f);
			float num2 = Mathf.Max(0f, Tuning.F("moves.expertise_duck", 130f));
			float num3 = Mathf.Max(0.05f, Tuning.F("moves.expertise_hold", 0.36f));
			for (int num4 = Stays.Count - 1; num4 >= 0; num4--)
			{
				Stay stay = Stays[num4];
				if (!FxKit.Alive((GodotObject)(object)stay.Root) || !FxKit.Alive((GodotObject)(object)stay.Sheen) || !Shows(stay.Node, stay.Card))
				{
					if (FxKit.Alive((GodotObject)(object)stay.Node) && stay.Node.Model != stay.Card)
					{
						stay.Nudge = Vector2.Zero;
					}
					EndStay(stay);
					Stays.RemoveAt(num4);
				}
				else
				{
					stay.T += dt;
					if (!(stay.T < 0f))
					{
						float t = stay.T;
						float num5 = ((stay.Duck && t < num3) ? num2 : 0f);
						int num6 = Mathf.Clamp(Mathf.CeilToInt(dt / 0.006f), 1, 12);
						float num7 = dt / (float)num6;
						for (int i = 0; i < num6; i++)
						{
							stay.Speed += (676f * (num5 - stay.Down) - 40.559998f * stay.Speed) * num7;
							stay.Down += stay.Speed * num7;
						}
						Node parent = ((Node)stay.Node).GetParent();
						CanvasItem val = (CanvasItem)(object)((parent is CanvasItem) ? parent : null);
						if (val != null)
						{
							Transform2D val2 = val.GetGlobalTransform();
							val2 = val2.AffineInverse();
							Vector2 val3 = val2.BasisXform(new Vector2(0f, stay.Down));
							NCard node = stay.Node;
							((Control)node).Position = ((Control)node).Position + (val3 - stay.Nudge);
							stay.Nudge = val3;
						}
						float num8 = (stay.Duck ? (num3 + 0.06f) : 0.08f);
						float num9 = Mathf.Clamp((t - num8) / 0.34f, 0f, 1f);
						float num10 = (FxKit.Alive((GodotObject)(object)stay.Node.Body) ? stay.Node.Body.Scale.X : 1f);
						stay.Root.Scale = new Vector2(num10, num10);
						((Node2D)stay.Sheen).Position = new Vector2(Mathf.Lerp(-190f, 190f, Smooth(num9)), 0f);
						Color val4 = StayBlue.Lerp(Colors.White, 0.35f);
						((CanvasItem)stay.Sheen).Modulate = new Color(val4.R, val4.G, val4.B, (t < num8) ? 0f : (num * Mathf.Sin((float)Math.PI * num9)));
						bool flag = Mathf.Abs(stay.Down) < 0.6f && Mathf.Abs(stay.Speed) < 6f && num5 <= 0f;
						if (((num9 >= 1f) & flag) || t > 2.5f)
						{
							EndStay(stay);
							Stays.RemoveAt(num4);
						}
					}
				}
			}
		}

		private static void OnHandDrawBegins(Player player)
		{
			try
			{
				Ties.Clear();
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandMoves.OnHandDrawBegins: " + ex);
				FreeTies();
			}
		}

		private static void FreeTies()
		{
			try
			{
				foreach (Stay stay in Stays)
				{
					EndStay(stay);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandMoves (stay lights): " + ex);
			}
			Stays.Clear();
			Ties.Clear();
		}

		private static void OnDiscardBegins(IReadOnlyList<CardModel> cards, AbstractModel cause, int cardsToDraw)
		{
			try
			{
				if (cards == null)
				{
					return;
				}
				_discarding = (!(cause is CalculatedGamble) && !(cause is StormOfSteel) && !(cause is ShadowStep)) || Tuning.F("sly.hop_gamble", 0f) > 0.5f;
				_discardFor = 0f;
				if (!HandPace.On)
				{
					return;
				}
				bool flag = Tuning.F("moves.on", 1f) > 0.5f;
				bool flag2 = Tuning.F("sly.on", 1f) > 0.5f;
				for (int i = 0; i < cards.Count; i++)
				{
					CardModel val = cards[i];
					if (val == null)
					{
						continue;
					}
					float seconds = 3f + 0.6f * (float)i;
					if (Sly(val))
					{
						if (flag2 && !CardFlight.Claimed(val))
						{
							ClaimFlick(val, seconds);
						}
					}
					else if (flag && cause is Acrobatics)
					{
						ClaimSpring(val, seconds);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandMoves.OnDiscardBegins: " + ex);
			}
		}

		private static void OnDiscardEnded(AbstractModel cause)
		{
			_discarding = false;
		}

		private static void ClaimSpring(CardModel card, float seconds)
		{
			float time = Seconds("moves.acro_out", 0.55f);
			CardFlight.Claim(card, new CardFlight.Plan
			{
				HideCard = true,
				HideTrail = true,
				Duration = time,
				Started = (NCard node, Vector2 start, Vector2 pile) =>
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0008: Unknown result type (might be due to invalid IL or missing references)
					Leave(card, node, start, pile, time, 0.2f, SpringShape);
				}
			}, seconds);
		}

		private static void Leave(CardModel card, NCard node, Vector2 start, Vector2 pile, float time, float small, CardRide.Shaper shape)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			try
			{
				Vector2 centre = start;
				float rotation = 0f;
				Vector2 scale = Vector2.One;
				if (FxKit.Alive((GodotObject)(object)node))
				{
					CardStandIn.Pose(node, out centre, out rotation, out scale);
					if (centre.DistanceSquaredTo(start) > 160000f)
					{
						centre = start;
					}
				}
				CardRide.Pose value = new CardRide.Pose
				{
					Position = centre,
					Rotation = rotation,
					Scale = scale,
					Alpha = 1f
				};
				CardRide.Pose last = new CardRide.Pose
				{
					Position = pile,
					Rotation = rotation,
					Scale = scale * small,
					Alpha = 0f
				};
				flag = CardRide.Fly(card, new CardRide.Spec
				{
					Duration = time,
					From = value,
					Shape = shape
				}, () => last);
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandMoves (discard): " + ex);
			}
			if (flag)
			{
				return;
			}
			try
			{
				if (FxKit.Alive((GodotObject)(object)node))
				{
					((CanvasItem)node).Visible = true;
					((CanvasItem)node).Modulate = Colors.White;
				}
			}
			catch (Exception ex2)
			{
				DevLog.Warn("HandMoves (discard, restore): " + ex2.Message);
			}
		}

		private static void SpringShape(ref CardRide.Pose pose, float t, in CardRide.Pose a, in CardRide.Pose b)
		{
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			float num = Tuning.F("moves.acro_apex", 260f);
			pose.Position = FxKit.Bezier(b: new Vector2(Mathf.Lerp(a.Position.X, b.Position.X, 0.2f), a.Position.Y - num * 2f), a: a.Position, c: b.Position, t: t);
			float num2 = Mathf.Round(Mathf.Clamp(Tuning.F("moves.acro_turns", 1f), 0f, 3f));
			pose.Rotation = a.Rotation + Side(in a, in b) * ((float)Math.PI * 2f) * num2 * Smooth(Mathf.Clamp(t / 0.84f, 0f, 1f));
			pose.Scale = a.Scale.Lerp(b.Scale, Smooth(Mathf.Clamp((t - 0.5f) / 0.5f, 0f, 1f)));
			pose.Alpha = ((t < 0.8f) ? 1f : (1f - (t - 0.8f) / 0.2f));
		}

		private static void OnSelectionBegan(AbstractModel source)
		{
			try
			{
				_picking = DiscardsFrom(source);
				_pickLost = 0f;
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandMoves.OnSelectionBegan: " + ex);
			}
		}

		private static void OnSelectionEnded(AbstractModel source, IReadOnlyList<CardModel> cards)
		{
			_picking = false;
		}

		private static bool DiscardsFrom(AbstractModel source)
		{
			if (source == null)
			{
				return false;
			}
			Type type = ((object)source).GetType();
			if (Array.IndexOf(Discarders, type) >= 0)
			{
				return true;
			}
			string name = type.Name;
			if (!name.Contains("ToolsOfTheTrade") && !name.Contains("Gambl"))
			{
				return name.Contains("Discard");
			}
			return true;
		}

		private static bool Discards(CardModel card)
		{
			if (Array.IndexOf(Discarders, ((object)card).GetType()) < 0)
			{
				return false;
			}
			if (card is CalculatedGamble && Tuning.F("sly.hop_gamble", 0f) < 0.5f)
			{
				return false;
			}
			try
			{
				return card.CanPlay();
			}
			catch
			{
				return true;
			}
		}

		private static bool Hopping(float real)
		{
			CardModel carried = HeldFxManager.Carried;
			if (carried != _carried)
			{
				_carried = carried;
				_carriedDiscards = carried != null && Discards(carried);
			}
			if (_picking)
			{
				if (HandCards.Selecting)
				{
					_pickLost = 0f;
				}
				else if ((_pickLost += real) > 0.4f)
				{
					_picking = false;
				}
			}
			if (_discarding && (_discardFor += real) > 2.5f)
			{
				_discarding = false;
			}
			if (Tuning.F("sly.on", 1f) < 0.5f || Tuning.F("sly.hop", 0f) < 0.5f)
			{
				return false;
			}
			if (!_carriedDiscards && !_picking)
			{
				return _discarding;
			}
			return true;
		}

		private static void UpdateHop(float real, float dt)
		{
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Invalid comparison between Unknown and I4
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			bool flag = Hopping(real);
			if (!flag && Hoppers.Count == 0)
			{
				return;
			}
			CardModel carried = HeldFxManager.Carried;
			float rise = Mathf.Clamp(Tuning.F("sly.lean_rise", 0.5f), 0f, 1f);
			foreach (Hopper value2 in Hoppers.Values)
			{
				value2.Wanted = false;
			}
			if (flag)
			{
				CardModel val = HandCards.Focused();
				HandCards.Fill(Hand);
				int num = 0;
				for (int i = 0; i < Hand.Count; i++)
				{
					HandCards.Entry entry = Hand[i];
					if (!Sly(entry.Card) || entry.Card == carried || entry.Card == val || !FxKit.Alive((GodotObject)(object)entry.Holder) || !Shows(entry.Node, entry.Card) || CardRide.Riding(entry.Card) || Performing(entry.Card))
					{
						continue;
					}
					Hoppers.TryGetValue(entry.Node, out var value);
					if (value != null && value.Card != entry.Card)
					{
						value = null;
					}
					if (MouseOnTop(entry.Node, value?.Moved ?? Vector2.Zero, rise))
					{
						continue;
					}
					if (value == null)
					{
						Node parent = ((Node)entry.Node).GetParent();
						if (!(parent is NHandCardHolder))
						{
							continue;
						}
						value = new Hopper
						{
							Node = entry.Node,
							Card = entry.Card,
							Home = parent,
							Phase = (float)i * 0.37f - Mathf.Floor((float)i * 0.37f),
							Wait = 0.05f * (float)(num % 5)
						};
						Hoppers[entry.Node] = value;
					}
					num++;
					value.Wanted = true;
				}
			}
			float sway = Mathf.DegToRad(Mathf.Clamp(Tuning.F("sly.lean_sway", 3.2f), 0f, 12f));
			float num2 = Mathf.Clamp(Tuning.F("sly.lean_speed", 1f), 0.2f, 3f);
			bool flag2 = _carriedDiscards || _picking;
			HopScratch.Clear();
			HopScratch.AddRange(Hoppers.Values);
			foreach (Hopper item in HopScratch)
			{
				int num3;
				if (Shows(item.Node, item.Card) && ((Node)item.Node).GetParent() == item.Home)
				{
					CardPile pile = item.Card.Pile;
					if (pile != null && (int)pile.Type == 2)
					{
						num3 = ((item.Card == carried) ? 1 : 0);
						goto IL_02fd;
					}
				}
				num3 = 1;
				goto IL_02fd;
				IL_02fd:
				bool flag3 = (byte)num3 != 0;
				if (!flag3)
				{
					Spring(item, dt);
					item.Lean = FxKit.Damp(item.Lean, flag2 ? 1f : 0f, 6f, dt);
					flag3 = !item.Wanted && item.Out < 0.004f && Mathf.Abs(item.Speed) < 0.05f;
				}
				if (flag3)
				{
					Settle(item);
					Hoppers.Remove(item.Node);
				}
				else
				{
					item.Clock += dt * num2;
					Strain(item, rise, sway);
				}
			}
			HopScratch.Clear();
		}

		private static bool MouseOnTop(NCard node, Vector2 moved, float rise)
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (Tuning.F("sly.lean_dodge", 1f) < 0.5f)
				{
					return false;
				}
				Node parent = ((Node)node).GetParent();
				CanvasItem val = (CanvasItem)(object)((parent is CanvasItem) ? parent : null);
				if (val == null)
				{
					return false;
				}
				Transform2D globalTransform = val.GetGlobalTransform();
				Vector2 scale = globalTransform.Scale;
				Vector2 val2 = scale.Abs();
				CardStandIn.Pose(node, out var _, out var _, out var scale2);
				float num = 150f * scale2.X / Mathf.Max(0.05f, val2.X);
				float num2 = 211f * scale2.Y / Mathf.Max(0.05f, val2.Y);
				Vector2 val3 = globalTransform.AffineInverse() * val.GetGlobalMousePosition() - (((Control)node).Position - moved);
				return Mathf.Abs(val3.X) < num && val3.Y < 0f - num2 && val3.Y > 0f - num2 - 2f * num2 * rise - 12f;
			}
			catch
			{
				return false;
			}
		}

		private static void Spring(Hopper hopper, float dt)
		{
			float num = 0f;
			if (hopper.Wanted)
			{
				if (hopper.Wait > 0f)
				{
					hopper.Wait -= dt;
				}
				else
				{
					num = 1f;
				}
			}
			float num2 = ((num > 0f) ? 13f : 10f);
			float num3 = ((num > 0f) ? 0.68f : 0.9f);
			float num4 = Mathf.Clamp(dt, 0f, 0.1f);
			while (num4 > 0.0001f)
			{
				float num5 = Mathf.Min(num4, 0.016f);
				num4 -= num5;
				float num6 = num2 * num2 * (num - hopper.Out) - 2f * num3 * num2 * hopper.Speed;
				hopper.Speed += num6 * num5;
				hopper.Out += hopper.Speed * num5;
			}
			if (hopper.Out < 0f)
			{
				hopper.Out = 0f;
				hopper.Speed = 0f;
			}
		}

		private static void Strain(Hopper hopper, float rise, float sway)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			NCard node = hopper.Node;
			Node parent = ((Node)node).GetParent();
			CanvasItem val = (CanvasItem)(object)((parent is CanvasItem) ? parent : null);
			if (val != null)
			{
				CardStandIn.Pose(node, out var _, out var _, out var scale);
				Transform2D globalTransform = val.GetGlobalTransform();
				float num = Mathf.Max(0.05f, Mathf.Abs(globalTransform.Scale.Y));
				float num2 = 422f * Mathf.Clamp(scale.Y, 0.2f, 1.5f) / num;
				float num3 = Mathf.Clamp(hopper.Out, 0f, 1.3f);
				float clock = hopper.Clock;
				float num4 = hopper.Phase * ((float)Math.PI * 2f);
				float num5 = 0.62f * Mathf.Sin(clock * 3.3f + num4) + 0.38f * Mathf.Sin(clock * 5.7f + num4 * 1.9f + 1.3f);
				float num6 = sway * num5 * Mathf.Min(1f, num3) * hopper.Lean;
				float num7 = Mathf.Max(0f, Mathf.Sin(clock * 4.3f + num4 * 2.3f));
				float num8 = num2 * rise * num3 * (1f + 0.05f * num7 * num7);
				float num9 = num2 * 0.5f;
				Vector2 val2 = new Vector2(num9 * Mathf.Sin(num6), 0f - num8 + num9 * (1f - Mathf.Cos(num6)));
				((Control)node).Position = ((Control)node).Position + (val2 - hopper.Moved);
				((Control)node).Rotation = ((Control)node).Rotation + (num6 - hopper.Turned);
				hopper.Moved = val2;
				hopper.Turned = num6;
			}
		}

		private static void Settle(Hopper hopper)
		{
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				NCard node = hopper.Node;
				if (FxKit.Alive((GodotObject)(object)node) && node.Model == hopper.Card)
				{
					if (((Node)node).GetParent() == hopper.Home)
					{
						((Control)node).Position = ((Control)node).Position - hopper.Moved;
					}
					((Control)node).Rotation = ((Control)node).Rotation - hopper.Turned;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandMoves (lean, settle): " + ex.Message);
			}
			hopper.Moved = Vector2.Zero;
			hopper.Turned = 0f;
			hopper.Out = 0f;
			hopper.Speed = 0f;
		}

		private static void SettleAll()
		{
			foreach (Hopper value in Hoppers.Values)
			{
				Settle(value);
			}
			Hoppers.Clear();
			HopScratch.Clear();
		}

		private static void ClaimFlick(CardModel card, float seconds)
		{
			CardFlight.Claim(card, new CardFlight.Plan
			{
				HideTrail = true,
				Started = (NCard node, Vector2 start, Vector2 pile) =>
				{
					//IL_0022: Unknown result type (might be due to invalid IL or missing references)
					//IL_0023: Unknown result type (might be due to invalid IL or missing references)
					//IL_002d: Unknown result type (might be due to invalid IL or missing references)
					//IL_004b: Unknown result type (might be due to invalid IL or missing references)
					//IL_004c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0061: Unknown result type (might be due to invalid IL or missing references)
					try
					{
						SlyFlying[card] = node;
						Node uiLayer = GameRefs.UiLayer;
						if (FxKit.Alive((GodotObject)(object)uiLayer))
						{
							FxKit.Flash(uiLayer, start, With(Gold, 0.75f), 90f, 2f, 0.16);
							Brush.Sparks(uiLayer, start, Gold, 5, 260f, new Vector2(0f, -1f), 70f);
						}
					}
					catch (Exception ex)
					{
						DevLog.Warn("HandMoves (flick): " + ex);
					}
				}
			}, seconds);
		}

		private static void OnSlyPlayBegins(CardModel card)
		{
			try
			{
				if (card == null || !HandPace.On || Tuning.F("sly.on", 1f) < 0.5f)
				{
					return;
				}
				foreach (Watch watch in Watches)
				{
					if (watch.Card == card)
					{
						return;
					}
				}
				Watches.Add(new Watch
				{
					Card = card
				});
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandMoves.OnSlyPlayBegins: " + ex);
			}
		}

		private static void OnPlayerTurnBegins(Player player)
		{
			try
			{
				SlyFlying.Clear();
				Watches.Clear();
				_picking = false;
				_discarding = false;
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandMoves.OnPlayerTurnBegins: " + ex);
			}
		}

		private static NCard FindPlayNode(CardModel card, NCard flying)
		{
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Invalid comparison between Unknown and I8
			NCard val;
			try
			{
				val = NCard.FindOnTable(card, (PileType?)(PileType)5);
			}
			catch
			{
				val = null;
			}
			if (Shows(val, card))
			{
				return val;
			}
			Node uiLayer = GameRefs.UiLayer;
			if (!FxKit.Alive((GodotObject)(object)uiLayer))
			{
				return null;
			}
			int childCount = uiLayer.GetChildCount(false);
			for (int i = 0; i < childCount; i++)
			{
				Node child = uiLayer.GetChild(i, false);
				NCard val2 = (NCard)(object)((child is NCard) ? child : null);
				if (val2 != null && val2 != flying && Shows(val2, card) && (long)((Control)val2).MouseFilter != 2)
				{
					return val2;
				}
			}
			return null;
		}

		private static void UpdateSly(float real)
		{
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Invalid comparison between Unknown and I4
			for (int num = Vanished.Count - 1; num >= 0; num--)
			{
				Hidden hidden = Vanished[num];
				hidden.For += real;
				if (!(hidden.For < 1f) || !Shows(hidden.Node, hidden.Card))
				{
					CardStandIn.Show(hidden.Node);
					Vanished.RemoveAt(num);
				}
			}
			for (int num2 = Watches.Count - 1; num2 >= 0; num2--)
			{
				Watch watch = Watches[num2];
				watch.For += real;
				bool flag = watch.For > 0.5f;
				if (!flag && HandTrickWipe.Holds(watch.Card))
				{
					flag = true;
				}
				if (!flag)
				{
					CardPile pile = watch.Card.Pile;
					if (pile != null && (int)pile.Type == 5)
					{
						SlyFlying.TryGetValue(watch.Card, out var value);
						NCard val = FindPlayNode(watch.Card, value);
						if (val != null)
						{
							flag = DartBack(watch.Card, val, value);
						}
					}
				}
				if (flag)
				{
					Watches.RemoveAt(num2);
					SlyFlying.Remove(watch.Card);
				}
			}
		}

		private static bool DartBack(CardModel card, NCard node, NCard flying)
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			bool flag = flying != node && Shows(flying, card);
			CardRide.Pose? pose = null;
			if (flag)
			{
				CardStandIn.Pose(flying, out var centre, out var rotation, out var scale);
				pose = new CardRide.Pose
				{
					Position = centre,
					Rotation = rotation,
					Scale = scale,
					Alpha = 1f
				};
			}
			float time = Seconds("sly.dart_time", 0.15f);
			Streak streak = new Streak();
			if (!CardRide.ToCard(node, new CardRide.Spec
			{
				Duration = time,
				From = pose,
				Shape = DartShape,
				Frame = (NCard copy, CardRide.Pose pose2, float t) =>
				{
					//IL_0008: Unknown result type (might be due to invalid IL or missing references)
					//IL_0014: Unknown result type (might be due to invalid IL or missing references)
					//IL_001e: Unknown result type (might be due to invalid IL or missing references)
					Trail(streak, copy, pose2.Position, t, time, With(Gold, 0.85f), 13f, 170f);
				},
				Arrived = (CardRide.Pose pose2) =>
				{
					//IL_0010: Unknown result type (might be due to invalid IL or missing references)
					//IL_0015: Unknown result type (might be due to invalid IL or missing references)
					//IL_001f: Unknown result type (might be due to invalid IL or missing references)
					Node uiLayer = GameRefs.UiLayer;
					if (FxKit.Alive((GodotObject)(object)uiLayer))
					{
						FxKit.Flash(uiLayer, pose2.Position, With(Gold, 0.7f), 150f, 1.9f, 0.18);
					}
				}
			}))
			{
				return false;
			}
			if (flag)
			{
				CardStandIn.Hide(flying);
				Vanished.Add(new Hidden
				{
					Node = flying,
					Card = card
				});
			}
			return true;
		}

		private static void DartShape(ref CardRide.Pose pose, float t, in CardRide.Pose a, in CardRide.Pose b)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			float num = 1f - (1f - t) * (1f - t) * (1f - t);
			pose.Position = a.Position.Lerp(b.Position, num);
			pose.Rotation = Mathf.LerpAngle(a.Rotation, b.Rotation, num);
			pose.Alpha = 1f;
			Vector2 val = a.Scale.Lerp(b.Scale, num);
			float num2 = Tuning.F("sly.dart_min", 0.45f) * Mathf.Sqrt(Mathf.Max(0f, Mathf.Sin((float)Math.PI * t)));
			val = new Vector2(Mathf.Max(val.X, num2), Mathf.Max(val.Y, num2));
			Vector2 val2 = b.Position - a.Position;
			float num3 = val2.Length();
			if (num3 > 1f)
			{
				Vector2 val3 = val2 / num3;
				Vector2 val4 = val3.Rotated(0f - pose.Rotation);
				float num4 = 0.2f * Mathf.Sin((float)Math.PI * t);
				float num5 = Mathf.Abs(val4.X);
				float num6 = Mathf.Abs(val4.Y);
				val = new Vector2(val.X * (1f + num4 * (num5 - 0.5f * num6)), val.Y * (1f + num4 * (num6 - 0.5f * num5)));
			}
			pose.Scale = val;
		}

		private static void Update(double realDt)
		{
			float real = (float)realDt;
			float num = (float)(realDt * (double)Mathf.Clamp((float)Engine.TimeScale, 0f, 4f));
			_clock += num;
			try
			{
				StepActs(num);
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandMoves (acts): " + ex);
				try
				{
					EndAllActs();
				}
				catch (Exception ex2)
				{
					DevLog.Warn("HandMoves (acts, restore): " + ex2.Message);
				}
			}
			try
			{
				UpdateCover(real);
			}
			catch (Exception ex3)
			{
				DevLog.Warn("HandMoves (ghost): " + ex3);
				_ghostDraw = null;
				try
				{
					foreach (Hidden item in Covered)
					{
						CardStandIn.Show(item.Node);
					}
				}
				catch (Exception ex4)
				{
					DevLog.Warn("HandMoves (ghost, restore): " + ex4.Message);
				}
				Covered.Clear();
			}
			try
			{
				UpdateHop(real, num);
			}
			catch (Exception ex5)
			{
				DevLog.Warn("HandMoves (hop): " + ex5);
				_picking = false;
				_discarding = false;
				try
				{
					SettleAll();
				}
				catch (Exception ex6)
				{
					DevLog.Warn("HandMoves (hop, restore): " + ex6.Message);
				}
			}
			try
			{
				UpdateSly(real);
			}
			catch (Exception ex7)
			{
				DevLog.Warn("HandMoves (sly): " + ex7);
				Watches.Clear();
				SlyFlying.Clear();
			}
			try
			{
				UpdateTies(real, num);
			}
			catch (Exception ex8)
			{
				DevLog.Warn("HandMoves (threads): " + ex8);
				FreeTies();
			}
		}

		static HandMoves()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		}
	}
