using System;
using UnityEngine;

public class BaseTileSelector : EMono
{
	public enum SelectType
	{
		Single,
		Multiple,
		None
	}

	public enum HitType
	{
		None,
		Default,
		Floor,
		Block,
		Inspect
	}

	public enum BoxType
	{
		Box,
		Fence
	}

	public enum ProcessMode
	{
		Render,
		Prpcess,
		Summary
	}

	public HitType inspectHitType;

	public bool inspectorHighlight;

	public HitSummary summary = new HitSummary();

	[NonSerialized]
	public Point start;

	[NonSerialized]
	public Point temp = new Point();

	[NonSerialized]
	public Point lastPoint = new Point();

	[NonSerialized]
	public Point lastClickPoint = new Point();

	[NonSerialized]
	public bool processing;

	[NonSerialized]
	public bool autoClick;

	public bool firstInMulti;

	public ActionMode mode => EMono.scene.actionMode;

	public SelectType selectType => mode.selectType;

	public HitType hitType => mode.hitType;

	public BoxType boxType => mode.boxType;

	public int hitW => mode.hitW;

	public int hitH => mode.hitH;

	public bool multisize
	{
		get
		{
			if (hitW == 1)
			{
				return hitH != 1;
			}
			return true;
		}
	}

	public int Width
	{
		get
		{
			if (Scene.HitPoint.IsValid)
			{
				if (start != null)
				{
					return Mathf.Abs(Scene.HitPoint.x - start.x) + 1;
				}
				return 1;
			}
			return 0;
		}
	}

	public int Height
	{
		get
		{
			if (Scene.HitPoint.IsValid)
			{
				if (start != null)
				{
					return Mathf.Abs(Scene.HitPoint.z - start.z) + 1;
				}
				return 1;
			}
			return 0;
		}
	}

	public void OnChangeActionMode()
	{
		start = null;
		RefreshMouseInfo(force: true);
	}

	public virtual void OnRenderTile(Point point, HitResult result, bool shouldHide)
	{
	}

	public void OnUpdate()
	{
		Point hitPoint = Scene.HitPoint;
		if (!ActionMode.Adv.IsActive)
		{
			RefreshMouseInfo(force: true);
		}
		if (EInput.leftMouse.pressedLong && mode.AllowAutoClick)
		{
			autoClick = true;
		}
		if (autoClick && !EInput.leftMouse.pressing)
		{
			autoClick = false;
			if (start != null)
			{
				mode.OnSelectEnd(cancel: true);
				start = null;
				RefreshSummary();
				RefreshMouseInfo(force: true);
			}
		}
		if (autoClick && hitPoint.Distance(lastClickPoint) > 1 && selectType == SelectType.Multiple && start != null)
		{
			start = hitPoint.Copy();
			mode.OnSelectStart(start);
			RefreshMouseInfo(force: true);
			lastClickPoint.Set(hitPoint);
		}
		if (((EInput.rightScroll && EInput.rightMouse.clicked) || (!EInput.rightScroll && EInput.rightMouse.down) || Input.GetKeyDown(KeyCode.Escape)) && !EMono.ui.wasActive)
		{
			if (selectType == SelectType.Multiple && start != null)
			{
				mode.OnSelectEnd(cancel: true);
				start = null;
				RefreshSummary();
				RefreshMouseInfo(force: true);
			}
			else
			{
				mode.OnCancel();
			}
			EMono.ui.hud.hint.UpdateText();
		}
		if (!hitPoint.IsValid || EMono.ui.isPointerOverUI || EMono.ui.wasActive || mode.selectType == SelectType.None || !EMono.pc.ai.ShowCursor || (UIDragPanel.dragging && Input.GetMouseButton(0)))
		{
			return;
		}
		if (mode.hitType != 0)
		{
			if (mode.IsFillMode())
			{
				start = null;
				ProcessFillTiles(hitPoint, ProcessMode.Render);
			}
			else
			{
				ProcessTiles(start ?? hitPoint, hitPoint, ProcessMode.Render);
			}
		}
		if (start == null && (mode.AreaHihlight == AreaHighlightMode.Edit || mode.AreaHihlight == AreaHighlightMode.Sim) && hitPoint.area != null)
		{
			hitPoint.area.OnHoverArea(EMono.screen.guide.passArea);
		}
		if ((!autoClick && !EInput.leftMouse.down && (!mode.ContinuousClick || !EInput.leftMouse.pressing)) || !mode.CanSelectTile)
		{
			return;
		}
		if (start != null || hitPoint.IsSeen)
		{
			if (mode.IsFillMode())
			{
				TryProcessTiles(hitPoint);
			}
			else if (selectType == SelectType.Single)
			{
				HitResult hitResult = mode._HitTest(temp, start);
				if ((hitResult == HitResult.Valid || hitResult == HitResult.Warning) && mode.CanProcessTiles())
				{
					TryProcessTiles(hitPoint);
				}
				else if (!autoClick)
				{
					SE.Beep();
				}
			}
			else
			{
				if (start == null)
				{
					start = hitPoint.Copy();
					mode.OnSelectStart(start);
					RefreshMouseInfo(force: true);
					if (autoClick)
					{
						TryProcessTiles(hitPoint);
					}
				}
				else
				{
					TryProcessTiles(hitPoint);
				}
				if (autoClick)
				{
					start = hitPoint.Copy();
					mode.OnSelectStart(start);
					RefreshMouseInfo(force: true);
				}
			}
		}
		lastClickPoint.Set(hitPoint);
		EMono.ui.hud.hint.UpdateText();
	}

