public class StanceSongSleep : BaseSong
{
	public override void OnStart()
	{
		owner.ShowEmo(Emo.happy);
	}

	public override void Tick()
	{
		int num = 0;
		foreach (Chara item in owner.pos.ListCharasInRadius(owner, 4, (Chara c) => !c.IsDeadOrSleeping && c.IsHostile(owner)))
		{
			if ((item.IsPowerful ? 10 : 25) > EClass.rnd(100))
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
