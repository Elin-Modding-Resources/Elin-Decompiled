using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WidgetStatsBar : Widget
{
	public class Item
	{
		public UIItem component;

		public Element ele;

		public string id;

		public Sprite sprite;

		public Func<string> func;

		public Func<FontColor> funcCol;

		public Func<bool> funcShow;

		public FontColor lastColor;
	}

	public class Extra
	{
		public bool attributes;

		public bool money;

		public bool money2;

		public bool plat;

		public bool medal;

		public bool karma;

		public bool mood;

		public bool weight;

		public bool influence;

		public bool maxAlly;

		public bool maxMinion;

		public bool hearthLv;

		public bool tourism_value;

		public bool fame;

		public bool dv;

		public bool fertility;

		public bool elec;
	}

	public static WidgetStatsBar Instance;

	public LayoutGroup layout;

	public List<Item> items = new List<Item>();

	public Sprite iconMoney;

	public Sprite iconMoney2;

	public Sprite iconPlat;

	public Sprite iconMedal;

	public Sprite iconKarma;

	public Sprite iconWeight;

	public Sprite iconInfluence;

	public Sprite iconMaxAlly;

	public Sprite iconGodMood;

	public Sprite iconHearth;

	public Sprite iconTourismValue;

	public Sprite iconFame;

	public Sprite iconDvPv;

	public Sprite iconMaxMinion;

	public Sprite iconFertility;

	public Sprite iconElec;

	private UIItem mold;

	public Extra extra => base.config.extra as Extra;

	public Color colorDefault => base.config.skin.BG.colors.textDefault;

	public Color colorBad => base.config.skin.BG.colors.textBad;

	public Color colorVeryBad => base.config.skin.BG.colors.textBad;

	public override object CreateExtra()
	{
		return new Extra();
	}

	public override void OnActivate()
	{
		Instance = this;
		mold = layout.CreateMold<UIItem>();
		if (extra == null)
		{
			base.config.extra = CreateExtra();
		}
		Build();
		InvokeRepeating("Refresh", 0.2f, 0.2f);
	}

	public void Build()
	{
		items.Clear();
		layout.DestroyChildren();
		if (extra.attributes)
		{
			foreach (Element item in EMono.pc.elements.ListElements((Element e) => e.HasTag("primary")))
			{
				Add(item);
			}
			Add(EMono.pc.elements.GetElement(79));
		}
		if (extra.dv)
		{
			Add(null, "dvpv", iconDvPv, () => EMono.pc.DV + "/" + EMono.pc.PV);
		}
		if (extra.maxAlly)
		{
			Add(null, "maxAlly", iconMaxAlly, () => EMono.pc.party.members.Count - 1 + "/" + EMono.player.MaxAlly, () => (EMono.player.lastEmptyAlly >= 0) ? FontColor.Default : FontColor.Bad);
		}
		if (extra.maxMinion)
		{
			Add(null, "maxMinion", iconMaxMinion, () => EMono._zone.CountMinions(EMono.pc) + "/" + EMono.pc.MaxSummon, () => (EMono.pc.MaxSummon - EMono._zone.CountMinions(EMono.pc) >= 0) ? FontColor.Default : FontColor.Bad);
		}
		if (extra.money)
		{
			Add(null, "money", iconMoney, () => EMono.pc.GetCurrency().ToString("#,0") ?? "");
		}
		if (extra.money2)
		{
			Add(null, "money2", iconMoney2, () => EMono.pc.GetCurrency("money2").ToString("#,0") ?? "");
		}
		if (extra.plat)
		{
			Add(null, "plat", iconPlat, () => EMono.pc.GetCurrency("plat").ToString("#,0") ?? "");
		}
		if (extra.medal)
		{
			Add(null, "medal", iconMedal, () => EMono.pc.GetCurrency("medal").ToString("#,0") ?? "");
		}
		if (extra.karma)
		{
			Add(null, "karma", iconKarma, () => EMono.player.karma.ToString() ?? "");
		}
		if (extra.mood)
		{
			Add(null, "mood", iconGodMood, () => EMono.pc.faith.TextMood ?? "");
		}
		if (extra.fame)
		{
			Add(null, "fame", iconFame, () => EMono.player.fame.ToString() ?? "");
		}
		if (extra.influence)
		{
			Add(null, "influence", iconInfluence, () => EMono._zone.influence.ToString() ?? "", null, () => EMono._zone.influence != 0);
		}
		if (extra.tourism_value)
		{
			Add(null, "tourism_value", iconTourismValue, () => (EMono.Branch != null) ? EMono.Branch.tourism.ToFormat() : "", null, () => EMono._zone.IsPCFaction);
		}
		if (extra.hearthLv)
		{
			Add(null, "hearth_lv", iconHearth, () => (EMono.Branch != null) ? EMono.Branch.TextLv : "", () => (EMono.Branch != null && EMono.Branch.exp < EMono.Branch.GetNextExp()) ? FontColor.Default : FontColor.Good, () => EMono._zone.IsPCFaction);
		}
		if (extra.fertility)
		{
			Add(null, "fertility", iconFertility, delegate
			{
				object obj;
				if (EMono.Branch != null || EMono._zone is Zone_Tent)
				{
					obj = (EMono._zone.MaxSoil - EMono._zone.GetSoilCost()).ToString();
					if (obj == null)
					{
						return "";
					}
				}
				else
				{
					obj = "";
				}
				return (string)obj;
			}, () => ((EMono.Branch == null && !(EMono._zone is Zone_Tent)) || EMono._zone.MaxSoil - EMono._zone.GetSoilCost() >= 0) ? FontColor.Default : FontColor.Bad, () => EMono._zone.IsPCFaction || EMono._zone is Zone_Tent);
		}
		if (extra.elec)
		{
			Add(null, "elec", iconElec, () => (EMono.Branch != null || EMono._zone is Zone_Tent) ? (EMono._zone.GetElectricity(cost: true) + " / " + EMono._zone.GetElectricity()) : "", () => ((EMono.Branch == null && !(EMono._zone is Zone_Tent)) || EMono._zone.GetElectricity() - EMono._zone.GetElectricity(cost: true) >= 0) ? FontColor.Default : FontColor.Bad, () => EMono._zone.IsPCFaction || EMono._zone is Zone_Tent);
		}
		if (extra.weight)
		{
			Add(null, "weight", iconWeight, () => ((float)EMono.pc.ChildrenWeight / 1000f).ToString("F1") + " / " + ((float)EMono.pc.WeightLimit / 1000f).ToString("F1"), () => EMono.pc.burden.GetPhase() switch
			{
				1 => FontColor.Warning, 
				0 => FontColor.Default, 
				_ => FontColor.Bad, 
			});
		}
		Refresh();
	}

	public UIItem Add(Element ele, string id = "", Sprite sprite = null, Func<string> func = null, Func<FontColor> funcCol = null, Func<bool> funcShow = null)
	{
		UIItem uIItem = Util.Instantiate(mold, layout);
		Item item = new Item
		{
			ele = ele,
			component = uIItem,
			id = id,
			func = func,
			funcCol = funcCol,
			funcShow = funcShow
		};
		if ((bool)sprite)
		{
			uIItem.image1.sprite = sprite;
		}
		else
		{
			uIItem.image1.sprite = ele.GetIcon("_m");
		}
		uIItem.image1.SetNativeSize();
		uIItem.text1.skinRoot = base.skinRoot;
		Shadow component = uIItem.text1.GetComponent<Shadow>();
		if ((bool)component)
		{
			component.enabled = base.config.skin.BG.textShadow;
		}
		items.Add(item);
		return uIItem;
	}

	public void Refresh()
	{
		bool flag = false;
		foreach (Item item in items)
		{
			string text = item.component.text1.text;
			string text2 = "";
			FontColor fontColor = FontColor.Default;
			if (!item.id.IsEmpty())
			{
				text2 = item.func();
				if (item.funcCol != null)
				{
					fontColor = item.funcCol();
				}
			}
			else
			{
				Element ele = item.ele;
				text2 = ele.DisplayValue.ToString() ?? "";
				if (ele.DisplayValue < ele.ValueWithoutLink)
				{
					text2 = text2 + " (" + ele.ValueWithoutLink + ")";
				}
				int num = ((EMono.pc.tempElements != null) ? EMono.pc.tempElements.Value(ele.id) : 0);
				if (ele.DisplayValue < ele.ValueWithoutLink || num < 0)
				{
					fontColor = FontColor.Bad;
				}
				else if (num > 0)
				{
					fontColor = FontColor.Good;
				}
			}
			if (text2 != text || item.lastColor != fontColor)
			{
				item.component.text1.SetText(text2, fontColor);
				item.component.RebuildLayout();
				flag = true;
				item.lastColor = fontColor;
			}
			if (item.funcShow != null)
			{
				item.component.SetActive(item.funcShow());
			}
		}
		if (flag)
		{
			layout.RebuildLayout();
			this.RebuildLayout();
		}
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		UIContextMenu uIContextMenu = m.AddChild("setting");
		uIContextMenu.AddToggle("attributes", extra.attributes, delegate(bool a)
		{
			extra.attributes = a;
			Build();
		});
		uIContextMenu.AddToggle("dvpv", extra.dv, delegate(bool a)
		{
			extra.dv = a;
			Build();
		});
		uIContextMenu.AddToggle("maxAlly", extra.maxAlly, delegate(bool a)
		{
			extra.maxAlly = a;
			Build();
		});
		uIContextMenu.AddToggle("maxMinion", extra.maxMinion, delegate(bool a)
		{
			extra.maxMinion = a;
			Build();
		});
		uIContextMenu.AddToggle("money", extra.money, delegate(bool a)
		{
			extra.money = a;
			Build();
		});
		uIContextMenu.AddToggle("money2", extra.money2, delegate(bool a)
		{
			extra.money2 = a;
			Build();
		});
		uIContextMenu.AddToggle("plat", extra.plat, delegate(bool a)
		{
			extra.plat = a;
			Build();
		});
		uIContextMenu.AddToggle("medal", extra.medal, delegate(bool a)
		{
			extra.medal = a;
			Build();
		});
		uIContextMenu.AddToggle("karma", extra.karma, delegate(bool a)
		{
			extra.karma = a;
			Build();
		});
		uIContextMenu.AddToggle("fame", extra.fame, delegate(bool a)
		{
			extra.fame = a;
			Build();
		});
		uIContextMenu.AddToggle("influence", extra.influence, delegate(bool a)
		{
			extra.influence = a;
			Build();
		});
		if ((bool)EMono.debug)
		{
			uIContextMenu.AddToggle("godMood", extra.mood, delegate(bool a)
			{
				extra.mood = a;
				Build();
			});
		}
		uIContextMenu.AddToggle("tourism_value", extra.tourism_value, delegate(bool a)
		{
			extra.tourism_value = a;
			Build();
		});
		uIContextMenu.AddToggle("hearth_lv", extra.hearthLv, delegate(bool a)
		{
			extra.hearthLv = a;
			Build();
		});
		uIContextMenu.AddToggle("fertility", extra.fertility, delegate(bool a)
		{
			extra.fertility = a;
			Build();
		});
		uIContextMenu.AddToggle("elec", extra.elec, delegate(bool a)
		{
			extra.elec = a;
			Build();
		});
		uIContextMenu.AddToggle("invWeight", extra.weight, delegate(bool a)
		{
			extra.weight = a;
			Build();
		});
		SetBaseContextMenu(m);
	}

	public override void OnApplySkin()
	{
		base.OnApplySkin();
		Build();
	}
}
