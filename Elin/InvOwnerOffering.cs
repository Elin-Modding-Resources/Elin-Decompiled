public class InvOwnerOffering : InvOwnerDraglet
{
	public TraitAltar altar;

	public override string langTransfer => "invOffering";

	public override ProcessType processType => ProcessType.Consume;

	public override bool DenyImportant => true;

	public InvOwnerOffering(Card owner = null, Card container = null, CurrencyType _currency = CurrencyType.Money)
		: base(owner, container, _currency)
	{
	}

	public override bool ShouldShowGuide(Thing t)
	{
		return altar.CanOffer(EClass.pc, t);
	}

	public override void _OnProcess(Thing t)
	{
		altar.OnOffer(EClass.pc, t);
	}
}
