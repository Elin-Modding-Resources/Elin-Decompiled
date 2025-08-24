using UnityEngine;

public class ZoneEventSurvival : ZoneEvent
{
	public override void OnTickRound()
	{
		Debug.Log("tick:" + rounds);
		Cell cell = EClass._map.cells[100, 100];
		if (!cell.HasObj)
		{
			EClass._map.SetObj(cell.x, cell.z, 46);
		}
		if (EClass.player.stats.days >= 10 && !EClass.game.survival.flags.raid)
		{
			EClass.game.survival.StartRaid();
		}
		if (EClass.game.survival.flags.raid)
		{
			TraitVoidgate traitVoidgate = EClass._map.FindThing<TraitVoidgate>();
			if (traitVoidgate != null)
			{
				traitVoidgate.owner.isOn = EClass.game.survival.IsInRaid;
			}
			if (!EClass.game.survival.IsInRaid && EClass.world.date.GetRemainingHours(EClass.game.survival.flags.dateNextRaid) <= 0)
			{
				EClass._zone.events.Add(new ZoneEventRaid());
			}
		}
	}
}
