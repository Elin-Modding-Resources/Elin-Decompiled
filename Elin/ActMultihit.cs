public class ActMultihit : Ability
{
	public override bool CanPerform()
	{
		if (Act.TC == null)
		{
			return false;
		}
		return base.CanPerform();
	}

	public override bool Perform()
	{
		int num = 0;
		Card orgTC = Act.TC;
		int num2 = 4 + EClass.rnd(6);
		for (int i = 0; i < num2; i++)
		{
			if (!Act.CC.IsAliveInCurrentZone || !orgTC.IsAliveInCurrentZone)
			{
				break;
			}
			bool anime = i % 4 == 0;
			TweenUtil.Delay((float)num * 0.07f, delegate
			{
				if (anime)
				{
					orgTC.pos.PlayEffect("ab_bladestorm");
				}
				orgTC.pos.PlaySound("ab_swarm");
			});
			num++;
			new ActMeleeBladeStorm().Perform(Act.CC, orgTC);
		}
		return true;
	}
}
