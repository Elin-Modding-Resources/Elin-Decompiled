public class TraitMerchantPlat : TraitMerchant
{
	public override ShopType ShopType
	{
		get
		{
			if (!base.owner.IsPCFaction || EClass.game.IsSurvival)
			{
				return ShopType.Plat;
			}
			return ShopType.None;
		}
	}

	public override CurrencyType CurrencyType => CurrencyType.Plat;

	public override string LangBarter => "daBuyPlat";

	public override bool CanInvite => false;
}
