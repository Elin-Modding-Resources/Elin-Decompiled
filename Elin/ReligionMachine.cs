public class ReligionMachine : Religion
{
	public override string id => "machine";

	public override void OnReforge(Thing t)
	{
		t.c_idDeity = id;
		if (t.id == "gun_mani")
		{
			return;
		}
		foreach (Element value in t.elements.dict.Values)
		{
			if (IsFaithElement(value))
			{
				value.vExp = -1;
			}
		}
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
