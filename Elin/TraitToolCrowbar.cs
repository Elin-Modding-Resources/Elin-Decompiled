public class TraitToolCrowbar : TraitTool
{
	public override void TrySetHeldAct(ActPlan p)
	{
		p.pos.Things.ForEach(delegate(Thing t)
		{
			if (!t.isMasked && !t.isHidden && t.IsContainer && (t.c_lockLv > 0 || t.isNPCProperty))
			{
				p.TrySetAct(new AI_PryOpen
				{
					target = t
				}, t);
			}
		});
	}
}
