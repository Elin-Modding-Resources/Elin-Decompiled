public class GlobalGoalAdv : GlobalGoal
{
	public override void OnAdvanceHour()
	{
		if (!owner.isDead && !owner.IsPCFaction && owner.currentZone != EClass._zone && !owner.GetBool("StayHomeZone") && ((EClass.debug.enable && EClass.rnd(24) == 0) || (hours > 720 && EClass.rnd(720) == 0)))
		{
			Zone randomTown = EClass.world.region.GetRandomTown();
			if (randomTown != owner.currentZone)
			{
				owner.MoveZone(randomTown, ZoneTransition.EnterState.RandomVisit);
			}
		}
	}
}
