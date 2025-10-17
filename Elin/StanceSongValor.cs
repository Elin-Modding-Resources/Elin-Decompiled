public class StanceSongValor : BaseSong
{
	public override void TickSong()
	{
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
		}
	}
}
