public class ActTimeStop : Ability
{
	public override bool Perform()
	{
		foreach (Chara chara in EClass._map.charas)
		{
			chara.AddCondition(Condition.Create(GetPower(Act.CC), delegate(ConTimeStop con)
			{
				con.uidMaster = Act.CC.uid;
			}), force: true);
		}
		return true;
	}
}
