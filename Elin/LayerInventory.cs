using System;
using System.Collections.Generic;
using UnityEngine;

public class LayerInventory : ELayer
{
	public static List<LayerInventory> listInv = new List<LayerInventory>();

	public static InvOwner highlightInv;

	public List<UIInventory> invs = new List<UIInventory>();

	public UICurrency uiCurrency;

	public bool floatInv;

	public bool mainInv;

	public WindowCharaMini mini;

	[NonSerialized]
	public bool wasInventoryOpen;

	public InvOwner Inv => invs[0].tabs[0].owner;

	public static LayerInventory GetTopLayer(Thing t, bool includePlayer = false, InvOwner exclude = null)
	{
		LayerInventory result = null;
		int num = -1;
		foreach (LayerInventory item in listInv)
		{
			if (item.IsPlayerContainer(includePlayer) && item.Inv != exclude && !item.Inv.Container.IsToolbelt && !item.Inv.Container.things.IsFull(t, recursive: false))
			{
				int siblingIndex = item.transform.GetSiblingIndex();
				if (siblingIndex > num)
				{
					num = siblingIndex;
					result = item;
				}
			}
		}
		return result;
	}

	public static LayerInventory GetLayer(Card t)
	{
		LayerInventory result = null;
		foreach (LayerInventory item in listInv)
		{
			if (item.Inv.Container == t)
			{
				return item;
			}
		}
		return result;
	}

	public static LayerInventory GetPCLayer()
	{
		return GetLayer(ELayer.pc);
	}

	public static void Close(Thing t)
	{
		foreach (LayerInventory item in listInv)
		{
			if (item.invs[0].owner.Container == t)
			{
				item.Close();
				break;
			}
		}
	}

	public static bool IsOpen(Thing t)
	{
		foreach (LayerInventory item in listInv)
		{
			if (item.invs[0].owner.Container == t)
			{
				return true;
			}
		}
		return false;
	}

	public static void SetDirty(Thing t)
	{
		if (!ELayer.game.altInv || t == null)
		{
			return;
		}
		foreach (LayerInventory item in listInv)
		{
			if (item.invs[0].owner.Container == t.parent || ((bool)item.mini && item.mini.gameObject.activeInHierarchy))
			{
				item.invs[0].dirty = true;
			}
		}
		if (t.invY == 1 || ELayer.pc.held == t)
		{
			WidgetCurrentTool.dirty = true;
		}
		if (t.isEquipped)
		{
			WidgetEquip.dirty = true;
		}
	}

	public static void SetDirtyAll(bool immediate = false)
	{
		foreach (LayerInventory item in listInv)
		{
			item.invs[0].dirty = true;
			if (immediate)
			{
				item.invs[0].CheckDirty();
			}
		}
		if ((bool)WidgetEquip.Instance)
		{
			WidgetEquip.dirty = true;
			if (immediate)
			{
				WidgetEquip.Instance.CheckDirty();
			}
		}
		WidgetCurrentTool.dirty = true;
		if (immediate)
		{
			WidgetCurrentTool.Instance.CheckDirty();
		}
	}

	public static void TryShowGuide(UIList list)
	{
		List<ButtonGrid> list2 = new List<ButtonGrid>();
		foreach (UIList.ButtonPair button in list.buttons)
		{
			ButtonGrid buttonGrid = button.component as ButtonGrid;
			if ((bool)buttonGrid)
			{
				list2.Add(buttonGrid);
			}
		}
		TryShowGuide(list2);
	}

	public static void TryShowGuide(List<ButtonGrid> list)
	{
		bool flag = InvOwner.HasTrader && InvOwner.Trader.UseGuide;
		if (flag)
		{
			WidgetSearch.selected = null;
		}
		bool flag2 = (bool)WidgetSearch.Instance && WidgetSearch.selected != null && !ELayer.ui.IsDragging;
		if (!flag2 && WidgetEquip.dragEquip == null && LayerAbility.hotElement == null && !flag)
		{
			return;
		}
		foreach (ButtonGrid item in list)
		{
			Thing thing = item.card as Thing;
			if (LayerAbility.hotElement != null)
			{
				if ((bool)item && item.invOwner != null && (thing == null || thing.trait is TraitAbility) && item.invOwner.owner == ELayer.pc && !(item.invOwner is InvOwnerEquip))
				{
					item.Attach("guide", rightAttach: false);
				}
			}
			else if (WidgetEquip.dragEquip != null && !flag)
			{
				if (item.invOwner is InvOwnerEquip invOwnerEquip && invOwnerEquip.slot.elementId == WidgetEquip.dragEquip.category.slot)
				{
					item.Attach("guide", rightAttach: false);
				}
			}
			else if (flag2)
			{
				if (item.card == WidgetSearch.selected || item.card == WidgetSearch.selected.parent)
				{
					item.Attach("guide", rightAttach: false);
				}
			}
			else
			{
				if (thing == null)
				{
					continue;
				}
				bool show = InvOwner.Trader.ShouldShowGuide(thing);
				if (!show && thing.CanSearchContents)
				{
					thing.things.Foreach(delegate(Thing _t)
					{
						if (!show && InvOwner.Trader.ShouldShowGuide(_t))
						{
							show = true;
						}
					});
				}
				if (show)
				{
					item.Attach("guide", rightAttach: false);
				}
			}
		}
	}

