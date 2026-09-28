public class TraitSorin : TraitUniqueMerchant
{
	public override ShopType ShopType => ShopType.Sorin;

	public override CurrencyType CurrencyType => CurrencyType.Money3;

	public override bool CanInvest => false;

	public override int ShopLv => 1;

	public override string LangBarter => "daTradeMandrake";

	public override bool CanBeBanished => false;

	public override bool ShouldShowQuestIcon()
	{
		return !EClass.player.dialogFlags.ContainsKey("sorin_relic");
	}
}
