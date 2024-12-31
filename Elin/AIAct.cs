using System;
using System.Collections.Generic;
using UnityEngine;

public class AIAct : Act
{
	public enum Status
	{
		Running,
		Fail,
		Success
	}

	public new Chara owner;

	public Status status;

	public IEnumerator<Status> Enumerator;

	public AIAct child;

	public AIAct parent;

	public byte restartCount;

	public Func<Status> onChildFail;

	public Func<bool> isFail;

	public virtual bool IsRunning
	{
		get
		{
			if (status == Status.Running)
			{
				return owner != null;
			}
			return false;
		}
	}

	public bool IsChildRunning
	{
		get
		{
			if (child != null && child.status == Status.Running)
			{
				return child.owner != null;
			}
			return false;
		}
	}

	public bool IsMoveAI
	{
		get
		{
			if (!IsChildRunning)
			{
				return this is AI_Goto;
			}
			return child.IsMoveAI;
		}
	}

	public virtual int MaxRestart => 5;

	public new virtual string Name => base.source.GetName();

	protected virtual MultiSprite stateIcon => null;

	public virtual Sprite actionIcon => EClass.core.refs.orbitIcons.Default;

	public virtual bool IsNoGoal => false;

	public virtual bool IsAutoTurn => false;

	public virtual bool IsIdle => false;

	public virtual bool CancelOnAggro => true;

	public virtual bool PushChara => true;

	public virtual int MaxProgress => 20;

	public virtual bool ShowProgress => true;

	public virtual bool UseTurbo => true;

	public virtual int CurrentProgress => 0;

	public virtual bool ShowCursor => true;

	public virtual bool CancelWhenDamaged => true;

	public virtual bool CancelWhenMoved => false;

	public virtual bool InformCancel => true;

	public virtual Thing RenderThing => null;

	public AIAct Current
	{
		get
		{
			if (!IsChildRunning)
			{
				return this;
			}
			return child.Current;
		}
	}

	public override bool IsAct => false;

	public override bool ShowPotential => false;

	public override bool UsePotential => false;

	public override bool ShowRelativeAttribute => true;

	public virtual bool HasProgress => true;

	public override string ToString()
	{
		return GetType().Name;
	}

	public override MultiSprite GetStateIcon()
	{
		return (child ?? this).stateIcon;
	}

	public virtual bool ShouldAllyAttack(Chara tg)
	{
		return child?.ShouldAllyAttack(tg) ?? true;
	}

	public virtual Point GetDestination()
	{
		if (!IsChildRunning)
		{
			return owner.pos;
		}
		return child.GetDestination();
	}

	public AIProgress GetProgress()
	{
		if (IsChildRunning)
		{
			return child.GetProgress();
		}
		return this as AIProgress;
	}

	public string GetCurrentActionText()
	{
		if (IsChildRunning && child.source.id != 0 && !child.source.name_JP.IsEmpty())
		{
			return child.GetCurrentActionText();
		}
		return base.source.GetName().IsEmpty("idle".lang());
	}

	public override bool IsToolValid()
	{
		if (owner == EClass.pc && Act.TOOL != null)
		{
			return Act.TOOL.parent == EClass.pc;
		}
		return false;
	}

	public virtual void OnStart()
	{
	}

	public void SetOwner(Chara c)
	{
		owner = c;
		status = Status.Running;
		OnSetOwner();
	}

	public virtual void OnSetOwner()
	{
	}

	public void SetChild(AIAct seq, Func<Status> _onChildFail = null)
	{
		if (child != null)
		{
			child.Reset();
		}
		child = seq;
		child.parent = this;
		child.SetOwner(owner);
		onChildFail = _onChildFail ?? new Func<Status>(Cancel);
	}

	public void Start()
	{
		Enumerator = Run().GetEnumerator();
		OnStart();
		if (owner == null)
		{
			return;
		}
		if (owner.held != null)
		{
			if (DropHeldOnStart)
			{
				owner.DropHeld();
			}
			else if (PickHeldOnStart)
			{
				owner.PickHeld();
			}
		}
		if (RightHand != 0)
		{
			owner.SetTempHand(RightHand, LeftHand);
		}
		if (owner.IsPC && UseTurbo)
		{
			ActionMode.Adv.SetTurbo();
		}
	}

