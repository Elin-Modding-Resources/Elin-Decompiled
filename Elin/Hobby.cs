using System;

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
			return c.Evalue(source.skill);
		}
		return c.LV;
	}

	public int GetEfficiency(Chara c)
	{
		int num = 50;
		if (c.homeBranch == null || c.currentZone != c.homeBranch.owner)
		{
			return 0;
		}
		if (c.currentZone == EClass._zone)
		{
			if ((!source.destTrait.IsEmpty() || !source.workTag.IsEmpty()) && !GetAI(c).SetDestination())
			{
				return 0;
			}
			if (c.noMove && c.pos.FindThing<TraitGeneratorWheel>() != null)
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
		if (source.alias == "Breeding")
		{
			num = num * c.race.breeder / 100;
		}
		num += GetLv(c);
		return num * (100 + c.homeBranch.Evalue(3708) * 10) / 100;
	}
}
