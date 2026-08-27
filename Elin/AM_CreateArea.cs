public class AM_CreateArea : AM_BaseTileSelect
{
	public Area area;

	public override bool IsBuildMode => true;

	public override string textHintTitle => area.Name;

	public override string idSound => null;

	public override AreaHighlightMode AreaHihlight => AreaHighlightMode.Edit;

	public override void OnUpdateCursor()
	{
		SetCursorOnMap(CursorSystem.Build);
	}

	public void CreateNew(Area a)
	{
		area = Area.Create(a.type.id);
		area.data.name = null;
	}

	public void SetArea(Area a)
	{
		CreateNew(a);
	}

	public override HitResult HitTest(Point point, Point start)
	{
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
		area.SetRandomName();
		EClass._map.rooms.AddArea(area, point);
		EClass.Sound.Play("build_area");
	}

	public override void OnAfterProcessTiles(Point start, Point end)
	{
		CreateNew(area);
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
