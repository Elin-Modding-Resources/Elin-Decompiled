public class TraitTile : Trait
{
	public virtual TileRow source => null;

	public virtual string suffix => "";

	public virtual bool ConsumeOnUse => true;

	public override bool CanExtendBuild => true;

	public override bool CanBuildInTown => false;

	public override void SetName(ref string s)
	{
		s = source.GetName();
	}

	public override Recipe GetRecipe()
	{
		return Recipe.Create(RecipeManager.dict[source.RecipeID + suffix], owner.material.id);
	}

	public override Recipe GetBuildModeRecipe()
	{
		return Recipe.Create(RecipeManager.dict[source.RecipeID + suffix], -1, owner.Thing);
	}
}
