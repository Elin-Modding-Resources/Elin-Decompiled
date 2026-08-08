using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICurrency : EMono
{
	[Serializable]
	public class Options
	{
		public bool money;

		public bool plat;

		public bool ecopo;

		public bool knowledge;

		public bool influence;

		public bool medal;

		public bool money2;

		public bool resources;

		public bool branchMoney;

		public bool branchFood;

		public bool branchMaterial;

		public bool branchKnowledge;

		public bool admin;

		public bool casino;

		public bool weight;

		public bool ticketSky;
	}

	public class Item
	{
		public Func<string> func;

		public UIText text;
	}

	public List<Item> items = new List<Item>();

	public bool autoBuild;

	public bool disable;

	public Options options;

	public Sprite[] icons;

	public LayoutGroup layout;

	public Card target;

	private UIButton mold;

	private void Awake()
	{
		if (autoBuild)
		{
			Build();
		}
		InvokeRepeating("Refresh", 0.1f, 0.1f);
	}

	private void OnEnable()
	{
		Refresh();
	}

	public void Build(Options _options)
	{
		options = _options;
		Build();
	}

	public void Build()
	{
		items.Clear();
		mold = layout.CreateMold<UIButton>();
		if (options.plat)
		{
			Add(icons[1], "plat", () => EMono.pc.GetCurrency("plat").ToString("#,0") ?? "");
		}
		if (options.money)
		{
			Add(icons[0], "money", () => EMono.pc.GetCurrency().ToString("#,0") ?? "");
		}
		if (options.money2)
		{
			Add(icons[5], "money2", () => EMono.pc.GetCurrency("money2").ToString("#,0") ?? "");
		}
		if (options.medal)
		{
			Add(icons[4], "medal", () => EMono.pc.GetCurrency("medal").ToString("#,0") ?? "");
		}
		if (options.ecopo)
		{
			Add(EMono.sources.cards.map["ecopo"].GetSprite(), "ecopo", () => EMono.pc.GetCurrency("ecopo").ToString("#,0") ?? "");
		}
		if (options.ticketSky)
		{
			Add(EMono.sources.cards.map["ticket_sky"].GetSprite(), "ticket_sky", () => EMono.pc.GetCurrency("ticket_sky").ToString("#,0") ?? "");
		}
		if (options.influence)
		{
			Add(icons[3], "influence", () => EMono._zone.influence.ToString() ?? "");
		}
		if (options.casino)
		{
			Add(icons[10], "casino_coin", () => EMono.pc.GetCurrency("casino_coin").ToString("#,0") ?? "");
		}
		if (options.weight)
		{
			Add(icons[11], "weightInv", () => Lang._weight(target.ChildrenWeight, showUnit: false) + " / " + Lang._weight(target.WeightLimit));
		}
		if (EMono.BranchOrHomeBranch != null)
		{
			HomeResourceManager resources = EMono.BranchOrHomeBranch.resources;
			if (options.branchMoney)
			{
				Add(icons[5], resources.money.Name, () => resources.money.value.ToString("#,0") ?? "");
			}
			if (options.branchFood)
			{
				Add(icons[6], resources.food.Name, () => resources.food.value.ToString("#,0") ?? "");
			}
			if (options.branchKnowledge)
			{
				Add(icons[7], resources.knowledge.Name, () => resources.knowledge.value.ToString("#,0") ?? "");
			}
			if (options.admin)
			{
				Add(icons[9], "ap", () => EMono.Branch.policies.CurrentAP() + "/" + EMono.Branch.MaxAP);
			}
			if (options.resources)
			{
				foreach (BaseHomeResource r in EMono.BranchOrHomeBranch.resources.list)
				{
					if (r.IsAvailable)
					{
						Add(r.Sprite, r.Name, () => r.value.ToString("#,0") ?? "");
					}
				}
			}
		}
		layout.RebuildLayout();
	}

	public void Add(Sprite icon, string lang, Func<string> func)
	{
		UIButton uIButton = Util.Instantiate(mold, layout);
		uIButton.icon.sprite = icon;
		uIButton.icon.SetNativeSize();
		uIButton.tooltip.lang = lang.lang().ToTitleCase(wholeText: true);
		items.Add(new Item
		{
			func = func,
			text = uIButton.mainText
		});
		uIButton.mainText.SetText(func());
		uIButton.mainText.RebuildLayout();
		uIButton.RebuildLayout();
	}

	public void Refresh()
	{
		foreach (Item item in items)
		{
			string text = item.func();
			if (item.text.text != (text ?? ""))
			{
				item.text.SetText(text);
				item.text.RebuildLayout();
				item.text.transform.parent.RebuildLayout();
			}
		}
	}
}
