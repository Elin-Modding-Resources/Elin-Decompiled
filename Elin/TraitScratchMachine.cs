public class TraitScratchMachine : TraitCrafter
{
	public override string IdSource => "Scratch";

	public override string CrafterTitle => "invScratch";

	public override AnimeID IdAnimeProgress => AnimeID.Shiver;

	public override string idSoundProgress => "craft_scratch";

	public override int GetDuration(AI_UseCrafter ai, int costSp)
	{
		return GetSource(ai).time;
	}
}
