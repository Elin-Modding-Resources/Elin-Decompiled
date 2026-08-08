public class StanceSongPebble : BaseSong
{
	public override int IdAbility => 6754;

	public override void TickSong()
	{
		foreach (Chara item in owner.pos.ListCharasInRadius(owner, 4, (Chara c) => !c.IsDeadOrSleeping && ((!owner.IsPCFactionOrMinion) ? (!c.IsHostile(owner)) : c.IsPCFactionOrMinion)))
		{
			if (!item.HasCondition<ConSongPebble>())
			{
				item.AddCondition<ConSongPebble>(base.power, owner.Evalue(1294));
			}
			else
			{
				item.GetCondition<ConSongPebble>()?.SetValue(10 + owner.Evalue(1294) * 2, owner.Evalue(1294));
			}
		}
	}
}
