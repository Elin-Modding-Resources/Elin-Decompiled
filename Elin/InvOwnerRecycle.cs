using UnityEngine;

public class InvOwnerRecycle : InvOwnerDraglet
{
	public TraitRecycle recycle;

	public override string langTransfer => "invRecycle";

	public override ProcessType processType => ProcessType.Consume;

	public override bool InvertSell => false;

	public override bool DenyImportant => true;

	public InvOwnerRecycle(Card owner = null, Card container = null, CurrencyType _currency = CurrencyType.Ecopo)
		: base(owner, container, _currency)
	{
	}

	public override void BuildUICurrency(UICurrency uiCurrency, bool canReroll = false)
	{
		uiCurrency.SetActive(enable: true);
		uiCurrency.Build(new UICurrency.Options
		{
			ecopo = true,
			influence = canReroll
		});
	}

	public override bool ShouldShowGuide(Thing t)
	{
		if (t.c_isImportant)
		{
			return false;
		}
		if (t.id == "1084")
		{
			return true;
		}
		if (t.things.Count == 0 && t.trait.CanBeDestroyed && !t.trait.CanOnlyCarry && t.rarity < Rarity.Artifact && t.category.GetRoot().id != "currency")
		{
			return !(t.trait is TraitRecycle);
		}
		return false;
	}

	public override void _OnProcess(Thing t)
	{
		SE.Play("trash");
		Msg.Say("dump", t, Container.Name);
		int a = t.Num * Mathf.Clamp(t.GetPrice() / 100, 1, 100);
		a = EClass.rndHalf(a);
		if (t.id == "1084")
		{
			a *= 10;
		}
		if (a != 0)
		{
			EClass.pc.Pick(ThingGen.Create("ecopo").SetNum(a / 10 + 1));
		}
		t.Destroy();
	}
}
