public class GrowSystemCha : GrowSystemCrop
{
	public override int HarvestStage => 3;

	protected override bool UseGenericFirstStageTile => false;

	public override bool CanReapSeed()
	{
		return base.stage.idx >= 3;
	}

	public override void OnMineObj(Chara c = null)
	{
		if (EClass.rnd(2) == 0)
		{
			TryPick(GrowSystem.cell, ThingGen.Create("grass", EClass.sources.materials.alias["grass"].id), c);
		}
		if (base.stage.idx == HarvestStage)
		{
			PopHarvest(c ?? EClass.pc, ThingGen.Create(idHarvestThing.IsEmpty("leaf_tea")));
		}
	}
}
