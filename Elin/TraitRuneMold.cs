public class TraitRuneMold : TraitCrafter
{
	public override string IdSource => "RuneMold";

	public override string CrafterTitle => "invSculpt";

	public override AnimeID IdAnimeProgress => AnimeID.Shiver;

	public override string idSoundProgress => "craft_sculpt";

	public override bool HoldAsDefaultInteraction => false;

	public override bool IsConsumeIng => false;

	public override bool CloseOnComplete => true;

	public virtual Rarity MaxRarity => Rarity.Normal;

	public override bool IsIngredient(string cat, Card c)
	{
		if (c.rarity > MaxRarity || c.c_isImportant)
		{
			return false;
		}
		return base.IsIngredient(cat, c);
	}

	public override bool ShouldConsumeIng(SourceRecipe.Row item, int index)
	{
		return false;
	}
}
