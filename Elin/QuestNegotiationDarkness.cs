public class QuestNegotiationDarkness : QuestProgression
{
	public const int AfterTalkBicerin = 2;

	public const int AfterFindDuponne = 3;

	public const int AfterTakeCareDuponne = 4;

	public override string TitlePrefix => "★";

	public override bool CanUpdateOnTalk(Chara c)
	{
		return false;
	}

	public override void ShowCompleteText()
	{
		Msg.Say("completeQuest", GetTitle());
	}
}
