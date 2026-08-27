public class TraitGeneratorWheel : TraitGenerator
{
	public override bool HaveUpdate => true;

	public override bool Waterproof => true;

	public override ToggleType ToggleType => ToggleType.Custom;

	public override bool IsOn
	{
		get
		{
			if (!owner.isOn)
			{
				return !EClass._zone.IsPCFaction;
			}
			return true;
		}
	}

	public override bool CanUse(Chara c)
	{
		if (owner.IsInstalled && EClass._zone.IsPCFaction)
		{
			return !owner.pos.HasChara;
		}
		return false;
	}

	public override bool OnUse(Chara c)
	{
		LayerPeople.CreateSelect("", "", delegate(BaseList l)
		{
			foreach (Chara member in EClass.Branch.members)
			{
				if (member.IsAliveInCurrentZone && !member.IsPCParty && member.memberType == FactionMemberType.Default)
				{
					l.Add(member);
				}
			}
		}, delegate(Chara chara)
		{
			chara.Teleport(owner.pos, silent: false, force: true);
			chara.RemoveCondition<ConSleep>();
			chara.noMove = true;
			chara.orgPos = new Point(owner.pos);
			chara.PlaySound("ride");
		});
		return true;
	}

	public override void OnStepped(Chara c)
	{
		if (!IsOn && ShouldWork())
		{
			owner.isOn = true;
			EClass._zone.dirtyElectricity = true;
			owner.Say("generator_start", owner);
			owner.PlaySound("electricity_on");
			Refresh(c);
		}
	}

	public override void OnSteppedOut(Chara c)
	{
		Refresh(c);
	}

	public override void Update()
	{
		if (owner.isOn && EClass._zone.IsPCFaction && !ShouldWork())
		{
			owner.isOn = false;
			EClass._zone.dirtyElectricity = true;
			owner.Say("generator_stop", owner);
			owner.PlaySound("electricity_off");
		}
	}

	public void Refresh(Chara c)
	{
		if (c.IsPCFaction && c.homeBranch == EClass._zone.branch)
		{
			c.RefreshWorkElements(EClass._zone.elements);
		}
	}

	public bool ShouldWork()
	{
		if (!owner.pos.HasChara)
		{
			return false;
		}
		int num = 0;
		foreach (Thing thing in owner.pos.Things)
		{
			if (thing.IsInstalled && thing.trait is TraitGeneratorWheel)
			{
				num++;
			}
		}
		if (num != 1)
		{
			return false;
		}
		if (!EClass._zone.IsPCFaction)
		{
			return true;
		}
		foreach (Chara chara in owner.pos.Charas)
		{
			if (chara.IsPCFaction && chara.memberType == FactionMemberType.Default)
			{
				return true;
			}
		}
		return false;
	}
}
