// namespace: SilentFX.Cards
// type: ShadowEcho (class)
// source lines: 90151-90739
	public static class ShadowEcho
	{
		private sealed class ChainState
		{
			internal int Chain;

			internal int Turn;

			internal ulong At;

			internal Creature Target;

			internal CardModel Source;
		}

		private static readonly HashSet<Creature> Stepped = new HashSet<Creature>(ReferenceEqualityComparer.Instance);

		private static readonly Color Pale = new Color(0.82f, 0.68f, 1f, 0.95f);

		private static int _turn;

		private static ulong _chainAt;

		private static int _chain;

		private static Creature _chainTarget;

		private static CardModel _lastSource;

		private static ulong _beatAt;

		private static readonly Dictionary<Creature, ChainState> PlayerChains = new Dictionary<Creature, ChainState>(ReferenceEqualityComparer.Instance);

		public static void Mark(Creature owner)
		{
			if (owner != null)
			{
				Stepped.Add(owner);
			}
		}

		public static bool HasStepped(Creature owner)
		{
			if (owner != null)
			{
				return Stepped.Contains(owner);
			}
			return false;
		}

		public static void Reset()
		{
			PlayerChains.Clear();
			Stepped.Clear();
			_chain = 0;
			_chainAt = 0uL;
			_chainTarget = null;
			_lastSource = null;
			_beatAt = 0uL;
		}

		public static Creature Master(Creature dealer)
		{
			try
			{
				object obj;
				if (dealer == null)
				{
					obj = null;
				}
				else
				{
					Player petOwner = dealer.PetOwner;
					obj = ((petOwner != null) ? petOwner.Creature : null);
				}
				if (obj == null)
				{
					obj = dealer;
				}
				return (Creature)obj;
			}
			catch
			{
				return dealer;
			}
		}

		public static bool Active(Creature dealer)
		{
			try
			{
				Creature val = Master(dealer);
				return val != null && Stepped.Contains(val) && val.HasPower<DoubleDamagePower>();
			}
			catch
			{
				return false;
			}
		}

		public static void OnHit(CardModel source, Creature dealer, Creature target, bool handled, int damage = -1)
		{
			if (target == null)
			{
				return;
			}
			Creature val = Master(dealer);
			PowerFxManager.TriggerFor<DoubleDamagePower>(val, target);
			if (handled)
			{
				return;
			}
			CardFxContext cardFxContext = CardFxManager.BuildContext(val, target);
			if (cardFxContext == null || !cardFxContext.HasTarget)
			{
				return;
			}
			ChainState value = null;
			if (MultiplayerFx.Active && val != null && !PlayerChains.TryGetValue(val, out value))
			{
				value = (PlayerChains[val] = new ChainState());
			}
			int num = ((value == null) ? Count(target) : Count(target, value));
			bool num2 = num == 0 || source != ((value == null) ? _lastSource : value.Source);
			if (value == null)
			{
				_lastSource = source;
			}
			else
			{
				value.Source = source;
			}
			float lunge = Mathf.Clamp(Tuning.F("shadow.spike_lunge", 0.045f), 0.01f, 0.2f);
			if (num2)
			{
				try
				{
					Beat(cardFxContext, val, target, lunge);
				}
				catch (Exception ex)
				{
					DevLog.Warn("Shadow beat failed: " + ex.Message);
				}
			}
			try
			{
				Strike(cardFxContext, num, damage, lunge, value);
			}
			catch (Exception ex2)
			{
				DevLog.Warn("Shadow spike failed: " + ex2.Message);
			}
		}

		private static void Beat(CardFxContext c, Creature who, Creature target, float lunge)
		{
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			float num = Mathf.Clamp(Tuning.F("shadow.strike_stop", 0.06f), 0f, 0.2f);
			if (num > 0.005f)
			{
				CreatureHold.Hold(target, num);
			}
			ulong milliseconds = FxClock.Milliseconds;
			if (_beatAt != 0L && milliseconds >= _beatAt && milliseconds - _beatAt < 200)
			{
				return;
			}
			_beatAt = milliseconds;
			if (num > 0.005f && who != null && who != target)
			{
				CreatureHold.Hold(who, num);
			}
			float dark = Mathf.Clamp(Tuning.F("shadow.strike_dark", 0.5f), 0f, 0.85f);
			Node frontParent = c.FrontParent;
			if (dark <= 0.01f || !FxKit.Alive((GodotObject)(object)frontParent) || !frontParent.IsInsideTree())
			{
				return;
			}
			ColorRect veil = FxKit.Veil(new Color(0.02f, 0f, 0.05f, 0f));
			frontParent.AddChild((Node)(object)veil, false, (InternalMode)0);
			if (!((Node)veil).IsInsideTree())
			{
				((Node)veil).QueueFree();
				return;
			}
			float stay = lunge + 0.08f;
			float total = stay + 0.16f;
			FxKit.FreeAfter((Node)(object)veil, total + 0.1f);
			FxKit.Drive((Node)(object)veil, total, (float u) =>
			{
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)veil))
				{
					float num2 = u * total;
					float num3 = Mathf.Min(1f, num2 / 0.02f) * (1f - Mathf.Clamp((num2 - stay) / 0.16f, 0f, 1f));
					veil.Color = new Color(0.02f, 0f, 0.05f, dark * num3);
				}
			}, (TransitionType)0, (EaseType)2).TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)veil))
				{
					((Node)veil).QueueFree();
				}
			})));
		}

		private static int Count(Creature target)
		{
			ulong milliseconds = FxClock.Milliseconds;
			ulong num = ((milliseconds >= _chainAt) ? (milliseconds - _chainAt) : 0);
			if (_chainAt == 0L || num > 600)
			{
				_chain = 0;
			}
			else if (num > 40 || target == _chainTarget)
			{
				_chain++;
			}
			_chainAt = milliseconds;
			_chainTarget = target;
			return _chain;
		}

		private static int ShakeFor(int index, int damage)
		{
			if (Tuning.F("shadow.strike_shake", 1f) < 0.5f)
			{
				return 0;
			}
			float num = Mathf.Max(1f, Tuning.F("shadow.strike_shake_at", 18f));
			if ((float)damage >= num * 2.2f)
			{
				return 2;
			}
			if (index == 0)
			{
				return 1;
			}
			if (!((float)damage >= num) || index > 1)
			{
				return 0;
			}
			return 1;
		}

		private static int Count(Creature target, ChainState state)
		{
			ulong milliseconds = FxClock.Milliseconds;
			ulong num = ((milliseconds >= state.At) ? (milliseconds - state.At) : 0);
			if (state.At == 0L || num > 600)
			{
				state.Chain = 0;
			}
			else if (num > 40 || target == state.Target)
			{
				state.Chain++;
			}
			state.At = milliseconds;
			state.Target = target;
			return state.Chain;
		}

		private static void Strike(CardFxContext c, int index, int damage, float lunge, ChainState ownerState = null)
		{
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0427: Unknown result type (might be due to invalid IL or missing references)
			Node front = c.FrontParent;
			if (!FxKit.Alive((GodotObject)(object)front))
			{
				return;
			}
			Node ground = (FxKit.Alive((GodotObject)(object)c.BackParent) ? c.BackParent : front);
			Node sky = ((Tuning.F("shadow.spike_over_ui", 1f) >= 0.5f) ? GameRefs.Overlay : null);
			if (FxKit.Alive((GodotObject)(object)sky) && sky.IsInsideTree())
			{
				Node val = sky;
				CanvasItem val2 = (CanvasItem)(object)((val is CanvasItem) ? val : null);
				if (val2 == null || val2.IsVisibleInTree())
				{
					goto IL_00b0;
				}
			}
			sky = front;
			goto IL_00b0;
			IL_00b0:
			float zoom = 1f;
			Node val3 = front;
			CanvasItem val4 = (CanvasItem)(object)((val3 is CanvasItem) ? val3 : null);
			if (val4 != null)
			{
				Transform2D globalTransform = val4.GetGlobalTransform();
				float num = Mathf.Abs(globalTransform.Scale.X);
				if (float.IsFinite(num) && num > 0.05f)
				{
					zoom = num;
				}
			}
			float num2 = ((index <= 0) ? 1f : Mathf.Max(0.72f, 1f - 0.1f * (float)index));
			float k = Mathf.Clamp(Tuning.F("shadow.spike", 1f), 0.3f, 3f) * num2;
			float w = Mathf.Max(c.TargetWidth, 150f);
			float h = Mathf.Max(c.TargetHeight, 180f);
			Vector2 feet = c.TargetFeet;
			Vector2 chest = c.TargetChest;
			float broad = Mathf.Max(w * 0.95f, 170f) * k;
			float thick = Mathf.Max(w * Mathf.Clamp(Tuning.F("shadow.spike_width", 0.4f), 0.15f, 3f), 64f) * k;
			float over = Mathf.Clamp(Tuning.F("shadow.spike_over", 0.18f), 0f, 0.6f);
			float tall = ((over > 0.001f) ? Mathf.Clamp((feet.Y + c.View.Y * over) / zoom, h * 1.2f, 2600f) : (Mathf.Clamp(h * Mathf.Clamp(Tuning.F("shadow.spike_height", 2.1f), 0.8f, 4f), 480f, 1250f) * k));
			float lean = ((((ownerState == null) ? _turn++ : ownerState.Turn++) % 2 == 0) ? 1f : (-1f));
			Vector2 from = c.Feet + new Vector2(0f, (0f - c.Body.Y) * Tuning.F("shadow.ground", 0.07f));
			Murk dart = Murk.Make(ground, 0.45f);
			if (dart != null)
			{
				float total = lunge + 0.1f;
				FxKit.FreeAfter((Node)(object)dart.Root, Mathf.Max(0f, total) + 0.1f);
				dart.Lay(from, from.Lerp(feet, 0.1f), 36f * k, 0f);
				FxKit.Drive((Node)(object)dart.Root, total, (float u) =>
				{
					//IL_0058: Unknown result type (might be due to invalid IL or missing references)
					//IL_007e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0083: Unknown result type (might be due to invalid IL or missing references)
					//IL_0095: Unknown result type (might be due to invalid IL or missing references)
					//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
					//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
					//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
					//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
					float num3 = u * total;
					float num4 = Mathf.Clamp(num3 / lunge, 0f, 1f);
					float num5 = Mathf.Clamp((num3 - lunge) / 0.1f, 0f, 1f);
					Vector2 b = from.Lerp(feet, 0.1f + 0.9f * (1f - (1f - num4) * (1f - num4)));
					Vector2 a = from.Lerp(feet, 0.92f * num5 * num5);
					dart.Lay(a, b, Mathf.Lerp(64f, 38f, num5) * k, 1f - 0.7f * num5);
				}, (TransitionType)0, (EaseType)2).TweenCallback(Callable.From((Action)(() =>
				{
					dart.Free();
				})));
			}
			int shake = ShakeFor(index, damage);
			FxKit.After(front, lunge, () =>
			{
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				Spike(front, ground, sky, zoom, feet, chest, lean, w, h, tall, thick, broad, over > 0.001f, k, index, shake);
			});
		}

		private static ShadeMist Thorn(float seed, float taper, float wisp, float glow, float speed, float lean)
		{
			ShadeMist shadeMist = new ShadeMist(0, Mathf.PosMod(seed, 10f));
			shadeMist.Set("foot", 1f);
			shadeMist.Set("bulge", 0.03f);
			shadeMist.Set("taper", taper);
			shadeMist.Set("wisp", wisp);
			shadeMist.Set("glow", glow);
			shadeMist.Set("speed", speed);
			shadeMist.Set("lean", lean);
			return shadeMist;
		}

		private static void Spike(Node front, Node ground, Node sky, float zoom, Vector2 feet, Vector2 chest, float lean, float w, float h, float tall, float thick, float broad, bool through, float k, int index, int shake)
		{
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Expected Obj, but got Unknown
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0401: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_044b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0521: Unknown result type (might be due to invalid IL or missing references)
			//IL_0526: Unknown result type (might be due to invalid IL or missing references)
			//IL_0564: Unknown result type (might be due to invalid IL or missing references)
			//IL_0601: Unknown result type (might be due to invalid IL or missing references)
			//IL_060c: Unknown result type (might be due to invalid IL or missing references)
			//IL_063a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0640: Unknown result type (might be due to invalid IL or missing references)
			//IL_064d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0652: Unknown result type (might be due to invalid IL or missing references)
			//IL_057e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0668: Unknown result type (might be due to invalid IL or missing references)
			//IL_0708: Unknown result type (might be due to invalid IL or missing references)
			//IL_070e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0726: Unknown result type (might be due to invalid IL or missing references)
			//IL_072b: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c9: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)front))
			{
				return;
			}
			if (FxKit.Alive((GodotObject)(object)sky) && sky.IsInsideTree())
			{
				CanvasItem val = (CanvasItem)(object)((sky is CanvasItem) ? sky : null);
				if (val == null || val.IsVisibleInTree())
				{
					goto IL_005d;
				}
			}
			sky = front;
			goto IL_005d;
			IL_005d:
			bool flag = sky != front;
			float s = (flag ? (zoom / FogKit.ScaleOf(sky)) : 1f);
			Node2D holder = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_ShadowSpike")
			};
			sky.AddChild((Node)(object)holder, false, (InternalMode)0);
			if (!((Node)holder).IsInsideTree())
			{
				((Node)holder).QueueFree();
				return;
			}
			float rise = Mathf.Clamp(Tuning.F("shadow.spike_rise", 0.06f), 0.02f, 0.3f);
			float hold = Mathf.Clamp(Tuning.F("shadow.spike_hold", 0.1f), 0f, 0.6f);
			float fall = Mathf.Clamp(Tuning.F("shadow.spike_fall", 0.15f), 0.05f, 0.6f);
			float num = lean * 0.07f;
			Vector2 up = new Vector2(Mathf.Sin(num), 0f - Mathf.Cos(num));
			Vector2 root = feet + new Vector2(0f, h * 0.03f * zoom);
			float total = rise + hold + fall;
			Sprite2D dusk = Shade.Blob(new Vector2(w * 1.4f, h * 1.35f), 0f);
			front.AddChild((Node)(object)dusk, false, (InternalMode)0);
			if (((Node)dusk).IsInsideTree())
			{
				((Node2D)dusk).GlobalPosition = chest;
				FxKit.FreeAfter((Node)(object)dusk, Mathf.Max(0f, total) + 0.1f);
			}
			else
			{
				((Node)dusk).QueueFree();
				dusk = null;
			}
			float num2 = feet.X * 0.013f + (float)index * 1.7f;
			ShadeMist body = Thorn(num2, 0.95f, 0.7f, 1f, 3f, lean * 0.1f);
			ShadeMist core = Thorn(num2 + 3.3f, 0.8f, 0.28f, 0.2f, 2.2f, lean * 0.1f);
			((Node)holder).AddChild((Node)(object)body.Root, false, (InternalMode)0);
			((Node)holder).AddChild((Node)(object)core.Root, false, (InternalMode)0);
			body.Root.GlobalPosition = root;
			core.Root.GlobalPosition = root;
			body.Root.Rotation = num;
			core.Root.Rotation = num;
			FxKit.FreeAfter((Node)(object)holder, Mathf.Max(0f, total) + 0.1f);
			Action<float> action = (float u) =>
			{
				//IL_0119: Unknown result type (might be due to invalid IL or missing references)
				float num3 = u * total;
				float num4 = Mathf.Clamp(num3 / rise, 0f, 1f);
				float num5 = Mathf.Clamp((num3 - rise - hold) / fall, 0f, 1f);
				float num6 = Intro.BackOut(num4);
				float num7 = tall * Mathf.Max(0.04f, (0.12f + 0.88f * num6) * (1f - 0.94f * num5 * num5));
				float num8 = Mathf.Lerp(1.55f, 1f, num4) * (1f - 0.45f * num5);
				float num9 = 1f + 0.012f * Mathf.Sin(num3 * 95f) * (1f - num5);
				float intensity = Mathf.Min(1f, num4 * 5f) * (1f - num5 * num5 * num5);
				if (dusk != null && FxKit.Alive((GodotObject)(object)dusk))
				{
					((CanvasItem)dusk).Modulate = Shade.Inked(0.55f * Mathf.Min(1f, num4 * 4f) * (1f - num5));
				}
				if (FxKit.Alive((GodotObject)(object)body.Root))
				{
					body.Size(thick * 2f * num8 * s, num7 * num9 * s);
					body.Intensity = intensity;
					body.Pulse = 0.45f * (1f - Mathf.Clamp(num3 / (rise + hold * 0.6f + 0.04f), 0f, 1f));
				}
				if (FxKit.Alive((GodotObject)(object)core.Root))
				{
					core.Size(thick * 1.05f * num8 * s, num7 * 0.94f * num9 * s);
					core.Intensity = intensity;
				}
			};
			action(0f);
			FxKit.Drive((Node)(object)holder, total, action, (TransitionType)0, (EaseType)2).TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)holder))
				{
					((Node)holder).QueueFree();
				}
				if (dusk != null && FxKit.Alive((GodotObject)(object)dusk))
				{
					((Node)dusk).QueueFree();
				}
			})));
			Brush.Cut(sky, root, root + up * (tall * 0.97f * s), (9f * k + thick * 0.035f) * s, Pale, 0f, rise * 0.8f, hold * 0.4f, fall * 0.8f);
			FxKit.Flash(front, root, Shade.Glowing(0.6f), broad * 0.95f, 2f, 0.2);
			if (FxKit.Alive((GodotObject)(object)ground))
			{
				FxKit.RingPulse(ground, feet, broad * 2.6f, Shade.Rimmed(0.95f), 0f, 0.26f, 0.16f, 1f, 0.26f, 0f, 0.055f, 0.9f);
				Sprite2D stain = Shade.Blob(new Vector2(broad * 1.1f, Mathf.Max(18f, broad * 0.28f)), 0.85f);
				ground.AddChild((Node)(object)stain, false, (InternalMode)0);
				if (((Node)stain).IsInsideTree())
				{
					((Node2D)stain).GlobalPosition = feet;
					FxKit.FreeAfter((Node)(object)stain, 0.41999998688697815);
					Vector2 small = ((Node2D)stain).Scale;
					FxKit.Drive((Node)(object)stain, 0.32f, (float u) =>
					{
						//IL_001f: Unknown result type (might be due to invalid IL or missing references)
						//IL_0045: Unknown result type (might be due to invalid IL or missing references)
						//IL_0067: Unknown result type (might be due to invalid IL or missing references)
						if (FxKit.Alive((GodotObject)(object)stain))
						{
							((Node2D)stain).Scale = small * (1f + 1.9f * (1f - (1f - u) * (1f - u)));
							((CanvasItem)stain).Modulate = Shade.Inked(0.85f * (1f - u));
						}
					}, (TransitionType)0, (EaseType)2).TweenCallback(Callable.From((Action)(() =>
					{
						if (FxKit.Alive((GodotObject)(object)stain))
						{
							((Node)stain).QueueFree();
						}
					})));
				}
				else
				{
					((Node)stain).QueueFree();
				}
				Brush.Dust(ground, feet, 0f, (index <= 1) ? 12 : 6, 1.25f * k);
			}
			FxKit.Burst(front, root + up * (h * 0.1f), Shade.Inked(0.95f), (index <= 1) ? 14 : 7, 280f * k, 820f * k, 0.44, 0.16f, 0.46f, new Vector2(0f, 1100f), 58f, up, broad * 0.22f, 0f, additive: false);
			Shade.Sparks(front, root + up * (h * 0.3f), (index == 0) ? 8 : 4, 540f * k, up, 50f, broad * 0.2f);
			if (index <= 2)
			{
				if (through && root.Y > 140f)
				{
					Shade.Puff(sky, root + up * (root.Y - 46f), Mathf.Max(thick * 1.1f, 90f) * s, 2f, 0.46f, 0.55f, 0.8f);
				}
				else
				{
					Shade.Puff(sky, root + up * (tall * 0.88f * s), thick * 0.55f * s, 2f, 0.46f, 0.55f, 0.8f);
				}
				FxKit.After(front, rise + hold + fall * 0.55f, () =>
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					//IL_001e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0023: Unknown result type (might be due to invalid IL or missing references)
					Shade.Puff(front, root + up * (broad * 0.15f), broad * 0.9f, 1.8f, 0.38f, 0.75f, 0.8f);
				});
			}
			if (shake > 0)
			{
				GameRefs.Shake(shake);
			}
		}

		static ShadowEcho()
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		}
	}
