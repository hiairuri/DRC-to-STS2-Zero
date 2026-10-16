// namespace: SilentFX.Powers
// type: PowerFx (class)
// source lines: 11707-11730
	public abstract class PowerFx
	{
		public FxAnchor Anchor;

		public int Amount;

		public abstract bool IsAlive { get; }

		public abstract void Start(bool animateIn);

		public virtual void Update(double dt)
		{
		}

		public virtual void OnAmountChanged(int amount)
		{
		}

		public virtual void Trigger(Vector2? at = null)
		{
		}

		public abstract void Stop(bool animateOut);
	}
