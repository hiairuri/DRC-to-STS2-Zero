// namespace: SilentFX.Cards
// type: HeldSession (class)
// source lines: 68194-68431
	public abstract class HeldSession
	{
		private bool _leaving;

		private float _leaveRate = 9f;

		private float _releasedFor;

		private float _facing = 1f;

		public CardFxContext C { get; private set; }

		protected Node2D Back { get; private set; }

		protected Node2D Front { get; private set; }

		public float Time { get; private set; }

		public float Arm { get; private set; }

		public float Fade { get; private set; }

		public bool Armed { get; internal set; }

		public bool Hovering { get; internal set; }

		public float Lift { get; private set; }

		public bool Released { get; private set; }

		public bool Playing { get; private set; }

		public bool Finished { get; private set; }

		internal float ReleasedFor => _releasedFor;

		protected bool Leaving => _leaving;

		internal void Start(CardFxContext c)
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Expected Obj, but got Unknown
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Expected Obj, but got Unknown
			C = c;
			using (RemoteFxFade.Capture(c.Remote, c.BackParent, c.FrontParent, GameRefs.Overlay, (Node)GameRefs.CombatRoom))
			{
				_facing = c.Facing;
				Back = new Node2D
				{
					Name = StringName.op_Implicit("SilentFX_HeldBack")
				};
				Front = new Node2D
				{
					Name = StringName.op_Implicit("SilentFX_HeldFront")
				};
				TimeFreeze.KeepLive((Node)(object)Back);
				TimeFreeze.KeepLive((Node)(object)Front);
				c.BackParent.AddChild((Node)(object)Back, false, (InternalMode)0);
				c.FrontParent.AddChild((Node)(object)Front, false, (InternalMode)0);
				Build();
				Step(0f);
			}
		}

		internal void Step(float dt)
		{
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			if (Finished)
			{
				return;
			}
			using (RemoteFxFade.Capture(C.Remote, C.BackParent, C.FrontParent, GameRefs.Overlay, (Node)GameRefs.CombatRoom))
			{
				if (!FxKit.Alive((GodotObject)(object)Back) || !FxKit.Alive((GodotObject)(object)Front))
				{
					End();
					return;
				}
				CardModel card = C.Card;
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
				if (obj != null)
				{
					NCombatRoom combatRoom = GameRefs.CombatRoom;
					NCreature val = ((combatRoom != null) ? combatRoom.GetCreatureNode(C.Card.Owner.Creature) : null);
					if (FxKit.Alive((GodotObject)(object)val))
					{
						_facing = GameRefs.FacingOf(val);
					}
				}
				C.Facing = (C.HasTarget ? C.TargetSide : _facing);
				Time += dt;
				if (Released && !Playing)
				{
					_releasedFor += dt;
				}
				Arm = FxKit.Damp(Arm, Armed ? 1f : 0f, 8f, dt);
				Lift = FxKit.Damp(Lift, Hovering ? 0f : 1f, 9f, dt);
				Fade = FxKit.Damp(Fade, _leaving ? 0f : 1f, _leaving ? _leaveRate : 7f, dt);
				Color modulate = new Color(1f, 1f, 1f, Mathf.Clamp(Fade, 0f, 1f));
				((CanvasItem)Back).Modulate = modulate;
				((CanvasItem)Front).Modulate = modulate;
				Tick(dt);
				if (_leaving && Fade < 0.03f)
				{
					End();
				}
			}
		}

		protected abstract void Build();

		protected abstract void Tick(float dt);

		public virtual void OnTargetChanged()
		{
		}

		internal void Release()
		{
			Released = true;
			Armed = true;
			OnRelease();
		}

		protected virtual void OnRelease()
		{
		}

		internal void Play(CardFxContext play)
		{
			using (RemoteFxFade.Capture(C.Remote, C.BackParent, C.FrontParent, GameRefs.Overlay, (Node)GameRefs.CombatRoom))
			{
				Playing = true;
				OnPlay(play);
			}
		}

		protected virtual void OnPlay(CardFxContext play)
		{
			Leave(16f);
		}

		public virtual void OnCardFinished()
		{
			Leave();
		}

		public void Leave(float rate = 9f)
		{
			if (!_leaving)
			{
				OnLeaving();
			}
			_leaving = true;
			_leaveRate = Mathf.Max(_leaveRate, rate);
		}

		protected virtual void OnLeaving()
		{
		}

		public void End()
		{
			if (Finished)
			{
				return;
			}
			Finished = true;
			try
			{
				using (RemoteFxFade.Capture(C.Remote, C.BackParent, C.FrontParent, GameRefs.Overlay, (Node)GameRefs.CombatRoom))
				{
					OnEnd();
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Held effect cleanup failed: " + ex.Message);
			}
			if (FxKit.Alive((GodotObject)(object)Back))
			{
				((Node)Back).QueueFree();
			}
			if (FxKit.Alive((GodotObject)(object)Front))
			{
				((Node)Front).QueueFree();
			}
		}

		protected virtual void OnEnd()
		{
		}

		internal void SetTarget(CardFxContext withTarget)
		{
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			CardFxContext c = C;
			using (RemoteFxFade.Capture(c.Remote, c.BackParent, c.FrontParent, GameRefs.Overlay, (Node)GameRefs.CombatRoom))
			{
				c.HasTarget = withTarget?.HasTarget ?? false;
				if (c.HasTarget)
				{
					c.TargetFeet = withTarget.TargetFeet;
					c.TargetChest = withTarget.TargetChest;
					c.TargetWidth = withTarget.TargetWidth;
					c.TargetHeight = withTarget.TargetHeight;
					c.TargetPoison = withTarget.TargetPoison;
					c.TargetSide = withTarget.TargetSide;
					c.Target = withTarget.Target;
					c.Facing = c.TargetSide;
				}
				else
				{
					c.Target = null;
					c.Facing = _facing;
				}
				OnTargetChanged();
			}
		}
	}
