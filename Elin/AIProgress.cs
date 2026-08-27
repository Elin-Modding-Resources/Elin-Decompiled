using System.Collections.Generic;

public class AIProgress : AIAct
{
	public int progress;

	public override bool IsAutoTurn => true;

	public override int MaxProgress => 20;

	public override int CurrentProgress => progress;

	public virtual int Interval => 2;

	public virtual string TextOrbit => progress * 100 / MaxProgress + "%";

	public virtual string TextHint => null;

	public override bool CancelWhenMoved => true;

	public override IEnumerable<Status> Run()
	{
		if (owner.IsPC)
		{
			ActionMode.Adv.SetTurbo();
		}
		while (true)
		{
			OnBeforeProgress();
			if (!CanProgress())
			{
				yield return Cancel();
			}
			if (progress == 0)
			{
				OnProgressBegin();
			}
			if (status != Status.Running)
			{
				yield return status;
			}
			if (progress % Interval == 0)
			{
				OnProgress();
			}
			progress++;
			if (status != Status.Running)
			{
				yield return status;
			}
			if (progress >= MaxProgress)
			{
				OnProgressComplete();
				yield return Success();
			}
			yield return Status.Running;
		}
	}

	public virtual void OnProgressBegin()
	{
	}

	public void CompleteProgress()
	{
		progress = MaxProgress;
	}
}
