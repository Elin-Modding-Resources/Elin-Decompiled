using System;
using System.Collections.Generic;
using UnityEngine;

public class InvOwner : EClass
{
	public class ErrorMessage
	{
		public string sound = "beep_small";

		public string lang = "";

		public Card card;

		public void Set(string _lang, Card c = null, string _sound = "beep_small")
		{
			lang = _lang;
			if (c != null)
			{
				card = c;
			}
			sound = _sound;
		}
	}

	public class ForceGiveData
	{
		public Card card;

		public int tries;
	}

	public class Transaction
	{
		public static ErrorMessage error = new ErrorMessage();

		public Thing thing;

		public ButtonGrid button;

		public int num = 1;

		public DragItemCard.DragInfo from;

		public DragItemCard.DragInfo to;

		public UIInventory destUIInv;

		public InvOwner inv;

		public InvOwner destInv
		{
			get
			{
				if (to == null)
				{
					if (!(destUIInv != null))
					{
						if (!inv.owner.IsPC)
						{
							return Main;
						}
						return Trader;
					}
					return destUIInv.owner;
				}
				return to.invOwner;
			}
		}

		public bool sell
		{
			get
			{
				if (destInv.InvertSell || destInv.owner.IsPC)
				{
					if (destInv.InvertSell)
					{
						return destInv.owner.IsPC;
					}
					return false;
				}
				return true;
			}
		}

		public string IDCurrency => currency.ToString().ToLowerInvariant();

		public CurrencyType currency => Trader.currency;

		public bool FreeTrade
		{
			get
			{
				if (HasTrader && currency != 0 && (to == null || to.invOwner.owner != EClass.pc))
				{
					if ((bool)destUIInv)
					{
						return destUIInv.owner.owner == EClass.pc;
					}
					return false;
				}
				return true;
			}
		}

		public Transaction(DragItemCard.DragInfo from, UIInventory destUIInv, int num = 1)
		{
			this.from = from;
			button = from.button;
			thing = from.thing;
			this.num = num;
			inv = from.invOwner;
			this.destUIInv = destUIInv;
		}

		public Transaction(DragItemCard.DragInfo from, DragItemCard.DragInfo to, int num = 1)
		{
			this.from = from;
			this.to = to;
			button = from.button;
			thing = from.thing;
			this.num = num;
			inv = from.invOwner;
		}

		public Transaction(ButtonGrid button, int num = 1, InvOwner owner = null)
		{
			this.button = button;
			thing = button.card as Thing;
			this.num = num;
			inv = owner ?? button.invOwner;
		}

