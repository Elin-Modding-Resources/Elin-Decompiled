using System.Collections.Generic;
using UnityEngine;

public class AI_Shopping : AIAct
{
	public Card container;

	public Card dest;

	public static bool TryShop(Chara c, bool realtime)
	{
		if (c.memberType != FactionMemberType.Guest || !EClass._zone.IsPCFaction || EClass._map.props.sales.Count == 0)
		{
			return false;
		}
		Card card = EClass._map.props.sales.RandomItem();
		int c_allowance = c.c_allowance;
		if (!EClass.debug.enable && c_allowance <= 0)
		{
			return false;
		}
		bool flag = card.IsContainer && card.things.Count > 0;
		List<Card> list = new List<Card>();
		if (flag)
		{
			foreach (Thing thing in card.things)
			{
				if (TraitSalesTag.CanTagSale(thing, insideContainer: true))
				{
					list.Add(thing);
				}
			}
		}
		else
		{
			list.Add(card);
		}
		foreach (Card item in list)
		{
			int num = 25;
			int price = item.GetPrice(CurrencyType.Money, sell: true, PriceType.PlayerShop);
			if (price >= c_allowance)
			{
				num = num * 10 * price / c_allowance;
			}
			if (price >= 10000)
			{
				num *= 15;
			}
			else if (price >= 5000)
			{
				num *= 10;
			}
			else if (price >= 1000)
			{
				num *= 6;
			}
			else if (price >= 200)
			{
				num *= 3;
			}
			if (EClass.Branch != null)
			{
				if (EClass.Branch.policies.IsActive(2817))
				{
					num = num * (300 + EClass.Branch.Evalue(2817)) / 100;
				}
				if (EClass.Branch.policies.IsActive(2816))
				{
					num = num * (1000 + EClass.Branch.Evalue(2816) * 2) / 100;
				}
			}
			if (!realtime)
			{
				num /= 4;
			}
			if (EClass.debug.enable || EClass.rnd(num + 1) == 0)
			{
				if (realtime)
				{
					c.SetAI(new AI_Shopping
					{
						container = (flag ? card : null),
						dest = item
					});
					return true;
				}
				Buy(c, item, realtime: false, flag ? card : null);
			}
		}
		return false;
	}

	public static bool TryRestock(Chara c, bool realtime)
	{
		for (int i = 0; i < (realtime ? 1 : 10); i++)
		{
			bool flag = false;
			foreach (Thing thing2 in EClass._map.things)
			{
				if (EClass.rnd(2) == 0 || !thing2.IsInstalled || !(thing2.trait is TraitSalesTag { IsOn: not false }) || thing2.GetStr(11).IsEmpty())
				{
					continue;
				}
				Thing thing = EClass._zone.TryGetRestock<TraitSpotStock>(thing2.GetStr(11));
				if (thing != null)
				{
					if (realtime)
					{
						thing2.PlaySound("restock");
					}
					thing2.Destroy();
					thing.isSale = true;
					EClass._zone.AddCard(thing, thing2.pos).Install();
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				break;
			}
		}
		return false;
	}

	public override IEnumerable<Status> Run()
	{
		Card _dest = container ?? dest;
		if (_dest.ExistsOnMap)
		{
			yield return DoGoto(_dest.pos, 1);
		}
		if (!_dest.ExistsOnMap || !_dest.isSale)
		{
			yield return Success();
		}
		Buy(owner, dest, realtime: true, container);
		yield return Success();
	}

	public static void SellChara(Chara c)
	{
		Msg.Say("sell_resident", c);
		c.homeBranch.BanishMember(c, skipMsg: true);
		EClass.player.ModKarma(-1);
	}

	public static void Buy(Chara c, Card dest, bool realtime, Card container)
	{
		Point point = dest.pos.Copy();
		int price = dest.GetPrice(CurrencyType.Money, sell: true, PriceType.PlayerShop);
		if ((dest.isThing && (dest.isDestroyed || dest.Thing.IsSharedContainer)) || (container == null && !dest.isSale) || (container != null && container.c_lockLv != 0))
		{
			return;
		}
		Card card = (dest.isChara ? dest : dest.Split(1));
		EClass.Branch.incomeShop += price;
		card.isSale = false;
		if (card == dest)
		{
			EClass._map.props.sales.Remove(card);
		}
		if (realtime)
		{
			c.Talk("shop_buy");
			c.PlaySound("money");
			Msg.alwaysVisible = true;
			Msg.Say("shop_buy", c, card, Lang._currency(price, "money"));
			if (card.isThing)
			{
				c.AddCard(card);
				c.c_allowance -= price;
				if (!c.TryUse(card.Thing))
				{
					card.ModNum(-1);
				}
			}
			else
			{
				SellChara(card.Chara);
			}
		}
		else if (card.isThing)
		{
			card.ModNum(-1);
		}
		else
		{
			SellChara(card.Chara);
		}
		if (card.isStolen || card.isChara)
		{
			Guild.Thief.AddContribution((int)Mathf.Sqrt(price) / 2);
		}
		if (container != null)
		{
			if (container.things.Count == 0)
			{
				container.isSale = false;
				EClass._map.props.sales.Remove(container);
			}
		}
		else if (!dest.ExistsOnMap && dest.isThing)
		{
			Thing thing = ThingGen.Create("tag_sell");
			thing.isOn = true;
			thing.SetStr(11, dest.category.id);
			EClass._zone.AddCard(thing, point).Install();
		}
		EClass.Branch.Log("shop_buy", c, card, Lang._currency(price, "money"));
	}
}
