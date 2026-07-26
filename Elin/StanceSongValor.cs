public class StanceSongValor : BaseSong
{
	public override int IdAbility => 6752;

	public override void TickSong()
	{
		foreach (Chara item in owner.pos.ListCharasInRadius(owner, 4, (Chara c) => !c.IsDeadOrSleeping && ((!owner.IsPCFactionOrMinion) ? (!c.IsHostile(owner)) : c.IsPCFactionOrMinion)))
		{
			if (!item.HasCondition<ConSongValor>())
			{
				item.AddCondition<ConSongValor>(base.power);
			}
			else
			{
				item.GetCondition<ConSongValor>()?.SetValue(10);
			}
		}
	}
}
