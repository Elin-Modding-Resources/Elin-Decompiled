public class ReligionElement : Religion
{
	public override string id => "element";

	public override bool IsValidArtifact(string id)
	{
		return id == "staff_element";
	}

	public override string[] GetValidArtifacts()
	{
		return new string[1] { "staff_element" };
	}

	public override bool IsFaithElement(Element e)
	{
		if (e.id != 411)
		{
			if (e is Resistance)
			{
				return e.id != 959;
			}
			return false;
		}
		return true;
	}
}
