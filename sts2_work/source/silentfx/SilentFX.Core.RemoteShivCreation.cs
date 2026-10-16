// namespace: SilentFX.Core
// type: RemoteShivCreation (class)
// source lines: 42011-42117
	internal static class RemoteShivCreation
	{
		internal sealed class Batch
		{
			internal int Made;

			internal int Shown;

			internal int Add(int count)
			{
				int made = Made;
				Made += count;
				return made;
			}

			internal int ShowCloak(int count, int start, int expected, out int slot)
			{
				int num = ((start == 0) ? Math.Clamp(Math.Max(count, expected), 1, 4) : (start + count));
				slot = Shown;
				if (num <= Shown)
				{
					return 0;
				}
				Shown = num;
				return num - slot;
			}
		}

		internal sealed class Frame
		{
			internal object Room;

			internal CardPlay Play;

			internal CardModel Card;

			internal Player Owner;

			internal Creature Target;

			private readonly Dictionary<Player, Batch> Batches = new Dictionary<Player, Batch>(ReferenceEqualityComparer.Instance);

			internal Batch For(Player recipient)
			{
				if (!Batches.TryGetValue(recipient, out var value))
				{
					value = (Batches[recipient] = new Batch());
				}
				return value;
			}
		}

		private static readonly ConditionalWeakTable<object, Frame> Frames = new ConditionalWeakTable<object, Frame>();

		internal static void Begin(object invocation, CardPlay play, object room)
		{
			if (invocation != null)
			{
				object obj;
				if (play == null)
				{
					obj = null;
				}
				else
				{
					CardModel card = play.Card;
					obj = ((card != null) ? card.Owner : null);
				}
				if (obj != null && room != null)
				{
					Frames.Remove(invocation);
					Frames.Add(invocation, new Frame
					{
						Room = room,
						Play = play,
						Card = play.Card,
						Owner = play.Card.Owner,
						Target = play.Target
					});
				}
			}
		}

		internal static bool TryGet(object invocation, CardModel card, object room, out Frame frame)
		{
			frame = null;
			if (invocation == null || ((card != null) ? card.Owner : null) == null || !Frames.TryGetValue(invocation, out var value) || value.Card != card || value.Owner != card.Owner || room == null || value.Room != room)
			{
				return false;
			}
			frame = value;
			return true;
		}

		internal static void Finish(object invocation, CardPlay play)
		{
			if (invocation != null && Frames.TryGetValue(invocation, out var value) && value.Play == play)
			{
				Frames.Remove(invocation);
			}
		}

		internal static void Reset()
		{
			Frames.Clear();
		}
	}
