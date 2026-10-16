// namespace: SilentFX.Cards
// type: HandShade (class)
// source lines: 90814-91096
	internal sealed class HandShade
	{
		private sealed class Cover
		{
			public NCard Node;

			public CardModel Card;

			public float Shown;

			public Color Base;

			public Color Wrote;

			public bool Has;
		}

		private static readonly List<Cover> Covers = new List<Cover>();

		private static HandShade _driver;

		private static int _made;

		private readonly int _rank = ++_made;

		private readonly List<HandCards.Entry> _hand = new List<HandCards.Entry>();

		private bool _freed;

		public List<HandCards.Entry> Hand => _hand;

		public static bool Clear
		{
			get
			{
				for (int i = 0; i < Covers.Count; i++)
				{
					if (Covers[i].Shown > 0.004f)
					{
						return false;
					}
				}
				return true;
			}
		}

		private bool Drives()
		{
			if (_freed)
			{
				return false;
			}
			if (_driver == null || _driver._freed || _driver == this || _rank > _driver._rank)
			{
				_driver = this;
				return true;
			}
			return false;
		}

		private static bool Same(Color a, Color b)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			if (Mathf.Abs(a.R - b.R) < 0.0015f && Mathf.Abs(a.G - b.G) < 0.0015f && Mathf.Abs(a.B - b.B) < 0.0015f)
			{
				return Mathf.Abs(a.A - b.A) < 0.0015f;
			}
			return false;
		}

		public bool Update(CardModel except, float dark, float dt, float rate = 14f, float frontX = 0f, float reach = -1f, float fall = 0f, float peak = 1f)
		{
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			HandCards.Fill(_hand);
			if (!Drives())
			{
				return false;
			}
			for (int num = Covers.Count - 1; num >= 0; num--)
			{
				Cover cover = Covers[num];
				bool flag = false;
				if (FxKit.Alive((GodotObject)(object)cover.Node) && cover.Node.Model == cover.Card)
				{
					for (int i = 0; i < _hand.Count; i++)
					{
						if (_hand[i].Node == cover.Node)
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					Release(cover);
					Covers.RemoveAt(num);
				}
			}
			dark = Mathf.Clamp(dark, 0f, 1f);
			for (int j = 0; j < _hand.Count; j++)
			{
				HandCards.Entry entry = _hand[j];
				if (!FxKit.Alive((GodotObject)(object)entry.Node))
				{
					continue;
				}
				Cover cover2 = null;
				for (int k = 0; k < Covers.Count; k++)
				{
					if (Covers[k].Node == entry.Node)
					{
						cover2 = Covers[k];
						break;
					}
				}
				float num2 = dark;
				if (entry.Card == except)
				{
					num2 = 0f;
				}
				else if (reach >= 0f)
				{
					float num3 = Mathf.Abs(((CanvasItem)entry.Node).GetGlobalTransform().Origin.X - frontX) - reach;
					float num4 = Mathf.Clamp(peak, 0f, 1f);
					if (num3 <= 0f)
					{
						num2 = num4;
					}
					else if (fall > 0f)
					{
						num2 = Mathf.Lerp(num4, dark, Mathf.Clamp(num3 / fall, 0f, 1f));
					}
				}
				if (cover2 == null)
				{
					if (num2 <= 0.001f)
					{
						continue;
					}
					cover2 = new Cover
					{
						Node = entry.Node,
						Card = entry.Card
					};
					Covers.Add(cover2);
				}
				cover2.Shown = FxKit.Damp(cover2.Shown, num2, rate, dt);
				if (num2 <= 0.001f && cover2.Shown < 0.004f)
				{
					Release(cover2);
					Covers.Remove(cover2);
				}
				else
				{
					Write(cover2);
				}
			}
			return true;
		}

		private static void Write(Cover cover)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			Control body = cover.Node.Body;
			if (FxKit.Alive((GodotObject)(object)body))
			{
				Color modulate = ((CanvasItem)body).Modulate;
				if (!cover.Has || !Same(modulate, cover.Wrote))
				{
					cover.Base = modulate;
				}
				float num = Mathf.Clamp(cover.Shown, 0f, 1f);
				Color val = new Color(cover.Base.R * Mathf.Lerp(1f, Shade.Ink.R, num), cover.Base.G * Mathf.Lerp(1f, Shade.Ink.G, num), cover.Base.B * Mathf.Lerp(1f, Shade.Ink.B, num), cover.Base.A);
				if (!Same(val, modulate))
				{
					((CanvasItem)body).Modulate = val;
				}
				else
				{
					val = modulate;
				}
				cover.Wrote = val;
				cover.Has = true;
			}
		}

		internal static float ShownOf(NCard node)
		{
			for (int i = 0; i < Covers.Count; i++)
			{
				if (Covers[i].Node == node)
				{
					return Mathf.Clamp(Covers[i].Shown, 0f, 1f);
				}
			}
			return 0f;
		}

		public void Drop(NCard node)
		{
			for (int num = Covers.Count - 1; num >= 0; num--)
			{
				if (Covers[num].Node == node)
				{
					Release(Covers[num]);
					Covers.RemoveAt(num);
				}
			}
		}

		public void Free()
		{
			if (!_freed)
			{
				bool flag = _driver == this;
				_freed = true;
				_hand.Clear();
				if (flag)
				{
					_driver = null;
					ReleaseAll();
				}
			}
		}

		public static void ResetAll()
		{
			_driver = null;
			ReleaseAll();
		}

		private static void ReleaseAll()
		{
			for (int i = 0; i < Covers.Count; i++)
			{
				Release(Covers[i]);
			}
			Covers.Clear();
		}

		private static void Release(Cover cover)
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				NCard node = cover.Node;
				if (cover.Has && FxKit.Alive((GodotObject)(object)node) && node.Model == cover.Card && FxKit.Alive((GodotObject)(object)node.Body) && Same(((CanvasItem)node.Body).Modulate, cover.Wrote))
				{
					((CanvasItem)node.Body).Modulate = cover.Base;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandShade.Release: " + ex.Message);
			}
			cover.Node = null;
			cover.Has = false;
		}
	}
