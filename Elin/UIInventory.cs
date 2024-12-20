using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : EMono
{
	public class Transaction
	{
		public Thing thing;

		public int num;

		public int price;

		public Mode mode;

		public Card from;
	}

	public enum Mode
	{
		Default,
		Equip,
		Buy,
		Sell,
		Gear,
		Other,
		Select,
		Food,
		Item,
		Resource,
		Read,
		Hold,
		Tool,
		Put,
		Take,
		Give,
		Trade,
		All,
		Offer,
		Identify,
		Drink,
		Deliver,
		HoldFurniture,
		HoldBlock,
		Recycle
	}

	public enum SubMode
	{
		Default,
		IdentifyScroll,
		DeliverTax
	}

	public enum InteractMode
	{
		Auto,
		Hold,
		Drop
	}

	public class Tab
	{
		public Card dest;

		public InvOwner owner;

		public Mode mode;

		public SubMode subMode;

		public string textTab;

		public bool smallTab;
	}

	public UIList list;

	public BodySlot bodySlot;

	public LayerInventory layer;

	public Transform headerRow;

	public Transform transOverflow;

	public string idMold;

	public Func<Thing, bool> funcList;

	public Action<Thing> actionClick;

	public Window window;

	public UIContent content;

	public Tab currentTab;

	public AIAct currentAct;

	public InteractMode interactMode;

	public bool floatMode;

	public bool dirty;

	public bool isList;

	public Transaction lastTransaction;

	public Image imageHighlight;

	public Image imageHighlightGrid;

	public UIButton moldSmallTab;

	public ButtonGrid moldButtonGrid;

	public ButtonGrid moldButtonGridless;

	public UIButton buttonTakeAll;

	public UIMagicChest uiMagic;

	private int destNum;

	private int lastNum;

	private bool firstMouseRightDown;

	public List<Tab> tabs = new List<Tab>();

	public bool wasDirty;

	public InvOwner owner => (currentTab ?? tabs[0]).owner;

	public Card dest => currentTab.dest;

	public bool UseBG
	{
		get
		{
			if (!window.saveData.useBG)
			{
				return !UseGrid;
			}
			return true;
		}
	}

	public bool UseGrid => EMono.core.config.game.useGrid;

	public bool IsMagicChest => owner.Container.trait is TraitMagicChest;

	public bool IsToolbelt => owner.Container.trait is TraitToolBelt;

	public bool IsShop => owner.Container.trait is TraitChestMerchant;

	public CoreRef.InventoryStyle InvStyle => EMono.core.refs.invStyle[(UseBG || IsToolbelt) ? owner.Container.trait.IDInvStyle : "transparent"];

	public bool IsMainMode
	{
		get
		{
			switch (currentTab.mode)
			{
			case Mode.Gear:
			case Mode.Food:
			case Mode.Item:
			case Mode.Resource:
			case Mode.Read:
			case Mode.Hold:
			case Mode.Tool:
			case Mode.All:
			case Mode.Drink:
			case Mode.HoldFurniture:
			case Mode.HoldBlock:
				return true;
			default:
				return false;
			}
		}
	}

	public CurrencyType CurrencyType => currentTab.owner.currency;

	public string IDCurrency => CurrencyType switch
	{
		CurrencyType.Medal => "medal", 
		CurrencyType.Ecopo => "ecopo", 
		_ => "money", 
	};

	public static void RefreshAllList()
	{
		UIInventory[] componentsInChildren = EMono.ui.rectLayers.GetComponentsInChildren<UIInventory>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].list.Redraw();
		}
	}

	public void SetHeader(string s)
	{
		window.SetCaption(s.lang());
	}

	public Tab AddTab(Card c, Mode mode, Thing container = null)
	{
		return AddTab(new InvOwner(c, container), mode);
	}

	public Tab AddTab(InvOwner owner, Mode mode = Mode.All)
	{
		owner.Init();
		Tab tab = new Tab
		{
			mode = mode,
			owner = owner
		};
		tabs.Add(tab);
		return tab;
	}

	public void OnInit()
	{
		interactMode = EMono.player.pref.interactMode;
		foreach (Tab t in tabs)
		{
			string text = t.textTab.IsEmpty("inv" + t.mode);
			if (!floatMode)
			{
				text = text.lang() + " - " + (t.owner.owner.IsPC ? "you".lang().ToTitleCase() : (t.owner.Chara?.Name ?? t.owner.Container.Name));
			}
			Window.Setting.Tab tab = window.AddTab(text, content, delegate
			{
				SwitchTab(t);
			}, EMono.core.refs.icons.invTab.TryGetValue(t.mode), text);
			if (t.smallTab)
			{
				tab.customTab = moldSmallTab;
			}
		}
		if ((bool)layer.mini)
		{
			layer.mini.SetActive(owner.Container.isChara);
			if (owner.Container.isChara)
			{
				layer.mini.SetChara(owner.Container.Chara);
			}
		}
		if ((bool)uiMagic)
		{
			uiMagic.SetActive(IsMagicChest);
			if (IsMagicChest)
			{
				uiMagic.Init();
			}
		}
		if (IsMagicChest)
		{
			TooltipManager.Instance.disableTimer = 0.1f;
		}
	}

	public void RefreshWindow()
	{
		if (window.saveData == null)
		{
			window.saveData = Window.dictData.TryGetValue(window.idWindow) ?? new Window.SaveData
			{
				useBG = EMono.core.config.game.showInvBG
			};
		}
		window.setting.allowMove = !window.saveData.fixedPos;
		window.bgCollider.raycastTarget = !window.saveData.fixedPos || window.saveData.useBG;
		window.imageBG.SetActive(UseBG);
		CoreRef.InventoryStyle inventoryStyle = EMono.core.refs.invStyle[owner.Container.trait.IDInvStyle];
		if (UseBG)
		{
			window.imageBG.sprite = inventoryStyle.bg;
			window.imageBG.rectTransform.anchoredPosition = inventoryStyle.posFix;
			window.imageBG.rectTransform.sizeDelta = inventoryStyle.sizeDelta;
			window.imageBG.alphaHitTestMinimumThreshold = 1f;
		}
		if (IsToolbelt)
		{
			window.cgFloatMenu.SetActive(enable: false);
			window.cgFloatMenu = null;
			window.bgCollider.rectTransform.anchoredPosition = new Vector2(-2.5f, 0f);
			window.bgCollider.rectTransform.sizeDelta = new Vector2(-20f, -20f);
		}
		if (EMono.core.config.ui.showFloatButtons && (bool)window.cgFloatMenu)
		{
			window.cgFloatMenu.alpha = 1f;
			window.cgFloatMenu.enabled = false;
			window.cgFloatMenu.SetActive(enable: true);
		}
	}

	public void Close()
	{
		if (!floatMode)
		{
			layer.Close();
		}
	}

	public void SwitchTab(Tab tab)
	{
		if (currentTab == tab)
		{
			return;
		}
		currentTab = tab;
		RefreshWindow();
		destNum = (lastNum = (DestNumIsMax() ? (-1) : 0));
		if (isList)
		{
			RefreshList();
		}
		else
		{
			RefreshGrid();
		}
		if (IsShop || window.saveData.alwaysSort)
		{
			Sort();
		}
		if ((bool)layer.uiCurrency)
		{
			tab.owner.BuildUICurrency(layer.uiCurrency, tab.owner.owner.trait.CostRerollShop != 0);
		}
		layer.TryShowHint("h_inv" + tab.mode);
		if ((bool)headerRow)
		{
			UIHeader[] componentsInChildren = headerRow.GetComponentsInChildren<UIHeader>(includeInactive: true);
			componentsInChildren[0].SetText("headerItem");
			switch (tab.mode)
			{
			case Mode.Buy:
			case Mode.Sell:
			case Mode.Identify:
				componentsInChildren[1].SetText("headerPrice".lang(IDCurrency.lang()));
				break;
			default:
				componentsInChildren[1].SetText("headerWeight");
				break;
			case Mode.Recycle:
				componentsInChildren[1].SetText("headerRecycle");
				break;
			}
		}
	}

	public void DoAct(Act act)
	{
		if (!EMono.pc.HasNoGoal || (act.LocalAct && EMono._zone.IsRegion))
		{
			SE.Beep();
			return;
		}
		EMono.player.hotItemToRestore = EMono.player.currentHotItem;
		if (act.IsAct)
		{
			act.Perform(EMono.pc);
			return;
		}
		currentAct = act as AIAct;
		EMono.pc.SetAI(currentAct);
		ActionMode.Adv.SetTurbo();
		if (!floatMode)
		{
			Close();
		}
	}

	private void Update()
	{
		if (owner.Container.isDestroyed)
		{
			Close();
			return;
		}
		imageHighlightGrid.SetActive(LayerInventory.highlightInv == owner);
		CheckDirty();
		if (EInput.action == EAction.GetAll && (bool)buttonTakeAll && !IsMagicChest)
		{
			buttonTakeAll.onClick.Invoke();
		}
		if ((IsMagicChest || window.saveData.alwaysSort) && wasDirty && (Input.GetMouseButtonUp(1) || Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift)))
		{
			list.Redraw();
			if (!Input.GetMouseButton(1) && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
			{
				wasDirty = false;
				UIButton.TryShowTip();
			}
		}
		if (Input.GetMouseButtonDown(1))
		{
			firstMouseRightDown = true;
		}
	}

	public void CheckDirty()
	{
		if (dirty)
		{
			wasDirty = true;
			if (UseGrid)
			{
				list.Redraw();
			}
			else
			{
				list.List();
			}
			UIButton.TryShowTip();
		}
	}

	public void RefreshDestNum()
	{
		if (destNum <= 0)
		{
			destNum = ((!DestNumIsMax()) ? (EInput.isShiftDown ? (-1) : 0) : ((!EInput.isShiftDown) ? (-1) : 0));
			if (destNum != lastNum)
			{
				lastNum = destNum;
				list.Redraw();
			}
		}
	}

	public bool DestNumIsMax()
	{
		Mode mode = currentTab.mode;
		if ((uint)(mode - 2) <= 1u)
		{
			return false;
		}
		return true;
	}

	public void RefreshMenu()
	{
		UIButton b2 = window.buttonSort;
		Window.SaveData data = window.saveData;
		if ((bool)b2)
		{
			b2.onClick.RemoveAllListeners();
			b2.onClick.AddListener(delegate
			{
				UIContextMenu uIContextMenu = EMono.ui.CreateContextMenuInteraction();
				uIContextMenu.layoutGroup.childAlignment = TextAnchor.UpperLeft;
				uIContextMenu.alwaysPopLeft = true;
				UIContextMenu uIContextMenu2 = uIContextMenu.AddChild("sort", TextAnchor.UpperRight);
				UIList.SortMode[] sorts = list.sorts;
				foreach (UIList.SortMode sortMode in sorts)
				{
					UIList.SortMode _sort = sortMode;
					uIContextMenu2.AddButton((((IsShop ? EMono.player.pref.sortInvShop : EMono.player.pref.sortInv) == _sort) ? "context_checker".lang() : "") + _sort.ToString().lang(), delegate
					{
						if (IsShop)
						{
							EMono.player.pref.sortInvShop = _sort;
						}
						else
						{
							EMono.player.pref.sortInv = _sort;
						}
						Sort();
						SE.Click();
					});
				}
				uIContextMenu2.AddToggle("sort_ascending", IsShop ? EMono.player.pref.sort_ascending_shop : EMono.player.pref.sort_ascending, delegate(bool a)
				{
					if (IsShop)
					{
						EMono.player.pref.sort_ascending_shop = a;
					}
					else
					{
						EMono.player.pref.sort_ascending = a;
					}
					Sort();
					SE.Click();
				});
				if (!IsMagicChest)
				{
					uIContextMenu2.AddToggle("sort_always", data.alwaysSort, delegate(bool a)
					{
						data.alwaysSort = a;
						if (data.alwaysSort)
						{
							Sort();
						}
						SE.Click();
					});
				}
				if (IsMagicChest)
				{
					UIContextMenu uIContextMenu3 = uIContextMenu.AddChild("catFilterType", TextAnchor.UpperRight);
					foreach (Window.SaveData.CategoryType item in Util.EnumToList<Window.SaveData.CategoryType>())
					{
						Window.SaveData.CategoryType _c2 = item;
						uIContextMenu3.AddButton(((data.category == item) ? "context_checker".lang() : "") + ("catFilterType_" + _c2).lang(), delegate
						{
							data.category = _c2;
							uiMagic.idCat = "";
							list.Redraw();
							SE.Click();
						});
					}
				}
				Card con2 = owner.Container;
				bool flag4 = (!con2.isNPCProperty && !con2.isChara && (con2.trait is TraitShippingChest || (con2.GetRoot() is Zone && EMono._zone.IsPCFaction) || con2.GetRootCard() == EMono.pc)) || EMono._zone is Zone_Tent;
				if (con2.IsPC)
				{
					flag4 = true;
				}
				if (con2.trait is TraitChestMerchant)
				{
					flag4 = false;
				}
				UIContextMenu dis2;
				Transform dist2;
				Transform distAdv2;
				if (flag4)
				{
					dis2 = uIContextMenu.AddChild("distribution", TextAnchor.UpperRight);
					dis2.AddSlider("priority_hint", (float a) => a.ToString() ?? "", data.priority, delegate(float a)
					{
						data.priority = (int)a;
					}, -5f, 20f, isInt: true, hideOther: false);
					dist2 = ShowDistribution(dis2, data);
					distAdv2 = ShowAdvDistribution(dis2, data);
					RefreshDist();
					if (con2.trait.IsFridge || EMono.core.config.game.advancedMenu)
					{
						dis2.AddToggle("onlyRottable", data.onlyRottable, delegate(bool a)
						{
							data.onlyRottable = a;
							SE.ClickOk();
						});
					}
					dis2.AddToggle("noRotten", data.noRotten, delegate(bool a)
					{
						data.noRotten = a;
						SE.ClickOk();
					});
					dis2.AddToggle("advDistribution", data.advDistribution, delegate(bool a)
					{
						data.advDistribution = a;
						RefreshDist();
						SE.ClickOk();
					});
					if (EMono.core.config.game.advancedMenu)
					{
						dis2.AddButton(data.filter.IsEmpty() ? "distFilter" : "distFilter2".lang(data.filter), delegate
						{
							Dialog.InputName("distFilter3", data.filter.IsEmpty(""), delegate(bool cancel, string s)
							{
								if (!cancel)
								{
									data._filterStrs = null;
									data.filter = s;
								}
							}, Dialog.InputType.DistributionFilter);
						});
					}
				}
				Window.SaveData data2;
				UIContextMenu dis;
				Transform dist;
				Transform distAdv;
				if (con2.IsPC && EMono.core.config.game.advancedMenu)
				{
					data2 = EMono.player.dataPick;
					dis = uIContextMenu.AddChild("autopick", TextAnchor.UpperRight);
					dist = ShowDistribution(dis, data2);
					distAdv = ShowAdvDistribution(dis, data2);
					RefreshDist();
					dis.AddToggle("noRotten", data2.noRotten, delegate(bool a)
					{
						data2.noRotten = a;
						SE.ClickOk();
					});
					dis.AddToggle("advDistribution", data2.advDistribution, delegate(bool a)
					{
						data2.advDistribution = a;
						RefreshDist();
						SE.ClickOk();
					});
					if (EMono.core.config.game.advancedMenu)
					{
						dis.AddButton(data2.filter.IsEmpty() ? "distFilter" : "distFilter2".lang(data2.filter), delegate
						{
							Dialog.InputName("distFilter3", data2.filter.IsEmpty(""), delegate(bool cancel, string s)
							{
								if (!cancel)
								{
									data2._filterStrs = null;
									data2.filter = s;
								}
							}, Dialog.InputType.DistributionFilter);
						});
					}
				}
				if (con2.trait is TraitShippingChest || (con2.IsInstalled && (EMono._zone.IsPCFaction || EMono._zone is Zone_Tent)))
				{
					UIContextMenu uIContextMenu4 = uIContextMenu.AddChild("autodump", TextAnchor.UpperRight);
					foreach (AutodumpFlag item2 in new List<AutodumpFlag>
					{
						AutodumpFlag.existing,
						AutodumpFlag.sameCategory,
						AutodumpFlag.distribution,
						AutodumpFlag.none
					})
					{
						string text2 = ((data.autodump == item2) ? "context_checker".lang() : "");
						AutodumpFlag _e = item2;
						UIButton uIButton = uIContextMenu4.AddButton(text2 + ("dump_" + item2).lang(), delegate
						{
							SE.Click();
							data.autodump = _e;
						});
						if (item2 != AutodumpFlag.none)
						{
							uIButton.SetTooltipLang("dump_" + item2.ToString() + "_tip");
						}
					}
				}
				if (con2.IsPC || (con2.isThing && !(con2.trait is TraitChestMerchant) && !con2.isNPCProperty))
				{
					UIContextMenu uIContextMenu5 = uIContextMenu.AddChild("config", TextAnchor.UpperRight);
					uIContextMenu5.AddToggle("toggleExcludeCraft", data.excludeCraft, delegate(bool a)
					{
						data.excludeCraft = a;
						SE.ClickOk();
					});
					if (con2.GetRootCard() == EMono.pc)
					{
						uIContextMenu5.AddToggle("toggleDump", data.excludeDump, delegate(bool a)
						{
							data.excludeDump = a;
							SE.ClickOk();
						});
					}
					if (EMono.core.config.game.advancedMenu)
					{
						if (!con2.IsPC)
						{
							uIContextMenu5.AddToggle("noRightClickClose", data.noRightClickClose, delegate(bool a)
							{
								data.noRightClickClose = a;
								SE.ClickOk();
							});
						}
						uIContextMenu5.AddToggle("fixedPos", data.fixedPos, delegate(bool a)
						{
							data.fixedPos = a;
							RefreshWindow();
							SE.ClickOk();
						});
						uIContextMenu5.AddToggle("toggleItemCompress", data.compress, delegate(bool a)
						{
							data.compress = a;
							SE.ClickOk();
						});
					}
					if (con2.IsPC)
					{
						uIContextMenu5.AddToggle("placeContainerCenter", EMono.player.openContainerCenter, delegate(bool a)
						{
							EMono.player.openContainerCenter = a;
							SE.ClickOk();
						});
					}
					if (!con2.isChara && !con2.trait.IsSpecialContainer)
					{
						uIContextMenu5.AddButton("changeName", delegate
						{
							Dialog.InputName("dialogChangeName", con2.c_altName.IsEmpty(""), delegate(bool cancel, string text)
							{
								if (!cancel)
								{
									con2.c_altName = text;
								}
							}, Dialog.InputType.Item);
						});
					}
					if (EMono.core.config.game.advancedMenu)
					{
						uIContextMenu5.AddButton("copyContainer", delegate
						{
							SE.ClickOk();
							EMono.player.windowDataCopy = IO.DeepCopy(data);
							EMono.player.windowDataName = con2.Name;
						});
						if (EMono.player.windowDataCopy != null && !con2.IsPC)
						{
							uIContextMenu5.AddButton("pasteContainer".lang(EMono.player.windowDataName), delegate
							{
								SE.ClickOk();
								window.saveData.CopyFrom(EMono.player.windowDataCopy);
								RefreshWindow();
								RefreshGrid();
							});
						}
					}
				}
				UIContextMenu uIContextMenu6 = uIContextMenu.AddChild("appearanceWindow", TextAnchor.UpperRight);
				uIContextMenu6.AddToggle("toggleBG", data.useBG, delegate(bool a)
				{
					data.useBG = a;
					data.color = InvStyle.gridColor;
					RefreshWindow();
					RefreshGrid();
					SE.ClickOk();
				});
				uIContextMenu6.AddSlider("size", (float a) => a.ToString() ?? "", data.size, delegate(float b)
				{
					data.size = (int)b;
					RefreshGrid();
				}, -25f, 25f, isInt: true, hideOther: false);
				if (EMono.core.config.game.advancedMenu && !IsMagicChest)
				{
					uIContextMenu6.AddSlider("columns", (float a) => a.ToString() ?? "", data.columns, delegate(float b)
					{
						data.columns = (int)b;
						RefreshGrid();
					}, 0f, 20f, isInt: true, hideOther: false);
				}
				uIContextMenu6.AddButton("colorGrid", delegate
				{
					EMono.ui.AddLayer<LayerColorPicker>().SetColor(data.color, InvStyle.gridColor, delegate(PickerState state, Color _c)
					{
						data.color = _c;
						list.bgGrid.color = _c;
						if (data.color.a == 0)
						{
							list.bgGrid.color = InvStyle.gridColor;
						}
					});
				});
				if (!con2.isChara)
				{
					uIContextMenu6.AddButton("changeIcon", delegate
					{
						EMono.ui.contextMenu.currentMenu.Hide();
						UIContextMenu uIContextMenu9 = EMono.ui.CreateContextMenuInteraction();
						GridLayoutGroup parent = uIContextMenu9.AddGridLayout();
						int num = 0;
						foreach (Sprite item3 in EMono.core.refs.spritesContainerIcon)
						{
							_ = item3;
							UIButton uIButton2 = Util.Instantiate<UIButton>("UI/Element/Button/ButtonContainerIcon", parent);
							int _i = num;
							uIButton2.icon.sprite = EMono.core.refs.spritesContainerIcon[_i];
							uIButton2.SetOnClick(delegate
							{
								SE.Click();
								owner.Container.c_indexContainerIcon = _i;
								LayerInventory.SetDirty(owner.Container.Thing);
								EMono.ui.contextMenu.currentMenu.Hide();
							});
							num++;
						}
						uIContextMenu9.Show();
					});
				}
				if (EMono.core.config.game.advancedMenu)
				{
					UIContextMenu uIContextMenu7 = uIContextMenu.AddChild("anchor");
					foreach (RectPosition p in Util.EnumToList<RectPosition>())
					{
						uIContextMenu7.AddButton(((data.customAnchor == p) ? "★ " : "") + p.ToString().lang(), delegate
						{
							data.customAnchor = p;
							window.UpdateSaveData();
							SE.ClickGeneral();
						});
					}
				}
				if (EMono.debug.enable)
				{
					UIContextMenu uIContextMenu8 = uIContextMenu.AddChild("debug", TextAnchor.UpperRight);
					uIContextMenu8.AddToggle("toggleGrid", EMono.core.config.game.useGrid, delegate(bool a)
					{
						EMono.core.config.game.useGrid = a;
						foreach (LayerInventory item4 in LayerInventory.listInv)
						{
							item4.invs[0].RefreshWindow();
							item4.invs[0].RefreshGrid();
						}
					});
					uIContextMenu8.AddSlider("iconSize", (float a) => a.ToString() ?? "", EMono.game.config.gridIconSize, delegate(float b)
					{
						EMono.game.config.gridIconSize = (int)b;
						RefreshGrid();
					}, 100f, 150f, isInt: true, hideOther: false);
				}
				uIContextMenu.Show();
				uIContextMenu.hideOnMouseLeave = false;
				void RefreshDist()
				{
					dist2.SetActive(!data.advDistribution);
					distAdv2.SetActive(data.advDistribution);
					dis2.layoutGroup.RebuildLayout();
				}
				void RefreshDist()
				{
					dist.SetActive(!data2.advDistribution);
					distAdv.SetActive(data2.advDistribution);
					dis.layoutGroup.RebuildLayout();
				}
			});
		}
		b2 = window.buttonQuickSort;
		if ((bool)b2)
		{
			b2.onClick.RemoveAllListeners();
			b2.onClick.AddListener(delegate
			{
				Sort();
				SE.Click();
			});
		}
		b2 = window.buttonExtra;
		if ((bool)b2)
		{
			b2.SetActive(owner.Container.IsPC);
			b2.onClick.RemoveAllListeners();
			b2.onClick.AddListener(delegate
			{
				TaskDump.TryPerform();
			});
		}
		b2 = window.buttonShared;
		if ((bool)b2)
		{
			Card con = owner.Container;
			bool flag = !con.isChara && ((con.IsInstalled && EMono._zone.IsPCFaction) || owner.owner.IsPC);
			b2.SetActive(flag);
			if (flag)
			{
				RefreshShareButton();
				b2.SetOnClick(delegate
				{
					bool flag3 = data.sharedType == ContainerSharedType.Shared;
					SE.ClickOk();
					Msg.Say("changePermission", con, (flag3 ? "stPersonal" : "stShared").lang());
					data.sharedType = ((!flag3) ? ContainerSharedType.Shared : ContainerSharedType.Personal);
					RefreshShareButton();
				});
			}
		}
		Mode mode = currentTab.mode;
		WindowMenu menuBottom = window.menuBottom;
		menuBottom.Clear();
		switch (mode)
		{
		case Mode.Take:
			if (owner.Container.isNPCProperty || owner.Container.isChara || IsMagicChest)
			{
				break;
			}
			buttonTakeAll = menuBottom.AddButton("takeAll".lang(EInput.keys.getAll.key.ToString().ToLower()), delegate
			{
				owner.Container.things.ForeachReverse(delegate(Thing t)
				{
					if (!EMono.pc.things.IsFull(t))
					{
						EMono.pc.Pick(t, msg: false);
					}
				});
				if (owner.Container.things.Count > 0)
				{
					SE.Beep();
				}
				else
				{
					SE.Play("pick_thing");
					Close();
				}
			});
			break;
		case Mode.Buy:
		{
			Card _owner = owner.owner;
			int cost = _owner.trait.CostRerollShop;
			if (cost <= 0)
			{
				break;
			}
			menuBottom.AddButton("rerollShop".lang(cost.ToString() ?? ""), delegate
			{
				if (EMono._zone.influence < cost)
				{
					SE.Beep();
					Msg.Say("notEnoughInfluence");
				}
				else
				{
					SE.Dice();
					EMono._zone.influence -= cost;
					_owner.c_dateStockExpire = 0;
					_owner.trait.OnBarter();
					RefreshGrid();
					Sort();
					SE.Play("shop_open");
				}
			});
			break;
		}
		}
		void RefreshShareButton()
		{
			bool flag2 = data.sharedType == ContainerSharedType.Shared;
			b2.image.sprite = (flag2 ? EMono.core.refs.icons.shared : EMono.core.refs.icons.personal);
			b2.tooltip.lang = (flag2 ? "hintShared" : "hintPrivate");
			b2.ShowTooltipForced();
		}
	}

	public Transform ShowDistribution(UIContextMenu dis, Window.SaveData data)
	{
		List<UIContextMenuItem> items = new List<UIContextMenuItem>();
		UIContextMenuItem itemAll = null;
		List<ContainerFlag> obj = Util.EnumToList<ContainerFlag>();
		obj.Remove(ContainerFlag.none);
		GridLayoutGroup gridLayoutGroup = dis.AddGridLayout("Context_LayoutDistribution");
		itemAll = dis.AddToggle("all", isOn: false, delegate(bool a)
		{
			foreach (UIContextMenuItem item in items)
			{
				item.toggle.isOn = a;
			}
			Refresh();
			SE.ClickOk();
		});
		foreach (ContainerFlag f2 in obj)
		{
			SourceCategory.Row row = EMono.sources.categories.map[f2.ToString()];
			items.Add(dis.AddToggle(row.GetName(), !data.flag.HasFlag(f2), delegate(bool a)
			{
				SetOn(f2, !a);
				Refresh();
				SE.ClickOk();
			}));
		}
		itemAll.transform.SetParent(gridLayoutGroup.transform);
		itemAll.GetComponentInChildren<UIText>().SetText("all".lang(), FontColor.Topic);
		foreach (UIContextMenuItem item2 in items)
		{
			item2.transform.SetParent(gridLayoutGroup.transform);
		}
		Refresh();
		return gridLayoutGroup.transform;
		void Refresh()
		{
			bool isOnWithoutNotify = true;
			foreach (UIContextMenuItem item3 in items)
			{
				if (!item3.toggle.isOn)
				{
					isOnWithoutNotify = false;
				}
			}
			itemAll.toggle.SetIsOnWithoutNotify(isOnWithoutNotify);
		}
		void SetOn(ContainerFlag f, bool on)
		{
			if (!on && data.flag.HasFlag(f))
			{
				data.flag &= ~f;
			}
			else if (on && !data.flag.HasFlag(f))
			{
				data.flag |= f;
			}
		}
	}

	public Transform ShowAdvDistribution(UIContextMenu dis, Window.SaveData data)
	{
		UIDistribution uIDistribution = Util.Instantiate<UIDistribution>("UI/Layer/UIDistribution");
		uIDistribution.SetContainer(owner.Container, data);
		return dis.AddGameObject(uIDistribution).transform;
	}

	public void Sort(bool redraw = true)
	{
		UIList.SortMode i = (IsShop ? EMono.player.pref.sortInvShop : EMono.player.pref.sortInv);
		bool flag = true;
		while (flag)
		{
			flag = false;
			foreach (Thing thing in owner.Container.things)
			{
				if (thing.invY == 1)
				{
					continue;
				}
				foreach (Thing thing2 in owner.Container.things)
				{
					if (thing != thing2 && thing2.invY != 1 && thing.TryStackTo(thing2))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					break;
				}
			}
		}
		int num = 0;
		foreach (Thing thing3 in owner.Container.things)
		{
			if (thing3.invY != 1)
			{
				thing3.invY = 0;
				thing3.invX = -1;
			}
			thing3.SetSortVal(i, owner.currency);
			num++;
		}
		owner.Container.things.Sort(delegate(Thing a, Thing b)
		{
			bool flag2 = (IsShop ? EMono.player.pref.sort_ascending_shop : EMono.player.pref.sort_ascending);
			if (i == UIList.SortMode.ByName)
			{
				if (flag2)
				{
					return string.Compare(a.GetName(NameStyle.FullNoArticle, 1), b.GetName(NameStyle.FullNoArticle, 1));
				}
				return string.Compare(b.GetName(NameStyle.FullNoArticle, 1), a.GetName(NameStyle.FullNoArticle, 1));
			}
			if (a.sortVal == b.sortVal)
			{
				return b.SecondaryCompare(i, a);
			}
			return (!flag2) ? (a.sortVal - b.sortVal) : (b.sortVal - a.sortVal);
		});
		if (!UseGrid)
		{
			int num2 = 0;
			int num3 = 0;
			Vector2 sizeDelta = list.Rect().sizeDelta;
			sizeDelta.x -= 60f;
			sizeDelta.y -= 60f;
			foreach (Thing thing4 in owner.Container.things)
			{
				if (thing4.invY != 0)
				{
					continue;
				}
				thing4.posInvX = num2 + 30;
				thing4.posInvY = (int)sizeDelta.y - num3 + 30;
				num2 += 40;
				if ((float)num2 > sizeDelta.x)
				{
					num2 = 0;
					num3 += 40;
					if ((float)num3 > sizeDelta.y)
					{
						num3 = 20;
					}
				}
			}
		}
		if (redraw)
		{
			list.Redraw();
		}
	}

	public void RefreshList()
	{
		Mode mode = currentTab.mode;
		_ = currentTab.subMode;
		RefreshMenu();
		list.Clear();
		UIList.Callback<Thing, ButtonGrid> callbacks = new UIList.Callback<Thing, ButtonGrid>
		{
			onClick = delegate(Thing a, ButtonGrid b)
			{
				if (destNum != 0)
				{
					if (destNum != -1)
					{
						_ = destNum;
					}
					else
					{
						_ = a.Num;
					}
				}
				if (actionClick != null)
				{
					actionClick(a);
					Close();
				}
				else
				{
					EMono.pc.HoldCard(a);
					SE.SelectHotitem();
				}
			},
			onInstantiate = delegate(Thing a, ButtonGrid b)
			{
				dirty = false;
				_ = a.Name;
				bool flag = mode == Mode.Buy || mode == Mode.Sell || mode == Mode.Identify;
				int num2 = ((destNum != -1) ? 1 : a.Num);
				int cost = (flag ? (a.GetPrice(CurrencyType, mode == Mode.Sell) * num2) : 0);
				bool canPay = cost < EMono.pc.GetCurrency(IDCurrency);
				if (mode == Mode.Identify)
				{
					cost = 100;
				}
				b.SetCard(a, ButtonGrid.Mode.Default, delegate(UINote n)
				{
					Mode mode2 = mode;
					if ((uint)(mode2 - 2) <= 1u)
					{
						n.Space(8);
						UIItem uIItem = n.AddExtra<UIItem>("costPrice");
						uIItem.text1.SetText(Lang._currency(cost), canPay ? FontColor.Good : FontColor.Bad);
						uIItem.image1.sprite = SpriteSheet.Get("icon_" + IDCurrency);
					}
				});
				if (flag && (bool)b.subText)
				{
					b.subText.SetText((cost.ToString() ?? "").TagColorGoodBad(() => mode == Mode.Sell || cost <= EMono.pc.GetCurrency(IDCurrency)));
				}
			},
			onList = delegate
			{
				List<Thing> list = new List<Thing>();
				foreach (Thing thing in owner.Things)
				{
					if (thing != null && (funcList == null || funcList(thing)) && !thing.HasTag(CTAG.noDrop) && (thing.c_equippedSlot == 0 || mode == Mode.Equip || mode == Mode.Identify) && (!(thing.category.id == "currency") || (mode != Mode.Buy && mode != Mode.Sell)))
					{
						list.Add(thing);
					}
				}
				int num = 0;
				_ = this.list;
				foreach (Thing item in list)
				{
					num += item.ChildrenAndSelfWeight;
					this.list.Add(item);
				}
				string text = "";
				text = "statsInv".lang(owner.Container.things.Count.ToString() ?? "", this.list.ItemCount.ToString() ?? "", Lang._weight(num));
				layer.windows[0].textStats.SetText(text);
			},
			onSort = null
		};
		list.callbacks = callbacks;
		list.List();
	}

	public void RefreshHighlight()
	{
		if (!imageHighlight)
		{
			return;
		}
		imageHighlight.SetActive(enable: false);
		if (EMono.game.altInv)
		{
			Thing thing = EMono.player.currentHotItem.Thing;
			if (currentTab.mode == Mode.All && thing != null && !thing.isDestroyed)
			{
				_ = thing.parent;
				_ = owner.Container;
			}
		}
	}

	public void RefreshGrid()
	{
		_ = currentTab.mode;
		_ = currentTab.subMode;
		CoreRef.InventoryStyle style = InvStyle;
		RefreshMenu();
		GridLayoutGroup g = list.GetComponent<GridLayoutGroup>();
		ContentSizeFitter component = list.GetComponent<ContentSizeFitter>();
		if (UseGrid)
		{
			g.constraintCount = ((window.saveData.columns == 0) ? owner.Container.things.width : window.saveData.columns);
			Vector2 cellSize = style.gridSize * (100 + window.saveData.size) / 100f;
			g.cellSize = cellSize;
			GridLayoutGroup gridLayoutGroup = g;
			bool flag2 = (component.enabled = true);
			gridLayoutGroup.enabled = flag2;
		}
		else
		{
			GridLayoutGroup gridLayoutGroup2 = g;
			bool flag2 = (component.enabled = false);
			gridLayoutGroup2.enabled = flag2;
			list.Rect().sizeDelta = style.sizeContainer;
		}
		list.Clear();
		int maxCapacity = (IsMagicChest ? owner.Container.things.MaxCapacity : 0);
		UIList.Callback<Thing, ButtonGrid> callback = new UIList.Callback<Thing, ButtonGrid>
		{
			onRedraw = delegate(Thing a, ButtonGrid b, int i)
			{
				dirty = false;
				b.index = i;
				bool flag4 = i >= owner.Things.Count;
				if (IsMagicChest)
				{
					flag4 = uiMagic.page * uiMagic.GridSize + i >= maxCapacity;
				}
				if (UseGrid)
				{
					b.SetCardGrid(a, flag4 ? null : owner);
				}
				else
				{
					b.SetCardGrid(a, owner);
					if (a != null)
					{
						b.Rect().anchoredPosition = new Vector2(a.posInvX, a.posInvY);
					}
					float num3 = 0.01f * (float)EMono.game.config.gridIconSize;
					b.icon.Rect().localScale = new Vector3(num3, num3, 1f);
					b.Rect().sizeDelta = b.icon.rectTransform.sizeDelta;
				}
				if (flag4)
				{
					b.interactable = false;
					b.image.sprite = null;
					b.image.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 120);
					b.image.raycastTarget = true;
				}
				else
				{
					b.image.raycastTarget = true;
				}
			},
			onList = delegate
			{
				list.bgType = (UseGrid ? UIList.BGType.grid : UIList.BGType.none);
				if ((bool)list.bgGrid)
				{
					list.bgGrid.SetActive(UseGrid);
					list.bgGrid.color = style.gridColor;
					if (window.saveData.color.a != 0)
					{
						list.bgGrid.color = window.saveData.color;
					}
				}
				if (IsMagicChest)
				{
					owner.Container.things.RefreshGrid(uiMagic, window.saveData);
				}
				else
				{
					owner.Container.things.RefreshGrid();
				}
				if (UseGrid)
				{
					if (IsMagicChest)
					{
						for (int k = 0; k < owner.Container.things.GridSize; k++)
						{
							list.Add(owner.Things[k]);
						}
					}
					else
					{
						int count = owner.Things.Count;
						int num2 = (int)Mathf.Ceil(count / g.constraintCount + ((count % g.constraintCount != 0) ? 1 : 0)) * g.constraintCount;
						for (int l = 0; l < num2; l++)
						{
							list.Add((l < count) ? owner.Things[l] : null);
						}
					}
				}
				else
				{
					Vector2 sizeDelta2 = list.Rect().sizeDelta;
					foreach (Thing thing2 in owner.Things)
					{
						if (thing2 != null)
						{
							list.Add(thing2);
							if (thing2.posInvX == 0 && thing2.posInvY == 0)
							{
								thing2.posInvX = EMono.rnd((int)sizeDelta2.x - 60) + 30;
								thing2.posInvY = EMono.rnd((int)sizeDelta2.y - 60) + 30;
							}
						}
					}
				}
				if (floatMode)
				{
					window.setting.tabs[0].idLang = "captionInvFloat".lang(owner.Container.IsPC ? "" : owner.Container.Name, ((float)owner.Container.ChildrenWeight / 1000f).ToString("F1") + "/" + ((float)owner.Container.WeightLimit / 1000f).ToString("F1"));
				}
			}
		};
		bool sorted = false;
		list.callbacks = callback;
		list.onBeforeRedraw = delegate
		{
			if (IsMagicChest)
			{
				if (firstMouseRightDown || (!Input.GetMouseButton(1) && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift)))
				{
					Sort(redraw: false);
					firstMouseRightDown = false;
					owner.Container.things.RefreshGrid(uiMagic, window.saveData);
				}
			}
			else
			{
				if (window.saveData.alwaysSort && !sorted && (firstMouseRightDown || (!Input.GetMouseButton(1) && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))))
				{
					sorted = true;
					Sort();
					sorted = false;
					firstMouseRightDown = false;
				}
				owner.Container.things.RefreshGrid();
			}
			dirty = false;
			if (UseGrid)
			{
				UIList uIList = list;
				for (int j = 0; j < uIList.buttons.Count; j++)
				{
					UIList.ButtonPair value = uIList.buttons[j];
					value.obj = ((j < owner.Container.things.grid.Count) ? owner.Container.things.grid[j] : null);
					uIList.buttons[j] = value;
				}
			}
		};
		list.onAfterRedraw = delegate
		{
			if (!layer.floatInv)
			{
				int num = 0;
				foreach (UIList.ButtonPair button in list.buttons)
				{
					if ((button.component as ButtonGrid).card is Thing thing)
					{
						num += thing.ChildrenAndSelfWeight;
					}
				}
				string text = "";
				text = "statsInv2".lang(owner.Container.things.Count.ToString() ?? "", list.ItemCount.ToString() ?? "", Lang._weight(num) + "/" + Lang._weight(owner.Container.WeightLimit));
				layer.windows[0].textStats.SetText(text);
			}
			if (owner.Container.things.IsOverflowing())
			{
				if (!transOverflow)
				{
					transOverflow = Util.Instantiate("UI/Element/Other/InvOverflow", window.transform);
				}
			}
			else if ((bool)transOverflow)
			{
				UnityEngine.Object.Destroy(transOverflow.gameObject);
			}
			LayerInventory.TryShowGuide(list);
			if ((bool)layer.mini)
			{
				layer.mini.Refresh(layer.mini.window.idTab);
			}
			if ((bool)uiMagic)
			{
				uiMagic.OnAfterRedraw();
			}
			UIButton.TryShowTip();
		};
		callback.mold = (UseGrid ? moldButtonGrid : moldButtonGridless);
		if (!UseGrid && !window.saveData.firstSorted)
		{
			Vector2 sizeDelta = list.Rect().sizeDelta;
			foreach (Thing thing3 in owner.Things)
			{
				if (thing3 != null)
				{
					thing3.posInvX = EMono.rnd((int)sizeDelta.x - 60) + 30;
					thing3.posInvY = EMono.rnd((int)sizeDelta.y - 60) + 30;
				}
			}
			window.saveData.firstSorted = true;
		}
		list.List();
		g.RebuildLayoutTo<Layer>();
		list.onAfterRedraw();
		if ((bool)list.bgGrid)
		{
			list.bgGrid.color = style.gridColor;
			if (window.saveData.color.a != 0)
			{
				list.bgGrid.color = window.saveData.color;
			}
		}
		if (owner == InvOwner.Trader && owner.UseGuide)
		{
			LayerInventory.SetDirtyAll();
		}
	}
}
