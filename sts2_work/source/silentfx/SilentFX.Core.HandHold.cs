// namespace: SilentFX.Core
// type: HandHold (class)
// source lines: 39120-39507
	public static class HandHold
	{
		private static int _size;

		private static float _remaining;

		private static bool _squeezed;

		private static float _squeezeRemaining;

		private static bool _lined;

		private static float _lineRemaining;

		private static float _lineSpacing;

		private static float _lineLift;

		[ThreadStatic]
		private static bool _insideLinePosition;

		private static bool _registered;

		public static bool Active
		{
			get
			{
				if (_size > 0)
				{
					return _remaining > 0f;
				}
				return false;
			}
		}

		public static bool Lined
		{
			get
			{
				if (_lined)
				{
					return _lineRemaining > 0f;
				}
				return false;
			}
		}

		internal static void Init()
		{
			if (!_registered)
			{
				Ticker.Add(Update);
				_registered = true;
			}
		}

		public static void Line(float spacing, float seconds, float lift = 0f)
		{
			try
			{
				if (!float.IsFinite(spacing) || !float.IsFinite(lift) || !(seconds > 0f) || !float.IsFinite(seconds))
				{
					EndLine();
					Refresh();
					return;
				}
				Init();
				_lineSpacing = spacing;
				_lineLift = lift;
				_lineRemaining = seconds;
				_lined = true;
				Refresh();
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandHold.Line: " + ex);
				EndLine();
				Refresh();
			}
		}

		public static void Hold(int size, float seconds)
		{
			try
			{
				if (size <= 0 || !(seconds > 0f) || !float.IsFinite(seconds))
				{
					Release();
					return;
				}
				Init();
				_size = Math.Clamp(size, 1, 10);
				_remaining = seconds;
				Refresh();
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandHold.Hold: " + ex);
				Release();
			}
		}

		public static void Release()
		{
			_size = 0;
			_remaining = 0f;
			EndLine();
			Refresh();
		}

		internal static void RaiseSize(ref int handSize)
		{
			if (Active && handSize < _size)
			{
				handSize = _size;
			}
		}

		internal static bool TryLinePosition(int handSize, int cardIndex, out Vector2 position)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			position = Vector2.Zero;
			if (_insideLinePosition || !Lined || handSize <= 0)
			{
				return false;
			}
			position = LinePosition(handSize, cardIndex, _lineSpacing, _lineLift);
			return true;
		}

		private static Vector2 LinePosition(int size, int index, float spacing, float lift)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			bool insideLinePosition = _insideLinePosition;
			_insideLinePosition = true;
			try
			{
				Vector2 position = HandPosHelper.GetPosition(size, 0);
				Vector2 position2 = HandPosHelper.GetPosition(size, size - 1);
				Vector2 position3 = HandPosHelper.GetPosition(size, (size - 1) / 2);
				return new Vector2((position.X + position2.X) * 0.5f + ((float)index - (float)(size - 1) / 2f) * spacing, position3.Y - lift);
			}
			finally
			{
				_insideLinePosition = insideLinePosition;
			}
		}

		private static void EndLine()
		{
			_lined = false;
			_lineRemaining = 0f;
			_lineSpacing = 0f;
			_lineLift = 0f;
		}

		public static void Squeeze(float amount = 0.8f, float hold = 0.12f)
		{
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				NPlayerHand instance = NPlayerHand.Instance;
				if (!FxKit.Alive((GodotObject)(object)instance) || !((Node)instance).IsInsideTree())
				{
					return;
				}
				amount = (float.IsFinite(amount) ? Mathf.Clamp(amount, 0f, 1f) : 0.8f);
				hold = (float.IsFinite(hold) ? Mathf.Max(0f, hold) : 0.12f);
				List<NHandCardHolder> list = new List<NHandCardHolder>();
				Vector2 val = Vector2.Zero;
				foreach (NHandCardHolder activeHolder in instance.ActiveHolders)
				{
					if (FxKit.Alive((GodotObject)(object)activeHolder))
					{
						list.Add(activeHolder);
						val += activeHolder.TargetPosition;
					}
				}
				if (list.Count == 0)
				{
					return;
				}
				Init();
				val /= (float)list.Count;
				_squeezed = true;
				_squeezeRemaining = hold;
				foreach (NHandCardHolder item in list)
				{
					Vector2 targetPosition = item.TargetPosition;
					item.SetTargetPosition(targetPosition.Lerp(val, amount));
					item.SetTargetAngle(0f);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandHold.Squeeze: " + ex);
				_squeezed = false;
				Refresh();
			}
		}

		public static bool Slot(int size, int index, out Vector2 centre, out float angle, out Vector2 scale)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			centre = Vector2.Zero;
			angle = 0f;
			scale = Vector2.One;
			try
			{
				NPlayerHand instance = NPlayerHand.Instance;
				if (!FxKit.Alive((GodotObject)(object)instance) || !((Node)instance).IsInsideTree())
				{
					return false;
				}
				Control cardHolderContainer = instance.CardHolderContainer;
				if (!FxKit.Alive((GodotObject)(object)cardHolderContainer) || !((Node)cardHolderContainer).IsInsideTree())
				{
					return false;
				}
				size = Math.Clamp(size, 1, 10);
				RaiseSize(ref size);
				index = Math.Clamp(index, 0, size - 1);
				Transform2D globalTransform = ((CanvasItem)cardHolderContainer).GetGlobalTransform();
				centre = globalTransform * HandPosHelper.GetPosition(size, index);
				angle = globalTransform.Rotation + Mathf.DegToRad(HandPosHelper.GetAngle(size, index));
				Vector2 val = globalTransform.Scale * HandPosHelper.GetScale(size);
				scale = val.Abs();
				return true;
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandHold.Slot: " + ex);
				return false;
			}
		}

		public static bool LineSlot(int size, int index, float spacing, float lift, out Vector2 centre, out Vector2 scale)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			centre = Vector2.Zero;
			scale = Vector2.One;
			try
			{
				if (!float.IsFinite(spacing) || !float.IsFinite(lift))
				{
					return false;
				}
				NPlayerHand instance = NPlayerHand.Instance;
				if (!FxKit.Alive((GodotObject)(object)instance) || !((Node)instance).IsInsideTree())
				{
					return false;
				}
				Control cardHolderContainer = instance.CardHolderContainer;
				if (!FxKit.Alive((GodotObject)(object)cardHolderContainer) || !((Node)cardHolderContainer).IsInsideTree())
				{
					return false;
				}
				size = Math.Clamp(size, 1, 10);
				RaiseSize(ref size);
				index = Math.Clamp(index, 0, size - 1);
				Transform2D globalTransform = ((CanvasItem)cardHolderContainer).GetGlobalTransform();
				centre = globalTransform * LinePosition(size, index, spacing, lift);
				Vector2 val = globalTransform.Scale * HandPosHelper.GetScale(size);
				scale = val.Abs();
				return true;
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandHold.LineSlot: " + ex);
				return false;
			}
		}

		public static void Reset()
		{
			_squeezed = false;
			_squeezeRemaining = 0f;
			Release();
		}

		private static void Refresh()
		{
			try
			{
				NPlayerHand instance = NPlayerHand.Instance;
				if (FxKit.Alive((GodotObject)(object)instance) && ((Node)instance).IsInsideTree())
				{
					instance.ForceRefreshCardIndices();
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandHold.Refresh: " + ex);
			}
		}

		private static void Update(double realDt)
		{
			if (_size == 0 && !_squeezed && !_lined)
			{
				return;
			}
			try
			{
				float num = (float)(realDt * (double)Mathf.Clamp((float)Engine.TimeScale, 0f, 4f));
				bool flag = false;
				if (_size > 0 && (_remaining -= num) <= 0f)
				{
					_size = 0;
					_remaining = 0f;
					flag = true;
				}
				if (_squeezed && (_squeezeRemaining -= num) <= 0f)
				{
					_squeezed = false;
					flag = true;
				}
				if (_lined && (_lineRemaining -= num) <= 0f)
				{
					EndLine();
					flag = true;
				}
				if (flag)
				{
					Refresh();
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandHold.Update: " + ex);
				Reset();
			}
		}
	}
