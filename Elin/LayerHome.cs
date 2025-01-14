using System.Collections.Generic;
using System.Linq;

public class LayerHome : ELayer
{
	public static LayerHome Instance;

	public UIList listArea;

	public UIList listTech;

	public UIList listTech2;

	public UIList listFeat;

	public UIList listFeat2;

	public UIList listFeat3;

	public FactionBranch branch;

	public UIHomeInfo info;

	public const int TabResearch = 1;

	public const int TabPolicy = 2;

	public const int TabPopulation = 3;

	public const int TabFeat = 4;

	public override bool HeaderIsListOf(int id)
	{
		return id != 0;
	}

	public override void OnInit()
	{
		branch = ELayer.Branch;
		Instance = this;
	}

	public override void OnSwitchContent(Window window)
	{
		if (window.setting.tabs.Count == 0)
		{
			return;
		}
		WindowMenu menuLeft = window.menuLeft;
		menuLeft.Clear();
		if (ELayer.player.shippingResults.Count > 0)
		{
			menuLeft.AddButtonSimple(() => "shipping_result".lang(), delegate
			{
				ELayer.ui.AddLayer<LayerShippingResult>().Show();
			});
		}
		switch (window.CurrentTab.idLang)
		{
		case "top":
			info.Refresh();
			break;
		case "skills":
			RefreshFeat();
			break;
		case "chronicle":
			RefreshChronicle();
			break;
		case "population":
			break;
		}
	}

	public void RefreshChronicle()
	{
	}

	public void RefreshTech(bool listComplete = false)
	{
		UIList list = (listComplete ? listTech2 : listTech);
		list.sortMode = ELayer.player.pref.sortResearch;
		list.callbacks = new UIList.Callback<ResearchPlan, ItemResearch>
		{
			onInstantiate = delegate
			{
			},
			onList = delegate
			{
				foreach (ResearchPlan item in listComplete ? branch.researches.finished : branch.researches.plans)
				{
					list.Add(item);
				}
			},
			onSort = (ResearchPlan a, UIList.SortMode m) => a.GetSortVal(m)
		};
		list.List();
		RefreshFocus();
	}

	public void RefreshFocus()
	{
		ItemResearch[] componentsInChildren = listTech.GetComponentsInChildren<ItemResearch>();
		foreach (ItemResearch itemResearch in componentsInChildren)
		{
			itemResearch.goFocus.SetActive(itemResearch.plan == branch.researches.focused);
		}
	}

	public void RefreshFeat()
	{
		listFeat.callbacks = new UIList.Callback<Element, ButtonElement>
		{
			onClick = delegate(Element a, ButtonElement b)
			{
				int p = ELayer.Branch.GetTechUpgradeCost(a);
				int currency = ELayer.pc.GetCurrency("money2");
				if (p == 0 || p > currency || a.ValueWithoutLink == 0 || a.source.cost[0] == 0)
				{
					SE.Beep();
				}
				else
				{
					Dialog.YesNo("dialogUpgradeTech".lang(a.Name), delegate
					{
						SE.Pay();
						ELayer.pc.ModCurrency(-p, "money2");
						ELayer.Branch.elements.ModBase(a.id, 1);
						listFeat.List();
						ELayer.Branch.resources.SetDirty();
						ELayer.core.game.player.hotbars.ResetHotbar(2);
						if ((bool)WidgetHotbar.HotBarMainMenu)
						{
							WidgetHotbar.HotBarMainMenu.RebuildPage();
						}
						if ((bool)WidgetMenuPanel.Instance)
						{
							WidgetMenuPanel.Instance.OnChangeActionMode();
						}
					});
				}
			},
			onInstantiate = delegate(Element a, ButtonElement b)
			{
				b.SetElement(a, ELayer._zone.elements, ButtonElement.Mode.Tech);
			},
			onList = delegate
			{
				foreach (Element item in branch.elements.dict.Values.Where((Element b) => (b.Value > 0 || b.vBase > 0) && b.source.category != "policy" && b.source.category != "landfeat" && !b.HasTag("hidden")))
				{
					listFeat.Add(item);
				}
			},
			onSort = (Element a, UIList.SortMode m) => a.GetSortVal(m),
			onRefresh = null
		};
		listFeat.List();
		listFeat2.callbacks = new UIList.Callback<Element, ButtonElement>
		{
			onClick = delegate
			{
			},
			onInstantiate = delegate(Element a, ButtonElement b)
			{
				b.SetElement(a, ELayer._zone.elements, ButtonElement.Mode.Policy);
			},
			onList = delegate
			{
				foreach (Element item2 in branch.elements.dict.Values.Where((Element b) => b.source.category == "policy"))
				{
					listFeat2.Add(item2);
				}
			},
			onSort = (Element a, UIList.SortMode m) => a.GetSortVal(m),
			onRefresh = null
		};
		listFeat2.List();
		listFeat3.callbacks = new UIList.Callback<Element, ButtonElement>
		{
			onClick = delegate
			{
			},
			onInstantiate = delegate(Element a, ButtonElement b)
			{
				b.SetElement(a, ELayer._zone.elements, ButtonElement.Mode.LandFeat);
			},
			onList = delegate
			{
				IEnumerable<Element> enumerable = branch.elements.dict.Values;
				if (branch.HasNetwork)
				{
					enumerable = enumerable.Concat(ELayer.pc.faction.elements.dict.Values);
				}
				foreach (Element item3 in enumerable.Where((Element b) => (b.Value > 0 || b.vBase > 0) && b.source.category == "landfeat" && !b.HasTag("hidden")))
				{
					listFeat3.Add(item3);
				}
			},
			onSort = (Element a, UIList.SortMode m) => a.GetSortVal(m),
			onRefresh = null
		};
		listFeat3.List();
	}

	public void RefreshAreas()
	{
		listArea.callbacks = new UIList.Callback<Area, ItemGeneral>
		{
			onClick = delegate
			{
			},
			onInstantiate = delegate(Area a, ItemGeneral b)
			{
				_ = b.button1;
				b.SetMainText(a.Name);
				b.AddSubButton(ELayer.core.refs.icons.go, delegate
				{
					ELayer.screen.Focus(a.points[0]);
				}, "tFocus");
				b.Build();
			},
			onList = delegate
			{
				foreach (Area item in ELayer._map.rooms.listArea)
				{
					listArea.Add(item);
				}
			},
			onSort = (Area a, UIList.SortMode m) => a.GetSortVal(m),
			onRefresh = null
		};
		listArea.List();
	}

	public void RefreshSpots()
	{
		listArea.Clear();
		listArea.callbacks = new UIList.Callback<Trait, ItemGeneral>
		{
			onClick = delegate
			{
			},
			onInstantiate = delegate(Trait a, ItemGeneral b)
			{
				_ = b.button1;
				b.SetMainText(a.Name);
				b.AddSubButton(ELayer.core.refs.icons.go, delegate
				{
					ELayer.screen.Focus(a.GetPoint());
				}, "tFocus");
				b.Build();
			},
			onList = delegate
			{
				foreach (Card item in ELayer._map.Installed.all)
				{
					if (item.trait.IsSpot)
					{
						listArea.Add(item.trait);
					}
				}
			},
			onSort = (Trait a, UIList.SortMode m) => a.GetSortVal(m),
			onRefresh = null
		};
		listArea.List();
	}
}
