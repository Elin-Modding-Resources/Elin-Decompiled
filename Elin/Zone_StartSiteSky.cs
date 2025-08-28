public class Zone_StartSiteSky : Zone_StartSite
{
	public override bool UseFog => false;

	public override bool IsSkyLevel => true;

	public override string IDBaseLandFeat => "bfPlain,bfFertile,bfStart";

	public override bool BlockBorderExit => true;

	public override string IdBiome => "Plain";

	public override ZoneScaleType ScaleType
	{
		get
		{
			if (!EClass.game.IsSurvival || EClass.game.survival.flags.raidLv < 100)
			{
				return ZoneScaleType.None;
			}
			return ZoneScaleType.Void;
		}
	}
}
