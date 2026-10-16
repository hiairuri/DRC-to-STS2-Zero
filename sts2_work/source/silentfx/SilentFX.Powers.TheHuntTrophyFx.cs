// namespace: SilentFX.Powers
// type: TheHuntTrophyFx (class)
// source lines: 13487-13511
	public sealed class TheHuntTrophyFx : BasicPowerFx
	{
		protected override string NodeName => "HuntTrophy";

		protected override bool UseBack => false;

		protected override void Build(bool animateIn)
		{
			if (animateIn)
			{
				object owner = Anchor.Owner;
				TheHuntFx.Trophy((Creature)((owner is Creature) ? owner : null));
			}
		}

		protected override void OnStacked()
		{
			object owner = Anchor.Owner;
			TheHuntFx.Trophy((Creature)((owner is Creature) ? owner : null));
		}

		protected override void Layout()
		{
		}
	}
