// namespace: SilentFX.Core
// type: AttackInfo (class)
// source lines: 32147-32162
	public sealed class AttackInfo
	{
		public CardModel Card;

		public Creature Attacker;

		public int PlannedHits;

		public bool Random;

		public int Landed;

		public ulong BeganAtMs;

		internal object Invocation;
	}
