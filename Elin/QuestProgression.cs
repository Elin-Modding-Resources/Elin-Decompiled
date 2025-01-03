public class QuestProgression : QuestSequence
{
	public override string TitlePrefix => "☆";

	public override string GetDetail(bool onJournal = false)
	{
		string text = source.GetDetail().Split('|').TryGet(onJournal ? 1 : 0);
		string text2 = GetTextProgress();
		if (!text2.IsEmpty())
		{
			text2 = "\n\n" + text2;
		}
		return GameLang.Convert(text) + text2;
	}
}
