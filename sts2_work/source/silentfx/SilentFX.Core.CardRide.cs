// namespace: SilentFX.Core
// type: CardRide (class)
// source lines: 33570-33933
	public static class CardRide
	{
		public struct Pose
		{
			public Vector2 Position;

			public float Rotation;

			public Vector2 Scale;

			public float Alpha;
		}

		public delegate void Shaper(ref Pose pose, float t, in Pose from, in Pose to);

		public sealed class Spec
		{
			public float Duration = 0.3f;

			public float Delay;

			public Pose? From;

			public Shaper Shape;

			public Node Parent;

			public Action<NCard> Started;

			public Action<NCard, Pose, float> Frame;

			public Action<Pose> Arrived;
		}

		private sealed class Ride
		{
			public CardModel Card;

			public NCard Real;

			public NCard Copy;

			public Spec Spec;

			public Pose From;

			public Func<Pose> Target;

			public float Duration;

			public float Delay;

			public float Elapsed;

			public bool Done;
		}

		private static readonly List<Ride> Live = new List<Ride>();

		private static bool _registered;

		internal static void Init()
		{
			if (!_registered)
			{
				Ticker.AddLate(Update);
				_registered = true;
			}
		}

		public static bool ToCard(NCard real, Spec spec)
		{
			Ride ride = null;
			try
			{
				if (spec == null || !FxKit.Alive((GodotObject)(object)real) || !((Node)real).IsInsideTree() || real.Model == null)
				{
					return false;
				}
				CardModel model = real.Model;
				if (!Allowed(real, model))
				{
					return false;
				}
				Node val = spec.Parent ?? GameRefs.UiLayer;
				if (!FxKit.Alive((GodotObject)(object)val) || !val.IsInsideTree())
				{
					return false;
				}
				Finish(model);
				ride = NewRide(model, spec, spec.From ?? ReadPose(real));
				ride.Real = real;
				ride.Copy = CardStandIn.Make(model, val);
				if (!FxKit.Alive((GodotObject)(object)ride.Copy))
				{
					return false;
				}
				CardStandIn.Hide(real);
				return Start(ride);
			}
			catch (Exception ex)
			{
				DevLog.Warn("CardRide.ToCard: " + ex);
				Stop(ride);
				return false;
			}
		}

		public static bool Fly(CardModel card, Spec spec, Func<Pose> to)
		{
			Ride ride = null;
			try
			{
				if (card == null || spec == null || !spec.From.HasValue || to == null)
				{
					return false;
				}
				if (HeldFxManager.Carried == card || HandCards.Selecting)
				{
					return false;
				}
				Node val = spec.Parent ?? GameRefs.UiLayer;
				if (!FxKit.Alive((GodotObject)(object)val) || !val.IsInsideTree())
				{
					return false;
				}
				to();
				ride = NewRide(card, spec, spec.From.Value);
				ride.Target = to;
				ride.Copy = CardStandIn.Make(card, val);
				if (!FxKit.Alive((GodotObject)(object)ride.Copy))
				{
					return false;
				}
				return Start(ride);
			}
			catch (Exception ex)
			{
				DevLog.Warn("CardRide.Fly: " + ex);
				Stop(ride);
				return false;
			}
		}

		private static Ride NewRide(CardModel card, Spec spec, Pose from)
		{
			return new Ride
			{
				Card = card,
				Spec = spec,
				From = from,
				Duration = (float.IsFinite(spec.Duration) ? Mathf.Clamp(spec.Duration, 0.05f, 1.6f) : 0.3f),
				Delay = (float.IsFinite(spec.Delay) ? Mathf.Max(0f, spec.Delay) : 0f)
			};
		}

		private static bool Start(Ride ride)
		{
			Init();
			Live.Add(ride);
			Apply(ride.Copy, ride.From);
			ride.Spec.Started?.Invoke(ride.Copy);
			return !ride.Done;
		}

		public static bool Riding(CardModel card)
		{
			if (card == null)
			{
				return false;
			}
			foreach (Ride item in Live)
			{
				if (!item.Done && item.Real != null && item.Card == card)
				{
					return true;
				}
			}
			return false;
		}

		public static void Finish(CardModel card)
		{
			if (card == null)
			{
				return;
			}
			Ride[] array = Live.ToArray();
			foreach (Ride ride in array)
			{
				if (ride.Card == card)
				{
					Stop(ride);
				}
			}
		}

		public static void Reset()
		{
			Ride[] array = Live.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				Stop(array[i]);
			}
			Live.Clear();
		}

		private static bool Allowed(NCard real, CardModel card)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Invalid comparison between Unknown and I4
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Invalid comparison between Unknown and I4
			if (FxKit.Alive((GodotObject)(object)real) && ((Node)real).IsInsideTree() && real.Model == card && HeldFxManager.Carried != card && !HandCards.Selecting)
			{
				CardPile pile = card.Pile;
				if (pile == null || (int)pile.Type != 2)
				{
					CardPile pile2 = card.Pile;
					if (pile2 == null)
					{
						return false;
					}
					return (int)pile2.Type == 5;
				}
				return true;
			}
			return false;
		}

		private static Pose ReadPose(NCard real)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			CardStandIn.Pose(real, out var centre, out var rotation, out var scale);
			return new Pose
			{
				Position = centre,
				Rotation = rotation,
				Scale = scale,
				Alpha = 1f
			};
		}

		private static void Apply(NCard copy, Pose pose)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			CardStandIn.Place(copy, pose.Position, pose.Rotation, pose.Scale);
			((CanvasItem)copy).Modulate = new Color(1f, 1f, 1f, pose.Alpha);
		}

		private static void Update(double realDt)
		{
			Ride[] array = null;
			try
			{
				int count = Live.Count;
				if (count == 0)
				{
					return;
				}
				array = ArrayPool<Ride>.Shared.Rent(count);
				Live.CopyTo(array);
				float dt = (float)(realDt * (double)Mathf.Clamp((float)Engine.TimeScale, 0f, 4f));
				for (int i = 0; i < count; i++)
				{
					Ride ride = array[i];
					if (!ride.Done)
					{
						try
						{
							Step(ride, dt);
						}
						catch (Exception ex)
						{
							DevLog.Warn("CardRide (callback or frame): " + ex);
							Stop(ride);
						}
					}
				}
			}
			catch (Exception ex2)
			{
				DevLog.Error("CardRide.Update: " + ex2);
				Reset();
			}
			finally
			{
				if (array != null)
				{
					ArrayPool<Ride>.Shared.Return(array, clearArray: true);
				}
			}
		}

		private static void Step(Ride ride, float dt)
		{
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)ride.Copy) || !((Node)ride.Copy).IsInsideTree() || ride.Copy.Model != ride.Card || HeldFxManager.Carried == ride.Card || HandCards.Selecting || (ride.Real != null && !Allowed(ride.Real, ride.Card)))
			{
				Stop(ride);
				return;
			}
			ride.Elapsed += dt;
			if (ride.Elapsed > ride.Delay + ride.Duration + 1f)
			{
				Stop(ride);
				return;
			}
			Pose to = ((ride.Real != null) ? ReadPose(ride.Real) : ride.Target());
			if (ride.Done)
			{
				return;
			}
			float num = Mathf.Clamp((ride.Elapsed - ride.Delay) / ride.Duration, 0f, 1f);
			Pose from = ride.From;
			Pose pose = from;
			if (ride.Elapsed >= ride.Delay)
			{
				float num2 = ((ride.Spec.Shape == null) ? (num * num * (3f - 2f * num)) : num);
				pose = new Pose
				{
					Position = from.Position.Lerp(to.Position, num2),
					Rotation = Mathf.LerpAngle(from.Rotation, to.Rotation, num),
					Scale = from.Scale.Lerp(to.Scale, num),
					Alpha = Mathf.Lerp(from.Alpha, to.Alpha, num)
				};
				ride.Spec.Shape?.Invoke(ref pose, num, in from, in to);
			}
			if (!ride.Done)
			{
				Apply(ride.Copy, pose);
				ride.Spec.Frame?.Invoke(ride.Copy, pose, num);
				if (!ride.Done && !(num < 1f))
				{
					ride.Spec.Arrived?.Invoke(pose);
					Stop(ride);
				}
			}
		}

		private static void Stop(Ride ride)
		{
			if (ride != null && !ride.Done)
			{
				ride.Done = true;
				Live.Remove(ride);
				CardStandIn.Free(ride.Copy);
				if (ride.Real != null)
				{
					CardStandIn.Show(ride.Real);
				}
			}
		}
	}
