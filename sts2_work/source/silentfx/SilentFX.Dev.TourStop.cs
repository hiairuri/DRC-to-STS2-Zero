// namespace: SilentFX.Dev
// type: TourStop (class)
// source lines: 29539-29550
	public sealed class TourStop
	{
		public int Number;

		public string Name = "";

		public string Group = "";

		public readonly List<TourLine> Lines = new List<TourLine>();

		public string Cue = "";
	}
