public class TraitSalesTag : Trait
{
	public override bool CanBeDestroyed => false;

	public override bool IsTool => !owner.isOn;

	public override bool UseAltTiles => owner.isOn;

	public static bool CanTagSale(Card t, bool insideContainer = false)
	{
		if (t.isSale)
		{
			return false;
		}
		if (t.isChara)
		{
			Chara chara = t.Chara;
			if (chara.trait is TraitUniqueChara || chara.IsUnique || EClass.game.cards.listAdv.Contains(chara))
			{
				return false;
			}
			if (!chara.IsPCFaction || chara.IsPCParty)
			{
				return false;
			}
		}
		else
		{
			if (!insideContainer && !t.IsInstalled)
			{
				return false;
			}
			if (!t.trait.CanBeHeld || !t.trait.CanBeDestroyed || !t.trait.CanBeShipped || t.isMasked || t.trait is TraitSpot)
			{
				return false;
			}
			if (t.trait is TraitMannequin)
			{
				return false;
			}
		}
		if (t.GetPrice(CurrencyType.Money, sell: true, PriceType.PlayerShop) == 0)
		{
			return false;
		}
		return true;
	}

	public override void TrySetHeldAct(ActPlan p)
	{
		if (!EClass._zone.IsPCFaction || owner.isOn)
		{
			return;
		}
		p.pos.ListCards().ForEach(delegate(Card t)
		{
			if (t.trait is TraitSalesTag && t.isOn)
			{
				p.TrySetAct("actRemove", delegate
				{
					t.Destroy();
					SE.Trash();
					return false;
				}, t);
			}
			if (t.isSale)
			{
				p.TrySetAct("actRemoveSalesTag".lang(t.Name), delegate
				{
					SE.Play("paper");
					t.SetSale(sale: false);
					return false;
				});
			}
			else if (CanTagSale(t))
			{
				int price = t.GetPrice(CurrencyType.Money, sell: true, PriceType.PlayerShop);
				p.TrySetAct("actSalesTag".lang(Lang._currency(price, "money"), t.Name), delegate
				{
					SE.Play("paper");
					t.SetSale(sale: true);
					return false;
				});
			}
		});
	}

	public override bool CanStackTo(Thing to)
	{
		return false;
	}
}
