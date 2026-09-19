using UnityEngine;

public class TraitDoorBigL : TraitDoorBig
{
	public override TileMode tileMode => base.tileMode;

	public override bool UseAltTiles => IsOpen();

	public override bool UsePositionFix => IsOpen();

	public override void PositionFix(ref Vector3 v)
	{
		v += EClass.setting.render.bigdoorFix[(owner.dir != 0 && owner.dir != 2) ? 1 : 0];
	}
}
