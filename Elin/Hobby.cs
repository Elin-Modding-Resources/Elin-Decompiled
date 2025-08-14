using System;
using UnityEngine;

public class Hobby : EClass
{
	public int id;

	public SourceHobby.Row source => EClass.sources.hobbies.map[id];

	public string Name => source.GetName();

	public AIWork GetAI(Chara c)
	{
		AIWork aIWork = null;
		string text = "AIWork_" + source.ai.IsEmpty(source.alias);
		aIWork = ((!(Type.GetType(text + ", Elin") != null)) ? new AIWork() : ClassCache.Create<AIWork>(text, "Elin"));
		aIWork.owner = c;
		aIWork.sourceWork = source;
		return aIWork;
	}

	public int GetLv(Chara c)
	{
		if (!source.skill.IsEmpty())
		{
			return Mathf.Min(c.Evalue(source.skill), 100000);
		}
		return Mathf.Min(c.LV, 100000);
	}

	public int GetEfficiency(Chara c)
	{
		int num = 50;
		FactionBranch factionBranch = ((c.currentZone != null) ? c.homeBranch : EClass._zone?.branch);
		if (factionBranch == null || (c.currentZone != null && c.currentZone != factionBranch.owner))
		{
			return 0;
		}
		if (source.alias == "Breeding")
		{
			return c.race.breeder;
		}
		if (c.currentZone == null || c.currentZone == EClass._zone)
		{
			if ((!source.destTrait.IsEmpty() || !source.workTag.IsEmpty()) && !GetAI(c).SetDestination())
			{
				return 0;
			}
			if (c.noMove && c.pos != null && c.pos.FindThing<TraitGeneratorWheel>() != null)
			{
				return 0;
			}
			if (c.memberType != FactionMemberType.Livestock)
			{
				TraitBed traitBed = c.FindBed();
				if (traitBed != null)
				{
					num += 30 + traitBed.owner.GetTotalQuality() + traitBed.owner.Evalue(750);
				}
			}
		}
		num += GetLv(c);
		if (c.affinity.value < 0)
		{
			num += c.affinity.value;
		}
		num = num * (100 + factionBranch.Evalue(3708) * 10) / 100;
		if (num >= 0)
		{
			return num;
		}
		return 0;
	}
}
