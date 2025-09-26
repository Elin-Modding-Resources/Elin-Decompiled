public class TraitMiral : TraitUniqueMerchant
{
	public override int CostRerollShop => 3;

	public override ShopType ShopType => ShopType.Medal;

	public override CurrencyType CurrencyType => CurrencyType.Medal;

	public override string LangBarter => "daBuyMedal";

	public override string IdAmbience => "blacksmith";

	public override int ShopLv => 1;
}
