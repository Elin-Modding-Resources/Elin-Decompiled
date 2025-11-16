public class ActNTR : Ability
{
	public override bool CanPerform()
	{
		if (Act.TC == null || !Act.TC.isChara)
		{
			return false;
		}
		Chara chara = Act.TC.Chara;
		bool flag = chara.things.Find<TraitDreamBug>() != null;
		if (!flag && Act.TC.Evalue(418) > 0)
		{
			return false;
		}
		if (chara.conSleep != null || Act.CC.HasElement(1239) || chara.Evalue(418) < 0)
		{
			return true;
		}
		if (flag)
		{
			if (!chara.IsDisabled && !chara.isConfused && !chara.HasCondition<ConFreeze>() && !chara.HasCondition<ConDim>())
			{
				return chara.HasCondition<ConFear>();
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
			variation = AI_Fuck.Variation.NTR
		});
		return true;
	}
}
