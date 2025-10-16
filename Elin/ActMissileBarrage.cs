using System.Collections.Generic;
using System.Linq;

public class ActMissileBarrage : Ability
{
	public override bool Perform()
	{
		List<Point> list = new List<Point>();
		for (int i = 0; i < 10; i++)
		{
			Point p = Act.CC.pos.GetRandomPointInRadius(4, 9);
			if (p != null)
			{
				IEnumerable<Point> enumerable = list.Where((Point _p) => _p.Equals(p));
				if (enumerable != null && enumerable.Count() == 0)
				{
					list.Add(p);
				}
			}
		}
		Act.CC.Say("abMissileBarrage", Act.CC);
		if (list.Count > 0)
		{
			Chara cC = Act.CC;
			foreach (Point item in list)
			{
				Act.CC = cC;
				Act.TP.Set(item);
				ActEffect.ProcAt(EffectId.Rocket, GetPower(Act.CC) / 2, BlessedState.Normal, Act.CC, null, Act.TP, isNeg: true, new ActRef
				{
					origin = Act.CC.Chara,
					aliasEle = "eleImpact"
				});
				ActEffect.RapidDelay = 0.05f;
				ActEffect.RapidCount++;
			}
		}
		return true;
	}
}
