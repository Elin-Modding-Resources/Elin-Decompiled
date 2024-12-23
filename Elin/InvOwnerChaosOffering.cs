public class InvOwnerChaosOffering : InvOwnerDraglet
{
	public TraitAltarChaos altar;

	public override string langTransfer => "invOffering";

	public override ProcessType processType => ProcessType.Consume;

	public override bool DenyImportant => true;

	public InvOwnerChaosOffering(Card owner = null, Card container = null, CurrencyType _currency = CurrencyType.Money)
		: base(owner, container, _currency)
	{
	}

	public override bool ShouldShowGuide(Thing t)
	{
		return t.source._origin == "artifact_summon";
	}

	public override void _OnProcess(Thing t)
	{
		string id = "swordkeeper";
		if (!EClass.player.codex.Has(id))
		{
			Msg.SayNothingHappen();
			return;
		}
		count = 1;
		SE.Change();
		t.ModNum(-1);
		owner.PlayEffect("curse");
		Chara chara = CharaGen.Create(id);
		EClass._zone.AddCard(chara, owner.pos.GetNearestPoint(allowBlock: false, allowChara: false) ?? owner.pos);
		chara.PlayEffect("aura_heaven");
		Msg.Say("summon_god");
		Msg.Say("summon_god２", chara);
	}
}
