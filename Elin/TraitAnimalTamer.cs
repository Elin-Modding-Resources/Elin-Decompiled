public class TraitAnimalTamer : TraitMerchant
{
	public override int GuidePriotiy => 15;

	public override SlaverType SlaverType => SlaverType.Animal;

	public override ShopType ShopType => ShopType.AnimalGoods;
}
