using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CraftUtil : EClass
{
	public enum MixType
	{
		General,
		Food,
		NoMix
	}

	public enum WrapType
	{
		Love,
		Dark
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
			Thing thing = ThingGen.Create(ingredient.id, -1, lv);
			if (thing.id == "deadbody")
			{
				thing = ThingGen.Create("_meat");
			}
			thing = thing.TryMakeRandomItem(lv, TryMakeRandomItemSource.Cooking, crafter);
			TraitSeed.LevelSeed(thing, null, EClass.rnd(lv / 4) + 1);
			thing.SetEncLv(thing.encLV / 2);
			if (num > 0 && EClass.rnd(3) == 0)
			{
				thing.elements.SetBase(2, num);
			}
			list.Add(thing);
		}
		MakeDish(food, list, num, crafter);
		if (crafter != null && crafter.id != "rodwyn")
		{
			if (food.HasElement(709))
			{
				food.elements.Remove(709);
			}
			if (food.HasElement(708) && !crafter.HasElement(1205))
			{
				food.elements.Remove(708);
			}
		}
	}

	public static Thing MakeDarkSoup()
	{
		Thing thing = ThingGen.Create("soup_dark");
		for (int i = 0; i < 4 + EClass.rnd(4); i++)
		{
			Chara c = EClass._map.charas.RandomItem();
			WrapIngredient(thing, c, GetRandomDarkSoupIngredient(c), WrapType.Dark);
		}
		if (!EClass.debug.autoIdentify)
		{
			thing.c_IDTState = 1;
		}
		return thing;
	}

	public static Thing GetRandomDarkSoupIngredient(Chara c)
	{
		return ThingGen.CreateFromFilter("darksoup", c.LV);
	}

	public static Thing MakeLoveLunch(Chara c)
	{
		Thing thing = ThingGen.Create("lunch_love");
		thing.MakeRefFrom(c);
		int num = c.uid + EClass.world.date.year * 10000 + EClass.world.date.month * 100;
		Rand.SetSeed(num);
		float num2 = Mathf.Clamp(1f + Mathf.Sqrt(c.Evalue(287) / 5), 2f, 6f);
		if (!EClass.debug.autoIdentify)
		{
			thing.c_IDTState = 5;
		}
		for (int i = 0; (float)i < num2; i++)
		{
			Rand.SetSeed(num + i);
			WrapIngredient(thing, c, GetRandomLoveLunchIngredient(c), WrapType.Love);
		}
		thing.elements.SetBase(701, 0);
		if (thing.Evalue(753) < 0)
		{
			thing.elements.SetBase(753, 0);
		}
		Rand.SetSeed();
		return thing;
	}

	public static Thing GetRandomLoveLunchIngredient(Chara c)
	{
		Thing thing = null;
		for (int i = 0; i < 3; i++)
		{
			thing = ThingGen.Create("dish", -1, c.Evalue(287) + 5 + (EClass.debug.enable ? c.LV : 0));
			if (!thing.HasTag(CTAG.dish_fail))
			{
				break;
			}
		}
		MakeDish(thing, c.LV, c);
		return thing;
	}

	public static void WrapIngredient(Card product, Chara c, Card ing, WrapType wrapType)
	{
		if (product.c_mixedFoodData == null)
		{
			product.c_mixedFoodData = new MixedFoodData();
		}
		product.c_mixedFoodData.texts.Add(ing.Name + ((wrapType == WrapType.Dark) ? "isMixedBy".lang(c.NameSimple) : ""));
		foreach (Element value in ing.elements.dict.Values)
		{
			if (!IsValidTrait(value))
			{
				continue;
			}
			if (value.IsFoodTraitMain)
			{
				int num = value.Value;
				if (ing.id == "lunch_dystopia" && (wrapType == WrapType.Dark || num < 0))
				{
					num *= -1;
				}
				product.elements.ModBase(value.id, num);
			}
			else
			{
				int num2 = product.elements.Base(value.id);
				if ((num2 <= 0 && value.Value < 0 && value.Value < num2) || (value.Value > 0 && value.Value > num2))
				{
					product.elements.SetTo(value.id, value.Value);
				}
			}
		}
		product.elements.ModBase(10, ing.Evalue(10));
		static bool IsValidTrait(Element e)
		{
			if (e.HasTag("noInherit"))
			{
				return false;
			}
			if (e.IsFoodTrait || e.IsTrait || e.id == 2)
			{
				return true;
			}
			return false;
		}
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
		bool noMix = type == MixType.NoMix || product.HasTag(CTAG.noMix);
		bool isFood = type == MixType.Food;
		int nutFactor = 100 - (ings.Count - 1) * 5;
		Thing thing = ((ings.Count > 0) ? ings[0] : null);
		bool creative = crafter?.HasElement(487) ?? false;
		if (crafter != null && crafter.Evalue(1650) >= 3)
		{
			nutFactor -= 10;
		}
		if (!noMix)
		{
			foreach (Element value2 in product.elements.dict.Values)
			{
				int id = value2.id;
				if ((uint)(id - 914) > 1u && value2.Value >= 0 && (value2.HasTag("noInherit") || IsValidTrait(value2)))
				{
					product.elements.SetTo(value2.id, 0);
				}
			}
		}
		if (product.HasCraftBonusTrait())
		{
			foreach (Element item in product.ListCraftBonusTraits())
			{
				product.elements.ModBase(item.id, item.Value);
			}
		}
		if (isFood)
		{
			product.elements.SetTo(10, 5);
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		foreach (Thing ing in ings)
		{
			if (ing != null)
			{
				MixElements(ing);
				num3 += ing.c_priceCopy;
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
		product.c_priceCopy = num3;
		if (thing != null && product.trait is TraitFoodFishSlice)
		{
			product.elements.SetTo(10, thing.Evalue(10) / 4);
			product.isWeightChanged = true;
			product.c_weight = Mathf.Min(thing.SelfWeight / 6, 1000);
			product.c_idRefCard = thing.id;
			product.c_priceCopy = ((thing.c_priceCopy == 0) ? thing.GetValue() : thing.c_priceCopy);
			product.c_fixedValue = ((thing.c_fixedValue == 0) ? thing.sourceCard.value : thing.c_fixedValue) / 4;
			product.c_priceAdd = 0;
			product.decay = thing.decay;
			product.elements.SetBase(707, 1);
			product.SetTier(thing.tier, setTraits: false);
			product.idSkin = ((thing.trait is TraitFoodFishSlice) ? thing.idSkin : (thing.HasTag(CTAG.bigFish) ? 1 : 0));
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
				int num4 = 1 + product.Evalue(2) + product.Evalue(751);
				if (num4 < 1)
				{
					num4 = 1;
				}
				foreach (Thing ing2 in ings)
				{
					if (ing2 != null && ing2.Thing != null && !(ing2.id != "gem"))
					{
						num4 *= ing2.Thing.material.hardness / 20 + 2;
					}
				}
				if (num4 > EClass.pc.FameLv + 10 - 1)
				{
					num4 = EClass.pc.FameLv + 10 - 1;
				}
				product.SetInt(25, num4);
			}
		}
		else
		{
			product.elements.ModBase(10, 6);
		}
		if (product.HasElement(762))
		{
			product.elements.ModBase(10, product.Evalue(762) / 5);
			if (product.Evalue(10) < 1)
			{
				product.elements.SetTo(10, 1);
			}
		}
		if (creative && isFood && product.category.IsChildOf("meal"))
		{
			product.elements.SetBase(764, 1);
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
					return product.IsInheritFoodTraits;
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
				foreach (Element value3 in t.elements.dict.Values)
				{
					if (IsValidTrait(value3) && (!noMix || value3.id == 2))
					{
						if (isFood && value3.IsFoodTraitMain)
						{
							int num5 = value3.Value;
							if (product.id == "lunch_dystopia")
							{
								num5 *= -1;
							}
							if (creative && num5 > 500)
							{
								num5 = 500;
							}
							product.elements.ModBase(value3.id, num5);
						}
						else
						{
							int num6 = product.elements.Base(value3.id);
							if ((num6 <= 0 && value3.Value < 0 && value3.Value < num6) || (value3.Value > 0 && value3.Value > num6))
							{
								product.elements.SetTo(value3.id, value3.Value);
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
