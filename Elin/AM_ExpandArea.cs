public class AM_ExpandArea : AM_BaseTileSelect
{
	public Area area;

	public bool shrink;

	public override bool IsBuildMode => true;

	public override string textHintTitle => area.Name;

	public override string idSound => null;

	public override AreaHighlightMode AreaHihlight => AreaHighlightMode.Edit;

	public override void OnUpdateCursor()
	{
		SetCursorOnMap(CursorSystem.Build);
	}

	public void Activate(Area a, bool _shrink = false)
	{
		area = a;
		shrink = _shrink;
		Activate();
	}

	public override HitResult HitTest(Point point, Point start)
	{
		if (shrink)
		{
			if (area.points.Count <= 1)
			{
				return HitResult.Invalid;
			}
			if (point.area == area)
			{
				return HitResult.Valid;
			}
			if (point.area != null)
			{
				return HitResult.Invalid;
			}
			return base.HitTest(point, start);
		}
		HitResult hitResult = EClass._map.rooms.GetHitResult(point, start);
		if (hitResult != HitResult.Default)
		{
			return hitResult;
		}
		return base.HitTest(point, start);
	}

	public override void OnSelectStart(Point point)
	{
	}

	public override void OnProcessTiles(Point point, int dir)
	{
		if (shrink)
		{
			area.RemovePoint(point);
			SE.Trash();
		}
		else
		{
			area.AddPoint(point.Copy());
			EClass.Sound.Play("build_area");
		}
	}

	public override void OnDeactivate()
	{
		area = null;
		if ((bool)BuildMenu.Instance)
		{
			BuildMenu.Instance.Unselect();
		}
	}

	public override void OnCancel()
	{
		ActionMode.EditArea.Activate();
	}
}
