public class TraitPlatform : TraitTile
{
	public override bool IsHomeItem => true;

	public override TileRow source => EClass.sources.floors.rows[owner.refVal];

	public override string suffix => "-b";

	public override bool CanBeOnlyBuiltInHome => true;
}