	public bool IsPlayerContainer(bool includePlayer = false)
	{
		if (invs[0].tabs[0].mode != UIInventory.Mode.All)
		{
			return false;
		}
		if (!includePlayer && invs[0].owner.Container == ELayer.pc)
		{
			return false;
		}
		return invs[0].owner.Container.GetRootCard() == ELayer.pc;
	}

	public Card GetPlayerContainer()
	{
		UIInventory.Tab tab = invs[0].tabs[0];
		if (!IsPlayerContainer())
		{
			return null;
		}
		return tab.owner.Container;
	}

	public override bool HeaderIsListOf(int id)
	{
		return false;
	}

	public override void OnInit()
	{
		foreach (UIInventory inv in invs)
		{
			inv.OnInit();
		}
		listInv.Add(this);
		if (Inv.Container == ELayer.pc)
		{
			InvOwner.Main = Inv;
		}
		if (!floatInv)
		{
			InvOwner.Trader = Inv;
			wasInventoryOpen = ELayer.ui.IsInventoryOpen;
			if (!wasInventoryOpen)
			{
				ELayer.ui.OpenFloatInv(ignoreSound: true);
			}
		}
	}

	public override void OnAfterInit()
	{
		ELayer.core.actionsNextFrame.Add(delegate
		{
			ELayer.core.actionsNextFrame.Add(delegate
			{
				if ((bool)invs[0] && (bool)invs[0].gameObject)
				{
					invs[0].RefreshHighlight();
				}
			});
		});
	}

	public UIInventory SetInv(int idWindow = 0)
	{
		UIInventory uIInventory = invs[idWindow];
		uIInventory.window = windows[idWindow];
		uIInventory.layer = this;
		return uIInventory;
	}

	public override void OnUpdateInput()
	{
		if (EInput.action == EAction.MenuInventory || Input.GetKeyDown(KeyCode.Tab))
		{
			Close();
			EInput.WaitReleaseKey();
			return;
		}
		if (EInput.action == EAction.Dump)
		{
			TaskDump.TryPerform();
		}
		base.OnUpdateInput();
	}

	public override void OnKill()
	{
		listInv.Remove(this);
		EInput.haltInput = false;
		if (Inv == InvOwner.Trader)
		{
			if (Inv.UseGuide)
			{
				SetDirtyAll();
			}
			InvOwner.Trader = null;
			if (!wasInventoryOpen && ELayer.ui.IsInventoryOpen)
			{
				ELayer.ui.ToggleInventory();
			}
		}
		if (Inv.Container == ELayer.pc)
		{
			InvOwner.Main = null;
		}
		if (!ELayer.game.isKilling && Inv.owner == ELayer.pc)
		{
			SE.Play("pop_inventory_deactivate");
		}
	}

	private void OnDestroy()
	{
		if (invs.Count > 0)
		{
			SetDirty(invs[0].owner.Container.Thing);
		}
		listInv.Remove(this);
	}

	public override void OnRightClick()
	{
		if (invs[0].isList)
		{
			base.OnRightClick();
		}
		if (!invs[0].floatMode && InputModuleEX.GetComponentOf<ButtonGrid>() == null)
		{
			Close();
		}
	}

	public static LayerInventory _Create(string path = "")
	{
		if (ELayer.game.altInv && path.IsEmpty())
		{
			path = "LayerInventoryGrid";
		}
		return Layer.Create(path.IsEmpty() ? "LayerInventory" : ("LayerInventory/" + path)) as LayerInventory;
	}

	public static LayerInventory CreatePCBackpack(bool mousePos = false)
	{
		LayerInventory layerInventory = _Create("LayerInventoryFloatMain");
		Window window = layerInventory.windows[0];
		layerInventory.mainInv = true;
		window.setting.saveWindow = true;
		UIInventory uIInventory = layerInventory.SetInv();
		uIInventory.AddTab(new InvOwner(ELayer.pc));
		uIInventory.SetHeader("stash");
		uIInventory.floatMode = true;
		return layerInventory;
	}

