public class TraitDye : Trait
{
	public override bool IsBlendBase => true;

	public override bool CanUse(Chara c, Card tg)
	{
		if (tg.isThing && c.Dist(tg) <= 1)
		{
			return CanBlend(tg.Thing);
		}
		return false;
	}

	public override bool OnUse(Chara c, Card tg)
	{
		Dye(tg);
		return true;
	}

	public override void OnThrowGround(Chara c, Point p)
	{
		if (p.HasObj)
		{
			p.cell.objMat = owner.material.id;
			p.cell.isObjDyed = true;
			owner.Die();
		}
	}

	public override bool CanBlend(Thing t)
	{
		return t.id != "dye";
	}

	public override void OnBlend(Thing t, Chara c)
	{
		Dye(t);
	}

	public void Dye(Card tg)
	{
		tg.Dye(owner.material);
		Msg.Say("dye", tg);
		EClass.pc.PlaySound("water_farm");
		owner.ModNum(-1);
	}
}
