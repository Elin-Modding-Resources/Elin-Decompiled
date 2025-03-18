public class TraitMerchantFestival : TraitMerchant
{
	public override int CostRerollShop => 5;

	public override bool CanInvite => false;

	public override ShopType ShopType => ShopType.Festival;
}
