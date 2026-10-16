// namespace: SilentFX.Powers
// type: FogKit (class)
// source lines: 9328-9417
	public static class FogKit
	{
		private static readonly StringName POrigin = StringName.op_Implicit("origin");

		private static readonly StringName PSize = StringName.op_Implicit("size");

		public static float ScaleOf(Node parent)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				CanvasItem val = (CanvasItem)(object)((parent is CanvasItem) ? parent : null);
				if (val != null && FxKit.Alive((GodotObject)(object)val) && ((Node)val).IsInsideTree())
				{
					Transform2D globalTransform = val.GetGlobalTransform();
					Vector2 scale = globalTransform.Scale;
					float x = scale.Abs().X;
					if (x > 0.2f && x < 5f)
					{
						return x;
					}
				}
			}
			catch
			{
			}
			return 1f;
		}

		public static Node2D Quad(Node parent, string name, out ColorRect quad, out ShaderMaterial mat)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected Obj, but got Unknown
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			quad = null;
			mat = null;
			if (!FxKit.Alive((GodotObject)(object)parent))
			{
				return null;
			}
			ShaderMaterial val = ShaderLib.NewMaterial("toxic_fog");
			if (val == null)
			{
				return null;
			}
			Node2D val2 = new Node2D
			{
				Name = StringName.op_Implicit(name)
			};
			parent.AddChild((Node)(object)val2, false, (InternalMode)0);
			if (!((Node)val2).IsInsideTree())
			{
				((Node)val2).QueueFree();
				return null;
			}
			val.SetShaderParameter(StringName.op_Implicit("intensity"), Variant.op_Implicit(0f));
			quad = FxKit.Quad(new Vector2(64f, 64f), (Material)(object)val);
			((Node)val2).AddChild((Node)(object)quad, false, (InternalMode)0);
			mat = val;
			return val2;
		}

		public static void Place(Node2D holder, ColorRect quad, ShaderMaterial mat, Vector2 topLeft, Vector2 size)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)holder) && FxKit.Alive((GodotObject)(object)quad))
			{
				float num = ScaleOf(((Node)holder).GetParent());
				holder.GlobalPosition = topLeft;
				((Control)quad).Size = size / num;
				if (FxKit.Alive((GodotObject)(object)mat))
				{
					mat.SetShaderParameter(POrigin, Variant.op_Implicit(topLeft));
					mat.SetShaderParameter(PSize, Variant.op_Implicit(size));
				}
			}
		}
	}
