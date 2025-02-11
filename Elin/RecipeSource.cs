using System.Collections.Generic;

public class RecipeSource : EClass
{
	public RenderRow row;

	public string type;

	public string id;

	public int colorIng;

	public bool isBridge;

	public bool isBridgePillar;

	public bool isChara;

	public bool noListing;

	public bool isRandom;

	public bool alwaysKnown;

	public static List<Recipe.Ingredient> DefaultIngredients = new List<Recipe.Ingredient>
	{
		new Recipe.Ingredient
		{
			id = "log",
			req = 1
		}
	};

	public string recipeCat
	{
		get
		{
			if (!isBridge && !isBridgePillar)
			{
				return row.Category.recipeCat;
			}
			return "foundation";
		}
	}

	public string Name => row.GetName() + (isBridgePillar ? "recipeBridgePillar".lang() : (isBridge ? "recipeBridge".lang() : ""));

	public bool IsQuickCraft
	{
		get
		{
			if (!row.factory.IsEmpty())
			{
				return row.factory[0] == "self";
			}
			return false;
		}
	}

	public bool NeedFactory
	{
		get
		{
			if (!row.factory.IsEmpty() && row.factory[0] != "self" && row.factory[0] != "x" && row.factory[0] != "none")
			{
				return row.factory[0] != "None";
			}
			return false;
		}
	}

	public string NameFactory
	{
		get
		{
			if (!NeedFactory)
			{
				return EClass.sources.elements.map[6012].GetName();
			}
			return EClass.sources.cards.map[idFactory].GetName();
		}
	}

	public string idFactory
	{
		get
		{
			if (!row.factory.IsEmpty())
			{
				if (!isBridge)
				{
					if (!isBridgePillar)
					{
						return row.factory[0];
					}
					return "tool_carving";
				}
				return "factory_platform";
			}
			return null;
		}
	}

	public string GetDetail()
	{
		return row.GetDetail();
	}

	public Element GetReqSkill()
	{
		return Element.Create((NeedFactory ? EClass.sources.cards.GetModelCrafter(idFactory) : Trait.SelfFactory).IDReqEle(this), row.LV);
	}

	public List<Recipe.Ingredient> GetIngredients()
	{
		if (!row.factory.IsEmpty() && row.factory[0] == "x")
		{
			noListing = true;
		}
		if (row.components.Length == 0)
		{
			return DefaultIngredients;
		}
		if (row.components[0] == "-")
		{
			return DefaultIngredients;
		}
		if (row.recipeKey.Length != 0 && row.recipeKey[0] == "*")
		{
			alwaysKnown = true;
		}
		string[] components = row.components;
		List<Recipe.Ingredient> list = new List<Recipe.Ingredient>();
		if (!components.IsEmpty() && components[0] != "-")
		{
			for (int i = 0; i < components.Length; i++)
			{
				string[] array = components[i].Split('|');
				string[] array2 = array[0].Split('/');
				string[] array3 = array2[0].Split('@');
				bool optional = false;
				bool useCat = false;
				while (true)
				{
					switch (array3[0][0])
					{
					case '$':
						colorIng = i;
						array3[0] = array3[0].Remove(0, 1);
						break;
					case '#':
						useCat = true;
						array3[0] = array3[0].Remove(0, 1);
						break;
					case '+':
						goto IL_0142;
					}
					break;
					IL_0142:
					optional = true;
					array3[0] = array3[0].Remove(0, 1);
				}
				Recipe.Ingredient ingredient = new Recipe.Ingredient
				{
					id = array3[0],
					tag = ((array3.Length > 1) ? array3[1] : null),
					req = ((array2.Length <= 1) ? 1 : int.Parse(array2[1])),
					optional = optional,
					useCat = useCat
				};
				if (array.Length > 1)
				{
					for (int j = 1; j < array.Length; j++)
					{
						ingredient.idOther.Add(array[j]);
					}
				}
				list.Add(ingredient);
			}
		}
		return list;
	}

	public string GetIDIngredient()
	{
		string[] components = row.components;
		if (!components.IsEmpty() && components[0] != "-")
		{
			int num = 0;
			if (num < components.Length)
			{
				string[] array = components[num].Split('/')[0].Split('@');
				char c = array[0][0];
				if (c == '#' || c == '$' || c == '+')
				{
					array[0] = array[0].Remove(0, 1);
				}
				return array[0];
			}
		}
		return null;
	}

	public int GetSPCost(Card factory)
	{
		Element reqSkill = GetReqSkill();
		int num = row.Category.costSP + reqSkill.Value / 10;
		int num2 = EClass.pc.Evalue(reqSkill.id);
		if (num2 < reqSkill.Value)
		{
			num += (reqSkill.Value - num2) * 2 / 3;
		}
		return num;
	}
}
