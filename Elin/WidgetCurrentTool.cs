using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WidgetCurrentTool : Widget
{
	public class Extra
	{
		public int iconSize;

		public bool showShortcut;

		public bool top;
	}

	public const int SlotsPerPage = 10;

	public const int MaxPage = 2;

	public static WidgetCurrentTool Instance;

	public ButtonHotItem buttonHotItem;

	public UIButton buttonSwitchPage;

	public Image iconHeld;

	public Image imageHighlight;

	public UIList list;

	public static bool dirty;

	public int maxSlots = 9;

	public List<Thing> grid = new List<Thing>(new Thing[20]);

	public UIPlaceHelper placer;

	public GridLayoutGroup layout;

	public Transform transHighlightSwitch;

	public int selected = -1;

	public int selectedPage;

	public Extra extra => base.config.extra as Extra;

	public int page
	{
		get
		{
			return EMono.player.hotbarPage;
		}
		set
		{
			EMono.player.hotbarPage = value;
		}
	}

	public ButtonGrid selectedButton => list.buttons[selected].component as ButtonGrid;

	public override object CreateExtra()
	{
		return new Extra();
	}

	public override void OnActivate()
	{
		transHighlightSwitch.SetActive(!EMono.player.flags.toggleHotbarHighlightDisabled);
		Instance = this;
		_RefreshCurrentHotItem();
		buttonHotItem.onClick.AddListener(OnClickButtonHotItem);
		buttonSwitchPage.SetOnClick(delegate
		{
			if (!EMono.player.flags.toggleHotbarHighlightDisabled && page == 1)
			{
				EMono.player.flags.toggleHotbarHighlightDisabled = true;
				transHighlightSwitch.SetActive(enable: false);
			}
			SwitchPage();
		});
		RebuildSlots();
		dirty = true;
		placer.Refresh();
		ApplySkin();
	}

	public void RebuildSlots()
	{
		imageHighlight.transform.SetParent(base.transform, worldPositionStays: false);
		layout.cellSize = EMono.setting.ui.iconSizes[extra.iconSize];
		SkinConfig skin = base.config.skin;
		list.Clear();
		list.callbacks = new UIList.Callback<Thing, ButtonGridDrag>
		{
			onRedraw = delegate(Thing a, ButtonGridDrag b, int i)
			{
				int num2 = i + 1;
				b.subText2.SetText((num2 % 10).ToString() ?? "");
				b.subText2.SetActive(extra.showShortcut);
				b.index = i % 10 + 10 * page;
				b.SetCardGrid(a, new InvOwnerHotbar(EMono.pc)
				{
					index = i % 10 + 10 * page
				});
			}
		};
		for (int num = 0; num < maxSlots; num++)
		{
			list.Add(null);
		}
		list.onBeforeRedraw = delegate
		{
			dirty = false;
			for (int i = 0; i < grid.Count; i++)
			{
				grid[i] = null;
			}
			foreach (Thing thing in EMono.pc.things)
			{
				if (thing.invY == 1 && thing.invX >= page * 10 && thing.invX < (page + 1) * 10)
				{
					if (thing.invX < 0 || thing.invX >= grid.Count)
					{
						Debug.Log(thing.Name + "/" + thing.invX);
					}
					else
					{
						grid[thing.invX] = thing;
					}
				}
			}
			for (int j = 0; j < list.buttons.Count; j++)
			{
				UIList.ButtonPair value = list.buttons[j];
				value.obj = grid[j + page * 10];
				list.buttons[j] = value;
			}
			buttonSwitchPage.mainText.text = (page + 1).ToString() ?? "";
		};
		list.onAfterRedraw = delegate
		{
			LayerInventory.TryShowGuide(list);
		};
		list.Refresh();
		list.Redraw();
		UIRawImage obj = list.bgGrid as UIRawImage;
		obj.skinRoot = GetComponent<SkinRoot>();
		obj.color = skin.gridColor;
		ClampToScreen();
		RefreshHighlight();
	}

	private void Update()
	{
		CheckDirty();
	}

	public void CheckDirty()
	{
		if (dirty)
		{
			list.Redraw();
			_RefreshCurrentHotItem();
		}
		dirty = false;
	}

	public static void RefreshCurrentHotItem()
	{
		if ((bool)Instance)
		{
			Instance._RefreshCurrentHotItem();
		}
	}

	public void SwitchPage()
	{
		page = ((page == 0) ? 1 : 0);
		_RefreshCurrentHotItem();
		dirty = true;
		CheckDirty();
		placer.Refresh();
		SE.Play("switch_hotbar");
	}

	public void _RefreshCurrentHotItem()
	{
		HotItem hotItem = EMono.player.currentHotItem;
		if (hotItem == null)
		{
			hotItem = EMono.player.hotItemNoItem;
		}
		buttonHotItem.item = hotItem;
		buttonHotItem.icon.material = (hotItem.UseUIObjMaterial ? EMono.core.refs.matUIObj : null);
		hotItem.SetImage(buttonHotItem.icon);
		buttonHotItem.interactable = true;
		buttonHotItem.Refresh();
		iconHeld.SetActive(hotItem is HotItemHeld);
		RefreshHighlight();
	}

	public void OnClickButtonHotItem()
	{
		SE.SelectHotitem();
		if (selected != -1 && EMono.player.currentHotItem.Thing == selectedButton.card)
		{
			Select(-1);
			return;
		}
		int index = selected;
		selected = -1;
		Select(index);
	}

	public void ModSelected(int a)
	{
		SE.SelectHotitem();
		int index = ((a > 0) ? GetNextSelectableIndex() : GetPrevSelectableIndex());
		Select(index);
	}

	public int GetNextSelectableIndex()
	{
		int num = selected + 1;
		if (num >= maxSlots)
		{
			num = -1;
		}
		return num;
	}

	public int GetPrevSelectableIndex()
	{
		int num = selected - 1;
		if (num <= -2)
		{
			num = maxSlots - 1;
		}
		return num;
	}

	public void Reselect()
	{
		Select(selected);
	}

	public void Select(int index, bool fromHotkey = false)
	{
		if (index != -1 && fromHotkey && EMono.core.config.game.useAbilityOnHotkey)
		{
			ButtonGrid buttonGrid = list.buttons[index].component as ButtonGrid;
			TraitAbility traitAbility = ((buttonGrid.card != null) ? (buttonGrid.card.trait as TraitAbility) : null);
			if (traitAbility != null && traitAbility.CanUse(EMono.pc))
			{
				if (EMono.player.CanAcceptInput() && traitAbility.OnUse(EMono.pc))
				{
					EMono.player.EndTurn();
				}
				return;
			}
		}
		if (fromHotkey)
		{
			SE.SelectHotitem();
		}
		if (selected == index && selectedPage == page)
		{
			index = -1;
		}
		selected = index;
		selectedPage = page;
		if (selected >= maxSlots)
		{
			selected = -1;
		}
		if (selected == -1 || selectedButton.card == null)
		{
			EMono.player.ResetCurrentHotItem();
		}
		else
		{
			EMono.player.SetCurrentHotItem(selectedButton.card.trait.GetHotItem());
		}
		if (ActionMode.Adv.IsActive)
		{
			ActionMode.Adv.UpdatePlans();
		}
		RefreshHighlight();
	}

	public void RefreshHighlight()
	{
		bool flag = selected != -1;
		imageHighlight.SetActive(flag);
		if (flag)
		{
			imageHighlight.transform.SetParent(selectedButton.transform, worldPositionStays: false);
			imageHighlight.transform.position = selectedButton.transform.position;
			imageHighlight.Rect().sizeDelta = selectedButton.Rect().sizeDelta;
			imageHighlight.transform.SetAsFirstSibling();
			imageHighlight.SetAlpha((EMono.player.currentHotItem.Thing == selectedButton.card) ? 1f : 0.35f);
		}
	}

	public override bool CanShowContextMenu()
	{
		ButtonGrid componentOf = InputModuleEX.GetComponentOf<ButtonGrid>();
		if ((bool)componentOf && componentOf != buttonHotItem)
		{
			return false;
		}
		return base.CanShowContextMenu();
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		InputModuleEX.GetComponentOf<ButtonHotItem>();
		UIContextMenu uIContextMenu = m.AddChild("setting");
		UIContextMenu uIContextMenu2 = m.AddChild("style");
		uIContextMenu2.AddSlider("iconSize", (float n) => n.ToString() ?? "", extra.iconSize, delegate(float a)
		{
			extra.iconSize = (int)a;
			RebuildSlots();
		}, 0f, EMono.setting.ui.iconSizes.Count - 1, isInt: true);
		SetGridContextMenu(uIContextMenu2);
		uIContextMenu.AddToggle("showShortcut", extra.showShortcut, delegate(bool a)
		{
			extra.showShortcut = a;
			RebuildSlots();
		});
		SetBaseContextMenu(m);
	}
}
