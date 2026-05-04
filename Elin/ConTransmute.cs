public class ConTransmute : BaseBuff
{
	public override bool IsToggle => true;

	public override bool ShouldRefresh => true;

	public override bool ShouldTryNullify => true;

	public virtual bool ShouldRevealOnContact => true;

	public virtual bool ShouldRevealOnPush => true;

	public virtual bool ShouldRevealOnDamage => false;

	public override void Tick()
	{
		if (HasDuration && owner.host == null && owner.conSleep == null && (EClass.pc.conSleep == null || EClass.pc.conSleep.pcSleep == 0))
		{
			base.Tick();
		}
	}

	public override void OnStart()
	{
		Change();
	}

	public override void OnHit(Card attacker, AttackSource source)
	{
		if (ShouldRevealOnDamage)
		{
			Reveal(attacker);
		}
	}

	public void Change()
	{
		if (owner.ai is GoalCombat { abilities: not null } goalCombat)
		{
			goalCombat.BuildAbilityList();
		}
		owner._CreateRenderer();
		if (owner.IsPCParty)
		{
			WidgetRoster.SetDirty();
		}
	}

	public override void OnRemoved()
	{
		bool isSynced = owner.isSynced;
		owner._CreateRenderer();
		if (isSynced)
		{
			EClass.scene.syncList.Add(owner.renderer);
			owner.renderer.OnEnterScreen();
		}
		if (owner.IsPCParty)
		{
			WidgetRoster.SetDirty();
		}
	}

	public override bool TryNullify(Condition c)
	{
		if (c != this && c is ConTransmute)
		{
			owner.Say("nullify", owner, Name.ToLower(), c.Name.ToLower());
			return true;
		}
		return false;
	}

	public virtual void Reveal(Card attacker = null, bool surprise = false)
	{
		Kill();
	}
}
