using UnityEngine;

public class TraitGeneMachine : TraitStasisChamber
{
	public override bool CanBeOnlyBuiltInHome => true;

	public override bool IsHomeItem => true;

	public override bool CanBeHeld => !IsTargetUsingGene();

	public Chara GetTarget()
	{
		if (!owner.IsInstalled)
		{
			return null;
		}
		foreach (Chara chara in owner.pos.Charas)
		{
			ConSuspend condition = chara.GetCondition<ConSuspend>();
			if (condition != null && condition.uidMachine == owner.uid)
			{
				return chara;
			}
		}
		return null;
	}

	public bool IsTargetUsingGene()
	{
		return GetTarget()?.GetCondition<ConSuspend>().HasGene ?? false;
	}

	public float GetProgress()
	{
		ConSuspend condition = GetTarget().GetCondition<ConSuspend>();
		if (condition == null || !condition.HasGene)
		{
			return 0f;
		}
		int remainingHours = EClass.world.date.GetRemainingHours(condition.dateFinish);
		if (condition.duration == 0 || EClass.debug.enable)
		{
			return 1f;
		}
		return Mathf.Clamp((float)(condition.duration - remainingHours) / (float)condition.duration, 0f, 1f);
	}

	public string GetProgressText()
	{
		ConSuspend condition = GetTarget().GetCondition<ConSuspend>();
		int remainingHours = EClass.world.date.GetRemainingHours(condition.dateFinish);
		if (remainingHours > 0)
		{
			return remainingHours + " h";
		}
		return "progress_finish".lang();
	}

	public override bool CanUse(Chara c)
	{
		if (owner.IsInstalled && owner.isOn)
		{
			if (IsTargetUsingGene())
			{
				return GetProgress() >= 1f;
			}
			return true;
		}
		return false;
	}

	public override bool OnUse(Chara c)
	{
		if (EClass._zone.IsUserZone)
		{
			Msg.SayNothingHappen();
			return false;
		}
		Chara target = GetTarget();
		if (target == null)
		{
			LayerPeople.CreateSelect("", "", delegate(UIList l)
			{
				foreach (Chara member in EClass.Branch.members)
				{
					if (member.GetCondition<ConSuspend>() == null && member.host == null && !member.IsPC && member.IsAliveInCurrentZone && member.memberType == FactionMemberType.Default)
					{
						l.Add(member);
					}
				}
			}, delegate(Chara c)
			{
				if (c.IsPCParty)
				{
					EClass.pc.party.RemoveMember(c);
				}
				if (!c.pos.Equals(owner.pos))
				{
					EClass.pc.Kick(owner.pos);
					c.Teleport(owner.pos, silent: false, force: true);
					c.isRestrained = false;
				}
				if (EClass.debug.enable)
				{
					if (c.c_genes == null)
					{
						c.c_genes = new CharaGenes();
					}
					c.c_genes.inferior += 20;
					c.feat += 500;
				}
				c.RemoveCondition<ConSleep>();
				c.PlaySound("ride");
				(c.AddCondition<ConSuspend>(100, force: true) as ConSuspend).uidMachine = owner.uid;
			}, (Chara a) => "gene_note".lang(a.CurrentGeneSlot.ToString() ?? "", a.MaxGeneSlot.ToString() ?? "", a.feat.ToString() ?? "", a.GetTotalFeat() + " "));
		}
		else if (GetProgress() >= 1f)
		{
			ConSuspend condition = target.GetCondition<ConSuspend>();
			if (condition.gene.GetRootCard() != target)
			{
				Msg.SayNothingHappen();
			}
			else
			{
				target.Say("gene_finish", target, condition.gene);
				condition.gene.c_DNA.Apply(target);
				condition.gene.Destroy();
				condition.gene = null;
			}
			target.RemoveCondition<ConSuspend>();
			target.MoveNeighborDefinitely();
			target.PlaySound("ding_potential");
			target.pos.PlayEffect("mutation");
		}
		else
		{
			LayerDragGrid.Create(new InvOwnerGene(owner, target));
		}
		return true;
	}
}
