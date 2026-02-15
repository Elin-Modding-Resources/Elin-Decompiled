public class ActSlime : ActNTR
{
	public override bool IsHostileAct => true;

	public override bool CanPerform()
	{
		if (Act.CC.hunger.GetPhase() < 3 && !EClass.debug.enable)
		{
			return false;
		}
		if (Act.TC == null || Act.TC == Act.CC)
		{
			return false;
		}
		return true;
	}

	public override bool Perform()
	{
		Act.CC.SetAI(new AI_Fuck
		{
			target = Act.TC.Chara,
			variation = AI_Fuck.Variation.Slime
		});
		return true;
	}
}
