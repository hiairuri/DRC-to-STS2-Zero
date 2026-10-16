// namespace: SilentFX.Patches
// type: FlightHold (class)
// source lines: 21626-21808
	internal static class FlightHold
	{
		private sealed class Held
		{
			public NCardFlyVfx Fly;

			public NCard Card;

			public CanvasItem Trail;

			public CardFlight.Plan Plan;

			public float Speed;

			public float Accel;

			public float Rotation;

			public Vector2 At;

			public float For;
		}

		private static readonly List<Held> Live = new List<Held>();

		private static FieldRef<NCardFlyVfx, float> _speed;

		private static FieldRef<NCardFlyVfx, float> _accel;

		private static bool _looked;

		private static bool _ticking;

		public static void Begin(NCardFlyVfx fly, NCard card, CanvasItem trail, CardFlight.Plan plan)
		{
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			if (!_looked)
			{
				_looked = true;
				try
				{
					_speed = AccessTools.FieldRefAccess<NCardFlyVfx, float>("_speed");
					_accel = AccessTools.FieldRefAccess<NCardFlyVfx, float>("_accel");
				}
				catch (Exception ex)
				{
					_speed = null;
					_accel = null;
					DevLog.Warn("FlightHold: cards cannot be kept back (" + ex.Message + ")");
				}
			}
			bool flag = false;
			if (plan.Hold != null && _speed != null && _accel != null && plan.HoldMax > 0f)
			{
				try
				{
					flag = plan.Hold();
				}
				catch
				{
					flag = false;
				}
			}
			if (!flag)
			{
				Off(card, plan);
				return;
			}
			Held held = new Held
			{
				Fly = fly,
				Card = card,
				Trail = ((FxKit.Alive((GodotObject)(object)trail) && trail.Visible) ? trail : null),
				Plan = plan,
				Speed = _speed.Invoke(fly),
				Accel = _accel.Invoke(fly),
				Rotation = ((Control)card).Rotation,
				At = ((Control)card).GlobalPosition
			};
			_speed.Invoke(fly) = 0f;
			_accel.Invoke(fly) = 0f;
			if (held.Trail != null)
			{
				held.Trail.Visible = false;
			}
			Live.Add(held);
			if (!_ticking)
			{
				Init();
			}
		}

		public static void Init()
		{
			Ticker.Remove(Update);
			Ticker.AddLate(Update);
			_ticking = true;
		}

		private static void Off(NCard card, CardFlight.Plan plan)
		{
			if (plan.Released == null)
			{
				return;
			}
			try
			{
				plan.Released(card);
			}
			catch (Exception ex)
			{
				DevLog.Warn("FlightHold (released): " + ex);
			}
		}

		private static void Update(double realDt)
		{
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			if (Live.Count == 0)
			{
				return;
			}
			float num = (float)(realDt * (double)Mathf.Clamp((float)Engine.TimeScale, 0f, 4f));
			for (int num2 = Live.Count - 1; num2 >= 0; num2--)
			{
				Held held = Live[num2];
				try
				{
					if (!FxKit.Alive((GodotObject)(object)held.Fly) || !FxKit.Alive((GodotObject)(object)held.Card))
					{
						Live.RemoveAt(num2);
						Off(null, held.Plan);
					}
					else
					{
						held.For += num;
						bool flag = held.For < held.Plan.HoldMax;
						if (flag)
						{
							try
							{
								flag = held.Plan.Hold();
							}
							catch
							{
								flag = false;
							}
						}
						if (flag)
						{
							_speed.Invoke(held.Fly) = 0f;
							_accel.Invoke(held.Fly) = 0f;
							((Control)held.Card).Rotation = held.Rotation;
							((Control)held.Card).GlobalPosition = held.At;
						}
						else
						{
							_speed.Invoke(held.Fly) = held.Speed;
							_accel.Invoke(held.Fly) = held.Accel;
							if (FxKit.Alive((GodotObject)(object)held.Trail))
							{
								held.Trail.Visible = true;
							}
							Live.RemoveAt(num2);
							Off(held.Card, held.Plan);
						}
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("FlightHold: " + ex);
					Live.RemoveAt(num2);
				}
			}
		}

		public static void Reset()
		{
			Live.Clear();
		}
	}
	[HarmonyPatch(typeof(VfxCmd), "PlayOnCreatureCenter")]
