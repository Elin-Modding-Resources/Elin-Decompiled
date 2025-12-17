using UnityEngine;

public class TileTypeObjFloat : TileTypeObj
{
	public override bool CanStack => false;

	public override bool IsSkipLowBlock => true;

	public override bool CanBuiltOnBlock => true;

	public override bool UseMountHeight => true;

	public override bool AlwaysShowShadow => true;

	public override void GetMountHeight(ref Vector3 v, Point p, int d, Card target = null)
	{
		v = p.Position();
		v += EClass.screen.tileMap.altitudeFix * target.altitude;
	}
}
