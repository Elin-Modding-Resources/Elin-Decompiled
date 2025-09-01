public class TileTypeBridge : TileTypeFloor
{
	public override bool IsFloor => false;

	public override bool IsBridge => true;

	public override bool CanBuiltOnWater => true;

	public override int MaxAltitude
	{
		get
		{
			if (!EInput.isShiftDown)
			{
				return 15;
			}
			return 30;
		}
	}
}
