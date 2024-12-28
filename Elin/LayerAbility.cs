using System;
using System.Collections.Generic;
using UnityEngine;

public class LayerAbility : ELayer, IDragParent
{
	[Serializable]
	public class Config
	{
		public bool autoHideBG;

		public bool hideDepletedSpell;

		public bool fixedSize;

		public bool fixedPos;

		public int bgAlpha;
	}

	public static LayerAbility Instance;

	public static ButtonAbility hotElement;

	public UIDynamicList list;

	public Chara chara;

	public Transform headerRow;

	[NonSerialized]
	public string[] idGroup = new string[6] { "all", "attack", "defense", "util", "ability", "favAbility" };

	public new Config config => ELayer.player.layerAbilityConfig;

	public override void OnInit()
	{
		chara = ELayer.pc;
		string[] array = idGroup;
		foreach (string text in array)
		{
			windows[0].AddTab("cat_" + text, windows[0].CurrentContent);
		}
		Instance = this;
		list.sortMode = ELayer.player.pref.sortAbility;
		if (ELayer.game.altAbility)
		{
			UIButton buttonSort = windows[0].buttonSort;
			buttonSort.onClick.RemoveAllListeners();
			buttonSort.onClick.AddListener(delegate
			{
				UIContextMenu uIContextMenu = ELayer.ui.CreateContextMenuInteraction();
				uIContextMenu.layoutGroup.childAlignment = TextAnchor.UpperLeft;
				uIContextMenu.alwaysPopLeft = true;
				UIContextMenu uIContextMenu2 = uIContextMenu.AddChild("sort", TextAnchor.UpperRight);
				UIList.SortMode[] sorts = list.sorts;
				for (int j = 0; j < sorts.Length; j++)
				{
					UIList.SortMode sortMode = sorts[j];
					string text2 = ((list.sortMode == sortMode) ? "context_checker".lang() : "");
					UIList.SortMode _sort = sortMode;
					uIContextMenu2.AddButton(text2 + sortMode.ToString().lang(), delegate
					{
						list.List(_sort);
						SE.Click();
					});
				}
				UIContextMenu uIContextMenu3 = uIContextMenu.AddChild("config", TextAnchor.UpperRight);
				uIContextMenu3.AddSlider("alpha", (float a) => a.ToString() ?? "", config.bgAlpha, delegate(float b)
				{
					config.bgAlpha = (int)b;
					RefreshConfig();
				}, 0f, 100f, isInt: true, hideOther: false);
				uIContextMenu3.AddToggle("autoHideBG", config.autoHideBG, delegate(bool a)
				{
					config.autoHideBG = a;
					RefreshConfig();
				});
				uIContextMenu3.AddToggle("hideDepletedSpell", config.hideDepletedSpell, delegate(bool a)
				{
					config.hideDepletedSpell = a;
					list.List();
				});
				uIContextMenu3.AddToggle("fixedSize", config.fixedSize, delegate(bool a)
				{
					config.fixedSize = a;
					RefreshConfig();
				});
				uIContextMenu3.AddToggle("fixedPos", config.fixedPos, delegate(bool a)
				{
					config.fixedPos = a;
					RefreshConfig();
				});
				uIContextMenu.Show();
			});
		}
		windows[0].idTab = ELayer.player.pref.lastIdTabAbility;
		RefreshConfig();
	}

	public void RefreshConfig()
	{
		Window window = windows[0];
		window.imageBG.color = Color.white.SetAlpha(0.01f * (float)config.bgAlpha);
		window.listCgFloat.Clear();
		if (config.autoHideBG)
		{
			window.listCgFloat.Add(window.cgBG);
		}
		window.cgBG.enabled = config.autoHideBG;
		window.setting.allowMove = !config.fixedPos;
		window.setting.allowResize = !config.fixedSize;
	}

	public static void SetDirty(Element a)
	{
		if ((bool)Instance)
		{
			Instance.list.Redraw();
		}
		foreach (LayerInventory item in LayerInventory.listInv)
		{
			foreach (UIList.ButtonPair button in item.invs[0].list.buttons)
			{
				ButtonGrid buttonGrid = button.component as ButtonGrid;
				if (buttonGrid.card != null && buttonGrid.card.trait is TraitAbility)
				{
					item.invs[0].list.callbacks.OnRedraw(buttonGrid.card, buttonGrid, buttonGrid.index);
				}
			}
		}
		if (!WidgetCurrentTool.Instance)
		{
			return;
		}
		foreach (UIList.ButtonPair button2 in WidgetCurrentTool.Instance.list.buttons)
		{
			ButtonGrid buttonGrid2 = button2.component as ButtonGrid;
			if (buttonGrid2.card != null && buttonGrid2.card.trait is TraitAbility)
			{
				WidgetCurrentTool.Instance.list.callbacks.OnRedraw(buttonGrid2.card, buttonGrid2, buttonGrid2.index);
			}
		}
		WidgetCurrentTool.RefreshCurrentHotItem();
	}

