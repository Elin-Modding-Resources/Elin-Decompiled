public class TileTypePillar : TileTypeBaseBlock
{
	public override bool RepeatBlock => true;

	public override bool ForceRpeatBlock => true;

	public override int MaxAltitude => 7;

	public override bool AltitudeAsDir => true;

	public override bool IsBlockPass => true;

	public override bool IsOccupyCell => false;

	public override bool UseLowBlock => false;

	public override BlockRenderMode blockRenderMode => BlockRenderMode.Pillar;
}
