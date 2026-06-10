public class ReligionMachine : Religion
{
	public override string id => "machine";

	public override bool IsIgnoreReforge(Thing t)
	{
		return t.id == "gun_mani";
	}

	public override bool IsValidArtifact(string id)
	{
		if (!(id == "gun_mani"))
		{
			return id == "cloak_mani";
		}
		return true;
	}

	public override string[] GetValidArtifacts()
	{
		return new string[2] { "gun_mani", "cloak_mani" };
	}

	public override bool IsFaithElement(Element e)
	{
		switch (e.id)
		{
		case 105:
		case 427:
		case 466:
		case 664:
		case 957:
			return true;
		default:
			return false;
		}
	}
}
