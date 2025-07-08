using UnityEngine;

public class StanceSongSleep : BaseSong
{
	public override void OnStart()
	{
		owner.ShowEmo(Emo.happy);
	}

	public override void Tick()
	{
		if (owner.HasCondition<ConSilence>())
		{
			return;
		}
		int num = 0;
		foreach (Chara item in owner.pos.ListCharasInRadius(owner, 4, (Chara c) => !c.IsDeadOrSleeping && c.IsHostile(owner)))
		{
			if ((item.IsPowerful ? 10 : 30) * Mathf.Min(base.power / 4, 100) / 100 > EClass.rnd(100))
			{
				item.AddCondition<ConSleep>(50 + base.power / 2);
			}
			num++;
		}
		if (num > 0)
		{
			owner.mana.Mod(-(1 + owner.mana.max / 200));
		}
	}
}
