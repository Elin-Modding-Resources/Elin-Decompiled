using System.Collections.Generic;

public class VirtualDate : Date
{
	public static Date current;

	public bool IsRealTime;

	public HashSet<int> sunMap;

	public BranchMap branchMap;

	public static bool IsActive => current != null;

	public VirtualDate(int elapsedHours = 0)
	{
		for (int i = 0; i < raw.Length; i++)
		{
			raw[i] = EClass.world.date.raw[i];
		}
		base.min = 0;
		while (elapsedHours > 0)
		{
			base.hour--;
			if (base.hour < 0)
			{
				base.hour = 23;
				base.day--;
				if (base.day <= 0)
				{
					base.day = 30;
					base.month--;
					if (base.month <= 0)
					{
						base.month = 12;
						base.year--;
					}
				}
			}
			elapsedHours--;
		}
	}

	public BranchMap GetBranchMap()
	{
		if (branchMap == null)
		{
			branchMap = new BranchMap();
			branchMap.Refresh();
		}
		return branchMap;
	}

	public void BuildSunMap()
	{
		sunMap = new HashSet<int>();
		foreach (Card sun in EClass._map.props.installed.traits.suns)
		{
			foreach (Point item in sun.trait.ListPoints(null, onlyPassable: false))
			{
				sunMap.Add(item.index);
			}
		}
	}

	public void SimulateHour()
	{
		base.hour++;
		if (base.hour >= 24)
		{
			base.hour = 0;
			SimulateDay();
		}
		EClass._zone.OnSimulateHour(this);
	}

	public void SimulateDay()
	{
		base.day++;
		if (base.day > 30)
		{
			base.day = 1;
			SimulateMonth();
		}
		EClass._zone.OnSimulateDay(this);
	}

	public void SimulateMonth()
	{
		base.month++;
		if (base.month > 12)
		{
			base.month = 1;
			SimulateYear();
		}
		EClass._zone.OnSimulateMonth(this);
	}

	public void SimulateYear()
	{
		base.year++;
	}
}
