using System.Collections.Generic;
using System.Linq;

public class TraitBookSkill : TraitScroll
{
	public int idEle => owner.refVal;

	public override SourceElement.Row source => EClass.sources.elements.map[idEle];

	public virtual bool IsPlan => false;

	public override string IdNoRestock => owner.id + "_" + source?.id;

	public override bool IsOnlyUsableByPc
	{
		get
		{
			if (source != null)
			{
				return source.category == "ability";
			}
			return false;
		}
	}

	public override bool CanRead(Chara c)
	{
		return !c.isBlind;
	}

	public override int GetActDuration(Chara c)
	{
		return 5;
	}

	public override void OnCreate(int lv)
	{
		IEnumerable<SourceElement.Row> ie = EClass.sources.elements.rows.Where(delegate(SourceElement.Row a)
		{
			if (a.chance == 0)
			{
				return false;
			}
			if (a.tag.Contains("hidden") || a.tag.Contains("unused"))
			{
				return false;
			}
			if (IsPlan)
			{
				if (a.category != "policy" && a.category != "tech")
				{
					return false;
				}
				if (a.cost.Length == 0 || (a.category == "tech" && a.cost[0] == 0))
				{
					return false;
				}
			}
			else if (a.category != "skill")
			{
				return false;
			}
			return true;
		});
		owner.refVal = ie.RandomItem().id;
	}

	public override void SetName(ref string s)
	{
		if (idEle != 0)
		{
			string text = "";
			if ((EClass.sources.elements.map.TryGetValue(idEle) ?? EClass.sources.elements.map[0]).category == "policy")
			{
				text = " (" + "policy".lang() + ")";
			}
			s = "_of".lang((source.GetName() + text).Bracket(1), s);
			if (IsPlan && EClass.pc.homeBranch != null && EClass.pc.homeBranch.elements.HasBase(idEle))
			{
				s = s + " " + "alreadyLearned".lang();
			}
		}
	}

	public override void OnRead(Chara c)
	{
		if (IsPlan && !c.IsPC)
		{
			return;
		}
		if (IsPlan && !EClass._zone.IsPCFaction)
		{
			owner.Say("skillbook_invalidZone");
			return;
		}
		if (IsPlan && EClass.Branch.elements.HasBase(idEle))
		{
			owner.Say("skillbook_knownSkill", c, source.GetName());
			return;
		}
		if (IsOnlyUsableByPc && !c.IsPC)
		{
			owner.SayNothingHappans();
			return;
		}
		owner.Say(IsPlan ? "skillbook_learnPlan" : "skillbook_learn", c, source.GetName());
		if (IsPlan)
		{
			EClass.Branch.elements.Learn(idEle);
			if (source.category == "policy" && !EClass.Branch.policies.HasPolicy(idEle))
			{
				EClass.Branch.policies.AddPolicy(idEle);
			}
			foreach (FactionBranch child in EClass.pc.faction.GetChildren())
			{
				child.ValidateUpgradePolicies();
			}
		}
		else if (!c.elements.HasBase(idEle))
		{
			c.elements.Learn(idEle);
		}
		else
		{
			c.elements.ModExp(idEle, owner.IsBlessed ? 1500 : (owner.IsCursed ? 500 : 1000));
		}
		c.Say("spellbookCrumble", owner.Duplicate(1));
		owner.ModNum(-1);
	}

	public override int GetValue()
	{
		return owner.sourceCard.value;
	}

	public override void WriteNote(UINote n, bool identified)
	{
		base.WriteNote(n, identified);
		if (IsPlan)
		{
			return;
		}
		n.Space();
		foreach (Chara member in EClass.pc.party.members)
		{
			bool flag = member.elements.HasBase(idEle);
			n.AddText("_bullet".lang() + member.Name + " " + (flag ? "alreadyLearned" : "notLearned").lang(), flag ? FontColor.Good : FontColor.Warning);
		}
	}
}
