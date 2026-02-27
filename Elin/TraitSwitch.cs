public class TraitSwitch : Trait
{
	public static bool haltMove;

	public int TrapLv => EClass._zone.DangerLv;

	public virtual bool CanDisarmTrap => false;

	public virtual bool CanManucalActivate => false;

	public virtual bool StartHidden => false;

	public virtual bool IsNegativeEffect => false;

	public virtual bool IsJammed => owner.GetInt(60) >= 3;

	public virtual bool IsLaidByDog => owner.c_idRefCard == "dog_mine";

	public virtual bool IgnoreWhenLevitating()
	{
		return false;
	}

	public override void OnInstall(bool byPlayer)
	{
		if (byPlayer && StartHidden)
		{
			owner.SetHidden();
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		if (!EClass._zone.IsPCFaction && CanDisarmTrap)
		{
			p.TrySetAct("actDisarm", delegate
			{
				if (!TryDisarmTrap(EClass.pc) && EClass.pc.Evalue(1656) < 3 && EClass.rnd(2) == 0)
				{
					ActivateTrap(EClass.pc);
				}
				return true;
			}, owner);
		}
		else if (CanManucalActivate)
		{
			p.TrySetAct("actUse", delegate
			{
				ActivateTrap(EClass.pc);
				return true;
			}, owner);
		}
	}

	public bool TryDisarmTrap(Chara c)
	{
		if (EClass.rnd(c.Evalue(293) * 15 + 20 + c.DEX) > EClass.rnd(TrapLv * 12 + 100))
		{
			c.Say("disarm_success", c, owner);
			owner.PlaySound("disarm");
			c.ModExp(293, 50 + TrapLv);
			int num = (EClass.debug.enable ? 10 : (c.IsPCFactionOrMinion ? EClass.pc.Evalue(1656) : 0));
			if (!IsLaidByDog && num > 0 && num + 2 > EClass.rnd(10))
			{
				string id = "scrap";
				int idMat = -1;
				int lv = num * 5 + EClass.pc.Evalue(293) / 4 + EClass.pc.Evalue(210) / 4;
				if (EClass.rnd(3) == 0)
				{
					id = "junk";
				}
				if (EClass.rnd(4) == 0)
				{
					id = "microchip";
				}
				if (EClass.rnd(20) == 0)
				{
					id = "medal";
				}
				if (EClass.rnd(30) == 0)
				{
					id = "ic";
				}
				if (EClass.rnd(4) == 0)
				{
					id = "ingot";
					idMat = MATERIAL.GetRandomMaterial(lv, "metal").id;
				}
				if (EClass.rnd(4) == 0)
				{
					id = "texture";
					idMat = MATERIAL.GetRandomMaterial(lv, "leather").id;
				}
				Thing thing = ThingGen.Create(id, idMat, lv);
				thing.isHidden = false;
				EClass.pc.Say("scavenge", owner, thing);
				EClass._map.TrySmoothPick(owner.pos, thing, EClass.pc);
			}
			owner.Destroy();
			return true;
		}
		c.Say("disarm_fail", c, owner);
		c.PlaySound("disarm_fail");
		c.ModExp(293, 20 + TrapLv / 3);
		if (c.IsPCParty)
		{
			int @int = owner.GetInt(60);
			owner.SetInt(60, @int + 1);
		}
		if (IsJammed)
		{
			c.Say("trapJammed", owner);
			c.PlaySound("electricity_insufficient");
		}
		return false;
	}

	public void ActivateTrap(Chara c)
	{
		if (IsJammed)
		{
			c.Say("trapJammed2", owner);
			return;
		}
		haltMove = true;
		OnActivateTrap(c);
		if (c.IsPC && haltMove)
		{
			EClass.player.haltMove = true;
		}
	}

	public virtual void OnActivateTrap(Chara c)
	{
	}
}
