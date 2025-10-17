public class QuestCurry : QuestSequence
{
	public override void OnComplete()
	{
		EClass.player.DropReward(ThingGen.CreateCassette(110), silent: true);
		DropReward(TraitSeed.MakeSeed("redpepper").SetNum(3));
	}
}
