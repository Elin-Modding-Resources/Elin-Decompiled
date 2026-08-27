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
			int range = owner.Tool?.Evalue(770) ?? 0;
			range = ((range <= 0) ? 1 : (2 + range / 10));
			dest = GetTarget(dest, range);
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
			owner.Say("clean", owner);
			owner.PlaySound("clean_floor");
			if (range > 1)
			{
				List<Point> list = EClass._map.ListPointsInSquare(dest, range - 1, mustBeWalkable: false);
				list.Sort((Point a, Point b) => a.Distance(dest) - b.Distance(dest));
				foreach (Point item in list)
				{
					if (owner != null && !owner.isDead)
					{
						Clean(item);
						continue;
					}
					break;
				}
			}
			else
			{
				Clean(dest);
			}
			yield return KeepRunning();
		}
		void Clean(Point p)
		{
			if (CanClean(p))
			{
				EClass._map.SetDecal(p.x, p.z);
				EClass._map.SetLiquid(p.x, p.z, 0, 0);
				p.PlayEffect("vanish");
				owner.ModExp(293, 30);
				if (owner.IsPC)
				{
					EClass.player.stats.clean++;
				}
				owner.stamina.Mod(-1);
			}
		}
	}

	public static Point GetTarget(Point dest, int ranage)
	{
		List<Point> list = new List<Point>();
		foreach (Point item in EClass._map.ListPointsInCircle(dest, 2 + ranage * 2, mustBeWalkable: false))
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
