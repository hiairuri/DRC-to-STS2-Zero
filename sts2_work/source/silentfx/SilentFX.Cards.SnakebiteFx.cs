// namespace: SilentFX.Cards
// type: SnakebiteFx (class)
// source lines: 103172-104109
	public sealed class SnakebiteFx : CardFx
	{
		private enum Taper
		{
			Body,
			Jaw,
			Loop
		}

		private sealed class Strand
		{
			private readonly Line2D[] _lines = new Line2D[3];

			private readonly float[] _widths = new float[3];

			public Strand(Node parent, float width, Color venom, Taper taper)
			{
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0049: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_0081: Unknown result type (might be due to invalid IL or missing references)
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				Color color = new Color(0.92f, 1f, 0.84f, 0.95f);
				Make(0, parent, width * 2.6f, new Color(venom.R, venom.G, venom.B, 0.22f), taper);
				Make(1, parent, width, new Color(venom.R, venom.G, venom.B, 0.92f), taper);
				Make(2, parent, width * 0.3f, color, taper);
			}

			private void Make(int i, Node parent, float width, Color color, Taper taper)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Expected Obj, but got Unknown
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Expected Obj, but got Unknown
				//IL_0061: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
				//IL_011a: Unknown result type (might be due to invalid IL or missing references)
				//IL_013e: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d6: Expected Obj, but got Unknown
				//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
				//IL_0202: Unknown result type (might be due to invalid IL or missing references)
				//IL_0209: Unknown result type (might be due to invalid IL or missing references)
				//IL_0215: Unknown result type (might be due to invalid IL or missing references)
				//IL_021a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0221: Unknown result type (might be due to invalid IL or missing references)
				//IL_0223: Unknown result type (might be due to invalid IL or missing references)
				//IL_022a: Unknown result type (might be due to invalid IL or missing references)
				//IL_022c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0164: Unknown result type (might be due to invalid IL or missing references)
				//IL_0188: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
				Line2D val = new Line2D
				{
					Width = width,
					DefaultColor = color,
					JointMode = (LineJointMode)2,
					BeginCapMode = (LineCapMode)2,
					EndCapMode = (LineCapMode)2,
					Material = (Material)(object)FxKit.Additive,
					Antialiased = true
				};
				Curve val2 = new Curve();
				switch (taper)
				{
				case Taper.Body:
					val2.AddPoint(new Vector2(0f, 0.05f), 0f, 0f, (TangentMode)0, (TangentMode)0);
					val2.AddPoint(new Vector2(0.5f, 0.8f), 0f, 0f, (TangentMode)0, (TangentMode)0);
					val2.AddPoint(new Vector2(0.86f, 1f), 0f, 0f, (TangentMode)0, (TangentMode)0);
					val2.AddPoint(new Vector2(1f, 0.72f), 0f, 0f, (TangentMode)0, (TangentMode)0);
					break;
				case Taper.Jaw:
					val2.AddPoint(new Vector2(0f, 0.95f), 0f, 0f, (TangentMode)0, (TangentMode)0);
					val2.AddPoint(new Vector2(0.7f, 0.62f), 0f, 0f, (TangentMode)0, (TangentMode)0);
					val2.AddPoint(new Vector2(1f, 0.06f), 0f, 0f, (TangentMode)0, (TangentMode)0);
					break;
				default:
					val2.AddPoint(new Vector2(0f, 0.5f), 0f, 0f, (TangentMode)0, (TangentMode)0);
					val2.AddPoint(new Vector2(0.5f, 1f), 0f, 0f, (TangentMode)0, (TangentMode)0);
					val2.AddPoint(new Vector2(1f, 0.5f), 0f, 0f, (TangentMode)0, (TangentMode)0);
					break;
				}
				val.WidthCurve = val2;
				if (taper == Taper.Body)
				{
					Gradient val3 = new Gradient();
					val3.Offsets = new float[3] { 0f, 0.2f, 1f };
					val3.Colors = new Color[3]
					{
						new Color(color.R, color.G, color.B, 0f),
						color,
						color
					};
					val.Gradient = val3;
				}
				parent.AddChild((Node)(object)val, false, (InternalMode)0);
				_lines[i] = val;
				_widths[i] = width;
			}

			public void Set(Vector2[] points, float alpha = 1f, float thickness = 1f, float whiten = 0f)
			{
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
				for (int i = 0; i < 3; i++)
				{
					Line2D val = _lines[i];
					if (FxKit.Alive((GodotObject)(object)val))
					{
						((CanvasItem)val).Visible = alpha > 0.01f && points != null && points.Length >= 2;
						if (((CanvasItem)val).Visible)
						{
							val.Points = points;
							val.Width = _widths[i] * thickness;
							((CanvasItem)val).Modulate = new Color(1f + whiten, 1f + whiten, 1f + whiten, alpha);
						}
					}
				}
			}

			public void Hide()
			{
				Line2D[] lines = _lines;
				foreach (Line2D val in lines)
				{
					if (FxKit.Alive((GodotObject)(object)val))
					{
						((CanvasItem)val).Visible = false;
					}
				}
			}
		}

		private sealed class CoverState
		{
			internal ulong Until;

			internal Node Room;

			internal CardModel Card;

			internal Creature Owner;

			internal Node OwnerNode;

			internal Node Parent;

			internal Node Holder;

			internal int Epoch;

			internal bool Finished;

			internal bool Retired;
		}

		private const int Joints = 24;

		private const int JawPoints = 20;

		private const int LoopPoints = 60;

		private static CardModel _covering;

		private static ulong _coverUntil;

		private static Node _coverRoom;

		private static readonly ConditionalWeakTable<object, CoverState> Covers = new ConditionalWeakTable<object, CoverState>();

		private static int _coverEpoch;

		public static bool CoversBite
		{
			get
			{
				try
				{
					object currentInvocation = MultiplayerFx.CurrentInvocation;
					if (currentInvocation != null && Covers.TryGetValue(currentInvocation, out var value))
					{
						if (!ValidCover(value))
						{
							value.Retired = true;
							return false;
						}
						if (value.Card != MultiplayerFx.CurrentCard || value.Finished || value.Holder == null || FxClock.Milliseconds >= value.Until)
						{
							return false;
						}
						return Tuning.F("snake.keep_bite", 0f) < 0.5f;
					}
					if (currentInvocation != null || MultiplayerFx.Active)
					{
						return false;
					}
					if (_covering == null)
					{
						return false;
					}
					if ((object)_coverRoom != GameRefs.CombatRoom || FxClock.Milliseconds >= _coverUntil)
					{
						_covering = null;
						_coverRoom = null;
						return false;
					}
					return Tuning.F("snake.keep_bite", 0f) < 0.5f;
				}
				catch (Exception ex)
				{
					DevLog.Warn("Snakebite cover: " + ex);
					return false;
				}
			}
		}

		private static bool ValidCover(CoverState cover)
		{
			if (!cover.Retired && cover.Epoch == _coverEpoch && MultiplayerFx.Active && FxKit.Alive((GodotObject)(object)cover.Room) && cover.Room.IsInsideTree() && (object)cover.Room == GameRefs.CombatRoom && FxKit.Alive((GodotObject)(object)cover.Parent) && cover.Parent.IsInsideTree() && (cover.Holder == null || (FxKit.Alive((GodotObject)(object)cover.Holder) && cover.Holder.IsInsideTree())) && cover.Owner != null && cover.Owner.IsAlive)
			{
				Creature owner = cover.Owner;
				CardModel card = cover.Card;
				object obj;
				if (card == null)
				{
					obj = null;
				}
				else
				{
					Player owner2 = card.Owner;
					obj = ((owner2 != null) ? owner2.Creature : null);
				}
				if (owner == obj)
				{
					CardModel card2 = cover.Card;
					if (((card2 != null) ? card2.Owner : null) == cover.Owner.Player && FxKit.Alive((GodotObject)(object)cover.OwnerNode) && cover.OwnerNode.IsInsideTree())
					{
						NCombatRoom combatRoom = GameRefs.CombatRoom;
						if ((object)((combatRoom != null) ? combatRoom.GetCreatureNode(cover.Owner) : null) == cover.OwnerNode)
						{
							CombatManager instance = CombatManager.Instance;
							CombatState val = ((instance != null) ? instance.DebugOnlyGetState() : null);
							if (val == null || (object)cover.Owner.CombatState != val)
							{
								return false;
							}
							foreach (Player player in val.Players)
							{
								if (player == cover.Owner.Player)
								{
									return true;
								}
							}
							return false;
						}
					}
				}
			}
			return false;
		}

		public static void Reset()
		{
			_coverEpoch++;
			_covering = null;
			_coverRoom = null;
			_coverUntil = 0uL;
		}

		public static void NoteCardFinished(CardModel card)
		{
			object currentInvocation = MultiplayerFx.CurrentInvocation;
			if (currentInvocation != null && Covers.TryGetValue(currentInvocation, out var value))
			{
				if (card != null && card == value.Card)
				{
					value.Finished = true;
				}
			}
			else if (currentInvocation == null && !MultiplayerFx.Active && card != null && card == _covering)
			{
				_covering = null;
			}
		}

		public override void OnPlay(CardFxContext c)
		{
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_051a: Unknown result type (might be due to invalid IL or missing references)
			//IL_051f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0534: Expected Obj, but got Unknown
			//IL_0594: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_062b: Unknown result type (might be due to invalid IL or missing references)
			//IL_063a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0670: Unknown result type (might be due to invalid IL or missing references)
			//IL_067f: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f0: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node parent = c.FrontParent;
				if (!c.HasTarget || !FxKit.Alive((GodotObject)(object)parent))
				{
					return;
				}
				CoverState cover = null;
				if (c.Card != null)
				{
					if (MultiplayerFx.Active)
					{
						object currentInvocation = MultiplayerFx.CurrentInvocation;
						Player owner = c.Card.Owner;
						Creature val = ((owner != null) ? owner.Creature : null);
						NCombatRoom combatRoom = GameRefs.CombatRoom;
						if (currentInvocation == null || MultiplayerFx.CurrentCard != c.Card || Covers.TryGetValue(currentInvocation, out var _) || val == null || !FxKit.Alive((GodotObject)(object)combatRoom))
						{
							return;
						}
						cover = new CoverState
						{
							Until = FxClock.Milliseconds + 4000,
							Room = (Node)(object)combatRoom,
							Card = c.Card,
							Owner = val,
							OwnerNode = (Node)(object)combatRoom.GetCreatureNode(val),
							Parent = parent,
							Epoch = _coverEpoch
						};
						if (!ValidCover(cover))
						{
							return;
						}
						Covers.Add(currentInvocation, cover);
					}
					else
					{
						_covering = c.Card;
						_coverUntil = FxClock.Milliseconds + 4000;
						_coverRoom = (Node)(object)GameRefs.CombatRoom;
					}
				}
				Vector2 from = ((c.Held is IHeldOrigin heldOrigin) ? heldOrigin.Origin : c.WeaponHand);
				float width = Mathf.Max(c.TargetWidth, 130f);
				float height = Mathf.Max(c.TargetHeight, 1f);
				float num = Mathf.Clamp(Mathf.Max(width, height) / 280f, 1f, 2.2f);
				float root = Mathf.Sqrt(num);
				Vector2 mid = new Vector2(c.TargetChest.X, c.TargetFeet.Y - height * 0.5f);
				float way = ((mid.X >= from.X) ? 1f : (-1f));
				float hug = Tuning.F("snake.hug", 14f);
				float a = width * 0.5f + hug;
				float b = height * 0.5f + hug;
				Vector2 entry = mid + new Vector2((0f - way) * a, 0f);
				Vector2 exit = mid + new Vector2(way * a, 0f);
				Vector2 val2 = entry - from;
				float num2 = val2.Length();
				if (num2 < 1f)
				{
					return;
				}
				Vector2 side = new Vector2(0f - val2.Y, val2.X) / num2;
				Vector2 apex = (from + entry) * 0.5f + new Vector2(0f, 0f - Mathf.Min(170f, num2 * Tuning.F("snake.arc", 0.2f)));
				Color venom = PoisonFx.Venom;
				float num3 = Tuning.F("snake.beam", 22f) * Mathf.Pow(num, 0.75f);
				float sway = Tuning.F("snake.sway", 24f) * root;
				float bodyLen = Mathf.Min(0.9f, Tuning.F("snake.length", 0.58f) * (1f + 0.25f * (num - 1f)));
				float t1 = Mathf.Max(0.05f, Tuning.F("snake.flight", 0.28f));
				float t2 = Mathf.Max(0.05f, Tuning.F("snake.wrap", 0.24f));
				float t3 = Mathf.Max(0.05f, Tuning.F("snake.pull", 0.32f));
				float t4 = Mathf.Max(0.05f, Tuning.F("snake.leave", 0.4f));
				float drag = Tuning.F("snake.drag", 190f) * Mathf.Pow(num, 0.6f);
				float flee = Tuning.F("snake.flee", 760f);
				float tailTime = Tuning.F("snake.tail", 0.27f);
				float headSize = 60f * Mathf.Pow(num, 0.75f);
				float total = t1 + t2 + t3 + t4;
				FxKit.Flash(parent, from, new Color(venom.R, venom.G, venom.B, 0.85f), 80f, 2.2f, 0.22);
				Node2D holder = new Node2D
				{
					Name = StringName.op_Implicit("SilentFX_Snakebite")
				};
				parent.AddChild((Node)(object)holder, false, (InternalMode)0);
				if (cover != null)
				{
					cover.Holder = (Node)(object)holder;
				}
				FxKit.FreeAfter((Node)(object)holder, total + Mathf.Max(0f, tailTime) + 0.2f);
				Strand body = new Strand((Node)(object)holder, num3, venom, Taper.Body);
				Strand upper = new Strand((Node)(object)holder, num3 * 0.8f, venom, Taper.Jaw);
				Strand lower = new Strand((Node)(object)holder, num3 * 0.8f, venom, Taper.Jaw);
				Strand loop = new Strand((Node)(object)holder, num3 * 0.8f, venom, Taper.Loop);
				Sprite2D head = FxKit.Glow(new Color(venom.R, venom.G, venom.B, 0.95f), new Vector2(60f, 60f));
				Sprite2D head2 = FxKit.Glow(new Color(venom.R, venom.G, venom.B, 0.95f), new Vector2(60f, 60f));
				Sprite2D eye = FxKit.Glow(new Color(0.92f, 1f, 0.84f, 0.95f), new Vector2(24f, 24f));
				Sprite2D eye2 = FxKit.Glow(new Color(0.92f, 1f, 0.84f, 0.95f), new Vector2(24f, 24f));
				((Node)holder).AddChild((Node)(object)head, false, (InternalMode)0);
				((Node)holder).AddChild((Node)(object)head2, false, (InternalMode)0);
				((Node)holder).AddChild((Node)(object)eye, false, (InternalMode)0);
				((Node)holder).AddChild((Node)(object)eye2, false, (InternalMode)0);
				((CanvasItem)head2).Visible = false;
				((CanvasItem)eye2).Visible = false;
				Vector2[] bodyPoints = new Vector2[25];
				Vector2[] jawUp = new Vector2[20];
				Vector2[] jawDown = new Vector2[20];
				Vector2[] loopPoints = new Vector2[60];
				Vector2[] loopShape = new Vector2[60];
				bool opened = false;
				bool bitten = false;
				bool snapped = false;
				float farEndAtSnap = FarEnd(1f);
				FxKit.Drive((Node)(object)holder, total, (float u) =>
				{
					//IL_0658: Unknown result type (might be due to invalid IL or missing references)
					//IL_0668: Unknown result type (might be due to invalid IL or missing references)
					//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
					//IL_0327: Unknown result type (might be due to invalid IL or missing references)
					//IL_0340: Unknown result type (might be due to invalid IL or missing references)
					//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
					//IL_06e4: Unknown result type (might be due to invalid IL or missing references)
					//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
					//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
					//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
					//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
					//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
					//IL_0121: Unknown result type (might be due to invalid IL or missing references)
					//IL_0124: Unknown result type (might be due to invalid IL or missing references)
					//IL_014b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0150: Unknown result type (might be due to invalid IL or missing references)
					//IL_0155: Unknown result type (might be due to invalid IL or missing references)
					//IL_016a: Unknown result type (might be due to invalid IL or missing references)
					//IL_016c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0171: Unknown result type (might be due to invalid IL or missing references)
					//IL_0415: Unknown result type (might be due to invalid IL or missing references)
					//IL_041f: Unknown result type (might be due to invalid IL or missing references)
					//IL_0424: Unknown result type (might be due to invalid IL or missing references)
					//IL_0429: Unknown result type (might be due to invalid IL or missing references)
					//IL_042e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0442: Unknown result type (might be due to invalid IL or missing references)
					//IL_044b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0450: Unknown result type (might be due to invalid IL or missing references)
					//IL_0455: Unknown result type (might be due to invalid IL or missing references)
					//IL_045a: Unknown result type (might be due to invalid IL or missing references)
					//IL_04de: Unknown result type (might be due to invalid IL or missing references)
					//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
					//IL_050c: Unknown result type (might be due to invalid IL or missing references)
					//IL_053b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0540: Unknown result type (might be due to invalid IL or missing references)
					//IL_0568: Unknown result type (might be due to invalid IL or missing references)
					//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
					//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
					//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
					//IL_0848: Unknown result type (might be due to invalid IL or missing references)
					//IL_084d: Unknown result type (might be due to invalid IL or missing references)
					//IL_084f: Unknown result type (might be due to invalid IL or missing references)
					//IL_089a: Unknown result type (might be due to invalid IL or missing references)
					//IL_089f: Unknown result type (might be due to invalid IL or missing references)
					//IL_08a4: Unknown result type (might be due to invalid IL or missing references)
					//IL_08b4: Unknown result type (might be due to invalid IL or missing references)
					//IL_08b6: Unknown result type (might be due to invalid IL or missing references)
					//IL_08bb: Unknown result type (might be due to invalid IL or missing references)
					//IL_0910: Unknown result type (might be due to invalid IL or missing references)
					//IL_0915: Unknown result type (might be due to invalid IL or missing references)
					//IL_091a: Unknown result type (might be due to invalid IL or missing references)
					//IL_091c: Unknown result type (might be due to invalid IL or missing references)
					//IL_092c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0931: Unknown result type (might be due to invalid IL or missing references)
					//IL_0936: Unknown result type (might be due to invalid IL or missing references)
					//IL_093b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0772: Unknown result type (might be due to invalid IL or missing references)
					//IL_0782: Unknown result type (might be due to invalid IL or missing references)
					//IL_0788: Unknown result type (might be due to invalid IL or missing references)
					//IL_078d: Unknown result type (might be due to invalid IL or missing references)
					//IL_079f: Unknown result type (might be due to invalid IL or missing references)
					//IL_07a4: Unknown result type (might be due to invalid IL or missing references)
					//IL_07a9: Unknown result type (might be due to invalid IL or missing references)
					//IL_07ae: Unknown result type (might be due to invalid IL or missing references)
					//IL_096a: Unknown result type (might be due to invalid IL or missing references)
					//IL_096c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0958: Unknown result type (might be due to invalid IL or missing references)
					//IL_0250: Unknown result type (might be due to invalid IL or missing references)
					//IL_0255: Unknown result type (might be due to invalid IL or missing references)
					//IL_0239: Unknown result type (might be due to invalid IL or missing references)
					//IL_025a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0268: Unknown result type (might be due to invalid IL or missing references)
					//IL_027a: Unknown result type (might be due to invalid IL or missing references)
					//IL_027f: Unknown result type (might be due to invalid IL or missing references)
					//IL_0284: Unknown result type (might be due to invalid IL or missing references)
					//IL_0261: Unknown result type (might be due to invalid IL or missing references)
					//IL_0289: Unknown result type (might be due to invalid IL or missing references)
					//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
					//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
					//IL_029a: Unknown result type (might be due to invalid IL or missing references)
					//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
					//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
					//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
					if (FxKit.Alive((GodotObject)(object)holder))
					{
						if (cover != null && !ValidCover(cover))
						{
							cover.Retired = true;
							((Node)holder).QueueFree();
						}
						else
						{
							float num4 = u * total;
							if (num4 < t1 + t2)
							{
								float num6;
								if (num4 < t1)
								{
									float num5 = num4 / t1;
									num6 = num5 * (1.9f + num5 * (num5 - 1.9f));
								}
								else
								{
									num6 = 1f + bodyLen * ((num4 - t1) / t2);
								}
								int num7 = 0;
								for (int i = 0; i <= 24; i++)
								{
									float num8 = (float)i / 24f;
									float num9 = num6 - bodyLen * (1f - num8);
									if (num9 > 1f)
									{
										break;
									}
									num9 = Mathf.Max(num9, 0f);
									Vector2 val3 = FxKit.Bezier(from, apex, entry, num9);
									float num10 = Mathf.Sin((float)Math.PI * num9);
									float num11 = Mathf.Lerp(1.25f, 0.5f, num8);
									val3 += side * (Mathf.Sin(num9 * 18f - num4 * 30f) * sway * num10 * num11);
									bodyPoints[num7++] = holder.ToLocal(val3);
								}
								if (num4 >= t1 && num7 > 0 && num7 <= 24)
								{
									bodyPoints[num7++] = holder.ToLocal(entry);
								}
								if (num7 >= 2)
								{
									Vector2[] array = new Vector2[num7];
									Array.Copy(bodyPoints, array, num7);
									body.Set(array);
								}
								else
								{
									body.Hide();
								}
								if (num4 < t1)
								{
									float size = 1f + 0.8f * Mathf.SmoothStep(0.72f, 1f, num4 / t1);
									Vector2 val4 = ((num7 >= 1) ? holder.ToGlobal(bodyPoints[num7 - 1]) : from);
									Vector2 going = ((num7 >= 2) ? (val4 - holder.ToGlobal(bodyPoints[num7 - 2])) : Vector2.Zero);
									if (going.LengthSquared() < 0.01f)
									{
										going = apex - from;
									}
									PlaceHead(head, eye, val4, going, size, 1f);
									OneHead();
									upper.Hide();
									lower.Hide();
									loop.Hide();
									return;
								}
							}
							if (num4 < t1 + t2)
							{
								if (!opened)
								{
									opened = true;
									if (FxKit.Alive((GodotObject)(object)parent))
									{
										FxKit.Flash(parent, entry, new Color(0.92f, 1f, 0.84f, 0.75f), 46f * root, 1.8f, 0.14);
									}
								}
								float num12 = (num4 - t1) / t2;
								float num13 = (num12 - 0.72f) / 0.28f;
								float num14 = ((num12 < 0.72f) ? (0.8f * Mathf.Sin(num12 / 0.72f * (float)Math.PI * 0.5f)) : (0.8f + 0.2f * num13 * num13));
								for (int j = 0; j < 20; j++)
								{
									float num15 = num14 * (float)Math.PI * (float)j / 19f;
									float num16 = (0f - way) * a * Mathf.Cos(num15);
									float num17 = b * Mathf.Sin(num15);
									jawUp[j] = holder.ToLocal(mid + new Vector2(num16, 0f - num17));
									jawDown[j] = holder.ToLocal(mid + new Vector2(num16, num17));
								}
								upper.Set(jawUp);
								lower.Set(jawDown);
								loop.Hide();
								float num18 = num14 * (float)Math.PI;
								PlaceHead(head, eye, holder.ToGlobal(jawUp[19]), new Vector2(way * a * Mathf.Sin(num18), (0f - b) * Mathf.Cos(num18)), 0.62f, 1f);
								PlaceHead(head2, eye2, holder.ToGlobal(jawDown[19]), new Vector2(way * a * Mathf.Sin(num18), b * Mathf.Cos(num18)), 0.62f, 1f);
							}
							else
							{
								if (!bitten)
								{
									bitten = true;
									if (FxKit.Alive((GodotObject)(object)parent))
									{
										Bite(parent, exit, way, root);
									}
								}
								float num19 = num4 - t1 - t2;
								if (num19 < t3)
								{
									body.Hide();
									upper.Hide();
									lower.Hide();
									float num20 = num19 / t3;
									ShapeLoop(num20, num4);
									loop.Set(loopPoints, 1f, Mathf.Lerp(1f, 0.42f, Pull(num20)), 0.8f * Mathf.SmoothStep(0.55f, 1f, num20));
									PlaceHead(head, eye, HeadAfter(num19), new Vector2(way, 0f), 1f, 1f);
									OneHead();
								}
								else
								{
									if (!snapped)
									{
										snapped = true;
										upper.Hide();
										lower.Hide();
										ShapeLoop(1f, num4);
										if (FxKit.Alive((GodotObject)(object)parent))
										{
											Burst(parent, mid, width, height, new Vector2(way, 0f));
										}
									}
									float num21 = Mathf.Min(1f, (num19 - t3) / t4);
									float num22 = Mathf.Clamp(num21 / 0.26f, 0f, 1f);
									int num23 = (int)((1f - (1f - num22) * (1f - num22)) * 28f);
									int num24 = 60 - 2 * num23;
									if (num22 < 1f && num24 >= 2)
									{
										Vector2[] array2 = new Vector2[num24];
										for (int k = 0; k < num24; k++)
										{
											array2[k] = holder.ToLocal(mid + (loopShape[num23 + k] - mid) * (1f + 0.12f * num22));
										}
										loop.Set(array2, 1f - num22 * num22, 0.42f * (1f - 0.4f * num22), 1f - num22);
									}
									else
									{
										loop.Hide();
									}
									float num25 = tailTime * (1f - 0.4f * num21);
									for (int l = 0; l <= 24; l++)
									{
										float num26 = (float)l / 24f;
										float since = Mathf.Max(num19 - num25 * (1f - num26), t3 * 0.55f);
										Vector2 val5 = HeadAfter(since);
										val5 += new Vector2(0f, Mathf.Sin(num26 * 5.5f - num4 * 30f) * sway * 0.55f * (1f - 0.5f * num21) * Mathf.Sin((float)Math.PI * num26 * 0.9f));
										bodyPoints[l] = holder.ToLocal(val5);
									}
									float alpha = 1f - num21 * num21;
									body.Set(bodyPoints, alpha, Mathf.Lerp(1f, 0.6f, num21));
									Vector2 val6 = holder.ToGlobal(bodyPoints[24]);
									Vector2 going2 = val6 - holder.ToGlobal(bodyPoints[23]);
									if (going2.LengthSquared() < 0.01f)
									{
										going2 = new Vector2(way, 0f);
									}
									PlaceHead(head, eye, val6, going2, 1f, alpha);
									OneHead();
								}
							}
						}
					}
				}, (TransitionType)0, (EaseType)2).TweenCallback(Callable.From((Action)(() =>
				{
					if (FxKit.Alive((GodotObject)(object)holder))
					{
						((Node)holder).QueueFree();
					}
				})));
				float FarEnd(float p)
				{
					return HalfWidth(p) + drag * Pull(p);
				}
				float HalfWidth(float p)
				{
					return (a - hug * 0.7f * Mathf.SmoothStep(0f, 0.3f, p)) * (1f - 0.1f * Pull(p));
				}
				Vector2 HeadAfter(float since)
				{
					//IL_006f: Unknown result type (might be due to invalid IL or missing references)
					//IL_0093: Unknown result type (might be due to invalid IL or missing references)
					//IL_0098: Unknown result type (might be due to invalid IL or missing references)
					//IL_000a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0033: Unknown result type (might be due to invalid IL or missing references)
					//IL_0038: Unknown result type (might be due to invalid IL or missing references)
					if (since <= t3)
					{
						return mid + new Vector2(way * FarEnd(Mathf.Max(0f, since) / t3), 0f);
					}
					float num4 = Mathf.Min(1f, (since - t3) / t4);
					float num5 = 1f - (1f - num4) * (1f - num4);
					return mid + new Vector2(way * (farEndAtSnap + flee * num5), -80f * num4 * num4);
				}
				void OneHead()
				{
					if (FxKit.Alive((GodotObject)(object)head2))
					{
						((CanvasItem)head2).Visible = false;
					}
					if (FxKit.Alive((GodotObject)(object)eye2))
					{
						((CanvasItem)eye2).Visible = false;
					}
				}
				void PlaceHead(Sprite2D glow, Sprite2D core, Vector2 spot, Vector2 going, float size, float alpha)
				{
					//IL_0031: Unknown result type (might be due to invalid IL or missing references)
					//IL_0049: Unknown result type (might be due to invalid IL or missing references)
					//IL_005d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0091: Unknown result type (might be due to invalid IL or missing references)
					//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
					//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
					//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
					//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
					//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
					//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
					//IL_0106: Unknown result type (might be due to invalid IL or missing references)
					//IL_0128: Unknown result type (might be due to invalid IL or missing references)
					bool flag = going.LengthSquared() > 0.0001f;
					float globalRotation = (flag ? going.Angle() : 0f);
					if (FxKit.Alive((GodotObject)(object)glow))
					{
						((CanvasItem)glow).Visible = true;
						((Node2D)glow).GlobalPosition = spot;
						((Node2D)glow).GlobalRotation = globalRotation;
						((Node2D)glow).Scale = new Vector2(1.45f, 0.82f) * (size * headSize / 64f);
						((CanvasItem)glow).Modulate = new Color(venom.R, venom.G, venom.B, 0.95f * alpha);
					}
					if (FxKit.Alive((GodotObject)(object)core))
					{
						((CanvasItem)core).Visible = true;
						((Node2D)core).GlobalPosition = (flag ? (spot + going.Normalized() * (headSize * 0.1f * size)) : spot);
						((Node2D)core).GlobalRotation = globalRotation;
						((Node2D)core).Scale = new Vector2(1.5f, 0.62f) * (size * headSize * 0.4f / 64f);
						((CanvasItem)core).Modulate = new Color(0.92f, 1f, 0.84f, 0.95f * alpha);
					}
				}
				void ShapeLoop(float p, float now)
				{
					//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
					//IL_011c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0121: Unknown result type (might be due to invalid IL or missing references)
					//IL_0123: Unknown result type (might be due to invalid IL or missing references)
					//IL_0128: Unknown result type (might be due to invalid IL or missing references)
					//IL_013c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0141: Unknown result type (might be due to invalid IL or missing references)
					//IL_0143: Unknown result type (might be due to invalid IL or missing references)
					//IL_0148: Unknown result type (might be due to invalid IL or missing references)
					//IL_014d: Unknown result type (might be due to invalid IL or missing references)
					float num4 = Pull(p);
					float num5 = Mathf.SmoothStep(0.55f, 1f, p);
					float num6 = HalfWidth(p);
					float num7 = (b - hug * 0.7f * Mathf.SmoothStep(0f, 0.3f, p)) * (1f - 0.36f * num4);
					for (int i = 0; i < 60; i++)
					{
						float num8 = Mathf.Lerp(-(float)Math.PI, (float)Math.PI, (float)i / 59f);
						float num9 = (1f - Mathf.Cos(num8)) * 0.5f;
						float num10 = num9 * num9 * num9;
						Vector2 val3 = new Vector2((0f - way) * num6 * Mathf.Cos(num8) + way * drag * num4 * num10, (0f - num7) * Mathf.Sin(num8) * (1f - 0.55f * num10 * num4));
						val3.Y += Mathf.Sin(now * 95f + (float)i * 1.7f) * 3.5f * root * num5 * (1f - num10);
						loopShape[i] = mid + val3;
						loopPoints[i] = holder.ToLocal(mid + val3);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("SnakebiteFx.OnPlay: " + ex);
			}
			static float Pull(float p)
			{
				return 0.1f * Mathf.SmoothStep(0f, 0.3f, p) + 0.9f * p * p * p;
			}
		}

		private static void Bite(Node parent, Vector2 at, float way, float root)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			Sfx.Play("snake_bite");
			Color venom = PoisonFx.Venom;
			Color val = new Color(0.92f, 1f, 0.84f, 0.95f);
			FxKit.Flash(parent, at, new Color(val.R, val.G, val.B, 0.85f), 44f * root, 2f, 0.16);
			float num = 34f * root;
			Brush.Cut(parent, at + new Vector2((0f - way) * num * 0.35f, 0f - num), at + new Vector2(way * num * 0.1f, (0f - num) * 0.08f), 7f * root, val, 0f, 0.04f, 0.03f, 0.16f);
			Brush.Cut(parent, at + new Vector2((0f - way) * num * 0.35f, num), at + new Vector2(way * num * 0.1f, num * 0.08f), 7f * root, val, 0f, 0.04f, 0.03f, 0.16f);
			FxKit.Burst(parent, at, new Color(venom.R, venom.G, venom.B, 0.95f), 7, 40f, 160f, 0.45, 0.05f, 0.11f, new Vector2(0f, 560f), 40f, new Vector2(way * 0.3f, 1f), 6f);
		}

		private static void Burst(Node parent, Vector2 at, float width, float height, Vector2 along)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			Sfx.Play("snake_snap");
			Color venom = PoisonFx.Venom;
			Color acid = PoisonFx.Acid;
			float num = Mathf.Max(width, height * 0.8f);
			FxKit.Flash(parent, at, new Color(0.88f, 1f, 0.74f, 0.95f), num * 0.6f, 2.5f, 0.18);
			FxKit.Flash(parent, at, new Color(venom.R, venom.G, venom.B, 0.8f), num * 1.25f, 1.9f, 0.42);
			FxKit.RingPulse(parent, at, num * 2.1f, venom, 0f, 0.36f, 0.28f, 1.25f, 1f, 0f, 0.034f, 1f);
			FxKit.RingPulse(parent, at, num * 1.7f, acid, 0.06f, 0.48f, 0.3f, 1.4f, 1f, 18f, 0.018f);
			Brush.Cut(parent, at, at + new Vector2(along.X * num * 0.95f, (0f - num) * 0.22f), 9f, venom, 0f, 0.04f, 0.02f, 0.2f);
			Brush.Cut(parent, at, at + new Vector2(along.X * num * 0.95f, num * 0.22f), 9f, venom, 0f, 0.04f, 0.02f, 0.2f);
			FxKit.Burst(parent, at, new Color(venom.R, venom.G, venom.B, 0.95f), Tuning.I("snake.venom", 30), 240f, 680f, 0.6, 0.07f, 0.2f, new Vector2(0f, 640f), 180f, null, num * 0.2f);
			FxKit.Burst(parent, at, new Color(venom.R, venom.G, venom.B, 0.9f), 10, 80f, 260f, 0.9, 0.12f, 0.22f, new Vector2(0f, 900f), 180f, null, num * 0.25f, 0f, additive: true, 0.9f);
			FxKit.Burst(parent, at, new Color(acid.R, acid.G, acid.B, 0.95f), 12, 300f, 760f, 0.5, 0.06f, 0.15f, new Vector2(0f, 300f), 46f, along + new Vector2(0f, -0.35f), 12f);
			GameRefs.Shake(1);
		}
	}
