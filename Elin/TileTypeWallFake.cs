public class TileTypeWallFake : TileTypeWallOpen
{
	public override bool IsBlockPass => false;

	public override bool UseLowBlock => false;
}
