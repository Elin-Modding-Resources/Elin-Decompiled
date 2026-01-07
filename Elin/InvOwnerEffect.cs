public class InvOwnerEffect : InvOwnerDraglet
{
	public Chara cc;

	public EffectId idEffect;

	public bool superior;

	public int power = 100;

	public InvOwnerEffect(Card owner = null, Card container = null, CurrencyType _currency = CurrencyType.Money)
		: base(owner, container, _currency)
	{
	}
}
