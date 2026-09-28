public class TraitAshland : TraitUniqueChara
{
	public override bool CanInvite => EClass._zone.id == "lothria";

	public override bool ShouldShowQuestIcon()
	{
		if (EClass.game.IsSurvival)
		{
			return false;
		}
		int phase = EClass.game.quests.GetPhase<QuestMain>();
		if ((phase == 0 && EClass.player.dialogFlags.TryGetValue("ash1", 0) == 0) || phase == 200)
		{
			return true;
		}
		return false;
	}
}
