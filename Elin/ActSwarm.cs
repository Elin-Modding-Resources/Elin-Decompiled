using System.Linq;

public class ActSwarm : Ability
{
	public override bool CanPerform()
	{
		if (Act.TC == null || !Act.CC.IsHostile(Act.TC.Chara))
		{
			return false;
		}
		return base.CanPerform();
	}

	public override bool Perform()
	{
		float num = 0f;
		foreach (Chara item in EClass._map.charas.ToList())
		{
			if (!Act.CC.IsAliveInCurrentZone)
			{
				break;
			}
			if (item.IsAliveInCurrentZone && item != Act.CC && item.IsHostile(Act.CC) && Act.CC.CanSeeLos(item))
			{
				Point pos = item.pos;
				TweenUtil.Delay(num, delegate
				{
					pos.PlayEffect("ab_swarm");
					pos.PlaySound("ab_swarm");
				});
				if (num < 1f)
				{
					num += 0.07f;
				}
				new ActMeleeSwarm().Perform(Act.CC, item);
			}
		}
		return true;
	}
}
