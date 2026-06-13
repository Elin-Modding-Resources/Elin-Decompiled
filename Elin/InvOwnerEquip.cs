public class InvOwnerEquip : InvOwner
{
	public BodySlot slot;

	public override bool AlwaysShowTooltip => true;

	public override bool ShowNew => true;

	public override bool AllowDrop(Thing t)
	{
		return t.blessedState >= BlessedState.Normal;
	}

	public InvOwnerEquip(Card owner, BodySlot slot, Card container = null, CurrencyType _currency = CurrencyType.None)
		: base(owner, container, _currency)
	{
		this.slot = slot;
	}

	public override void ListInteractions(ListInteraction list, Thing t, Trait trait, ButtonGrid b, bool context)
	{
		list.Add("actUnequip", 0, delegate
		{
			if (!IsFailByCurse(t))
			{
				if (EClass.pc.things.IsFull())
				{
					Msg.Say("backpack_full");
					SE.BeepSmall();
				}
				else
				{
					EClass.pc.body.Unequip(t);
					EClass.Sound.Play("equip");
				}
			}
		});
		trait.OnListInteraction(list, b, context);
	}

	public override bool IsFailByCurse(Thing t)
	{
		if (t != null && t.blessedState <= BlessedState.Cursed)
		{
			Msg.Say("unequipCursed", t);
			SE.Play("curse3");
			return true;
		}
		return false;
	}

	public override void OnWriteNote(ButtonGrid button, UINote n)
	{
		if (button.card == null)
		{
			n.Clear();
			n.AddHeader(slot.name);
			n.AddText("noEQ".lang());
			if (slot.elementId == 35)
			{
				Thing.AddAttackEvaluation(n, base.Chara);
			}
		}
		else
		{
			base.OnWriteNote(button, n);
		}
	}
}
