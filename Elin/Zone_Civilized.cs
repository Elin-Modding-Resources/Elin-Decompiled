public class Zone_Civilized : Zone
{
	public override bool ShouldRegenerate => true;

	public override bool HasLaw => true;

	public override bool AllowCriminal => true;

	public override string IDHat
	{
		get
		{
			if (EClass.world.date.month != 12)
			{
				return base.IDHat;
			}
			return "hat_santa";
		}
	}
}
