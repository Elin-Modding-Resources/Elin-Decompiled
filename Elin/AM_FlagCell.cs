public class AM_FlagCell : AM_BaseTileSelect
{
	public enum Mode
	{
		flagSnow,
		flagFloat,
		flagWallPillar,
		flagClear,
		flagShadow,
		flagWall
	}

	public Mode mode;

	public override bool IsBuildMode => true;

	public override bool UseSubMenu => true;

	public override bool SubMenuAsGroup => true;

	public override int SubMenuModeIndex => (int)mode;

	public override BaseTileSelector.SelectType selectType
	{
		get
		{
			if (mode != Mode.flagWallPillar)
			{
				return BaseTileSelector.SelectType.Multiple;
			}
			return BaseTileSelector.SelectType.Single;
		}
	}

	public override HitResult HitTest(Point point, Point start)
	{
		switch (mode)
		{
		case Mode.flagWallPillar:
			if (!point.cell.HasWall)
			{
				return HitResult.Default;
			}
			break;
		case Mode.flagShadow:
			if (point.Things.Count == 0)
			{
				return HitResult.Default;
			}
			break;
		case Mode.flagFloat:
			if (start != null && start.cell.isForceFloat != point.cell.isForceFloat)
			{
				return HitResult.Default;
			}
			break;
		case Mode.flagSnow:
			if (start != null && start.cell.isClearSnow != point.cell.isClearSnow)
			{
				return HitResult.Default;
			}
			break;
		case Mode.flagClear:
			if (start != null && start.cell.isClearArea != point.cell.isClearArea)
			{
				return HitResult.Default;
			}
			break;
		}
		return HitResult.Valid;
	}

	public override void OnProcessTiles(Point point, int dir)
	{
		switch (mode)
		{
		case Mode.flagWallPillar:
			point.cell.isToggleWallPillar = !point.cell.isToggleWallPillar;
			break;
		case Mode.flagShadow:
			point.Things.ForeachReverse(delegate(Thing t)
			{
				t.noShadow = !t.noShadow;
			});
			break;
		case Mode.flagFloat:
			point.cell.isForceFloat = !point.cell.isForceFloat;
			break;
		case Mode.flagSnow:
			point.cell.isClearSnow = !point.cell.isClearSnow;
			break;
		case Mode.flagClear:
			point.cell.isClearArea = !point.cell.isClearArea;
			if (point.cell.isClearArea)
			{
				point.Things.ForeachReverse(delegate(Thing t)
				{
					t.Destroy();
				});
			}
			break;
		}
		point.RefreshNeighborTiles();
	}

	public override void OnClickSubMenu(int a)
	{
		mode = a.ToEnum<Mode>();
		base.tileSelector.start = null;
	}

	public override string OnSetSubMenuButton(int a, UIButton b)
	{
		if (a < 5)
		{
			Mode mode = a.ToEnum<Mode>();
			if (a >= 3 && mode != Mode.flagShadow)
			{
				return null;
			}
			return mode.ToString();
		}
		return null;
	}
}
