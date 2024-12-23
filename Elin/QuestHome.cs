public class QuestHome : QuestSequence
{
	public const int Started = 0;

	public const int AfterReadDeed = 1;

	public const int AfterReportAsh = 2;

	public override string TitlePrefix => "★";

	public override bool CanAutoAdvance => false;

	public override void OnChangePhase(int a)
	{
		if (a == 2)
		{
			track = false;
		}
	}
}
