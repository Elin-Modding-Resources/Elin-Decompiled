using System.Collections.Generic;

public class ActWhirlwind : Ability
{
	public override bool CanPerform()
	{
		if (Act.TC == null)
		{
			return false;
		}
		return ACT.Melee.CanPerform();
	}

	public override bool Perform()
	{
		List<Chara> list = Act.CC.pos.ListCharasInNeighbor((Chara c) => c != Act.CC);
		Act.CC.PlaySound("ab_shred2");
		Act.CC.Say("abWhirlwind", Act.CC);
		foreach (Chara item in list.Copy())
		{
			Act.TC = item;
			if (!Act.CC.IsAliveInCurrentZone || !Act.TC.IsAliveInCurrentZone)
			{
				break;
			}
			Act.TC.PlayEffect("ab_bladestorm");
			new ActMeleeWhilrwind().Perform(Act.CC, Act.TC);
		}
		return true;
	}
}
