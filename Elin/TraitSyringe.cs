public class TraitSyringe : Trait
{
	public override bool CanChangeHeight => false;

	public void EffectInject(Chara cc, Chara tc)
	{
		cc.PlaySound("syringe");
		cc.Say("syringe", cc, tc, owner.NameOne);
		tc.PlayEffect("blood").SetParticleColor(EClass.Colors.matColors[tc.material.alias].main).Emit(20);
		tc.AddBlood(2 + EClass.rnd(2));
	}
}
