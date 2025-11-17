public class TraitSyringeBlood : TraitTicketChampagne.TraitSyringe
{
	public override void TrySetHeldAct(ActPlan p)
	{
		p.pos.Charas.ForEach(delegate(Chara c)
		{
			p.TrySetAct("actInject".lang("", c.Name), delegate
			{
				EClass.pc.PlaySound("syringe");
				EClass.pc.Say("syringe", EClass.pc, c, owner.NameOne);
				c.PlayEffect("blood").SetParticleColor(EClass.Colors.matColors[c.material.alias].main).Emit(20);
				c.AddBlood(2 + EClass.rnd(2));
				EClass.pc.PickOrDrop(EClass.pc.pos, CraftUtil.MakeBloodSample(EClass.pc, c));
				owner.ModNum(-1);
				return false;
			}, c);
		});
	}
}