	public override void OnSwitchContent(Window window)
	{
		ELayer.player.pref.lastIdTabAbility = window.idTab;
		SelectGroup(windows[0].setting.tabs[window.idTab].idLang.Replace("cat_", ""));
	}

	public override void OnUpdateInput()
	{
		if (EInput.middleMouse.down)
		{
			ButtonAbility componentOf = InputModuleEX.GetComponentOf<ButtonAbility>();
			if ((bool)componentOf)
			{
				ELayer.player.SetCurrentHotItem(new HotItemAct(componentOf.source));
				SE.SelectHotitem();
				return;
			}
		}
		base.OnUpdateInput();
	}

	public static void ClearHotElement()
	{
		ButtonAbility buttonAbility = hotElement;
		hotElement = null;
		LayerInventory.SetDirtyAll();
		if ((bool)buttonAbility && buttonAbility.attach != null)
		{
			UnityEngine.Object.Destroy(buttonAbility.attach.gameObject);
		}
		EInput.Consume();
		ELayer.ui.hud.SetDragImage(null);
	}

	public void SelectGroup(string id)
	{
		list.Clear();
		list.callbacks = new UIList.Callback<Element, ButtonAbility>
		{
			onClick = delegate(Element a, ButtonAbility b)
			{
				if (ELayer.ui.IsActive)
				{
					SE.BeepSmall();
				}
				else
				{
					SE.Equip();
					hotElement = b;
					b.attach = Util.Instantiate("UI/Element/Grid/Attach/guide_ability", b.transform);
					LayerInventory.SetDirtyAll();
					ELayer.ui.hud.SetDragImage(b.icon);
				}
			},
			onRedraw = delegate(Element a, ButtonAbility b, int i)
			{
				b.dragParent = this;
				b.SetAct(chara, a);
			},
			onList = delegate(UIList.SortMode m)
			{
				List<Element> list = new List<Element>();
				foreach (Element value in ELayer.pc.elements.dict.Values)
				{
					string categorySub = value.source.categorySub;
					if (value.Value != 0 && (!config.hideDepletedSpell || !value.PotentialAsStock || value.vPotential > 0))
					{
						if (id == "favAbility" && ELayer.player.favAbility.Contains(value.id))
						{
							list.Add(value);
						}
						else if (id == categorySub || (id == "all" && idGroup.Contains(categorySub)))
						{
							list.Add(value);
						}
					}
				}
				list.Sort((Element a, Element b) => a.GetSortVal(m) - b.GetSortVal(m));
				foreach (Element item in list)
				{
					this.list.Add(item);
				}
			}
		};
		list.List();
	}

	public static void Redraw()
	{
		if ((bool)Instance)
		{
			Instance._Redraw();
		}
	}

	public void _Redraw()
	{
		list.List();
	}

	public void OnStartDrag(UIButton b)
	{
		ELayer.ui.hud.SetDragImage(b.icon);
	}

	public void OnDrag(UIButton b)
	{
		ButtonAbility buttonAbility = b as ButtonAbility;
		string text = "";
		LayerRegisterHotbar layer = ELayer.ui.GetLayer<LayerRegisterHotbar>();
		if ((bool)(layer?.GetButton() ?? null))
		{
			text = "hotitemRegister";
		}
		else if (!layer)
		{
			ELayer.ui.AddLayer<LayerRegisterHotbar>().SetItem(buttonAbility.source);
		}
		ELayer.ui.hud.SetDragText(text);
	}

	public void OnEndDrag(UIButton b, bool cancel = false)
	{
		ELayer.ui.hud.imageDrag.SetActive(enable: false);
		_ = (b as ButtonAbility).source;
		LayerRegisterHotbar layer = ELayer.ui.GetLayer<LayerRegisterHotbar>();
		if ((bool)layer)
		{
			layer.OnEndDrag();
			ELayer.ui.RemoveLayer<LayerRegisterHotbar>();
		}
	}

	public void OnDisable()
	{
		ELayer.player.pref.sortAbility = list.sortMode;
		ELayer.ui.hud.imageDrag.SetActive(enable: false);
		ELayer.ui.RemoveLayer<LayerRegisterHotbar>();
	}
}
