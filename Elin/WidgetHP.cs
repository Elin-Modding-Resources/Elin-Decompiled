using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class WidgetHP : Widget
{
	public class Extra
	{
		[JsonProperty]
		public int layout;

		[JsonProperty]
		public int spacing;

		[JsonProperty]
		public int fontSize;

		[JsonProperty]
		public bool showGauge;

		[JsonProperty]
		public bool showMax;

		[JsonProperty]
		public bool stamina;
	}

	public Gauge gaugeHP;

	public Gauge gaugeMP;

	public Gauge gaugeStamina;

	public UIText textHP;

	public UIText textMP;

	public UIText textStamina;

	public UIText textBarrier;

	public GridLayoutGroup grid;

	public GameObject goBarrier;

	public Extra extra => base.config.extra as Extra;

	public override object CreateExtra()
	{
		return new Extra();
	}

	public override void OnActivate()
	{
		Rebuild();
		InvokeRepeating("Refresh", 0f, 0.2f);
	}

	public void Rebuild()
	{
		grid.constraintCount = extra.layout + 1;
		Refresh();
		this.RebuildLayout();
	}

	public void Refresh()
	{
		if (!EMono.game.isLoading)
		{
			if (EMono.pc.hp > EMono.pc.MaxHP)
			{
				EMono.pc.hp = EMono.pc.MaxHP;
			}
			if (EMono.pc.mana.value > EMono.pc.mana.max)
			{
				EMono.pc.mana.value = EMono.pc.mana.max;
			}
			if (EMono.pc.stamina.value > EMono.pc.stamina.max)
			{
				EMono.pc.stamina.value = EMono.pc.stamina.max;
			}
		}
		grid.spacing = new Vector2(extra.spacing, 0f);
		gaugeHP.hideBar = !extra.showGauge;
		gaugeMP.hideBar = !extra.showGauge;
		gaugeStamina.hideBar = !extra.showGauge;
		gaugeStamina.SetActive(extra.stamina);
		gaugeHP.textNow.SetSize(gaugeHP.textNow.orgSize + extra.fontSize);
		gaugeMP.textNow.SetSize(gaugeMP.textNow.orgSize + extra.fontSize);
		gaugeStamina.textNow.SetSize(gaugeStamina.textNow.orgSize + extra.fontSize);
		gaugeHP.UpdateValue(EMono.pc.hp, EMono.pc.MaxHP);
		gaugeMP.UpdateValue(EMono.pc.mana.value, EMono.pc.mana.max);
		gaugeStamina.UpdateValue(EMono.pc.stamina.value, EMono.pc.stamina.max);
		Color c = EMono.Colors.Dark.gradientHP.Evaluate((float)EMono.pc.hp / (float)EMono.pc.MaxHP);
		gaugeHP.textNow.text = "".TagColor(c, EMono.pc.hp.ToString() ?? "") + (extra.showMax ? (" / " + EMono.pc.MaxHP) : "");
		c = EMono.Colors.Dark.gradientMP.Evaluate((float)EMono.pc.mana.value / (float)EMono.pc.mana.max);
		gaugeMP.textNow.text = "".TagColor(c, EMono.pc.mana.value.ToString() ?? "") + (extra.showMax ? (" / " + EMono.pc.mana.max) : "");
		c = EMono.Colors.Dark.gradientSP.Evaluate((float)EMono.pc.stamina.value / (float)EMono.pc.stamina.max);
		gaugeStamina.textNow.text = "".TagColor(c, EMono.pc.stamina.value.ToString() ?? "") + (extra.showMax ? (" / " + EMono.pc.stamina.max) : "");
		goBarrier.SetActive(value: false);
		textBarrier.text = "10";
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		UIContextMenu uIContextMenu = m.AddChild("setting");
		uIContextMenu.AddSlider("layout", (float n) => n.ToString() ?? "", extra.layout, delegate(float a)
		{
			extra.layout = (int)a;
			Rebuild();
			ClampToScreen();
		}, 0f, 2f, isInt: true);
		uIContextMenu.AddSlider("spacing", (float n) => n.ToString() ?? "", extra.spacing, delegate(float a)
		{
			extra.spacing = (int)a;
		}, 0f, 100f, isInt: true);
		uIContextMenu.AddSlider("fontSize", (float n) => n.ToString() ?? "", extra.fontSize, delegate(float a)
		{
			extra.fontSize = (int)a;
		}, -2f, 5f, isInt: true);
		uIContextMenu.AddToggle("showGauge", extra.showGauge, delegate(bool a)
		{
			extra.showGauge = a;
			Refresh();
			this.RebuildLayout(recursive: true);
		});
		uIContextMenu.AddToggle("showMax", extra.showMax, delegate(bool a)
		{
			extra.showMax = a;
			Refresh();
			this.RebuildLayout(recursive: true);
		});
		uIContextMenu.AddToggle("stamina", extra.stamina, delegate(bool a)
		{
			extra.stamina = a;
			Refresh();
			this.RebuildLayout(recursive: true);
		});
		SetBaseContextMenu(m);
	}
}
