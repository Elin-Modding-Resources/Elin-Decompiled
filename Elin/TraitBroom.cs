public class TraitBroom : TraitTool
{
	public override bool CanHarvest => false;

	public override void TrySetHeldAct(ActPlan p)
	{
		p.TrySetAct(new TaskClean
		{
			dest = p.pos.Copy()
		});
	}
}
