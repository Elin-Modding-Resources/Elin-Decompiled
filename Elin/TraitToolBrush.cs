public class TraitToolBrush : TraitTool
{
	public override bool DisableAutoCombat => true;

	public override Emo2 GetHeldEmo(Chara c)
	{
		if (c.IsPC || c.interest <= 0)
		{
			return Emo2.none;
		}
		if (c._affinity < 0)
		{
			return Emo2.brush_hate;
		}
		if (c.affinity.CurrentStage >= Affinity.Stage.Respected)
		{
			return Emo2.brush_like3;
		}
		if (c.affinity.CurrentStage >= Affinity.Stage.Approved)
		{
			return Emo2.brush_like2;
		}
		return Emo2.brush_like;
	}

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

	public static bool IsTamePossible(Chara c)
	{
		if (c == null || c.isDead)
		{
			return false;
		}
		if (!c.trait.CanBeTamed)
		{
			return false;
		}
		return !EClass._zone.IsInstance;
	}
}
