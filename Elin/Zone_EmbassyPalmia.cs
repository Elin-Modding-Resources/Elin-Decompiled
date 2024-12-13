public class Zone_EmbassyPalmia : Zone_Civilized
{
	public override ZoneTransition.EnterState RegionEnterState => ZoneTransition.EnterState.Bottom;

	public override bool AllowCriminal => false;

	public override bool AllowInvest => true;
}
