public class Zone_Gathering : Zone_RandomDungeon
{
	public override int StartLV => 0;

	public override int LvBoss => 0;

	public override string IDGenerator => null;

	public override ZoneTransition.EnterState RegionEnterState => ZoneTransition.EnterState.Dir;

	public override string GetDungenID()
	{
		return null;
	}
}
