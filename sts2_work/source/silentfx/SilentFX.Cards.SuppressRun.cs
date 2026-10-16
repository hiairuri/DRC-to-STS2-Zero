// namespace: SilentFX.Cards
// type: SuppressRun (class)
// source lines: 109345-110609
	internal sealed class SuppressRun
	{
		private static readonly List<SuppressRun> Live = new List<SuppressRun>();

		private static readonly Action<double> Ticker1 = Update;

		private static readonly Action<double> Late = LateUpdate;

		private static bool _ticking;

		private static FieldInfo _arrowField;

		private static bool _arrowTried;

		private static int _failures;

		private static readonly StringName MStep = StringName.op_Implicit("step");

		private static readonly StringName MLift = StringName.op_Implicit("lift");

		private static readonly StringName MAim = StringName.op_Implicit("aim");

		private static readonly StringName MUnaim = StringName.op_Implicit("unaim");

		private static readonly StringName MRelease = StringName.op_Implicit("release");

		private static readonly StringName MAuto = StringName.op_Implicit("play_auto");

		private static readonly StringName MHit = StringName.op_Implicit("hit");

		private static readonly StringName MCancel = StringName.op_Implicit("cancel");

		private static readonly StringName MDismiss = StringName.op_Implicit("dismiss");

		private static readonly StringName MSetTop = StringName.op_Implicit("set_top");

		private static readonly StringName MSetSoul = StringName.op_Implicit("set_soul");

		private Node2D _back;

		private Node2D _front;

		private Node2D _top;

		private Node _stage;

		private CardModel _card;

		private Creature _target;

		private SoulCopy _soul;

		private bool _lifted;

		private bool _aimed;

		private bool _sent;

		private bool _hidArrow;

		private bool _released;

		private bool _cancelled;

		private bool _dismissed;

		private bool _played;

		private bool _done;

		private float _presence = 1f;

		private double _age;

		private double _sinceDismiss;

		private bool _hideCard;

		private float _cardAlpha = 1f;

		private bool _touchedCard;

		private NCard _cardNode;

		private Node2D _yankBody;

		private Vector2 _yankApplied;

		private Vector2 _yankWrote;

		private Node2D _dimBody;

		private Color _dimWas;

		private Color _dimWrote;

		private bool _dimWritten;

		private bool _dimGaveUp;

		private bool _remote;

		private object _remoteRoom;

		public CardModel Card => _card;

		public bool Dead => _done;

		public bool Released => _released;

		private bool RemoteInvalid
		{
			get
			{
				if (_remote)
				{
					if (MultiplayerFx.Active)
					{
						return _remoteRoom != GameRefs.CombatRoom;
					}
					return true;
				}
				return false;
			}
		}

		public static SuppressRun Create(CardFxContext c)
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Expected Obj, but got Unknown
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Expected Obj, but got Unknown
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			if (c == null || !SuppressStage.Ready)
			{
				return null;
			}
			SuppressRun suppressRun = new SuppressRun
			{
				_card = c.Card,
				_remote = c.Remote,
				_remoteRoom = (c.Remote ? GameRefs.CombatRoom : null)
			};
			try
			{
				suppressRun._back = new Node2D
				{
					Name = StringName.op_Implicit("SilentFX_SuppressBack")
				};
				suppressRun._front = new Node2D
				{
					Name = StringName.op_Implicit("SilentFX_SuppressFront")
				};
				TimeFreeze.KeepLive((Node)(object)suppressRun._back);
				TimeFreeze.KeepLive((Node)(object)suppressRun._front);
				c.BackParent.AddChild((Node)(object)suppressRun._back, false, (InternalMode)0);
				c.FrontParent.AddChild((Node)(object)suppressRun._front, false, (InternalMode)0);
				suppressRun._back.GlobalTransform = Transform2D.Identity;
				suppressRun._front.GlobalTransform = Transform2D.Identity;
				FxKit.FreeAfter((Node)(object)suppressRun._back, 300.0);
				FxKit.FreeAfter((Node)(object)suppressRun._front, 300.0);
				Vector2 view = ((c.View.X > 100f && c.View.Y > 100f) ? c.View : FxKit.ViewSize());
				WraithFormIntroFx.Seen(c.BackParent, view, out var lo, out var hi);
				suppressRun._stage = SuppressStage.Make(suppressRun._back, suppressRun._front, lo, hi, suppressRun._remote);
				if (suppressRun._stage == null)
				{
					suppressRun.Free();
					return null;
				}
				suppressRun.MakeTop();
				suppressRun.FilterStandbyScreen();
			}
			catch (Exception ex)
			{
				DevLog.Warn("SuppressRun.Create: " + ex);
				suppressRun.Free();
				return null;
			}
			Live.Add(suppressRun);
			if (!_ticking)
			{
				_ticking = true;
				Ticker.Add(Ticker1);
				Ticker.AddLate(Late);
			}
			return suppressRun;
		}

		private void MakeTop()
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Expected Obj, but got Unknown
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Invalid comparison between Unknown and I8
			if ((_remote && !_played) || Tuning.F("suppress.v3.over_ui", 1f) < 0.5f)
			{
				return;
			}
			try
			{
				Node overlay = GameRefs.Overlay;
				if (!FxKit.Alive((GodotObject)(object)overlay) || !overlay.IsInsideTree())
				{
					return;
				}
				Node2D val = new Node2D
				{
					Name = StringName.op_Implicit("SilentFX_SuppressTop")
				};
				TimeFreeze.KeepLive((Node)(object)val);
				overlay.AddChild((Node)(object)val, false, (InternalMode)0);
				if (!((Node)val).IsInsideTree())
				{
					((Node)val).QueueFree();
					return;
				}
				FxKit.FreeAfter((Node)(object)val, 300.0);
				_top = val;
				FollowTop();
				Variant val2 = ((GodotObject)_stage).Call(MSetTop, new Variant[1] { Variant.op_Implicit((GodotObject)(object)val) });
				if ((long)val2.VariantType != 1 || !val2.AsBool())
				{
					((Node)val).QueueFree();
					_top = null;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Suppress (layer over the interface): " + ex.Message);
			}
		}

		private void FollowTop()
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)_top) && ((Node)_top).IsInsideTree() && FxKit.Alive((GodotObject)(object)_back) && ((Node)_back).IsInsideTree())
			{
				Node2D top = _top;
				Transform2D canvasTransform = ((CanvasItem)_top).GetCanvasTransform();
				top.GlobalTransform = canvasTransform.AffineInverse() * ((CanvasItem)_back).GetCanvasTransform();
			}
		}

		public void SetPresence(float presence)
		{
			_presence = Mathf.Clamp(presence, 0f, 1f);
		}

		public void Lift()
		{
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			if (_lifted || _done)
			{
				return;
			}
			_lifted = true;
			_hideCard = !_remote;
			foreach (SuppressRun item in Live)
			{
				if (item != this && item._card == _card)
				{
					item.LetGoOfCard();
				}
			}
			Call(MLift, Variant.op_Implicit(Mouse()));
		}

		public void Aim(CardFxContext c)
		{
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			if (!_done && c != null && c.HasTarget)
			{
				if (_target != c.Target)
				{
					_soul?.Dispose();
					_soul = null;
					_target = c.Target;
					_soul = SoulCopy.Make(_target, (Node)(object)_back);
					PassSoul();
				}
				_aimed = true;
				Call(MAim, Variant.op_Implicit(c.TargetFeet), Variant.op_Implicit(c.TargetChest), Variant.op_Implicit(c.TargetWidth), Variant.op_Implicit(c.TargetHeight));
			}
		}

		public void Unaim()
		{
			if (!_done)
			{
				_aimed = false;
				Call(MUnaim);
			}
		}

		public void Release()
		{
			if (!_done && !_released)
			{
				_released = true;
				if ((!_remote || _played) && _aimed)
				{
					_sent = true;
					Call(MRelease);
				}
			}
		}

		public void Played(CardFxContext c, bool fromStandby)
		{
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			if (_done)
			{
				return;
			}
			_played = true;
			if (_remote && !FxKit.Alive((GodotObject)(object)_top))
			{
				MakeTop();
			}
			if (fromStandby && _sent)
			{
				return;
			}
			if (c == null || !c.HasTarget)
			{
				Dismiss();
				return;
			}
			_released = true;
			_sent = true;
			if (_target != c.Target)
			{
				_soul?.Dispose();
				_target = c.Target;
				_soul = SoulCopy.Make(_target, (Node)(object)_back);
				PassSoul();
			}
			Call(MAuto, Variant.op_Implicit(c.TargetFeet), Variant.op_Implicit(c.TargetChest), Variant.op_Implicit(c.TargetWidth), Variant.op_Implicit(c.TargetHeight));
		}

		public void Hit(bool killed)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			if (!_done)
			{
				Call(MHit, Variant.op_Implicit(killed));
			}
		}

		public void Cancel()
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			if (!_done && !_released && !_cancelled)
			{
				_cancelled = true;
				if (_remote)
				{
					Dismiss();
					return;
				}
				Call(MCancel, Variant.op_Implicit(Mouse()));
			}
		}

		public void CardFinished()
		{
			_hideCard = false;
		}

		public void Dismiss()
		{
			if (!_done && !_dismissed)
			{
				_dismissed = true;
				_hideCard = false;
				Call(MDismiss);
			}
		}

		public static SuppressRun For(CardModel card)
		{
			if (card == null)
			{
				return null;
			}
			for (int num = Live.Count - 1; num >= 0; num--)
			{
				if (!Live[num]._done && Live[num]._card == card && Live[num]._released)
				{
					return Live[num];
				}
			}
			return null;
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
				SuppressRun suppressRun = Live[num];
				try
				{
					suppressRun.Step(dt, (float)realDt);
				}
				catch (Exception ex)
				{
					DevLog.Error("Suppress failed and was removed: " + ex);
					if (++_failures >= 3)
					{
						SuppressStage.Fail("it failed three times");
					}
					suppressRun.Free();
				}
				if (suppressRun._done)
				{
					Live.Remove(suppressRun);
				}
			}
			if (Live.Count == 0 && _ticking)
			{
				_ticking = false;
				Ticker.Remove(Ticker1);
				Ticker.Remove(Late);
			}
		}

		private static void LateUpdate(double realDt)
		{
			foreach (SuppressRun item in Live)
			{
				try
				{
					if (item.RemoteInvalid)
					{
						item.Free();
					}
					else
					{
						item.FollowTop();
					}
				}
				catch
				{
				}
			}
		}

		private void Step(float dt, float realDt)
		{
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Invalid comparison between Unknown and I8
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			if (_done)
			{
				return;
			}
			if (RemoteInvalid)
			{
				Free();
				return;
			}
			if (!FxKit.Alive((GodotObject)(object)_back) || !FxKit.Alive((GodotObject)(object)_front) || !FxKit.Alive((GodotObject)(object)_stage) || !((Node)_back).IsInsideTree())
			{
				Free();
				return;
			}
			_age += dt;
			if (_dismissed)
			{
				_sinceDismiss += dt;
			}
			if (_soul != null)
			{
				_soul.Follow();
				PassSoul();
			}
			Variant val = ((GodotObject)_stage).Call(MStep, new Variant[4]
			{
				Variant.op_Implicit(dt),
				Variant.op_Implicit(Mouse()),
				Variant.op_Implicit(_presence),
				Variant.op_Implicit(PlayerSettings.ReduceScreenEffects)
			});
			FilterStandbyScreen();
			if ((long)val.VariantType != 27)
			{
				DevLog.Warn("Suppress: step() did not answer - this showing is removed");
				if (++_failures >= 3)
				{
					SuppressStage.Fail("step() did not answer three times");
				}
				Free();
				return;
			}
			Dictionary val2 = val.AsGodotDictionary();
			Variant val3;
			int num;
			if (val2.ContainsKey(Variant.op_Implicit("done")))
			{
				val3 = val2[Variant.op_Implicit("done")];
				num = (val3.AsBool() ? 1 : 0);
			}
			else
			{
				num = 0;
			}
			Vector2 val4;
			if (!val2.ContainsKey(Variant.op_Implicit("yank")))
			{
				val4 = Vector2.Zero;
			}
			else
			{
				val3 = val2[Variant.op_Implicit("yank")];
				val4 = val3.AsVector2();
			}
			Vector2 val5 = val4;
			int num2;
			if (!val2.ContainsKey(Variant.op_Implicit("shake")))
			{
				num2 = 0;
			}
			else
			{
				val3 = val2[Variant.op_Implicit("shake")];
				num2 = val3.AsInt32();
			}
			int num3 = num2;
			float num4;
			if (!val2.ContainsKey(Variant.op_Implicit("hold")))
			{
				num4 = 0f;
			}
			else
			{
				val3 = val2[Variant.op_Implicit("hold")];
				num4 = val3.AsSingle();
			}
			float num5 = num4;
			int num6;
			if (val2.ContainsKey(Variant.op_Implicit("card_show")))
			{
				val3 = val2[Variant.op_Implicit("card_show")];
				num6 = (val3.AsBool() ? 1 : 0);
			}
			else
			{
				num6 = 0;
			}
			bool flag = (byte)num6 != 0;
			float num7;
			if (!val2.ContainsKey(Variant.op_Implicit("dim")))
			{
				num7 = 0f;
			}
			else
			{
				val3 = val2[Variant.op_Implicit("dim")];
				num7 = val3.AsSingle();
			}
			float amount = num7;
			if (val2.ContainsKey(Variant.op_Implicit("cues")))
			{
				val3 = val2[Variant.op_Implicit("cues")];
				Hear(val3.AsGodotArray());
			}
			if (num3 > 0)
			{
				GameRefs.Shake(num3);
			}
			if (num5 > 0f)
			{
				_soul?.Freeze();
				if (_target != null && Tuning.F("suppress.v3.hold", 1f) >= 0.5f)
				{
					CreatureHold.Hold(_target, num5);
				}
			}
			Yank(val5);
			Dim(amount);
			if (_cancelled & flag)
			{
				_hideCard = false;
			}
			if (_top != null && !FxKit.Alive((GodotObject)(object)_top))
			{
				_top = null;
			}
			if (_lifted && !_released && !_cancelled && !_dismissed)
			{
				HideArrow();
			}
			CardPicture(realDt);
			if (num != 0 || _age > 240.0 || (_dismissed && _sinceDismiss > 1.5))
			{
				Free();
			}
		}

		private void Hear(Array cues)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			if ((_remote && !_played) || cues == null)
			{
				return;
			}
			foreach (Variant cue in cues)
			{
				Variant current = cue;
				switch (current.AsString())
				{
				case "lift":
					if (!_remote)
					{
						Sfx.Play("suppress_mist");
					}
					break;
				case "bite":
					Sfx.PlayAt("suppress_bite", 0.08f);
					break;
				case "tear":
					Sfx.Play("suppress_tear");
					break;
				case "burst":
					Sfx.Play("suppress_burst");
					break;
				}
			}
		}

		private Vector2 Mouse()
		{
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			if (_remote)
			{
				if (!FxKit.Alive((GodotObject)(object)_stage))
				{
					return Vector2.Zero;
				}
				Variant val = ((GodotObject)_stage).Call(StringName.op_Implicit("hand_pos"), new Variant[2]
				{
					Variant.op_Implicit("k2"),
					Variant.op_Implicit("front")
				});
				return val.AsVector2();
			}
			try
			{
				if (FxKit.Alive((GodotObject)(object)_back) && ((Node)_back).IsInsideTree())
				{
					return ((CanvasItem)_back).GetGlobalMousePosition();
				}
			}
			catch
			{
			}
			return Vector2.Zero;
		}

		private void FilterStandbyScreen()
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			if (_remote && !_played && FxKit.Alive((GodotObject)(object)_stage))
			{
				Variant val = ((GodotObject)_stage).Get(StringName.op_Implicit("fogb"));
				GodotObject val2 = val.AsGodotObject();
				CanvasItem val3 = (CanvasItem)(object)((val2 is CanvasItem) ? val2 : null);
				if (val3 != null && FxKit.Alive((GodotObject)(object)val3))
				{
					val3.Visible = false;
				}
				val = ((GodotObject)_stage).Get(StringName.op_Implicit("veil"));
				GodotObject val4 = val.AsGodotObject();
				CanvasItem val5 = (CanvasItem)(object)((val4 is CanvasItem) ? val4 : null);
				if (val5 != null && FxKit.Alive((GodotObject)(object)val5))
				{
					val5.Visible = false;
				}
			}
		}

		private void Call(StringName method, params Variant[] args)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			if (_done || !FxKit.Alive((GodotObject)(object)_stage))
			{
				return;
			}
			try
			{
				((GodotObject)_stage).Call(method, args);
			}
			catch (Exception ex)
			{
				DevLog.Warn("Suppress (" + StringName.op_Implicit(method) + "): " + ex.Message);
			}
		}

		private void PassSoul()
		{
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)_stage))
			{
				return;
			}
			try
			{
				if (_soul != null && _soul.Ok)
				{
					((GodotObject)_stage).Call(MSetSoul, new Variant[3]
					{
						Variant.op_Implicit((GodotObject)(object)_soul.Texture),
						Variant.op_Implicit(_soul.TPos),
						Variant.op_Implicit(_soul.TSize)
					});
				}
				else
				{
					((GodotObject)_stage).Call(MSetSoul, new Variant[3]
					{
						default,
						Variant.op_Implicit(Vector2.Zero),
						Variant.op_Implicit(Vector2.One)
					});
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Suppress (soul): " + ex.Message);
			}
		}

		private void Yank(Vector2 by)
		{
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			if (_target == null || Tuning.F("suppress.v3.yank", 1f) < 0.5f)
			{
				return;
			}
			try
			{
				if (_yankBody == null && by.LengthSquared() < 0.01f)
				{
					return;
				}
				if (_yankBody == null)
				{
					NCombatRoom combatRoom = GameRefs.CombatRoom;
					NCreature val = ((combatRoom != null) ? combatRoom.GetCreatureNode(_target) : null);
					if (!FxKit.Alive((GodotObject)(object)val) || !FxKit.Alive((GodotObject)(object)val.Body))
					{
						return;
					}
					_yankBody = val.Body;
					_yankApplied = Vector2.Zero;
					foreach (SuppressRun item in Live)
					{
						if (item != this && (_remote || item._remote) && item._yankBody == _yankBody && !(_yankBody.Position != item._yankWrote))
						{
							_yankApplied = item._yankApplied;
							_yankWrote = item._yankWrote;
							item._yankBody = null;
							item._yankApplied = Vector2.Zero;
						}
					}
				}
				if (!FxKit.Alive((GodotObject)(object)_yankBody))
				{
					_yankBody = null;
					return;
				}
				Vector2 val2 = by;
				Node parent = ((Node)_yankBody).GetParent();
				Node2D val3 = (Node2D)(object)((parent is Node2D) ? parent : null);
				if (val3 != null)
				{
					Transform2D val4 = val3.GlobalTransform;
					val4 = val4.AffineInverse();
					val2 = val4.BasisXform(by);
				}
				if (_yankApplied != Vector2.Zero && _yankBody.Position != _yankWrote)
				{
					_yankApplied = Vector2.Zero;
					_yankBody = null;
					return;
				}
				Node2D yankBody = _yankBody;
				yankBody.Position += val2 - _yankApplied;
				_yankApplied = val2;
				_yankWrote = _yankBody.Position;
				if (by.LengthSquared() < 0.01f)
				{
					Node2D yankBody2 = _yankBody;
					yankBody2.Position -= _yankApplied;
					_yankApplied = Vector2.Zero;
					_yankBody = null;
				}
			}
			catch
			{
				_yankBody = null;
			}
		}

		private void Dim(float amount)
		{
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			if (_target == null || _dimGaveUp || Tuning.F("suppress.v3.dim", 1f) < 0.5f)
			{
				return;
			}
			try
			{
				if (_dimBody == null)
				{
					if (amount < 0.003f)
					{
						return;
					}
					NCombatRoom combatRoom = GameRefs.CombatRoom;
					NCreature val = ((combatRoom != null) ? combatRoom.GetCreatureNode(_target) : null);
					if (!FxKit.Alive((GodotObject)(object)val) || !FxKit.Alive((GodotObject)(object)val.Body))
					{
						return;
					}
					_dimBody = val.Body;
					_dimWas = ((CanvasItem)_dimBody).Modulate;
					_dimWritten = false;
					foreach (SuppressRun item in Live)
					{
						if (item != this && item._dimBody == _dimBody)
						{
							_dimWas = item._dimWas;
							item._dimBody = null;
							item._dimGaveUp = true;
						}
					}
				}
				if (!FxKit.Alive((GodotObject)(object)_dimBody))
				{
					_dimBody = null;
					return;
				}
				if (_dimWritten && ((CanvasItem)_dimBody).Modulate != _dimWrote)
				{
					_dimBody = null;
					_dimGaveUp = true;
					return;
				}
				if (amount < 0.003f)
				{
					((CanvasItem)_dimBody).Modulate = _dimWas;
					_dimBody = null;
					return;
				}
				Color val2 = new Color(_dimWas.R * (1f - 0.5f * amount), _dimWas.G * (1f - 0.42f * amount), _dimWas.B * (1f - 0.4f * amount), _dimWas.A);
				((CanvasItem)_dimBody).Modulate = val2;
				_dimWrote = val2;
				_dimWritten = true;
			}
			catch
			{
				try
				{
					if (FxKit.Alive((GodotObject)(object)_dimBody) && _dimWritten)
					{
						((CanvasItem)_dimBody).Modulate = _dimWas;
					}
				}
				catch
				{
				}
				_dimBody = null;
				_dimGaveUp = true;
			}
		}

		private void Undim()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (FxKit.Alive((GodotObject)(object)_dimBody) && _dimWritten && ((CanvasItem)_dimBody).Modulate == _dimWrote)
				{
					((CanvasItem)_dimBody).Modulate = _dimWas;
				}
			}
			catch
			{
			}
			_dimBody = null;
		}

		private void Unyank()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (FxKit.Alive((GodotObject)(object)_yankBody) && _yankApplied != Vector2.Zero && _yankBody.Position == _yankWrote)
				{
					Node2D yankBody = _yankBody;
					yankBody.Position -= _yankApplied;
				}
			}
			catch
			{
			}
			_yankBody = null;
			_yankApplied = Vector2.Zero;
		}

		private void HideArrow()
		{
			if (_remote || Tuning.F("suppress.v3.hide_arrow", 1f) < 0.5f)
			{
				return;
			}
			try
			{
				NTargetManager instance = NTargetManager.Instance;
				if (!FxKit.Alive((GodotObject)(object)instance))
				{
					return;
				}
				if (!_arrowTried)
				{
					_arrowTried = true;
					_arrowField = typeof(NTargetManager).GetField("_targetingArrow", BindingFlags.Instance | BindingFlags.NonPublic);
					if (_arrowField == null)
					{
						DevLog.Warn("Suppress: the targeting arrow was not found (it stays)");
					}
				}
				object? obj = _arrowField?.GetValue(instance);
				CanvasItem val = (CanvasItem)((obj is CanvasItem) ? obj : null);
				if (val != null && FxKit.Alive((GodotObject)(object)val) && val.Visible)
				{
					val.Visible = false;
					_hidArrow = true;
				}
			}
			catch
			{
			}
		}

		private void CardPicture(float realDt)
		{
			if (_remote || _card == null || (!_hideCard && !_touchedCard))
			{
				return;
			}
			try
			{
				NCard val = HandCards.NodeOf(_card);
				if (!FxKit.Alive((GodotObject)(object)val))
				{
					val = ((FxKit.Alive((GodotObject)(object)_cardNode) && _cardNode.Model == _card) ? _cardNode : null);
				}
				if (FxKit.Alive((GodotObject)(object)_cardNode) && _cardNode != val)
				{
					CardStandIn.Show(_cardNode);
					_cardNode = null;
				}
				if (!FxKit.Alive((GodotObject)(object)val))
				{
					return;
				}
				_cardNode = val;
				if (_hideCard)
				{
					_cardAlpha = 0f;
					_touchedCard = true;
					CardStandIn.Hide(val);
					return;
				}
				_cardAlpha = Mathf.Min(1f, _cardAlpha + realDt / 0.15f);
				if (_cardAlpha >= 1f)
				{
					CardStandIn.Show(val);
					_touchedCard = false;
					_cardNode = null;
				}
				else
				{
					CardStandIn.Dim(val, _cardAlpha);
				}
			}
			catch
			{
			}
		}

		private void LetGoOfCard()
		{
			_hideCard = false;
			_touchedCard = false;
			_cardNode = null;
		}

		private void ShowCardNow()
		{
			if (_remote || !_touchedCard)
			{
				return;
			}
			try
			{
				NCard val = ((_card != null) ? HandCards.NodeOf(_card) : null);
				if (FxKit.Alive((GodotObject)(object)val))
				{
					CardStandIn.Show(val);
				}
				if (FxKit.Alive((GodotObject)(object)_cardNode) && _cardNode != val)
				{
					CardStandIn.Show(_cardNode);
				}
			}
			catch
			{
			}
			_touchedCard = false;
			_cardNode = null;
		}

		private void Free()
		{
			if (_done)
			{
				return;
			}
			_done = true;
			Unyank();
			Undim();
			ShowCardNow();
			if (_hidArrow && !_released && !_cancelled)
			{
				try
				{
					if (_card != null && HeldFxManager.Carried == _card)
					{
						object? obj = _arrowField?.GetValue(NTargetManager.Instance);
						CanvasItem val = (CanvasItem)((obj is CanvasItem) ? obj : null);
						if (val != null && FxKit.Alive((GodotObject)(object)val))
						{
							val.Visible = true;
						}
					}
				}
				catch
				{
				}
			}
			_soul?.Dispose();
			_soul = null;
			if (FxKit.Alive((GodotObject)(object)_top))
			{
				((Node)_top).QueueFree();
			}
			if (FxKit.Alive((GodotObject)(object)_back))
			{
				((Node)_back).QueueFree();
			}
			if (FxKit.Alive((GodotObject)(object)_front))
			{
				((Node)_front).QueueFree();
			}
			_top = null;
			_stage = null;
		}

		public static void Reset()
		{
			SuppressRun[] array = Live.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Free();
			}
			SoulCopy.Reset();
			Live.Clear();
			if (_ticking)
			{
				_ticking = false;
				Ticker.Remove(Ticker1);
				Ticker.Remove(Late);
			}
		}
	}
