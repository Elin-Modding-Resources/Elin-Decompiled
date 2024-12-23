public class TraitAltarChaos : Trait
{
	public override void TrySetAct(ActPlan p)
	{
		p.TrySetAct("actOffer", delegate
		{
			LayerDragGrid.CreateChaosOffering(this);
			return false;
		}, owner);
	}
}
