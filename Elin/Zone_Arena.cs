public class Zone_Arena : Zone
{
	public override bool RestrictBuild => true;

	public override bool AllowCriminal => true;

	public override ZoneScaleType ScaleType
	{
		get
		{
			if (base._dangerLv <= 50)
			{
				return ZoneScaleType.None;
			}
			return ZoneScaleType.Void;
		}
	}

	public override bool MakeTownProperties => true;

	public override bool UseFog => true;
}
