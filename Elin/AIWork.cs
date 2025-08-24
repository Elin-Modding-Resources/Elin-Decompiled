using System;
using System.Collections.Generic;
using UnityEngine;

public class AIWork : AIAct
{
	public enum Work_Type
	{
		Default,
		Explore
	}

	public BaseArea destArea;

	public Point destPos;

	public Thing destThing;

	public SourceHobby.Row sourceWork;

	public override string Name => sourceWork.name_JP;

	public virtual int destDist => 0;

	public override int MaxRestart => 3;

	public virtual Work_Type WorkType => Work_Type.Default;

	public override IEnumerable<Status> Run()
	{
		yield return DoIdle(100);
		_ = WorkType;
		SetDestPos();
		if (destPos != null)
		{
			yield return DoGoto(destPos, destDist);
		}
		else
		{
			destPos = new Point(owner.pos);
		}
		AIAct work = GetWork(destPos);
		if (work != null)
		{
			owner.Talk("work_" + sourceWork.talk);
			yield return Do(work, base.KeepRunning);
		}
		yield return Restart();
	}

	public virtual AIAct GetWork(Point p)
	{
		return new AI_Idle();
	}

	public bool SetDestination()
	{
		if (WorkType == Work_Type.Explore)
		{
			if (!EClass.world.date.IsExpired(owner.GetInt(51)))
			{
				return false;
			}
			return true;
		}
		if (!sourceWork.workTag.IsEmpty())
		{
			if (destArea != null)
			{
				destThing = EClass._map.FindThing(sourceWork.workTag, destArea);
			}
			else
			{
				destThing = EClass._map.FindThing(sourceWork.workTag, owner);
			}
			if (destThing != null)
			{
				return true;
			}
		}
		if (!sourceWork.destTrait.IsEmpty())
		{
			if (Application.isEditor && Type.GetType("Trait" + sourceWork.destTrait + ", Elin") == null)
			{
				Debug.Log(sourceWork.destTrait + "/" + sourceWork.alias);
			}
			if (destArea != null)
			{
				destThing = EClass._map.FindThing(Type.GetType("Trait" + sourceWork.destTrait + ", Elin"), destArea);
				return destThing != null;
			}
			destThing = EClass._map.FindThing(Type.GetType("Trait" + sourceWork.destTrait + ", Elin"), owner);
			return destThing != null;
		}
		SetDestPos();
		return destPos != null;
	}

	public virtual void SetDestPos()
	{
		if (destThing != null && destThing.ExistsOnMap)
		{
			destPos = destThing.trait.GetRandomPoint(_FuncWorkPoint);
		}
	}

	public bool _FuncWorkPoint(Point p)
	{
		Room room = destThing.pos.cell.room;
		if (p.cell.room != room)
		{
			return false;
		}
		return FuncWorkPoint(p);
	}

	public virtual bool FuncWorkPoint(Point p)
	{
		return true;
	}

	public virtual WorkSession GetSession()
	{
		WorkSession workSession = new WorkSession
		{
			id = sourceWork.id,
			efficiency = 80
		};
		if (destThing != null)
		{
			workSession.efficiency = destThing.GetEfficiency();
		}
		else if (destArea != null)
		{
			workSession.efficiency = destArea.type.GetEfficiency();
		}
		OnGetSession(workSession);
		return workSession;
	}

	public virtual void OnGetSession(WorkSession s)
	{
	}

	public void AddDeliverable(Thing t)
	{
		owner.GetWorkSummary().AddThing(t);
	}

	public virtual void OnPerformWork(bool realtime)
	{
	}

	public void DailyOutcome()
	{
	}
}
