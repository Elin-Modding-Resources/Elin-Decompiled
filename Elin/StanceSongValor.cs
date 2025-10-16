public class StanceSongValor : BaseSong
{
	public override void OnStart()
	{
		owner.ShowEmo(Emo.happy);
	}

	public override void Tick()
	{
		if (owner.HasCondition<ConSilence>() || EClass._zone.IsRegion)
		{
			return;
		}
		int num = 0;
		foreach (Chara item in owner.pos.ListCharasInRadius(owner, 4, (Chara c) => (c.IsDeadOrSleeping || !owner.IsPCFactionOrMinion) ? (!c.IsHostile(owner)) : c.IsPCFactionOrMinion))
		{
			if (!item.HasCondition<ConEuphoric>())
			{
				item.AddCondition<ConEuphoric>(base.power);
			}
			if (!item.HasCondition<ConHero>() && (item.HasCondition<ConFear>() || item.HasCondition<ConConfuse>()))
			{
				item.AddCondition<ConHero>(base.power);
			}
			num++;
		}
		if (num > 0)
		{
			owner.mana.Mod(-(1 + owner.mana.max / 200));
		}
	}
}
