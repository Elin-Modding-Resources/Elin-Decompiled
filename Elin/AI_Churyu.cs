using System.Collections.Generic;

public class AI_Churyu : AIWork
{
	public Card churyu;

	public Chara slave;

	public override int MaxRestart => 100000;

	public override IEnumerable<Status> Run()
	{
		if (!slave.ExistsOnMap || churyu.GetRootCard() != slave)
		{
			yield return Success();
		}
		if (owner.Dist(slave) < 2)
		{
			if (owner.TalkTopic().IsEmpty())
			{
				owner.Talk("idle");
			}
			owner.PlaySound("Animal/Cat/cat");
			yield return DoWait(1 + EClass.rnd(3));
		}
		else
		{
			yield return DoGoto(slave.pos.GetNearestPoint(allowBlock: false, allowChara: false));
		}
		yield return Restart();
	}
}
