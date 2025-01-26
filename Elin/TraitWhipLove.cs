using System;
using System.Collections.Generic;

public class TraitWhipLove : TraitTool
{
	public override bool HasCharges => true;

	public override void OnCreate(int lv)
	{
		owner.c_charges = EClass.rnd(7) + 3;
	}

	public override void TrySetHeldAct(ActPlan p)
	{
		p.pos.ListCards().ForEach(delegate(Card a)
		{
			Chara c = a.Chara;
			if (c != null)
			{
				List<Hobby> list = c.ListWorks(useMemberType: false);
				List<Hobby> list2 = c.ListHobbies(useMemberType: false);
				if (p.IsSelfOrNeighbor && EClass.pc.CanSee(a) && c.IsPCFaction && c.homeBranch != null && list.Count > 0 && list2.Count > 0)
				{
					p.TrySetAct("actWhip", delegate
					{
						EClass.pc.Say("use_whip", c, owner);
						EClass.pc.Say("use_scope2", c);
						c.Talk("pervert2");
						EClass.pc.PlaySound("whip");
						c.PlayAnime(AnimeID.Shiver);
						c.OnInsulted();
						if (this is TraitWhipInterest)
						{
							c.bio.idInterest = EClass.rnd(Enum.GetNames(typeof(Interest)).Length);
							c.SetRandomTone();
							c.SetRandomTalk();
							EClass.pc.Say("use_whip4", c);
						}
						else
						{
							c.RerollHobby();
							Hobby hobby = c.ListWorks(useMemberType: false)[0];
							Hobby hobby2 = c.ListHobbies(useMemberType: false)[0];
							EClass.pc.Say("use_whip2", c, hobby.Name, hobby2.Name);
							c.RefreshWorkElements(c.homeBranch.elements);
						}
						owner.ModCharge(-1);
						if (owner.c_charges <= 0)
						{
							EClass.pc.Say("spellbookCrumble", owner);
							owner.Destroy();
						}
						return true;
					}, c);
				}
			}
		});
	}
}
