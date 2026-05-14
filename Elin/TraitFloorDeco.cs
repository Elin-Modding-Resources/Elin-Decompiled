public class TraitFloorDeco : TraitTile
{
	public override bool ConsumeOnUse => false;

	public override TileRow source => EClass.sources.decos.rows[owner.refVal];
}
