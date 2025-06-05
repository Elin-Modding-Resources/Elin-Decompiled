public class GrowSystemSeaweed : GrowSystemWheat
{
	public override int HarvestStage => -1;

	public override bool GrowOnLand => false;

	public override bool GrowUndersea => true;

	public override bool NeedSunlight => false;

	public override bool GenerateStraw => false;

	public override bool CanReapSeed()
	{
		return base.stage.idx >= 1;
	}

	public override void OnMineObj(Chara c = null)
	{
		if (IsWithered() || base.stage.idx == 0)
		{
			base.OnMineObj(c);
			return;
		}
		int num = 1 + EClass.rnd(base.stage.idx * 2) + ((base.stage.idx >= 2) ? 1 : 0);
		PopHarvest(c, "seaweed2", num);
	}
}
