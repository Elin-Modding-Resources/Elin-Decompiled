public class AI_PryOpen : AI_TargetThing
{
	public override CursorInfo CursorIcon => CursorSystem.Container;

	public override bool HasProgress => true;

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

	public override bool CanManualCancel()
	{
		return true;
	}

	public override AIProgress CreateProgress()
	{
		return new Progress_Custom
		{
			canProgress = CanProgress,
			onProgressBegin = delegate
			{
				owner.Say("pry_start", owner, base.target);
			},
			onProgress = delegate(Progress_Custom p)
			{
				owner.PlaySound("lock_pry");
				if (p.progress >= 3)
				{
					if (base.target.trait.TryPryOpenLock(owner, msgFail: false) == LockOpenState.Success)
					{
						p.CompleteProgress();
					}
					else if (EClass._zone.IsCrime(owner, this))
					{
						owner.pos.TryWitnessCrime(owner);
					}
				}
			},
			onProgressComplete = delegate
			{
			}
		}.SetDuration(100, 10);
	}

	public override void OnCancelOrSuccess()
	{
		if (owner != null && !base.target.isDestroyed && base.target.c_lockLv != 0)
		{
			owner.Say("pry_end", owner, base.target);
		}
		EClass.Sound.Stop("lock_pry");
	}
}
