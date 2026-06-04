public class ReligionHealing : Religion
{
	public override string id => "healing";

	public override bool IsAvailable => true;

	public override void OnBecomeBranchFaith()
	{
	}

	public override bool IsValidArtifact(string id)
	{
		return id == "pole_holy";
	}

	public override string[] GetValidArtifacts()
	{
		return new string[1] { "pole_holy" };
	}

	public override bool IsFaithElement(Element e)
	{
		int num = e.id;
		if (num == 60 || num == 423 || num == 461)
		{
			return true;
		}
		return false;
	}
}
