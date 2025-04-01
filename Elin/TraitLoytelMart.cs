public class TraitLoytelMart : TraitVendingMachine
{
	public int LV => EClass.player.flags.loytelMartLv;

	public override int ShopLv => LV * 10 + ((LV <= 0) ? 1 : 10) + (EClass.debug.enable ? 50 : 0);

	public override bool IsHomeItem => true;

	public override string IDInvStyle => "default";

	public override bool CanBeOnlyBuiltInHome => true;

	public override ShopType ShopType => ShopType.LoytelMart;

	public override int CostRerollShop
	{
		get
		{
			if (!EClass.debug.enable)
			{
				return 0;
			}
			return 1;
		}
	}

	public override bool CanUse(Chara c)
	{
		if (owner.IsInstalled)
		{
			return EClass._zone.IsPCFaction;
		}
		return false;
	}

	public override void SetName(ref string s)
	{
		if (LV > 0)
		{
			s = s + Lang.space + "+" + LV;
		}
	}
}
