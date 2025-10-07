public class TraitBambi : TraitUniqueMerchant
{
	public override ShopType ShopType => ShopType.Curry;

	public override int ShopLv => base.ShopLv + 10;

	public override bool CanInvite => false;
}