	public void TryProcessTiles(Point _end)
	{
		if (EInput.skipFrame > 0)
		{
			return;
		}
		if (!mode.CanProcessTiles())
		{
			if (!EMono.screen.tileSelector.autoClick)
			{
				SE.Beep();
			}
			return;
		}
		Point point = _end.Copy();
		ActionMode actionMode = mode;
		mode.OnBeforeProcessTiles();
		if (mode.IsFillMode())
		{
			ProcessFillTiles(point, ProcessMode.Prpcess);
		}
		else if (start == null)
		{
			mode.OnProcessTiles(point, -1);
		}
		else
		{
			ProcessTiles(start, point, ProcessMode.Prpcess);
			mode.OnSelectEnd(cancel: false);
		}
		if (mode == actionMode)
		{
			mode.OnAfterProcessTiles(start, point);
			start = null;
			ExecuteSummary();
			RefreshMouseInfo(force: true);
			mode.OnFinishProcessTiles();
		}
	}

	public void ProcessTiles(Point start, Point end, ProcessMode processMode)
	{
		new Point();
		int num = ((start.x > end.x) ? start.x : end.x);
		int num2 = ((start.z < end.z) ? start.z : end.z);
		int num3 = ((start.x > end.x) ? end.x : start.x);
		int num4 = ((start.z < end.z) ? end.z : start.z);
		int num5 = num - num3 + 1;
		int num6 = num4 - num2 + 1;
		BoxType boxType = this.boxType;
		int num7 = -1;
		int num8 = 0;
		firstInMulti = false;
		if (multisize && selectType == SelectType.Single && processMode != ProcessMode.Prpcess)
		{
			num3 -= hitW - 1;
			num4 += hitH - 1;
			num5 += hitW - 1;
			num6 += hitH - 1;
		}
		if (processMode == ProcessMode.Prpcess)
		{
			processing = true;
		}
		for (int num9 = num; num9 > num3 - 1; num9--)
		{
			for (int i = num2; i < num4 + 1; i++)
			{
				if (boxType == BoxType.Fence && (num5 > 1 || num6 > 1))
				{
					num7 = ((num5 <= num6) ? 1 : 0);
					if ((num7 == 0 && i != start.z) || (num7 == 1 && num9 != start.x))
					{
						continue;
					}
				}
				temp.Set(num9, i);
				if (start.x != num9 || start.z != i)
				{
					_ProcessTile(start, temp, processMode, num7);
				}
			}
			num8++;
			if (num8 > 30000)
			{
				Debug.Log(num8);
				break;
			}
		}
		firstInMulti = true;
		_ProcessTile(start, start, processMode, num7);
		processing = false;
	}

