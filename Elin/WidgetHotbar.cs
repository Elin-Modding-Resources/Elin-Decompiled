using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WidgetHotbar : Widget, IDragParent
{
	public class Extra
	{
		public int iconSize = 2;

		public int width = 1;

		public int rows;

		public bool showShortcut;

		public bool vertical;

		public bool visible;

		public bool alwaysShow;

		public bool reverse;

		public bool autoSize;
	}

	public static bool registering;

	public static bool dirtyCurrentItem;

	public static HotItem registeringItem;

	public static WidgetHotbar HotbarBuild;

	public static WidgetHotbar HotBarMainMenu;

	public static WidgetHotbar HotbarExtra;

	public int idHotbar;

	public GridLayoutGroup layout;

	public ButtonHotItem mold;

	public RawImage imageGrid;

	public Image imageSelect;

	public Hotbar.Type type;

	public bool useMask;

	[NonSerialized]
	public List<ButtonHotItem> buttons = new List<ButtonHotItem>();

	private bool showThisWidget;

	private float timeSinceBecomeVisible;

	public Extra extra => base.config.extra as Extra;

	public Hotbar hotbar => EMono.player.hotbars.bars[idHotbar];

	public bool Visible
	{
		get
		{
			return extra.visible;
		}
		set
		{
			extra.visible = value;
		}
	}

	public override bool ShowInBuildMode => extra.alwaysShow;

	public bool IsHotbarSpeed => idHotbar == 7;

	public bool CanRegisterItem => hotbar.IsUserHotbar;

	public override object CreateExtra()
	{
		return new Extra();
	}

	public override bool CanShowContextMenu()
	{
		if (hotbar.IsUserHotbar)
		{
			ButtonHotItem componentOf = InputModuleEX.GetComponentOf<ButtonHotItem>();
			if ((bool)componentOf && componentOf.item != null)
			{
				return true;
			}
		}
		return base.CanShowContextMenu();
	}

	public override void OnActivate()
	{
		mold = layout.CreateMold<ButtonHotItem>();
		if (idHotbar == 2)
		{
			HotBarMainMenu = this;
		}
		if (idHotbar == 3)
		{
			HotbarBuild = this;
		}
		if (idHotbar == 5)
		{
			HotbarExtra = this;
		}
		if (extra.rows == 0)
		{
			extra.rows = hotbar.itemsPerPage;
		}
		hotbar.actor = this;
		Rebuild();
	}

	public override void OnChangeActionMode()
	{
		base.OnChangeActionMode();
		if (hotbar.dirty)
		{
			hotbar.dirty = false;
			Rebuild();
		}
		RefreshHighlight();
	}

	public void Rebuild()
	{
		buttons.Clear();
		layout.cellSize = EMono.setting.ui.iconSizes[extra.iconSize];
		layout.constraintCount = extra.width;
		layout.constraint = (extra.vertical ? GridLayoutGroup.Constraint.FixedColumnCount : GridLayoutGroup.Constraint.FixedRowCount);
		layout.startCorner = (extra.reverse ? GridLayoutGroup.Corner.LowerRight : GridLayoutGroup.Corner.UpperLeft);
		int num = extra.rows;
		if (extra.autoSize)
		{
			num = 0;
			for (int i = 0; i < hotbar.CurrentPage.items.Count && hotbar.CurrentPage.items[i] != null; i++)
			{
				num++;
			}
		}
		int num2 = (num - 1) / extra.width + 1;
		int num3 = (extra.vertical ? extra.width : num2);
		int num4 = (extra.vertical ? num2 : extra.width);
		imageGrid.uvRect = new Rect(1f, 1f, num3, num4);
		layout.DestroyChildren();
		for (int j = 0; j < num; j++)
		{
			ButtonHotItem buttonHotItem = Util.Instantiate(mold, layout);
			buttonHotItem.index = j;
			buttonHotItem.mainText.text = ((j > 10) ? "" : ((j + 1).ToString() ?? ""));
			buttonHotItem.widget = this;
			buttons.Add(buttonHotItem);
			if (useMask)
			{
				buttonHotItem.gameObject.AddComponent<RectMask2D>();
			}
		}
		layout.RebuildLayout();
		this.RebuildLayout();
		RebuildPage();
	}

	public void RebuildPage(int page = -1)
	{
		hotbar.SetPage((page == -1) ? hotbar.currentPage : page);
		foreach (ButtonHotItem button in buttons)
		{
			HotItem item = hotbar.GetItem(button.index);
			button.SetItem(item);
		}
		SetVisible();
		RefreshHighlight();
	}

	public static void RebuildPages()
	{
		foreach (Widget item in EMono.ui.widgets.list)
		{
			WidgetHotbar widgetHotbar = item as WidgetHotbar;
			if ((bool)widgetHotbar)
			{
				widgetHotbar.RebuildPage();
			}
		}
	}

	public void SwitchPage()
	{
		SE.ClickGeneral();
		RebuildPage((hotbar.currentPage == 0) ? 1 : 0);
	}

	public HotItem GetItem(int index)
	{
		return hotbar.CurrentPage.items.TryGet(index, returnNull: true);
	}

	public void TryUse(int index)
	{
		HotItem item = GetItem(index);
		if (item == null)
		{
			SE.BeepSmall();
		}
		else
		{
			item.OnClick(item.button, hotbar);
		}
	}

	public void OnClickEmptyItem(ButtonHotItem b)
	{
		UIContextMenu uIContextMenu = EMono.ui.CreateContextMenu();
		SetShortcutMenu(b, uIContextMenu);
		uIContextMenu.Show();
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		ButtonHotItem b = InputModuleEX.GetComponentOf<ButtonHotItem>();
		if (showThisWidget || !b || b.item == null || base.IsSealed || b.widget.hotbar.IsLocked)
		{
			showThisWidget = false;
			_ = base.config.skin.Skin;
			UIContextMenu uIContextMenu = m.AddChild("setting");
			UIContextMenu uIContextMenu2 = m.AddChild("style");
			if (!base.IsSealed)
			{
				uIContextMenu.AddSlider("numSlot", (float n) => n.ToString() ?? "", extra.rows, delegate(float a)
				{
					extra.rows = (int)a;
					hotbar.SetSlotNum((int)a);
					Rebuild();
					ClampToScreen();
				}, 1f, 20f, isInt: true, hideOther: false);
			}
			uIContextMenu2.AddSlider("iconSize", (float n) => n.ToString() ?? "", extra.iconSize, delegate(float a)
			{
				extra.iconSize = (int)a;
				Rebuild();
				ClampToScreen();
			}, 0f, EMono.setting.ui.iconSizes.Count - 1, isInt: true);
			SetGridContextMenu(uIContextMenu2);
			uIContextMenu.AddToggle("vertical", extra.vertical, delegate(bool a)
			{
				extra.vertical = a;
				Rebuild();
				ClampToScreen();
			});
			uIContextMenu.AddToggle("doubleBar", extra.width == 2, delegate(bool a)
			{
				extra.width = ((!a) ? 1 : 2);
				Rebuild();
				ClampToScreen();
			});
			uIContextMenu.AddToggle("reverseOrder", extra.reverse, delegate(bool a)
			{
				extra.reverse = a;
				Rebuild();
				ClampToScreen();
			});
			if (!base.IsSealed)
			{
				uIContextMenu.AddToggle("alwaysShow2", extra.alwaysShow, delegate(bool a)
				{
					extra.alwaysShow = a;
				});
			}
			SetBaseContextMenu(m);
			if (base.IsSealed)
			{
				return;
			}
			m.AddButton("resetHotbar", delegate
			{
				Dialog.YesNo("dialogResetHotbar", delegate
				{
					EMono.player.hotbars.ResetHotbar(hotbar.id);
					SE.Trash();
				});
			});
			return;
		}
		m.AddButton("removeHotItem", delegate
		{
			hotbar.SetItem(null, b.index);
			RebuildPage();
		});
		b.item.OnShowContextMenu(m);
		m.AddToggle("alwaysShow", b.item.always, delegate(bool on)
		{
			b.item.always = on;
		});
		m.AddButton("thisWidget", delegate
		{
			showThisWidget = true;
			EMono.core.actionsNextFrame.Add(delegate
			{
				ShowContextMenu();
			});
		});
	}

	public void SetShortcutMenu(ButtonHotItem b, UIContextMenu m)
	{
		Action<UIContextMenu, HotItem> action = delegate(UIContextMenu _m, HotItem i)
		{
			_m.AddButton(i.Name, delegate
			{
				SetItem(b, i);
			});
		};
		UIContextMenu arg = m.AddChild("layerShortcuts");
		action(arg, new HotItemLayer
		{
			id = "LayerHelp"
		});
		action(arg, new HotItemLayer
		{
			id = "stash"
		});
		action(arg, new HotItemLayer
		{
			id = "LayerAbility"
		});
		action(arg, new HotItemLayer
		{
			id = "LayerChara"
		});
		action(arg, new HotItemLayer
		{
			id = "LayerJournal"
		});
		action(arg, new HotItemWidget
		{
			id = "Roster"
		});
		action(arg, new HotItemWidget
		{
			id = "Codex"
		});
		action(arg, new HotItemWidget
		{
			id = "Search"
		});
		action(arg, new HotItemWidget
		{
			id = "Tracker"
		});
		action(arg, new HotItemWidget
		{
			id = "Memo"
		});
		action(arg, new HotItemWidget
		{
			id = "QuestTracker"
		});
		arg = m.AddChild("utilShortcuts");
		action(arg, new HotItemToggle
		{
			type = HotItemToggle.Type.ToggleNoRoof
		});
		action(arg, new HotItemToggle
		{
			type = HotItemToggle.Type.muteBGM
		});
		action(arg, new HotItemToggle
		{
			type = HotItemToggle.Type.showBalloon
		});
		action(arg, new HotItemActionAudoDump());
		arg.AddButton("hotActionWidgetSet".lang(), delegate
		{
			SetItem(b, new HotItemWidgetSet().Register());
		});
		if (EMono.core.config.test.unsealWidgets)
		{
			arg = m.AddChild("menuShortcuts");
			action(arg, new HotItemContext
			{
				id = "system"
			});
			if ((bool)this == IsHotbarSpeed)
			{
				arg = m.AddChild("uniqueShortcuts");
				action(arg, new HotItemSpeed
				{
					id = 0
				});
				action(arg, new HotItemSpeed
				{
					id = 1
				});
				action(arg, new HotItemSpeed
				{
					id = 2
				});
				action(arg, new HotItemSpeed
				{
					id = 3
				});
			}
		}
		arg = m.AddChild("specialShortcuts");
		action(arg, new HotItemTogglePage());
		action(arg, new HotItemToggleVisible());
		m.AddButton("registerPos".lang(), delegate
		{
			SetItem(b, new HotItemFocusPos
			{
				zone = EMono.game.activeZone,
				x = EMono.pc.pos.x,
				y = EMono.pc.pos.z
			});
		});
		m.AddButton("hotActionEQSet".lang(), delegate
		{
			SetItem(b, new HotItemEQSet().Register());
		});
		m.AddButton("hotActionSleep".lang(), delegate
		{
			SetItem(b, new HotItemActionSleep());
		});
	}

	public void SetItem(ButtonHotItem b, HotItem item)
	{
		item = hotbar.SetItem(item, b.index);
		b.SetItem(item);
		RefreshHighlight();
		SetVisible();
	}

	public static void RefreshHighlights()
	{
		foreach (Widget item in EMono.ui.widgets.list)
		{
			WidgetHotbar widgetHotbar = item as WidgetHotbar;
			if ((bool)widgetHotbar)
			{
				widgetHotbar.RefreshHighlight();
			}
		}
	}

	public static void RefreshButtons()
	{
		foreach (Widget item in EMono.ui.widgets.list)
		{
			WidgetHotbar widgetHotbar = item as WidgetHotbar;
			if (!widgetHotbar)
			{
				continue;
			}
			foreach (ButtonHotItem button in widgetHotbar.buttons)
			{
				button.RefreshItem();
			}
		}
	}

	public bool RefreshHighlight()
	{
		bool result = false;
		foreach (ButtonHotItem button in buttons)
		{
			if (button.item == null)
			{
				continue;
			}
			if (button.item.UseIconForHighlight)
			{
				if (button.item.ShouldHighlight())
				{
					result = true;
					button.icon.sprite = button.item.GetSprite(highlight: true);
				}
				else
				{
					button.icon.sprite = button.item.GetSprite();
				}
			}
			else if (button.item.ShouldHighlight())
			{
				result = true;
				button.image.sprite = button.item.SpriteHighlight;
				if (!Visible && button.item.KeepVisibleWhenHighlighted)
				{
					ToggleVisible();
				}
			}
			else
			{
				button.image.sprite = EMono.core.refs.spritesHighlight[0];
			}
		}
		return result;
	}

	public void _OnDirtyInventory()
	{
		foreach (ButtonHotItem button in buttons)
		{
			if (button.item != null)
			{
				button.Refresh();
			}
		}
	}

	public void SetVisible()
	{
		UIButton componentOf = InputModuleEX.GetComponentOf<UIButton>();
		int num = 0;
		bool flag = false;
		foreach (ButtonHotItem button in buttons)
		{
			bool flag2 = button.item is HotItemToggleVisible;
			bool flag3 = Visible || flag2 || (button.item != null && button.item.always);
			button.image.enabled = flag3;
			button.mainText.enabled = flag3;
			button.icon.SetActive(flag3);
			if (!flag2 && button != componentOf)
			{
				button.DoNormalTransition();
			}
			if (flag3)
			{
				num++;
			}
			if (flag2)
			{
				flag = true;
			}
		}
		if (num == 0 && !Visible)
		{
			Visible = true;
			SetVisible();
		}
		CancelInvoke("CheckAutoHide");
		if (flag)
		{
			timeSinceBecomeVisible = 0f;
			InvokeRepeating("CheckAutoHide", 0.1f, 0.2f);
		}
		Image image = imageBG;
		bool flag4 = (imageGrid.enabled = extra.visible);
		image.enabled = flag4;
		dragPanel.SetActive(extra.visible);
	}

	public void ToggleVisible()
	{
		extra.visible = !extra.visible;
		SetVisible();
		if (!Visible)
		{
			return;
		}
		imageBG.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
		imageBG.transform.DOScale(1f, 0.12f).SetEase(Ease.Linear);
		imageGrid.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
		imageGrid.transform.DOScale(1f, 0.12f).SetEase(Ease.Linear);
		foreach (ButtonHotItem button in buttons)
		{
			if (!(button.item is HotItemToggleVisible) && (button.item == null || !button.item.always))
			{
				button.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
				button.transform.DOScale(1f, 0.12f).SetEase(Ease.Linear);
			}
		}
		imageBG.SetAlpha(0f);
		imageGrid.SetAlpha(0f);
		SkinConfig skin = base.config.skin;
		imageBG.DOFade(skin.bgColor.a, 0.4f).SetEase(Ease.OutQuint);
		imageGrid.DOFade(skin.gridColor.a, 0.4f).SetEase(Ease.OutQuint);
	}

	public void CheckAutoHide()
	{
		if (!Visible)
		{
			return;
		}
		timeSinceBecomeVisible += 0.2f;
		if (timeSinceBecomeVisible < 1f)
		{
			return;
		}
		if (Input.GetMouseButton(0) || EMono.ui.contextMenu.currentMenu != null || registering)
		{
			timeSinceBecomeVisible = 0f;
			return;
		}
		if (InputModuleEX.IsPointerOver(base.transform))
		{
			timeSinceBecomeVisible = 0f;
			return;
		}
		foreach (ButtonHotItem button in buttons)
		{
			if (button.item != null && button.item.KeepVisibleWhenHighlighted && button.item.ShouldHighlight())
			{
				return;
			}
		}
		ToggleVisible();
	}

	public void OnStartDrag(UIButton b)
	{
		EMono.ui.hud.SetDragImage(b.icon);
	}

	public void OnDrag(UIButton b)
	{
		string text = "";
		if ((bool)GetSwapButton(b))
		{
			text = "hotitemSwap";
		}
		else if (!EMono.ui.isPointerOverUI)
		{
			text = "hotitemTrash";
		}
		EMono.ui.hud.SetDragText(text);
	}

	public ButtonHotItem GetSwapButton(UIButton b)
	{
		foreach (Widget item in EMono.ui.widgets.list)
		{
			WidgetHotbar widgetHotbar = item as WidgetHotbar;
			if (widgetHotbar == null)
			{
				continue;
			}
			foreach (ButtonHotItem button in widgetHotbar.buttons)
			{
				if (InputModuleEX.IsPointerOver(button) && b != button)
				{
					return button;
				}
			}
		}
		return null;
	}

	public void OnEndDrag(UIButton b, bool cancel = false)
	{
		EMono.ui.hud.imageDrag.SetActive(enable: false);
		ButtonHotItem swapButton = GetSwapButton(b);
		if ((bool)swapButton)
		{
			SE.SelectHotitem();
			ButtonHotItem buttonHotItem = b as ButtonHotItem;
			HotItem hotItem = (HotItem)swapButton.item;
			HotItem hotItem2 = (HotItem)buttonHotItem.item;
			Hotbar hotbar = swapButton.widget.hotbar;
			Hotbar hotbar2 = buttonHotItem.widget.hotbar;
			hotbar.actor.SetItem(swapButton, hotItem2);
			hotbar2.actor.SetItem(buttonHotItem, hotItem);
			if (hotItem != null)
			{
				hotItem.button = buttonHotItem;
				hotItem.hotbar = hotbar2;
			}
			if (hotItem2 != null)
			{
				hotItem2.button = swapButton;
				hotItem2.hotbar = hotbar;
			}
			UIInventory.RefreshAllList();
		}
		else if (EMono.ui.isPointerOverUI)
		{
			SE.Beep();
		}
		else
		{
			SE.Trash();
			SetItem(b as ButtonHotItem, null);
			UIInventory.RefreshAllList();
		}
	}
}
