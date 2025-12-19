public class QuestIntoDarkness : QuestProgression
{
	public override string TitlePrefix => "★";

	public override void OnStart()
	{
		EClass.game.quests.Add("demitas_spellwriter").startDate = EClass.world.date.GetRaw() + 1440;
	}

	public override bool CanUpdateOnTalk(Chara c)
	{
		if (phase == 6)
		{
			return EClass._zone.IsPCFaction;
		}
		return false;
	}

	public override void OnComplete()
	{
		AddResident("sorin");
	}
}
