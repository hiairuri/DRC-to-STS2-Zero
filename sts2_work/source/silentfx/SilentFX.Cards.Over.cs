// namespace: SilentFX.Cards
// type: Over (class)
// source lines: 46681-46747
	internal static class Over
	{
		public static Node Ui(CardFxContext c, double life = 1.5)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Expected Obj, but got Unknown
			Node val = (FxKit.Alive((GodotObject)(object)c.UiParent) ? c.UiParent : null);
			if (val == null)
			{
				return c.FrontParent;
			}
			Node2D val2 = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_OverUi"),
				ZIndex = 10
			};
			val.AddChild((Node)(object)val2, false, (InternalMode)0);
			if (!((Node)val2).IsInsideTree())
			{
				((Node)val2).QueueFree();
				return c.FrontParent;
			}
			FxKit.FreeAfter((Node)(object)val2, life);
			return (Node)(object)val2;
		}

		public static void HandSpan(CardFxContext c, out Vector2 left, out Vector2 right)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			left = new Vector2(c.View.X * 0.28f, c.View.Y * 0.9f);
			right = new Vector2(c.View.X * 0.72f, c.View.Y * 0.9f);
			List<HandCards.Entry> list = new List<HandCards.Entry>();
			HandCards.Fill(list);
			if (list.Count == 0)
			{
				return;
			}
			float num = float.MaxValue;
			float num2 = float.MinValue;
			float num3 = 0f;
			foreach (HandCards.Entry item in list)
			{
				Vector2 origin = ((CanvasItem)item.Node).GetGlobalTransform().Origin;
				num = Mathf.Min(num, origin.X);
				num2 = Mathf.Max(num2, origin.X);
				num3 += origin.Y;
			}
			num3 /= (float)list.Count;
			left = new Vector2(num - 110f, num3);
			right = new Vector2(num2 + 110f, num3);
		}
	}
