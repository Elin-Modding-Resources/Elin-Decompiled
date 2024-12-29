public class TraitWoodMill : TraitCrafter
{
	public override string IdSource => "WoodMill";

	public override int numIng => 2;

	public override string CrafterTitle => "invWoodMill";

	public override AnimeID IdAnimeProgress => AnimeID.Shiver;

	public override string idSoundProgress => "cook_pound";

	public override ToggleType ToggleType => ToggleType.Custom;

	public override AnimeType animeType => AnimeType.Microwave;

	public override bool AutoTurnOff => true;

	public override bool AutoToggle => false;

	public override void PlayToggleEffect(bool silent)
	{
	}
}
