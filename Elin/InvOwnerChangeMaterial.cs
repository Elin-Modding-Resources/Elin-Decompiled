public class InvOwnerChangeMaterial : InvOwnerEffect
{
	public SourceMaterial.Row mat;

	public Thing consume;

	public override bool CanTargetAlly => true;

	public override string langTransfer => "invChangeMaterial";

	public override string langWhat => "changeMaterial_what";

	public override Thing CreateDefaultContainer()
	{
		return ThingGen.Create("mathammer", mat.alias);
	}

	public override bool ShouldShowGuide(Thing t)
	{
		if (t.trait is TraitGodStatue)
		{
			return true;
		}
		if (!t.category.IsChildOf("currency") && !t.IsUnique && t.trait.CanBeDropped && !t.source.fixedMaterial && !(t.trait is TraitCatalyst) && !(t.trait is TraitTile) && !(t.trait is TraitMaterialHammer))
		{
			return !(t.trait is TraitSeed);
		}
		return false;
	}

	public override void _OnProcess(Thing t)
	{
		ActEffect.Proc(idEffect, 100, state, t.GetRootCard(), t, new ActRef
		{
			n1 = mat.alias
		});
		if (consume != null)
		{
			consume.ModNum(-1);
		}
		if (t.trait is TraitGodStatue traitGodStatue)
		{
			traitGodStatue.OnChangeMaterial();
		}
	}
}
