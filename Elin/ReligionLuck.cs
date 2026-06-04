public class ReligionLuck : Religion
{
	public override string id => "luck";

	public override bool IsAvailable => true;

	public override void OnBecomeBranchFaith()
	{
	}

	public override bool IsValidArtifact(string id)
	{
		return id == "luckydagger";
	}

	public override string[] GetValidArtifacts()
	{
		return new string[1] { "luckydagger" };
	}

	public override bool IsFaithElement(Element e)
	{
		return e.id != 426;
	}
}
