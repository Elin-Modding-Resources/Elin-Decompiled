using DG.Tweening;
using UnityEngine;

public class InvOwnerDraglet : InvOwner
{
	public enum ProcessType
	{
		None,
		Consume
	}

	public int count = -1;

	public BlessedState state;

	public int price;

	public virtual ProcessType processType => ProcessType.None;

	public virtual bool ShowFuel => false;

	public virtual bool CanTargetAlly => false;

	public virtual string langWhat => "";

	public virtual bool AllowStockIngredients => false;

	public override bool AllowTransfer => true;

	public override bool AllowAutouse => false;

	public override bool AllowContext => false;

	public override bool UseGuide => true;

	public override bool CopyOnTransfer => true;

	public override bool InvertSell => true;

	public override bool DenyImportant => false;

	public LayerDragGrid dragGrid => LayerDragGrid.Instance;

	public override bool AllowHold(Thing t)
	{
		return false;
	}

	public InvOwnerDraglet(Card owner = null, Card container = null, CurrencyType _currency = CurrencyType.Money)
		: base(owner, container, _currency)
	{
	}

	public override void OnInit()
	{
		if (price == 0)
		{
			currency = CurrencyType.None;
		}
		if (!langWhat.IsEmpty())
		{
			Msg.Say(langWhat);
		}
	}

	public override void OnClick(ButtonGrid button)
	{
		if (button.card != null)
		{
			if (button.index == 0)
			{
				dragGrid.ClearButtons();
			}
			else
			{
				button.SetCard(null);
				dragGrid.RefreshCurrentGrid();
			}
			dragGrid.TryPutBack();
			dragGrid.uiIngredients.Refresh();
			SE.Equip();
			if (EClass.pc.ai is AI_UseCrafter && EClass.pc.ai.IsRunning)
			{
				EClass.pc.ai.Success();
			}
		}
	}

	public override void OnRightClick(ButtonGrid button)
	{
		OnClick(button);
	}

	public override int GetPrice(Thing t, CurrencyType currency, int num, bool sell)
	{
		return price;
	}

	public override string GetTextDetail(Thing t, CurrencyType currency, int num, bool sell)
	{
		int num2 = GetPrice(t, currency, num, sell);
		return "invInteraction2".lang(num.ToString() ?? "", langTransfer.lang()) + ((num2 == 0) ? "" : "invInteraction3".lang(num2.ToString() ?? "", EClass.sources.things.map[base.IDCurrency].GetName()));
	}

	public virtual void OnWriteNote(Thing t, UINote n)
	{
	}

	public sealed override void OnProcess(Thing t)
	{
		if (!dragGrid.IsAllGridSet())
		{
			dragGrid.RefreshCurrentGrid();
			dragGrid.uiIngredients.Refresh();
			t.PlaySoundDrop(spatial: false);
			return;
		}
		_OnProcess(t);
		UIButton.TryShowTip();
		LayerInventory.SetDirty(t);
		if (processType == ProcessType.Consume)
		{
			EInput.haltInput = true;
			foreach (ButtonGrid b in dragGrid.buttons)
			{
				b.icon.rectTransform.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(delegate
				{
					if (b.transform != null)
					{
						b.icon.rectTransform.localScale = Vector3.one;
						b.SetCardGrid(null, b.invOwner);
						RedrawButton();
					}
					EInput.haltInput = false;
				});
			}
		}
		else
		{
			RedrawButton();
		}
		dragGrid.uiIngredients.Refresh();
		if (count > 0)
		{
			count--;
			if (count == 0)
			{
				dragGrid.Close();
			}
			else if (!langWhat.IsEmpty())
			{
				Msg.Say(langWhat);
			}
		}
	}

	public virtual void _OnProcess(Thing t)
	{
	}

	public virtual void OnAfterRefuel()
	{
	}

	public void RedrawButton()
	{
		dragGrid.RefreshCost();
		dragGrid.RefreshCurrentGrid();
	}

	public override void BuildUICurrency(UICurrency uiCurrency, bool canReroll = false)
	{
		base.BuildUICurrency(uiCurrency);
		uiCurrency.SetActive(price != 0);
	}
}