		public bool Process(bool startTransaction = false)
		{
			error = new ErrorMessage
			{
				card = this.thing
			};
			if (!IsValid())
			{
				SE.Play(error.sound);
				if (!error.lang.IsEmpty())
				{
					Msg.Say(error.lang, error.card);
				}
				return false;
			}
			if (inv.IsFailByCurse(this.thing))
			{
				return false;
			}
			if (destInv.CopyOnTransfer)
			{
				int num = this.thing.Num;
				if (from != null && !(from.invOwner is InvOwnerAlly))
				{
					this.thing = from.invOwner.PutBack(from);
				}
				else if (this.thing.parent != inv.Container)
				{
					this.thing = inv.Container.AddThing(this.thing);
				}
				if (num != this.thing.Num)
				{
					this.thing = this.thing.Split(num);
					this.thing = EClass.pc.Pick(this.thing, msg: false, tryStack: false);
				}
				if (destInv.SingleTarget)
				{
					this.thing = this.thing.Split(1);
				}
				if (to != null)
				{
					to.button.SetCardGrid(this.thing, to.button.invOwner);
				}
				else
				{
					int currentIndex = LayerDragGrid.Instance.currentIndex;
					destInv.buttons[currentIndex].SetCardGrid(this.thing, destInv.buttons[currentIndex].invOwner);
				}
				destInv.OnProcess(this.thing);
				if (GetPrice() != 0)
				{
					SE.Play("buy");
					EClass.pc.ModCurrency(GetPrice() * -1, IDCurrency);
				}
				return true;
			}
			if (!startTransaction && to == null && (destInv.owner.IsPC ? EClass.pc : destInv.Container).things.IsFull(this.thing))
			{
				if (destInv.owner == EClass.pc)
				{
					error.Set("backpack_full");
				}
				SE.Beep();
				return false;
			}
			Thing thing = this.thing.parent as Thing;
			if (thing != null && thing.isNPCProperty)
			{
				Msg.Say("steal_container", thing, this.thing);
				this.thing.isNPCProperty = false;
				EClass.player.ModKarma(-1);
				EClass.pc.pos.TryWitnessCrime(EClass.pc);
			}
			DragItemCard dragItemCard = new DragItemCard(button, startTransaction);
			Thing thing2 = this.thing.Split(this.num);
			Thing thing3 = thing2;
			bool flag = false;
			if (FreeTrade)
			{
				flag = true;
			}
			else
			{
				SE.Play(sell ? "sell" : "buy");
				int price = GetPrice();
				if (Trader.UseHomeResource)
				{
					Trader.homeResource.Mod(sell ? price : (-price));
				}
				else
				{
					if (sell)
					{
						EClass.pc.ModCurrency(price, IDCurrency);
					}
					else
					{
						EClass.pc.ModCurrency(-price, IDCurrency);
					}
					if (sell && !thing2.IsIdentified && !ShopTransaction.current.HasBought(thing2))
					{
						thing2.Identify(show: true, IDTSource.SuperiorIdentify);
					}
					else
					{
						ShopTransaction.current.Process(thing2, thing2.Num, sell);
					}
					Msg.Say(sell ? "sold" : "bought", thing2, Lang._currency(Mathf.Abs(price), IDCurrency));
					if (thing2.id == "statue_weird" && sell)
					{
						EClass.pc.Say("statue_install");
					}
				}
			}
			if (destInv.Container.isChara && !destInv.owner.IsPC && !destInv.owner.Chara.CanAcceptItem(thing2))
			{
				destInv.owner.Chara.Talk("tooHeavy");
				return false;
			}
			if (thing2.c_isImportant && !destInv.owner.IsPC && destInv.DenyImportant)
			{
				Msg.Say("markedImportant");
				return false;
			}
			if (startTransaction)
			{
				dragItemCard.from.thing = thing2;
				EClass.ui.StartDrag(dragItemCard);
			}
			else
			{
				if (to != null)
				{
					thing2.invY = to.invY;
					thing3 = to.container.AddThing(thing2, tryStack: true, to.invX, to.invY);
					if (thing3 == thing2)
					{
						if (to.thing != null)
						{
							if (FreeTrade && !destInv.IsMagicChest)
							{
								if (!EClass.core.config.game.doubleClickToHold || !EInput.leftMouse.down || !(EClass.ui.dragDuration < 0.35f) || from == null || !(from.button == to.button) || to.button.invOwner is InvOwnerHotbar)
								{
									EClass.ui.nextDrag = new DragItemCard(to.button);
									to.invOwner.Grab(to);
									flag = false;
								}
							}
							else if (destInv.Container != thing2.parent)
							{
								thing3 = destInv.Container.AddThing(thing2);
							}
						}
						to.grid[to.invX] = thing2;
						thing2.invX = to.invX;
						to.button.card = thing2;
					}
				}
				else
				{
					bool useGrid = EClass.game.UseGrid;
					if (!EClass.game.UseGrid && (bool)destUIInv)
					{
						RectTransform rectTransform = new GameObject().AddComponent<RectTransform>();
						rectTransform.SetParent(destUIInv.list.transform);
						rectTransform.SetAnchor(0f, 0f, 0f, 0f);
						rectTransform.localScale = Vector3.one;
						rectTransform.sizeDelta = Vector3.one;
						rectTransform.position = Input.mousePosition;
						from.thing.posInvX = (int)rectTransform.anchoredPosition.x;
						from.thing.posInvY = (int)rectTransform.anchoredPosition.y;
					}
					thing3 = ((!destInv.owner.IsPC) ? destInv.Container.AddThing(thing2, useGrid) : EClass.pc.Pick(thing2, msg: false));
				}
				destInv.OnProcess(thing3);
			}
			if (destInv.Container.trait is TraitDeliveryChest)
			{
				thing2.SetInt(102, EClass._zone.uid);
			}
			else
			{
				thing2.SetInt(102);
			}
			Chara chara = destInv.Container.Chara;
			if (chara != null && !chara.IsPC)
			{
				chara.TryEquip(thing2);
				if (thing2.id == "lovepotion" || thing2.id == "dreambug")
				{
					EClass.pc.GiveLovePotion(chara, thing2);
					EClass.ui.CloseLayers();
					return true;
				}
			}
			if (thing2.id == "statue_weird")
			{
				if (!destInv.owner.IsPC)
				{
					EClass.pc.Say("statue_sell");
				}
				else if (thing == null || thing.GetRootCard() != EClass.pc)
				{
					EClass.pc.Say("statue_pick");
				}
			}
			if (flag)
			{
				SE.Drop();
			}
			return true;
		}

		public int GetPrice()
		{
			if (Trader != null)
			{
				return Trader.GetPrice(thing, currency, num, sell);
			}
			return 0;
		}

		public bool IsValid()
		{
			if (destInv is InvOwnerToolbelt)
			{
				return false;
			}
			if (destInv.UseGuide && !destInv.ShouldShowGuide(thing))
			{
				return false;
			}
			if (destInv.Container.c_lockLv != 0)
			{
				return false;
			}
			if (thing.trait is TraitAbility && destInv.owner != EClass.pc)
			{
				return false;
			}
			if (destInv.CopyOnTransfer && to != null && to.button.index != LayerDragGrid.Instance.currentIndex)
			{
				return false;
			}
			if (!destInv.AllowTransfer)
			{
				return false;
			}
			if (thing.trait.IsContainer && destInv.Container != EClass.pc && !(destInv.Container.trait is TraitToolBelt) && thing.things.Count > 0 && (!destInv.CopyOnTransfer || !destInv.ShouldShowGuide(thing)) && (destInv != Trader || ShopTransaction.current == null || !ShopTransaction.current.HasBought(thing)))
			{
				error.Set("errorUnemptyContainer");
				return false;
			}
			if (HasTrader && Trader.currency != 0)
			{
				int price = GetPrice();
				if (sell)
				{
					if (destInv.destInvY == 0 && destInv.Container.things.IsFull(thing))
					{
						return false;
					}
					if (thing.c_isImportant)
					{
						return false;
					}
					if (ShopTransaction.current == null || !ShopTransaction.current.CanSellBack(thing))
					{
						if (!Trader.AllowSell)
						{
							return false;
						}
						if (!FreeTrade && price == 0)
						{
							return false;
						}
					}
				}
				else if (!FreeTrade)
				{
					if (Trader.UseHomeResource)
					{
						if (Trader.homeResource.value < price)
						{
							return false;
						}
					}
					else if (EClass.pc.GetCurrency(IDCurrency) < price || price < 0 || (price == 0 && Trader.GetPrice(thing, currency, 1, sell) > 0))
					{
						return false;
					}
				}
			}
			if (to != null)
			{
				if (to.invOwner.IsMagicChest && to.container.things.IsFull())
				{
					return false;
				}
				if (to.thing != null && !to.thing.isDestroyed && !destInv.AllowHold(to.thing))
				{
					return false;
				}
			}
			if (destInv.owner.IsPC && !destInv.Container.IsPC && destInv.IsWeightOver(thing))
			{
				error.Set("errorOverweight");
				return false;
			}
			return true;
		}

