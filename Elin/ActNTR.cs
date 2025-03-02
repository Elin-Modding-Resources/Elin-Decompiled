public class ActNTR : Ability
{
	public override bool CanPerform()
	{
		if (Act.TC.isChara)
		{
			if (Act.TC.Chara.conSleep == null)
			{
				return Act.CC.HasElement(1239);
			}
			return true;
		}
		return false;
	}

	public override bool Perform()
	{
		Act.CC.SetAI(new AI_Fuck
		{
			target = Act.TC.Chara,
			bitch = true,
			ntr = true
		});
		return true;
	}
}
