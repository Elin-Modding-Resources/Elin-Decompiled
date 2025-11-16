using System;
using System.Collections.Generic;
using System.Linq;
using SFB;
using UnityEngine;
using UnityEngine.UI;

public class WidgetManager : EMono
{
	public class SaveData
	{
		public Dictionary<string, Widget.Config> dict;
	}

	public Dictionary<string, Widget.Meta> metaMap = new Dictionary<string, Widget.Meta>();

	public string currentPath;

	[NonSerialized]
	public List<Widget> list = new List<Widget>();

	private bool first;

	public List<Widget.Meta> metas => EMono.setting.ui.widgetMetas;

	public Dictionary<string, Widget.Config> configs => EMono.player.widgets.dict;

	public void OnActivateZone()
	{
		if (!first)
		{
			return;
		}
		first = false;
		foreach (Widget.Config value in configs.Values)
		{
			if (value.state == Widget.State.Active)
			{
				ActivateWidget(value);
			}
		}
		bool flag = EMono.ui.layerFloat.GetLayer<LayerInventory>();
		if (GetWidget("Equip") == null)
		{
			if (flag)
			{
				Activate("Equip");
			}
		}
		else if (!flag)
		{
			DeactivateWidget("Equip");
		}
	}

	public void OnGameInstantiated()
	{
		if (Application.isEditor && EMono.debug.resetPlayerConfig && !EMono.player.isEditor)
		{
			EMono.player.useSubWidgetTheme = false;
			EMono.ui.widgets.Load(isSubTheme: false);
		}
		first = true;
		if (EMono.player.useSubWidgetTheme)
		{
			if (EMono.player.subWidgets == null)
			{
				Load(isSubTheme: true);
			}
		}
		else if (EMono.player.mainWidgets == null)
		{
			Load(isSubTheme: false);
			if (Screen.width <= 1300 && !EMono.player.useSubWidgetTheme)
			{
				foreach (KeyValuePair<string, Widget.Config> item in EMono.player.mainWidgets.dict)
				{
					if (item.Key == "StatsBar")
					{
						item.Value.state = Widget.State.Inactive;
					}
				}
			}
		}
		if (metaMap.Count == 0)
		{
			foreach (Widget.Meta meta in metas)
			{
				metaMap.Add(meta.id, meta);
			}
		}
		foreach (Widget.Meta meta2 in metas)
		{
			if (!configs.ContainsKey(meta2.id))
			{
				Widget.Config config = new Widget.Config
				{
					meta = meta2,
					valid = true,
					id = meta2.id,
					state = ((!meta2.enabled) ? Widget.State.Inactive : Widget.State.Active),
					locked = meta2.locked
				};
				config.skin.SetID(0);
				configs.Add(meta2.id, config);
			}
			else
			{
				Widget.Config config2 = configs[meta2.id];
				config2.valid = true;
				config2.meta = meta2;
			}
		}
		foreach (Widget.Config item2 in configs.Values.ToList())
		{
			if (!item2.valid)
			{
				configs.Remove(item2.id);
			}
		}
	}

	public void OnKillGame()
	{
		KillWidgets();
	}

	public void OnChangeActionMode()
	{
		foreach (Widget item in list)
		{
			item.OnChangeActionMode();
		}
	}

	public void UpdateConfigs()
	{
		foreach (Widget item in list)
		{
			item.UpdateConfig();
		}
	}

	public void Activate(string id)
	{
		if (!GetWidget(id))
		{
			ActivateWidget(configs[id]);
		}
	}

	public Widget Toggle(string id)
	{
		Widget widget = GetWidget(id);
		if ((bool)widget)
		{
			DeactivateWidget(widget);
			return null;
		}
		return ActivateWidget(configs[id]);
	}

	public Widget Toggle(Widget.Config c)
	{
		Widget widget = GetWidget(c.id);
		if ((bool)widget)
		{
			DeactivateWidget(widget);
			return null;
		}
		return ActivateWidget(c);
	}

	public void ToggleLock(Widget.Config c)
	{
		c.locked = !c.locked;
		RefreshWidget(c);
	}

	public Widget ActivateWidget(Widget.Config c)
	{
		return ActivateWidget(c.id);
	}

	public Widget ActivateWidget(string id)
	{
		string text = "Widget" + id;
		Widget widget = Util.Instantiate<Widget>("UI/Widget/" + text, this) ?? Util.Instantiate<Widget>("UI/Widget/" + text + "/" + text, this);
		if (!widget)
		{
			Debug.LogError("Widget:" + id + " not found.");
			return null;
		}
		list.Add(widget);
		widget.gameObject.name = text;
		widget.Activate();
		RefreshWidget(widget);
		if ((bool)LayerWidget.Instance)
		{
			widget.OnManagerActivate();
		}
		return widget;
	}