		public string GetTextDetail()
		{
			if (FreeTrade || (destInv.UseGuide && !destInv.ShouldShowGuide(thing)))
			{
				return "";
			}
			return destInv.GetTextDetail(thing, currency, num, sell);
		}
	}

	public class Interaction
	{
		public string name;

		public string idPriority;

		public Action action;

		public int priority;

		public bool repeatable;
	}

	public class ListInteraction : List<Interaction>
	{
		public Thing thing;

		public Interaction Add(string s, int priority, Action action, string idPriority = null)
		{
			Interaction interaction = new Interaction
			{
				name = s,
				priority = priority,
				idPriority = idPriority,
				action = delegate
				{
					if (!idPriority.IsEmpty())
					{
						if (idPriority == "remove")
						{
							EClass.player.SetPriorityAction(null, thing);
						}
						else
						{
							EClass.player.SetPriorityAction(idPriority, thing);
						}
					}
					action();
				}
			};
			Add(interaction);
			return interaction;
		}

		public bool Contains(string s)
		{
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.name == s)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	public static ForceGiveData forceGive = new ForceGiveData();

	public static InvOwner Trader;

	public static InvOwner Main;

	public static float clickTimer;

	public bool includeChildren;

	public CurrencyType currency;

	public PriceType priceType;

	public HomeResource homeResource;

	public Card owner;

	public Card Container;

	public List<ButtonGrid> buttons = new List<ButtonGrid>();

	public static bool HasTrader => Trader != null;

	public static bool FreeTransfer
	{
		get
		{
			if (HasTrader)
			{
				return Trader.currency == CurrencyType.None;
			}
			return true;
		}
	}

	public virtual bool AllowAutouse => true;

	public virtual bool AllowContext => true;

	public virtual bool AllowSell
	{
		get
		{
			if (currency != 0)
			{
				return owner.trait.AllowSell;
			}
			return true;
		}
	}

	public virtual bool AlwaysShowTooltip => false;

	public virtual bool UseGuide => false;

	public virtual bool AllowTransfer
	{
		get
		{
			if (!Container.isChara)
			{
				return !Container.isNPCProperty;
			}
			return true;
		}
	}

	public virtual bool AllowDropOnDrag => true;

	public virtual string langTransfer => "actTransfer";

	public virtual int destInvY => 0;

	public virtual bool HasTransaction => false;

	public virtual bool CopyOnTransfer => false;

	public virtual bool SingleTarget => false;

	public bool UseHomeResource => homeResource != null;

	public bool IsMagicChest => Container.trait is TraitMagicChest;

	public List<Thing> Things
	{
		get
		{
			if (!Container.things.HasGrid)
			{
				return Container.things;
			}
			return Container.things.grid;
		}
	}

	public Chara Chara => owner as Chara;

	public ContainerType ContainerType => Container.trait.ContainerType;

	public virtual bool InvertSell => false;

	public virtual int numDragGrid => 1;

	public virtual bool ShowNew => owner.IsPC;

	public virtual bool DenyImportant => true;

	public InvOwner destInvOwner
	{
		get
		{
			if (!owner.IsPC)
			{
				return Main;
			}
			return Trader;
		}
	}

	public string IDCurrency => Currency.ToID(Trader.currency);

	public virtual bool AllowHold(Thing t)
	{
		if (!t.trait.CanBeDropped)
		{
			return false;
		}
		if (t.isEquipped && t.IsCursed)
		{
			return false;
		}
		if (Container.isChara && !Container.IsPC)
		{
			if (t.id == "money")
			{
				return false;
			}
			if (t.isGifted || t.isNPCProperty)
			{
				return false;
			}
			if (t.isEquipped && !Container.Chara.affinity.CanForceTradeEquip())
			{
				return false;
			}
			return true;
		}
		if (!t.trait.CanOnlyCarry)
		{
			if (Container.isNPCProperty)
			{
				return !FreeTransfer;
			}
			return true;
		}
		return false;
	}

	public virtual bool AllowMoved(Thing t)
	{
		return true;
	}

	public virtual bool ShouldShowGuide(Thing t)
	{
		return false;
	}

	public virtual bool AllowDrop(Thing t)
	{
		if (EClass._zone.IsRegion)
		{
			return t.trait is TraitAbility;
		}
		return true;
	}

	public virtual Thing CreateDefaultContainer()
	{
		return ThingGen.Create("chest3");
	}

	public virtual void BuildUICurrency(UICurrency uiCurrency, bool canReroll = false)
	{
		bool flag = Container.isChara && !Container.IsPC;
		uiCurrency.SetActive(currency != CurrencyType.None || flag);
		uiCurrency.target = owner;
		if (currency != CurrencyType.None || flag)
		{
			uiCurrency.Build(new UICurrency.Options
			{
				weight = flag,
				money = (currency == CurrencyType.Money),
				plat = (currency == CurrencyType.Plat),
				medal = (currency == CurrencyType.Medal),
				money2 = (currency == CurrencyType.Money2),
				influence = (currency == CurrencyType.Influence),
				casino = (currency == CurrencyType.Casino_coin),
				ecopo = (currency == CurrencyType.Ecopo)
			});
		}
	}

	public bool IsWeightOver(Thing t)
	{
		return false;
	}

