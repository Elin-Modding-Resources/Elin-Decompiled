public class QuestDummy : Quest
{
	public override SourceQuest.Row source => EClass.sources.quests.map["dummy"];

	public override bool CanAbandon => true;
}
