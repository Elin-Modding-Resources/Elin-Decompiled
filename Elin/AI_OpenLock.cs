public class AI_OpenLock : AI_TargetThing
{
	public override CursorInfo CursorIcon => CursorSystem.Container;

	public override bool HasProgress => true;

	public override bool CanTargetInventory => true;

	public override bool IsHostileAct
	{
		get
		{
			if (base.target != null)
			{
				return base.target.isNPCProperty;
			}
			return false;
		}
	}

	public override AIProgress CreateProgress()
	{
		return new Progress_Custom
		{
			canProgress = CanProgress,
			onProgressBegin = delegate
			{
				Thing thing = owner.things.FindBest<TraitLockpick>((Thing t) => -t.c_charges);
				if (thing != null)
				{
					owner.Say("lockpick_start_pick", thing, base.target);
				}
				else
				{
					owner.Say("lockpick_start", owner, base.target);
				}
				owner.PlaySound("lock_pick");
			},
			onProgress = delegate(Progress_Custom p)
			{
				switch (base.target.trait.TryOpenLock(owner, msgFail: false))
				{
				case LockOpenState.Success:
					p.CompleteProgress();
					EClass.Sound.Stop("lock_pick");
					break;
				case LockOpenState.NotEnoughSkill:
					owner.Say("lockpick_end", owner, base.target);
					p.Cancel();
					EClass.Sound.Stop("lock_pick");
					break;
				default:
					if (EClass._zone.IsCrime(owner, this))
					{
						owner.pos.TryWitnessCrime(owner);
					}
					break;
				}
			},
			onProgressComplete = delegate
			{
				if (base.target.c_lockLv != 0 && owner != null)
				{
					owner.Say("lockpick_end", owner, base.target);
				}
			}
		}.SetDuration(30, 10);
	}
}
