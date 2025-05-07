public class TileTypeBlockOpen : TileTypeBlock
{
	public override bool RepeatBlock => false;

	public override bool UseLowBlock => false;

	public override bool CastShadowSelf => false;

	public override bool CastShadowBack => false;

	public override bool CastAmbientShadow => false;

	public override bool IsSkipFloor => false;

	public override bool IsBlockSight => false;
}
