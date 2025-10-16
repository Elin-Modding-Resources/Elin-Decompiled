public class QuestCurry : QuestSequence
{
	public override void OnComplete()
	{
		DropReward(TraitSeed.MakeSeed("redpepper").SetNum(3));
	}
}
