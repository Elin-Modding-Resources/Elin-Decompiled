public class ReligionHarvest : Religion
{
	public override string id => "harvest";

	public override bool IsValidArtifact(string id)
	{
		return id == "scythe_kumi";
	}

	public override string[] GetValidArtifacts()
	{
		return new string[1] { "scythe_kumi" };
	}

	public override bool IsFaithElement(Element e)
	{
		switch (e.id)
		{
		case 428:
		case 480:
		case 640:
		case 665:
		case 959:
		case 6650:
			return true;
		default:
			return false;
		}
	}
}
