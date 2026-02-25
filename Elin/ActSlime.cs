public class ActSlime : ActNTR
{
	public override bool IsHostileAct => true;

	public override bool CanPerform()
	{
		if (Act.TC == null || !Act.TC.isChara || Act.TC == Act.CC)
		{
			return false;
		}
		return true;
	}

	public override bool ValidatePerform(Chara _cc, Card _tc, Point _tp)
	{
		if (Act.CC.hunger.GetPhase() < 3 && !EClass.debug.godFood)
		{
			Msg.Say("not_hungry");
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