	public InvOwner(Card owner, Card container = null, CurrencyType _currency = CurrencyType.None, PriceType _price = PriceType.Default)
	{
		currency = _currency;
		priceType = _price;
		this.owner = owner;
		Container = container ?? owner;
		if (currency == CurrencyType.BranchMoney)
		{
			homeResource = EClass.BranchOrHomeBranch.resources.money;
		}
	}

	public void Init()
	{
		if (owner == null)
		{
			CardBlueprint.SetNormalRarity();
			owner = CreateDefaultContainer();
			owner.c_lockLv = 0;
			owner.c_IDTState = 0;
			Container = Container ?? owner;
		}
		OnInit();
		forceGive = new ForceGiveData();
	}

	public virtual void OnInit()
	{
	}

	public virtual void OnClick(ButtonGrid button)
	{
		Card card = button.card;
		Card card2 = button.invOwner.owner;
		if (card == null || EClass.ui.currentDrag != null)
		{
			return;
		}
		bool flag = false;
		if (card.Thing.isEquipped && card.Thing.IsEquipmentOrRanged && card.Thing.IsCursed)
		{
			SE.Play("curse3");
		}
		else if (!AllowHold(card.Thing) && card2.isChara && !card2.IsPC && card2.IsPCFaction)
		{
			if (forceGive.card != card)
			{
				forceGive.card = card;
				forceGive.tries = 0;
			}
			else
			{
				if (!EInput.isShiftDown)
				{
					forceGive.tries++;
				}
				if (forceGive.tries >= 2)
				{
					if (card.HasTag(CTAG.gift) && card.isGifted)
					{
						EClass.ui.CloseLayers();
						card2.Say("angry", card2);
						card2.ShowEmo(Emo.angry);
						card2.Talk("noGiveRing");
						card2.Chara.ModAffinity(EClass.pc, -30);
						card2.Chara.InstantEat(card.Thing);
						return;
					}
					forceGive.card = null;
					flag = true;
					bool flag2 = card.trait is TraitCurrency;
					card.isGifted = false;
					card.isNPCProperty = false;
					card2.Talk(flag2 ? "forceGiveCurrency" : "forceGive");
					int num = ((!flag2) ? 1 : 3);
					if (card.id == "money")
					{
						num += card.Num / 1000;
					}
					if (num >= 5)
					{
						num = 5;
					}
					EClass.player.ModKarma(-num);
					EClass.pc.Pick(card.Thing);
					return;
				}
			}
		}
		if (AllowHold(card.Thing) || flag)
		{
			if (EInput.isAltDown)
			{
				if (CanAltClick(button))
				{
					OnAltClick(button);
				}
				else
				{
					SE.BeepSmall();
				}
			}
			else if (EInput.isCtrlDown)
			{
				if (CanCtrlClick(button))
				{
					OnCtrlClick(button);
				}
				else
				{
					SE.BeepSmall();
				}
			}
			else if (EInput.isShiftDown)
			{
				if (CanShiftClick(button))
				{
					OnShiftClick(button);
				}
				else
				{
					SE.BeepSmall();
				}
			}
			else if (!owner.IsPC)
			{
				new Transaction(button, (HasTrader && !FreeTransfer) ? 1 : card.Num).Process(startTransaction: true);
			}
			else if (button.card == null || !IsFailByCurse(button.card.Thing))
			{
				EClass.ui.StartDrag(new DragItemCard(button));
			}
		}
		else
		{
			SE.BeepSmall();
			if (card2.isChara && !card2.IsPC)
			{
				card2.Talk("noGive");
			}
		}
	}

	public virtual void OnRightClick(ButtonGrid button)
	{
		if (!AllowAutouse)
		{
			OnClick(button);
		}
		if (button.card != null)
		{
			AutoUse(button);
		}
	}

	public virtual void OnRightPressed(ButtonGrid button)
	{
		if (!EInput.rightMouse.pressedLong)
		{
			return;
		}
		float num = 1f;
		float pressedTimer = EInput.rightMouse.pressedTimer;
		if (pressedTimer > 2f)
		{
			num = 2f;
		}
		else if (pressedTimer > 4f)
		{
			num = 5f;
		}
		else if (pressedTimer > 6f)
		{
			num = 50f;
		}
		clickTimer -= Core.delta * num;
		if (clickTimer < 0f)
		{
			clickTimer = 0.1f;
			if (button.card != null)
			{
				AutoUse(button, repeat: true);
			}
		}
	}

	public InvOwner GetShitDestOwner(ButtonGrid b, bool rightMouse = false)
	{
		Thing thing = b.card.Thing;
		if (rightMouse && !b.invOwner.owner.IsPC)
		{
			return LayerInventory.GetPCLayer()?.Inv;
		}
		if (Trader != null)
		{
			if (b.invOwner.owner.IsPC)
			{
				if (!Trader.Container.things.IsFull(thing))
				{
					return Trader;
				}
				return null;
			}
			return LayerInventory.GetTopLayer(thing, includePlayer: true, Trader)?.Inv;
		}
		LayerInventory topLayer = LayerInventory.GetTopLayer(thing, includePlayer: true, this);
		if (topLayer == null)
		{
			return null;
		}
		return topLayer.Inv;
	}

	public virtual void OnShiftClick(ButtonGrid b, bool rightMouse = false)
	{
		InvOwner shitDestOwner = GetShitDestOwner(b, rightMouse);
		Thing thing = b.card.Thing;
		if (thing == null || thing.isDestroyed)
		{
			return;
		}
		Card container = shitDestOwner.Container;
		if (rightMouse && !owner.IsPC)
		{
			EClass.pc.Pick(thing, msg: false);
			return;
		}
		if ((thing.parent == container || thing == container) && !thing.IsHotItem)
		{
			if (EInput.isShiftDown)
			{
				SE.BeepSmall();
			}
			return;
		}
		thing.PlaySoundDrop(spatial: false);
		if (thing.IsHotItem && thing.parent == container)
		{
			container.RemoveCard(thing);
		}
		if (container.things.CanStack(thing) != thing)
		{
			container.things.TryStack(thing);
		}
		else
		{
			container.AddThing(thing);
		}
	}

