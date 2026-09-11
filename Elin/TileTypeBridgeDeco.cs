public class TileTypeBridgeDeco : TileTypeFloor
{
	public override bool IsBridge => true;

	public override BaseTileSelector.SelectType SelectType => BaseTileSelector.SelectType.Single;

	public override bool CastShadowSelf => false;
}