	public static bool CloseAllyInv(Chara c)
	{
		foreach (LayerInventory item in listInv.Copy())
		{
			if (item.Inv.owner == c)
			{
				ELayer.ui.layerFloat.RemoveLayer(item);
				return true;
			}
		}
		return false;
	}

	public static void CloseAllyInv()
	{
		foreach (LayerInventory item in listInv.Copy())
		{
			if (!item.IsPlayerContainer(includePlayer: true))
			{
				ELayer.ui.layerFloat.RemoveLayer(item);
			}
		}
	}

	public static LayerInventory CreateContainerAlly(Chara owner, Card container)
	{
		SetDirty(container.Thing);
		CloseAllyInv();
		LayerInventory layerInventory = _Create("LayerInventoryFloat");
		UIInventory uIInventory = layerInventory.SetInv();
		Window window = layerInventory.windows[0];
		window.buttonClose.SetActive(enable: true);
		uIInventory.AddTab(new InvOwnerAlly(owner, container.Thing));
		uIInventory.tabs[0].textTab = container.Name;
		uIInventory.floatMode = true;
		if (ELayer.player.windowAllyInv == null)
		{
			Vector2 sizeDelta = window.Rect().sizeDelta;
			ELayer.player.windowAllyInv = new Window.SaveData
			{
				x = 0f,
				y = 200f,
				w = sizeDelta.x,
				h = sizeDelta.y,
				anchor = RectPosition.Center,
				useBG = ELayer.core.config.game.showInvBG,
				advDistribution = false
			};
		}
		window.saveData = ELayer.player.windowAllyInv;
		window.saveData.open = true;
		ELayer.ui.layerFloat.AddLayer(layerInventory);
		window.ClampToScreen();
		window.UpdateSaveData();
		return layerInventory;
	}

	public static bool IsOpen(Card container)
	{
		foreach (LayerInventory item in listInv)
		{
			if (item.GetPlayerContainer() == container)
			{
				return true;
			}
		}
		return false;
	}

	public static LayerInventory CreateContainerPC(Card container)
	{
		SetDirty(container.Thing);
		foreach (LayerInventory item in listInv)
		{
			if (item.GetPlayerContainer() == container)
			{
				ELayer.ui.layerFloat.RemoveLayer(item);
				return null;
			}
		}
		LayerInventory layerInventory = _Create("LayerInventoryFloat");
		UIInventory uIInventory = layerInventory.SetInv();
		Window window = layerInventory.windows[0];
		Vector2 anchoredPosition = default(Vector2);
		bool flag = container.c_windowSaveData == null;
		window.buttonClose.SetActive(enable: true);
		uIInventory.AddTab(new InvOwner(ELayer.pc, container.Thing));
		uIInventory.tabs[0].textTab = container.Name;
		uIInventory.floatMode = true;
		if (container.c_windowSaveData == null)
		{
			anchoredPosition = window.Rect().anchoredPosition + new Vector2(-80f, -80f);
			Vector2 sizeDelta = window.Rect().sizeDelta;
			container.c_windowSaveData = new Window.SaveData
			{
				x = anchoredPosition.x,
				y = anchoredPosition.y,
				w = sizeDelta.x,
				h = sizeDelta.y,
				anchor = RectPosition.Auto,
				useBG = ELayer.core.config.game.showInvBG,
				advDistribution = false
			};
			if (container.trait.IsFridge)
			{
				container.c_windowSaveData.onlyRottable = true;
			}
		}
		if (container.IsToolbelt)
		{
			container.c_windowSaveData.useBG = false;
		}
		window.saveData = container.c_windowSaveData;
		window.saveData.open = true;
		if (container.IsToolbelt)
		{
			return layerInventory;
		}
		ELayer.ui.layerFloat.AddLayer(layerInventory);
		if (flag)
		{
			RectTransform rectTransform = window.Rect();
			if (ELayer.player.openContainerCenter)
			{
				rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
				rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
				anchoredPosition = new Vector2(0f, rectTransform.sizeDelta.y / 2f);
			}
			else
			{
				RectTransform rectTransform2 = null;
				RectTransform rectTransform3 = null;
				foreach (LayerInventory item2 in listInv)
				{
					if (!(item2 == layerInventory) && item2.IsFloat)
					{
						RectTransform rectTransform4 = item2.windows[0].Rect();
						if (!rectTransform3 || rectTransform4.Rect().localPosition.x < rectTransform3.Rect().localPosition.x)
						{
							rectTransform3 = rectTransform4;
						}
						if (!rectTransform2 || rectTransform4.Rect().localPosition.y > rectTransform2.Rect().localPosition.y)
						{
							rectTransform2 = rectTransform4;
						}
					}
				}
				if (uIInventory.tabs[0].owner.Container.things.width < 3 && (bool)rectTransform3)
				{
					rectTransform.anchorMin = rectTransform3.anchorMin;
					rectTransform.anchorMax = rectTransform3.anchorMax;
					anchoredPosition.x = rectTransform3.anchoredPosition.x - rectTransform.sizeDelta.x - rectTransform3.sizeDelta.x * 0.5f + 35f;
					anchoredPosition.y = rectTransform3.anchoredPosition.y;
				}
				else if ((bool)rectTransform2)
				{
					rectTransform.anchorMin = rectTransform2.anchorMin;
					rectTransform.anchorMax = rectTransform2.anchorMax;
					anchoredPosition.x = rectTransform2.anchoredPosition.x;
					anchoredPosition.y = rectTransform2.anchoredPosition.y + rectTransform.sizeDelta.y - 25f;
				}
				else
				{
					rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
					rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
					anchoredPosition = Vector2.one;
				}
			}
			rectTransform.anchoredPosition = anchoredPosition;
			window.ClampToScreen();
			window.UpdateSaveData();
		}
		return layerInventory;
	}

