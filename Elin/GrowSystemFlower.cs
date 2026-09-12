public class GrowSystemFlower : GrowSystemPlant
{
	public override int HarvestStage => 3;

	public override bool CanReapSeed()
	{
		return base.stage.idx >= 2;
	}

	public override void OnMineObj(Chara c = null)
	{
		if (EClass.rnd(2) == 0)
		{
			TryPick(GrowSystem.cell, ThingGen.Create("grass", EClass.sources.materials.alias["grass"].id), c);
		}
		if (base.Grown || base.Mature)
		{
			PopHarvest(c ?? EClass.pc, ThingGen.Create(idHarvestThing.IsEmpty("flower")));
		}
	}

	public override void OnSetObj()
	{
		string alias = source.alias;
		if (alias == "tulip_red" || alias == "roseflower")
		{
			GrowSystem.cell.DyeObj("rubinus");
			GrowSystem.cell.objDir = EClass.rnd(4);
		}
	}
}
