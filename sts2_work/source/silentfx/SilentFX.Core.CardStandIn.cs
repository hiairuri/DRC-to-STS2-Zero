// namespace: SilentFX.Core
// type: CardStandIn (class)
// source lines: 38333-38589
	public static class CardStandIn
	{
		private static readonly List<(NCard Node, CardModel Model)> Hidden = new List<(NCard, CardModel)>();

		private static readonly Dictionary<NCard, (CardModel Model, MouseFilterEnum Filter, MouseBehaviorRecursiveEnum Behavior)> Copies = new Dictionary<NCard, (CardModel, MouseFilterEnum, MouseBehaviorRecursiveEnum)>(ReferenceEqualityComparer.Instance);

		public static NCard Make(CardModel card, Node parent)
		{
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			NCard val = null;
			try
			{
				if (card == null || !FxKit.Alive((GodotObject)(object)parent))
				{
					DevLog.Warn("CardStandIn.Make: no card or parent");
					return null;
				}
				val = NCard.Create(card, (ModelVisibility)1);
				if (!FxKit.Alive((GodotObject)(object)val))
				{
					DevLog.Warn("CardStandIn.Make: no card node");
					return null;
				}
				parent.AddChild((Node)(object)val, false, (InternalMode)0);
				val.UpdateVisuals((PileType)2, (CardPreviewMode)1);
				Copies[val] = (card, ((Control)val).MouseFilter, ((Control)val).MouseBehaviorRecursive);
				((Control)val).MouseFilter = (MouseFilterEnum)2;
				((Control)val).MouseBehaviorRecursive = (MouseBehaviorRecursiveEnum)1;
				return val;
			}
			catch (Exception ex)
			{
				DevLog.Warn("CardStandIn.Make: " + ex);
				Free(val);
				return null;
			}
		}

		public static void Free(NCard standIn)
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			if (standIn == null)
			{
				return;
			}
			try
			{
				bool flag = Copies.Remove(standIn, out (CardModel, MouseFilterEnum, MouseBehaviorRecursiveEnum) value);
				if (!FxKit.Alive((GodotObject)(object)standIn) || (flag && standIn.Model != value.Item1))
				{
					return;
				}
				Node parent = ((Node)standIn).GetParent();
				try
				{
					if (flag)
					{
						((Control)standIn).MouseFilter = value.Item2;
						((Control)standIn).MouseBehaviorRecursive = value.Item3;
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("CardStandIn.Free (input): " + ex);
				}
				try
				{
					GodotTreeExtensions.QueueFreeSafely((Node)(object)standIn);
				}
				finally
				{
					if (GodotObject.IsInstanceValid((GodotObject)(object)standIn) && FxKit.Alive((GodotObject)(object)parent) && ((Node)standIn).GetParent() == parent)
					{
						parent.RemoveChild((Node)(object)standIn);
					}
				}
			}
			catch (Exception ex2)
			{
				DevLog.Warn("CardStandIn.Free: " + ex2);
			}
		}

		public static void Hide(NCard real)
		{
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (!FxKit.Alive((GodotObject)(object)real))
				{
					return;
				}
				CardModel model = real.Model;
				for (int num = Hidden.Count - 1; num >= 0; num--)
				{
					if (Hidden[num].Node == real)
					{
						Hidden.RemoveAt(num);
					}
				}
				Hidden.Add((real, model));
				((CanvasItem)real).Modulate = new Color(1f, 1f, 1f, 0f);
			}
			catch (Exception ex)
			{
				DevLog.Warn("CardStandIn.Hide: " + ex);
			}
		}

		public static void Dim(NCard real, float alpha)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (FxKit.Alive((GodotObject)(object)real))
				{
					Hide(real);
					((CanvasItem)real).Modulate = new Color(1f, 1f, 1f, Mathf.Clamp(alpha, 0f, 1f));
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("CardStandIn.Dim: " + ex);
			}
		}

		public static void Pose(NCard node, out Vector2 centre, out float rotation, out Vector2 scale)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			centre = Vector2.Zero;
			rotation = 0f;
			scale = Vector2.One;
			try
			{
				if (FxKit.Alive((GodotObject)(object)node))
				{
					Transform2D globalTransform = ((CanvasItem)node).GetGlobalTransform();
					centre = globalTransform.Origin;
					rotation = globalTransform.Rotation;
					Vector2 val = globalTransform.Scale * (FxKit.Alive((GodotObject)(object)node.Body) ? node.Body.Scale : Vector2.One);
					scale = val.Abs();
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("CardStandIn.Pose: " + ex);
			}
		}

		public static void Place(NCard standIn, Vector2 centre, float rotation, Vector2 scale)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (FxKit.Alive((GodotObject)(object)standIn))
				{
					Vector2 val;
					if (!FxKit.Alive((GodotObject)(object)standIn.Body))
					{
						val = Vector2.One;
					}
					else
					{
						Vector2 scale2 = standIn.Body.Scale;
						val = scale2.Abs();
					}
					Vector2 val2 = val;
					Vector2 val3 = new Vector2(scale.X / Mathf.Max(0.001f, val2.X), scale.Y / Mathf.Max(0.001f, val2.Y));
					Transform2D val4 = new Transform2D(rotation, val3, 0f, centre);
					Node parent = ((Node)standIn).GetParent();
					CanvasItem val5 = (CanvasItem)(object)((parent is CanvasItem) ? parent : null);
					if (val5 != null && FxKit.Alive((GodotObject)(object)val5))
					{
						Transform2D globalTransform = val5.GetGlobalTransform();
						val4 = globalTransform.AffineInverse() * val4;
					}
					((Control)standIn).Position = val4.Origin;
					((Control)standIn).Rotation = val4.Rotation;
					((Control)standIn).Scale = val4.Scale;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("CardStandIn.Place: " + ex);
			}
		}

		public static void Show(NCard real)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (FxKit.Alive((GodotObject)(object)real))
				{
					((CanvasItem)real).Modulate = Colors.White;
				}
				for (int num = Hidden.Count - 1; num >= 0; num--)
				{
					if (Hidden[num].Node == real)
					{
						Hidden.RemoveAt(num);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("CardStandIn.Show: " + ex);
			}
		}

		public static void Reset()
		{
			for (int num = Hidden.Count - 1; num >= 0; num--)
			{
				Show(Hidden[num].Node);
			}
			Hidden.Clear();
		}
	}
