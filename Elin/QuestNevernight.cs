using Newtonsoft.Json;

public class QuestNevernight : QuestProgression
{
	[JsonProperty]
	public int dateStarted;

	public override string TitlePrefix => "★";

	public override void OnStart()
	{
		dateStarted = EClass.world.date.GetRaw();
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
	}
}
