using System.Collections.Generic;

public class GoalSleep : Goal
{
	public int timer = 20;

	public Thing bed;

	public override IEnumerable<Status> Run()
	{
		for (int i = 0; i < 5; i++)
		{
			yield return Status.Running;
		}
		TraitBed traitBed = owner.FindBed();
		if (traitBed == null)
		{
			traitBed = owner.TryAssignBed();
		}
		if (traitBed != null)
		{
			bed = traitBed.owner.Thing;
		}
		if (bed != null)
		{
			yield return DoGoto(bed, base.KeepRunning);
		}
		else if (owner.memberType != FactionMemberType.Livestock)
		{
			BaseArea baseArea = EClass._map.FindPublicArea();
			if (baseArea != null)
			{
				yield return DoGoto(baseArea.GetRandomFreePos(), 0, ignoreConnection: false, base.KeepRunning);
			}
		}
		for (int i = 0; i < 5; i++)
		{
			if (!owner.pos.HasMultipleChara)
			{
				break;
			}
			owner.MoveRandom();
			yield return Status.Running;
		}
		owner.AddCondition<ConSleep>(3000, force: true);
		yield return Status.Running;
	}

	public override void OnSimulatePosition()
	{
		owner.AddCondition<ConSleep>(2000, force: true);
		bed = owner.FindBed()?.owner.Thing ?? null;
		if (bed == null && EClass._zone.IsPCFaction)
		{
			owner.TryAssignBed();
		}
		if (bed != null)
		{
			owner.MoveImmediate(bed.pos);
			return;
		}
		BaseArea baseArea = EClass._map.FindPublicArea();
		if (baseArea != null)
		{
			owner.MoveImmediate(baseArea.GetRandomFreePos());
		}
	}
}
