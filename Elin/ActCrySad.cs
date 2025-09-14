public class ActCrySad : Ability
{
	public override bool Perform()
	{
		Act.CC.PlaySound("warcry");
		Act.CC.Say("abSadcry", Act.CC);
		foreach (Chara chara in EClass._map.charas)
		{
			if (!chara.IsInMutterDistance() || EClass.rnd(2) != 0)
			{
				continue;
			}
			if (Act.CC.IsHostile(chara))
			{
				switch (EClass.rnd(3))
				{
				case 0:
					chara.AddCondition<ConFear>(5, force: true);
					break;
				case 1:
					chara.AddCondition<ConSupress>(5, force: true);
					break;
				case 2:
					chara.AddCondition<ConSilence>(5, force: true);
					break;
				}
			}
			else if (chara != Act.CC)
			{
				chara.AddCondition<ConBerserk>(200);
			}
		}
		return true;
	}
}
