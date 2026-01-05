using System.Collections.Generic;
using UnityEngine;

public class RankedZoneManager : EClass
{
	public int GetIncome(Zone z)
	{
		List<FactionBranch> children = EClass.pc.faction.GetChildren();
		children.Sort((FactionBranch a, FactionBranch b) => GetRank(a.owner) - GetRank(b.owner));
		int num = children.IndexOf(z.branch);
		return CalcIncome(z) * 100 / (100 + num * 40);
		int CalcIncome(Zone _z)
		{
			int rank = GetRank(_z);
			if (rank > 125)
			{
				return 0;
			}
			return (126 - rank) * 100;
		}
	}

	public string GetRankText(Zone z)
	{
		int rank = GetRank(z);
		string[] list = Lang.GetList("num_rank");
		int num = rank % 10;
		if (num >= list.Length)
		{
			num = 0;
		}
		return rank + list[num];
	}

	public int GetRank(Zone z)
	{
		foreach (RankedZone item in GetList())
		{
			if (item.z == z)
			{
				return item.rank;
			}
		}
		return 1;
	}

	public List<RankedZone> GetList()
	{
		List<RankedZone> list = new List<RankedZone>();
		foreach (Spatial z in EClass.game.spatials.map.Values)
		{
			if ((z == null || z.source.value <= 0 || z.lv != 0) && !z.IsPlayerFaction)
			{
				continue;
			}
			int v = 0;
			if (z.IsPlayerFaction)
			{
				v = (z as Zone).branch.Worth;
			}
			else
			{
				v = z.source.value;
				Rand.UseSeed(z.uid, delegate
				{
					v = z.source.value + EClass.rnd(z.source.value / 10);
				});
			}
			list.Add(new RankedZone
			{
				z = (z as Zone),
				value = v
			});
		}
		list.Sort((RankedZone a, RankedZone b) => b.Value - a.Value);
		int num = 0;
		int num2 = 0;
		foreach (RankedZone item in list)
		{
			int num3 = num2 - item.value;
			int num4 = 1000;
			num4 = ((item.value < 100000000) ? ((item.value < 10000000) ? ((item.value < 5000000) ? ((item.value < 1000000) ? ((item.value < 500000) ? ((item.value < 100000) ? ((item.value < 50000) ? ((item.value < 10000) ? 1000 : 2000) : 5000) : 10000) : 50000) : 500000) : 1000000) : 5000000) : 100000000);
			num = (item.rank = num + Mathf.Max(num3 / num4, 1));
			num2 = item.Value;
		}
		return list;
	}
}
