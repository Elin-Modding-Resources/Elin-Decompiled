public class TraitSyringeHeaven : TraitSyringe
{
	public override void TrySetHeldAct(ActPlan p)
	{
		p.pos.Charas.ForEach(delegate(Chara c)
		{
			p.TrySetAct("actInject".lang("", c.Name), delegate
			{
				EffectInject(EClass.pc, c);
				c.AddCondition<ConHallucination>(50);
				if (c.trait is TraitLittleOne && !c.HasCondition<ConDeathSentense>())
				{
					EClass.player.ModKarma(3);
					c.AddCondition(Condition.Create(100, delegate(ConDeathSentense con)
					{
						con.euthanasia = true;
					}), force: true);
				}
				owner.ModNum(-1);
				return true;
			}, c);
		});
	}
}