	public void ProcessFillTiles(Point start, ProcessMode processMode)
	{
		EMono._map.ForeachPoint(delegate(Point dest)
		{
			if (!start.Equals(dest))
			{
				_ProcessTile(start, dest, processMode);
			}
		});
		_ProcessTile(start, start, processMode);
	}

	private void _ProcessTile(Point start, Point dest, ProcessMode processMode, int dir = -1)
	{
		if (!dest.IsValid)
		{
			return;
		}
		HitResult hitResult = mode._HitTest(dest, start);
		switch (processMode)
		{
		case ProcessMode.Render:
			mode.OnRenderTile(dest, hitResult, dir);
			firstInMulti = false;
			break;
		case ProcessMode.Prpcess:
			if (hitResult == HitResult.Valid || hitResult == HitResult.Warning)
			{
				mode.OnProcessTiles(dest, dir);
			}
			break;
		case ProcessMode.Summary:
			mode.OnRefreshSummary(dest, hitResult, summary);
			break;
		}
	}

	public bool IsInRange(int x, int z, Point end)
	{
		if (start.x > end.x)
		{
			if (x > start.x || x < end.x)
			{
				return false;
			}
		}
		else if (x < start.x || x > end.x)
		{
			return false;
		}
		if (start.z > end.z)
		{
			if (z > start.z || z < end.z)
			{
				return false;
			}
		}
		else if (z < start.z || z > end.z)
		{
			return false;
		}
		return true;
	}

	public void RefreshMouseInfo(bool force = false)
	{
		bool enable = true;
		Point hitPoint = Scene.HitPoint;
		if (EMono.ui.IsActive || EMono.ui.isPointerOverUI || !mode.enableMouseInfo || !hitPoint.IsValid)
		{
			enable = false;
		}
		else if (!hitPoint.Equals(lastPoint) || force)
		{
			if (start != null && !hitPoint.Equals(lastPoint))
			{
				SE.Play("highlight3");
			}
			RefreshSummary();
			string text = "";
			int num = 0;
			if (start != null)
			{
				int num2 = Mathf.Abs(hitPoint.x - start.x) + 1;
				int num3 = Mathf.Abs(hitPoint.z - start.z) + 1;
				if (!hitPoint.IsValid)
				{
					num2 = 0;
					num3 = 0;
				}
				text = text + "(" + num2 + " x " + num3 + ")";
			}
			else if (selectType == SelectType.Multiple)
			{
				text += "selectRange".lang();
			}
			if (selectType == SelectType.Single)
			{
				num = mode.CostMoney;
			}
			else if (start != null)
			{
				num = summary.money;
			}
			EMono.ui.mouseInfo.textCost.SetActive(num != 0);
			if (num != 0)
			{
				EMono.ui.mouseInfo.textCost.text = num.ToString() ?? "";
			}
			if (mode.TopHeight(hitPoint) != -1)
			{
				text += "hintAltitude".lang(mode.TopHeight(hitPoint).ToString() ?? "");
			}
			if (EMono.scene.actionMode == ActionMode.Picker)
			{
				AM_Picker.Result result = ActionMode.Picker.Test(hitPoint);
				if (result.IsValid)
				{
					text += result.GetText();
				}
			}
			if (mode.IsRoofEditMode())
			{
				text = text + Environment.NewLine + "roofEdit".lang();
			}
			else if (mode.IsFillMode())
			{
				text = text + Environment.NewLine + "fillMode".lang();
			}
			text += "\n";
			EMono.ui.mouseInfo.text.SetText(mode.SetMouseInfo(ref text));
		}
		EMono.ui.mouseInfo.SetActive(enable);
		lastPoint.Set(hitPoint);
	}

	public void ExecuteSummary()
	{
		if ((bool)BuildMenu.Instance)
		{
			summary.Execute();
			BuildMenu.Instance.info1.Refresh();
			RefreshSummary();
			mode.SEExecuteSummary();
		}
	}

	public void RefreshSummary()
	{
		Point hitPoint = Scene.HitPoint;
		summary.Clear();
		ProcessTiles(start ?? hitPoint, hitPoint, ProcessMode.Summary);
		if ((bool)BuildMenu.Instance)
		{
			BuildMenu.Instance.info1.RefreshBalance();
		}
	}
}
