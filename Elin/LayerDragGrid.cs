using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerDragGrid : LayerBaseCraft
{
	public class PutbackInfo
	{
		public Thing thing;

		public Thing container;
	}

	public static LayerDragGrid Instance;

	public List<PutbackInfo> putBacks = new List<PutbackInfo>();

	public UIItem itemCost;

	public Image imageOwner;

	public UIText textTitle;

	public UIText textFuel;

	public Transform transIndex;

	public List<ButtonGrid> buttons;

	public ButtonGrid buttonOwner;

	public UIButton buttonRefuel;

	public UIButton buttonAutoRefuel;

	public UIButton buttonAlly;

	public UIButton buttonStock;

	public InvOwnerDraglet owner;

	public UICurrency uiCurrency;

	public int currentIndex;

	public UIDragGridInfo info;

	public UIList listAlly;

	public UIDragGridIngredients uiIngredients;

	private bool wasInventoryOpen;

	private int lastNum;

	public ButtonGrid CurrentButton => buttons[currentIndex];

	public override bool CanCancelAI => true;

	public override bool RepeatAI => true;

	public void OnOpen()
	{
		Instance = this;
		InvOwner.Trader = owner;
		wasInventoryOpen = ELayer.ui.IsInventoryOpen;
		if (!wasInventoryOpen)
		{
			ELayer.ui.OpenFloatInv(ignoreSound: true);
		}
	}

	public LayerDragGrid SetInv(InvOwnerDraglet owner, bool refuelFromLayerDragGrid = false)
	{
		owner.Init();
		owner.owner.SetImage(imageOwner);
		this.owner = owner;
		textTitle.text = owner.langTransfer.lang();
		OnOpen();
		buttonOwner.SetCardGrid(owner.owner);
		owner.Container.things.RefreshGrid();
		for (int j = 0; j < buttons.Count; j++)
		{
			buttons[j].gameObject.AddComponent<CanvasGroup>();
			if (owner.numDragGrid > j)
			{
				buttons[j].SetCardGrid(null, owner);
				buttons[j].index = j;
				owner.buttons.Add(buttons[j]);
			}
			else
			{
				buttons[j].SetActive(enable: false);
			}
		}
		uiIngredients.Refresh();
		textFuel.SetActive(owner.owner.trait.IsRequireFuel);
		owner.BuildUICurrency(uiCurrency);
		InvokeRepeating("RefreshCost", 0f, 0.2f);
		this.RebuildLayout(recursive: true);
		RefreshCurrentGrid();
		RefreshCost();
		this.RebuildLayout(recursive: true);
		buttonRefuel.SetActive(owner.ShowFuel);
		buttonAutoRefuel.SetActive(owner.ShowFuel);
		buttonStock.SetActive(owner.AllowStockIngredients);
		if (owner.CanTargetAlly && ELayer.pc.party.members.Count > 1)
		{
			listAlly.callbacks = new UIList.Callback<Chara, UIButton>
			{
				onInstantiate = delegate(Chara a, UIButton b)
				{
					a.SetImage(b.icon);
					b.SetTooltipLang(a.Name);
				},
				onClick = delegate(Chara c, UIButton i)
				{
					if (!LayerInventory.CloseAllyInv(c))
					{
						SE.PopInventory();
						LayerInventory.CreateContainerAlly(c, c);
					}
				},
				onSort = delegate(Chara a, UIList.SortMode m)
				{
					a.SetSortVal(m);
					return -a.sortVal;
				},
				onList = delegate
				{
					foreach (Chara member in ELayer.pc.party.members)
					{
						if (!member.IsPC)
						{
							listAlly.Add(member);
						}
					}
				}
			};
			listAlly.List();
		}
		else
		{
			listAlly.SetActive(enable: false);
		}
		buttonRefuel.SetOnClick(delegate
		{
			if (!ELayer.pc.HasNoGoal)
			{
				SE.BeepSmall();
			}
			else
			{
				windows[0].SetInteractable(enable: false, 0f);
				info.InitFuel(owner.owner);
				Create(new InvOwnerRefuel(owner.owner), refuelFromLayerDragGrid: true).SetOnKill(delegate
				{
					if (!isDestroyed)
					{
						windows[0].SetInteractable(enable: true);
						info.Init(owner.owner);
						OnOpen();
						RefreshCurrentGrid();
						Redraw();
						owner.OnAfterRefuel();
					}
				});
			}
		});
		buttonAutoRefuel.SetOnClick(delegate
		{
			SE.Click();
			owner.owner.autoRefuel = !owner.owner.autoRefuel;
			RefreshCost();
			if (owner.owner.trait.IsFuelEnough())
			{
				owner.OnAfterRefuel();
			}
		});
		buttonStock.SetOnClick(delegate
		{
			SE.Click();
			owner.owner.c_isDisableStockUse = !owner.owner.c_isDisableStockUse;
			uiIngredients.Refresh();
			RefreshCost();
		});
		if (owner is InvOwnerRefuel)
		{
			if (refuelFromLayerDragGrid)
			{
				info.SetActive(enable: false);
			}
			else
			{
				Debug.Log(owner.owner);
				info.InitFuel(owner.owner);
			}
		}
		else
		{
			info.Init(owner.owner);
		}
		return this;
	}

	public int GetPrice()
	{
		return owner.price;
	}

	public void RefreshCost()
	{
		buttonAutoRefuel.mainText.text = (owner.owner.autoRefuel ? "On" : "Off");
		buttonAutoRefuel.icon.SetAlpha(owner.owner.autoRefuel ? 1f : 0.4f);
		buttonStock.mainText.text = ((!owner.owner.c_isDisableStockUse) ? "On" : "Off");
		buttonStock.icon.SetAlpha((!owner.owner.c_isDisableStockUse) ? 1f : 0.4f);
		textFuel.text = (int)((float)owner.owner.c_charges / (float)owner.owner.trait.MaxFuel * 100f) + "%";
		int price = GetPrice();
		itemCost.transform.parent.SetActive(price != 0);
		if (price != 0)
		{
			itemCost.text1.SetText(Lang._currency(GetPrice()), (ELayer.pc.GetCurrency(owner.IDCurrency) >= price) ? FontColor.Good : FontColor.Bad);
			itemCost.image1.sprite = SpriteSheet.Get("icon_" + owner.IDCurrency);
		}
	}

	public bool IsAllGridSet()
	{
		for (int i = 0; i < owner.numDragGrid; i++)
		{
			if (buttons[i].card == null)
			{
				return false;
			}
		}
		return true;
	}

	public override void ClearButtons()
	{
		for (int i = 0; i < owner.numDragGrid; i++)
		{
			buttons[i].SetCardGrid(null);
		}
		RefreshCurrentGrid();
		uiIngredients.Refresh();
	}

	public override void RefreshCurrentGrid()
	{
		int num = -1;
		for (int i = 0; i < buttons.Count; i++)
		{
			bool flag = buttons[i].Card != null;
			if (num == -1 && (!flag || i == owner.numDragGrid - 1))
			{
				num = i;
			}
			buttons[i].interactable = flag || num == i;
			buttons[i].GetComponent<CanvasGroup>().alpha = (buttons[i].interactable ? 1f : 0.5f);
		}
		currentIndex = num;
		bool flag2 = num >= 0 && num < owner.numDragGrid;
		if (flag2)
		{
			transIndex.position = buttons[num].transform.position;
		}
		transIndex.SetActive(flag2);
		Redraw();
	}

	public override List<Thing> GetTargets()
	{
		List<Thing> list = new List<Thing>();
		TraitCrafter traitCrafter = owner.owner.trait as TraitCrafter;
		for (int i = 0; i < traitCrafter.numIng; i++)
		{
			list.Add(buttons[i].card?.Thing ?? null);
		}
		return list;
	}

	private void Update()
	{
		Validate();
	}

	public void Validate()
	{
		Card card = buttonOwner.card;
		if (card.Num != lastNum)
		{
			lastNum = card.Num;
			buttonOwner.SetCardGrid(owner.owner);
		}
		if (card == null || card.isDestroyed)
		{
			Close();
		}
	}

	public void Redraw()
	{
		foreach (ButtonGrid button in buttons)
		{
			if (button.gameObject.activeSelf)
			{
				button.Redraw();
			}
		}
		LayerInventory.SetDirtyAll(immediate: true);
	}

	public void AddPutBack(Thing t, Thing container)
	{
		if (container != null)
		{
			putBacks.Add(new PutbackInfo
			{
				thing = t,
				container = container
			});
		}
	}

	public void TryPutBack()
	{
		foreach (PutbackInfo putBack in putBacks)
		{
			if (!putBack.thing.isDestroyed && putBack.thing.Num != 0 && putBack.thing.parent != putBack.container && !putBack.container.isDestroyed)
			{
				putBack.container.AddCard(putBack.thing);
			}
		}
		putBacks.Clear();
		RefreshCurrentGrid();
	}

	public override void OnEndCraft()
	{
		TryPutBack();
	}

	public override void OnKill()
	{
		EInput.haltInput = false;
		LayerInventory.CloseAllyInv();
		InvOwner.Trader = null;
		if (!wasInventoryOpen && ELayer.ui.IsInventoryOpen)
		{
			ELayer.ui.ToggleInventory();
		}
		LayerInventory.SetDirtyAll();
		if (ELayer.pc.ai is AI_UseCrafter && ELayer.pc.ai.IsRunning)
		{
			ELayer.pc.ai.Cancel();
		}
	}

	public override void OnUpdateInput()
	{
		if (EInput.action == EAction.MenuInventory || Input.GetKeyDown(KeyCode.Tab))
		{
			Close();
			EInput.WaitReleaseKey();
		}
		else
		{
			base.OnUpdateInput();
		}
	}

	public override void OnRightClick()
	{
		if (InputModuleEX.GetComponentOf<ButtonGrid>() == null)
		{
			Close();
		}
	}

	public static LayerDragGrid Create(InvOwnerDraglet owner, bool refuelFromLayerDragGrid = false)
	{
		return ELayer.ui.AddLayer<LayerDragGrid>("LayerInventory/LayerDragGrid").SetInv(owner, refuelFromLayerDragGrid);
	}

	public static LayerDragGrid CreateChaosOffering(TraitAltarChaos altar)
	{
		Msg.Say("offer_what");
		return Create(new InvOwnerChaosOffering(altar.owner)
		{
			altar = altar
		});
	}

	public static LayerDragGrid CreateOffering(TraitAltar altar)
	{
		Msg.Say("offer_what");
		return Create(new InvOwnerOffering(altar.owner)
		{
			altar = altar
		});
	}

	public static LayerDragGrid CreateDeliver(InvOwnerDeliver.Mode mode = InvOwnerDeliver.Mode.Default, Card owner = null)
	{
		if (mode == InvOwnerDeliver.Mode.Tax)
		{
			Msg.Say("bills", ELayer.player.taxBills.ToString() ?? "");
		}
		Msg.Say("deliver_what");
		return Create(new InvOwnerDeliver(owner)
		{
			mode = mode
		});
	}

	public static LayerDragGrid CreateGive(Chara c)
	{
		return Create(new InvOwnerGive(c)
		{
			chara = c
		});
	}

	public static LayerDragGrid CreateCraft(TraitCrafter crafter)
	{
		return Create(new InvOwnerCraft(crafter.owner)
		{
			crafter = crafter
		});
	}

	public static LayerDragGrid CreateRecycle(TraitRecycle recycle)
	{
		Msg.Say("recycle_what");
		return Create(new InvOwnerRecycle(recycle.owner)
		{
			recycle = recycle
		});
	}

	public static LayerDragGrid CreateGacha(TraitGacha gacha)
	{
		Msg.Say("target_what");
		return Create(new InvOwnerGacha(gacha.owner)
		{
			gacha = gacha
		});
	}

	public static LayerDragGrid TryProc(Chara cc, InvOwnerEffect owner)
	{
		owner.cc = cc;
		if (cc.IsPC)
		{
			return Create(owner);
		}
		int num = ((owner.count == -1) ? 1 : owner.count);
		for (int i = 0; i < num; i++)
		{
			List<Thing> list = cc.things.List((Thing t) => owner.ShouldShowGuide(t));
			if (list.Count > 0)
			{
				Thing t2 = list.RandomItem();
				owner._OnProcess(t2);
				continue;
			}
			if (i == 0)
			{
				cc.SayNothingHappans();
			}
			break;
		}
		return null;
	}

	public static LayerDragGrid CreateIdentify(Chara cc, bool superior = false, BlessedState state = BlessedState.Normal, int price = 0, int count = 1)
	{
		return TryProc(cc, new InvOwnerIdentify
		{
			state = state,
			price = price,
			count = count,
			superior = superior
		});
	}

	public static LayerDragGrid CreateEnchant(Chara cc, bool armor, bool superior = false, BlessedState state = BlessedState.Normal, int count = 1)
	{
		return TryProc(cc, new InvOwnerEnchant
		{
			armor = armor,
			state = state,
			superior = superior,
			count = count
		});
	}

	public static LayerDragGrid CreateChangeMaterial(Chara cc, Thing consume, SourceMaterial.Row mat, EffectId idEffect, BlessedState state = BlessedState.Normal, int price = 0, int count = 1)
	{
		return TryProc(cc, new InvOwnerChangeMaterial
		{
			consume = consume,
			mat = mat,
			state = state,
			price = price,
			count = count,
			idEffect = idEffect
		});
	}

	public static LayerDragGrid CreateUncurse(Chara cc, BlessedState state = BlessedState.Normal, int price = 0, int count = 1)
	{
		return TryProc(cc, new InvOwnerUncurse
		{
			state = state,
			price = price,
			count = count
		});
	}

	public static LayerDragGrid CreateLighten(Chara cc, BlessedState state = BlessedState.Normal, int price = 0, int count = 1)
	{
		return TryProc(cc, new InvOwnerLighten
		{
			state = state,
			price = price,
			count = count
		});
	}

	public static LayerDragGrid CreateReconstruction(Chara cc, BlessedState state = BlessedState.Normal, int price = 0, int count = 1)
	{
		return TryProc(cc, new InvOwnerReconstruction
		{
			state = state,
			price = price,
			count = count
		});
	}
}
