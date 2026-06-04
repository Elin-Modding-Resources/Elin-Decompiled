public class ReligionStrife : ReligionMinor
{
	public override string id => "strife";

	public override bool IsValidArtifact(string id)
	{
		return id == "warmonger";
	}

	public override string[] GetValidArtifacts()
	{
		return new string[1] { "warmonger" };
	}

	public override bool IsFaithElement(Element e)
	{
		switch (e.id)
		{
		case 423:
		case 460:
		case 463:
		case 464:
		case 465:
		case 468:
			return true;
		default:
			return false;
		}
	}
}
