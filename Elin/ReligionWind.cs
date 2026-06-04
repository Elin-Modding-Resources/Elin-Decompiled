public class ReligionWind : Religion
{
	public override string id => "wind";

	public override void OnReforge(Thing t)
	{
		t.c_idDeity = id;
		if (t.id == "windbow")
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
		if (!(id == "windbow"))
		{
			return id == "shirt_wind";
		}
		return true;
	}

	public override string[] GetValidArtifacts()
	{
		return new string[2] { "windbow", "shirt_wind" };
	}

	public override bool IsFaithElement(Element e)
	{
		if (!(e is Resistance) && e.id != 226 && e.id != 152)
		{
			return e.id != 77;
		}
		return false;
	}
}
