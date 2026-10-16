// namespace: SilentFX.Cards
// type: WraithHeld2 (class)
// source lines: 117633-118094
	public sealed class WraithHeld2 : HeldSession
	{
		private static readonly StringName MSetState = StringName.op_Implicit("set_state");

		private static readonly StringName MSetParam = StringName.op_Implicit("set_param");

		private static readonly StringName MTimes = StringName.op_Implicit("times");

		private static readonly StringName MPhase = StringName.op_Implicit("phase");

		private static readonly StringName MSetTop = StringName.op_Implicit("set_top");

		private static readonly List<WraithHeld2> Tops = new List<WraithHeld2>();

		private static readonly Action<double> Follower = Follow;

		private Node2D _back;

		private Node2D _front;

		private Node2D _top;

		private float _topShow = 1f;

		private Node _form;

		private float _t = -1f;

		private float _wake;

		private float _show = 1f;

		private float _clock;

		private float _readyAt = 1.06f;

		private Vector2 _lo;

		private Vector2 _hi;

		private Vector2 _org;

		private Vector2 _who;

		private float _k;

		private WraithPlayFx.Run _run;

		private bool _runTried;

		private double _scriptTime;

		private double _scriptWorst;

		private int _scriptFrames;

		internal bool FormedForPlay
		{
			get
			{
				if (FxKit.Alive((GodotObject)(object)_form) && !Finished)
				{
					return _t >= _readyAt;
				}
				return true;
			}
		}

		protected override void Build()
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Expected Obj, but got Unknown
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Expected Obj, but got Unknown
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			if (Tuning.F("wraith.held.on", 1f) < 0.5f)
			{
				return;
			}
			WraithStage.Warm();
			_back = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_WraithHeldBack")
			};
			((Node)Back).AddChild((Node)(object)_back, false, (InternalMode)0);
			_back.GlobalTransform = Transform2D.Identity;
			_front = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_WraithHeldFront")
			};
			((Node)Front).AddChild((Node)(object)_front, false, (InternalMode)0);
			_front.GlobalTransform = Transform2D.Identity;
			Vector2 view = ((C.View.X > 100f && C.View.Y > 100f) ? C.View : FxKit.ViewSize());
			WraithFormIntroFx.Seen(C.BackParent, view, out _lo, out _hi);
			WraithStage.Place(C, _lo, _hi, out _org, out _k, out _who);
			_form = WraithStage.MakeForm((Node)(object)_back, (Node)(object)_front, _lo, _hi, _org, _k);
			if (_form == null)
			{
				return;
			}
			try
			{
				((GodotObject)_form).Call(MSetParam, new Variant[2]
				{
					Variant.op_Implicit("haze"),
					Variant.op_Implicit(Mathf.Clamp(Tuning.F("wraith.held.haze", 1f), 0f, 1.5f))
				});
				((GodotObject)_form).Call(MSetParam, new Variant[2]
				{
					Variant.op_Implicit("shake"),
					Variant.op_Implicit(Mathf.Clamp(Tuning.F("wraith.held.shake", 0.6f), 0f, 1.5f))
				});
				Variant val = ((GodotObject)_form).Call(MTimes, Array.Empty<Variant>());
				float[] array = val.AsFloat32Array();
				if (array != null && array.Length >= 2)
				{
					_readyAt = array[1];
				}
				WraithStage.Tune(_form, C, _lo, _hi);
				((GodotObject)_form).Call(MSetParam, new Variant[2]
				{
					Variant.op_Implicit("haze_ui"),
					Variant.op_Implicit(Mathf.Clamp(Tuning.F("wraith.held.haze_ui", 0.45f), 0f, 1f))
				});
			}
			catch (Exception ex)
			{
				DevLog.Warn("Wraith Form standby: " + ex.Message);
			}
			Above();
		}

		private void Above()
		{
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Expected Obj, but got Unknown
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Invalid comparison between Unknown and I8
			if (Tuning.F("wraith.held.over_ui", 1f) < 0.5f)
			{
				return;
			}
			Node2D val = null;
			try
			{
				if (!FxKit.Alive((GodotObject)(object)_form) || !FxKit.Alive((GodotObject)(object)_back) || !((GodotObject)_form).HasMethod(MSetTop))
				{
					return;
				}
				Node overlay = GameRefs.Overlay;
				if (!FxKit.Alive((GodotObject)(object)overlay) || !overlay.IsInsideTree())
				{
					return;
				}
				CanvasItem val2 = (CanvasItem)(object)((overlay is CanvasItem) ? overlay : null);
				if (val2 != null && !val2.IsVisibleInTree())
				{
					return;
				}
				val = new Node2D
				{
					Name = StringName.op_Implicit("SilentFX_WraithHeldTop")
				};
				TimeFreeze.KeepLive((Node)(object)val);
				overlay.AddChild((Node)(object)val, false, (InternalMode)0);
				if (!((Node)val).IsInsideTree())
				{
					((Node)val).QueueFree();
					return;
				}
				val.GlobalTransform = _back.GlobalTransform;
				((CanvasItem)val).Modulate = new Color(1f, 1f, 1f, 0f);
				_top = val;
				FxKit.FreeAfter((Node)(object)val, 1800.0);
				Variant val3 = ((GodotObject)_form).Call(MSetTop, new Variant[1] { Variant.op_Implicit((GodotObject)(object)val) });
				if ((long)val3.VariantType == 1 && val3.AsBool())
				{
					if (!Tops.Contains(this))
					{
						Tops.Add(this);
					}
					Ticker.AddLate(Follower);
					return;
				}
				DevLog.Warn("Wraith Form standby: the layer in front of everything could not be set up - it is drawn as before");
			}
			catch (Exception ex)
			{
				DevLog.Warn("Wraith Form standby (the layer in front of everything): " + ex.Message);
			}
			if (FxKit.Alive((GodotObject)(object)val) && ((Node)val).GetChildCount(false) > 0)
			{
				_top = val;
				if (!Tops.Contains(this))
				{
					Tops.Add(this);
				}
				Ticker.AddLate(Follower);
			}
			else
			{
				if (FxKit.Alive((GodotObject)(object)val))
				{
					((Node)val).QueueFree();
				}
				_top = null;
			}
		}

		private void DropTop()
		{
			Tops.Remove(this);
			Node2D top = _top;
			_top = null;
			try
			{
				if (FxKit.Alive((GodotObject)(object)top))
				{
					((Node)top).QueueFree();
				}
			}
			catch
			{
			}
		}

		private static void Follow(double realDt)
		{
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			if (Tops.Count == 0)
			{
				Ticker.Remove(Follower);
				return;
			}
			for (int num = Tops.Count - 1; num >= 0; num--)
			{
				WraithHeld2 wraithHeld = Tops[num];
				try
				{
					if (wraithHeld.Finished || !FxKit.Alive((GodotObject)(object)wraithHeld._top) || !((Node)wraithHeld._top).IsInsideTree() || !FxKit.Alive((GodotObject)(object)wraithHeld._back) || !((Node)wraithHeld._back).IsInsideTree())
					{
						wraithHeld.DropTop();
						if (!wraithHeld.Finished)
						{
							wraithHeld.Leave(16f);
						}
					}
					else
					{
						wraithHeld._top.GlobalTransform = wraithHeld._back.GlobalTransform;
					}
				}
				catch
				{
				}
			}
		}

		protected override void Tick(float dt)
		{
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			_clock += dt;
			bool num = !Hovering && Armed && !Leaving;
			if (!Hovering && !Leaving && !Released)
			{
				Sfx.Hold("wraith_held", Mathf.Clamp(Lift, 0f, 1f) * (0.55f + 0.45f * Mathf.Clamp(Arm, 0f, 1f)));
			}
			_wake = Mathf.MoveToward(_wake, Leaving ? 0f : 1f, dt / 0.45f);
			if (num)
			{
				if (_t < 0f)
				{
					_t = 0f;
				}
				_t += dt * (Released ? 1.6f : 1f);
				_show = Mathf.MoveToward(_show, 1f, dt / 0.15f);
				if (!_runTried)
				{
					_runTried = true;
					Ready();
				}
			}
			else if (_t >= 0f)
			{
				_t += dt;
				_show = Mathf.MoveToward(_show, 0f, dt / 0.25f);
				if (_show <= 0f)
				{
					_t = -1f;
					_wake = 0f;
					_show = 1f;
				}
			}
			if (_top != null && FxKit.Alive((GodotObject)(object)_top))
			{
				_topShow = Mathf.MoveToward(_topShow, WraithStage.FightOver() ? 0f : 1f, dt / 0.25f);
				((CanvasItem)_top).Modulate = new Color(1f, 1f, 1f, Mathf.Clamp(Fade, 0f, 1f) * _topShow);
			}
			float num2 = ((_run != null) ? Mathf.Clamp(_run.Show, 0f, 1f) : 0f);
			if (FxKit.Alive((GodotObject)(object)_form))
			{
				long timestamp = Stopwatch.GetTimestamp();
				((GodotObject)_form).Call(MSetState, new Variant[4]
				{
					Variant.op_Implicit(_t),
					Variant.op_Implicit(_wake),
					Variant.op_Implicit(_clock),
					Variant.op_Implicit(_show * (1f - num2))
				});
				double totalSeconds = Stopwatch.GetElapsedTime(timestamp).TotalSeconds;
				_scriptTime += totalSeconds;
				_scriptFrames++;
				if (totalSeconds > _scriptWorst)
				{
					_scriptWorst = totalSeconds;
				}
			}
			if (!Released && !Leaving && C.Card != null)
			{
				WraithCardFire.Set(C.Card, 1f, 1f + 0.7f * Lift + 0.9f * Arm);
			}
		}

		internal float SwayPhase()
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (!FxKit.Alive((GodotObject)(object)_form))
				{
					return 1f;
				}
				Variant val = ((GodotObject)_form).Call(MPhase, Array.Empty<Variant>());
				float[] array = val.AsFloat32Array();
				return (array != null && array.Length >= 1 && float.IsFinite(array[0])) ? array[0] : 1f;
			}
			catch
			{
				return 1f;
			}
		}

		internal bool Placement(out Vector2 lo, out Vector2 hi, out Vector2 org, out float k, out Vector2 who)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			lo = _lo;
			hi = _hi;
			org = _org;
			k = _k;
			who = _who;
			if (FxKit.Alive((GodotObject)(object)_form))
			{
				return _k > 0.01f;
			}
			return false;
		}

		private void Ready()
		{
			try
			{
				if (_run == null || _run.Done)
				{
					_run = WraithPlayFx.Prepare(this, C);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Wraith Form standby (the playing made ready): " + ex.Message);
				_run = null;
			}
		}

		protected override void OnRelease()
		{
			Ready();
			WraithPlayFx.LetGo(_run);
		}

		protected override void OnPlay(CardFxContext play)
		{
			Leave(5f);
		}

		protected override void OnLeaving()
		{
			if (!Playing)
			{
				WraithPlayFx.Cancel(_run);
			}
		}

		protected override void OnEnd()
		{
			if (_scriptFrames > 0 && PowerFxManager.LogTriggers)
			{
				DevLog.Info($"Wraith Form: held - her script took {_scriptTime / (double)_scriptFrames * 1000.0:F2} ms a frame (worst {_scriptWorst * 1000.0:F2}) over {_scriptFrames} frames");
			}
			if (!Playing)
			{
				WraithPlayFx.Cancel(_run);
			}
			_run = null;
			_form = null;
			DropTop();
		}
	}
