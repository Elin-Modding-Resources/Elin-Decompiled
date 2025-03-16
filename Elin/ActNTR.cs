public class ActNTR : Ability
{
	public override bool CanPerform()
	{
		if (Act.TC.isChara && (Act.TC.Chara.conSleep != null || Act.CC.HasElement(1239) || Act.TC.Evalue(418) < 0))
		{
			return Act.TC.Evalue(418) <= 0;
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
