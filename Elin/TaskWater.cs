using System.Collections.Generic;
using UnityEngine;

public class TaskWater : Task
{
	public Point dest;

	public TraitToolWaterCan waterCan => (owner ?? Act.CC)?.held?.trait as TraitToolWaterCan;

	public override string GetText(string str = "")
	{
		if (dest == null || !dest.cell.HasFire)
		{
			return base.GetText(str);
		}
		return "TaskWaterFire".lang();
	}

	public override bool CanPerform()
	{
		if (ShouldWater(Act.TP))
		{
			return IsWaterCanValid(msg: false);
		}
		return false;
	}

	public override bool CanManualCancel()
	{
		return true;
	}

	public override IEnumerable<Status> Run()
	{
		dest = new Point(dest);
		List<Point> list = ListPoints();
		while (list.Count != 0)
		{
			Debug.Log(list.Count);
			list.Sort((Point a, Point b) => a.Distance(dest) - b.Distance(dest));
			Point p = list[0];
			dest.Set(p);
			list.RemoveAt(0);
			if (!ShouldWater(p))
			{
				continue;
			}
			if (!IsWaterCanValid())
			{
				yield return Cancel();
			}
			bool fail = false;
			Status status = DoGoto(p, 1, ignoreConnection: false, delegate
			{
				fail = true;
				return Status.Running;
			});
			if (fail || status == Status.Fail)
			{
				continue;
			}
			yield return Status.Running;
			if (!IsWaterCanValid())
			{
				yield return Cancel();
			}
			if (!ShouldWater(p))
			{
				continue;
			}
			if (owner.Dist(dest) > 1)
			{
				yield return Cancel();
			}
			int num = waterCan.owner.Evalue(770);
			num = ((num <= 0) ? 1 : Mathf.Min(waterCan.owner.c_charges, 2 + num / 10));
			if (num > 1)
			{
				List<Point> list2 = EClass._map.ListPointsInSquare(p, num - 1, mustBeWalkable: false, los: false);
				list2.Sort((Point a, Point b) => a.Distance(p) - b.Distance(p));
				foreach (Point item in list2)
				{
					Water(item);
				}
			}
			else
			{
				Water(p);
			}
			owner.PlaySound("water_farm");
			owner.Say("water_farm", owner, p.cell.GetFloorName());
			waterCan.owner.ModCharge(-num);
			if (!IsWaterCanValid())
			{
				yield return Cancel();
			}
			yield return KeepRunning();
		}
		void Water(Point point)
		{
			if (ShouldWater(point))
			{
				owner.ModExp(286, 15);
			}
			point.cell.isWatered = true;
			if (!point.cell.blocked && EClass.rnd(5) == 0)
			{
				EClass._map.SetLiquid(point.x, point.z, 1);
			}
			if (point.cell.HasFire)
			{
				point.ModFire(-50, extinguish: true);
			}
		}
	}

	public static bool ShouldWater(Point p)
	{
		Cell cell = p.cell;
		if (cell.HasFire)
		{
			return true;
		}
		if (cell.isWatered || cell.IsTopWater || cell.IsSnowTile || !p.IsFarmField)
		{
			return false;
		}
		if (p.cell.detail != null)
		{
			foreach (Thing thing in p.Things)
			{
				if (thing.trait is TraitSeed)
				{
					return true;
				}
			}
		}
		return p.growth != null;
	}

	public bool IsWaterCanValid(bool msg = true)
	{
		bool num = waterCan != null && waterCan.owner.c_charges > 0;
		if (!num && msg)
		{
			Msg.Say("water_deplete");
		}
		return num;
	}

	public List<Point> ListPoints()
	{
		List<Point> list = new List<Point>();
		EClass._map.bounds.ForeachPoint(delegate(Point p)
		{
			if (ShouldWater(p))
			{
				list.Add(p.Copy());
			}
		});
		return list;
	}
}
