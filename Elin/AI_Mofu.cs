using System.Collections.Generic;
using System.Linq;

public class AI_Mofu : AIWork
{
	public Chara mofu;

	public override int MaxRestart => 100;

	public Chara GetMofu()
	{
		return EClass._map.charas.Where((Chara c) => c.IsMofuable && c != owner).RandomItem();
	}

	public override IEnumerable<Status> Run()
	{
		yield return DoWait(3 + EClass.rnd(10));
		if (mofu == null)
		{
			mofu = GetMofu();
		}
		if (mofu == null || !mofu.ExistsOnMap)
		{
			yield return Success();
		}
		if (owner.Dist(mofu) > 1)
		{
			yield return DoGoto(mofu);
		}
		if (mofu == null || !mofu.ExistsOnMap)
		{
			yield return Success();
		}
		owner.Cuddle(mofu);
		if (EClass.rnd(222) != 0)
		{
			yield return Restart();
		}
	}
}
