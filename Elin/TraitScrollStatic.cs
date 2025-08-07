public class TraitScrollStatic : TraitScroll
{
	public override SourceElement.Row source
	{
		get
		{
			if (owner.sourceCard.vals.Length == 0)
			{
				return null;
			}
			return EClass.sources.elements.alias.TryGetValue(owner.sourceCard.vals[0]);
		}
	}

	public virtual EffectId idEffect => owner.sourceCard.vals[1].ToEnum<EffectId>();

	public virtual int Power => int.Parse(owner.sourceCard.vals[2]);

	public virtual string AliasEle => owner.sourceCard.vals.TryGet(3, returnNull: true);

	public virtual string N1 => owner.sourceCard.vals.TryGet(4);

	public override bool CanRead(Chara c)
	{
		return !c.isBlind;
	}

	public override void OnRead(Chara c)
	{
		if ((c.isConfused || c.HasCondition<ConDim>()) && EClass.rnd(4) == 0)
		{
			c.Say("stagger", c);
			if (EClass.rnd(2) == 0)
			{
				TraitBaseSpellbook.ReadFailEffect(c);
			}
			return;
		}
		if (c.IsPC && (idEffect == EffectId.Identify || idEffect == EffectId.GreaterIdentify))
		{
			foreach (Thing item in EClass.pc.things.List((Thing t) => t.id == owner.id, onlyAccessible: true))
			{
				item.Identify(show: false);
			}
		}
		owner.ModNum(-1);
		c.PlayEffect("cast");
		ActEffect.ProcAt(idEffect, Power, owner.blessedState, c, null, c.pos, isNeg: false, new ActRef
		{
			aliasEle = AliasEle,
			n1 = N1,
			refThing = owner.Thing
		});
		c.elements.ModExp(285, 50f);
	}
}
