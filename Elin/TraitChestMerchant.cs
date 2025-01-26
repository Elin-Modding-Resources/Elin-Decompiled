public class TraitChestMerchant : TraitContainer
{
	public override bool CanUseContent => false;

	public override string IDInvStyle
	{
		get
		{
			if (EClass._zone is Zone_Casino)
			{
				return "casino";
			}
			if (EClass._zone.source.tag.Contains("tech"))
			{
				return "modern";
			}
			if (owner.GetRootCard().trait is TraitVendingMachine)
			{
				return owner.GetRootCard().trait.IDInvStyle;
			}
			return base.idContainer;
		}
	}

	public override void Prespawn(int lv)
	{
	}
}
