// namespace: SilentFX.Cards
// type: StormHeld (class)
// source lines: 105521-106594
	public sealed class StormHeld : HeldSession, IShivSource
	{
		private sealed class Arc
		{
			public Line2D Back;

			public Line2D Front;

			public Gradient GradBack;

			public Gradient GradFront;
		}

		private sealed class Layer
		{
			public bool HasCard;

			public NCard Card;

			public CardModel Model;

			public Color CardTint;

			public Color FaceTint;

			public float CardTurn;

			public Vector2 Home;

			public Arc[] Arcs;

			public Sprite2D Haze;

			public Line2D Stream;

			public Node2D Ghost;

			public Node2D Blade;

			public bool BladeTaken;

			public bool BladeNear;

			public float Level;

			public float Angle;

			public float Way;

			public int Order;

			public float U;

			public float GoAt = -1f;

			public Vector2 Head;

			public Vector2 HeadDir;
		}

		private sealed class Speck
		{
			public Line2D Back;

			public Line2D Front;

			public float Phase;

			public float Turn;

			public float Way;
		}

		private static readonly StringName PAlpha = StringName.op_Implicit("alpha");

		private static readonly StringName PSpin = StringName.op_Implicit("spin");

		private const int LinePoints = 22;

		private const int StreamPoints = 9;

		private const int SpeckPoints = 4;

		private static readonly Color Steel = new Color(0.75f, 0.92f, 1f, 1f);

		private static readonly Dictionary<Control, Layer> Hidden = new Dictionary<Control, Layer>();

		private static ulong _hiddenUntil;

		private readonly List<Layer> _layers = new List<Layer>();

		private readonly List<Speck> _specks = new List<Speck>();

		private readonly Vector2[] _ring = new Vector2[22];

		private readonly bool[] _near = new bool[22];

		private readonly float[] _lengths = new float[22];

		private readonly Vector2[] _pointsBack = new Vector2[22];

		private readonly Vector2[] _pointsFront = new Vector2[22];

		private readonly float[] _offsets = new float[22];

		private readonly Color[] _colorsBack = new Color[22];

		private readonly Color[] _colorsFront = new Color[22];

		private readonly Vector2[] _streamPoints = new Vector2[9];

		private readonly Vector2[] _speckPoints = new Vector2[4];

		private Node2D _ghosts;

		private Node2D _floor;

		private ShaderMaterial _floorMat;

		private Sprite2D _column;

		private int _cards;

		private float _lift;

		private float _gulp;

		private bool _wasLifted;

		private bool _played;

		private bool _handsOff;

		private float _playedAt;

		private float _linesGoneAt;

		private bool _cardDone;

		private Vector2 _pile;

		public static bool Swallowed(Control card)
		{
			if (card == null || Hidden.Count == 0)
			{
				return false;
			}
			if (FxClock.Milliseconds > _hiddenUntil)
			{
				Hidden.Clear();
				return false;
			}
			if (Hidden.TryGetValue(card, out var value) && FxKit.Alive((GodotObject)(object)value.Card))
			{
				return value.Card.Model == value.Model;
			}
			return false;
		}

		protected override void Build()
		{
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Expected Obj, but got Unknown
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_034b: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Expected Obj, but got Unknown
			//IL_0385: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Expected Obj, but got Unknown
			//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0535: Unknown result type (might be due to invalid IL or missing references)
			//IL_0558: Unknown result type (might be due to invalid IL or missing references)
			//IL_055d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0562: Unknown result type (might be due to invalid IL or missing references)
			//IL_0516: Unknown result type (might be due to invalid IL or missing references)
			//IL_0502: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0606: Unknown result type (might be due to invalid IL or missing references)
			//IL_0611: Unknown result type (might be due to invalid IL or missing references)
			//IL_0618: Unknown result type (might be due to invalid IL or missing references)
			//IL_0624: Expected Obj, but got Unknown
			//IL_063b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0640: Unknown result type (might be due to invalid IL or missing references)
			//IL_064c: Expected Obj, but got Unknown
			//IL_065e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0663: Unknown result type (might be due to invalid IL or missing references)
			//IL_0674: Unknown result type (might be due to invalid IL or missing references)
			//IL_0679: Unknown result type (might be due to invalid IL or missing references)
			//IL_068a: Unknown result type (might be due to invalid IL or missing references)
			//IL_068f: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fd: Expected Obj, but got Unknown
			//IL_071f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0759: Unknown result type (might be due to invalid IL or missing references)
			//IL_075e: Unknown result type (might be due to invalid IL or missing references)
			//IL_076f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0774: Unknown result type (might be due to invalid IL or missing references)
			//IL_0785: Unknown result type (might be due to invalid IL or missing references)
			//IL_078a: Unknown result type (might be due to invalid IL or missing references)
			//IL_079b: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_051b: Unknown result type (might be due to invalid IL or missing references)
			List<NCard> list = new List<NCard>();
			if (!C.Remote && C.Card != null)
			{
				try
				{
					NPlayerHand instance = NPlayerHand.Instance;
					if (FxKit.Alive((GodotObject)(object)instance))
					{
						foreach (NHandCardHolder activeHolder in instance.ActiveHolders)
						{
							if (FxKit.Alive((GodotObject)(object)activeHolder) && ((NCardHolder)activeHolder).CardModel != C.Card)
							{
								NCard cardNode = ((NCardHolder)activeHolder).CardNode;
								if (FxKit.Alive((GodotObject)(object)cardNode))
								{
									list.Add(cardNode);
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("Storm of Steel: could not read the hand: " + ex.Message);
				}
			}
			ref int cards = ref _cards;
			int num;
			if (C.Remote)
			{
				num = StormOfSteelFx.SnapshotHandCount(C.Card, 10);
			}
			else
			{
				num = ((C.Card == null) ? 6 : Mathf.Min(list.Count, 10));
			}
			cards = num;
			int num2 = Mathf.Clamp(Mathf.Max(_cards, Tuning.I("storm.min_layers", 7)), 1, 10);
			if (!C.Remote)
			{
				Node val = (FxKit.Alive((GodotObject)(object)C.UiParent) ? C.UiParent : C.FrontParent);
				_ghosts = new Node2D
				{
					Name = StringName.op_Implicit("SilentFX_StormGhosts")
				};
				val.AddChild((Node)(object)_ghosts, false, (InternalMode)0);
				_pile = GameRefs.PilePoint((PileType)3);
			}
			_column = FxKit.Glow(new Color(Steel.R, Steel.G, Steel.B, 0f), new Vector2(64f, 64f));
			((Node)Back).AddChild((Node)(object)_column, false, (InternalMode)0);
			_floor = FxKit.Sigil(C.Body.X * 1.9f, Steel, out _floorMat);
			((Node)Back).AddChild((Node)(object)_floor, false, (InternalMode)0);
			if (_floorMat != null)
			{
				_floorMat.SetShaderParameter(StringName.op_Implicit("radius"), Variant.op_Implicit(0.8f));
				_floorMat.SetShaderParameter(StringName.op_Implicit("width"), Variant.op_Implicit(0.022f));
				_floorMat.SetShaderParameter(StringName.op_Implicit("dashes"), Variant.op_Implicit(9f));
				_floorMat.SetShaderParameter(StringName.op_Implicit("glow"), Variant.op_Implicit(0.3f));
				_floorMat.SetShaderParameter(PAlpha, Variant.op_Implicit(0f));
			}
			for (int i = 0; i < num2; i++)
			{
				Layer layer = new Layer
				{
					Level = ((num2 == 1) ? 0.5f : ((float)i / (float)(num2 - 1))),
					Angle = (float)i * 2.399f,
					Way = ((i % 2 == 0) ? 1f : (-1f)),
					Arcs = new Arc[2]
				};
				layer.Haze = FxKit.Glow(new Color(Steel.R, Steel.G, Steel.B, 0f), new Vector2(64f, 64f));
				((Node)Back).AddChild((Node)(object)layer.Haze, false, (InternalMode)0);
				for (int j = 0; j < 2; j++)
				{
					Arc arc = new Arc
					{
						GradBack = new Gradient(),
						GradFront = new Gradient()
					};
					arc.Back = MakeLine(arc.GradBack);
					arc.Front = MakeLine(arc.GradFront);
					((Node)Back).AddChild((Node)(object)arc.Back, false, (InternalMode)0);
					((Node)Front).AddChild((Node)(object)arc.Front, false, (InternalMode)0);
					layer.Arcs[j] = arc;
				}
				_layers.Add(layer);
			}
			for (int k = 0; k < _cards; k++)
			{
				int l;
				for (l = Mathf.Clamp(Mathf.RoundToInt(((float)k + 0.5f) * (float)num2 / (float)_cards - 0.5f), 0, num2 - 1); _layers[l].HasCard && l < num2 - 1; l++)
				{
				}
				Layer layer2 = _layers[l];
				if (!layer2.HasCard)
				{
					layer2.HasCard = true;
					layer2.Order = k;
					if (k < list.Count)
					{
						layer2.Card = list[k];
						layer2.Model = list[k].Model;
						layer2.CardTint = ((CanvasItem)list[k]).Modulate;
						layer2.CardTurn = ((Control)list[k]).Rotation;
						layer2.FaceTint = (FxKit.Alive((GodotObject)(object)list[k].Body) ? ((CanvasItem)list[k].Body).Modulate : Colors.White);
					}
					if (!C.Remote)
					{
						layer2.Home = C.Hand + new Vector2(((float)k - (float)(_cards - 1) * 0.5f) * 120f, 0f);
					}
					layer2.Blade = ShivFx.MakeShiv(ink: false);
					((CanvasItem)layer2.Blade).Visible = false;
					((Node)Front).AddChild((Node)(object)layer2.Blade, false, (InternalMode)0);
					layer2.BladeNear = true;
					if (!C.Remote)
					{
						layer2.Stream = new Line2D
						{
							Width = 3f,
							DefaultColor = new Color(Steel.R, Steel.G, Steel.B, 0.85f),
							BeginCapMode = (LineCapMode)2,
							EndCapMode = (LineCapMode)2,
							JointMode = (LineJointMode)2,
							Material = (Material)(object)FxKit.Additive,
							Antialiased = true,
							Visible = false
						};
						((Node)_ghosts).AddChild((Node)(object)layer2.Stream, false, (InternalMode)0);
						layer2.Ghost = new Node2D
						{
							Visible = false
						};
						Polygon2D val2 = FxKit.Shape(new Vector2[4]
						{
							new Vector2(-48f, -66f),
							new Vector2(48f, -66f),
							new Vector2(48f, 66f),
							new Vector2(-48f, 66f)
						}, new Color(Steel.R, Steel.G, Steel.B, 0.26f));
						((CanvasItem)val2).Material = (Material)(object)FxKit.Additive;
						((Node)layer2.Ghost).AddChild((Node)(object)val2, false, (InternalMode)0);
						Line2D val3 = new Line2D();
						val3.Width = 2.5f;
						val3.DefaultColor = new Color(0.9f, 0.97f, 1f, 0.85f);
						val3.Closed = true;
						((CanvasItem)val3).Material = (Material)(object)FxKit.Additive;
						val3.Antialiased = true;
						val3.Points = new Vector2[4]
						{
							new Vector2(-48f, -66f),
							new Vector2(48f, -66f),
							new Vector2(48f, 66f),
							new Vector2(-48f, 66f)
						};
						Line2D val4 = val3;
						((Node)layer2.Ghost).AddChild((Node)(object)val4, false, (InternalMode)0);
						((Node)_ghosts).AddChild((Node)(object)layer2.Ghost, false, (InternalMode)0);
					}
				}
			}
			int num3 = Mathf.Clamp(Tuning.I("storm.grit", 16), 0, 40);
			for (int m = 0; m < num3; m++)
			{
				Speck speck = new Speck
				{
					Phase = FxKit.Hash(m * 7 + 2),
					Turn = FxKit.Hash(m * 3 + 5) * ((float)Math.PI * 2f),
					Way = ((m % 2 == 0) ? 1f : (-1f)),
					Back = MakeDash(),
					Front = MakeDash()
				};
				((Node)Back).AddChild((Node)(object)speck.Back, false, (InternalMode)0);
				((Node)Front).AddChild((Node)(object)speck.Front, false, (InternalMode)0);
				_specks.Add(speck);
			}
		}

		private static Line2D MakeLine(Gradient gradient)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Expected Obj, but got Unknown
			return new Line2D
			{
				Width = 3f,
				DefaultColor = Colors.White,
				Gradient = gradient,
				BeginCapMode = (LineCapMode)2,
				EndCapMode = (LineCapMode)2,
				JointMode = (LineJointMode)2,
				Material = (Material)(object)FxKit.Additive,
				Antialiased = true
			};
		}

		private static Line2D MakeDash()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Expected Obj, but got Unknown
			return new Line2D
			{
				Width = 2.6f,
				DefaultColor = new Color(0.92f, 0.98f, 1f, 1f),
				BeginCapMode = (LineCapMode)2,
				EndCapMode = (LineCapMode)2,
				Material = (Material)(object)FxKit.Additive,
				Antialiased = true,
				Visible = false
			};
		}

		private static float Facing(Vector2 dir)
		{
			return dir.Angle() + (float)Math.PI / 2f;
		}

		private void Ring(float level, out Vector2 centre, out float rx, out float ry)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			Vector2 body = C.Body;
			float num = Mathf.Sin(Time * 2.1f - level * 2.6f) * (3f + 20f * level) * Tuning.F("storm.sway", 1f);
			centre = C.Feet + new Vector2(num, (0f - body.Y) * (0.04f + Tuning.F("storm.height", 1.22f) * level));
			rx = body.X * (0.14f + Tuning.F("storm.spread", 0.52f) * Mathf.Pow(level, 1.3f)) + 10f;
			rx *= 1f - 0.1f * Arm;
			ry = rx * 0.26f;
		}

		private static Vector2 OnRing(Vector2 centre, float rx, float ry, float angle)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			return centre + new Vector2(Mathf.Cos(angle) * rx, Mathf.Sin(angle) * ry);
		}

		private Vector2 Foot()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			return C.Feet + new Vector2(0f, (0f - C.Body.Y) * 0.04f);
		}

		protected override void Tick(float dt)
		{
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0624: Unknown result type (might be due to invalid IL or missing references)
			//IL_0641: Unknown result type (might be due to invalid IL or missing references)
			//IL_064b: Unknown result type (might be due to invalid IL or missing references)
			//IL_069c: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_071e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0747: Unknown result type (might be due to invalid IL or missing references)
			//IL_04af: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0521: Unknown result type (might be due to invalid IL or missing references)
			//IL_052b: Unknown result type (might be due to invalid IL or missing references)
			//IL_057b: Unknown result type (might be due to invalid IL or missing references)
			bool flag = !Hovering && !Leaving;
			float num = 0.42f + 0.035f * (float)_cards;
			_lift = Mathf.Clamp(_lift + (flag ? dt : ((0f - dt) * (Leaving ? 3f : 1.4f))), 0f, num);
			float num2 = Mathf.Clamp(Fade, 0f, 1f);
			float num3 = Mathf.Clamp(Lift, 0f, 1f);
			if ((!C.Remote & flag) && !_played)
			{
				Sfx.Hold("storm_wind", num3 * (0.55f + 0.45f * Arm), 0.92f + 0.22f * Arm);
			}
			if (FxKit.Alive((GodotObject)(object)_ghosts))
			{
				((CanvasItem)_ghosts).Modulate = new Color(1f, 1f, 1f, num2);
			}
			if (_played && !Leaving && Time > _linesGoneAt && (_cardDone || Time > _playedAt + 4f))
			{
				Leave();
			}
			if (flag && !_played)
			{
				_gulp -= dt;
				if (!_wasLifted || _gulp <= 0f)
				{
					_gulp = (_wasLifted ? Tuning.F("storm.gust_every", 0.42f) : 0.22f);
					FxKit.RingPulse((Node)(object)Back, Foot(), C.Body.X * (_wasLifted ? 2.6f : 3.2f), Steel, 0f, 0.36f, 1.25f, 0.22f, 0.28f, 7f, 0.022f, _wasLifted ? 0.45f : 0.85f);
				}
			}
			_wasLifted = flag;
			float num4 = Tuning.F("storm.line_faint", 0.26f);
			float num5 = Tuning.F("storm.turn", 3f);
			float num6 = 1f + 1.3f * num3 + 0.9f * Arm + (Playing ? 0.8f : 0f);
			float num7 = (_played ? Mathf.Clamp((Time - _playedAt) / 0.35f, 0f, 1f) : 0f);
			for (int i = 0; i < _layers.Count; i++)
			{
				Layer layer = _layers[i];
				float num8 = (layer.U = (layer.HasCard ? Mathf.SmoothStep((float)layer.Order * 0.035f, (float)layer.Order * 0.035f + 0.42f, _lift) : 0f));
				float num9 = num8 * num8 * (3f - 2f * num8);
				Ring(layer.Level, out var centre, out var rx, out var ry);
				layer.Angle += dt * layer.Way * num5 * (1.25f - 0.45f * layer.Level) * num6 * (1f + 0.5f * num9);
				layer.Head = OnRing(centre, rx, ry, layer.Angle);
				Vector2 val = new Vector2((0f - Mathf.Sin(layer.Angle)) * rx, Mathf.Cos(layer.Angle) * ry) * layer.Way;
				layer.HeadDir = val.Normalized();
				bool headNear = Mathf.Sin(layer.Angle) > 0f;
				float num10 = Mathf.Lerp(Mathf.Lerp(num4, num4 + 0.22f, num3), 0.98f, num9);
				float num11 = ((layer.GoAt < 0f) ? 0f : Mathf.Clamp((Time - layer.GoAt) / 0.34f, 0f, 1f));
				if (_played)
				{
					num10 *= (layer.HasCard ? (1f - num11 * num11) : (1f - num7));
				}
				float width = Mathf.Lerp(2.4f, 4.8f, num9) + 0.6f * num3 + 1f * Arm * num9;
				float reach = (float)Math.PI * (0.62f + 0.22f * num9);
				for (int j = 0; j < 2; j++)
				{
					DrawArc(layer.Arcs[j], centre, rx, ry, layer.Angle + (float)j * (float)Math.PI, layer.Way, reach, num10, width, num11);
				}
				if (FxKit.Alive((GodotObject)(object)layer.Haze))
				{
					((Node2D)layer.Haze).GlobalPosition = centre;
					((Node2D)layer.Haze).Scale = new Vector2(rx * 2.6f, ry * 6.5f + 26f) / 64f;
					((CanvasItem)layer.Haze).Modulate = new Color(Steel.R, Steel.G, Steel.B, (0.035f + 0.045f * num3 + 0.04f * num9) * num2 * (1f - num7));
				}
				PlaceCard(layer, i, num8, num2 * (0.45f + 0.55f * num3));
				PlaceStream(layer, num8, num3);
				PlaceGhost(layer, num8, num9);
				PlaceBlade(layer, num8, headNear);
			}
			if (FxKit.Alive((GodotObject)(object)_column))
			{
				Ring(0.5f, out var centre2, out var rx2, out var _);
				float num12 = C.Body.Y * Tuning.F("storm.height", 1.22f);
				((Node2D)_column).GlobalPosition = centre2;
				((Node2D)_column).Scale = new Vector2(rx2 * 2.2f, num12 * 1.5f) / 64f;
				((CanvasItem)_column).Modulate = new Color(Steel.R, Steel.G, Steel.B, (0.05f + 0.07f * num3 + 0.05f * Arm) * (1f - num7));
			}
			if (FxKit.Alive((GodotObject)(object)_floor))
			{
				_floor.GlobalPosition = Foot();
				float num13 = 0.8f + 0.2f * num3;
				_floor.Scale = new Vector2(num13, num13 * 0.28f);
				if (FxKit.Alive((GodotObject)(object)_floorMat))
				{
					_floorMat.SetShaderParameter(PAlpha, Variant.op_Implicit((0.2f + 0.4f * num3) * (1f - num7)));
					_floorMat.SetShaderParameter(PSpin, Variant.op_Implicit(Time * (2.5f + 3.5f * num3)));
				}
			}
			PlaceSpecks(num3, num7);
		}

		private void DrawArc(Arc arc, Vector2 centre, float rx, float ry, float head, float way, float reach, float bright, float width, float go)
		{
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			if (C.Remote)
			{
				go = 0f;
			}
			if (!FxKit.Alive((GodotObject)(object)arc.Back) || !FxKit.Alive((GodotObject)(object)arc.Front))
			{
				return;
			}
			bool flag = bright > 0.01f;
			((CanvasItem)arc.Back).Visible = flag && go <= 0f;
			((CanvasItem)arc.Front).Visible = flag;
			if (!flag)
			{
				return;
			}
			float num = 0f;
			for (int i = 0; i < 22; i++)
			{
				float num2 = head - way * reach * (1f - (float)i / 21f);
				Vector2 val = OnRing(centre, rx, ry, num2);
				_near[i] = Mathf.Sin(num2) > 0f;
				if (go > 0f)
				{
					float num3 = Mathf.Clamp(go * 1.3f - (float)(21 - i) * 0.014285715f, 0f, 1f);
					Vector2 b = val.Lerp(_pile, 0.5f) + new Vector2(0f, -140f);
					val = FxKit.Bezier(val, b, _pile, num3 * num3);
					_near[i] = true;
				}
				_ring[i] = val;
				if (i > 0)
				{
					num += _ring[i].DistanceTo(_ring[i - 1]);
				}
				_lengths[i] = num;
			}
			if (num < 0.001f)
			{
				num = 1f;
			}
			for (int j = 0; j < 22; j++)
			{
				_pointsBack[j] = ((Node2D)arc.Back).ToLocal(_ring[j]);
				_pointsFront[j] = ((Node2D)arc.Front).ToLocal(_ring[j]);
				_offsets[j] = ((j == 21) ? 1f : Mathf.Min(0.999f, _lengths[j] / num + (float)j * 1E-05f));
				float num4 = Mathf.Pow((float)j / 21f, 0.8f);
				_colorsFront[j] = new Color(Steel.R, Steel.G, Steel.B, _near[j] ? (num4 * bright) : 0f);
				_colorsBack[j] = new Color(Steel.R, Steel.G, Steel.B, _near[j] ? 0f : (num4 * bright * 0.75f));
			}
			arc.Back.Points = _pointsBack;
			arc.Front.Points = _pointsFront;
			arc.Back.Width = width;
			arc.Front.Width = width;
			arc.GradBack.Offsets = _offsets;
			arc.GradBack.Colors = _colorsBack;
			arc.GradFront.Offsets = _offsets;
			arc.GradFront.Colors = _colorsFront;
		}

		private void PlaceCard(Layer layer, int i, float u, float pull)
		{
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)layer.Card) || layer.Card.Model != layer.Model)
			{
				return;
			}
			if (_played)
			{
				if (((CanvasItem)layer.Card).Visible && Swallowed((Control)(object)layer.Card))
				{
					((CanvasItem)layer.Card).Visible = false;
				}
			}
			else
			{
				if (_handsOff)
				{
					return;
				}
				layer.Home = ((CanvasItem)layer.Card).GetGlobalTransform() * (((Control)layer.Card).Size * 0.5f);
				float num = (1f - u) * pull;
				float num2 = -1f;
				foreach (FxTarget enemy in C.Enemies)
				{
					if (!(enemy.Side >= 0f))
					{
						num2 = ((Foot().X > layer.Home.X) ? 1f : (-1f));
						break;
					}
				}
				float num3 = num2 * Tuning.F("storm.lean", 0.07f) * num;
				float num4 = Mathf.Sin(Time * 41f + (float)i * 1.9f) * Tuning.F("storm.shake", 0.024f) * num;
				((Control)layer.Card).Rotation = layer.CardTurn + num3 + num4;
				((CanvasItem)layer.Card).Modulate = new Color(layer.CardTint.R, layer.CardTint.G, layer.CardTint.B, layer.CardTint.A * Mathf.Lerp(1f, Tuning.F("storm.card_alpha", 0.16f), u));
			}
		}

		private void PlaceStream(Layer layer, float u, float strong)
		{
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)layer.Stream))
			{
				return;
			}
			float num = (1f - u) * (_played ? 0f : 1f);
			((CanvasItem)layer.Stream).Visible = num > 0.02f;
			if (((CanvasItem)layer.Stream).Visible)
			{
				Vector2 val = layer.Home + new Vector2(0f, -70f);
				Vector2 val2 = Foot();
				Vector2 b = new Vector2(Mathf.Lerp(val.X, val2.X, 0.45f), Mathf.Max(val.Y, val2.Y) + 40f);
				float num2 = Time * Tuning.F("storm.pull_speed", 2.3f) + (float)layer.Order * 0.37f;
				float num3 = num2 - Mathf.Floor(num2);
				for (int i = 0; i < 9; i++)
				{
					float t = Mathf.Clamp(num3 * 1.25f - 0.25f * (1f - (float)i / 8f), 0f, 1f);
					_streamPoints[i] = ((Node2D)layer.Stream).ToLocal(FxKit.Bezier(val, b, val2, t));
				}
				layer.Stream.Points = _streamPoints;
				layer.Stream.Width = 2.2f + 1.6f * strong;
				((CanvasItem)layer.Stream).Modulate = new Color(1f, 1f, 1f, Mathf.Sin((float)Math.PI * num3) * (0.3f + 0.5f * strong) * num);
			}
		}

		private void PlaceGhost(Layer layer, float u, float e)
		{
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)layer.Ghost))
			{
				return;
			}
			bool flag = u > 0.01f && u < 0.99f && !_played;
			((CanvasItem)layer.Ghost).Visible = flag;
			if (flag)
			{
				Vector2 globalPosition;
				if (e < 0.5f)
				{
					float num = e / 0.5f;
					Vector2 val = Foot();
					globalPosition = FxKit.Bezier(b: new Vector2(Mathf.Lerp(layer.Home.X, val.X, 0.4f), Mathf.Max(layer.Home.Y, val.Y) + 30f), a: layer.Home, c: val, t: num * num);
				}
				else
				{
					float num2 = (e - 0.5f) / 0.5f;
					Ring(layer.Level * num2, out var centre, out var rx, out var ry);
					globalPosition = OnRing(centre, rx, ry, layer.Angle - layer.Way * (1f - num2) * ((float)Math.PI * 2f) * 1.25f);
				}
				layer.Ghost.GlobalPosition = globalPosition;
				float num3 = Mathf.Lerp(1f, 0.12f, e);
				layer.Ghost.Scale = new Vector2(num3, num3 * (1f + 0.9f * Mathf.Sin((float)Math.PI * e)));
				layer.Ghost.Rotation = layer.Way * e * 7f;
				((CanvasItem)layer.Ghost).Modulate = new Color(1f, 1f, 1f, Mathf.Sin((float)Math.PI * u) * 0.9f);
			}
		}

		private void PlaceBlade(Layer layer, float u, bool headNear)
		{
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			if (layer.BladeTaken || !FxKit.Alive((GodotObject)(object)layer.Blade))
			{
				return;
			}
			float num = Mathf.SmoothStep(0.55f, 1f, u);
			((CanvasItem)layer.Blade).Visible = num > 0.01f;
			if (!((CanvasItem)layer.Blade).Visible)
			{
				return;
			}
			if (headNear != layer.BladeNear)
			{
				layer.BladeNear = headNear;
				Node parent = ((Node)layer.Blade).GetParent();
				if (parent != null)
				{
					parent.RemoveChild((Node)(object)layer.Blade);
				}
				((Node)(headNear ? Front : Back)).AddChild((Node)(object)layer.Blade, false, (InternalMode)0);
			}
			layer.Blade.GlobalPosition = layer.Head;
			layer.Blade.Rotation = Facing(layer.HeadDir);
			layer.Blade.Scale = new Vector2(num, num) * (0.85f + 0.15f * Arm);
			((CanvasItem)layer.Blade).Modulate = new Color(1f, 1f, 1f, headNear ? 1f : 0.7f);
		}

		private void PlaceSpecks(float strong, float gone)
		{
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			float num = Tuning.F("storm.grit_speed", 0.75f) * (1f + 0.8f * strong);
			for (int i = 0; i < _specks.Count; i++)
			{
				Speck speck = _specks[i];
				if (!FxKit.Alive((GodotObject)(object)speck.Back) || !FxKit.Alive((GodotObject)(object)speck.Front))
				{
					continue;
				}
				float num2 = Time * num + speck.Phase;
				float num3 = num2 - Mathf.Floor(num2);
				float num4 = Mathf.Sin((float)Math.PI * num3) * (0.22f + 0.6f * strong) * (1f - gone);
				if (num4 < 0.02f)
				{
					((CanvasItem)speck.Back).Visible = false;
					((CanvasItem)speck.Front).Visible = false;
					continue;
				}
				Ring(num3, out var centre, out var rx, out var ry);
				float num5 = speck.Turn + speck.Way * Time * (5.5f + 3f * strong);
				bool flag = Mathf.Sin(num5) > 0f;
				Line2D val = (flag ? speck.Front : speck.Back);
				((CanvasItem)speck.Front).Visible = flag;
				((CanvasItem)speck.Back).Visible = !flag;
				for (int j = 0; j < 4; j++)
				{
					float angle = num5 - speck.Way * 0.42f * (1f - (float)j / 3f);
					_speckPoints[j] = ((Node2D)val).ToLocal(OnRing(centre, rx * 1.07f, ry * 1.07f, angle));
				}
				val.Points = _speckPoints;
				((CanvasItem)val).Modulate = new Color(1f, 1f, 1f, num4 * (flag ? 1f : 0.6f));
			}
		}

		public override void OnCardFinished()
		{
			_cardDone = true;
			if (!_played)
			{
				Leave();
			}
		}

		protected override void OnPlay(CardFxContext play)
		{
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			_played = true;
			_playedAt = Time;
			_linesGoneAt = Time + 0.4f;
			_handsOff = true;
			if (!C.Remote)
			{
				_hiddenUntil = FxClock.Milliseconds + 3500;
			}
			int num = 0;
			foreach (Layer layer in _layers)
			{
				if (!layer.HasCard)
				{
					continue;
				}
				layer.GoAt = Time + 0.04f + (float)num * 0.03f;
				_linesGoneAt = layer.GoAt + 0.4f;
				num++;
				if (!C.Remote && FxKit.Alive((GodotObject)(object)layer.Card) && layer.Card.Model == layer.Model)
				{
					((Control)layer.Card).Rotation = layer.CardTurn;
					((CanvasItem)layer.Card).Visible = false;
					if (FxKit.Alive((GodotObject)(object)layer.Card.Body))
					{
						((CanvasItem)layer.Card.Body).Modulate = new Color(1f, 1f, 1f, 0f);
					}
					Hidden[(Control)(object)layer.Card] = layer;
				}
			}
			Node frontParent = C.FrontParent;
			if (FxKit.Alive((GodotObject)(object)frontParent))
			{
				FxKit.Flash(frontParent, C.Chest, new Color(0.85f, 0.95f, 1f, 0.55f), C.Body.X * 1.4f, 1.9f, 0.26);
				FxKit.RingPulse(frontParent, Foot(), C.Body.X * 3.2f, Steel, 0f, 0.4f, 0.25f, 1.25f, 0.28f, 0f, 0.03f, 0.9f);
			}
			GameRefs.Shake(1);
		}

		public bool TryTake(out Node2D blade)
		{
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			blade = null;
			if (Finished)
			{
				return false;
			}
			foreach (Layer layer in _layers)
			{
				if (layer.HasCard && !layer.BladeTaken && FxKit.Alive((GodotObject)(object)layer.Blade))
				{
					layer.BladeTaken = true;
					Node2D blade2 = layer.Blade;
					layer.Blade = null;
					Vector2 globalPosition = blade2.GlobalPosition;
					float globalRotation = blade2.GlobalRotation;
					Node parent = ((Node)blade2).GetParent();
					if (parent != null)
					{
						parent.RemoveChild((Node)(object)blade2);
					}
					Node frontParent = C.FrontParent;
					if (!FxKit.Alive((GodotObject)(object)frontParent))
					{
						((Node)blade2).QueueFree();
						return false;
					}
					frontParent.AddChild((Node)(object)blade2, false, (InternalMode)0);
					FxKit.FreeAfter((Node)(object)blade2, 3.0);
					blade2.GlobalPosition = globalPosition;
					blade2.GlobalRotation = globalRotation;
					blade2.Scale = Vector2.One;
					((CanvasItem)blade2).Modulate = Colors.White;
					((CanvasItem)blade2).Visible = true;
					blade = blade2;
					return true;
				}
			}
			return false;
		}

		private void RestoreCards()
		{
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			if (C.Remote)
			{
				return;
			}
			HashSet<Control> hashSet = null;
			if (_played)
			{
				hashSet = new HashSet<Control>();
				try
				{
					NPlayerHand instance = NPlayerHand.Instance;
					if (FxKit.Alive((GodotObject)(object)instance))
					{
						foreach (NHandCardHolder activeHolder in instance.ActiveHolders)
						{
							if (FxKit.Alive((GodotObject)(object)activeHolder) && FxKit.Alive((GodotObject)(object)((NCardHolder)activeHolder).CardNode))
							{
								hashSet.Add((Control)(object)((NCardHolder)activeHolder).CardNode);
							}
						}
					}
				}
				catch
				{
				}
			}
			foreach (Layer layer in _layers)
			{
				if (layer.Card != null && Hidden.TryGetValue((Control)(object)layer.Card, out var value) && value == layer)
				{
					Hidden.Remove((Control)(object)layer.Card);
				}
				if (FxKit.Alive((GodotObject)(object)layer.Card) && layer.Card.Model == layer.Model && (hashSet == null || hashSet.Contains((Control)(object)layer.Card)))
				{
					((Control)layer.Card).Rotation = layer.CardTurn;
					((CanvasItem)layer.Card).Modulate = layer.CardTint;
					((CanvasItem)layer.Card).Visible = true;
					if (_played && FxKit.Alive((GodotObject)(object)layer.Card.Body))
					{
						((CanvasItem)layer.Card.Body).Modulate = layer.FaceTint;
					}
				}
			}
			_handsOff = true;
		}

		protected override void OnEnd()
		{
			try
			{
				RestoreCards();
			}
			catch (Exception ex)
			{
				DevLog.Warn("StormOfSteel restore: " + ex);
			}
			try
			{
				if (FxKit.Alive((GodotObject)(object)_ghosts))
				{
					((Node)_ghosts).QueueFree();
				}
			}
			catch (Exception ex2)
			{
				DevLog.Warn("StormOfSteel ghosts: " + ex2);
			}
		}

		static StormHeld()
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		}
	}
