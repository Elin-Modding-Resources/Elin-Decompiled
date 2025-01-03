public class QuestSequence : Quest
{
	public SourceQuest.Row originalSource => EClass.sources.quests.map[id];

	public override string idSource => id + ((phase == 0) ? "" : (phase.ToString() ?? ""));

	public override string GetTitle()
	{
		string text = source.GetText("name", returnNull: true);
		if (!text.IsEmpty())
		{
			return text;
		}
		for (int num = phase; num > 0; num--)
		{
			text = EClass.sources.quests.map[id].GetText("name", returnNull: true);
			if (!text.IsEmpty())
			{
				return text;
			}
		}
		return originalSource.GetText();
	}

	public override void OnCompleteTask()
	{
		NextPhase();
	}
}
