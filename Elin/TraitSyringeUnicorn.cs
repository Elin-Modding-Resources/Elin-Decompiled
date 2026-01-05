public class TraitSyringeUnicorn : TraitSyringe
{
	public override void TrySetHeldAct(ActPlan p)
	{
		p.pos.Charas.ForEach(delegate(Chara c)
		{
			p.TrySetAct("actInject".lang("", c.Name), delegate
			{
				EffectInject(EClass.pc, c);
				c.PlayEffect("heal");
				c.PlaySound("heal_tick");
				c.CureHost(CureType.Unicorn, 100, owner.blessedState);
				owner.ModNum(-1);
				return true;
			}, c);
		});
	}
}
