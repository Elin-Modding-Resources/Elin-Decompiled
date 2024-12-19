public class TraitObj : TraitTile
{
	public override TileRow source => EClass.sources.objs.rows[owner.refVal];
}