	public void RefreshWidget(Widget.Config c)
	{
		RefreshWidget(GetWidget(c.id));
	}

	public void RefreshWidget(Widget w)
	{
		if ((bool)w)
		{
			w.dragPanel.GetComponent<Graphic>().raycastTarget = !w.config.locked;
		}
	}

	public Widget GetWidget(string id)
	{
		foreach (Widget item in list)
		{
			if (item.gameObject.name == "Widget" + id)
			{
				return item;
			}
		}
		return null;
	}

	public void DeactivateWidget(string id)
	{
		DeactivateWidget(GetWidget(id));
	}

	public void DeactivateWidget(Widget w)
	{
		if (!(w == null))
		{
			list.Remove(w);
			w.Deactivate();
			if ((bool)LayerWidget.Instance)
			{
				LayerWidget.Instance.Refresh();
			}
		}
	}

	public void KillWidgets()
	{
		this.DestroyChildren(destroyInactive: true);
		list.Clear();
		first = true;
	}

	public void Show()
	{
		foreach (Widget item in list)
		{
			if (item.IsInRightMode())
			{
				item.gameObject.SetActive(value: true);
			}
		}
	}

	public void Hide()
	{
		foreach (Widget item in list)
		{
			if (item is WidgetCurrentTool || item is WidgetQuestTracker || item is WidgetTracker || item is WidgetMemo || item is WidgetEquip)
			{
				item.gameObject.SetActive(value: false);
			}
		}
	}

	public void Reset(bool toggleTheme)
	{
		MsgBox msgBox = null;
		if ((bool)WidgetMainText.Instance)
		{
			if (WidgetMainText.Instance.box.isShowingLog)
			{
				WidgetMainText.Instance._ToggleLog();
			}
			msgBox = (WidgetMainText.boxBk = WidgetMainText.Instance.box);
			msgBox.transform.SetParent(base.transform.parent, worldPositionStays: false);
		}
		KillWidgets();
		if (toggleTheme)
		{
			EMono.player.useSubWidgetTheme = !EMono.player.useSubWidgetTheme;
		}
		OnGameInstantiated();
		OnActivateZone();
		OnChangeActionMode();
		if (!WidgetMainText.Instance && (bool)msgBox)
		{
			UnityEngine.Object.Destroy(msgBox.gameObject);
		}
	}

	public void DialogSave(Action onSave = null)
	{
		EMono.core.WaitForEndOfFrame(delegate
		{
			string text = StandaloneFileBrowser.SaveFilePanel("Save Widget Theme", CorePath.WidgetSave, "new theme", "json");
			if (!string.IsNullOrEmpty(text))
			{
				if (!EMono.debug.enable && (text.Contains("Default.json") || text.Contains("Modern.json") || text.Contains("Classic.json")))
				{
					Dialog.Ok("dialogInvalidTheme");
				}
				else
				{
					Save(text);
					if (onSave != null)
					{
						onSave();
					}
				}
			}
		});
	}

	public void DialogLoad(Action onLoad = null)
	{
		EMono.core.WaitForEndOfFrame(delegate
		{
			string[] array = StandaloneFileBrowser.OpenFilePanel("Load Widget Theme", CorePath.WidgetSave, "json", multiselect: false);
			if (array.Length != 0)
			{
				Load(EMono.player.useSubWidgetTheme, array[0]);
				Reset(toggleTheme: false);
				if (onLoad != null)
				{
					onLoad();
				}
			}
		});
	}

	public void Save(string path = null)
	{
		if (path == null)
		{
			path = CorePath.WidgetSave + (EMono.player.useSubWidgetTheme ? EMono.core.config.other.idSubWidgetTheme : EMono.core.config.other.idMainWidgetTheme) + ".json";
		}
		UpdateConfigs();
		IO.SaveFile(path, EMono.player.widgets);
	}

	public void Load(bool isSubTheme, string path = null)
	{
		if (path == null)
		{
			path = CorePath.WidgetSave + (isSubTheme ? EMono.core.config.other.idSubWidgetTheme : EMono.core.config.other.idMainWidgetTheme) + ".json";
		}
		SaveData saveData = IO.LoadFile<SaveData>(path);
		if (isSubTheme)
		{
			EMono.player.subWidgets = saveData;
		}
		else
		{
			EMono.player.mainWidgets = saveData;
		}
		currentPath = path;
	}
}
