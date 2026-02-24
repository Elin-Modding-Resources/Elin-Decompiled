public class ActKiss : Ability
{
	public override bool CanPressRepeat => true;

	public override bool CanPerform()
	{
		if (Act.TC == null || !Act.TC.isChara)
		{
			return false;
		}
		return true;
	}

	public override bool Perform()
	{
		Act.CC.Kiss(Act.TC.Chara);
		return true;
	}
}
