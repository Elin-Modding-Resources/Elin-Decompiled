public class TraitPowerStatue : TraitItem
{
	public override bool CanBeDestroyed
	{
		get
		{
			if (!owner.isNPCProperty)
			{
				return base.CanBeDestroyed;
			}
			return false;
		}
	}

	public override bool CanBeStolen => false;

	public override bool CanBeTeleported => false;

	public override bool CanUseFromInventory => false;

	public override bool UseExtra => owner.isOn;

	public override bool CanStack => false;

	public override void OnCreate(int lv)
	{
		owner.isOn = true;
		owner.ChangeMaterial(12);
		owner.c_seed = EClass.rnd(20000);
		owner.SetLv(lv);
	}

	public override bool CanUse(Chara c)
	{
		if (base.CanUse(c) && owner.isOn)
		{
			return !EClass._zone.IsUserZone;
		}
		return false;
	}

	public override bool OnUse(Chara c)
	{
		bool flag = this is TraitGodStatue;
		if (!IsImplemented())
		{
			Msg.SayNothingHappen();
			return true;
		}
		Msg.Say("shrine_power", owner);
		if (flag)
		{
			SE.Play("godbless");
			owner.PlayEffect("aura_heaven");
		}
		else
		{
			SE.Play("shrine");
			owner.PlayEffect("buff");
		}
		_OnUse(c);
		owner.isOn = false;
		if (flag)
		{
			owner.ChangeMaterial("onyx");
		}
		owner.rarity = Rarity.Normal;
		owner.renderer.RefreshExtra();
		return true;
	}

	public virtual void _OnUse(Chara c)
	{
	}

	public virtual bool IsImplemented()
	{
		return true;
	}
}
