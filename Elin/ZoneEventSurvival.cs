public class ZoneEventSurvival : ZoneEvent
{
	public override void OnTickRound()
	{
		if (EClass._map.cells.GetLength(0) <= 100)
		{
			return;
		}
		Cell cell = EClass._map.cells[100, 100];
		if (!cell.HasObj)
		{
			EClass._map.SetObj(cell.x, cell.z, 46);
		}
		if (EClass.game.survival.flags.raid)
		{
			TraitVoidgate traitVoidgate = EClass._map.FindThing<TraitVoidgate>();
			if (traitVoidgate != null)
			{
				traitVoidgate.owner.isOn = EClass.game.survival.IsInRaid;
			}
			if (!EClass.game.survival.IsInRaid)
			{
				EClass.world.date.GetRemainingHours(EClass.game.survival.flags.dateNextRaid);
				_ = 0;
			}
		}
	}
}
