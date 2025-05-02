public class TraitExileElder : TraitUniqueChara
{
	public override int GuidePriotiy => 99;

	public override ShopType ShopType => ShopType.Influence;

	public override CurrencyType CurrencyType => CurrencyType.Influence;

	public override bool CanInvite => false;
}
