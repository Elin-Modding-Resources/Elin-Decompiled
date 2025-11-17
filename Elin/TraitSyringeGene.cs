public class TraitSyringeGene : TraitTicketChampagne.TraitSyringe
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
				c.AddCondition<ConHallucination>(50);
				TraitGeneMachine traitGeneMachine = c.pos.FindThing<TraitGeneMachine>();
				if (traitGeneMachine != null && traitGeneMachine.GetTarget() == c)
				{
					int remainingHours = EClass.world.date.GetRemainingHours(c.conSuspend.dateFinish);
					remainingHours = remainingHours * 2 / 3 - 10;
					if (remainingHours < 0)
					{
						remainingHours = 0;
					}
					c.conSuspend.dateFinish = EClass.world.date.GetRaw(remainingHours);
					c.PlayEffect("heal_tick");
					c.PlaySound("heal_tick");
				}
				else
				{
					c.ModCorruption(-50);
				}
				owner.ModNum(-1);
				return false;
			}, c);
		});
	}
}
