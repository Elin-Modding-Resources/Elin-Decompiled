using System;

public class AM_Designation<T> : AM_BaseTileSelect where T : TaskDesignation
{
	public TaskList<T> list;

	public T mold;

	public override string idSound => "actionMode";

	public override bool IsBuildMode => true;

	public virtual bool CanInstaComplete(T t)
	{
		if (!EClass.player.instaComplete)
		{
			return ForcedInstaComplete(t);
		}
		return true;
	}

	public virtual bool ForcedInstaComplete(T t)
	{
		return false;
	}

	public virtual HitResult HitResultOnDesignation(Point p)
	{
		return HitResult.Outline;
	}

	public override void OnActivate()
	{
		CreateNewMold();
	}

	public void CreateNewMold(bool processing = false)
	{
		mold = Activator.CreateInstance<T>();
		OnCreateMold(processing);
	}

	public virtual void OnCreateMold(bool processing = false)
	{
	}

	public override HitResult HitTest(Point point, Point start)
	{
		if (!base.Summary.CanExecute())
		{
			return HitResult.Invalid;
		}
		if (point.HasDesignation)
		{
			return HitResultOnDesignation(point);
		}
		mold.pos.Set(point);
		HitResult hitResult = mold.GetHitResult();
		if (hitResult != HitResult.Default)
		{
			return hitResult;
		}
		return base.HitTest(point, start);
	}

	public override bool CanProcessTiles()
	{
		if (!base.Summary.CanExecute())
		{
			return false;
		}
		if (base.Summary.countValid == 0)
		{
			return false;
		}
		return base.CanProcessTiles();
	}

	public override void OnBeforeProcessTiles()
	{
		CreateNewMold(processing: true);
		EClass._map.tasks.undo.NewItem();
	}

	public override void OnProcessTiles(Point point, int dir)
	{
		mold.pos.Set(point);
		if (list.TryAdd(mold))
		{
			EClass._map.tasks.undo.Add(mold);
			CreateNewMold(processing: true);
		}
		T val = list.items.LastItem();
		if (val != null && CanInstaComplete(val))
		{
			val.owner = EClass.player.Agent;
			EClass.player.Agent.pos.Set(val.pos);
			val.OnProgressComplete();
			val.Destroy();
		}
	}

	public override void OnAfterProcessTiles(Point start, Point end)
	{
		CreateNewMold();
	}

	public override void OnRefreshSummary(Point point, HitResult result, HitSummary summary)
	{
		summary.count++;
		if (result == HitResult.Valid || result == HitResult.Warning)
		{
			mold.pos.Set(point);
			if (CanInstaComplete(mold) && !ForcedInstaComplete(mold))
			{
				summary.money += CostMoney;
			}
			summary.countValid++;
		}
	}
}
