// namespace: SilentFX.Cards
// type: WraithPlayFx (class)
// source lines: 119308-120543
	public sealed class WraithPlayFx : CardFx
	{
		internal sealed class Flight
		{
			public NCard Card;

			public readonly AnimatedSprite2D[] Fire = new AnimatedSprite2D[5];

			public Sprite2D Glow;

			public readonly Vector2[] Trail = new Vector2[10];

			public int Seen;

			public float Age;
		}

		internal sealed class Run
		{
			public object Owner;

			public CardModel Card;

			public WraithHeld2 Held;

			public Node2D Back;

			public Node2D Front;

			public Node2D Top;

			public Node Play;

			public float Show;

			public float TPre;

			public float T;

			public float Clock;

			public float Wait;

			public float Pace = 1f;

			public float TBurst = 0.6f;

			public float TGo = 0.67f;

			public float THit = 2.12f;

			public float TEnd = 3.72f;

			public bool Phased;

			public bool LetGo;

			public bool Going;

			public bool Flying;

			public float FlyLeft;

			public bool Knocked;

			public bool Screamed;

			public bool Landed;

			public bool SaidKnock;

			public bool SaidScream;

			public bool SaidLand;

			public bool Cancelled;

			public bool Done;

			public float TopShow = 1f;

			public int Phase;

			public ulong Born;

			public Vector2 Into;

			public Vector2 Feet;

			public Vector2 Body;

			public Flight Flight;

			public double ScriptTime;

			public double ScriptWorst;

			public int ScriptFrames;
		}

		private const int Tongues = 5;

		private static readonly StringName MSetTime = StringName.op_Implicit("set_time");

		private static readonly StringName MSetFade = StringName.op_Implicit("set_fade");

		private static readonly StringName MSetParam = StringName.op_Implicit("set_param");

		private static readonly StringName MSetTop = StringName.op_Implicit("set_top");

		private static readonly StringName MTimes = StringName.op_Implicit("times");

		private static readonly StringName MChest = StringName.op_Implicit("chest");

		private static readonly StringName AnimDefault = StringName.op_Implicit("default");

		private static readonly List<Run> Runs = new List<Run>();

		private static readonly Action<double> Driver = Drive;

		private static readonly Action<double> Follower = Follow;

		private static FieldRef<NCardFlyPowerVfx, Path2D> _swoosh;

		private static bool _swooshLooked;

		private readonly WraithFormIntroFx _old = new WraithFormIntroFx();

		public static int PhaseFor(object owner)
		{
			try
			{
				ulong ticksMsec = Time.GetTicksMsec();
				for (int num = Runs.Count - 1; num >= 0; num--)
				{
					Run run = Runs[num];
					if (!run.Done && !run.Cancelled && run.Going && run.Owner == owner && (ticksMsec <= run.Born || ticksMsec - run.Born <= 30000))
					{
						return run.Phase;
					}
				}
			}
			catch
			{
			}
			return 0;
		}

		public static void Reset()
		{
			for (int num = Runs.Count - 1; num >= 0; num--)
			{
				Free(Runs[num]);
			}
			Runs.Clear();
			try
			{
				WraithCardFire.Reset();
			}
			catch (Exception ex)
			{
				DevLog.Warn("WraithPlayFx.Reset: " + ex.Message);
			}
		}

		internal static Run Prepare(WraithHeld2 held, CardFxContext c)
		{
			if (c == null || !WraithStage.Ready)
			{
				return null;
			}
			if (Tuning.F("intro.on", 1f) < 0.5f || Tuning.F("wraith.intro.on", 1f) < 0.5f)
			{
				return null;
			}
			Run run = new Run();
			try
			{
				CardModel card = c.Card;
				object owner;
				if (card == null)
				{
					owner = null;
				}
				else
				{
					Player owner2 = card.Owner;
					owner = ((owner2 != null) ? owner2.Creature : null);
				}
				run.Owner = owner;
				run.Card = c.Card;
				run.Held = held;
				for (int num = Runs.Count - 1; num >= 0; num--)
				{
					if (Runs[num].Owner == run.Owner && !Runs[num].Going)
					{
						Free(Runs[num]);
						Runs.RemoveAt(num);
					}
				}
				if (!Begin(run, c))
				{
					Free(run);
					return null;
				}
				Runs.Add(run);
				Ticker.Add(Driver);
				Ticker.AddLate(Follower);
				return run;
			}
			catch (Exception ex)
			{
				DevLog.Warn("Wraith Form (made ready): " + ex.Message);
				Free(run);
				Runs.Remove(run);
				return null;
			}
		}

		internal static void LetGo(Run run)
		{
			if (run != null && !run.Done && !run.Cancelled)
			{
				run.LetGo = true;
			}
		}

		internal static void Cancel(Run run)
		{
			if (run != null && !run.Done && !run.Going)
			{
				run.Cancelled = true;
			}
		}

		public override void OnPlay(CardFxContext c)
		{
			if (c == null)
			{
				return;
			}
			if (!WraithStage.Ready)
			{
				_old.OnPlay(c);
			}
			else
			{
				if (Tuning.F("intro.on", 1f) < 0.5f || Tuning.F("wraith.intro.on", 1f) < 0.5f)
				{
					return;
				}
				try
				{
					Run run = null;
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
					object obj2 = obj;
					for (int num = Runs.Count - 1; num >= 0 && run == null; num--)
					{
						Run run2 = Runs[num];
						if (run2.Done || run2.Cancelled || run2.Going)
						{
							continue;
						}
						bool num2;
						if (c.Held == null)
						{
							if (run2.Owner != obj2)
							{
								continue;
							}
							num2 = run2.Card == c.Card;
						}
						else
						{
							num2 = run2.Held == c.Held;
						}
						if (num2)
						{
							run = run2;
						}
					}
					if (run == null)
					{
						run = Prepare(c.Held as WraithHeld2, c);
					}
					if (run != null)
					{
						run.LetGo = true;
					}
					if (run == null)
					{
						if (!WraithStage.Ready)
						{
							_old.OnPlay(c);
						}
					}
					else
					{
						Go(run);
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("Wraith Form (played): " + ex.Message);
				}
			}
		}

		private static bool Begin(Run run, CardFxContext c)
		{
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0404: Unknown result type (might be due to invalid IL or missing references)
			//IL_0409: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)c.BackParent) || !FxKit.Alive((GodotObject)(object)c.FrontParent))
			{
				return false;
			}
			if (run.Held == null || !run.Held.Placement(out var lo, out var hi, out var org, out var k, out var who))
			{
				Vector2 view = ((c.View.X > 100f && c.View.Y > 100f) ? c.View : FxKit.ViewSize());
				WraithFormIntroFx.Seen(c.FrontParent, view, out lo, out hi);
				WraithStage.Place(c, lo, hi, out org, out k, out who);
			}
			run.Body = new Vector2(Mathf.Clamp(c.Body.X, 120f, 520f), Mathf.Clamp(c.Body.Y, 160f, 620f));
			run.Into = who;
			run.Feet = c.Feet;
			run.Pace = Mathf.Clamp(Tuning.F("wraith.intro.pace", 1f), 0.4f, 3f);
			run.Born = Time.GetTicksMsec();
			run.Back = MakeRoot(c.BackParent, "SilentFX_WraithPlayBack");
			run.Front = MakeRoot(c.FrontParent, "SilentFX_WraithPlayFront");
			if (run.Back == null || run.Front == null)
			{
				return false;
			}
			run.Play = WraithStage.MakePlay((Node)(object)run.Back, (Node)(object)run.Front, lo, hi, org, k, who);
			if (run.Play == null)
			{
				return false;
			}
			((GodotObject)run.Play).Call(MSetParam, new Variant[2]
			{
				Variant.op_Implicit("shake"),
				Variant.op_Implicit(Mathf.Clamp(Tuning.F("wraith.play.shake", 0.6f), 0f, 1.5f))
			});
			((GodotObject)run.Play).Call(MSetParam, new Variant[2]
			{
				Variant.op_Implicit("dark"),
				Variant.op_Implicit(Mathf.Clamp(Tuning.F("wraith.play.dark", 1f), 0f, 1.4f))
			});
			WraithStage.Tune(run.Play, c, lo, hi);
			((GodotObject)run.Play).Call(MSetParam, new Variant[2]
			{
				Variant.op_Implicit("fire_pad"),
				Variant.op_Implicit(Mathf.Clamp(Tuning.F("wraith.play.fire_pad", 0.02f), 0f, 0.2f))
			});
			((GodotObject)run.Play).Call(MSetParam, new Variant[2]
			{
				Variant.op_Implicit("puff"),
				Variant.op_Implicit(Mathf.Clamp(Tuning.F("wraith.play.puff", 1f), 0f, 2f))
			});
			((GodotObject)run.Play).Call(MSetParam, new Variant[2]
			{
				Variant.op_Implicit("wave"),
				Variant.op_Implicit(WaveFor(c))
			});
			((GodotObject)run.Play).Call(MSetParam, new Variant[2]
			{
				Variant.op_Implicit("wave_push"),
				Variant.op_Implicit(Mathf.Clamp(Tuning.F("wraith.play.wave_push", 0.011f), 0f, 0.05f))
			});
			Variant val = ((GodotObject)run.Play).Call(MTimes, Array.Empty<Variant>());
			float[] array = val.AsFloat32Array();
			if (array != null && array.Length >= 4)
			{
				run.TBurst = array[0];
				run.TGo = array[1];
				run.THit = array[2];
				run.TEnd = array[3];
			}
			Above(run);
			((GodotObject)run.Play).Call(MSetFade, new Variant[1] { Variant.op_Implicit(0f) });
			((GodotObject)run.Play).Call(MSetTime, new Variant[2]
			{
				Variant.op_Implicit(0f),
				Variant.op_Implicit(0f)
			});
			return true;
		}

		private static float WaveFor(CardFxContext c)
		{
			try
			{
				float num = Mathf.Clamp(Tuning.F("wraith.play.wave", 1f), 0f, 3f);
				if (num < 0.01f)
				{
					return 0f;
				}
				CardModel card = c.Card;
				object obj;
				if (card == null)
				{
					obj = null;
				}
				else
				{
					ICombatState combatState = card.CombatState;
					obj = ((combatState != null) ? combatState.Encounter : null);
				}
				EncounterModel val = (EncounterModel)obj;
				if (val != null)
				{
					PropertyInfo property = typeof(EncounterModel).GetProperty("HasCustomBackground", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					bool flag = default;
					int num2;
					if (property != null)
					{
						object value = property.GetValue(val);
						if (value is bool)
						{
							flag = (bool)value;
							num2 = 1;
						}
						else
						{
							num2 = 0;
						}
					}
					else
					{
						num2 = 0;
					}
					if (((uint)num2 & (flag ? 1u : 0u)) != 0)
					{
						return 0f;
					}
				}
				return num;
			}
			catch
			{
				return 0f;
			}
		}

		private static void Above(Run run)
		{
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Invalid comparison between Unknown and I8
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			if (Tuning.F("wraith.play.over_ui", 1f) < 0.5f)
			{
				return;
			}
			Node2D val = null;
			try
			{
				Node overlay = GameRefs.Overlay;
				if (!FxKit.Alive((GodotObject)(object)overlay) || !overlay.IsInsideTree())
				{
					return;
				}
				CanvasItem val2 = (CanvasItem)(object)((overlay is CanvasItem) ? overlay : null);
				if ((val2 != null && !val2.IsVisibleInTree()) || !((GodotObject)run.Play).HasMethod(MSetTop))
				{
					return;
				}
				val = MakeRoot(overlay, "SilentFX_WraithPlayTop");
				if (val == null)
				{
					return;
				}
				if (FxKit.Alive((GodotObject)(object)run.Back))
				{
					val.GlobalTransform = run.Back.GlobalTransform;
				}
				Variant val3 = ((GodotObject)run.Play).Call(MSetTop, new Variant[1] { Variant.op_Implicit((GodotObject)(object)val) });
				if ((long)val3.VariantType == 1 && val3.AsBool())
				{
					run.Top = val;
					return;
				}
				DevLog.Warn("Wraith Form: the stage in front of everything could not be set up - she is drawn as before");
			}
			catch (Exception ex)
			{
				DevLog.Warn("Wraith Form (the stage in front of everything): " + ex.Message);
			}
			if (FxKit.Alive((GodotObject)(object)val))
			{
				if (((Node)val).GetChildCount(false) > 0)
				{
					run.Top = val;
				}
				else
				{
					((Node)val).QueueFree();
				}
			}
		}

		private static Node2D MakeRoot(Node parent, string name)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Expected Obj, but got Unknown
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			Node2D val = new Node2D
			{
				Name = StringName.op_Implicit(name)
			};
			TimeFreeze.KeepLive((Node)(object)val);
			parent.AddChild((Node)(object)val, false, (InternalMode)0);
			if (!((Node)val).IsInsideTree())
			{
				((Node)val).QueueFree();
				return null;
			}
			val.GlobalTransform = Transform2D.Identity;
			FxKit.FreeAfter((Node)(object)val, 180.0);
			return val;
		}

		private static void Go(Run run)
		{
			run.Going = true;
			run.Phase = 1;
			run.Born = Time.GetTicksMsec();
			if (run.Flying && run.FlyLeft > 0f)
			{
				run.T = run.TPre;
				return;
			}
			run.T = ((run.Show < 0.5f) ? Mathf.Max(run.TPre, 0.3f) : Mathf.Max(run.TPre, 0.36f));
			EndFlight(run);
			if (run.Card != null)
			{
				WraithCardFire.Drop(run.Card);
			}
		}

		internal static void CardFlies(NCardFlyPowerVfx vfx)
		{
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)vfx) || Runs.Count == 0)
			{
				return;
			}
			NCard cardNode = vfx.CardNode;
			if (!FxKit.Alive((GodotObject)(object)cardNode) || !(cardNode.Model is WraithForm))
			{
				return;
			}
			Player owner = cardNode.Model.Owner;
			object obj = ((owner != null) ? owner.Creature : null);
			Run run = null;
			int num = Runs.Count - 1;
			while (num >= 0 && run == null)
			{
				Run run2 = Runs[num];
				if (!run2.Done && !run2.Cancelled && !run2.Going && !run2.Flying && run2.LetGo && (run2.Card == cardNode.Model || (run2.Card == null && run2.Owner == obj)))
				{
					run = run2;
				}
				num--;
			}
			if (run == null || !FxKit.Alive((GodotObject)(object)run.Play))
			{
				if (PowerFxManager.LogTriggers)
				{
					DevLog.Info($"Wraith Form: the card flies, and she is not waiting for it ({Runs.Count} in hand, {((run == null) ? "none let go" : "hers is gone")})");
				}
				return;
			}
			string value = "to the player";
			if (Tuning.F("wraith.card.to_her", 1f) >= 0.5f)
			{
				try
				{
					if (!_swooshLooked)
					{
						_swooshLooked = true;
						_swoosh = AccessTools.FieldRefAccess<NCardFlyPowerVfx, Path2D>("_swooshPath");
					}
					Path2D val = ((_swoosh != null) ? _swoosh.Invoke(vfx) : null);
					Curve2D val2 = (FxKit.Alive((GodotObject)(object)val) ? val.Curve : null);
					if (val2 != null && val2.PointCount > 0)
					{
						Variant val3 = ((GodotObject)run.Play).Call(MChest, Array.Empty<Variant>());
						Vector2 val4 = val3.AsVector2();
						Vector2 pointPosition = val2.GetPointPosition(val2.PointCount - 1);
						val2.SetPointPosition(val2.PointCount - 1, val4 - ((Node2D)vfx).GlobalPosition);
						value = $"to her chest {val4} (the game's own end was {pointPosition + ((Node2D)vfx).GlobalPosition})";
					}
					else
					{
						value = "to the player (no path found)";
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("Wraith Form (the card's flight): " + ex.Message);
				}
			}
			float num2 = 0.3f;
			try
			{
				num2 = vfx.GetDuration();
			}
			catch
			{
				num2 = 0.3f;
			}
			run.Flying = true;
			run.FlyLeft = Mathf.Clamp(num2, 0.08f, 1.5f);
			StartFlight(run, cardNode);
			if (PowerFxManager.LogTriggers)
			{
				DevLog.Info($"Wraith Form: the card flies from {((Node2D)vfx).GlobalPosition} {value} for {num2:F2} s" + ((run.Flight != null) ? ", carrying its fire" : ""));
			}
		}

		private static void StartFlight(Run run, NCard card)
		{
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Expected Obj, but got Unknown
			if (Tuning.F("wraith.card.fire", 1f) < 0.5f || !FxKit.Alive((GodotObject)(object)run.Front))
			{
				return;
			}
			try
			{
				AnimatedSprite2D val = WraithCardFire.FireOf(card);
				if (!FxKit.Alive((GodotObject)(object)val) || val.SpriteFrames == null)
				{
					return;
				}
				int frameCount = val.SpriteFrames.GetFrameCount(AnimDefault);
				if (frameCount > 0)
				{
					float num = 0f;
					Texture2D frameTexture = val.SpriteFrames.GetFrameTexture(AnimDefault, 0);
					if (frameTexture != null)
					{
						num = frameTexture.GetHeight();
					}
					Flight flight = new Flight
					{
						Card = card
					};
					flight.Glow = FxKit.Glow(new Color(0.45f, 1f, 0.5f, 0f), new Vector2(260f, 260f));
					((CanvasItem)flight.Glow).Visible = false;
					((Node)run.Front).AddChild((Node)(object)flight.Glow, false, (InternalMode)0);
					for (int i = 0; i < 5; i++)
					{
						AnimatedSprite2D val2 = new AnimatedSprite2D
						{
							SpriteFrames = val.SpriteFrames,
							Material = (Material)(object)WraithCardFire.NewMaterial(1f, 1.25f),
							Offset = new Vector2(0f, -0.5f * num),
							SpeedScale = 1.7f,
							Visible = false
						};
						((Node)run.Front).AddChild((Node)(object)val2, false, (InternalMode)0);
						val2.Play(AnimDefault, 1f, false);
						val2.Frame = i * 3 % frameCount;
						flight.Fire[i] = val2;
					}
					run.Flight = flight;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Wraith Form (the card's fire): " + ex.Message);
				EndFlight(run);
			}
		}

		private static void StepFlight(Run run, float dt)
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			Flight flight = run.Flight;
			if (flight == null)
			{
				return;
			}
			if (!FxKit.Alive((GodotObject)(object)flight.Card) || !((Node)flight.Card).IsInsideTree())
			{
				EndFlight(run);
				return;
			}
			flight.Age += dt;
			Vector2 globalPosition = ((Control)flight.Card).GlobalPosition;
			int num = flight.Trail.Length;
			if (flight.Seen == 0)
			{
				for (int i = 0; i < num; i++)
				{
					flight.Trail[i] = globalPosition;
				}
			}
			else
			{
				for (int num2 = num - 1; num2 > 0; num2--)
				{
					flight.Trail[num2] = flight.Trail[num2 - 1];
				}
			}
			flight.Trail[0] = globalPosition;
			flight.Seen++;
			float num3 = Mathf.Clamp(Tuning.F("wraith.card.fire_size", 1f), 0.3f, 3f);
			float num4 = Mathf.Clamp(flight.Age / 0.06f, 0f, 1f);
			for (int j = 0; j < 5; j++)
			{
				AnimatedSprite2D val = flight.Fire[j];
				if (FxKit.Alive((GodotObject)(object)val))
				{
					Vector2 val2 = flight.Trail[Mathf.Min(j, num - 1)];
					Vector2 val3 = flight.Trail[Mathf.Min(j + 2, num - 1)];
					Vector2 val4 = val2 - val3;
					float num5 = (float)j / 4f;
					((Node2D)val).Position = val2;
					if (val4.LengthSquared() > 4f)
					{
						((Node2D)val).Rotation = Mathf.Atan2(0f - val4.X, val4.Y);
					}
					float num6 = num3 * Mathf.Lerp(1.25f, 0.55f, num5) * num4;
					((Node2D)val).Scale = new Vector2(num6 * 0.9f, num6 * (1.1f + 0.5f * Mathf.Clamp(val4.Length() / 60f, 0f, 1f)));
					((CanvasItem)val).Modulate = new Color(1f, 1f, 1f, Mathf.Lerp(0.95f, 0.25f, num5) * num4);
					if (!((CanvasItem)val).Visible)
					{
						((CanvasItem)val).Visible = true;
					}
				}
			}
			if (FxKit.Alive((GodotObject)(object)flight.Glow))
			{
				((Node2D)flight.Glow).Position = globalPosition;
				((CanvasItem)flight.Glow).Modulate = new Color(0.45f, 1f, 0.5f, 0.55f * num4);
				if (!((CanvasItem)flight.Glow).Visible)
				{
					((CanvasItem)flight.Glow).Visible = true;
				}
			}
		}

		private static void EndFlight(Run run)
		{
			Flight flight = run.Flight;
			run.Flight = null;
			if (flight == null)
			{
				return;
			}
			try
			{
				for (int i = 0; i < 5; i++)
				{
					if (FxKit.Alive((GodotObject)(object)flight.Fire[i]))
					{
						((Node)flight.Fire[i]).QueueFree();
					}
				}
				if (FxKit.Alive((GodotObject)(object)flight.Glow))
				{
					((Node)flight.Glow).QueueFree();
				}
			}
			catch
			{
			}
		}

		private static void Drive(double realDt)
		{
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			if (Runs.Count == 0)
			{
				Ticker.Remove(Driver);
				Ticker.Remove(Follower);
				return;
			}
			float dt = (float)(realDt * (double)Mathf.Clamp((float)Engine.TimeScale, 0f, 4f));
			for (int num = Runs.Count - 1; num >= 0; num--)
			{
				Run run = Runs[num];
				try
				{
					if (run.Done || !FxKit.Alive((GodotObject)(object)run.Back) || !FxKit.Alive((GodotObject)(object)run.Front) || !FxKit.Alive((GodotObject)(object)run.Play) || !((Node)run.Front).IsInsideTree() || (run.Top != null && (!FxKit.Alive((GodotObject)(object)run.Top) || !((Node)run.Top).IsInsideTree())))
					{
						Free(run);
						Runs.RemoveAt(num);
					}
					else
					{
						if (run.Top != null)
						{
							run.Top.GlobalTransform = run.Back.GlobalTransform;
							run.TopShow = Mathf.MoveToward(run.TopShow, WraithStage.FightOver() ? 0f : 1f, (float)realDt / 0.25f);
							((CanvasItem)run.Top).Modulate = new Color(1f, 1f, 1f, run.TopShow);
						}
						Step(run, dt);
						if (run.Done)
						{
							Free(run);
							Runs.RemoveAt(num);
						}
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("Wraith Form (playing): " + ex.Message);
					Free(run);
					Runs.Remove(run);
				}
			}
		}

		private static void Follow(double realDt)
		{
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < Runs.Count; i++)
			{
				Run run = Runs[i];
				try
				{
					if (run.Top != null && FxKit.Alive((GodotObject)(object)run.Top) && ((Node)run.Top).IsInsideTree() && FxKit.Alive((GodotObject)(object)run.Back) && ((Node)run.Back).IsInsideTree())
					{
						run.Top.GlobalTransform = run.Back.GlobalTransform;
					}
				}
				catch
				{
				}
			}
		}

		private static void Step(Run run, float dt)
		{
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			run.Clock += dt;
			if (run.Cancelled)
			{
				run.Show = Mathf.MoveToward(run.Show, 0f, dt / 0.25f);
				if (run.Show <= 0f)
				{
					run.Done = true;
				}
				((GodotObject)run.Play).Call(MSetTime, new Variant[2]
				{
					Variant.op_Implicit(run.TPre),
					Variant.op_Implicit(run.Clock)
				});
			}
			else
			{
				if (!run.LetGo)
				{
					return;
				}
				if (!run.Going)
				{
					run.Wait += dt;
					bool flag = run.Held == null || run.Held.Finished || run.Held.FormedForPlay;
					if (flag)
					{
						if (!run.Phased)
						{
							run.Phased = true;
							if (run.Held != null && !run.Held.Finished)
							{
								((GodotObject)run.Play).Call(MSetParam, new Variant[2]
								{
									Variant.op_Implicit("ph"),
									Variant.op_Implicit(run.Held.SwayPhase())
								});
							}
						}
						run.Show = Mathf.MoveToward(run.Show, 1f, dt / 0.18f);
						if (run.Flying)
						{
							float num = Mathf.Clamp((run.TBurst - run.TPre) / Mathf.Max(run.FlyLeft, 0.05f), 1f, 3f);
							run.TPre = Mathf.Min(run.TBurst, run.TPre + dt * num);
						}
						else
						{
							run.TPre = Mathf.Min(0.36f, run.TPre + dt);
						}
					}
					if (run.Flying)
					{
						run.FlyLeft -= dt;
					}
					if (flag && run.LetGo)
					{
						Hush(0.5f);
					}
					if (run.Wait > 9f)
					{
						run.Cancelled = true;
					}
					((GodotObject)run.Play).Call(MSetTime, new Variant[2]
					{
						Variant.op_Implicit(run.TPre),
						Variant.op_Implicit(run.Clock)
					});
					if (run.Card != null)
					{
						WraithCardFire.Set(run.Card, 1f, 3.2f);
					}
					StepFlight(run, dt);
				}
				else
				{
					run.Show = Mathf.MoveToward(run.Show, 1f, dt / 0.12f);
					if (run.Flying && run.FlyLeft > 0f && run.T < run.TBurst)
					{
						float num2 = Mathf.Clamp((run.TBurst - run.T) / Mathf.Max(run.FlyLeft, 0.05f), 1f, 3f);
						run.T = Mathf.Min(run.TBurst - 0.002f, run.T + dt * num2);
						run.FlyLeft -= dt;
						if (run.Card != null)
						{
							WraithCardFire.Set(run.Card, 1f, 3.2f);
						}
						StepFlight(run, dt);
					}
					else
					{
						if (run.Flight != null)
						{
							EndFlight(run);
							if (run.Card != null)
							{
								WraithCardFire.Drop(run.Card);
							}
						}
						run.T += dt / run.Pace;
					}
					long timestamp = Stopwatch.GetTimestamp();
					((GodotObject)run.Play).Call(MSetTime, new Variant[2]
					{
						Variant.op_Implicit(run.T),
						Variant.op_Implicit(run.Clock)
					});
					double totalSeconds = Stopwatch.GetElapsedTime(timestamp).TotalSeconds;
					run.ScriptTime += totalSeconds;
					run.ScriptFrames++;
					if (totalSeconds > run.ScriptWorst)
					{
						run.ScriptWorst = totalSeconds;
					}
					if (run.T < run.THit + 0.15f)
					{
						Hush(0.3f);
					}
					if (!run.SaidKnock && run.T >= run.TBurst - Sfx.Lead("wraith_knock") / run.Pace)
					{
						run.SaidKnock = true;
						Sfx.Play("wraith_knock");
					}
					if (!run.SaidScream && run.T >= run.TGo - Sfx.Lead("wraith_scream") / run.Pace)
					{
						run.SaidScream = true;
						float num3 = Mathf.Clamp(Tuning.F("wraith.intro.sound", 1f), 0f, 2f);
						if (num3 > 0.01f)
						{
							Sfx.Play("wraith_scream", num3);
						}
					}
					if (!run.SaidLand && run.T >= run.THit - Sfx.Lead("wraith_land") / run.Pace)
					{
						run.SaidLand = true;
						Sfx.Play("wraith_land");
					}
					if (!run.Knocked && run.T >= run.TBurst)
					{
						Knock(run);
					}
					if (!run.Screamed && run.T >= run.TGo)
					{
						Scream(run);
					}
					if (!run.Landed && run.T >= run.THit)
					{
						Land(run);
					}
					run.Phase = ((run.T < run.TGo) ? 1 : (run.Landed ? 3 : 2));
					if (run.T >= run.TEnd)
					{
						run.Done = true;
					}
				}
			}
			((GodotObject)run.Play).Call(MSetFade, new Variant[1] { Variant.op_Implicit(run.Show) });
		}

		private static void Free(Run run)
		{
			if (run == null)
			{
				return;
			}
			run.Done = true;
			run.Phase = 0;
			try
			{
				if (run.ScriptFrames > 0 && PowerFxManager.LogTriggers)
				{
					DevLog.Info($"Wraith Form: played - her script took {run.ScriptTime / (double)run.ScriptFrames * 1000.0:F2} ms a frame (worst {run.ScriptWorst * 1000.0:F2}) over {run.ScriptFrames} frames");
					run.ScriptFrames = 0;
				}
				EndFlight(run);
				if (FxKit.Alive((GodotObject)(object)run.Back))
				{
					((Node)run.Back).QueueFree();
				}
				if (FxKit.Alive((GodotObject)(object)run.Front))
				{
					((Node)run.Front).QueueFree();
				}
				if (FxKit.Alive((GodotObject)(object)run.Top))
				{
					((Node)run.Top).QueueFree();
				}
			}
			catch
			{
			}
			run.Back = null;
			run.Front = null;
			run.Top = null;
			run.Play = null;
			run.Held = null;
		}

		private static void Hush(float attack)
		{
			Mood.Hold("wraith", Sfx.Knob("mood.wraith.volume", 0.45f), 1f, Sfx.Knob("mood.wraith.far", 0.35f), Sfx.Knob("mood.wraith.hollow", 0.45f), attack, Mathf.Max(0.05f, Sfx.Knob("mood.wraith.back", 0.9f)), 0.3f);
		}

		private static void Knock(Run run)
		{
			run.Knocked = true;
			int num = Tuning.I("wraith.play.knock", 1);
			if (num > 0)
			{
				GameRefs.Shake(num);
			}
		}

		private static void Scream(Run run)
		{
			run.Screamed = true;
			int num = Tuning.I("wraith.intro.shake", 3);
			if (num > 0)
			{
				GameRefs.Shake(num);
			}
		}

		private static void Land(Run run)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			run.Landed = true;
			if (!(Tuning.F("wraith.intro.pour", 1f) < 0.5f) && FxKit.Alive((GodotObject)(object)run.Front))
			{
				Vector2 val = run.Into;
				float pace = run.Pace;
				FxKit.Flash((Node)(object)run.Front, val, new Color(0.6f, 1f, 0.62f, 0.85f), run.Body.Y * 0.95f, 1.9f, 0.26 * (double)pace);
				FxKit.RingPulse((Node)(object)run.Front, val, run.Body.Y * 1.5f, new Color(0.62f, 1f, 0.6f, 1f), 0f, 0.36f * pace, 0.25f, 1.25f, 1f, 0f, 0.05f, 0.65f);
				FxKit.Burst((Node)(object)run.Front, val, new Color(0.62f, 1f, 0.66f, 0.45f), 18, 30f, 150f, 0.42 * (double)pace, 0.25f, 0.6f, new Vector2(0f, -160f), 180f, null, run.Body.X * 0.28f, 80f);
				Vector2 globalPos = new Vector2(val.X, Mathf.Lerp(val.Y, run.Feet.Y, 0.8f));
				FxKit.Burst((Node)(object)run.Front, globalPos, new Color(0.3f, 0.86f, 0.46f, 0.3f), 22, 260f, 760f, 0.85 * (double)pace, 1f, 2.6f, new Vector2(0f, -30f), 14f, new Vector2(1f, 0f), run.Body.X * 0.2f, 900f, additive: true, 0.9f);
				FxKit.Burst((Node)(object)run.Front, globalPos, new Color(0.3f, 0.86f, 0.46f, 0.3f), 22, 260f, 760f, 0.85 * (double)pace, 1f, 2.6f, new Vector2(0f, -30f), 14f, new Vector2(-1f, 0f), run.Body.X * 0.2f, 900f, additive: true, 0.9f);
				int num = Tuning.I("wraith.intro.shake3", 0);
				if (num > 0)
				{
					GameRefs.Shake(num);
				}
			}
		}
	}
