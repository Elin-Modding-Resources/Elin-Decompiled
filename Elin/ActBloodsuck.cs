public class ActBloodsuck : ActNTR
{
	public override bool IsHostileAct => true;

	public override bool CanPerform()
	{
		if (Act.TC == null || !Act.TC.isChara || Act.TC == Act.CC)
		{
			return false;
		}
		if (Act.TC.Evalue(964) > 0)
		{
			return false;
		}
		if (Act.TC.things.Find((Thing a) => a.HasElement(432)) != null)
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
			variation = AI_Fuck.Variation.Bloodsuck
		});
		return true;
	}
}
