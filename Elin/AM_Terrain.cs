using UnityEngine;

public class AM_Terrain : AM_BaseTerrain
{
	public enum Mode
	{
		Flatten,
		Up,
		Down
	}

	public Mode mode;

	public override int SubMenuModeIndex => (int)mode;

	public override bool FixedPointer => true;

	public override int TopHeight(Point p)
	{
		return p.cell.height;
	}

	public override string OnSetSubMenuButton(int a, UIButton b)
	{
		if (a < 3)
		{
			return "terrain" + a.ToEnum<Mode>();
		}
		return null;
	}

	public override void OnClickSubMenu(int a)
	{
		mode = a.ToEnum<Mode>();
	}

	public override void OnProcessTiles(Point point, int dir)
	{
		if (timer < 0.1f)
		{
			return;
		}
		if (lastPoint != null)
		{
			point = lastPoint;
		}
		else
		{
			lastPoint = new Point(point);
		}
		timer = 0f;
		Cell cell = point.cell;
		EClass._map.ForeachSphere(point.x, point.z, brushRadius, delegate(Point p)
		{
			int num = p.Distance(point);
			if (mode == Mode.Flatten)
			{
				if (p.cell.IsFloorWater != cell.IsFloorWater)
				{
					return;
				}
				int num2 = p.cell.height - cell.height;
				if (!EInput.isShiftDown)
				{
					num2 = ((num2 >= 0) ? Mathf.Clamp(brushRadius - num, 1, num2) : Mathf.Clamp((brushRadius - num) * -1, num2, -1));
				}
				p.cell.height -= (byte)num2;
				if (p.cell._bridge != 0)
				{
					if (p.cell.bridgeHeight - num2 < 0)
					{
						p.cell.bridgeHeight = 0;
					}
					else
					{
						p.cell.bridgeHeight -= (byte)num2;
					}
				}
			}
			else
			{
				int num3 = brushRadius - num;
				if (EInput.isShiftDown)
				{
					num3 = 1;
				}
				if (mode == Mode.Down)
				{
					num3 *= -1;
				}
				if (p.cell.height + num3 < 0)
				{
					num3 = -p.cell.height;
				}
				else if (p.cell.height + num3 > EClass.setting.maxGenHeight)
				{
					num3 = EClass.setting.maxGenHeight - p.cell.height;
				}
				p.cell.height += (byte)num3;
				if (p.cell._bridge != 0)
				{
					if (p.cell.bridgeHeight + num3 < 0)
					{
						p.cell.bridgeHeight = 0;
					}
					else
					{
						p.cell.bridgeHeight += (byte)num3;
					}
				}
			}
			p.RefreshNeighborTiles();
			if (p.cell.room != null)
			{
				p.cell.room.SetDirty();
			}
		});
	}
}
