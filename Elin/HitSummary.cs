using System.Collections.Generic;
using UnityEngine;

public class HitSummary : EClass
{
	public int money;

	public int count;

	public int countValid;

	public Recipe recipe;

	public List<IInspect> targets = new List<IInspect>();

	public List<InspectGroup> groups = new List<InspectGroup>();

	public Thing factory;

	public bool hasFactory;

	public void Clear()
	{
		money = (count = (countValid = 0));
		targets.Clear();
		groups.Clear();
		SetRecipe(recipe);
	}

	public void SetRecipe(Recipe r)
	{
		recipe = r;
		hasFactory = true;
		if (r != null && !r.UseStock && r.source.NeedFactory)
		{
			PropSet propSet = EClass._map.Installed.cardMap.TryGetValue(r.source.idFactory);
			if ((propSet == null || propSet.Count == 0) && EClass.pc.things.Find((Thing t) => t.id == r.source.idFactory) == null)
			{
				hasFactory = false;
			}
		}
	}

	public bool CanExecute()
	{
		if (EClass.debug.ignoreBuildRule)
		{
			return true;
		}
		if (!hasFactory || EClass.pc.GetCurrency() < money)
		{
			return false;
		}
		if (EClass.screen.tileSelector.processing)
		{
			return true;
		}
		if (recipe != null)
		{
			if (!recipe.UseStock)
			{
				Dictionary<Thing, int> dictionary = new Dictionary<Thing, int>();
				foreach (Recipe.Ingredient ingredient in recipe.ingredients)
				{
					if (ingredient.thing != null)
					{
						if (!dictionary.ContainsKey(ingredient.thing))
						{
							dictionary.Add(ingredient.thing, 0);
						}
						dictionary[ingredient.thing] += ingredient.req * countValid;
					}
				}
				foreach (KeyValuePair<Thing, int> item in dictionary)
				{
					if (item.Key.Num < item.Value)
					{
						return false;
					}
				}
				foreach (Recipe.Ingredient ingredient2 in recipe.ingredients)
				{
					if (!ingredient2.optional)
					{
						if (ingredient2.thing == null)
						{
							return false;
						}
						if (ingredient2.thing.Num < ingredient2.req * countValid)
						{
							return false;
						}
					}
				}
			}
			else if (recipe.ingredients[0].thing == null || recipe.ingredients[0].thing.Num < countValid)
			{
				return false;
			}
		}
		return true;
	}

	public void Execute()
	{
		if (!EClass.debug.ignoreBuildRule)
		{
			EClass.pc.ModCurrency(-money);
		}
		if (recipe == null || (recipe.UseStock && !recipe.VirtualBlock))
		{
			return;
		}
		if (recipe.RequireIngredients)
		{
			BuildMenu.Instance.info1.lastMats[recipe.id] = recipe.ingredients[0].mat;
		}
		if (!recipe.tileType.CanInstaComplete && !EClass.player.instaComplete)
		{
			return;
		}
		foreach (Recipe.Ingredient ingredient in recipe.ingredients)
		{
			Thing thing = (recipe.UseStock ? recipe.ingredients[0].thing : ingredient.RefreshThing());
			if (thing == null)
			{
				if (!EClass.debug.enable)
				{
					Debug.LogError("no ing");
				}
				break;
			}
			int num = ingredient.req * countValid;
			int num2 = ((thing.Num >= num) ? num : thing.Num);
			if (!EClass.debug.godBuild || recipe.UseStock)
			{
				thing.ModNum(-num2);
			}
		}
	}
}
