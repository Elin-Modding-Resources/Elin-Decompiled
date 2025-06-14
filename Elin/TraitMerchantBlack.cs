public class TraitMerchantBlack : TraitMerchant
{
	public override bool AllowCriminal => true;

	public override int CostRerollShop => 2;

	public override ShopType ShopType => ShopType.Blackmarket;

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
