public class TraitMerchantEcopo : TraitMerchant
{
	public override ShopType ShopType => ShopType.Ecopo;

	public override CurrencyType CurrencyType => CurrencyType.Ecopo;

	public override int CostRerollShop => 2;
}