	public override bool Perform()
	{
		Act.CC.SetAIImmediate(this);
		return false;
	}

	public Status Restart()
	{
		restartCount++;
		if (restartCount >= MaxRestart)
		{
			return Success();
		}
		byte b = restartCount;
		Chara chara = owner;
		Reset();
		SetOwner(chara);
		restartCount = b;
		return status = Status.Running;
	}

	public Status Success(Action action = null)
	{
		status = Status.Success;
		OnSuccess();
		action?.Invoke();
		OnCancelOrSuccess();
		Reset();
		return status;
	}

	public virtual void OnSuccess()
	{
	}

	public bool TryCancel(Card c)
	{
		if (!IsRunning || Enumerator == null)
		{
			return false;
		}
		if (InformCancel)
		{
			if (owner.IsPC)
			{
				owner.Say("cancel_act_pc", owner);
			}
			else if (c != null)
			{
				owner.Say("cancel_act", owner, c);
			}
			else
			{
				owner.Say("cancel_act2", owner);
			}
		}
		Cancel();
		return true;
	}

	public virtual Status Cancel()
	{
		status = Status.Fail;
		if (owner != null && owner.held != null && !owner.IsPC)
		{
			owner.PickHeld();
		}
		OnCancel();
		OnCancelOrSuccess();
		Reset();
		return status;
	}

	public virtual void OnCancel()
	{
	}

	public virtual void OnCancelOrSuccess()
	{
	}

	public virtual bool CanManualCancel()
	{
		if (IsChildRunning)
		{
			return child is AI_Goto;
		}
		return false;
	}

	public Status KeepRunning()
	{
		return Status.Running;
	}

	public void Reset()
	{
		if (owner != null)
		{
			if (child != null)
			{
				child.Reset();
			}
			OnReset();
			if (Enumerator != null)
			{
				Enumerator.Dispose();
				Enumerator = null;
			}
			owner = null;
			child = null;
			restartCount = 0;
			onChildFail = null;
			isFail = null;
		}
	}

	public virtual void OnReset()
	{
	}

	public Status Tick()
	{
		if (owner == null || (isFail != null && isFail()))
		{
			return Cancel();
		}
		if (IsChildRunning)
		{
			switch (child.Tick())
			{
			case Status.Fail:
				if (onChildFail != null)
				{
					return onChildFail();
				}
				return Status.Fail;
			case Status.Running:
				return Status.Running;
			case Status.Success:
				if (owner == null || (isFail != null && isFail()))
				{
					return Cancel();
				}
				break;
			}
		}
		if (Enumerator == null)
		{
			Start();
			if (status != 0)
			{
				return status;
			}
		}
		if (!Enumerator.MoveNext())
		{
			return Success();
		}
		return status;
	}

	public virtual IEnumerable<Status> Run()
	{
		yield return Success();
	}

	public Status TickChild()
	{
		if (child == null || onChildFail == null)
		{
			Debug.Log(child?.ToString() + "/" + onChildFail);
			return Status.Fail;
		}
		if (child.Tick() == Status.Fail)
		{
			if (onChildFail != null)
			{
				return onChildFail();
			}
			return Status.Fail;
		}
		return Status.Running;
	}

	public Status Do(AIAct _seq, Func<Status> _onChildFail = null)
	{
		SetChild(_seq, _onChildFail);
		return TickChild();
	}

	public Status DoGotoInteraction(Point pos, Func<Status> _onChildFail = null)
	{
		if (pos == null || !pos.IsValid)
		{
			return _onChildFail?.Invoke() ?? Cancel();
		}
		SetChild(new AI_Goto(pos, 0, _ignoreConnection: false, _interaction: true), _onChildFail);
		return TickChild();
	}

