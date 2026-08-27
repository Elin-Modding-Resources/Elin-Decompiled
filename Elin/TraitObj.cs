public class TraitObj : TraitTile
{
	public override TileRow source => EClass.sources.objs[owner.refVal];

	public override bool CanBeOnlyBuiltInHome => true;
}
