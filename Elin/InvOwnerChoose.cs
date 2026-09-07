public class InvOwnerChoose : InvOwner
{
	public override bool UseGuide => true;

	public InvOwnerChoose(Card owner, Card container = null, CurrencyType _currency = CurrencyType.None)
		: base(owner, container, _currency)
	{
	}

	public override bool ShouldShowGuide(Thing t)
	{
		return t.parent == owner;
	}

	public override bool AllowMoved(Thing t)
	{
		return false;
	}

	public override bool OnCancelDrag(DragItemCard.DragInfo from)
	{
		return false;
	}

	public override void OnClick(ButtonGrid button)
	{
		Process(button);
	}

	public override void OnRightClick(ButtonGrid button)
	{
		Process(button);
	}

	public override void OnRightPressed(ButtonGrid button)
	{
	}

	public bool Process(ButtonGrid button)
	{
		if (!button || button.card == null || button.card.parent != owner)
		{
			return false;
		}
		owner.ChangeMaterial("onyx");
		owner.trait.OnUse(EClass.pc);
		EClass.pc.PickOrDrop(EClass.pc.pos, button.card.Thing);
		owner.things.DestroyAll();
		EClass.ui.CloseLayers();
		return true;
	}
}