	public static LayerInventory CreateContainer(Card owner)
	{
		if (owner.GetRootCard() == ELayer.pc)
		{
			return CreateContainerPC(owner);
		}
		Card card = owner;
		if (owner.trait is TraitShippingChest)
		{
			card = ELayer.game.cards.container_shipping;
			card.things.SetSize(owner.things.width, owner.things.height);
			ELayer.player.uidLastShippedZone = ELayer._zone.uid;
		}
		if (owner.trait is TraitDeliveryChest)
		{
			card = ELayer.game.cards.container_deliver;
			Tutorial.Play("deliver_box");
		}
		return CreateContainer(owner, card);
	}

	public static LayerInventory CreateContainer(Card owner, Card container)
	{
		if (container.isChara)
		{
			SE.PopInventory();
		}
		LayerInventory layerInventory = _Create();
		layerInventory.SetInv().AddTab(owner, UIInventory.Mode.Take, container.Thing).dest = ELayer.pc;
		if (container.c_windowSaveData == null)
		{
			container.c_windowSaveData = new Window.SaveData
			{
				useBG = true
			};
			if (container == ELayer.game.cards.container_shipping)
			{
				container.c_windowSaveData.autodump = AutodumpFlag.none;
			}
		}
		layerInventory.windows[0].saveData = container.c_windowSaveData;
		ELayer.ui.AddLayer(layerInventory);
		if (container.id == "barrel")
		{
			if ((bool)WidgetFeed.Instance)
			{
				WidgetFeed.Instance.Nerun("barrel".langList().RandomItem());
			}
			ELayer.player.forceTalk = true;
			ELayer.pc.party.members.RandomItem().Talk("barrel");
		}
		return layerInventory;
	}

	public static LayerInventory CreateContainer<T>(Card c, Card container, CurrencyType currency = CurrencyType.None) where T : InvOwner
	{
		LayerInventory layerInventory = _Create();
		UIInventory uIInventory = layerInventory.SetInv();
		T owner = Activator.CreateInstance(typeof(T), c, container, currency) as T;
		uIInventory.AddTab(owner, UIInventory.Mode.Buy).dest = ELayer.pc;
		if (container.c_windowSaveData == null)
		{
			container.c_windowSaveData = new Window.SaveData
			{
				useBG = true
			};
		}
		layerInventory.windows[0].saveData = container.c_windowSaveData;
		ELayer.ui.AddLayer(layerInventory);
		return layerInventory;
	}

	public static LayerInventory CreateBuy(Card c, CurrencyType currency = CurrencyType.Money, PriceType price = PriceType.Default)
	{
		LayerInventory layerInventory = _Create();
		UIInventory uIInventory = layerInventory.SetInv();
		Thing thing = c.things.Find("chest_merchant");
		SE.Play("shop_open");
		InvOwnerShop invOwnerShop = new InvOwnerShop(c, thing, currency, price);
		uIInventory.AddTab(invOwnerShop, UIInventory.Mode.Buy).dest = ELayer.pc;
		if (Window.dictData.TryGetValue("ChestMerchant") == null)
		{
			Window.dictData.Add("ChestMerchant", new Window.SaveData
			{
				useBG = true
			});
		}
		layerInventory.windows[0].saveData = thing.GetWindowSaveData();
		uIInventory.tabs[0].owner.BuildUICurrency(layerInventory.uiCurrency, c.trait.CostRerollShop != 0);
		ShopTransaction.current = new ShopTransaction
		{
			trader = invOwnerShop
		};
		layerInventory.SetOnKill(ShopTransaction.current.OnEndTransaction);
		return layerInventory;
	}
}
