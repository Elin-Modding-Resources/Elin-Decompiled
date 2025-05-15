public class Zone_Exile : Zone_SubTown
{
	public override bool CanSpawnAdv => false;

	public override void OnActivate()
	{
		if (EClass.game.quests.GetPhase<QuestNegotiationDarkness>() == 0)
		{
			EClass.game.quests.Get<QuestNegotiationDarkness>().NextPhase();
		}
	}
}
