using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CraftUtil : EClass
{
	public enum MixType
	{
		General,
		Food
	}

	public static string[] ListFoodEffect = new string[2] { "exp", "pot" };

	public static void ModRandomFoodEnc(Thing t)
	{
		List<Element> list = new List<Element>();
		foreach (Element value in t.elements.dict.Values)
		{
			if (value.IsFoodTrait)
			{
				list.Add(value);
			}
		}
		if (list.Count != 0)
		{
			Element element = list.RandomItem();
			t.elements.ModBase(element.id, EClass.rnd(6) + 1);
			if (element.Value > 60)
			{
				t.elements.SetTo(element.id, 60);
			}
		}
	}

	public static void AddRandomFoodEnc(Thing t)
	{
		List<SourceElement.Row> list = EClass.sources.elements.rows.Where((SourceElement.Row e) => e.foodEffect.Length > 1 && ListFoodEffect.Contains(e.foodEffect[0])).ToList();
		list.ForeachReverse(delegate(SourceElement.Row e)
		{
			if (t.elements.dict.ContainsKey(e.id))
			{
				list.Remove(e);
			}
		});
		if (list.Count != 0)
		{
			SourceElement.Row row = list.RandomItemWeighted((SourceElement.Row a) => a.chance);
			t.elements.SetBase(row.id, 1);
			t.c_seed = row.id;
		}
	}

	public static void MakeDish(Thing food, int lv, Chara crafter = null)
	{
		RecipeManager.BuildList();
		List<Thing> list = new List<Thing>();
		RecipeSource recipeSource = RecipeManager.Get(food.id);
		Debug.Log(recipeSource);
		if (recipeSource == null)
		{
			return;
		}
		int num = Mathf.Min(EClass.rnd(lv), 50);
		foreach (Recipe.Ingredient ingredient in recipeSource.GetIngredients())
		{
			Thing thing = ThingGen.Create(ingredient.id);
			TraitSeed.LevelSeed(thing, null, EClass.rnd(lv / 4) + 1);
			thing.SetEncLv(thing.encLV / 2);
			if (num > 0 && EClass.rnd(3) == 0)
			{
				thing.elements.SetBase(2, num);
			}
			list.Add(thing);
		}
		MakeDish(food, list, num, crafter);
	}

	public static void MakeDish(Card food, List<Thing> ings, int qualityBonus, Chara crafter = null)
	{
		List<Thing> list = new List<Thing>();
		bool flag = food.sourceCard.vals.Contains("fixed");
		for (int i = 0; i < ings.Count; i++)
		{
			Thing thing = ings[i];
			if (flag)
			{
				list.Add(thing);
				break;
			}
			if (!IsIgnoreName(thing))
			{
				list.Add(thing);
			}
		}
		if (list.Count > 0)
		{
			Thing thing2 = list.RandomItem();
			if (thing2 != null)
			{
				food.MakeRefFrom(thing2);
				if (thing2.c_idRefCard != null)
				{
					food.c_idRefCard = thing2.c_idRefCard;
					food.c_altName = food.TryGetFoodName(thing2);
					if (thing2.id == "_egg" || thing2.id == "egg_fertilized")
					{
						food.c_altName = "_egg".lang(food.c_altName);
					}
				}
			}
		}
		MixIngredients(food, ings, MixType.Food, qualityBonus, crafter);
		static bool IsIgnoreName(Card t)
		{
			if (t == null)
			{
				return true;
			}
			switch (t.sourceCard._origin)
			{
			case "dough":
			case "dish":
			case "dish_lunch":
				return true;
			default:
				return false;
			}
		}
	}

	public static Thing MixIngredients(string idProduct, List<Thing> ings, MixType type, int idMat = 0, Chara crafter = null)
	{
		Thing thing = ThingGen.Create(idProduct);
		if (idMat != 0)
		{
			thing.ChangeMaterial(idMat);
		}
		MixIngredients(thing, ings, type, 999, crafter);
		return thing;
	}

	public static Card MixIngredients(Card product, List<Thing> ings, MixType type, int maxQuality, Chara crafter = null)
	{
		bool isFood = type == MixType.Food;
		int nutFactor = 100 - (ings.Count - 1) * 5;
		if (crafter != null && crafter.Evalue(1650) >= 3)
		{
			nutFactor -= 10;
		}
		foreach (Element value2 in product.elements.dict.Values)
		{
			int id = value2.id;
			if ((uint)(id - 914) > 1u && value2.Value >= 0 && (value2.HasTag("noInherit") || IsValidTrait(value2)))
			{
				product.elements.SetTo(value2.id, 0);
			}
		}
		if (product.HasTag(CTAG.dish_bonus))
		{
			foreach (Element value3 in product.sourceCard.model.elements.dict.Values)
			{
				if (value3.IsFoodTraitMain)
				{
					product.elements.ModBase(value3.id, value3.Value);
				}
			}
		}
		if (isFood)
		{
			product.elements.SetTo(10, 5);
		}
		int num = 0;
		int num2 = 0;
		foreach (Thing ing in ings)
		{
			if (ing != null)
			{
				MixElements(ing);
				if (isFood)
				{
					num += Mathf.Clamp(ing.SelfWeight * 80 / 100, 50, 400 + ing.SelfWeight / 20);
					int value = ing.GetValue();
					num2 += value;
				}
			}
		}
		if (isFood)
		{
			product.isWeightChanged = true;
			product.c_weight = num;
			product.c_priceAdd = num2;
		}
		if (product.HasElement(652))
		{
			product.ChangeWeight((isFood ? num : product.Thing.source.weight) * 100 / (100 + product.Evalue(652)));
		}
		if (product.elements.Value(2) > maxQuality)
		{
			product.elements.SetTo(2, maxQuality);
		}
		string id2 = product.id;
		if (!(id2 == "zassouni"))
		{
			if (id2 == "map")
			{
				int num3 = 1 + product.Evalue(2) + product.Evalue(751);
				if (num3 < 1)
				{
					num3 = 1;
				}
				foreach (Thing ing2 in ings)
				{
					if (ing2 != null && ing2.Thing != null && !(ing2.id != "gem"))
					{
						num3 *= ing2.Thing.material.hardness / 20 + 2;
					}
				}
				if (num3 > EClass.pc.FameLv + 10 - 1)
				{
					num3 = EClass.pc.FameLv + 10 - 1;
				}
				product.SetInt(25, num3);
			}
		}
		else
		{
			product.elements.ModBase(10, 6);
		}
		return product;
		bool IsValidTrait(Element e)
		{
			if (e.HasTag("noInherit"))
			{
				return false;
			}
			switch (type)
			{
			case MixType.General:
				if (e.IsTrait)
				{
					return true;
				}
				if (e.IsFoodTrait)
				{
					return product.ShowFoodEnc;
				}
				break;
			case MixType.Food:
				if (e.IsFoodTrait || e.IsTrait || e.id == 2)
				{
					return true;
				}
				break;
			}
			return false;
		}
		void MixElements(Card t)
		{
			if (t != null)
			{
				foreach (Element value4 in t.elements.dict.Values)
				{
					if (IsValidTrait(value4))
					{
						if (isFood && value4.IsFoodTraitMain)
						{
							product.elements.ModBase(value4.id, value4.Value);
						}
						else
						{
							int num4 = product.elements.Base(value4.id);
							if ((num4 <= 0 && value4.Value < 0 && value4.Value < num4) || (value4.Value > 0 && value4.Value > num4))
							{
								product.elements.SetTo(value4.id, value4.Value);
							}
						}
					}
				}
				if (isFood)
				{
					product.elements.ModBase(10, t.Evalue(10) * nutFactor / 100);
				}
			}
		}
	}
}
