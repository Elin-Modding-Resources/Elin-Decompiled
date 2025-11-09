using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class WidgetRoster : Widget
{
	public class Extra
	{
		[JsonProperty]
		public int width = 40;

		[JsonProperty]
		public int margin;

		[JsonProperty]
		public bool vertical;

		[JsonProperty]
		public bool pc;

		[JsonProperty]
		public bool portrait;

		[JsonProperty]
		public bool reverse;

		[JsonProperty]
		public bool onlyName;

		[JsonProperty]
		public bool showHP;
	}

	public static WidgetRoster Instance;

	public GridLayoutGroup layout;

	public ButtonRoster mold;

	public RawImage imageGrid;

	public Dictionary<Chara, ButtonRoster> buttons = new Dictionary<Chara, ButtonRoster>();

	public UIButton moldDropperLeft;

	public UIButton moldDropperRight;

	public bool showName;

	public bool dirty;

	public int maxWidth;

	public Extra extra => base.config.extra as Extra;

	public override bool AlwaysBottom => true;

	public override Type SetSiblingAfter => typeof(WidgetBottomBar);

	public override object CreateExtra()
	{
		return new Extra();
	}

	public override void OnActivate()
	{
		if (Mathf.Abs(extra.margin) > 100)
		{
			extra.margin = 0;
			extra.width = 40;
			extra.portrait = false;
			extra.onlyName = true;
		}
		Instance = this;
		mold = layout.CreateMold<ButtonRoster>();
		Build();
	}

	private void OnEnable()
	{
		InvokeRepeating("Refresh", 0.2f, 0.2f);
	}

	private void OnDisable()
	{
		CancelInvoke();
	}

	public void OnMoveZone()
	{
		Build();
	}

	public void Build()
	{
		buttons.Clear();
		layout.DestroyChildren();
		layout.constraint = (extra.vertical ? GridLayoutGroup.Constraint.FixedColumnCount : GridLayoutGroup.Constraint.FixedRowCount);
		layout.startCorner = (extra.reverse ? GridLayoutGroup.Corner.LowerRight : GridLayoutGroup.Corner.UpperLeft);
		layout.cellSize = new Vector2(extra.width * 4 / 5, extra.onlyName ? 32 : extra.width);
		layout.spacing = new Vector2(extra.margin, extra.margin);
		List<Chara> list = EMono.pc.party.members.ToList();
		if (!extra.pc)
		{
			list.Remove(EMono.pc);
		}
		list.Sort(delegate(Chara a, Chara b)
		{
			int @int = a.GetInt(19);
			int int2 = b.GetInt(19);
			return (@int == int2) ? (EMono.pc.party.members.IndexOf(a) - EMono.pc.party.members.IndexOf(b)) : (int2 * 1000 - @int * 1000);
		});
		foreach (Chara item in list)
		{
			Add(item);
		}
		if (buttons.Count == 0)
		{
			Add(EMono.pc);
		}
		layout.RebuildLayout();
		this.RebuildLayout();
		OnChangeResolution();
		ClampToScreen();
	}

	public static void SetDirty()
	{
		if ((bool)Instance)
		{
			Instance.dirty = true;
		}
	}

	public void Refresh()
	{
		if (dirty)
		{
			Build();
		}
		foreach (ButtonRoster value in buttons.Values)
		{
			value.Refresh();
		}
		dirty = false;
	}

	public void Add(Chara c)
	{
		ButtonRoster buttonRoster = Util.Instantiate(mold, layout);
		buttonRoster.SetChara(c);
		buttons.Add(c, buttonRoster);
	}

	public void Remove(Chara c)
	{
		UnityEngine.Object.DestroyImmediate(buttons[c].gameObject);
		buttons.Remove(c);
	}

	public void OnAddMember(Chara c)
	{
		if (!buttons.ContainsKey(c))
		{
			Add(c);
		}
	}

	public void OnRemoveMember(Chara c)
	{
		if (buttons.ContainsKey(c))
		{
			Remove(c);
		}
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		ButtonRoster b = InputModuleEX.GetComponentOf<ButtonRoster>();
		if ((bool)b)
		{
			m.AddSlider("priority_hint", (float a) => a.ToString() ?? "", b.chara.GetInt(19), delegate(float a)
			{
				b.chara.SetInt(19, (int)a);
				Build();
			}, 0f, 20f, isInt: true, hideOther: false);
		}
		UIContextMenu uIContextMenu = m.AddChild("setting");
		uIContextMenu.AddSlider("width", (float a) => a.ToString() ?? "", extra.width, delegate(float a)
		{
			extra.width = (int)a;
			Build();
		}, 30f, 160f, isInt: true);
		uIContextMenu.AddSlider("margin", (float a) => a.ToString() ?? "", extra.margin, delegate(float a)
		{
			extra.margin = (int)a;
			Build();
		}, -50f, 50f, isInt: true);
		uIContextMenu.AddToggle("roster_pc", extra.pc, delegate(bool a)
		{
			extra.pc = a;
			Build();
		});
		uIContextMenu.AddToggle("vertical", extra.vertical, delegate(bool a)
		{
			extra.vertical = a;
			Build();
		});
		uIContextMenu.AddToggle("roster_portrait", extra.portrait, delegate(bool a)
		{
			extra.portrait = a;
			Build();
		});
		uIContextMenu.AddToggle("roster_onlyname", extra.onlyName, delegate(bool a)
		{
			extra.onlyName = a;
			Build();
		});
		uIContextMenu.AddToggle("reverseOrder", extra.reverse, delegate(bool a)
		{
			extra.reverse = a;
			Build();
		});
		m.AddChild("style").AddSlider("toggleButtonBG", (float a) => a.ToString() ?? "", base.config.skin.button, delegate(float a)
		{
			base.config.skin.button = (int)a;
			ApplySkin();
		}, 0f, base.config.skin.Skin.buttons.Count - 1, isInt: true);
		SetBaseContextMenu(m);
	}
}
