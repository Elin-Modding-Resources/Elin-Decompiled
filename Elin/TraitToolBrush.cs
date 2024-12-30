public class TraitToolBrush : TraitTool
{
	public override bool DisableAutoCombat => true;

	public override void TrySetHeldAct(ActPlan p)
	{
		foreach (Chara chara in p.pos.Charas)
		{
			if (chara.interest > 0)
			{
				p.TrySetAct(new AI_TendAnimal
				{
					target = chara
				}, chara);
			}
		}
	}
}
