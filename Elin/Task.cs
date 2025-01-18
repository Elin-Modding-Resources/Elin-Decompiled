using Newtonsoft.Json;

public class Task : AIAct
{
	public TaskList taskList;

	public bool isDestroyed;

	public int nextTry;

	[JsonProperty]
	public bool suspended;

	public override TargetType TargetType => TargetType.Ground;

	public virtual HitResult GetHitResult()
	{
		return HitResult.Valid;
	}

	public override void OnSuccess()
	{
		Destroy();
	}

	public void TryLater(int min = 30)
	{
		nextTry = EClass.game.sessionMin + min;
	}

	public Status Destroy()
	{
		if (status == Status.Running)
		{
			status = Status.Fail;
		}
		if (isDestroyed)
		{
			return status;
		}
		isDestroyed = true;
		if (taskList != null)
		{
			taskList.Remove(this);
		}
		OnDestroy();
		return status;
	}

	public virtual void OnDestroy()
	{
	}

	public override Status Cancel()
	{
		TryLater();
		return base.Cancel();
	}

	public override void OnReset()
	{
		if (!isDestroyed && taskList != null)
		{
			taskList.SetAstLastItem(this);
		}
	}

	public override bool CanProgress()
	{
		if (isDestroyed)
		{
			return false;
		}
		HitResult hitResult = GetHitResult();
		if (hitResult != HitResult.Valid)
		{
			return hitResult == HitResult.Warning;
		}
		return true;
	}

	public bool CanPerformTask(Chara chara, int radius)
	{
		if (!suspended)
		{
			return _CanPerformTask(chara, radius);
		}
		return false;
	}

	public virtual bool _CanPerformTask(Chara chara, int radius)
	{
		return true;
	}

	public virtual void OnAdd()
	{
	}

	public void ToggleSuspend()
	{
		suspended = !suspended;
		if (suspended && IsRunning)
		{
			Cancel();
		}
	}
}
