// namespace: SilentFX.Cards
// type: SlyGold (class)
// source lines: 102440-103171
	public static class SlyGold
	{
		private sealed class State
		{
			public Node2D Body;

			public Sprite2D[] Strips;

			public Sprite2D[] Glint;

			public float Show;

			public float Phase;
		}

		private sealed class Sheen
		{
			public Node2D Body;

			public Sprite2D Bloom;

			public Sprite2D Band;

			public Sprite2D[] Strips;

			public float Side;

			public float Sweep;

			public bool Flashed;
		}

		private const string Key = "sly.gold";

		private const string WipeKey = "sly.wipe";

		private const float HalfW = 150f;

		private const float HalfH = 211f;

		private const int Tail = 6;

		private const float PollEvery = 0.125f;

		private static readonly Color Gold = new Color(0.98f, 0.84f, 0.45f, 1f);

		private static readonly Color Bright = new Color(1f, 0.95f, 0.76f, 1f);

		private static readonly List<HandCards.Entry> Hand = new List<HandCards.Entry>();

		private static readonly HashSet<CardModel> Marked = new HashSet<CardModel>(ReferenceEqualityComparer.Instance);

		private static readonly Dictionary<CardModel, ulong> Grace = new Dictionary<CardModel, ulong>(ReferenceEqualityComparer.Instance);

		private static readonly Dictionary<CardModel, ulong> Waiting = new Dictionary<CardModel, ulong>(ReferenceEqualityComparer.Instance);

		private static readonly List<CardModel> Scratch = new List<CardModel>();

		private static float _poll;

		private static bool _ready;

		private static bool _off;

		private static int _failures;

		public static void Init()
		{
			if (!_ready)
			{
				_ready = true;
				Ticker.Add(Update);
				HandFlow.SelectionEnded += OnSelectionEnded;
				HandFlow.PlayerTurnBegins += OnTurnBegins;
			}
		}

		public static void Reset()
		{
			try
			{
				CardDeco.RemoveAll("sly.gold", 0f);
				CardDeco.RemoveAll("sly.wipe", 0f);
			}
			catch (Exception ex)
			{
				DevLog.Warn("SlyGold.Reset: " + ex);
			}
			try
			{
				HandTrickFx.Reset();
			}
			catch (Exception ex2)
			{
				DevLog.Warn("SlyGold.Reset (wipe): " + ex2);
			}
			Marked.Clear();
			Grace.Clear();
			Waiting.Clear();
			Hand.Clear();
			_poll = 0f;
			_failures = 0;
			_off = false;
		}

		private static void Fail(string where, Exception e)
		{
			DevLog.Warn("SlyGold." + where + ": " + e);
			if (!_off && ++_failures >= 4)
			{
				_off = true;
				try
				{
					CardDeco.RemoveAll("sly.gold", 0f);
					CardDeco.RemoveAll("sly.wipe", 0f);
				}
				catch (Exception ex)
				{
					DevLog.Warn("SlyGold (switching off): " + ex.Message);
				}
				Marked.Clear();
				Grace.Clear();
				Waiting.Clear();
				DevLog.Warn("SlyGold: switched off for this fight after repeated failures");
			}
		}

		private static bool Passes(CardModel card)
		{
			try
			{
				if (card == null)
				{
					return false;
				}
				if (Tuning.F("sly.gold_planner", 1f) > 0.5f && MasterPlannerFx.IsTagged(card))
				{
					return true;
				}
				return card.IsSlyThisTurn && (card.Keywords == null || !card.Keywords.Contains((CardKeyword)6));
			}
			catch
			{
				return false;
			}
		}

		private static void Mark(CardModel card)
		{
			if (!(Tuning.F("sly.gold_edge", 0f) < 0.5f))
			{
				CardDeco.Handle handle = CardDeco.Attach(card, "sly.gold", Build, Tick);
				if (handle != null && handle.Alive)
				{
					Marked.Add(card);
				}
			}
		}

		private static void Update(double realDt)
		{
			if (_off)
			{
				return;
			}
			try
			{
				if (Waiting.Count > 0)
				{
					TryWaiting();
				}
				_poll -= (float)realDt;
				if (!(_poll > 0f))
				{
					_poll = 0.125f;
					Poll();
				}
			}
			catch (Exception e)
			{
				Fail("Update", e);
			}
		}

		private static void Poll()
		{
			HandCards.Fill(Hand);
			foreach (HandCards.Entry item in Hand)
			{
				CardModel card = item.Card;
				if (card != null && !Marked.Contains(card) && Passes(card))
				{
					Mark(card);
				}
			}
			if (Marked.Count == 0)
			{
				if (Grace.Count > 0 && Waiting.Count == 0)
				{
					Grace.Clear();
				}
				return;
			}
			ulong milliseconds = FxClock.Milliseconds;
			Scratch.Clear();
			foreach (CardModel item2 in Marked)
			{
				CardDeco.Handle handle = CardDeco.Find(item2, "sly.gold");
				ulong value;
				if (handle == null)
				{
					Scratch.Add(item2);
				}
				else if (!Passes(item2) && (!Grace.TryGetValue(item2, out value) || milliseconds >= value))
				{
					handle.Remove();
					Scratch.Add(item2);
				}
			}
			foreach (CardModel item3 in Scratch)
			{
				Marked.Remove(item3);
				Grace.Remove(item3);
			}
		}

		private static void OnTurnBegins(Player player)
		{
			try
			{
				CardDeco.RemoveAll("sly.gold");
				Marked.Clear();
				Grace.Clear();
				Waiting.Clear();
			}
			catch (Exception e)
			{
				Fail("OnTurnBegins", e);
			}
		}

		private static void OnSelectionEnded(AbstractModel source, IReadOnlyList<CardModel> cards)
		{
			try
			{
				if (_off || !(source is HandTrick) || cards == null)
				{
					return;
				}
				bool flag = HandTrickWipe.Chosen(source, cards);
				ulong milliseconds = FxClock.Milliseconds;
				foreach (CardModel card in cards)
				{
					if (card != null)
					{
						Grace[card] = milliseconds + 1500;
						if (!flag)
						{
							Gild(card, 1f, 0f);
						}
					}
				}
			}
			catch (Exception e)
			{
				Fail("OnSelectionEnded", e);
			}
		}

		internal static void Gild(CardModel card, float side, float seconds)
		{
			try
			{
				if (!_off && card != null && !(Tuning.F("sly.wipe_flash", 1f) < 0.5f))
				{
					Grace[card] = FxClock.Milliseconds + 1500;
					float sweep = (float.IsFinite(seconds) ? Mathf.Clamp(seconds, 0f, 0.6f) : 0f);
					float way = ((side < 0f) ? (-1f) : 1f);
					CardDeco.Remove(card, "sly.wipe", 0f);
					CardDeco.Attach(card, "sly.wipe", (CardDeco.Handle h) =>
					{
						BuildSheen(h, way, sweep);
					}, TickSheen);
				}
			}
			catch (Exception e)
			{
				Fail("Gild", e);
			}
		}

		private static void BuildSheen(CardDeco.Handle h, float side, float sweep)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Expected Obj, but got Unknown
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (h != null && FxKit.Alive((GodotObject)(object)h.Root))
				{
					Sheen sheen = new Sheen
					{
						Side = side,
						Sweep = sweep
					};
					sheen.Body = new Node2D
					{
						Name = StringName.op_Implicit("Wipe")
					};
					((Node)h.Root).AddChild((Node)(object)sheen.Body, false, (InternalMode)0);
					Color color = new Color(Gold.R, Gold.G, Gold.B, 0f);
					sheen.Bloom = FxKit.Glow(color, new Vector2(435f, 569.7f));
					((Node)sheen.Body).AddChild((Node)(object)sheen.Bloom, false, (InternalMode)0);
					sheen.Strips = new Sprite2D[4]
					{
						FxKit.Glow(color, new Vector2(435f, 46f)),
						FxKit.Glow(color, new Vector2(435f, 46f)),
						FxKit.Glow(color, new Vector2(46f, 611.9f)),
						FxKit.Glow(color, new Vector2(46f, 611.9f))
					};
					((Node2D)sheen.Strips[0]).Position = new Vector2(0f, -211f);
					((Node2D)sheen.Strips[1]).Position = new Vector2(0f, 211f);
					((Node2D)sheen.Strips[2]).Position = new Vector2(-150f, 0f);
					((Node2D)sheen.Strips[3]).Position = new Vector2(150f, 0f);
					Sprite2D[] strips = sheen.Strips;
					foreach (Sprite2D val in strips)
					{
						((Node)sheen.Body).AddChild((Node)(object)val, false, (InternalMode)0);
					}
					sheen.Band = FxKit.Glow(new Color(Bright.R, Bright.G, Bright.B, 0f), new Vector2(120f, 548.6f));
					((Node)sheen.Body).AddChild((Node)(object)sheen.Band, false, (InternalMode)0);
					h.State = sheen;
				}
			}
			catch (Exception e)
			{
				Fail("BuildSheen", e);
			}
		}

		private static void Paint(Sheen s, float bloom, float edges)
		{
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)s.Bloom))
			{
				((CanvasItem)s.Bloom).Modulate = new Color(Gold.R, Gold.G, Gold.B, Mathf.Clamp(bloom, 0f, 1f));
			}
			Color modulate = new Color(Bright.R, Bright.G, Bright.B, Mathf.Clamp(edges, 0f, 1f));
			Sprite2D[] strips = s.Strips;
			foreach (Sprite2D val in strips)
			{
				if (FxKit.Alive((GodotObject)(object)val))
				{
					((CanvasItem)val).Modulate = modulate;
				}
			}
		}

		private static void TickSheen(CardDeco.Handle h, float dt)
		{
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (_off || !(h?.State is Sheen sheen) || !FxKit.Alive((GodotObject)(object)sheen.Body))
				{
					h?.Remove(0f);
					return;
				}
				float time = h.Time;
				if (time < sheen.Sweep)
				{
					float num = time / sheen.Sweep;
					float num2 = Mathf.Lerp(-420f, 120f, num) - 24f;
					float num3 = Mathf.Clamp((num2 + 150f) / 50f, 0f, 1f) * Mathf.Clamp((180f - num2) / 50f, 0f, 1f);
					if (FxKit.Alive((GodotObject)(object)sheen.Band))
					{
						((Node2D)sheen.Band).Position = new Vector2(num2 * sheen.Side, 0f);
						((CanvasItem)sheen.Band).Modulate = new Color(Bright.R, Bright.G, Bright.B, 0.8f * num3);
					}
					float num4 = 0.3f * Mathf.Clamp((num2 + 150f) / 300f, 0f, 1f);
					Paint(sheen, num4, num4 * 0.5f);
					return;
				}
				if (!sheen.Flashed)
				{
					sheen.Flashed = true;
					if (FxKit.Alive((GodotObject)(object)sheen.Band))
					{
						((CanvasItem)sheen.Band).Modulate = new Color(Bright.R, Bright.G, Bright.B, 0f);
					}
					Uncovered(h);
				}
				float num5 = (time - sheen.Sweep) / Mathf.Clamp(Tuning.F("sly.wipe_fade", 0.42f), 0.1f, 1.5f);
				if (num5 >= 1f)
				{
					h.Remove(0f);
					return;
				}
				float num6 = (1f - num5) * (1f - num5);
				float num7 = Mathf.Clamp(Tuning.F("sly.wipe_alpha", 0.62f), 0f, 1f);
				Paint(sheen, num7 * num6, Mathf.Min(1f, num7 * 1.45f) * num6);
			}
			catch (Exception e)
			{
				Fail("TickSheen", e);
			}
		}

		private static void Uncovered(CardDeco.Handle h)
		{
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				CardModel card = h.Card;
				if (card != null && Tuning.F("sly.toss", 0f) > 0.5f)
				{
					if (!Launch(card))
					{
						Waiting[card] = FxClock.Milliseconds + 500;
					}
					return;
				}
				Node uiLayer = GameRefs.UiLayer;
				if (h.Host != null && FxKit.Alive((GodotObject)(object)uiLayer) && uiLayer.IsInsideTree())
				{
					Brush.Sparks(uiLayer, h.LastCentre, new Color(Gold.R, Gold.G, Gold.B, 0.9f), 7, 240f * Mathf.Clamp(h.LastScale.Y, 0.4f, 1.2f), new Vector2(0f, -1f), 80f);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("SlyGold (uncovered): " + ex.Message);
			}
		}

		private static void TryWaiting()
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Invalid comparison between Unknown and I4
			ulong milliseconds = FxClock.Milliseconds;
			Scratch.Clear();
			foreach (KeyValuePair<CardModel, ulong> item in Waiting)
			{
				CardModel key = item.Key;
				CardPile pile = key.Pile;
				if (pile == null || (int)pile.Type != 2)
				{
					Scratch.Add(key);
				}
				else if (Launch(key))
				{
					Scratch.Add(key);
				}
				else if (milliseconds >= item.Value)
				{
					Mark(key);
					Scratch.Add(key);
				}
			}
			foreach (CardModel item2 in Scratch)
			{
				Waiting.Remove(item2);
			}
			Scratch.Clear();
		}

		private static bool Launch(CardModel card)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Invalid comparison between Unknown and I4
			try
			{
				CardPile pile = card.Pile;
				if (pile == null || (int)pile.Type != 2)
				{
					return false;
				}
				NCard val = HandCards.NodeOf(card);
				if (!FxKit.Alive((GodotObject)(object)val) || !((Node)val).IsInsideTree())
				{
					return false;
				}
				CardRide.Spec spec = new CardRide.Spec
				{
					Duration = Mathf.Clamp(Tuning.F("sly.toss_time", 0.32f), 0.1f, 0.35f),
					Shape = Flip,
					Arrived = (CardRide.Pose pose) =>
					{
						Landed(card, pose);
					}
				};
				return CardRide.ToCard(val, spec);
			}
			catch (Exception ex)
			{
				DevLog.Warn("SlyGold.Launch: " + ex);
				try
				{
					CardRide.Finish(card);
				}
				catch (Exception ex2)
				{
					DevLog.Warn("SlyGold.Launch (restore): " + ex2.Message);
				}
				return false;
			}
		}

		private static void Flip(ref CardRide.Pose pose, float t, in CardRide.Pose from, in CardRide.Pose to)
		{
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			float num = Mathf.Sin((float)Math.PI * Mathf.Clamp(t, 0f, 1f));
			float num2 = 1f + 0.08f * num;
			float num3 = Tuning.F("sly.toss_hop", 70f) * Mathf.Max(0.2f, to.Scale.Y);
			float num4 = Mathf.Max(0.05f, Mathf.Abs(Mathf.Cos((float)Math.PI * 2f * t)));
			pose.Position = to.Position + new Vector2(0f, (0f - num3) * num);
			pose.Rotation = to.Rotation;
			pose.Scale = new Vector2(to.Scale.X * num4 * num2, to.Scale.Y * num2);
			pose.Alpha = 1f;
		}

		private static void Landed(CardModel card, CardRide.Pose pose)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Invalid comparison between Unknown and I4
			try
			{
				Node uiLayer = GameRefs.UiLayer;
				if (FxKit.Alive((GodotObject)(object)uiLayer) && uiLayer.IsInsideTree())
				{
					FxKit.Flash(uiLayer, pose.Position, new Color(Gold.R, Gold.G, Gold.B, 0.75f), 230f * Mathf.Max(0.3f, pose.Scale.Y), 2f, 0.26);
				}
				if (card != null)
				{
					CardPile pile = card.Pile;
					if (pile != null && (int)pile.Type == 2)
					{
						Grace[card] = FxClock.Milliseconds + 1000;
						Mark(card);
					}
				}
			}
			catch (Exception e)
			{
				Fail("Landed", e);
			}
		}

		private static void Build(CardDeco.Handle h)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Expected Obj, but got Unknown
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (h != null && FxKit.Alive((GodotObject)(object)h.Root))
				{
					State state = new State
					{
						Phase = GD.Randf()
					};
					state.Body = new Node2D
					{
						Name = StringName.op_Implicit("Gold"),
						Modulate = new Color(1f, 1f, 1f, 0f)
					};
					((Node)h.Root).AddChild((Node)(object)state.Body, false, (InternalMode)0);
					float num = Mathf.Clamp(Tuning.F("sly.gold_width", 18f), 6f, 60f) * 2f;
					Color color = new Color(Gold.R, Gold.G, Gold.B, 0f);
					state.Strips = new Sprite2D[4]
					{
						FxKit.Glow(color, new Vector2(435f, num)),
						FxKit.Glow(color, new Vector2(435f, num)),
						FxKit.Glow(color, new Vector2(num, 611.9f)),
						FxKit.Glow(color, new Vector2(num, 611.9f))
					};
					((Node2D)state.Strips[0]).Position = new Vector2(0f, -211f);
					((Node2D)state.Strips[1]).Position = new Vector2(0f, 211f);
					((Node2D)state.Strips[2]).Position = new Vector2(-150f, 0f);
					((Node2D)state.Strips[3]).Position = new Vector2(150f, 0f);
					Sprite2D[] strips = state.Strips;
					foreach (Sprite2D val in strips)
					{
						((Node)state.Body).AddChild((Node)(object)val, false, (InternalMode)0);
					}
					state.Glint = new Sprite2D[7];
					for (int num2 = 6; num2 >= 0; num2--)
					{
						float num3 = ((num2 == 0) ? 34f : Mathf.Lerp(26f, 10f, (float)num2 / 6f));
						state.Glint[num2] = FxKit.Glow(new Color(Bright.R, Bright.G, Bright.B, 0f), new Vector2(num3, num3));
						((Node)state.Body).AddChild((Node)(object)state.Glint[num2], false, (InternalMode)0);
					}
					h.State = state;
				}
			}
			catch (Exception e)
			{
				Fail("Build", e);
			}
		}

		private static void Tick(CardDeco.Handle h, float dt)
		{
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (_off || !(h?.State is State state) || !FxKit.Alive((GodotObject)(object)state.Body))
				{
					return;
				}
				state.Show = Mathf.Min(1f, state.Show + dt / 0.2f);
				((CanvasItem)state.Body).Modulate = new Color(1f, 1f, 1f, state.Show);
				float num = Mathf.Clamp(Tuning.F("sly.gold_alpha", 0.5f), 0f, 1f);
				float num2 = 0.82f + 0.18f * Mathf.Sin(h.Time * 2.1f + state.Phase * ((float)Math.PI * 2f));
				Color modulate = new Color(Gold.R, Gold.G, Gold.B, num * num2);
				Sprite2D[] strips = state.Strips;
				foreach (Sprite2D val in strips)
				{
					if (FxKit.Alive((GodotObject)(object)val))
					{
						((CanvasItem)val).Modulate = modulate;
					}
				}
				float num3 = Mathf.Max(0.5f, Tuning.F("sly.glint_period", 2.4f));
				float num4 = 1444f;
				float num5 = h.Time / num3 + state.Phase;
				float num6 = (num5 - Mathf.Floor(num5)) * num4;
				for (int j = 0; j <= 6; j++)
				{
					Sprite2D val2 = state.Glint[j];
					if (FxKit.Alive((GodotObject)(object)val2))
					{
						((Node2D)val2).Position = Border(num6 - (float)j * 13f);
						float num7 = 1f - (float)j / 7f;
						((CanvasItem)val2).Modulate = new Color(Bright.R, Bright.G, Bright.B, (j == 0) ? 0.95f : (0.55f * num7 * num7));
					}
				}
			}
			catch (Exception e)
			{
				Fail("Tick", e);
			}
		}

		private static Vector2 Border(float d)
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			float num = 300f;
			float num2 = 422f;
			float num3 = 2f * (num + num2);
			d -= Mathf.Floor(d / num3) * num3;
			if (d < num)
			{
				return new Vector2(-150f + d, -211f);
			}
			d -= num;
			if (d < num2)
			{
				return new Vector2(150f, -211f + d);
			}
			d -= num2;
			if (d < num)
			{
				return new Vector2(150f - d, 211f);
			}
			d -= num;
			return new Vector2(-150f, 211f - d);
		}

		static SlyGold()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		}
	}
