public class TraitRollingFortune : TraitCrafter
{
	public override string IdSource => "Fortune";

	public override string CrafterTitle => "invRoll";

	public override bool CanUseFromInventory => false;

	public override AnimeType animeType => AnimeType.Microwave;

	public override string idSoundProgress => "fortuneroll";

	public override TileMode tileMode => TileMode.SignalAnime;

	public override int GetDuration(AI_UseCrafter ai, int costSp)
	{
		return GetSource(ai).time;
	}

	public override void OnEndAI(AI_UseCrafter ai)
	{
		if (EClass.pc.isDead || !owner.ExistsOnMap)
		{
			return;
		}
		foreach (Card item in owner.pos.ListThings<TraitFortuneBall>(onlyInstalled: false))
		{
			item.Destroy();
		}
	}
}
