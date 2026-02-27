public class ActZap : Act
{
	public TraitRod trait;

	public override int MaxRadius => 2;

	public override int PerformDistance => 99;

	public override TargetType TargetType => TargetType.Ground;

	public override bool Perform()
	{
		if (Act.CC.IsPC)
		{
			Act.CC.TryAbsorbRod(trait.owner.Thing);
		}
		Act.CC.Say("zapRod", Act.CC, trait.owner.Name);
		if (trait.owner.c_charges > 0)
		{
			trait.owner.ModCharge(-1);
			Act.CC.PlayEffect("rod");
			Act.CC.PlaySound("rod");
			Act.CC.RemoveCondition<ConInvisibility>();
			Act.TC = Act.CC;
			EffectId idEffect = trait.IdEffect;
			long a = trait.Power * (100 + (long)Act.CC.Evalue(305) * 10L + Act.CC.MAG / 2 + Act.CC.PER / 2) / 100;
			ActEffect.ProcAt(idEffect, MathEx.Min(a), trait.owner.blessedState, Act.CC, null, Act.TP, trait.IsNegative, new ActRef
			{
				refThing = trait.owner.Thing,
				aliasEle = trait.aliasEle,
				n1 = trait.N1,
				act = ((trait.source != null) ? ACT.Create(trait.source) : null)
			});
			if (Act.CC.IsPC && (idEffect == EffectId.Identify || idEffect == EffectId.GreaterIdentify))
			{
				trait.owner.Thing.Identify(Act.CC.IsPCParty);
			}
			Act.CC.ModExp(305, 50);
			return true;
		}
		Act.CC.Say("nothingHappens");
		Act.CC.PlaySound("rod_empty");
		return true;
	}
}
