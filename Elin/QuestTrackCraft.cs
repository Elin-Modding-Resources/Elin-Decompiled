using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class QuestTrackCraft : Quest
{
	[JsonProperty]
	public string idRecipe;

	public RecipeSource _recipe;

	public List<Recipe.Ingredient> ingredients;

	public RecipeSource recipe => _recipe ?? (_recipe = RecipeManager.Get(idRecipe));

	public override bool CanAbandon => true;

	public override string GetTitle()
	{
		return "trackCraft_title".lang(recipe.Name, recipe.NameFactory);
	}

	public void SetRecipe(Recipe r)
	{
		idRecipe = r.id;
	}

	public override string GetDetail(bool onJournal = false)
	{
		if (ingredients == null)
		{
			ingredients = recipe.GetIngredients();
		}
		string text = "";
		for (int i = 0; i < ingredients.Count; i++)
		{
			Recipe.Ingredient ingredient = ingredients[i];
			if (!text.IsEmpty())
			{
				text += Environment.NewLine;
			}
			ThingStack thingStack = EClass._map.Stocked.ListThingStack(ingredient, StockSearchMode.All);
			_ = thingStack.list;
			Thing thing = null;
			foreach (Thing item in thingStack.list)
			{
				if (thing == null || item.Num > thing.Num)
				{
					thing = item;
				}
			}
			int num = thing?.Num ?? 0;
			if (num >= ingredient.req)
			{
				text += "trackCraft_met".lang().TagColor(FontColor.Good);
			}
			text = text + (ingredient.optional ? "+" : "") + ingredient.GetName() + " " + num + " / " + ingredient.req;
		}
		return text;
	}
}
