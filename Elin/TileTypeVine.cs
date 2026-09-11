using UnityEngine;

public class TileTypeVine : TileTypeObj
{
	public override bool CanStack => true;

	public override bool IsBlockMount => true;

	public override bool UseMountHeight => true;

	public override bool UseHangZFix => true;

	public override int GetDesiredDir(Point p, int d)
	{
		if (p.cell.Back.HasFullBlockOrWallOrFence)
		{
			return 0;
		}
		if (p.cell.Left.HasFullBlockOrWallOrFence)
		{
			return 1;
		}
		if (p.cell.Front.HasFullBlockOrWallOrFence)
		{
			return 2;
		}
		if (p.cell.Right.HasFullBlockOrWallOrFence)
		{
			return 3;
		}
		return -1;
	}

	public override void GetMountHeight(ref Vector3 v, Point p, int d, Card target = null)
	{
		Point.shared2.Set(p.x, p.z);
		Vector3 vector = Point.shared2.Position();
		v.x = vector.x;
		v.y = vector.y + Point.shared2.sourceBlock.tileType.MountHeight;
		v.z = vector.z;
		v += EClass.screen.tileMap.altitudeFix * target.altitude;
		if (target != null)
		{
			v.z += target.Pref.z;
		}
	}
}
