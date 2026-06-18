using System.Collections.Generic;
using UnityEngine;

public class ActWater : Act
{
	public TraitToolWaterCan waterCan;

	public override CursorInfo CursorIcon => CursorSystem.Hand;

	public override TargetType TargetType => TargetType.SelfAndNeighbor;

	public override bool CanPerform()
	{
		return IsWaterCanValid(msg: false);
	}

	public override bool Perform()
	{
		Act.CC.Say("water_ground", Act.CC);
		int num = waterCan.owner.Evalue(770);
		num = ((num <= 0) ? 1 : Mathf.Min(waterCan.owner.c_charges, 2 + num / 10));
		if (num > 1)
		{
			List<Point> list = EClass._map.ListPointsInSquare(Act.TP, num - 1);
			list.Sort((Point a, Point b) => a.Distance(Act.TP) - b.Distance(Act.TP));
			foreach (Point item in list)
			{
				if (!IsWaterCanValid())
				{
					break;
				}
				Water(item);
			}
		}
		else
		{
			Water(Act.TP);
		}
		Act.CC.PlaySound("water_farm");
		waterCan.owner.ModCharge(-num);
		return base.Perform();
		static void Water(Point pos)
		{
			if (!pos.cell.IsTopWater && !pos.cell.IsSnowTile)
			{
				pos.cell.isWatered = true;
			}
			foreach (Chara chara in pos.Charas)
			{
				bool flag = false;
				if (chara.HasCondition<ConBurning>())
				{
					flag = true;
					chara.Talk("thanks");
				}
				else if (!chara.IsPCParty && EClass.rnd(2) == 0)
				{
					chara.Say("water_evade", chara);
					if (!chara.IsHostile())
					{
						chara.Talk("scold");
					}
					continue;
				}
				chara.AddCondition<ConWet>();
				if (!flag)
				{
					Act.CC.DoHostileAction(chara);
				}
			}
		}
	}

	public bool IsWaterCanValid(bool msg = true)
	{
		bool num = waterCan != null && waterCan.owner.c_charges > 0;
		if (!num && msg)
		{
			Msg.Say("water_deplete");
		}
		return num;
	}
}
