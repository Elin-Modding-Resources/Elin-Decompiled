public class TraitMerchantBlack : TraitMerchant
{
	public override bool AllowCriminal => true;

	public override int CostRerollShop => 2;

	public override ShopType ShopType
	{
		get
		{
			if (!Guild.Thief.IsCurrentZone || Guild.Thief.relation.rank >= 4)
			{
				return ShopType.Blackmarket;
			}
			return ShopType.None;
		}
	}

	public override bool CanSellStolenGoods => true;

	public override int ShopLv
	{
		get
		{
			if (!EClass.debug.enable)
			{
				return base.ShopLv;
			}
			return EClass.debug.startSetting.lv;
		}
	}
}
