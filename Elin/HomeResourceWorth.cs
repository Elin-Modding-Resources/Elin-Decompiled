using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class HomeResourceWorth : HomeResourceRate
{
	[JsonProperty]
	public int bestRank;

	public override bool IsAvailable => false;

	public void UpdateRank()
	{
		int rank = EClass.game.spatials.ranks.GetRank(branch.owner);
		if (bestRank == 0)
		{
			bestRank = rank;
		}
		if (rank < bestRank)
		{
			bestRank = rank;
			EClass.Sound.Play((rank <= 50) ? "clap3" : ((rank <= 100) ? "clap2" : "clap1"));
			Msg.Say("homerank_up", EClass.game.spatials.ranks.GetRankText(branch.owner));
		}
	}

	public override void Refresh()
	{
		base.Refresh();
		UpdateRank();
	}

	public int GetPrice(Thing t, bool top = false)
	{
		int num = t.GetPrice(CurrencyType.Money, sell: false, PriceType.Tourism);
		if (t.noSell)
		{
			num /= 50;
		}
		if (top && branch.policies.IsActive(2821))
		{
			num = (int)((long)num * (long)(150 + (int)Mathf.Sqrt(branch.Evalue(2821)) * 5) / 100);
		}
		return num;
	}

	public override int GetDestValue()
	{
		List<Thing> list = ListHeirloom();
		int num = 0;
		foreach (Thing item in list)
		{
			num += GetPrice(item, list[0] == item);
		}
		return num;
	}

	public List<Thing> ListHeirloom()
	{
		List<Thing> list = new List<Thing>();
		List<Thing> list2 = new List<Thing>();
		HashSet<string> hashSet = new HashSet<string>();
		int[] array = new int[EClass._map.SizeXZ];
		long num = 0L;
		int num2 = branch.Evalue(2814);
		int num3 = branch.Evalue(2823);
		List<Thing> list3 = EClass._map.things.Where((Thing t) => t.IsInstalled && t.HasTag(CTAG.tourism)).ToList();
		foreach (Thing item in list3)
		{
			item.sortVal = GetPrice(item);
		}
		list3.Sort((Thing a, Thing b) => b.sortVal - a.sortVal);
		foreach (Thing item2 in list3)
		{
			bool flag = item2.trait is TraitFigure;
			if (flag)
			{
				if (array[item2.pos.index] != 0)
				{
					continue;
				}
				array[item2.pos.index]++;
			}
			string text = "";
			int num4 = 1;
			if (flag)
			{
				text = "figure_" + item2.c_idRefCard;
				num4 = 2;
			}
			else
			{
				text = item2.id + "_" + item2.idSkin;
			}
			if (!hashSet.Contains(text))
			{
				int num5 = item2.sortVal * num4;
				if (num3 > 0)
				{
					num5 = num5 * (110 + (int)Mathf.Sqrt(num3) * 4) / 100;
				}
				num += num5;
				hashSet.Add(text);
			}
			else if (num2 > 0)
			{
				int num6 = item2.sortVal * num4 / Mathf.Max(20, 30 - (int)Mathf.Sqrt(num2));
				if (num6 > 0)
				{
					num += num6;
				}
			}
		}
		foreach (Thing thing in EClass._map.things)
		{
			if (thing.IsInstalled && (thing.IsFurniture || thing.trait is TraitToolMusic))
			{
				list2.Add(thing);
			}
		}
		int num7 = branch.Evalue(3780) + branch.Evalue(3781) + branch.Evalue(3782) + branch.Evalue(3783) + branch.Evalue(3784);
		num = (100 + num) * (100 + num7 * 15) / 100;
		branch.tourism = (int)num;
		list2.Sort((Thing a, Thing b) => GetPrice(b) - GetPrice(a));
		for (int i = 0; i < branch.NumHeirloom && i < list2.Count; i++)
		{
			list.Add(list2[i]);
		}
		return list;
	}

	public override void WriteNote(UINote n)
	{
		n.Clear();
		n.AddHeader(base.Name);
		n.AddTopic("TopicLeft", "vCurrent".lang(), value.ToFormat() ?? "");
		n.AddTopic("TopicLeft", "tourism_value".lang(), branch.tourism.ToFormat() ?? "");
		n.Space();
		n.AddHeader("HeaderTopic", "heirloom_list".lang(branch.NumHeirloom.ToString() ?? ""));
		n.Space(1);
		List<Thing> list = ListHeirloom();
		for (int i = 0; i < list.Count; i++)
		{
			Thing thing = list[i];
			n.AddText(i + 1 + ": " + thing.Name + (EClass.debug.showExtra ? ((object)GetPrice(thing, i == 0)) : ""));
		}
		n.Build();
	}
}
