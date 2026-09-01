public class InvOwnerChangeRarity : InvOwnerEffect
{
	public Thing consume;

	public override bool CanTargetAlly => true;

	public override string langTransfer => "invChangeRarity";

	public override string langWhat => "changeMaterial_what";

	public override Thing CreateDefaultContainer()
	{
		return ThingGen.Create("hammer_garokk");
	}

	public override bool ShouldShowGuide(Thing t)
	{
		if (t.IsEquipment && t.rarity <= Rarity.Legendary && !t.HasTag(CTAG.godArtifact) && !t.IsLightsource)
		{
			return t.category.id != "relic";
		}
		return false;
	}

	public override void _OnProcess(Thing t)
	{
		ActEffect.Proc(idEffect, 100, state, t.GetRootCard(), t);
		if (consume != null)
		{
			consume.ModNum(-1);
		}
	}
}
