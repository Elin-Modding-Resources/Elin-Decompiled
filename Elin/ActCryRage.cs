public class ActCryRage : Ability
{
	public override bool CanPerform()
	{
		return true;
	}

	public override bool Perform()
	{
		Act.CC.PlaySound("warcry");
		Act.CC.Say("abSoulcry", Act.CC);
		foreach (Chara chara in EClass._map.charas)
		{
			if (chara.IsInMutterDistance() && EClass.rnd(2) == 0)
			{
				chara.AddCondition<ConBerserk>();
			}
		}
		return true;
	}
}
