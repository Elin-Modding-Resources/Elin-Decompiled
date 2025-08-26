using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class TaskPoint : Task, IInspect
{
	private static Point tempPos = new Point();

	[JsonProperty]
	public Point pos = new Point();

	public bool isRepeated;

	public override bool HasProgress => false;

	public virtual int destDist => 0;

	public virtual bool destIgnoreConnection => false;

	public virtual bool isBlock => false;

	public virtual bool Loop => false;

	public bool CanInspect => !isDestroyed;

	public string InspectName => base.source.GetName();

	public Point InspectPoint => pos;

	public Vector3 InspectPosition => pos.Position();

	public override bool CanManualCancel()
	{
		if (base.IsChildRunning)
		{
			return child is AI_Goto;
		}
		return false;
	}

	public override bool CanPerform()
	{
		if (owner == null)
		{
			owner = Act.CC;
		}
		tempPos.Set(pos);
		pos.Set(Act.TP);
		HitResult hitResult = GetHitResult();
		pos.Set(tempPos);
		if (hitResult != HitResult.Valid)
		{
			return hitResult == HitResult.Warning;
		}
		return true;
	}

	public override bool _CanPerformTask(Chara chara, int radius)
	{
		if (radius != -1 && chara.pos.Distance(pos) > radius)
		{
			return false;
		}
		if (pos.cell.isSurrounded)
		{
			return false;
		}
		return true;
	}

	public override IEnumerable<Status> Run()
	{
		if (owner.IsPC && AM_Adv.actCount > 0)
		{
			while (EInput.rightMouse.pressing)
			{
				pos.Set(Act.TP.Set(ActionMode.Adv.mouseTarget.pos));
				if (CanPerform())
				{
					break;
				}
				yield return Status.Running;
			}
			pos.Set(Act.TP.Set(ActionMode.Adv.mouseTarget.pos));
			if (!CanPerform() || !EInput.rightMouse.pressing || (HasProgress && !CanProgress()))
			{
				yield return Destroy();
			}
		}
		do
		{
			if (owner.pos.Distance(pos) > destDist)
			{
				yield return DoGoto(pos, destDist, destIgnoreConnection);
				if (owner.pos.Distance(pos) > destDist)
				{
					Debug.Log(AM_Adv.actCount + "/" + pos?.ToString() + "/" + owner.pos);
					Debug.Log(destDist + "/" + owner.pos.Distance(pos));
					yield return Destroy();
				}
			}
			if (!CanProgress())
			{
				yield return Destroy();
			}
			if (HasProgress)
			{
				yield return DoProgress();
			}
			else
			{
				yield return KeepRunning();
				OnProgressComplete();
			}
			if (owner != null && owner.IsPC)
			{
				AM_Adv.actCount++;
			}
			if (child != null && child.status == Status.Fail)
			{
				yield return Cancel();
			}
		}
		while (Loop);
	}

	public void OnInspect()
	{
	}

	public void WriteNote(UINote n, Action<UINote> onWriteNote = null, IInspect.NoteMode mode = IInspect.NoteMode.Default, Recipe recipe = null)
	{
		n.Clear();
		n.AddHeaderCard(Name);
		n.Build();
	}
}
