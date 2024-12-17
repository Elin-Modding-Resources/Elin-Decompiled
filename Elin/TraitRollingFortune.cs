public class TraitRollingFortune : TraitCrafter
{
	public override string IdSource => "Fortune";

	public override string CrafterTitle => "invRoll";

	public override AnimeType animeType => AnimeType.Microwave;

	public override string idSoundProgress => "fortuneroll";

	public override TileMode tileMode => TileMode.SignalAnime;

	public override int GetDuration(AI_UseCrafter ai, int costSp)
	{
		return GetSource(ai).time;
	}
}
