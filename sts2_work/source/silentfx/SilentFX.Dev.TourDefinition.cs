// namespace: SilentFX.Dev
// type: TourDefinition (class)
// source lines: 28690-28717
	internal sealed class TourDefinition
	{
		internal TourStop Text = new TourStop();

		internal int EnemyHp;

		internal int LowHp;

		internal int Energy;

		internal string Relic = "";

		internal float Slow;

		internal string EnemyTurn = "";

		internal readonly string[] EnemyTurns = new string[3];

		internal List<TourCard> Hand = new List<TourCard>();

		internal List<TourCard> Draw = new List<TourCard>();

		internal List<TourCard> Exhaust = new List<TourCard>();

		internal List<TourCard> NextDraw = new List<TourCard>();

		internal readonly List<TourEnemyRule> EnemyRules = new List<TourEnemyRule>();
	}
