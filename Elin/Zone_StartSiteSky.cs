public class Zone_StartSiteSky : Zone_StartSite
{
	public override bool UseFog => false;

	public override bool IsSkyLevel => true;

	public override string IDBaseLandFeat => "bfPlain,bfFertile,bfStart";

	public override bool BlockBorderExit => true;
}
