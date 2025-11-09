public class InvOwnerReconstruction : InvOwnerEffect
{
	public override bool CanTargetAlly => true;

	public override string langTransfer => "invReconstruct";

	public override string langWhat => "target_what";

	public override Thing CreateDefaultContainer()
	{
		return ThingGen.CreateScroll(8288);
	}

	public override bool ShouldShowGuide(Thing t)
	{
		if (t.HasTag(CTAG.godArtifact))
		{
			return false;
		}
		if (t.trait is TraitTent)
		{
			return false;
		}
		if (t.trait is TraitStairs)
		{
			return false;
		}
		if (!t.trait.CanBeDropped || t.category.IsChildOf("currency"))
		{
			return false;
		}
		if (t.isReplica)
		{
			return false;
		}
		if (!t.IsEquipmentOrRanged)
		{
			return t.IsThrownWeapon;
		}
		return true;
	}

	public override void _OnProcess(Thing t)
	{
		ActEffect.Proc(EffectId.Reconstruction, 100, state, t.GetRootCard(), t);
	}
}
