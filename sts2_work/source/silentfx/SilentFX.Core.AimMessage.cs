// namespace: SilentFX.Core
// type: AimMessage (struct)
// source lines: 31288-31385
	internal struct AimMessage : INetMessage, IPacketSerializable
	{
		internal const byte Offer = 1;

		internal const byte Ack = 2;

		internal const byte Go = 3;

		internal const byte Snapshot = 4;

		internal byte Kind;

		internal ulong Seed;

		internal ulong Roster;

		internal ulong Epoch;

		internal ulong Round;

		internal ulong Generation;

		internal ulong Sequence;

		internal ulong Source;

		internal int Room;

		internal int Act;

		internal bool Active;

		internal bool HasCard;

		internal bool InPlayZone;

		internal NetCombatCard Card;

		internal uint? Target;

		public bool ShouldBroadcast => false;

		public bool ShouldBuffer => false;

		public NetTransferMode Mode => (NetTransferMode)2;

		public LogLevel LogLevel => (LogLevel)2;

		public void Serialize(PacketWriter writer)
		{
			writer.WriteByte(Kind, 8);
			writer.WriteULong(Seed, 64);
			writer.WriteInt(Room, 32);
			writer.WriteInt(Act, 32);
			writer.WriteULong(Roster, 64);
			writer.WriteULong(Epoch, 64);
			writer.WriteULong(Round, 64);
			writer.WriteULong(Generation, 64);
			writer.WriteULong(Sequence, 64);
			writer.WriteULong(Source, 64);
			writer.WriteBool(Active);
			writer.WriteBool(HasCard);
			writer.WriteBool(InPlayZone);
			if (HasCard)
			{
				Card.Serialize(writer);
			}
			writer.WriteBool(Target.HasValue);
			if (Target.HasValue)
			{
				writer.WriteUInt(Target.Value, 32);
			}
		}

		public void Deserialize(PacketReader reader)
		{
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			Kind = reader.ReadByte(8);
			Seed = reader.ReadULong(64);
			Room = reader.ReadInt(32);
			Act = reader.ReadInt(32);
			Roster = reader.ReadULong(64);
			Epoch = reader.ReadULong(64);
			Round = reader.ReadULong(64);
			Generation = reader.ReadULong(64);
			Sequence = reader.ReadULong(64);
			Source = reader.ReadULong(64);
			Active = reader.ReadBool();
			HasCard = reader.ReadBool();
			InPlayZone = reader.ReadBool();
			Card = default;
			if (HasCard)
			{
				Card.Deserialize(reader);
			}
			Target = (reader.ReadBool() ? new uint?(reader.ReadUInt(32)) : ((uint?)null));
		}
	}
