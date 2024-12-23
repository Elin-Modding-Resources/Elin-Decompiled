public class QuestMain : QuestSequence
{
	public const int Started = 0;

	public const int AfterMeetAsh = 100;

	public const int AfterReadDeed = 200;

	public const int AfterReportAsh = 250;

	public const int AfterMeetFarris = 300;

	public const int AfterNymelle = 600;

	public const int AfterAshLeaveHome = 700;

	public override string TitlePrefix => "★";

	public override bool TrackOnStart => false;

	public override string idSource => id;

	public static int Phase => EClass.game.quests.Get<QuestMain>()?.phase ?? 0;

	public override bool CanAutoAdvance => false;
}
