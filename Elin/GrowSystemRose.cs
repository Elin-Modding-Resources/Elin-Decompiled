public class GrowSystemRose : GrowSystemFlower
{
	protected override bool UseGenericFirstStageTile => false;

	public override int GetStageTile()
	{
		if (base.stage.idx == HarvestStage && source.alias == "roseflower")
		{
			return source._tiles[GrowSystem.cell.objDir % source._tiles.Length];
		}
		return base.GetStageTile();
	}

	public override bool CanRotate()
	{
		if (base.stage.idx == HarvestStage && source.alias == "roseflower")
		{
			return true;
		}
		return base.CanRotate();
	}
}
