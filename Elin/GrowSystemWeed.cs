public class GrowSystemWeed : GrowSystem
{
	protected override bool UseGenericFirstStageTile => false;

	public override int HarvestStage => 3;

	public override int AutoMineStage => 2;

	public override bool CanReapSeed()
	{
		return base.stage.idx >= 2;
	}

	public override void OnMineObj(Chara c = null)
	{
		PopHarvest(c ?? EClass.pc, ThingGen.Create("grass", EClass.sources.materials.alias["grass"].id), EClass.rnd(5));
		base.OnMineObj(c);
	}

	public override void OnSetObj()
	{
		GrowSystem.cell.objDir = EClass.rnd(source.tiles.Length);
	}

	public override int GetStageTile()
	{
		return source._tiles[GrowSystem.cell.objDir % source._tiles.Length] + GrowSystem.currentStage.idx - 2;
	}
}
