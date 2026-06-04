public class ReligionEarth : Religion
{
	public override string id => "earth";

	public override bool IsAvailable => true;

	public override void OnBecomeBranchFaith()
	{
	}

	public override bool IsValidArtifact(string id)
	{
		return id == "blunt_earth";
	}

	public override string[] GetValidArtifacts()
	{
		return new string[1] { "blunt_earth" };
	}

	public override bool IsFaithElement(Element e)
	{
		switch (e.id)
		{
		case 55:
		case 56:
		case 70:
		case 421:
		case 423:
		case 954:
			return true;
		default:
			return false;
		}
	}
}
