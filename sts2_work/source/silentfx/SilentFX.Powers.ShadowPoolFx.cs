// namespace: SilentFX.Powers
// type: ShadowPoolFx (class)
// source lines: 12948-13060
	public sealed class ShadowPoolFx : BasicPowerFx
	{
		private static readonly HashSet<Creature> Expected = new HashSet<Creature>(ReferenceEqualityComparer.Instance);

		private static readonly Dictionary<Creature, float> Forces = new Dictionary<Creature, float>(ReferenceEqualityComparer.Instance);

		private readonly ShadowPresence _presence = new ShadowPresence(full: false);

		private float _grown;

		private float _waited;

		private float _surge;

		private bool _arrived;

		protected override string NodeName => "ShadowPool";

		protected override float KickDecay => 3f;

		public static void Expect(Creature owner)
		{
			if (owner != null)
			{
				Expected.Add(owner);
				Forces.Remove(owner);
			}
		}

		public static void Landed(Creature owner, float force = 1f)
		{
			if (owner != null && Expected.Remove(owner) && force > 1.001f && float.IsFinite(force))
			{
				Forces[owner] = force;
			}
		}

		public static void Reset()
		{
			Expected.Clear();
			Forces.Clear();
		}

		protected override void Build(bool animateIn)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			_presence.Build(Back, Front, Anchor.SafeBodySize());
			_arrived = !animateIn;
			_grown = (animateIn ? 0f : 1f);
		}

		protected override void Tick(float dt)
		{
			_waited += dt;
			if (!_arrived)
			{
				float num = ShadowStepFx.HomeAfter() + 1f;
				if (_waited < num)
				{
					object owner = Anchor.Owner;
					Creature val = (Creature)((owner is Creature) ? owner : null);
					if (val != null && Expected.Contains(val))
					{
						return;
					}
				}
				_arrived = true;
				float num2 = 1f;
				object owner2 = Anchor.Owner;
				Creature val2 = (Creature)((owner2 is Creature) ? owner2 : null);
				if (val2 != null && Forces.Remove(val2, out var value))
				{
					num2 = Mathf.Clamp(value, 1f, 2.5f);
				}
				_surge = num2;
				try
				{
					_presence.Erupt(Anchor, num2);
				}
				catch (Exception ex)
				{
					DevLog.Warn("Shadow pool eruption failed: " + ex.Message);
				}
				if (Tuning.F("shadow.erupt_shake", 0f) >= 0.5f)
				{
					GameRefs.Shake(1);
				}
			}
			_grown = Mathf.Min(1f, _grown + dt / Mathf.Max(0.05f, Tuning.F("shadow.flood", 0.36f)));
			_surge = FxKit.Damp(_surge, 0f, 3.2f, dt);
		}

		protected override void Layout()
		{
			if (FxKit.Alive((GodotObject)(object)Back) && FxKit.Alive((GodotObject)(object)Front))
			{
				_presence.Layout(Anchor, Time, A, Intro.BackOut(_grown), Mathf.Clamp(_grown, 0f, 1f), _surge, 0f, 1f, Kick);
			}
		}

		protected override void OnTrigger(Vector2? at)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			_surge = Mathf.Max(_surge, 0.5f);
			Node frontParent = Anchor.FrontParent;
			if (FxKit.Alive((GodotObject)(object)frontParent))
			{
				Shade.Motes(frontParent, Anchor.Feet(), 5, 220f, new Vector2(0f, -1f), 70f, Anchor.SafeBodySize().X * 0.25f);
			}
		}
	}
