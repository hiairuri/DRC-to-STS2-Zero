// namespace: SilentFX.Cards
// type: CardFxContext (class)
// source lines: 49173-49248
	public sealed class CardFxContext
	{
		public CardModel Card;

		internal bool Remote;

		internal Vector2 AimPosition;

		public Vector2? ShivReturnPoint;

		public Node BackParent;

		public Node FrontParent;

		public Vector2 Feet;

		public Vector2 Chest;

		public Vector2 Body;

		public bool HasTarget;

		public Vector2 TargetFeet;

		public Vector2 TargetChest;

		public float TargetWidth = 200f;

		public float TargetHeight = 240f;

		public float Facing = 1f;

		public float TargetSide = 1f;

		public Creature Target;

		public int TargetPoison;

		public List<FxTarget> Enemies = new List<FxTarget>();

		public Vector2 View = new Vector2(1920f, 1080f);

		public HeldSession Held;

		public Node UiParent;

		public Vector2 ExhaustPile = new Vector2(1830f, 790f);

		public Vector2 Hand
		{
			get
			{
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				return new Vector2(View.X * 0.5f, View.Y * 0.93f);
			}
		}

		public Vector2 WeaponHand
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				return Chest + new Vector2(Body.X * 0.15f * Facing, Body.Y * 0.22f);
			}
		}

		public CardFxContext()
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		}
	}
