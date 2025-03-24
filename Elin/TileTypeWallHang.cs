using UnityEngine;

public class TileTypeWallHang : TileTypeObj
{
	public static bool[] DIRS = new bool[4];

	public override string LangPlaceType => "place_WallMount";

	public override bool CanStack => true;

	public override bool IsBlockMount => true;

	public override bool UseMountHeight => true;

	public override bool UseHangZFix => true;

	public virtual bool NoBackSide => false;

	public override int GetDesiredDir(Point p, int d)
	{
		if (EClass.scene.actionMode != null && EClass.scene.actionMode.IsRoofEditMode())
		{
			return -1;
		}
		Cell cell = p.cell;
		DIRS[0] = cell.Back.sourceBlock.tileType.IsMountBlock && (!cell.Back.sourceBlock.tileType.IsWallOrFence || cell.Back.blockDir != 1);
		DIRS[1] = cell.Left.sourceBlock.tileType.IsMountBlock && (!cell.Left.sourceBlock.tileType.IsWallOrFence || cell.Left.blockDir != 0);
		if (NoBackSide)
		{
			DIRS[2] = false;
			DIRS[3] = false;
		}
		else
		{
			DIRS[2] = cell.Front.sourceBlock.tileType.IsMountBlock && (!cell.Front.sourceBlock.tileType.IsWallOrFence || cell.Front.blockDir != 1);
			DIRS[3] = cell.Right.sourceBlock.tileType.IsMountBlock && (!cell.Right.sourceBlock.tileType.IsWallOrFence || cell.Right.blockDir != 0);
		}
		if (d == -1 || !DIRS[d])
		{
			if (d == -1)
			{
				d = 0;
			}
			for (int i = 0; i < 4; i++)
			{
				if (DIRS[(i + d) % 4])
				{
					return (i + d) % 4;
				}
			}
		}
		if (DIRS[d])
		{
			return d;
		}
		return -1;
	}

	public override void GetMountHeight(ref Vector3 v, Point p, int d, Card target = null)
	{
		if (d == 0 && p.z < EClass._map.Size - 1)
		{
			Point.shared2.Set(p.x, p.z + 1);
			Vector3 vector = Point.shared2.Position();
			v.x = vector.x;
			v.y = vector.y + Point.shared2.sourceBlock.tileType.MountHeight;
			v.z = vector.z;
			if (Point.shared2.HasWallOrFence)
			{
				v += EClass.screen.tileMap.wallHangFix[0];
			}
			v += EClass.screen.tileMap.altitudeFix * target.altitude;
		}
		else if (d == 1 && p.x > 0)
		{
			Point.shared2.Set(p.x - 1, p.z);
			Vector3 vector2 = Point.shared2.Position();
			v.x = vector2.x;
			v.y = vector2.y + Point.shared2.sourceBlock.tileType.MountHeight;
			v.z = vector2.z;
			if (Point.shared2.HasWallOrFence)
			{
				v += EClass.screen.tileMap.wallHangFix[1];
			}
			v += EClass.screen.tileMap.altitudeFix * target.altitude;
		}
		else if (d == 2 && p.z > 0)
		{
			Point.shared2.Set(p.x, p.z - 1);
			Vector3 vector3 = p.Position();
			v.x = vector3.x;
			v.y = vector3.y + p.sourceBlock.tileType.MountHeight;
			v.z = vector3.z;
			if (Point.shared2.HasWallOrFence)
			{
				v += EClass.screen.tileMap.wallHangFix[2];
			}
			v += EClass.screen.tileMap.altitudeFix * target.altitude;
		}
		else if (d == 3 && p.x < EClass._map.Size - 1)
		{
			Point.shared2.Set(p.x + 1, p.z);
			Vector3 vector4 = p.Position();
			v.x = vector4.x;
			v.y = vector4.y + p.sourceBlock.tileType.MountHeight;
			v.z = vector4.z;
			if (Point.shared2.HasWallOrFence)
			{
				v += EClass.screen.tileMap.wallHangFix[3];
			}
			v += EClass.screen.tileMap.altitudeFix * target.altitude;
		}
		else
		{
			v.y += p.sourceBlock.tileType.MountHeight;
		}
		if (target != null)
		{
			v.z += target.Pref.z;
		}
	}

	protected override HitResult HitTest(Point pos)
	{
		if (GetDesiredDir(pos, -1) == -1)
		{
			return HitResult.Invalid;
		}
		return base.HitTest(pos);
	}
}
