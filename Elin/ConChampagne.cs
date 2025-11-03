using Newtonsoft.Json;
using UnityEngine;

public class ConChampagne : BaseBuff
{
	[JsonProperty]
	public int count;

	public override void Tick()
	{
		count--;
		if (count <= 0)
		{
			count += 15;
			owner.Talk("champagne");
			int num = Mathf.Max(EClass.curve(owner.CHA * 10, 400, 100), 100);
			if (!owner.IsPCFactionOrMinion && num > 500)
			{
				num = 500;
			}
			foreach (Chara chara in EClass._map.charas)
			{
				if (chara != owner && !chara.IsHostile(owner) && owner.Dist(chara) <= 10)
				{
					chara.AddCondition<ConHero>(num);
					chara.AddCondition<ConEuphoric>(num);
					chara.AddCondition<ConSeeInvisible>(num);
				}
			}
		}
		Mod(-1);
	}
}
