// namespace: SilentFX.Cards
// type: WraithCardFire (class)
// source lines: 115724-115990
	internal static class WraithCardFire
	{
		private sealed class Entry
		{
			public CardModel Card;

			public NCard Host;

			public AnimatedSprite2D Fire;

			public Material Material0;

			public Vector2 Scale0;

			public Vector2 Position0;

			public float Speed0;

			public float High;

			public ShaderMaterial Mat;

			public float Green;

			public float Size = 1f;

			public float WantGreen;

			public float WantSize = 1f;

			public float Idle;
		}

		private const string FirePath = "CardContainer/AncientBanner/Fire";

		private static readonly StringName PGreen = StringName.op_Implicit("green");

		private static readonly StringName PPower = StringName.op_Implicit("power");

		private static readonly List<Entry> Live = new List<Entry>();

		private static readonly Action<double> Driver = Drive;

		public static ShaderMaterial NewMaterial(float green, float power)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			ShaderMaterial val = ShaderLib.NewMaterial("wraith_cardfire");
			if (val == null)
			{
				return null;
			}
			val.SetShaderParameter(PGreen, Variant.op_Implicit(green));
			val.SetShaderParameter(PPower, Variant.op_Implicit(power));
			return val;
		}

		public static AnimatedSprite2D FireOf(NCard node)
		{
			try
			{
				return FxKit.Alive((GodotObject)(object)node) ? ((Node)node).GetNodeOrNull<AnimatedSprite2D>(NodePath.op_Implicit("CardContainer/AncientBanner/Fire")) : null;
			}
			catch
			{
				return null;
			}
		}

		public static void Set(CardModel card, float green, float size)
		{
			if (card == null || Tuning.F("wraith.card.fire", 1f) < 0.5f)
			{
				return;
			}
			try
			{
				Entry entry = null;
				for (int i = 0; i < Live.Count; i++)
				{
					if (Live[i].Card == card)
					{
						entry = Live[i];
					}
				}
				if (entry == null)
				{
					entry = new Entry
					{
						Card = card
					};
					Live.Add(entry);
					Ticker.AddLate(Driver);
				}
				entry.WantGreen = Mathf.Clamp(green, 0f, 1f);
				entry.WantSize = Mathf.Clamp(size, 0.5f, 4f);
				entry.Idle = 0f;
			}
			catch (Exception ex)
			{
				DevLog.Warn("WraithCardFire.Set: " + ex.Message);
			}
		}

		public static void Drop(CardModel card)
		{
			for (int num = Live.Count - 1; num >= 0; num--)
			{
				if (Live[num].Card == card)
				{
					Restore(Live[num]);
					Live.RemoveAt(num);
				}
			}
		}

		public static void Reset()
		{
			for (int num = Live.Count - 1; num >= 0; num--)
			{
				Restore(Live[num]);
			}
			Live.Clear();
		}

		private static void Restore(Entry e)
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				AnimatedSprite2D fire = e.Fire;
				e.Fire = null;
				e.Host = null;
				if (FxKit.Alive((GodotObject)(object)fire))
				{
					if ((object)((CanvasItem)fire).Material == e.Mat)
					{
						((CanvasItem)fire).Material = e.Material0;
					}
					((Node2D)fire).Scale = e.Scale0;
					((Node2D)fire).Position = e.Position0;
					fire.SpeedScale = e.Speed0;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("WraithCardFire.Restore: " + ex.Message);
			}
		}

		private static bool Adopt(Entry e, NCard host)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			AnimatedSprite2D val = FireOf(host);
			if (!FxKit.Alive((GodotObject)(object)val))
			{
				return false;
			}
			e.Host = host;
			e.Fire = val;
			e.Material0 = ((CanvasItem)val).Material;
			e.Scale0 = ((Node2D)val).Scale;
			e.Position0 = ((Node2D)val).Position;
			e.Speed0 = val.SpeedScale;
			e.High = 0f;
			try
			{
				SpriteFrames spriteFrames = val.SpriteFrames;
				if (spriteFrames != null && spriteFrames.GetFrameCount(val.Animation) > 0)
				{
					Texture2D frameTexture = spriteFrames.GetFrameTexture(val.Animation, 0);
					if (frameTexture != null)
					{
						e.High = frameTexture.GetHeight();
					}
				}
			}
			catch
			{
				e.High = 0f;
			}
			if (e.Mat == null)
			{
				e.Mat = NewMaterial(0f, 1f);
			}
			if (e.Mat != null)
			{
				((CanvasItem)val).Material = (Material)(object)e.Mat;
			}
			return true;
		}

		private static void Drive(double realDt)
		{
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			if (Live.Count == 0)
			{
				Ticker.Remove(Driver);
				return;
			}
			float num = (float)realDt;
			for (int num2 = Live.Count - 1; num2 >= 0; num2--)
			{
				Entry entry = Live[num2];
				try
				{
					entry.Idle += num;
					if (entry.Idle > 0.25f)
					{
						entry.WantGreen = 0f;
						entry.WantSize = 1f;
					}
					entry.Green = FxKit.Damp(entry.Green, entry.WantGreen, 9f, num);
					entry.Size = FxKit.Damp(entry.Size, entry.WantSize, 7f, num);
					NCard val = HandCards.NodeOf(entry.Card);
					if (FxKit.Alive((GodotObject)(object)val) && val.Model != entry.Card)
					{
						val = null;
					}
					if (val != entry.Host || (entry.Fire != null && !FxKit.Alive((GodotObject)(object)entry.Fire)))
					{
						Restore(entry);
						if (FxKit.Alive((GodotObject)(object)val))
						{
							Adopt(entry, val);
						}
					}
					if ((entry.Idle > 0.25f && entry.Green < 0.02f && Mathf.Abs(entry.Size - 1f) < 0.02f) || (entry.Idle > 3f && !FxKit.Alive((GodotObject)(object)entry.Fire)))
					{
						Restore(entry);
						Live.RemoveAt(num2);
					}
					else
					{
						AnimatedSprite2D fire = entry.Fire;
						if (FxKit.Alive((GodotObject)(object)fire))
						{
							if (FxKit.Alive((GodotObject)(object)entry.Mat))
							{
								entry.Mat.SetShaderParameter(PGreen, Variant.op_Implicit(entry.Green));
								entry.Mat.SetShaderParameter(PPower, Variant.op_Implicit(1f + 0.18f * (entry.Size - 1f)));
							}
							((Node2D)fire).Scale = entry.Scale0 * entry.Size;
							((Node2D)fire).Position = entry.Position0 + new Vector2(0f, -0.5f * entry.High * entry.Scale0.Y * (entry.Size - 1f));
							fire.SpeedScale = entry.Speed0 * (1f + 0.5f * (entry.Size - 1f));
						}
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("WraithCardFire: " + ex.Message);
					Restore(entry);
					Live.RemoveAt(num2);
				}
			}
		}
	}
