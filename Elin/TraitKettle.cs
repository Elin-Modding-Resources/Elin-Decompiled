public class TraitKettle : TraitUniqueChara
{
	public override int CostRerollShop => 0;

	public override bool CanInvest => CanJoinParty;

	public override CopyShopType CopyShop
	{
		get
		{
			if (!CanJoinParty)
			{
				return CopyShopType.None;
			}
			return CopyShopType.Item;
		}
	}

	public override ShopType ShopType
	{
		get
		{
			if (!CanJoinParty)
			{
				return ShopType.None;
			}
			return ShopType.Copy;
		}
	}

	public override PriceType PriceType => PriceType.CopyShop;

	public override bool CanJoinParty
	{
		get
		{
			if (!EClass.game.quests.IsCompleted("vernis_gold"))
			{
				return EClass.debug.enable;
			}
			return true;
		}
	}

	public override bool CanBeBanished => false;

	public override int RestockDay => 28;

	public override bool CanCopy(Thing t)
	{
		if (t.noSell || t.isStolen || t.HasRune() || t.HasElement(1229))
		{
			return false;
		}
		if (t.trait is TraitSeed)
		{
			return true;
		}
		if (t.trait is TraitFoodFishSlice)
		{
			return false;
		}
		if (t.HasElementNoCopy())
		{
			return false;
		}
		if (t.sockets != null)
		{
			foreach (int socket in t.sockets)
			{
				if (socket != 0)
				{
					return false;
				}
			}
		}
		return t.isCrafted;
	}
}
