public class Zone_StartSite : Zone
{
	public override bool UseFog => base.lv < 0;

	public override bool isClaimable => true;

	public override string IDBaseLandFeat => "bfPlain,bfFertile,bfStart";

	public override void OnGenerateMap()
	{
	}
}
