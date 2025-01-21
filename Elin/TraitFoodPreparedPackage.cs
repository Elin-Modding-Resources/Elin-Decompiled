public class TraitFoodPreparedPackage : TraitFood
{
	public override string LangUse => "actContainer";

	public override bool CanEat(Chara c)
	{
		if (c.IsPC)
		{
			return owner.idSkin == 1;
		}
		return true;
	}

	public override bool CanUse(Chara c)
	{
		if (EClass._zone.IsUserZone && owner.isNPCProperty)
		{
			return false;
		}
		return owner.idSkin == 0;
	}

	public override bool OnUse(Chara c)
	{
		SE.Play("open");
		c.Say("openDoor", c, owner);
		owner.idSkin = 1;
		LayerInventory.SetDirty(owner.Thing);
		return true;
	}
}
