public class Zone_CurryRuin : Zone_Civilized
{
	public override string GetNewZoneID(int level)
	{
		if (level <= -1)
		{
			return "curryruin_dungeon";
		}
		return base.GetNewZoneID(level);
	}
}
