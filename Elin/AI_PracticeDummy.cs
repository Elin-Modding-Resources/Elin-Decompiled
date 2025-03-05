using System.Collections.Generic;

public class AI_PracticeDummy : AIAct
{
	public Card target;

	public Thing throwItem;

	public bool range;

	public override CursorInfo CursorIcon => CursorSystem.IconMelee;

	public override bool HasProgress => true;

	public override bool CanManualCancel()
	{
		return true;
	}

	public override IEnumerable<Status> Run()
	{
		isFail = () => !target.IsAliveInCurrentZone;
		yield return DoProgress();
	}

	public override AIProgress CreateProgress()
	{
		return new Progress_Custom
		{
			canProgress = () => !isFail(),
			onProgressBegin = delegate
			{
			},
			onProgress = delegate(Progress_Custom p)
			{
				if (p.progress % 10 == 0)
				{
					target.animeCounter = 0.01f;
				}
				if (throwItem != null)
				{
					if (!ActThrow.CanThrow(EClass.pc, throwItem, target))
					{
						p.Cancel();
						return;
					}
					ActThrow.Throw(EClass.pc, target.pos, target, throwItem);
				}
				else if (range && owner.GetCondition<ConReload>() == null)
				{
					if (!ACT.Ranged.CanPerform(owner, target, target.pos))
					{
						p.Cancel();
						return;
					}
					if (!ACT.Ranged.Perform(owner, target, target.pos))
					{
						p.Cancel();
					}
				}
				else
				{
					ACT.Melee.Perform(owner, target);
				}
				if (owner != null && EClass.rnd(5) < 2)
				{
					owner.stamina.Mod(-1);
				}
				if (owner != null && owner.stamina.value < 0)
				{
					p.Cancel();
				}
			},
			onProgressComplete = delegate
			{
			}
		}.SetDuration(10000);
	}
}