	public virtual bool CanShiftClick(ButtonGrid b, bool rightMouse = false)
	{
		if (!EInput.isShiftDown && !rightMouse)
		{
			return false;
		}
		if (!EClass.pc.HasNoGoal || Container.isNPCProperty || currency != 0)
		{
			return false;
		}
		if (rightMouse && !HasTrader)
		{
			return false;
		}
		if (HasTrader)
		{
			if (Trader is InvOwnerDraglet || Trader.Container.isNPCProperty || Trader.currency != 0)
			{
				return false;
			}
			if (Trader.Container.isChara)
			{
				return false;
			}
			Card card = b.card;
			if (card != null && card.c_isImportant)
			{
				return false;
			}
		}
		InvOwner shitDestOwner = GetShitDestOwner(b, rightMouse);
		if (shitDestOwner == null)
		{
			return false;
		}
		Thing thing = b.card.Thing;
		if (thing.trait.IsContainer && thing.things.Count > 0)
		{
			return false;
		}
		if (!AllowHold(thing))
		{
			return false;
		}
		if (HasTrader)
		{
			if (!Trader.Container.GetRootCard().IsPC && thing.trait is TraitCatalyst)
			{
				return false;
			}
			if (!Trader.AllowMoved(thing))
			{
				return false;
			}
		}
		if (shitDestOwner.Container.isChara && !shitDestOwner.owner.IsPC && !shitDestOwner.owner.Chara.CanAcceptItem(thing))
		{
			return false;
		}
		if (EInput.isShiftDown)
		{
			LayerInventory.highlightInv = shitDestOwner;
		}
		return true;
	}

	public virtual string GetTextShiftClick(ButtonGrid b)
	{
		return "actTransfer".lang();
	}

	public virtual void OnCtrlClick(ButtonGrid button)
	{
		button.card.Thing.ShowSplitMenu(button, HasTransaction ? new Transaction(button) : null);
	}

	public virtual bool CanCtrlClick(ButtonGrid b)
	{
		if (!EInput.isCtrlDown)
		{
			return false;
		}
		if (!EClass.pc.HasNoGoal || Container.isNPCProperty || b.card.Num <= 1)
		{
			return false;
		}
		return true;
	}

	public virtual string GetTextCtrlClick(ButtonGrid b)
	{
		return "actSplit".lang();
	}

	public virtual void OnAltClick(ButtonGrid button)
	{
		EClass.pc.DropThing(button.card.Thing);
	}

	public virtual bool CanAltClick(ButtonGrid b)
	{
		if (!EInput.isAltDown)
		{
			return false;
		}
		if (!EClass.pc.HasNoGoal || Container.isNPCProperty || currency != 0)
		{
			return false;
		}
		if (!AllowDrop(b.card.Thing))
		{
			return false;
		}
		return true;
	}

	public virtual string GetTextAltClick(ButtonGrid b)
	{
		return "actDrop".lang();
	}

	public void Grab(DragItemCard.DragInfo from)
	{
		bool isEquipped = from.thing.isEquipped;
		if (isEquipped)
		{
			from.invOwner.owner.Chara.body.Unequip(from.thing);
		}
		from.invOwner.Container.RemoveCard(from.thing);
		if (isEquipped)
		{
			from.list.Redraw();
		}
		LayerInventory.SetDirty(from.thing);
	}

	public Thing PutBack(DragItemCard.DragInfo from)
	{
		Thing thing = from.thing;
		if (from.invY == 1 && from.container.things.IsOccupied(from.invX, from.invY))
		{
			return EClass.pc.Pick(thing, msg: false);
		}
		if (from.container.things.IsFull(thing, recursive: false, tryStack: false))
		{
			return EClass.pc.Pick(thing, msg: false);
		}
		thing.invY = from.invY;
		thing = from.container.AddThing(thing, tryStack: true, from.invX, from.invY);
		if (from.equippedSlot != null)
		{
			from.invOwner.owner.Chara.body.Equip(thing, from.equippedSlot);
			from.list.Redraw();
		}
		else
		{
			thing.invX = from.invX;
		}
		if (from.invY == 1)
		{
			WidgetCurrentTool.dirty = true;
		}
		return thing;
	}

	public void OnStartDrag(DragItemCard.DragInfo from)
	{
		from.thing.PlaySoundDrop(spatial: false);
		if (from.thing.parent != null)
		{
			Grab(from);
		}
		if (from.thing.IsEquipment)
		{
			WidgetEquip.dragEquip = from.thing;
			WidgetEquip.Redraw();
		}
	}

	public virtual bool OnCancelDrag(DragItemCard.DragInfo from)
	{
		if (Container.isNPCProperty)
		{
			return false;
		}
		PutBack(from);
		return true;
	}

