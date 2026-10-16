// namespace: SilentFX.Powers
// type: BasicPowerFx (class)
// source lines: 1954-2199
	public abstract class BasicPowerFx : PowerFx
	{
		protected Node2D Back;

		protected Node2D Front;

		protected float Time;

		protected float Alpha;

		protected float AlphaTarget = 1f;

		protected float Kick;

		protected bool Stopping;

		protected bool Dead;

		private int _seenAmount = int.MinValue;

		protected abstract string NodeName { get; }

		protected virtual bool UseBack => true;

		protected virtual bool UseFront => true;

		protected virtual float FadeRate => 4.5f;

		protected virtual float KickDecay => 3f;

		protected float A => Mathf.Clamp(Alpha, 0f, 1f);

		public override bool IsAlive
		{
			get
			{
				if (!Dead && (!UseBack || (FxKit.Alive((GodotObject)(object)Back) && ((Node)Back).IsInsideTree())))
				{
					if (UseFront)
					{
						if (FxKit.Alive((GodotObject)(object)Front))
						{
							return ((Node)Front).IsInsideTree();
						}
						return false;
					}
					return true;
				}
				return false;
			}
		}

		public sealed override void Start(bool animateIn)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected Obj, but got Unknown
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Expected Obj, but got Unknown
			try
			{
				if (UseBack)
				{
					Back = new Node2D
					{
						Name = StringName.op_Implicit("SilentFX_" + NodeName + "Back")
					};
					Anchor.BackParent.AddChild((Node)(object)Back, false, (InternalMode)0);
				}
				if (UseFront)
				{
					Front = new Node2D
					{
						Name = StringName.op_Implicit("SilentFX_" + NodeName + "Front")
					};
					Anchor.FrontParent.AddChild((Node)(object)Front, false, (InternalMode)0);
				}
				Alpha = (animateIn ? 0f : 1f);
				Kick = (animateIn ? 1f : 0f);
				_seenAmount = Amount;
				Build(animateIn);
				Layout();
			}
			catch (Exception ex)
			{
				DevLog.Warn(NodeName + " start: " + ex);
				Dispose();
			}
		}

		protected abstract void Build(bool animateIn);

		protected abstract void Layout();

		protected virtual void Tick(float dt)
		{
		}

		protected virtual void OnTrigger(Vector2? at)
		{
		}

		protected virtual void OnStacked()
		{
		}

		public override void OnAmountChanged(int amount)
		{
			bool flag = _seenAmount != int.MinValue && amount > _seenAmount;
			_seenAmount = amount;
			try
			{
				if (flag && !Dead && !Stopping)
				{
					OnStacked();
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn(NodeName + " stacked: " + ex);
				Dispose();
			}
		}

		protected virtual void OnStopping()
		{
		}

		protected virtual void OnDispose()
		{
		}

		public sealed override void Update(double dt)
		{
			if (Dead)
			{
				return;
			}
			try
			{
				if (!Anchor.Valid() || !IsAlive)
				{
					Dispose();
					return;
				}
				Time += (float)dt;
				Alpha = FxKit.Damp(Alpha, AlphaTarget, FadeRate, dt);
				Kick = FxKit.Damp(Kick, 0f, KickDecay, dt);
				Tick((float)dt);
				Layout();
				if (Stopping && Alpha < 0.03f)
				{
					Dispose();
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn(NodeName + " update: " + ex);
				Dispose();
			}
		}

		public sealed override void Trigger(Vector2? at = null)
		{
			if (Dead || Stopping)
			{
				return;
			}
			Kick = 1f;
			try
			{
				OnTrigger(at);
			}
			catch (Exception ex)
			{
				DevLog.Warn(NodeName + " trigger: " + ex);
				Dispose();
			}
		}

		public sealed override void Stop(bool animateOut)
		{
			if (Dead || (Stopping & animateOut))
			{
				return;
			}
			bool flag = (FxKit.Alive((GodotObject)(object)Back) && ((Node)Back).IsInsideTree()) || (FxKit.Alive((GodotObject)(object)Front) && ((Node)Front).IsInsideTree());
			if (animateOut & flag)
			{
				Stopping = true;
				AlphaTarget = 0f;
				try
				{
					OnStopping();
					return;
				}
				catch (Exception ex)
				{
					DevLog.Warn(NodeName + " stopping: " + ex);
					Dispose();
					return;
				}
			}
			Dispose();
		}

		protected void Dispose()
		{
			if (Dead)
			{
				return;
			}
			Dead = true;
			try
			{
				OnDispose();
			}
			catch (Exception ex)
			{
				DevLog.Warn(NodeName + " dispose: " + ex);
			}
			try
			{
				if (FxKit.Alive((GodotObject)(object)Back))
				{
					((Node)Back).QueueFree();
				}
			}
			catch (Exception ex2)
			{
				DevLog.Warn(NodeName + " back cleanup: " + ex2);
			}
			try
			{
				if (FxKit.Alive((GodotObject)(object)Front))
				{
					((Node)Front).QueueFree();
				}
			}
			catch (Exception ex3)
			{
				DevLog.Warn(NodeName + " front cleanup: " + ex3);
			}
		}
	}
