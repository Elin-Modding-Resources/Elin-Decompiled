using System.Collections.Generic;

public class AI_Clean : AIAct
{
	public Point pos;

	public int maxRadius = -1;

	public static Point GetCleanPoint(Chara c, int r = -1, int tries = 10)
	{
		Point point = new Point();
		if (r != -1)
		{
			for (int i = 0; i < tries; i++)
			{
				if (i == 0)
				{
					point.Set(c.pos);
				}
				else
				{
					point.Set(c._x - r + EClass.rnd(r * 2 + 1), c.pos.z - r + EClass.rnd(r * 2 + 1));
				}
				if (!point.IsValid)
				{
					continue;
				}
				if ((point.HasDirt && !EClass._map.config.retainDecal) || point.cell.HasLiquid)
				{
					return point;
				}
				foreach (Card item in point.ListCards())
				{
					_ = item;
				}
			}
		}
		return null;
	}

	public override IEnumerable<Status> Run()
	{
		while (true)
		{
			if (pos == null)
			{
				pos = GetCleanPoint(owner, 4);
			}
			if (pos != null)
			{
				break;
			}
			yield return DoIdle(10);
		}
		yield return DoGoto(pos);
		if (owner.pos.HasDirt || owner.pos.cell.HasLiquid)
		{
			EClass._map.SetDecal(pos.x, pos.z);
			EClass._map.SetLiquid(pos.x, pos.z, 0, 0);
			pos.PlayEffect("vanish");
			owner.Say("clean", owner);
			owner.PlaySound("clean_floor");
			yield return KeepRunning();
		}
		List<Card> list = owner.pos.ListCards();
		bool flag = false;
		foreach (Card item in list)
		{
			if (item.isThing)
			{
				_ = item.IsInstalled;
			}
		}
		if (flag)
		{
			owner.Talk("clean");
		}
		yield return Success();
	}
}
