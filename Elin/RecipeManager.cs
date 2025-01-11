using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class RecipeManager : EClass
{
	public static bool rebuild;

	public static List<RecipeSource> list = new List<RecipeSource>();

	public static Dictionary<string, RecipeSource> dict = new Dictionary<string, RecipeSource>();

	[JsonProperty]
	public HashSet<string> knownIngredients = new HashSet<string>();

	[JsonProperty]
	public HashSet<string> craftedRecipes = new HashSet<string>();

	[JsonProperty]
	public Dictionary<string, int> knownRecipes = new Dictionary<string, int>();

	[JsonProperty]
	public HashSet<string> hoveredRecipes = new HashSet<string>();

	[JsonProperty]
	public HashSet<string> newRecipes = new HashSet<string>();

	[JsonProperty]
	public Dictionary<string, List<int>> lastIngredients = new Dictionary<string, List<int>>();

	public static void BuildList()
	{
		if (!rebuild && list.Count > 0)
		{
			return;
		}
		Debug.Log("Rebuilding recipe list");
		list.Clear();
		dict.Clear();
		foreach (CardRow row in EClass.sources.cards.rows)
		{
			if (!row.isOrigin && (row.factory.IsEmpty() || !(row.factory[0] == "x")))
			{
				Create(row, "", row.isChara ? "-c" : "");
			}
		}
		foreach (SourceBlock.Row row2 in EClass.sources.blocks.rows)
		{
			Create(row2, "Block");
			if (row2.tileType == TileType.Pillar)
			{
				Create(row2, "BridgePillar", "-p");
			}
		}
		foreach (SourceFloor.Row row3 in EClass.sources.floors.rows)
		{
			if (!row3.tag.Contains("noFloor"))
			{
				Create(row3, "Floor");
			}
		}
		foreach (SourceFloor.Row row4 in EClass.sources.floors.rows)
		{
			if (!row4.tag.Contains("noBridge") && !row4.factory.IsEmpty() && row4.factory[0] != "x")
			{
				Create(row4, "Bridge", "-b");
			}
		}
		foreach (SourceObj.Row row5 in EClass.sources.objs.rows)
		{
			Create(row5, "Obj");
		}
		foreach (SourceCellEffect.Row row6 in EClass.sources.cellEffects.rows)
		{
			Create(row6, "Liquid");
		}
		rebuild = false;
	}

	public static void Create(RenderRow row, string type, string suffix = "")
	{
		RecipeSource recipeSource = new RecipeSource();
		recipeSource.id = row.RecipeID + suffix;
		recipeSource.isBridge = type == "Bridge";
		recipeSource.isBridgePillar = type == "BridgePillar";
		recipeSource.type = type;
		recipeSource.row = row;
		recipeSource.isChara = row is SourceChara.Row;
		list.Add(recipeSource);
		dict[recipeSource.id] = recipeSource;
		_ = row.components;
		Recipe.Create(recipeSource).BuildIngredientList();
	}

	public static RecipeSource Get(string id)
	{
		return dict.TryGetValue(id);
	}

	public RecipeSource GetSource(string id)
	{
		return dict.TryGetValue(id);
	}

	public void Add(string id, bool showEffect = true)
	{
		if (id.IsEmpty())
		{
			return;
		}
		RecipeSource recipeSource = Get(id);
		if (recipeSource == null)
		{
			return;
		}
		if (!knownRecipes.ContainsKey(id))
		{
			newRecipes.Add(id);
			knownRecipes[id] = 0;
		}
		knownRecipes[id]++;
		int num = knownRecipes[id];
		if (showEffect)
		{
			EClass.pc.PlaySound("idea");
			EClass.pc.ShowEmo(Emo.idea, 0.5f, skipSame: false);
		}
		EClass.pc.Say("learnRecipe" + ((num == 1) ? "New" : ""), dict[id].Name.ToTitleCase(), num.ToString() ?? "");
		if (num == 1)
		{
			if (recipeSource.NeedFactory)
			{
				EClass.pc.Say("recipe_factory", recipeSource.NameFactory);
			}
			else
			{
				EClass.pc.Say("recipe_nofactory");
			}
		}
		if (recipeSource.row.category == "floor")
		{
			recipeSource = Get(id + "-b");
			if (recipeSource != null && !knownRecipes.ContainsKey(recipeSource.id))
			{
				Add(recipeSource.id, showEffect: false);
			}
			recipeSource = Get(id.Replace("-b", ""));
			if (recipeSource != null && !knownRecipes.ContainsKey(recipeSource.id))
			{
				Add(recipeSource.id, showEffect: false);
			}
		}
	}

	public bool IsKnown(string id)
	{
		if (!EClass.debug.allRecipe)
		{
			return EClass.player.recipes.knownRecipes.ContainsKey(id);
		}
		return true;
	}

	public List<RecipeSource> ListSources(Thing factory, List<RecipeSource> newRecipes = null)
	{
		BuildList();
		List<RecipeSource> list = new List<RecipeSource>();
		foreach (RecipeSource item in RecipeManager.list)
		{
			if (item.isBridgePillar || (factory == null && item.idFactory != "self") || (factory != null && !factory.trait.Contains(item)) || item.isChara || item.noListing)
			{
				continue;
			}
			if (!EClass.debug.godCraft && !EClass.player.recipes.knownRecipes.ContainsKey(item.id))
			{
				bool flag = false;
				if (item.row.recipeKey != null && item.row.recipeKey.Length != 0 && item.row.recipeKey[0][0] == '*')
				{
					flag = true;
				}
				string id = item.id;
				if (!(id == "waystone"))
				{
					if (id == "container_shipping" && EClass.game.quests.GetPhase<QuestShippingChest>() >= 0)
					{
						flag = true;
					}
				}
				else if (EClass.game.quests.GetPhase<QuestExploration>() >= 6)
				{
					flag = true;
				}
				if (!flag)
				{
					continue;
				}
				if (newRecipes != null)
				{
					EClass.player.recipes.Add(item.id);
					if (EClass.player.recipes.newRecipes.Contains(item.id))
					{
						newRecipes.Add(item);
						EClass.player.recipes.newRecipes.Remove(item.id);
					}
				}
			}
			list.Add(item);
		}
		return list;
	}

	public void OnSleep(bool ehe = false)
	{
		int slept = EClass.player.stats.slept;
		Rand.SetSeed(EClass.game.seed + slept);
		if ((slept <= 3 || EClass.rnd(ehe ? 777 : 3) != 0) && (EClass.rnd(EClass.pc.Evalue(1642) + 1) > 0 || ((slept <= 15 || EClass.rnd(3) != 0) && (slept <= 30 || EClass.rnd(3) != 0) && (slept <= 60 || EClass.rnd(3) != 0))))
		{
			Msg.Say("learnRecipeSleep");
			Rand.SetSeed();
			Add(GetRandomRecipe((ehe ? 5 : 0) + EClass.rnd(EClass.rnd(EClass.rnd(10)))));
		}
	}

	public void ComeUpWithRandomRecipe(string idCat = null, int lvBonus = 0)
	{
		string randomRecipe = GetRandomRecipe(lvBonus, idCat, onlyUnlearned: true);
		if (randomRecipe != null)
		{
			Msg.Say("learnRecipeIdea");
			Add(randomRecipe);
		}
	}

	public bool CanCeomUpWithRecipe(string idRecipe)
	{
		if (idRecipe.IsEmpty())
		{
			return false;
		}
		RecipeSource recipeSource = Get(idRecipe);
		if (recipeSource == null || EClass.player.recipes.knownRecipes.ContainsKey(idRecipe) || (!recipeSource.NeedFactory && !recipeSource.IsQuickCraft))
		{
			return false;
		}
		int id = recipeSource.GetReqSkill().id;
		if (EClass.pc.Evalue(id) + 5 < recipeSource.row.LV)
		{
			return false;
		}
		return true;
	}

	public void ComeUpWithRecipe(string idRecipe, int chanceForRandomRecipe = 0)
	{
		if (idRecipe.IsEmpty())
		{
			return;
		}
		RecipeSource recipeSource = Get(idRecipe);
		if (EClass.rnd(10) != 0 && !EClass.debug.enable)
		{
			return;
		}
		if (recipeSource == null || EClass.player.recipes.knownRecipes.ContainsKey(idRecipe) || (!recipeSource.NeedFactory && !recipeSource.IsQuickCraft))
		{
			if (recipeSource != null && chanceForRandomRecipe > 0 && EClass.rnd(EClass.debug.enable ? 1 : chanceForRandomRecipe) == 0)
			{
				ComeUpWithRandomRecipe(recipeSource.row.Category.id);
			}
			return;
		}
		int id = recipeSource.GetReqSkill().id;
		if (EClass.pc.Evalue(id) + 5 < recipeSource.row.LV)
		{
			Msg.Say("recipeReqLv", EClass.sources.elements.map[id].GetName());
			return;
		}
		Msg.Say("learnRecipeIdea");
		Add(idRecipe);
	}

	public static RecipeSource GetUnlearnedRecipe(int lvBonus, string cat, bool onlyUnlearned)
	{
		BuildList();
		List<RecipeSource> list = new List<RecipeSource>();
		foreach (RecipeSource item in RecipeManager.list)
		{
			if ((item.NeedFactory || item.IsQuickCraft) && (cat == null || item.row.Category.IsChildOf(cat)) && (!onlyUnlearned || !EClass.player.recipes.knownRecipes.ContainsKey(item.id)) && EClass.pc.Evalue(item.GetReqSkill().id) + 5 + lvBonus >= item.row.LV && !item.row.ContainsTag("hiddenRecipe"))
			{
				list.Add(item);
			}
		}
		return list.RandomItemWeighted((RecipeSource r) => (r.row.chance == 0) ? 100 : r.row.chance);
	}

	public static RecipeSource GetLearnedRecipe(string cat = null)
	{
		List<RecipeSource> list = new List<RecipeSource>();
		foreach (string key in EClass.player.recipes.knownRecipes.Keys)
		{
			RecipeSource recipeSource = Get(key);
			if (recipeSource != null && recipeSource.row is SourceThing.Row && (recipeSource.NeedFactory || recipeSource.IsQuickCraft) && (cat == null || recipeSource.row.Category.IsChildOf(cat)))
			{
				list.Add(recipeSource);
			}
		}
		return list.RandomItemWeighted((RecipeSource r) => (r.row.chance == 0) ? 100 : r.row.chance);
	}

	public static string GetRandomRecipe(int lvBonus, string cat = null, bool onlyUnlearned = false)
	{
		RecipeSource recipeSource = null;
		recipeSource = GetUnlearnedRecipe(lvBonus, cat, onlyUnlearned);
		if (recipeSource == null)
		{
			recipeSource = GetLearnedRecipe(cat);
		}
		return recipeSource?.id;
	}

	public void OnVersionUpdate()
	{
		EClass.pc.things.Foreach(delegate(Thing t)
		{
			knownIngredients.Add(t.id);
		});
		knownIngredients.Add("flower");
	}
}
