public class ReligionTrickery : ReligionMinor
{
	public override string id => "trickery";

	public override int GetOfferingMtp(Thing t)
	{
		switch (t.id)
		{
		case "1134":
		case "1218":
		case "mochi":
		case "kagamimochi":
			return 2;
		default:
			return 0;
		}
	}
}
