using System.Linq;

public class ActSwarm : Ability
{
	public override int PerformDistance => 3;

	public override bool CanPerform()
	{
		if (Act.CC == Act.TC || Act.TC == null || Act.CC.Dist(Act.TC) > PerformDistance)
		{
			return false;
		}
		return base.CanPerform();
	}

	public override bool Perform()
	{
		float num = 0f;
		Card tC = Act.TC;
		foreach (Card item in EClass._map.Cards.ToList())
		{
			if (!Act.CC.IsAliveInCurrentZone)
			{
				break;
			}
			if (item.IsAliveInCurrentZone && item != Act.CC && (!item.isChara || item == tC || item.Chara.IsHostile(Act.CC)) && (item.isChara || item.trait.CanBeAttacked) && item.Dist(Act.CC) <= PerformDistance && Act.CC.CanSeeLos(item))
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
