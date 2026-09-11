public class Zone_OldKeep : Zone_Civilized
{
	public override bool AllowCriminal => true;

	public override ZoneTransition.EnterState RegionEnterState => ZoneTransition.EnterState.Bottom;

	public override string GetNewZoneID(int level)
	{
		if (level <= -1)
		{
			return "oldkeep_dungeon";
		}
		return base.GetNewZoneID(level);
	}
}
