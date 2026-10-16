// namespace: SilentFX.Cards
// type: SkewerFx (class)
// source lines: 100287-102340
	public sealed class SkewerFx : CardFx
	{
		internal sealed class Shaft : EnergyBeads.Bead
		{
			public Sprite2D Halo;

			public Sprite2D Body;

			public Sprite2D Core;

			public Sprite2D Head;

			public Sprite2D BarbA;

			public Sprite2D BarbB;

			public Sprite2D FletchA;

			public Sprite2D FletchB;

			public Sprite2D Wound;

			public Node2D Mark;

			public ShaderMaterial MarkMat;

			public ShaderMaterial MarkInkMat;

			public Vector2 Drawn;

			public float Rot;

			public bool Placed;

			public float Morph;

			public int State;

			public float FlyAt = -1f;

			public float FlyStart;

			public float FlyTime;

			public Vector2 FlyFrom;

			public float StuckAt;

			public float StuckRot;

			public float MarkShow;

			public float MarkPop = -1f;

			public int Turn = -1;

			public bool Refill;

			public int Place = -1;
		}

		internal sealed class Volley
		{
			public Node2D Holder;

			public EnergyBeads Beads;

			public readonly List<Shaft> Shafts = new List<Shaft>();

			public readonly List<Shaft> Order = new List<Shaft>();

			public int Count;

			public bool FromCounter;

			public CardModel Card;

			public bool Remote;

			public bool Multiplayer;

			public Creature Owner;

			public Node OwnerNode;

			public Node Room;

			public object Invocation;

			public Creature Target;

			public int EnergyPart = int.MaxValue;

			public int BlackShown;

			public Vector2 Chest;

			public Vector2 Body = new Vector2(244f, 244f);

			public float Side = 1f;

			public bool HasAim;

			public bool Aimed;

			public float AimedAt;

			public Vector2 TargetChest;

			public float TargetW = 200f;

			public float TargetH = 240f;

			public float MarkSize = 50f;

			public float Len = 120f;

			public float Thick = 13f;

			public float Bead = 24f;

			public float Gap = 34f;

			public float StepY = 38f;

			public float Quiver = 0.2f;

			public float Time;

			public float OwnedAt;

			public bool Dismissed;

			public float DismissAt;

			public bool Playing;

			public bool Auto;

			public int AutoNext;

			public float AutoBeat = 0.3f;

			public float PlayAt;

			public float LastBeat;

			public float Beat = 0.3f;

			public float Flight = 0.09f;

			public float First = 0.16f;

			public float Patience = 1.6f;

			public int Expected;

			public int Hits;

			public bool Froze;

			public bool Let;

			public float DoneAt = -1f;

			public float Linger;

			public float PulseAt = -1f;

			public bool Dead;

			public Volley()
			{
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			}
		}

		private static readonly StringName PAlpha = StringName.op_Implicit("alpha");

		private static readonly StringName PSpin = StringName.op_Implicit("spin");

		public static readonly Color Energy = EnergyBeads.Green;

		private static readonly Color Pale = EnergyBeads.GreenPale;

		private const int Ready = 0;

		private const int Flying = 1;

		private const int Stuck = 2;

		private const int Gone = 3;

		private const float WindTime = 0.1f;

		private const float PullBack = 20f;

		private const float FadeTime = 0.32f;

		private const float Depth = 12f;

		private const float StuckShare = 0.74f;

		private const float BarbAngle = 0.56f;

		private const float FletchAngle = 0.62f;

		private static readonly List<Volley> Live = new List<Volley>();

		private static bool _ticking;

		private static Color A(Color c, float alpha)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			return new Color(c.R, c.G, c.B, alpha);
		}

		private static int MaxShown()
		{
			return Mathf.Clamp(Tuning.I("skewer.max", 12), 1, 16);
		}

		public override void OnPlay(CardFxContext c)
		{
			try
			{
				Volley volley = null;
				try
				{
					volley = (c.Held as SkewerHeld)?.Take(c);
					if (volley != null && (volley.Dead || !FxKit.Alive((GodotObject)(object)volley.Holder)))
					{
						volley = null;
					}
					bool flag = c.Card == null;
					for (int num = Live.Count - 1; num >= 0; num--)
					{
						if (Live[num] == volley || !Live[num].Playing || Live[num].Card != c.Card)
						{
							continue;
						}
						if (MultiplayerFx.Active)
						{
							if (MultiplayerFx.CurrentInvocation == null || Live[num].Invocation != MultiplayerFx.CurrentInvocation)
							{
								continue;
							}
							Creature owner = Live[num].Owner;
							CardModel card = c.Card;
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
							if (owner != obj || (object)Live[num].Room != GameRefs.CombatRoom)
							{
								continue;
							}
						}
						Free(Live[num]);
					}
					int num2 = (flag ? Math.Max(1, Tuning.I("skewer.preview_count", 3)) : HitsOf(c.Card, volley));
					if (!c.HasTarget || num2 <= 0)
					{
						if (volley != null)
						{
							Close(volley, 0f);
						}
						return;
					}
					int num3 = (flag ? num2 : EnergyBeads.EnergyPartOf(c.Card, num2));
					if (volley == null)
					{
						volley = Create(c, 0, 0, fromCounter: false);
						if (volley == null)
						{
							return;
						}
					}
					Aim(volley, c);
					Begin(volley, c, num2, num3, flag);
					if (PowerFxManager.LogTriggers && !flag)
					{
						DevLog.Info($"Skewer: {volley.Count} arrow(s) for X = {num2}" + ((num3 < num2) ? $" ({num2 - num3} of it black)" : ""));
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("Skewer: " + ex);
					Free(volley);
				}
			}
			catch (Exception ex2)
			{
				DevLog.Warn("SkewerFx.OnPlay: " + ex2);
			}
		}

		public override void OnHit(CardFxContext c, Vector2 at, bool killed)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Volley volley = Find(c);
				if (volley == null || !volley.Auto)
				{
					if (volley == null || volley.DoneAt >= 0f)
					{
						Stray(c, at);
					}
					else
					{
						Hit(volley, killed);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Skewer hit: " + ex);
			}
		}

		public static bool CoversStab()
		{
			if (Live.Count == 0 || Tuning.F("skewer.keep_stab", 0f) >= 0.5f)
			{
				return false;
			}
			for (int i = 0; i < Live.Count; i++)
			{
				Volley volley = Live[i];
				if (volley != null && !volley.Dead && volley.Playing && !volley.Auto && volley.Card != null && (!MultiplayerFx.Active || (MultiplayerFx.CurrentInvocation != null && volley.Card == MultiplayerFx.CurrentCard && volley.Invocation == MultiplayerFx.CurrentInvocation)) && (volley.DoneAt < 0f || volley.Time - volley.DoneAt < 0.12f))
				{
					return true;
				}
			}
			return false;
		}

		public static void Reset()
		{
			Volley[] array = Live.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				Free(array[i]);
			}
			Live.Clear();
		}

		private static int HitsOf(CardModel card, Volley held)
		{
			try
			{
				return Math.Max(0, card.ResolveEnergyXValue());
			}
			catch (Exception ex)
			{
				DevLog.Warn("Skewer: could not read X: " + ex.Message);
				return held?.Count ?? 0;
			}
		}

		private static Volley Find(CardFxContext c)
		{
			Volley volley = null;
			for (int num = Live.Count - 1; num >= 0; num--)
			{
				Volley volley2 = Live[num];
				if (volley2.Dead || !volley2.Playing || volley2.Card != c.Card)
				{
					continue;
				}
				if (MultiplayerFx.Active)
				{
					if (MultiplayerFx.CurrentInvocation == null || volley2.Invocation != MultiplayerFx.CurrentInvocation)
					{
						continue;
					}
					Creature owner = volley2.Owner;
					CardModel card = c.Card;
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
					if (owner != obj || volley2.Target != c.Target || (object)volley2.Room != GameRefs.CombatRoom)
					{
						continue;
					}
				}
				if (volley2.DoneAt < 0f)
				{
					return volley2;
				}
				if (volley == null)
				{
					volley = volley2;
				}
			}
			return volley;
		}

		private static void Stray(CardFxContext c, Vector2 at)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			Node parent = c.FrontParent;
			if (FxKit.Alive((GodotObject)(object)parent))
			{
				Vector2 val = c.Chest + new Vector2((0f - c.TargetSide) * (c.Body.X * 0.5f + 90f), (0f - c.Body.Y) * 0.06f);
				Brush.Dart(parent, val, at, 150f, 13f, A(Energy, 0.92f), 0.06f, 0f, () =>
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0016: Unknown result type (might be due to invalid IL or missing references)
					FxKit.Flash(parent, at, A(Pale, 0.8f), 48f, 1.7f, 0.13);
				});
			}
		}

		internal static Volley Create(CardFxContext c, int count, int chem, bool fromCounter)
		{
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Expected Obj, but got Unknown
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			bool flag = MultiplayerFx.Active && c.Remote;
			if (flag)
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
				if (obj == null)
				{
					return null;
				}
			}
			Node val = (((!flag & fromCounter) && FxKit.Alive((GodotObject)(object)c.UiParent) && c.UiParent.IsInsideTree()) ? c.UiParent : c.FrontParent);
			if (!FxKit.Alive((GodotObject)(object)val) || !val.IsInsideTree())
			{
				return null;
			}
			for (int num = Live.Count - 1; num >= 0; num--)
			{
				if (!Live[num].Dismissed)
				{
					continue;
				}
				if (MultiplayerFx.Active)
				{
					CardModel card2 = c.Card;
					object obj2;
					if (card2 == null)
					{
						obj2 = null;
					}
					else
					{
						Player owner2 = card2.Owner;
						obj2 = ((owner2 != null) ? owner2.Creature : null);
					}
					if (obj2 == null || Live[num].Owner != c.Card.Owner.Creature)
					{
						continue;
					}
				}
				Free(Live[num]);
			}
			Volley volley = new Volley
			{
				FromCounter = (fromCounter && !flag),
				Card = c.Card,
				Remote = flag,
				Multiplayer = MultiplayerFx.Active
			};
			CardModel card3 = c.Card;
			object owner3;
			if (card3 == null)
			{
				owner3 = null;
			}
			else
			{
				Player owner4 = card3.Owner;
				owner3 = ((owner4 != null) ? owner4.Creature : null);
			}
			volley.Owner = (Creature)owner3;
			object ownerNode;
			if (!flag)
			{
				ownerNode = null;
			}
			else
			{
				NCombatRoom combatRoom = GameRefs.CombatRoom;
				if (combatRoom == null)
				{
					ownerNode = null;
				}
				else
				{
					CardModel card4 = c.Card;
					object obj3;
					if (card4 == null)
					{
						obj3 = null;
					}
					else
					{
						Player owner5 = card4.Owner;
						obj3 = ((owner5 != null) ? owner5.Creature : null);
					}
					ownerNode = combatRoom.GetCreatureNode((Creature)obj3);
				}
			}
			volley.OwnerNode = (Node)ownerNode;
			volley.Room = (Node)(object)GameRefs.CombatRoom;
			volley.Chest = c.Chest;
			volley.Body = c.Body;
			volley.Side = ((c.Facing < 0f) ? (-1f) : 1f);
			volley.Len = Mathf.Clamp(Tuning.F("skewer.arrow_length", 120f), 40f, 320f);
			volley.Thick = Mathf.Clamp(Tuning.F("skewer.arrow_thick", 13f), 4f, 40f);
			volley.Bead = Mathf.Clamp(Tuning.F("skewer.bead", 24f), 8f, 60f);
			volley.Gap = Tuning.F("skewer.column_gap", 34f);
			volley.StepY = Mathf.Max(8f, Tuning.F("skewer.column_step", 38f));
			volley.Quiver = Tuning.F("skewer.quiver", 0.2f);
			Volley volley2 = volley;
			volley2.Holder = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_Skewer")
			};
			TimeFreeze.KeepLive((Node)(object)volley2.Holder);
			val.AddChild((Node)(object)volley2.Holder, false, (InternalMode)0);
			Live.Add(volley2);
			if (!_ticking)
			{
				_ticking = true;
				Ticker.Add(Update);
			}
			if (flag & fromCounter)
			{
				int num2 = Mathf.Clamp(chem, 0, MaxShown());
				int num3 = Mathf.Clamp(count, 0, MaxShown() - num2);
				int num4 = (volley2.Count = num3 + num2);
				for (int i = 0; i < num4; i++)
				{
					Shaft shaft = AddShaft(volley2, (i >= num3) ? BeadKind.Chem : BeadKind.Energy);
					shaft.Pos = Slot(volley2, i, num4, 0f);
					shaft.Out = 1f;
					shaft.Left = true;
					shaft.Placed = true;
				}
			}
			else if (fromCounter)
			{
				volley2.Beads = EnergyBeads.Open<Shaft>(volley2.Holder, count, chem, MaxShown(), Tuning.F("skewer.bead_gap", 0.07f), Mathf.Clamp(Tuning.F("skewer.cap_size", 1f), 0.3f, 3f), Mathf.Clamp(Tuning.F("skewer.cap_dark", 0.86f), 0f, 1f));
				if (volley2.Beads != null)
				{
					for (int j = 0; j < volley2.Beads.Count; j++)
					{
						Build(volley2, (Shaft)volley2.Beads.All[j]);
					}
				}
			}
			volley2.Count = volley2.Shafts.Count;
			if (fromCounter)
			{
				Step(volley2, 0f);
			}
			return volley2;
		}

		private static Sprite2D Dot(Node parent)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			Sprite2D val = FxKit.Glow(new Color(1f, 1f, 1f, 0f), new Vector2(64f, 64f));
			parent.AddChild((Node)(object)val, false, (InternalMode)0);
			return val;
		}

		private static Shaft AddShaft(Volley v, BeadKind kind)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			Vector2 pos = ((v.Beads != null) ? v.Beads.Counter : Vector2.Zero);
			Shaft shaft = new Shaft
			{
				Kind = kind,
				Tint = ((kind == BeadKind.Chem) ? EnergyBeads.Rim : Energy),
				Pos = pos
			};
			Build(v, shaft);
			return shaft;
		}

		private static void Build(Volley v, Shaft s)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Expected Obj, but got Unknown
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			bool flag = s.Kind == BeadKind.Chem;
			s.Drawn = s.Pos;
			s.Rot = ((v.Side >= 0f) ? 0f : ((float)Math.PI));
			if (!v.Remote)
			{
				s.Mark = FxKit.Sigil(100f, flag ? EnergyBeads.Rim : Energy, out s.MarkMat);
				((Node)v.Holder).AddChild((Node)(object)s.Mark, false, (InternalMode)0);
				if (s.MarkMat != null)
				{
					s.MarkMat.SetShaderParameter(StringName.op_Implicit("radius"), Variant.op_Implicit(0.58f));
					s.MarkMat.SetShaderParameter(StringName.op_Implicit("width"), Variant.op_Implicit(0.085f));
					s.MarkMat.SetShaderParameter(StringName.op_Implicit("ticks"), Variant.op_Implicit(4f));
					s.MarkMat.SetShaderParameter(StringName.op_Implicit("tick_len"), Variant.op_Implicit(0.44f));
					s.MarkMat.SetShaderParameter(StringName.op_Implicit("tick_width"), Variant.op_Implicit(0.09f));
					s.MarkMat.SetShaderParameter(StringName.op_Implicit("tick_out"), Variant.op_Implicit(0.5f));
					s.MarkMat.SetShaderParameter(StringName.op_Implicit("glow"), Variant.op_Implicit(0.35f));
					s.MarkMat.SetShaderParameter(StringName.op_Implicit("fill"), Variant.op_Implicit(0.06f));
					s.MarkMat.SetShaderParameter(PAlpha, Variant.op_Implicit(0f));
				}
				if (flag)
				{
					BlackSight(s);
				}
			}
			s.Wound = Dot((Node)(object)v.Holder);
			if (flag && FxKit.Alive((GodotObject)(object)s.Wound))
			{
				((CanvasItem)s.Wound).Material = null;
			}
			s.Root = new Node2D();
			((Node)v.Holder).AddChild((Node)(object)s.Root, false, (InternalMode)0);
			s.Halo = Dot((Node)(object)s.Root);
			s.Body = Dot((Node)(object)s.Root);
			s.Head = Dot((Node)(object)s.Root);
			s.FletchA = Dot((Node)(object)s.Root);
			s.FletchB = Dot((Node)(object)s.Root);
			s.BarbA = Dot((Node)(object)s.Root);
			s.BarbB = Dot((Node)(object)s.Root);
			s.Core = Dot((Node)(object)s.Root);
			if (flag)
			{
				EnergyBeads.MakeInk(s.Head);
				EnergyBeads.MakeInk(s.Core);
			}
			v.Shafts.Add(s);
		}

		private static void BlackSight(Shaft s)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			if (s.MarkMat != null)
			{
				s.MarkMat.SetShaderParameter(StringName.op_Implicit("width"), Variant.op_Implicit(0.16f));
				s.MarkMat.SetShaderParameter(StringName.op_Implicit("tick_len"), Variant.op_Implicit(0.5f));
				s.MarkMat.SetShaderParameter(StringName.op_Implicit("tick_width"), Variant.op_Implicit(0.18f));
				s.MarkMat.SetShaderParameter(StringName.op_Implicit("glow"), Variant.op_Implicit(0.4f));
				s.MarkMat.SetShaderParameter(StringName.op_Implicit("fill"), Variant.op_Implicit(0.03f));
			}
			if (!FxKit.Alive((GodotObject)(object)s.Mark))
			{
				return;
			}
			Node2D val = EnergyBeads.InkSigil(100f, out s.MarkInkMat);
			if (val != null)
			{
				((Node)s.Mark).AddChild((Node)(object)val, false, (InternalMode)0);
				if (s.MarkInkMat != null)
				{
					s.MarkInkMat.SetShaderParameter(StringName.op_Implicit("radius"), Variant.op_Implicit(0.58f));
					s.MarkInkMat.SetShaderParameter(StringName.op_Implicit("width"), Variant.op_Implicit(0.085f));
					s.MarkInkMat.SetShaderParameter(StringName.op_Implicit("ticks"), Variant.op_Implicit(4f));
					s.MarkInkMat.SetShaderParameter(StringName.op_Implicit("tick_len"), Variant.op_Implicit(0.44f));
					s.MarkInkMat.SetShaderParameter(StringName.op_Implicit("tick_width"), Variant.op_Implicit(0.09f));
					s.MarkInkMat.SetShaderParameter(StringName.op_Implicit("tick_out"), Variant.op_Implicit(0.5f));
					s.MarkInkMat.SetShaderParameter(StringName.op_Implicit("glow"), Variant.op_Implicit(0.1f));
					s.MarkInkMat.SetShaderParameter(StringName.op_Implicit("fill"), Variant.op_Implicit(0.1f));
					s.MarkInkMat.SetShaderParameter(PAlpha, Variant.op_Implicit(0f));
				}
			}
		}

		internal static void Aim(Volley v, CardFxContext c)
		{
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			if (v == null || v.Dead || !c.HasTarget)
			{
				return;
			}
			bool flag = !v.HasAim || v.Target != c.Target || v.TargetChest.DistanceTo(c.TargetChest) > 40f;
			v.Target = c.Target;
			v.TargetChest = c.TargetChest;
			v.TargetW = c.TargetWidth;
			v.TargetH = c.TargetHeight;
			v.Side = ((c.TargetSide < 0f) ? (-1f) : 1f);
			v.Chest = c.Chest;
			v.Body = c.Body;
			v.HasAim = true;
			if (!v.Aimed | flag)
			{
				v.Aimed = true;
				v.AimedAt = v.Time;
				if (flag && !v.Playing)
				{
					foreach (Shaft shaft in v.Shafts)
					{
						shaft.MarkShow = 0f;
					}
				}
			}
			Resize(v);
		}

		internal static void Unaim(Volley v)
		{
			if (v != null && !v.Dead && !v.Playing && v.Aimed)
			{
				v.Aimed = false;
				v.AimedAt = v.Time;
			}
		}

		internal static void Dismiss(Volley v)
		{
			if (v != null && !v.Dead && !v.Playing && !v.Dismissed)
			{
				if (v.Remote)
				{
					v.Dismissed = true;
					v.DismissAt = v.Time;
					Close(v, 0f);
				}
				else if (!v.FromCounter)
				{
					Close(v, 0f);
				}
				else
				{
					v.Dismissed = true;
					v.DismissAt = v.Time;
					v.Aimed = false;
					v.Beads?.SendHome(v.Time);
				}
			}
		}

		private static void Resize(Volley v)
		{
			float num = Mathf.Max(Mathf.Max(v.TargetW, v.TargetH) / 260f, 1f);
			v.MarkSize = Mathf.Clamp(66f - 3.5f * (float)v.Count, 34f, 60f) * num;
		}

		private static void Begin(Volley v, CardFxContext c, int x, int energyPart, bool auto)
		{
			v.Card = c.Card;
			v.Multiplayer = MultiplayerFx.Active;
			if (MultiplayerFx.Active)
			{
				v.Invocation = MultiplayerFx.CurrentInvocation ?? new object();
			}
			v.EnergyPart = energyPart;
			v.Beads?.Release();
			v.Playing = true;
			v.Auto = auto;
			v.Aimed = true;
			v.PlayAt = v.Time;
			v.LastBeat = v.Time;
			v.Expected = x;
			v.Hits = 0;
			v.AutoNext = 0;
			v.Beat = Mathf.Clamp(Tuning.F("skewer.beat", 0.3f), 0.12f, 0.8f);
			v.AutoBeat = v.Beat;
			v.Flight = Mathf.Clamp(Tuning.F("skewer.flight", 0.09f), 0.03f, 0.3f);
			v.First = Mathf.Clamp(Tuning.F("skewer.first_hit", 0.16f), 0f, 1f);
			v.Patience = Mathf.Max(0.6f, Tuning.F("skewer.timeout", 1.6f));
			int num = Math.Min(x, MaxShown());
			int num2 = num - Math.Min(Math.Max(0, x - energyPart), num);
			while (v.Shafts.Count < num)
			{
				Shaft shaft = AddShaft(v, (v.Shafts.Count >= num2) ? BeadKind.Chem : BeadKind.Energy);
				shaft.Out = 1f;
				shaft.Left = true;
				shaft.Morph = 1f;
			}
			for (int i = 0; i < v.Shafts.Count; i++)
			{
				Shaft shaft2 = v.Shafts[i];
				if (i >= num)
				{
					shaft2.State = 3;
					continue;
				}
				shaft2.Out = 1f;
				shaft2.Left = true;
			}
			v.Count = num;
			v.BlackShown = 0;
			v.Order.Clear();
			for (int j = 0; j < num; j++)
			{
				Shaft shaft3 = v.Shafts[j];
				if (shaft3.Kind == BeadKind.Chem)
				{
					v.BlackShown++;
				}
				shaft3.Turn = j;
				shaft3.Place = j;
				v.Order.Add(shaft3);
			}
			Resize(v);
			if (num > 0)
			{
				v.Shafts[0].FlyAt = v.Time + Mathf.Max(0f, v.First - v.Flight);
			}
			Step(v, 0f);
		}

		private static void Hit(Volley v, bool killed)
		{
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			int num = v.Hits++;
			float time = v.Time;
			if (num >= 1)
			{
				v.Beat = Mathf.Clamp(time - v.LastBeat, 0.12f, 0.7f);
			}
			v.LastBeat = time;
			if (num < v.Order.Count)
			{
				Shaft shaft = v.Order[num];
				if (shaft.State == 0)
				{
					Launch(v, shaft, shaft.Drawn, Mathf.Min(v.Flight, 0.045f));
				}
				else if (shaft.State == 1)
				{
					Land(v, shaft, num);
				}
				else if (shaft.State == 2)
				{
					shaft.StuckAt = time;
				}
			}
			else
			{
				Extra(v, num - v.BlackShown >= v.EnergyPart);
			}
			Freeze(v);
			if (killed)
			{
				Thaw(v);
				Close(v, 0.1f);
			}
			else if (num + 1 >= v.Expected)
			{
				v.PulseAt = time;
				Close(v, Mathf.Clamp(Tuning.F("skewer.linger", 0.3f), 0f, 2f));
			}
			else if (num + 1 < v.Order.Count)
			{
				Shaft shaft2 = v.Order[num + 1];
				if (shaft2.State == 0)
				{
					shaft2.FlyAt = time + Mathf.Max(0f, v.Beat - v.Flight);
				}
			}
		}

		private static void Refill(Volley v, Shaft gone)
		{
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			if (v.Playing && !(v.DoneAt >= 0f) && v.Order.Count < v.Expected && v.Count > 0 && FxKit.Alive((GodotObject)(object)v.Holder))
			{
				int count = v.Order.Count;
				Shaft shaft = AddShaft(v, (count - v.BlackShown >= v.EnergyPart) ? BeadKind.Chem : BeadKind.Energy);
				Vector2 val;
				if (v.Remote)
				{
					val = v.Chest;
				}
				else
				{
					val = ((v.Beads != null) ? v.Beads.Counter : GameRefs.EnergyPoint());
				}
				shaft.Refill = true;
				shaft.Turn = count;
				shaft.Place = ((gone != null && gone.Place >= 0) ? gone.Place : (count % v.Count));
				shaft.Start = val;
				shaft.Pos = val;
				shaft.Drawn = val;
				shaft.Out = 0f;
				shaft.Left = false;
				shaft.LeaveAt = v.Time;
				shaft.Show = 0f;
				shaft.Morph = 0f;
				v.Order.Add(shaft);
			}
		}

		private static Vector2 RefillOut(Volley v, Shaft s, Vector2 slot, float time, float dt)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			if (time < s.LeaveAt)
			{
				s.Show = 0f;
				return s.Start;
			}
			if (!s.Left)
			{
				s.Left = true;
				bool flag = s.Kind == BeadKind.Chem;
				FxKit.Flash((Node)(object)v.Holder, s.Start, flag ? A(EnergyBeads.Rim, 0.33f) : A(Energy, 0.6f), 70f, 1.5f, 0.16);
			}
			s.Out = Mathf.Min(1f, s.Out + dt / 0.34f);
			float t = 1f - Mathf.Pow(1f - s.Out, 3f);
			Vector2 b = s.Start.Lerp(slot, 0.45f) + new Vector2((FxKit.Hash(s.Turn * 3 + 1) - 0.5f) * 120f, -150f - 22f * (float)(s.Turn % 3));
			s.Show = Mathf.Min(1f, s.Out * 8f);
			return FxKit.Bezier(s.Start, b, slot, t);
		}

		private static void Extra(Volley v, bool black)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)v.Holder))
			{
				return;
			}
			Vector2 val = Slot(v, 0, 1, 1f);
			Vector2 at = v.TargetChest;
			Node2D parent = v.Holder;
			if (black)
			{
				Brush.Dart((Node)(object)parent, val, at, v.Len * 1.2f, v.Thick, A(EnergyBeads.Rim, 0.55f), 0.06f, 0f, () =>
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0016: Unknown result type (might be due to invalid IL or missing references)
					FxKit.Flash((Node)(object)parent, at, A(EnergyBeads.Rim, 0.5f), 48f, 1.7f, 0.13);
				});
			}
			else
			{
				Brush.Dart((Node)(object)parent, val, at, v.Len * 1.2f, v.Thick, A(Energy, 0.92f), 0.06f, 0f, () =>
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0016: Unknown result type (might be due to invalid IL or missing references)
					FxKit.Flash((Node)(object)parent, at, A(Pale, 0.8f), 48f, 1.7f, 0.13);
				});
			}
		}

		private static void Launch(Volley v, Shaft s, Vector2 from, float flight)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			s.State = 1;
			s.FlyStart = v.Time;
			s.FlyTime = flight;
			s.FlyFrom = from;
			s.Morph = 1f;
			if (s.Turn >= 0)
			{
				Refill(v, s);
			}
		}

		private static void Land(Volley v, Shaft s, int index)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			s.State = 2;
			s.StuckAt = v.Time;
			s.MarkPop = v.Time;
			Vector2 val = SpotOf(v, index);
			Vector2 val2 = val - s.FlyFrom;
			Vector2 val3 = (Vector2)((val2.LengthSquared() > 1f) ? val2.Normalized() : new Vector2(v.Side, 0f));
			s.StuckRot = val3.Angle() + (FxKit.Hash(index * 5 + 2) - 0.5f) * 0.14f;
			if (FxKit.Alive((GodotObject)(object)v.Holder))
			{
				if (s.Kind == BeadKind.Chem)
				{
					Brush.Cut((Node)(object)v.Holder, s.FlyFrom, val, v.Thick * 0.9f, A(EnergyBeads.Rim, 0.3f), 0f, 0.02f, 0.01f, 0.16f);
					FxKit.Flash((Node)(object)v.Holder, val, A(EnergyBeads.Rim, 0.45f), 44f, 1.7f, 0.14);
					FxKit.Burst((Node)(object)v.Holder, val, A(EnergyBeads.Ink, 0.9f), 6, 105f, 300f, 0.34, 0.06f, 0.16f, new Vector2(0f, 320f), 36f, -val3, 6f, 0f, additive: false);
					Brush.Sparks((Node)(object)v.Holder, val, A(EnergyBeads.Rim, 0.7f), 3, 300f, -val3, 36f);
				}
				else
				{
					Brush.Cut((Node)(object)v.Holder, s.FlyFrom, val, v.Thick * 0.9f, A(Energy, 0.5f), 0f, 0.02f, 0.01f, 0.16f);
					FxKit.Flash((Node)(object)v.Holder, val, A(Pale, 0.85f), 52f, 1.7f, 0.14);
					Brush.Sparks((Node)(object)v.Holder, val, Energy, 5, 300f, -val3, 36f);
				}
			}
			Freeze(v);
		}

		private static void Freeze(Volley v)
		{
			if (v.Target == null || v.Let || (v.DoneAt >= 0f && v.Time - v.DoneAt >= v.Linger))
			{
				return;
			}
			try
			{
				float maxSeconds = v.Beat + Mathf.Clamp(Tuning.F("skewer.hold_pad", 0.9f), 0.2f, 3f);
				v.Multiplayer = MultiplayerFx.Active;
				if (v.Multiplayer)
				{
					CreatureHold.Hold(v.Target, maxSeconds, v);
				}
				else
				{
					CreatureHold.Hold(v.Target, maxSeconds);
				}
				v.Froze = true;
			}
			catch (Exception ex)
			{
				DevLog.Warn("Skewer hold: " + ex.Message);
			}
		}

		private static void Thaw(Volley v)
		{
			v.Let = true;
			if (!v.Froze || v.Target == null)
			{
				return;
			}
			if (v.Multiplayer)
			{
				try
				{
					CreatureHold.Release(v.Target, v);
					return;
				}
				catch (Exception ex)
				{
					DevLog.Warn("Skewer release: " + ex.Message);
					return;
				}
			}
			if (MultiplayerFx.Active)
			{
				return;
			}
			for (int i = 0; i < Live.Count; i++)
			{
				Volley volley = Live[i];
				if (volley != v && !volley.Dead && !volley.Let && volley.Froze && volley.Target == v.Target)
				{
					return;
				}
			}
			try
			{
				CreatureHold.Release(v.Target);
			}
			catch (Exception ex2)
			{
				DevLog.Warn("Skewer release: " + ex2.Message);
			}
		}

		private static void Close(Volley v, float linger)
		{
			if (v != null && !v.Dead && !(v.DoneAt >= 0f))
			{
				v.DoneAt = v.Time;
				v.Linger = linger;
			}
		}

		private static void Free(Volley v)
		{
			if (v == null || v.Dead)
			{
				return;
			}
			v.Dead = true;
			try
			{
				if (!v.Let)
				{
					Thaw(v);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Skewer cleanup hold: " + ex);
			}
			try
			{
				if (FxKit.Alive((GodotObject)(object)v.Holder))
				{
					((Node)v.Holder).QueueFree();
				}
			}
			catch (Exception ex2)
			{
				DevLog.Warn("Skewer cleanup nodes: " + ex2);
			}
			try
			{
				v.Beads?.Free();
			}
			catch (Exception ex3)
			{
				DevLog.Warn("Skewer cleanup beads: " + ex3);
			}
		}

		internal static void Touch(Volley v)
		{
			if (v != null && !v.Dead)
			{
				v.OwnedAt = v.Time;
			}
		}

		private static Vector2 Slot(Volley v, int i, int n, float m)
		{
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			float num = Mathf.Max(v.Body.Y * 1.05f, 180f);
			float num2 = ((n > 1) ? Mathf.Min(v.StepY, num / (float)(n - 1)) : 0f);
			float num3 = v.Body.X * 0.5f + v.Gap + v.Len * 0.5f * m;
			Vector2 val = v.Chest + new Vector2((0f - v.Side) * num3, (0f - v.Body.Y) * 0.06f + ((float)i - (float)(n - 1) * 0.5f) * num2);
			float num4 = 1f - m;
			return val + new Vector2(Mathf.Sin(v.Time * 1.7f + (float)i * 2.4f) * 14f, Mathf.Cos(v.Time * 2.3f + (float)i * 1.3f) * 6f) * num4;
		}

		private static Vector2 Spot(Volley v, int i, int n)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			if (n <= 1 || i >= n)
			{
				return v.TargetChest;
			}
			float num = Mathf.Max(v.TargetW, 120f);
			float num2 = Mathf.Max(v.TargetH, 1f);
			float num3 = ((float)i + 0.5f) / (float)n - 0.5f;
			float num4 = FxKit.Hash(i * 7 + 3) - 0.5f;
			return v.TargetChest + new Vector2(num4 * num * 0.46f, num3 * num2 * 0.62f);
		}

		private static Vector2 SpotOf(Volley v, int turn)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			int count = v.Count;
			if (turn < count || count <= 1)
			{
				return Spot(v, turn, count);
			}
			int num = turn % count;
			int num2 = turn / count;
			float num3 = Mathf.Max(v.TargetW, 120f);
			float num4 = Mathf.Max(v.TargetH, 1f);
			float num5 = Mathf.Clamp(((float)num + 0.5f + 0.5f * (float)(num2 % 2)) / (float)count - 0.5f, -0.5f, 0.5f);
			float num6 = FxKit.Hash(turn * 7 + 3) - 0.5f;
			return v.TargetChest + new Vector2(num6 * num3 * 0.46f, num5 * num4 * 0.62f);
		}

		private static void Track(Volley v)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (v.Remote && !v.Dismissed)
				{
					NCombatRoom combatRoom = GameRefs.CombatRoom;
					NCreature val = ((combatRoom != null) ? combatRoom.GetCreatureNode(v.Owner) : null);
					if (FxKit.Alive((GodotObject)(object)val))
					{
						v.Chest = val.VfxSpawnPosition;
					}
				}
				if (v.FromCounter && !v.Playing)
				{
					v.Beads?.Track();
				}
				if (v.Target != null && v.HasAim)
				{
					NCombatRoom combatRoom2 = GameRefs.CombatRoom;
					NCreature val2 = ((combatRoom2 != null) ? combatRoom2.GetCreatureNode(v.Target) : null);
					if (FxKit.Alive((GodotObject)(object)val2) && ((Node)val2).IsInsideTree())
					{
						v.TargetChest = val2.VfxSpawnPosition;
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Skewer track: " + ex.Message);
			}
		}

		private static void Update(double realDt)
		{
			if (Live.Count == 0)
			{
				return;
			}
			float dt = (float)(realDt * (double)Mathf.Clamp((float)Engine.TimeScale, 0f, 4f));
			for (int num = Live.Count - 1; num >= 0; num--)
			{
				Volley volley = Live[num];
				try
				{
					if (!volley.Dead)
					{
						Step(volley, dt);
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("Skewer failed and was removed: " + ex);
					Free(volley);
				}
				if (volley.Dead)
				{
					Live.RemoveAt(num);
				}
			}
		}

		private static void Step(Volley v, float dt)
		{
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0829: Unknown result type (might be due to invalid IL or missing references)
			//IL_082b: Unknown result type (might be due to invalid IL or missing references)
			//IL_083b: Unknown result type (might be due to invalid IL or missing references)
			//IL_083d: Unknown result type (might be due to invalid IL or missing references)
			//IL_083f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0844: Unknown result type (might be due to invalid IL or missing references)
			//IL_0856: Unknown result type (might be due to invalid IL or missing references)
			//IL_0788: Unknown result type (might be due to invalid IL or missing references)
			//IL_0794: Unknown result type (might be due to invalid IL or missing references)
			//IL_0796: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0660: Unknown result type (might be due to invalid IL or missing references)
			//IL_0664: Unknown result type (might be due to invalid IL or missing references)
			//IL_0669: Unknown result type (might be due to invalid IL or missing references)
			//IL_066e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0961: Unknown result type (might be due to invalid IL or missing references)
			//IL_096b: Unknown result type (might be due to invalid IL or missing references)
			//IL_08cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_08df: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0694: Unknown result type (might be due to invalid IL or missing references)
			//IL_0698: Unknown result type (might be due to invalid IL or missing references)
			//IL_068d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0941: Unknown result type (might be due to invalid IL or missing references)
			//IL_094e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0933: Unknown result type (might be due to invalid IL or missing references)
			//IL_093a: Unknown result type (might be due to invalid IL or missing references)
			//IL_069d: Unknown result type (might be due to invalid IL or missing references)
			//IL_069f: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0462: Unknown result type (might be due to invalid IL or missing references)
			//IL_046d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0447: Unknown result type (might be due to invalid IL or missing references)
			//IL_0451: Unknown result type (might be due to invalid IL or missing references)
			//IL_0430: Unknown result type (might be due to invalid IL or missing references)
			//IL_0434: Unknown result type (might be due to invalid IL or missing references)
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			//IL_0472: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_04af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0511: Unknown result type (might be due to invalid IL or missing references)
			//IL_0513: Unknown result type (might be due to invalid IL or missing references)
			//IL_0515: Unknown result type (might be due to invalid IL or missing references)
			//IL_051a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ada: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0afd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b2b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b54: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0603: Unknown result type (might be due to invalid IL or missing references)
			//IL_0605: Unknown result type (might be due to invalid IL or missing references)
			//IL_0607: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bfe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb5: Unknown result type (might be due to invalid IL or missing references)
			if (v.Remote)
			{
				if (MultiplayerFx.Active && (object)v.Room == GameRefs.CombatRoom && RemoteOwnerPresent(v))
				{
					CardModel card = v.Card;
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
					if (obj == v.Owner && FxKit.Alive((GodotObject)(object)v.OwnerNode) && v.OwnerNode.IsInsideTree())
					{
						NCombatRoom combatRoom = GameRefs.CombatRoom;
						if ((object)((combatRoom != null) ? combatRoom.GetCreatureNode(v.Owner) : null) == v.OwnerNode)
						{
							goto IL_008d;
						}
					}
				}
				Free(v);
				return;
			}
			goto IL_008d;
			IL_008d:
			if (!FxKit.Alive((GodotObject)(object)v.Holder) || !((Node)v.Holder).IsInsideTree() || (v.Card != null && GameRefs.CombatRoom == null))
			{
				Free(v);
				return;
			}
			v.Time += dt;
			float time = v.Time;
			Track(v);
			if (!v.Playing && !v.Dismissed && time - v.OwnedAt > 1f)
			{
				Close(v, 0f);
			}
			if (v.Playing && v.Target != null)
			{
				NCombatRoom combatRoom2 = GameRefs.CombatRoom;
				NCreature val = ((combatRoom2 != null) ? combatRoom2.GetCreatureNode(v.Target) : null);
				if (!v.Target.IsAlive || !FxKit.Alive((GodotObject)(object)val) || !((Node)val).IsInsideTree())
				{
					Thaw(v);
					Close(v, 0f);
				}
			}
			if (v.Playing && v.DoneAt < 0f)
			{
				while (v.Auto && v.DoneAt < 0f && v.AutoNext < v.Expected && time - v.PlayAt >= v.First + (float)v.AutoNext * v.AutoBeat)
				{
					v.AutoNext++;
					Hit(v, killed: false);
				}
				if (v.DoneAt < 0f && time - v.LastBeat > v.Patience)
				{
					Close(v, 0f);
				}
			}
			float num = 0f;
			if (v.DoneAt >= 0f)
			{
				float num2 = time - v.DoneAt - v.Linger;
				if (num2 >= 0f && !v.Let)
				{
					Thaw(v);
				}
				num = Mathf.Clamp(num2 / 0.32f, 0f, 1f);
			}
			float num3 = 1f - num;
			float num4 = ((v.PulseAt < 0f) ? 0f : Mathf.Exp(-9f * (time - v.PulseAt)));
			int count = v.Count;
			bool flag = (v.Aimed || v.Playing) && !v.Dismissed;
			for (int i = 0; i < v.Shafts.Count; i++)
			{
				Shaft shaft = v.Shafts[i];
				if (!FxKit.Alive((GodotObject)(object)shaft.Root))
				{
					continue;
				}
				int num5 = ((shaft.Turn >= 0) ? shaft.Turn : i);
				int num6 = ((shaft.Place >= 0) ? shaft.Place : i);
				bool flag2 = i < count || shaft.Refill;
				Vector2 val2 = ((shaft.Turn >= 0) ? SpotOf(v, num5) : Spot(v, i, count));
				Vector2 val3 = shaft.Pos;
				Vector2 val4 = Vector2.Zero;
				float num7 = shaft.Rot;
				float num8 = 0f;
				float sink = 0f;
				float bright = 0f;
				float num9 = 1f;
				if (v.Remote && v.Dismissed)
				{
					val3 = shaft.Drawn;
				}
				else if (shaft.State == 0)
				{
					Vector2 val5 = ((flag2 && num6 < count) ? Slot(v, num6, count, shaft.Morph) : shaft.Pos);
					bool flag3 = flag && shaft.Out >= 1f;
					if (!flag3 || v.Playing || time - v.AimedAt >= (float)i * 0.03f)
					{
						shaft.Morph = FxKit.Damp(shaft.Morph, flag3 ? 1f : 0f, flag3 ? 11f : 14f, dt);
					}
					if (!shaft.Refill || !(shaft.Out < 1f))
					{
						val3 = ((v.Beads != null) ? v.Beads.Move(shaft, i, val5, shaft.Placed, time, dt) : EnergyBeads.Settle(shaft, val5, shaft.Placed, dt));
					}
					else
					{
						val3 = RefillOut(v, shaft, val5, time, dt);
					}
					if (v.Remote && !v.Playing)
					{
						shaft.Show = Mathf.Clamp(time / 0.32f, 0f, 1f);
					}
					Vector2 val6 = val3 - shaft.Pos;
					num8 = Mathf.Clamp(((dt > 0.0001f) ? (val6.Length() / dt) : 0f) * 0.035f, 0f, 64f) * (1f - shaft.Morph);
					float num10;
					if (!v.HasAim)
					{
						num10 = ((v.Side >= 0f) ? 0f : ((float)Math.PI));
					}
					else
					{
						Vector2 val7 = val2 - val3;
						num10 = val7.Angle();
					}
					float num11 = num10;
					if (shaft.Placed)
					{
						num7 = ((!(num8 > 6f) || !(val6.LengthSquared() > 0.01f)) ? Mathf.LerpAngle(num7, num11, 1f - Mathf.Exp(-14f * dt)) : val6.Angle());
					}
					else
					{
						num7 = num11;
					}
					if (v.Playing && shaft.FlyAt >= 0f && v.DoneAt < 0f)
					{
						float num12 = Mathf.Clamp((time - (shaft.FlyAt - 0.1f)) / 0.1f, 0f, 1f);
						bright = num12;
						val4 = -new Vector2(Mathf.Cos(num7), Mathf.Sin(num7)) * (20f * num12 * num12);
						if (time >= shaft.FlyAt)
						{
							Launch(v, shaft, val3 + val4, v.Flight);
						}
					}
				}
				else if (shaft.State == 1)
				{
					float num13 = ((shaft.FlyTime > 0.001f) ? Mathf.Clamp((time - shaft.FlyStart) / shaft.FlyTime, 0f, 1f) : 1f);
					Vector2 val8 = val2 - shaft.FlyFrom;
					float num14 = val8.Length();
					Vector2 val9 = (Vector2)((num14 > 1f) ? (val8 / num14) : new Vector2(v.Side, 0f));
					Vector2 val10 = val2 - val9 * (v.Len * 0.5f - 12f);
					val3 = shaft.FlyFrom.Lerp(val10, num13 * num13);
					num7 = val9.Angle();
					num9 = 1f + 0.45f * Mathf.Sin((float)Math.PI * num13);
					bright = 1f;
					shaft.Morph = 1f;
					shaft.Show = 1f;
					if (num13 >= 1f)
					{
						Land(v, shaft, num5);
					}
				}
				else if (shaft.State == 2)
				{
					float num15 = time - shaft.StuckAt;
					num7 = shaft.StuckRot + v.Quiver * Mathf.Exp(-7f * num15) * Mathf.Sin(44f * num15);
					Vector2 val11 = new Vector2(Mathf.Cos(num7), Mathf.Sin(num7));
					num9 = 0.74f;
					val3 = val2 - val11 * (v.Len * num9 * 0.5f - 12f) - val11 * (16f * num);
					sink = 1f;
					bright = Mathf.Min(1f, Mathf.Exp(-10f * num15) + num4);
					shaft.Morph = 1f;
					shaft.Show = 1f;
				}
				else
				{
					shaft.Show = FxKit.Damp(shaft.Show, 0f, 14f, dt);
				}
				shaft.Pos = val3;
				shaft.Rot = num7;
				shaft.Drawn = val3 + val4;
				shaft.Placed = true;
				Pose(v, shaft, shaft.Drawn, num7, shaft.Morph, shaft.Show * num3, sink, bright, num8, num9);
				if (FxKit.Alive((GodotObject)(object)shaft.Wound))
				{
					if (shaft.State == 2)
					{
						float num16 = time - shaft.StuckAt;
						float num17 = 24f + 34f * Mathf.Exp(-9f * num16) + 14f * num4;
						((Node2D)shaft.Wound).GlobalPosition = val2;
						((Node2D)shaft.Wound).Scale = new Vector2(num17, num17) / 64f;
						float num18 = Mathf.Min(1f, 0.42f + 0.5f * Mathf.Exp(-6f * num16) + 0.4f * num4) * num3;
						((CanvasItem)shaft.Wound).Modulate = ((shaft.Kind == BeadKind.Chem) ? A(EnergyBeads.Ink, 0.85f * num18) : A(Pale, num18));
					}
					else
					{
						((CanvasItem)shaft.Wound).Modulate = A(Pale, 0f);
					}
				}
				bool flag4 = (((!v.Remote & flag) && v.HasAim) & flag2) && shaft.State < 2 && shaft.Out >= 1f && (v.Playing || time - v.AimedAt >= (float)i * 0.035f);
				shaft.MarkShow = FxKit.Damp(shaft.MarkShow, flag4 ? 1f : 0f, flag4 ? 12f : 16f, dt);
				if (!FxKit.Alive((GodotObject)(object)shaft.Mark))
				{
					continue;
				}
				float num19 = ((shaft.MarkPop < 0f) ? 0f : Mathf.Clamp((time - shaft.MarkPop) / 0.18f, 0f, 1f));
				float num20 = ((shaft.MarkPop < 0f) ? shaft.MarkShow : (1f - num19));
				float num21 = ((shaft.MarkPop < 0f) ? (Mathf.Lerp(2.3f, 1f, Mathf.SmoothStep(0f, 1f, shaft.MarkShow)) * (1f + 0.04f * Mathf.Sin(time * 7f + (float)i * 1.7f))) : (1f + 0.9f * num19));
				shaft.Mark.GlobalPosition = val2;
				shaft.Mark.Scale = new Vector2(num21, num21) * (v.MarkSize / 100f);
				if (FxKit.Alive((GodotObject)(object)shaft.MarkMat))
				{
					shaft.MarkMat.SetShaderParameter(PAlpha, Variant.op_Implicit(0.95f * num20 * num3));
					shaft.MarkMat.SetShaderParameter(PSpin, Variant.op_Implicit((1f - shaft.MarkShow) * 1.5f));
				}
				if (shaft.MarkInkMat != null && FxKit.Alive((GodotObject)(object)shaft.MarkInkMat))
				{
					if (FxKit.Alive((GodotObject)(object)shaft.MarkMat))
					{
						shaft.MarkMat.SetShaderParameter(PAlpha, Variant.op_Implicit(0.95f * num20 * num3 * Mathf.Min(1f, 0.8f * EnergyBeads.RimLight)));
					}
					shaft.MarkInkMat.SetShaderParameter(PAlpha, Variant.op_Implicit(0.95f * num20 * num3));
					shaft.MarkInkMat.SetShaderParameter(PSpin, Variant.op_Implicit((1f - shaft.MarkShow) * 1.5f));
				}
			}
			v.Beads?.Step(time, dt, num3);
			if (num >= 1f)
			{
				Free(v);
			}
			else if (!v.Remote && v.Dismissed && (v.Beads == null || v.Beads.Settled(time) || time - v.DismissAt > 3f))
			{
				Free(v);
			}
		}

		private static bool RemoteOwnerPresent(Volley v)
		{
			if (v.Owner == null || !v.Owner.IsAlive || v.Owner.Player == null || MultiplayerFx.IsLocal(v.Owner))
			{
				return false;
			}
			CombatManager instance = CombatManager.Instance;
			CombatState val = ((instance != null) ? instance.DebugOnlyGetState() : null);
			if (val == null || (object)v.Owner.CombatState != val)
			{
				return false;
			}
			foreach (Player player in val.Players)
			{
				if (player == v.Owner.Player)
				{
					return true;
				}
			}
			return false;
		}

		private static void Pose(Volley v, Shaft s, Vector2 at, float rot, float m, float alpha, float sink, float bright, float stretch, float lenMul)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_043e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0457: Unknown result type (might be due to invalid IL or missing references)
			//IL_0464: Unknown result type (might be due to invalid IL or missing references)
			//IL_0469: Unknown result type (might be due to invalid IL or missing references)
			//IL_047a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0484: Unknown result type (might be due to invalid IL or missing references)
			//IL_0489: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			//IL_049a: Unknown result type (might be due to invalid IL or missing references)
			//IL_049f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04be: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0512: Unknown result type (might be due to invalid IL or missing references)
			//IL_0514: Unknown result type (might be due to invalid IL or missing references)
			//IL_051c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0523: Unknown result type (might be due to invalid IL or missing references)
			//IL_0528: Unknown result type (might be due to invalid IL or missing references)
			//IL_0548: Unknown result type (might be due to invalid IL or missing references)
			//IL_0555: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)s.Root))
			{
				return;
			}
			if (s.Kind == BeadKind.Chem)
			{
				PoseBlack(v, s, at, rot, m, alpha, sink, bright, stretch, lenMul);
				return;
			}
			s.Root.GlobalPosition = at;
			s.Root.GlobalRotation = rot;
			float num = Mathf.Clamp(alpha, 0f, 1f);
			float num2 = v.Len * lenMul;
			float num3 = v.Thick * (1f + 0.25f * bright);
			float bead = v.Bead;
			float num4 = 1f + 0.6f * bright;
			Vector2 val = new Vector2((0f - stretch) * 0.5f, 0f);
			if (FxKit.Alive((GodotObject)(object)s.Halo))
			{
				((Node2D)s.Halo).Position = val;
				((Node2D)s.Halo).Scale = new Vector2(Mathf.Lerp(bead * 2.6f, num2 * 1.5f, m) + stretch, Mathf.Lerp(bead * 2.6f, num3 * 3.2f, m)) / 64f;
				((CanvasItem)s.Halo).Modulate = A(Energy, Mathf.Min(1f, 0.28f * num4) * num);
			}
			if (FxKit.Alive((GodotObject)(object)s.Body))
			{
				((Node2D)s.Body).Position = val;
				((Node2D)s.Body).Scale = new Vector2(Mathf.Lerp(bead * 1.2f, num2 * 1.3f, m) + stretch, Mathf.Lerp(bead * 1.2f, num3, m)) / 64f;
				((CanvasItem)s.Body).Modulate = A(Energy, 0.92f * num);
			}
			if (FxKit.Alive((GodotObject)(object)s.Core))
			{
				((Node2D)s.Core).Position = val * 0.6f;
				((Node2D)s.Core).Scale = new Vector2(Mathf.Lerp(bead * 0.6f, num2 * 1.1f, m) + stretch * 0.6f, Mathf.Lerp(bead * 0.6f, num3 * 0.36f, m)) / 64f;
				((CanvasItem)s.Core).Modulate = A(Pale, Mathf.Min(1f, 0.9f * num4) * num);
			}
			float num5 = Mathf.Lerp(bead, num2, m);
			float num6 = Mathf.SmoothStep(0.35f, 1f, m) * (1f - sink);
			if (FxKit.Alive((GodotObject)(object)s.Head))
			{
				((Node2D)s.Head).Position = new Vector2(num5 * 0.2f, 0f);
				((Node2D)s.Head).Scale = new Vector2(num5 * 0.62f, num3 * 1.75f) / 64f;
				((CanvasItem)s.Head).Modulate = A(Energy, 0.8f * num * num6);
			}
			float num7 = num5 * 0.25f;
			Vector2 val2 = new Vector2(num5 * 0.5f, 0f);
			Vector2 val3 = new Vector2(Mathf.Cos(0.56f), Mathf.Sin(0.56f)) * (num7 * 0.5f);
			Vector2 scale = new Vector2(num7 * 1.4f, num3 * 0.8f) / 64f;
			Color modulate = A(Pale, 0.85f * num * num6);
			if (FxKit.Alive((GodotObject)(object)s.BarbA))
			{
				((Node2D)s.BarbA).Position = val2 + new Vector2(0f - val3.X, 0f - val3.Y);
				((Node2D)s.BarbA).Rotation = 0.56f;
				((Node2D)s.BarbA).Scale = scale;
				((CanvasItem)s.BarbA).Modulate = modulate;
			}
			if (FxKit.Alive((GodotObject)(object)s.BarbB))
			{
				((Node2D)s.BarbB).Position = val2 + new Vector2(0f - val3.X, val3.Y);
				((Node2D)s.BarbB).Rotation = -0.56f;
				((Node2D)s.BarbB).Scale = scale;
				((CanvasItem)s.BarbB).Modulate = modulate;
			}
			float num8 = Mathf.SmoothStep(0.5f, 1f, m);
			float num9 = num5 * 0.16f;
			Vector2 val4 = new Vector2((0f - num5) * 0.46f, 0f);
			Vector2 val5 = new Vector2(Mathf.Cos(0.62f), Mathf.Sin(0.62f)) * (num9 * 0.5f);
			Vector2 scale2 = new Vector2(num9 * 1.4f, num3 * 0.7f) / 64f;
			Color modulate2 = A(Energy, 0.6f * num * num8);
			if (FxKit.Alive((GodotObject)(object)s.FletchA))
			{
				((Node2D)s.FletchA).Position = val4 + new Vector2(0f - val5.X, 0f - val5.Y);
				((Node2D)s.FletchA).Rotation = 0.62f;
				((Node2D)s.FletchA).Scale = scale2;
				((CanvasItem)s.FletchA).Modulate = modulate2;
			}
			if (FxKit.Alive((GodotObject)(object)s.FletchB))
			{
				((Node2D)s.FletchB).Position = val4 + new Vector2(0f - val5.X, val5.Y);
				((Node2D)s.FletchB).Rotation = -0.62f;
				((Node2D)s.FletchB).Scale = scale2;
				((CanvasItem)s.FletchB).Modulate = modulate2;
			}
		}

		private static void PoseBlack(Volley v, Shaft s, Vector2 at, float rot, float m, float alpha, float sink, float bright, float stretch, float lenMul)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			//IL_044c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0459: Unknown result type (might be due to invalid IL or missing references)
			//IL_045e: Unknown result type (might be due to invalid IL or missing references)
			//IL_046f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0479: Unknown result type (might be due to invalid IL or missing references)
			//IL_047e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0480: Unknown result type (might be due to invalid IL or missing references)
			//IL_0499: Unknown result type (might be due to invalid IL or missing references)
			//IL_049e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0511: Unknown result type (might be due to invalid IL or missing references)
			//IL_0513: Unknown result type (might be due to invalid IL or missing references)
			//IL_051b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0522: Unknown result type (might be due to invalid IL or missing references)
			//IL_0527: Unknown result type (might be due to invalid IL or missing references)
			//IL_0547: Unknown result type (might be due to invalid IL or missing references)
			//IL_0554: Unknown result type (might be due to invalid IL or missing references)
			s.Root.GlobalPosition = at;
			s.Root.GlobalRotation = rot;
			float num = Mathf.Clamp(alpha, 0f, 1f);
			float num2 = v.Len * lenMul;
			float num3 = v.Thick * (1f + 0.25f * bright);
			float bead = v.Bead;
			float num4 = 1f + 0.6f * bright;
			float rimLight = EnergyBeads.RimLight;
			Color rim = EnergyBeads.Rim;
			Color ink = EnergyBeads.Ink;
			Vector2 val = new Vector2((0f - stretch) * 0.5f, 0f);
			if (FxKit.Alive((GodotObject)(object)s.Halo))
			{
				((Node2D)s.Halo).Position = val;
				((Node2D)s.Halo).Scale = new Vector2(Mathf.Lerp(bead * 2.6f, num2 * 1.5f, m) + stretch, Mathf.Lerp(bead * 2.6f, num3 * 3.2f, m)) / 64f;
				((CanvasItem)s.Halo).Modulate = A(rim, Mathf.Min(1f, 0.28f * num4) * EnergyBeads.HaloLight * num);
			}
			if (FxKit.Alive((GodotObject)(object)s.Body))
			{
				((Node2D)s.Body).Position = val;
				((Node2D)s.Body).Scale = new Vector2(Mathf.Lerp(bead * 1.6f, num2 * 1.36f, m) + stretch, Mathf.Lerp(bead * 1.6f, num3 * 2.1f, m)) / 64f;
				((CanvasItem)s.Body).Modulate = A(rim, Mathf.Min(1f, 0.95f * rimLight * num4) * num);
			}
			if (FxKit.Alive((GodotObject)(object)s.Core))
			{
				((Node2D)s.Core).Position = val * 0.6f;
				((Node2D)s.Core).Scale = new Vector2(Mathf.Lerp(bead * 0.84f, num2 * 1.1f, m) + stretch * 0.8f, Mathf.Lerp(bead * 0.84f, num3 * 0.95f, m)) / 64f;
				((CanvasItem)s.Core).Modulate = A(ink, 0.96f * num);
			}
			float num5 = Mathf.Lerp(bead, num2, m);
			float num6 = Mathf.SmoothStep(0.35f, 1f, m) * (1f - sink);
			if (FxKit.Alive((GodotObject)(object)s.Head))
			{
				((Node2D)s.Head).Position = new Vector2(num5 * 0.2f, 0f);
				((Node2D)s.Head).Scale = new Vector2(num5 * 0.62f, num3 * 1.3f) / 64f;
				((CanvasItem)s.Head).Modulate = A(ink, 0.9f * num * num6);
			}
			float num7 = num5 * 0.25f;
			Vector2 val2 = new Vector2(num5 * 0.5f, 0f);
			Vector2 val3 = new Vector2(Mathf.Cos(0.56f), Mathf.Sin(0.56f)) * (num7 * 0.5f);
			Vector2 scale = new Vector2(num7 * 1.4f, num3 * 0.8f) / 64f;
			Color modulate = A(rim, Mathf.Min(1f, 0.85f * rimLight) * num * num6);
			if (FxKit.Alive((GodotObject)(object)s.BarbA))
			{
				((Node2D)s.BarbA).Position = val2 + new Vector2(0f - val3.X, 0f - val3.Y);
				((Node2D)s.BarbA).Rotation = 0.56f;
				((Node2D)s.BarbA).Scale = scale;
				((CanvasItem)s.BarbA).Modulate = modulate;
			}
			if (FxKit.Alive((GodotObject)(object)s.BarbB))
			{
				((Node2D)s.BarbB).Position = val2 + new Vector2(0f - val3.X, val3.Y);
				((Node2D)s.BarbB).Rotation = -0.56f;
				((Node2D)s.BarbB).Scale = scale;
				((CanvasItem)s.BarbB).Modulate = modulate;
			}
			float num8 = Mathf.SmoothStep(0.5f, 1f, m);
			float num9 = num5 * 0.16f;
			Vector2 val4 = new Vector2((0f - num5) * 0.46f, 0f);
			Vector2 val5 = new Vector2(Mathf.Cos(0.62f), Mathf.Sin(0.62f)) * (num9 * 0.5f);
			Vector2 scale2 = new Vector2(num9 * 1.4f, num3 * 0.7f) / 64f;
			Color modulate2 = A(rim, Mathf.Min(1f, 0.55f * rimLight) * num * num8);
			if (FxKit.Alive((GodotObject)(object)s.FletchA))
			{
				((Node2D)s.FletchA).Position = val4 + new Vector2(0f - val5.X, 0f - val5.Y);
				((Node2D)s.FletchA).Rotation = 0.62f;
				((Node2D)s.FletchA).Scale = scale2;
				((CanvasItem)s.FletchA).Modulate = modulate2;
			}
			if (FxKit.Alive((GodotObject)(object)s.FletchB))
			{
				((Node2D)s.FletchB).Position = val4 + new Vector2(0f - val5.X, val5.Y);
				((Node2D)s.FletchB).Rotation = -0.62f;
				((Node2D)s.FletchB).Scale = scale2;
				((CanvasItem)s.FletchB).Modulate = modulate2;
			}
		}

		static SkewerFx()
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		}
	}
