public class ConBaseTransmuteMimic : ConTransmute
{
	public virtual Card Card => null;

	public override bool HasDuration => false;

	public virtual bool IsThing => Card.isThing;

	public virtual bool IsChara => Card.isChara;

	public override bool ShouldRevealOnDamage => true;

	public override void SetOwner(Chara _owner, bool onDeserialize = false)
	{
		base.SetOwner(_owner);
		owner.mimicry = this;
	}

	public override void OnRemoved()
	{
		owner.mimicry = null;
		base.OnRemoved();
	}

	public override void Reveal(Card attacker = null, bool surprise = false)
	{
		if (attacker is Chara && owner.IsHostile(attacker.Chara))
		{
			owner.DoHostileAction(attacker, immediate: true);
		}
		if (surprise)
		{
			owner.AddCondition<ConAmbush>();
		}
		base.Reveal(attacker, surprise);
	}

	public virtual string GetName(NameStyle style, int num = -1)
	{
		return Card.GetName(style, num);
	}

	public virtual string GetHoverText()
	{
		return Card.GetHoverText();
	}

	public virtual string GetHoverText2()
	{
		return Card.GetHoverText2();
	}

	public virtual void TrySetAct(ActPlan p)
	{
	}

	public virtual bool ShouldEndMimicry(Act act)
	{
		return true;
	}
}
