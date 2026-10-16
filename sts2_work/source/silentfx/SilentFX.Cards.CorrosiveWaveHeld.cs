// namespace: SilentFX.Cards
// type: CorrosiveWaveHeld (class)
// source lines: 84445-84552
	public sealed class CorrosiveWaveHeld : HeldSession
	{
		private CorrosiveWaveFx.Flood _flood;

		private Sprite2D _glow;

		private float _creep;

		private float _handoverReach;

		private object _remoteRoom;

		private bool _handoverRecorded;

		protected override void Build()
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			_remoteRoom = (C.Remote ? GameRefs.CombatRoom : null);
			_flood = CorrosiveWaveFx.Flood.Make((Node)(object)Back, (Node)(object)Front);
			if (_flood == null)
			{
				Color venom = PoisonFx.Venom;
				_glow = FxKit.Glow(new Color(venom.R, venom.G, venom.B, 0f), new Vector2(64f, 64f));
				((Node)Back).AddChild((Node)(object)_glow, false, (InternalMode)0);
			}
		}

		protected override void Tick(float dt)
		{
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			float num = Mathf.Min(1f, 0.25f + Time / 0.45f);
			float num2 = (C.Body.X * 0.75f + 50f) * num;
			float num3 = C.View.X * Tuning.F("wave.held_creep", 0.1f);
			_creep = (Armed ? Mathf.Min(num3 * 0.6f, _creep + num3 * 0.6f * dt / Mathf.Max(0.3f, Tuning.F("wave.held_creep_time", 2.5f))) : FxKit.Damp(_creep, 0f, 6f, dt));
			float num4 = num2 + num3 * 0.8f * Arm + _creep;
			float alpha = Mathf.Clamp(Fade, 0f, 1f);
			if (C.Remote && !Leaving)
			{
				_handoverReach = num4;
			}
			if (_flood != null)
			{
				_flood.Alpha = alpha;
				_flood.Reach = num4;
				_flood.Crest = 0.25f + 0.35f * Arm;
				_flood.Flow = Time;
				_flood.Kick = 0f;
				_flood.Bubbles = 0.5f * Arm;
				_flood.Haze = 0f;
				_flood.ClearFeet();
				_flood.Stand(C.Feet, C.Body.X * 0.42f, -1f);
				_flood.Show(CorrosiveWaveFx.Survey(C.Feet, C.Body, C.Enemies, C.View));
			}
			else if (FxKit.Alive((GodotObject)(object)_glow))
			{
				Color venom = PoisonFx.Venom;
				((Node2D)_glow).GlobalPosition = C.Feet + new Vector2(0f, 6f);
				((Node2D)_glow).Scale = new Vector2(num4 * 2.2f, num4 * 0.45f) / 64f;
				((CanvasItem)_glow).Modulate = new Color(venom.R, venom.G, venom.B, 0.2f + 0.16f * Arm);
			}
			if (!C.Remote && Armed && !Leaving)
			{
				CorrosiveWaveFx.HandOver(num4);
			}
		}

		protected override void OnPlay(CardFxContext play)
		{
			if (C.Remote && MultiplayerFx.Active && !_handoverRecorded && !Leaving && !Finished && play != null && C.Card == play.Card && C.Card == MultiplayerFx.CurrentCard && MultiplayerFx.CurrentInvocation != null && _remoteRoom == GameRefs.CombatRoom)
			{
				float handoverReach = _handoverReach;
				CardModel card = C.Card;
				CardModel card2 = C.Card;
				object owner;
				if (card2 == null)
				{
					owner = null;
				}
				else
				{
					Player owner2 = card2.Owner;
					owner = ((owner2 != null) ? owner2.Creature : null);
				}
				_handoverRecorded = CorrosiveWaveFx.RecordRemoteHandover(handoverReach, card, (Creature)owner, MultiplayerFx.CurrentInvocation, _remoteRoom);
			}
			base.OnPlay(play);
		}
	}