	public virtual bool OnDrag(DragItemCard.DragInfo from, DragItemCard.DragInfo to, bool execute, bool cancel = false)
	{
		if (cancel)
		{
			return OnCancelDrag(from);
		}
		bool flag = false;
		string text = "";
		if ((object)EClass.ui.GetLayer<LayerRegisterHotbar>() == null)
		{
			EClass.ui.AddLayer<LayerRegisterHotbar>().SetItem(from.thing);
		}
		LayerInventory componentOf = InputModuleEX.GetComponentOf<LayerInventory>();
		if (!EClass.ui.isPointerOverUI && !EClass._zone.IsRegion)
		{
			flag = AllowDropOnDrag;
			text = ((!AllowDropOnDrag) ? "" : ((from.thing.trait is TraitAbility) ? "dragForget" : "dragDropThing"));
			if (execute)
			{
				if (!AllowDropOnDrag || from.thing.c_isImportant)
				{
					return OnCancelDrag(from);
				}
				EClass.pc.DropThing(from.thing);
				return true;
			}
		}
		else if (!EClass.core.config.game.useGrid && (bool)componentOf)
		{
			if ((bool)to.button && to.invOwner != null && to.thing != null && to.thing.CanStackTo(from.thing))
			{
				Transaction transaction = new Transaction(from, to, from.thing.Num);
				flag = transaction.IsValid();
				text = transaction.GetTextDetail();
				if (execute)
				{
					return transaction.Process();
				}
			}
			else
			{
				Transaction transaction2 = new Transaction(from, componentOf.invs[0], from.thing.Num);
				flag = transaction2.IsValid();
				text = transaction2.GetTextDetail();
				if (execute)
				{
					return transaction2.Process();
				}
			}
		}
		else if ((bool)to.button && to.invOwner != null && to.button.interactable)
		{
			if (to.invOwner is InvOwnerEquip)
			{
				InvOwnerEquip invOwnerEquip = to.invOwner as InvOwnerEquip;
				Chara chara = invOwnerEquip.owner.Chara;
				BodySlot slot = invOwnerEquip.slot;
				if (execute && to.thing != null && to.thing.blessedState <= BlessedState.Cursed)
				{
					Msg.Say("unequipCursed", to.thing);
					return false;
				}
				if (from.thing.category.slot == slot.elementId)
				{
					flag = true;
					text = "dragEquip";
					if (execute)
					{
						if (!chara.body.IsEquippable(from.thing, slot))
						{
							return false;
						}
						chara.AddCard(from.thing);
						if (to.thing != null)
						{
							EClass.ui.nextDrag = new DragItemCard(to.button);
							Grab(to);
						}
						chara.body.Equip(from.thing, invOwnerEquip.slot);
						EClass.Sound.Play("equip");
						if (EClass.game.UseGrid)
						{
							from.list.Redraw();
						}
						to.list.Redraw();
						to.invOwner.OnProcess(from.thing);
						return true;
					}
				}
			}
			else
			{
				if (to.invOwner is InvOwnerCopyShop && !to.invOwner.owner.trait.CanCopy(from.thing))
				{
					return false;
				}
				if (!to.invOwner.AllowTransfer)
				{
					return false;
				}
				Transaction transaction3 = new Transaction(from, to, from.thing.Num);
				flag = transaction3.IsValid();
				text = transaction3.GetTextDetail();
				if (execute)
				{
					return transaction3.Process();
				}
			}
		}
		EClass.ui.hud.SetDragText(flag ? text : "", (from.thing.Num > 1) ? (from.thing.Num.ToString() ?? "") : null);
		CursorSystem.SetCursor(flag ? null : CursorSystem.Invalid, 100);
		return false;
	}

	public virtual void OnProcess(Thing t)
	{
	}

	public bool CanOpenContainer(Thing t)
	{
		if (t.trait.CanOpenContainer)
		{
			if (t.parent != EClass.pc)
			{
				return (t.parent as Thing)?.trait is TraitToolBelt;
			}
			return true;
		}
		return false;
	}

