using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WidgetDate : Widget
{
	public class Extra
	{
		public bool date;

		public bool realDate;

		public bool period;

		public bool room;

		public bool clock;

		public bool loc;

		public bool weather;
	}

	public static WidgetDate Instance;

	public Text textTime;

	public Text textLevel;

	public RectTransform rectClock;

	public Image imageHour;

	public Color colorFestival;

	public Extra extra => base.config.extra as Extra;

	public override object CreateExtra()
	{
		return new Extra();
	}

	public override void OnActivate()
	{
		Instance = this;
		_Refresh();
		InvokeRepeating("_Refresh", 0.1f, 0.1f);
	}

	public void _Refresh()
	{
		if (EMono.game.activeZone == null)
		{
			return;
		}
		string text = "";
		GameDate date = EMono.world.date;
		text += (extra.date ? EMono.world.date.GetText(Date.TextFormat.Widget) : "");
		if (extra.period)
		{
			text = text + " (" + EMono.world.date.periodOfDay.ToString().lang() + ") ";
		}
		text += (extra.realDate ? (" (" + DateTime.Now.ToString("H:mm") + ") ") : "");
		if (extra.loc)
		{
			text = text + " " + EMono._zone.NameWithLevel;
			if (EMono._zone is Zone_Dungeon)
			{
				text += EMono._zone.TextDangerLv;
			}
			if (EMono._zone.isClaimable && !EMono._zone.IsPCFaction)
			{
				text += "(";
				List<Element> list = EMono._zone.ListLandFeats();
				foreach (Element item in list)
				{
					if (item != list[0])
					{
						text += ", ";
					}
					text += item.Name;
				}
				text += ")";
			}
		}
		if (EMono._zone.idCurrentSubset != null)
		{
			text += ("(" + (EMono._zone.idExport + "_" + EMono._zone.idCurrentSubset).lang() + ")").TagColor(colorFestival);
		}
		if (EMono._zone.isPeace)
		{
			text += "zone_peace".lang().TagColor(colorFestival);
		}
		if (EMono.debug.showExtra)
		{
			text = text + " <size=12>(" + EMono._zone.biome.name + "/Lv:" + EMono._zone.lv + "/Danger:" + EMono._zone.DangerLv + "/_danger:" + EMono._zone._dangerLv + "/elec:" + EMono._zone.electricity + ")</size>";
			if (EMono._zone.IsInstance)
			{
				text += "Instance";
			}
		}
		if (extra.weather)
		{
			text = text + " - " + EMono.world.weather.GetName();
		}
		if (extra.room && EMono.pc.IsInActiveZone && EMono._zone.IsPCFaction)
		{
			Room room = EMono.pc.Cell.room;
			if (room != null)
			{
				text = text + " (" + room.Name + ")";
			}
			if (EMono.pc.pos.area != null)
			{
				text = text + " (" + EMono.pc.pos.area.Name + ")";
			}
		}
		text += EMono._zone.TextWidgetDate;
		foreach (ZoneEvent item2 in EMono._zone.events.list)
		{
			text += item2.TextWidgetDate;
		}
		if (EMono.Branch != null && EMono.Branch.luckyMonth)
		{
			text = text + " " + "lucky_month".lang().TagColor(Msg.colors.colors["save"]);
		}
		if (EMono.debug.enable)
		{
			text += " *DEBUG MODE*";
		}
		textTime.text = text;
		rectClock.SetActive(extra.clock);
		if (extra.clock)
		{
			imageHour.transform.localEulerAngles = new Vector3(0f, 0f, -date.hour * 30 + 90);
		}
		_ = base.transform.position.x / (float)Screen.width;
		bool flag = base.config.pivot == RectPosition.BottomRIGHT || base.config.pivot == RectPosition.TopRIGHT || base.config.pivot == RectPosition.Right;
		this.Rect().SetPivot(flag ? 1 : 0, 0f);
		rectClock.SetAnchor(flag ? 1 : 0, 1f, flag ? 1 : 0, 1f);
		rectClock.anchoredPosition = new Vector2(flag ? (-60) : 60, -75f);
		textTime.RebuildLayout();
	}

	public override void OnChangePivot()
	{
		_Refresh();
	}

	public static void Refresh()
	{
		if ((bool)Instance)
		{
			Instance._Refresh();
		}
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		UIContextMenu uIContextMenu = m.AddChild("setting");
		uIContextMenu.AddToggle("showPeriodOfDay", extra.period, delegate(bool a)
		{
			extra.period = a;
			_Refresh();
		});
		uIContextMenu.AddToggle("showRealDate", extra.realDate, delegate(bool a)
		{
			extra.realDate = a;
			_Refresh();
		});
		uIContextMenu.AddToggle("showRoom", extra.room, delegate(bool a)
		{
			extra.room = a;
			_Refresh();
		});
		uIContextMenu.AddToggle("showClock", extra.clock, delegate(bool a)
		{
			extra.clock = a;
			_Refresh();
		});
		uIContextMenu.AddToggle("showLoc", extra.loc, delegate(bool a)
		{
			extra.loc = a;
			_Refresh();
		});
		uIContextMenu.AddToggle("showWeather", extra.weather, delegate(bool a)
		{
			extra.weather = a;
			_Refresh();
		});
		SetBaseContextMenu(m);
	}
}
