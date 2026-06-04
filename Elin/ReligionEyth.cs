public class ReligionEyth : Religion
{
	public override string id => "eyth";

	public override void LeaveFaith(Chara c, Religion newFaith, ConvertType type)
	{
		if (c.IsPC)
		{
			EClass.pc.faction.charaElements.OnLeaveFaith();
		}
		OnLeaveFaith();
		c.RefreshFaithElement();
	}
}
