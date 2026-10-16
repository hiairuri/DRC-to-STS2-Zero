// namespace: SilentFX.Dev
// type: TourDocument (class)
// source lines: 28662-28677
	internal sealed class TourDocument
	{
		internal readonly List<TourDefinition> Stops = new List<TourDefinition>();

		internal readonly List<TourScreen> Intro = new List<TourScreen>();

		internal readonly List<TourScreen> Outro = new List<TourScreen>();

		internal TourDefinition Finale;

		internal string FinaleText;

		internal (DateTime Time, long Length) ScriptStamp;

		internal (DateTime Time, long Length) WordsStamp;
	}
