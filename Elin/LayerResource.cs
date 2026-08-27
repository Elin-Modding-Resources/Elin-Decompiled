using System.Collections.Generic;

public class LayerResource : ELayer
{
	public UIList list;

	public UIDynamicList list2;

	public UICardInfo info;

	public UINote noteOverview;

	public static string selectCat;

	public override void OnInit()
	{
		list2.sortMode = ELayer.player.pref.sortResources;
		RefreshCategory();
		if (!selectCat.IsEmpty())
		{
			list.Select((SourceCategory.Row a) => a.id == selectCat, invoke: true);
		}
		else
		{
			list.Select(0, invoke: true);
		}
		selectCat = null;
		WindowMenu menuRight = windows[0].menuRight;
		menuRight.AddButton2Line("sort", () => list2.sortMode.ToString(), delegate
		{
			list2.NextSort();
		});
		menuRight.AddButton2Line("stockCapacity", () => ELayer._map.Stocked.weight + "/" + ELayer._map.Stocked.maxWeight);
	}

	public override void OnKill()
	{
		ELayer.player.pref.sortResources = list2.sortMode;
	}

	public void RefreshCategory()
	{
		HashSet<string> track = ELayer.player.trackedCategories;
		list.Clear();
		list.callbacks = new UIList.Callback<SourceCategory.Row, ButtonCategory>
		{
			onClick = delegate(SourceCategory.Row a, ButtonCategory b)
			{
				ShowThings(a);
			},
			onInstantiate = delegate(SourceCategory.Row cat, ButtonCategory b)
			{
				b.toggle.SetActive(enable: true);
				b.toggle.isOn = track.Contains(cat.id);
				b.toggle.onValueChanged.AddListener(delegate(bool isOn)
				{
					SE.ClickGeneral();
					if (isOn)
					{
						track.Add(cat.id);
					}
					else
					{
						track.Remove(cat.id);
					}
					UIResourceTrack.Refresh();
				});
				int sum = ELayer._map.Stocked.categoryMap[cat.id].sum;
				b.mainText.text = cat.GetText().ToTitleCase() + " (" + sum + ")";
				b.SetFold(cat.children.Count > 0, folded: true, delegate(UIList l)
				{
					foreach (SourceCategory.Row row in ELayer.sources.categories.rows)
					{
						if (row.parent == cat)
						{
							l.Add(row);
						}
					}
				});
			},
			onRefresh = null
		};
		foreach (SourceCategory.Row row2 in ELayer.sources.categories.rows)
		{
			if (!(row2.id == "new") && !(row2.id == "stash") && row2.parent == null)
			{
				list.Add(row2);
			}
		}
		list.Refresh();
		UIResourceTrack.Refresh();
	}

	public void ShowThings(SourceCategory.Row cat)
	{
		HashSet<string> track = ELayer.player.trackedCards;
		list2.callbacks = new UIList.Callback<Thing, ButtonGrid>
		{
			onRedraw = delegate(Thing a, ButtonGrid b, int i)
			{
				b.SetCard(a);
				bool check = track.Contains(a.source.id);
				b.SetCheck(check);
			},
			onClick = delegate(Thing a, ButtonGrid b)
			{
				if (!track.Contains(a.source.id))
				{
					track.Add(a.source.id);
				}
				else
				{
					track.Remove(a.source.id);
				}
				UIResourceTrack.Refresh();
				list2.dsv.refresh();
				SE.Tab();
			},
			onList = delegate(UIList.SortMode m)
			{
				List<Thing> list = new List<Thing>();
				list = ELayer._map.Stocked.ListThingsInCategory(cat);
				for (int i = 0; i < list.Count; i++)
				{
					list[i].SetSortVal(m);
				}
				list.Sort((Thing a, Thing b) => a.sortVal - b.sortVal);
				int num = 0;
				int num2 = 0;
				foreach (Thing item in list)
				{
					list2.Add(item);
					num += item.Num;
					num2 += item.Num;
				}
				windows[0].textStats.SetText("statsTotal".lang(list.Count.ToString() ?? "", num.ToString() ?? "", num2.ToString() ?? ""));
			}
		};
		list2.List();
	}
}
