public class TraitMimu : TraitUniqueMerchant
{
	public override bool CanInvite => false;

	public override int CostRerollShop => 0;

	public override CurrencyType CurrencyType => CurrencyType.Money2;

	public override ShopType ShopType => ShopType.Specific;
}
