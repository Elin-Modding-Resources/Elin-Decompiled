public class ActNTR : Ability
{
	public override bool CanPerform()
	{
		if (Act.TC.isChara)
		{
			return Act.TC.Chara.conSleep != null;
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