	public ListInteraction ListInteractions(ButtonGrid b, bool context)
	{
		ListInteraction listInteraction = new ListInteraction();
		if (b == null || b.card == null || this is InvOwnerAlly)
		{
			return listInteraction;
		}
		Thing t = b.card.Thing;
		if (t == null)
		{
			return listInteraction;
		}
		listInteraction.thing = t;
		Trait trait = t.trait;
		bool flag = trait is TraitAbility;
		if (HasTrader)
		{
			if (Trader == null || destInvOwner == null)
			{
				return listInteraction;
			}
			if (CanOpenContainer(t))
			{
				listInteraction.Add("actContainer", 10, delegate
				{
					(t.trait as TraitContainer).TryOpen();
				});
			}
			bool flag2 = !flag && !trait.CanOnlyCarry && (!destInvOwner.UseGuide || destInvOwner.ShouldShowGuide(t));
			bool flag3 = ShopTransaction.current != null && ShopTransaction.current.CanSellBack(t);
			if (destInvOwner == Trader && !Trader.AllowSell && !flag3)
			{
				flag2 = false;
			}
			if (!AllowHold(t))
			{
				flag2 = false;
			}
			if (!Trader.AllowMoved(t))
			{
				flag2 = false;
			}
			if (flag2 && (!t.c_isImportant || !destInvOwner.DenyImportant))
			{
				if (!flag3 && (Trader.currency == CurrencyType.None || (Trader.currency != CurrencyType.Influence && t.GetPrice(Trader.currency, sell: false, Trader.priceType) == 0)))
				{
					Transaction trans2 = new Transaction(b, t.Num);
					if (trans2.IsValid())
					{
						listInteraction.Add(Container.isNPCProperty ? "actSteal".lang().TagColor(FontColor.Bad, SkinManager.DarkColors) : destInvOwner.langTransfer, 0, delegate
						{
							if (Container.isNPCProperty)
							{
								Dialog.TryWarnCrime(delegate
								{
									trans2.Process();
								});
							}
							else
							{
								trans2.Process();
							}
						});
					}
				}
				else
				{
					bool isShiftDown = EInput.isShiftDown;
					Transaction trans = new Transaction(b);
					listInteraction.Add(trans.GetTextDetail(), 0, delegate
					{
						trans.Process();
					}).repeatable = true;
					if (t.Num > 1)
					{
						Transaction trans3 = new Transaction(b, t.Num);
						listInteraction.Add(trans3.GetTextDetail(), 10, delegate
						{
							trans3.Process();
						});
						if (isShiftDown)
						{
							listInteraction.Add(listInteraction[0]);
							listInteraction.RemoveAt(0);
						}
					}
				}
			}
		}
		ListInteractions(listInteraction, t, trait, b, context);
		if (context && trait is TraitLightSource)
		{
			listInteraction.Add("customBrightness", 300, delegate
			{
				UIContextMenu uIContextMenu = EClass.ui.CreateContextMenuInteraction();
				uIContextMenu.AddSlider("brightness", (float a) => a.ToString() ?? "", EClass.player.customLightMod, delegate(float b)
				{
					EClass.player.customLightMod = (int)b;
					EClass.pc.RecalculateFOV();
				}, 1f, 6f, isInt: true, hideOther: false);
				uIContextMenu.Show();
			});
		}
		if (AllowHold(t) && !t.isEquipped && !HasTrader)
		{
			Interaction item = listInteraction.Add((EClass.pc.held == t) ? "actPick" : "actHold", 60, delegate
			{
				TryHold(t);
			});
			if (t.trait.HoldAsDefaultInteraction)
			{
				listInteraction.Remove(item);
				listInteraction.Insert(0, item);
			}
			if (t.trait.CanBeHeldAsFurniture)
			{
				listInteraction.Add("actHoldTool", 70, delegate
				{
					if (t != EClass.pc.held)
					{
						if (TryHold(t))
						{
							HotItemHeld.disableTool = true;
						}
					}
					else
					{
						HotItemHeld.disableTool = true;
					}
				}, "remove");
			}
		}
		if (context)
		{
			bool flag4 = false;
			if (AllowHold(t) && !Container.isNPCProperty)
			{
				if (t.Num > 1)
				{
					listInteraction.Add("actSplit", 50, delegate
					{
						t.ShowSplitMenu(b, (HasTrader && currency != 0 && !owner.IsPC) ? new Transaction(b) : null);
					});
				}
				if (owner.IsPC)
				{
					flag4 = true;
					listInteraction.Add(t.c_isImportant ? "important_off" : "important_on", 299, delegate
					{
						t.c_isImportant = !t.c_isImportant;
						LayerInventory.SetDirty(t);
						SE.ClickOk();
					});
					if (AllowDrop(t))
					{
						listInteraction.Add(flag ? "dragForget" : "actDrop", 300, delegate
						{
							EClass.pc.DropThing(t);
						});
					}
				}
			}
			if (!flag4 && t.c_isImportant)
			{
				listInteraction.Add(t.c_isImportant ? "important_off" : "important_on", 299, delegate
				{
					t.c_isImportant = !t.c_isImportant;
					LayerInventory.SetDirty(t);
					SE.ClickOk();
				});
			}
		}
		return listInteraction;
	}

	public bool TryHold(Thing t)
	{
		if (!AllowHold(t) || t.isEquipped || HasTrader)
		{
			return false;
		}
		if (EClass.pc.held == t)
		{
			if (t.trait.CanOnlyCarry)
			{
				SE.Beep();
				return true;
			}
			EClass.pc.PickHeld();
		}
		else
		{
			EClass.pc.HoldCard(t);
		}
		EClass.player.RefreshCurrentHotItem();
		SE.SelectHotitem();
		return true;
	}

	public virtual void ListInteractions(ListInteraction list, Thing t, Trait trait, ButtonGrid b, bool context)
	{
		if (!owner.IsPC)
		{
			return;
		}
		if (CanOpenContainer(t) && !list.Contains("actContainer"))
		{
			list.Add(LayerInventory.IsOpen(t) ? "close" : "actContainer", 100, delegate
			{
				(t.trait as TraitContainer).TryOpen();
			});
		}
		if (HasTrader)
		{
			return;
		}
		CharaBody body = owner.Chara.body;
		BodySlot slot = body.GetSlot(t, onlyEmpty: false, EInput.isShiftDown);
		if (slot != null)
		{
			Card tParent = t.parentCard;
			int tInvX = t.invX;
			int tInvY = t.invY;
			if (tInvY != 1)
			{
				list.Add("invEquip", 90, delegate
				{
					if (slot.thing != null && slot.thing.blessedState <= BlessedState.Cursed)
					{
						Msg.Say("unequipCursed", slot.thing);
						SE.Play("curse3");
					}
					else
					{
						if (EClass.pc.held == t)
						{
							EClass.pc.PickHeld();
						}
						Thing thing = slot.thing;
						body.Equip(t, slot);
						EClass.Sound.Play("equip");
						if (thing != null)
						{
							if (tParent != null)
							{
								tParent.AddThing(thing);
								thing.invX = tInvX;
								thing.invY = tInvY;
							}
							else if (thing.parent is Card && (thing.parent as Card).things.IsOverflowing())
							{
								thing.parent.RemoveCard(thing);
								EClass.pc.Pick(thing);
							}
						}
					}
				});
			}
		}
		if (!LayerChara.Instance)
		{
			if (trait.CanRead(EClass.pc))
			{
				list.Add("invRead", 110, delegate
				{
					t.DoAct(new AI_Read
					{
						target = t
					});
				}, "remove");
			}
			if (trait.CanUse(EClass.pc))
			{
				list.Add(trait.LangUse, 120, delegate
				{
					if (trait.OnUse(EClass.pc))
					{
						EClass.player.EndTurn();
					}
				}, "use");
			}
			if (trait.CanDrink(EClass.pc))
			{
				list.Add("invDrink", 130, delegate
				{
					t.DoAct(new AI_Drink
					{
						target = t
					});
				}, "remove");
			}
			if (trait.CanEat(EClass.pc))
			{
				list.Add("invFood", 140, delegate
				{
					t.DoAct(new AI_Eat
					{
						cook = false,
						target = t
					});
				}, "remove");
			}
			if (trait.IsBlendBase)
			{
				list.Add("invBlend", 150, delegate
				{
					LayerDragGrid.Create(new InvOwnerBlend(t));
				}, "blend");
			}
		}
		if (!context)
		{
			return;
		}
		if (trait is TraitCard)
		{
			list.Add("invCollect", 150, delegate
			{
				ContentCodex.Collect(t);
			});
		}
		if (!trait.CanName)
		{
			return;
		}
		list.Add("changeName", 200, delegate
		{
			Dialog.InputName("dialogChangeName", t.c_refText.IsEmpty(""), delegate(bool cancel, string text)
			{
				if (!cancel)
				{
					t.c_refText = text;
				}
			});
		});
	}

