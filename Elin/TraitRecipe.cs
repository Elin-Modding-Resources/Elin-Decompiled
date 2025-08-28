public class TraitRecipe : TraitScroll
{
	public override string IdNoRestock => owner.id + "_" + ID;

	public string ID => owner.GetStr(53);

	public override bool CanBeShipped => false;

	public override bool CanBeDestroyed => false;

	public RecipeSource recipe => RecipeManager.dict.TryGetValue(ID) ?? RecipeManager.dict.TryGetValue("bait");

	public override void OnCreate(int lv)
	{
		owner.SetStr(53, RecipeManager.GetRandomRecipe(lv));
	}

	public override void OnRead(Chara c)
	{
		EClass.player.recipes.Add(recipe.id);
		owner.ModNum(-1);
	}

	public override void SetName(ref string s)
	{
		int num = EClass.player.recipes.knownRecipes.TryGetValue(ID, 0);
		s = "_recipe".lang(recipe?.Name.ToTitleCase() ?? "???", s) + ((num == 0) ? "" : "recipe_learnt".lang(num.ToString() ?? ""));
	}

	public override bool CanStackTo(Thing to)
	{
		return ID == to.GetStr(53);
	}

	public override void WriteNote(UINote n, bool identified)
	{
		if (recipe.NeedFactory)
		{
			n.AddText("isCraftedAt".lang(recipe.NameFactory.ToTitleCase(wholeText: true)));
		}
	}

	public override int GetValue()
	{
		return owner.sourceCard.value * (100 + recipe.row.LV * 15) / 100;
	}
}
