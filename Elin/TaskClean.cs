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
			for (int i = 0; i < ((!dest.cell.HasLiquid) ? 1 : 5); i++)
			{
				owner.LookAt(dest);
				owner.renderer.NextFrame();
				yield return KeepRunning();
			}
			if (!CanClean(dest) || owner.Dist(dest) > 1)
			{
				yield return Cancel();
			}
			EClass._map.SetDecal(dest.x, dest.z);
			EClass._map.SetLiquid(dest.x, dest.z, 0, 0);
			dest.PlayEffect("vanish");
			EClass.pc.Say("clean", owner);
			EClass.pc.PlaySound("clean_floor");
			EClass.pc.ModExp(293, 30);
			EClass.player.stats.clean++;
			EClass.pc.stamina.Mod(-1);
			yield return KeepRunning();
		}
	}

	public static Point GetTarget(Point dest)
	{
		List<Point> list = new List<Point>();
		foreach (Point item in EClass._map.ListPointsInCircle(dest, 3f, mustBeWalkable: false))
		{
			if (CanClean(item))
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
