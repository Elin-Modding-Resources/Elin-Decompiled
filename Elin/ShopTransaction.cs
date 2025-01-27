using System.Collections.Generic;
using UnityEngine;

public class ShopTransaction : EClass
{
	public class Item
	{
		public Thing thing;

		public int num;

		public int price;
	}

	public static ShopTransaction current;

	public InvOwner trader;

	public List<Item> bought = new List<Item>();

	public List<Item> sold = new List<Item>();

	public void Log()
	{
		foreach (Item item3 in bought)
		{
			Debug.Log("Bought:" + item3.thing.Name + "/" + item3.num + " x " + item3.price + " = " + item3.num * item3.price);
		}
		foreach (Item item4 in sold)
		{
			Debug.Log("Sold:" + item4.thing.Name + "/" + item4.num + " x " + item4.price + " = " + item4.num * item4.price);
		}
	}

	public bool HasBought(Thing t)
	{
		foreach (Item item in bought)
		{
			if (item.thing == t)
			{
				return true;
			}
		}
		return false;
	}

	public bool CanSellBack(Thing t, int num = -1)
	{
		if (num == -1)
		{
			num = t.Num;
		}
		foreach (Item item in bought)
		{
			if (item.thing == t && item.num >= num)
			{
				return true;
			}
		}
		return false;
	}

	public void Process(Thing t, int n, bool sell)
	{
		int price = t.GetPrice(trader.currency, sell: false, trader.priceType);
		int price2 = t.GetPrice(trader.currency, sell: true, trader.priceType);
		int num = n;
		Debug.Log((sell ? "■Selling:" : "■Buying:") + t.Name + "/" + n + " x " + (sell ? price2 : price) + " = " + n * (sell ? price2 : price));
		int num2 = (sell ? price : price2);
		List<Item> list = (sell ? bought : sold);
		foreach (Item item in list)
		{
			if (item.thing.id == t.id && item.price == num2)
			{
				int num3 = ((item.num >= num) ? num : item.num);
				num -= num3;
				item.num -= num3;
			}
			if (num == 0)
			{
				break;
			}
		}
		list.ForeachReverse(delegate(Item i)
		{
			if (i.num == 0)
			{
				list.Remove(i);
			}
		});
		if (num > 0)
		{
			if (sell)
			{
				sold.Add(new Item
				{
					thing = t,
					num = num,
					price = price2
				});
			}
			else
			{
				bought.Add(new Item
				{
					thing = t,
					num = num,
					price = price
				});
			}
		}
		Log();
	}

	public int GetPrice(Thing t, int n, bool sell)
	{
		long num = t.GetPrice(trader.currency, sell: false, trader.priceType);
		long num2 = t.GetPrice(trader.currency, sell: true, trader.priceType);
		int num3 = n;
		long num4 = 0L;
		long num5 = (sell ? num : num2);
		foreach (Item item in sell ? bought : sold)
		{
			if (item.thing.id == t.id && item.price == num5)
			{
				int num6 = ((item.num >= num3) ? num3 : item.num);
				num3 -= num6;
				num4 += num6 * num5;
			}
			if (num3 == 0)
			{
				break;
			}
		}
		num4 += (sell ? num2 : num) * num3;
		if (num4 >= int.MaxValue || num4 <= -2147483647)
		{
			return -1;
		}
		return (int)num4;
	}

	public void OnEndTransaction()
	{
		Thing thing = trader.owner.things.Find("chest_merchant");
		if (thing != null)
		{
			thing.c_lockLv = 1;
		}
		foreach (Item item4 in bought)
		{
			Debug.Log(item4.thing);
			item4.thing.SetInt(101);
			item4.thing.isStolen = false;
		}
		if (trader.currency != CurrencyType.Money)
		{
			current = null;
			return;
		}
		int num = 0;
		foreach (Item item5 in bought)
		{
			for (int i = 0; i < item5.num; i++)
			{
				num += (int)Mathf.Sqrt(Mathf.Abs(item5.price * 5));
			}
			if (item5.thing.trait is TraitDeed)
			{
				EClass.player.flags.landDeedBought += item5.num;
			}
		}
		if (num >= 10)
		{
			EClass.pc.ModExp(291, num);
		}
		Debug.Log("negotiation total:" + num);
		num = 0;
		foreach (Item item6 in sold)
		{
			if (item6.thing.isStolen)
			{
				item6.thing.isStolen = false;
				for (int j = 0; j < item6.num; j++)
				{
					num += (int)Mathf.Sqrt(item6.price) / 2;
				}
			}
		}
		Debug.Log("stolen total:" + num);
		if (num > 0)
		{
			Guild.Thief.AddContribution(num);
		}
		current = null;
	}
}
