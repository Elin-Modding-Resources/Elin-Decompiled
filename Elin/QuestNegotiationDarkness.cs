public class QuestNegotiationDarkness : QuestProgression
{
	public override string TitlePrefix => "★";

	public override bool CanUpdateOnTalk(Chara c)
	{
		return false;
	}
}
