public class ReligionMoonShadow : ReligionMinor
{
	public override string id => "moonshadow";

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

	public override bool IsValidArtifact(string id)
	{
		return id == "sword_muramasa2";
	}

	public override string[] GetValidArtifacts()
	{
		return new string[1] { "sword_muramasa2" };
	}

	public override bool IsFaithElement(Element e)
	{
		int num = e.id;
		if (num == 401 || num == 661 || num == 916)
		{
			return true;
		}
		return false;
	}
}
