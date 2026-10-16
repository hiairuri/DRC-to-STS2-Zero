// namespace: SilentFX.Powers
// type: FxAnchor (class)
// source lines: 6286-6395
	public sealed class FxAnchor
	{
		public object Owner;

		public Node BackParent;

		public Node FrontParent;

		public Func<Vector2> Feet;

		public Func<Vector2> Chest;

		public Func<Vector2> BodySize;

		public Func<CanvasItem> Body;

		public Func<List<FxTarget>> Targets;

		public Action<List<FxTarget>> FillTargets;

		public Func<bool> IsValid;

		public Func<float> Facing;

		public float Side()
		{
			try
			{
				return (Facing != null && Facing() < 0f) ? (-1f) : 1f;
			}
			catch
			{
				return 1f;
			}
		}

		public Vector2 SafeBodySize()
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Vector2 val = ((BodySize != null) ? BodySize() : Vector2.Zero);
				return (Vector2)((val.X < 40f || val.Y < 40f) ? new Vector2(200f, 300f) : val);
			}
			catch
			{
				return new Vector2(200f, 300f);
			}
		}

		public List<FxTarget> SafeTargets()
		{
			try
			{
				return Targets?.Invoke() ?? new List<FxTarget>();
			}
			catch
			{
				return new List<FxTarget>();
			}
		}

		public void SafeTargets(List<FxTarget> into)
		{
			if (into == null)
			{
				return;
			}
			try
			{
				if (FillTargets != null)
				{
					FillTargets(into);
					return;
				}
				List<FxTarget> list = Targets?.Invoke();
				into.Clear();
				if (list != null)
				{
					into.AddRange(list);
				}
			}
			catch
			{
				into.Clear();
			}
		}

		public bool Valid()
		{
			try
			{
				return IsValid == null || IsValid();
			}
			catch
			{
				return false;
			}
		}
	}
