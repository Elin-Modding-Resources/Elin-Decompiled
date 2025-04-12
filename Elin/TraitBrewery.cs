using System.Collections.Generic;
using UnityEngine;

public class TraitBrewery : TraitContainer
{
	public enum Type
	{
		Food,
		Drink,
		Fertilizer
	}

	public override int DecaySpeedChild => 500;

	public virtual string idMsg => "brew";

	public virtual Type type => Type.Drink;

	public virtual bool IsFood => false;

	public override bool CanChildDecay(Card c)
	{
		switch (c.id)
		{
		case "48":
		case "cheese":
		case "jerky":
			return false;
		default:
			if (!(c.trait is TraitDrinkMilk))
			{
				return c.material.id == 94;
			}
			return true;
		}
	}

	public override bool OnChildDecay(Card c, bool firstDecay)
	{
		switch (type)
		{
		case Type.Food:
		case Type.Drink:
			if (!firstDecay)
			{
				return true;
			}
			if (!c.IsFood && !(c.id == "508") && !(c.id == "778") && !(c.id == "milk"))
			{
				return true;
			}
			if (c.category.IsChildOf("meal"))
			{
				return true;
			}
			break;
		default:
			if (!firstDecay)
			{
				return true;
			}
			break;
		case Type.Fertilizer:
			break;
		}
		string productID = GetProductID(c);
		if (productID == null)
		{
			return true;
		}
		Thing thing = c.Duplicate(c.Num);
		c.Destroy();
		switch (type)
		{
		case Type.Food:
			c = CraftUtil.MixIngredients(productID, new List<Thing> { c.Thing }, CraftUtil.MixType.Food, 0, EClass.pc).SetNum(thing.Num);
			break;
		case Type.Fertilizer:
		{
			int num = 20 + thing.SelfWeight;
			if (num > 100)
			{
				num = 100 + (int)Mathf.Sqrt((num - 100) * 10);
			}
			int num2 = 0;
			for (int i = 0; i < thing.Num; i++)
			{
				num2 += num / 100 + ((num % 100 > EClass.rnd(100)) ? 1 : 0);
			}
			if (num2 <= 0)
			{
				return false;
			}
			c = ((!thing.isCopy) ? ThingGen.Create(productID).SetNum(num2) : ThingGen.Create("ash3").SetNum(Mathf.Min(num2, 10)));
			break;
		}
		default:
			c = ThingGen.Create(productID).SetNum(thing.Num);
			break;
		}
		if (type != Type.Fertilizer)
		{
			c.MakeFoodRef(thing);
		}
		if (type == Type.Drink)
		{
			c.c_priceAdd = thing.GetValue() * 125 / 100;
		}
		OnProduce(c);
		owner.AddThing(c.Thing);
		owner.GetRootCard().Say(idMsg, thing, c);
		return false;
	}

	public virtual string GetProductID(Card c)
	{
		switch (c.id)
		{
		case "crim":
		case "drug_crim":
			return "crimAle";
		case "rice_plant":
		case "rice":
		case "692":
		case "719":
		case "720":
			return "1134";
		default:
			if (c.category.IsChildOf("mushroom") || c.category.IsChildOf("nuts"))
			{
				return "54";
			}
			return "48";
		}
	}

	public virtual void OnProduce(Card c)
	{
	}
}