	public Status DoGoto(Point pos, int dist = 0, bool ignoreConnection = false, Func<Status> _onChildFail = null)
	{
		if (pos == null || !pos.IsValid)
		{
			return _onChildFail?.Invoke() ?? Cancel();
		}
		SetChild(new AI_Goto(pos, dist, ignoreConnection), _onChildFail);
		return TickChild();
	}

	public Status DoGoto(Card card, Func<Status> _onChildFail = null)
	{
		return DoGoto(card, (card.isChara || card.pos.cell.blocked) ? 1 : 0, _onChildFail);
	}

	public Status DoGoto(Card card, int dist, Func<Status> _onChildFail = null)
	{
		if (card != null && card == owner.held)
		{
			return Status.Running;
		}
		if (card == null || !card.ExistsOnMap)
		{
			return _onChildFail?.Invoke() ?? Cancel();
		}
		SetChild(new AI_Goto(card, dist), _onChildFail);
		return TickChild();
	}

	public Status DoGotoSpot(Card card, Func<Status> _onChildFail = null)
	{
		if (card != null && card == owner.held)
		{
			return Status.Running;
		}
		if (card == null || !card.ExistsOnMap)
		{
			return _onChildFail?.Invoke() ?? Cancel();
		}
		Point randomPoint = card.trait.GetRandomPoint();
		int dist = (randomPoint.cell.blocked ? 1 : 0);
		SetChild(new AI_Goto(randomPoint, dist), _onChildFail);
		return TickChild();
	}

	public Status DoGoto<T>(Func<Status> _onChildFail = null) where T : Trait
	{
		return DoGoto(EClass._map.Installed.traits.GetTraitSet<T>().GetRandom()?.owner, _onChildFail);
	}

	public Status DoGotoSpot<T>(Func<Status> _onChildFail = null, bool ignoreAccessType = false) where T : Trait
	{
		Trait random = EClass._map.Installed.traits.GetTraitSet<T>().GetRandom(ignoreAccessType ? null : owner);
		if (random != null)
		{
			DoGoto(random.GetRandomPoint(null, ignoreAccessType ? null : owner), 0, ignoreConnection: false, _onChildFail);
		}
		return _onChildFail?.Invoke() ?? Status.Fail;
	}

	public Status DoGrab(Card card, int num = -1, bool pickHeld = false, Func<Status> _onChildFail = null)
	{
		if (card != null && card == owner.held)
		{
			return Status.Running;
		}
		if (card == null || !card.GetRootCard().ExistsOnMap)
		{
			return Cancel();
		}
		SetChild(new AI_Grab
		{
			target = card,
			num = num,
			pickHeld = pickHeld
		}, _onChildFail);
		return TickChild();
	}

	public Status DoGrab<T>() where T : Trait
	{
		SetChild(new AI_Grab<T>());
		return TickChild();
	}

	public Status DoProgress()
	{
		SetChild(CreateProgress());
		return TickChild();
	}

	public Status DoIdle(int repeat = 3)
	{
		SetChild(new AI_Idle
		{
			maxRepeat = repeat
		}, KeepRunning);
		return TickChild();
	}

	public Status DoWait(int count = 1)
	{
		SetChild(new AI_Wait
		{
			count = count
		}, KeepRunning);
		return TickChild();
	}

	public virtual AIProgress CreateProgress()
	{
		Progress_Custom progress_Custom = new Progress_Custom
		{
			onProgress = delegate
			{
				OnProgress();
			},
			onProgressComplete = OnProgressComplete,
			canProgress = CanProgress,
			onBeforeProgress = OnBeforeProgress
		};
		OnCreateProgress(progress_Custom);
		return progress_Custom;
	}

	public virtual void OnCreateProgress(Progress_Custom p)
	{
	}

	public virtual bool CanProgress()
	{
		return true;
	}

	public virtual void OnBeforeProgress()
	{
	}

	public virtual void OnProgress()
	{
	}

	public virtual void OnProgressComplete()
	{
	}

	public void SimulateZone(int days)
	{
		if (owner.conSuspend == null)
		{
			OnSimulateZone(days);
		}
	}

	public virtual void OnSimulateZone(int days)
	{
	}

	public virtual void OnSimulatePosition()
	{
	}
}