	public virtual string GetAutoUseLang(ButtonGrid button)
	{
		if (button == null || button.gameObject == null)
		{
			return "";
		}
		ListInteraction list = ListInteractions(button, context: false);
		foreach (Interaction item in list)
		{
			if (EClass.player.IsPriorityAction(item.idPriority, list.thing))
			{
				list.Insert(0, item);
				break;
			}
		}
		if (button.card.trait.HoldAsDefaultInteraction && list.Count > 0)
		{
			Interaction interaction = list[0];
			if (interaction.name == "actPick" || interaction.name == "actHold" || interaction.name == "actHoldTool")
			{
				return null;
			}
		}
		list.ForeachReverse(delegate(Interaction a)
		{
			if (a.name == "actPick" || a.name == "actHold" || a.name == "actHoldTool")
			{
				list.Remove(a);
			}
		});
		if (list.Count == 0)
		{
			return null;
		}
		if (list[0].name == "actTransfer")
		{
			return null;
		}
		return list[0].name.lang();
	}

	public void AutoUse(ButtonGrid button, bool repeat = false)
	{
		_ = button.card;
		if (!ActionMode.Adv.IsActive && !ActionMode.Region.IsActive)
		{
			return;
		}
		if (!EClass.pc.HasNoGoal)
		{
			SE.Beep();
			return;
		}
		ListInteraction listInteraction = ListInteractions(button, context: false);
		if (listInteraction.Count == 0)
		{
			SE.Beep();
			return;
		}
		foreach (Interaction item in listInteraction)
		{
			if (EClass.player.IsPriorityAction(item.idPriority, listInteraction.thing))
			{
				listInteraction.Insert(0, item);
				break;
			}
		}
		if (!repeat || listInteraction[0].repeatable)
		{
			listInteraction[0].action();
		}
	}

	public void ShowContextMenu(ButtonGrid button)
	{
		if (!EClass.pc.HasNoGoal)
		{
			SE.Beep();
			return;
		}
		ListInteraction listInteraction = ListInteractions(button, context: true);
		if (listInteraction.Count == 0)
		{
			SE.BeepSmall();
			return;
		}
		UIContextMenu uIContextMenu = EClass.ui.CreateContextMenuInteraction();
		listInteraction.Sort((Interaction a, Interaction b) => a.priority - b.priority);
		foreach (Interaction item in listInteraction)
		{
			uIContextMenu.AddButton(item.name, item.action);
		}
		uIContextMenu.Show();
	}

	public virtual int GetPrice(Thing t, CurrencyType currency, int num, bool sell)
	{
		return ShopTransaction.current.GetPrice(t, num, sell);
	}

	public virtual string GetTextDetail(Thing t, CurrencyType currency, int num, bool sell)
	{
		int price = GetPrice(t, currency, num, sell);
		string @ref = (Trader.UseHomeResource ? Trader.homeResource.Name : ((currency == CurrencyType.Influence) ? "influence".lang() : EClass.sources.things.map[IDCurrency].GetName()));
		string ref2 = ((price == 0) ? "" : "invInteraction3".lang(price.ToFormat() ?? "", @ref));
		string text = "invInteraction1".lang(num.ToString() ?? "", ref2, (sell ? "invSell" : "invBuy").lang());
		if (!sell && EClass.pc.GetCurrency(Currency.ToID(currency)) < price)
		{
			text = text.TagColor(FontColor.Bad, SkinManager.DarkColors);
		}
		return text;
	}

	public virtual void OnWriteNote(ButtonGrid button, UINote n)
	{
		if (button.card is Thing t)
		{
			bool flag = HasTrader && Trader.currency != 0 && (destInvOwner != Trader || Trader.AllowSell || (ShopTransaction.current?.CanSellBack(t) ?? false));
			if (flag && Trader.UseGuide && !Trader.ShouldShowGuide(t))
			{
				flag = false;
			}
			if (flag)
			{
				Transaction transaction = new Transaction(button);
				n.Space(8);
				UIItem uIItem = n.AddExtra<UIItem>("costPrice");
				string id = IDCostIcon(t);
				int price = transaction.GetPrice();
				uIItem.text1.SetText(Lang._currency(price), transaction.IsValid() ? FontColor.Good : FontColor.Bad);
				uIItem.image1.sprite = (Trader.UseHomeResource ? Trader.homeResource.Sprite : SpriteSheet.Get(id));
			}
		}
	}

	public virtual string IDCostIcon(Thing t)
	{
		return "icon_" + IDCurrency;
	}

	public virtual bool IsFailByCurse(Thing t)
	{
		return false;
	}
}
