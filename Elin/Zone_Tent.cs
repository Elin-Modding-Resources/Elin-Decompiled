public class Zone_Tent : Zone
{
	public override bool PetFollow => false;

	public override bool AllowNewZone => false;

	public override bool UseFog => true;

	public override bool GrowPlant => true;

	public override int MaxSoil => 50 + Evalue(2200) * 5;

	public override ZoneTransition.EnterState RegionEnterState => ZoneTransition.EnterState.Bottom;

	public override void OnBeforeDeactivate()
	{
		int num = 0;
		foreach (Thing thing in EClass._map.things)
		{
			num += thing.ChildrenAndSelfWeight;
			if (num >= 10000000 || num < 0)
			{
				num = 10000000;
				break;
			}
		}
		SetInt(1, num);
	}
}
