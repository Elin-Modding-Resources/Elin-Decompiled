using UnityEngine;

public class StanceSongSleep : BaseSong
{
	public override void TickSong()
	{
		foreach (Chara item in owner.pos.ListCharasInRadius(owner, 4, (Chara c) => !c.IsDeadOrSleeping && c.IsHostile(owner)))
		{
			if ((item.IsPowerful ? 10 : 30) * Mathf.Min(base.power / 4, 100) / 100 > EClass.rnd(100))
			{
				item.AddCondition<ConSleep>(50 + base.power / 2);
			}
		}
	}
}
