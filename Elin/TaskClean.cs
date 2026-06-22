using System.Collections.Generic;
using System.Linq;

public class TaskClean : Task
{
	public Point dest;

	public override string GetText(string str = "")
	{
		return "actClean".lang();
	}

	public static bool CanClean(Point p)
	{
		if (!p.HasDirt)
		{
			return p.cell.HasLiquid;
		}
		return true;
	}

	public override bool CanPerform()
	{
		return CanClean(dest);
	}

	public override bool CanManualCancel()
	{
		return true;
	}

	public override IEnumerable<Status> Run()
	{
		while (true)
		{
			dest = GetTarget(dest);
			if (dest == null)
			{
				yield return Success();
			}
			yield return DoGoto(dest, 1);
			if (!CanClean(dest) || owner.Dist(dest) > 1)
			{
				yield return Cancel();
			}
			for (int i = 0; i < ((!dest.cell.HasLiquid) ? 1 : 5); i++)
			{
				owner.LookAt(dest);
				owner.renderer.NextFrame();
				yield return KeepRunning();
			}
			EClass.pc.Say("clean", owner);
			EClass.pc.PlaySound("clean_floor");
			int num = owner.Tool?.Evalue(770) ?? 0;
			num = ((num <= 0) ? 1 : (2 + num / 10));
			if (num > 1)
			{
				List<Point> list = EClass._map.ListPointsInSquare(dest, num - 1, mustBeWalkable: false);
				list.Sort((Point a, Point b) => a.Distance(dest) - b.Distance(dest));
				foreach (Point item in list)
				{
					if (owner == null || owner.isDead)
					{
						break;
					}
					Clean(item);
				}
			}
			else
			{
				Clean(dest);
			}
			yield return KeepRunning();
		}
		static void Clean(Point p)
		{
			if (CanClean(p))
			{
				EClass._map.SetDecal(p.x, p.z);
				EClass._map.SetLiquid(p.x, p.z, 0, 0);
				p.PlayEffect("vanish");
				EClass.pc.ModExp(293, 30);
				EClass.player.stats.clean++;
				EClass.pc.stamina.Mod(-1);
			}
		}
	}

	public static Point GetTarget(Point dest)
	{
		List<Point> list = new List<Point>();
		foreach (Point item in EClass._map.ListPointsInCircle(dest, 3f, mustBeWalkable: false))
		{
			if (CanClean(item) && item.IsInBounds)
			{
				list.Add(item);
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		list.Sort((Point a, Point b) => dest.Distance(a) - dest.Distance(b));
		return list.First();
	}
}
