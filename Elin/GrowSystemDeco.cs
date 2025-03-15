public class GrowSystemDeco : GrowSystem
{
	public override bool CanLevelSeed => false;

	protected override bool WitherOnLastStage => false;

	protected override bool UseGenericFirstStageTile => false;

	public override int StageLength => 2;

	public override bool CanReapSeed()
	{
		return true;
	}

	public override void OnExceedLastStage()
	{
		SetStage(1);
	}

	public override int GetStageTile()
	{
		return source._tiles[GrowSystem.cell.objDir % source._tiles.Length];
	}
}
