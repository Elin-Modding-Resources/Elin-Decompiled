public class TraitMagicChest : TraitContainer
{
	public override int Electricity => base.Electricity + ((IsFridge ? 50 : 0) + owner.c_containerUpgrade.cap / 5) * -1;

	public override bool IsHomeItem => true;

	public override bool IsSpecialContainer => true;

	public override bool CanBeOnlyBuiltInHome => true;

	public override bool CanOpenContainer
	{
		get
		{
			if (EClass._zone.IsPCFaction)
			{
				return owner.IsInstalled;
			}
			return false;
		}
	}

	public override bool IsFridge => owner.c_containerUpgrade.cool > 0;

	public override bool UseAltTiles => owner.isOn;

	public override int DecaySpeedChild
	{
		get
		{
			if (!IsFridge || !owner.isOn)
			{
				return base.DecaySpeedChild;
			}
			return 0;
		}
	}

	public override bool CanSearchContent
	{
		get
		{
			if (EClass.core.IsGameStarted && owner.IsInstalled)
			{
				return EClass._zone.IsPCFaction;
			}
			return false;
		}
	}

	public override void SetName(ref string s)
	{
		if (IsFridge)
		{
			s = "chest_fridge".lang(s);
		}
	}
}
