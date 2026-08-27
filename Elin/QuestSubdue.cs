public class QuestSubdue : QuestInstance
{
	public override string IdZone => "instance_arena2";

	public override bool FameContent => true;

	public override int BaseMoney => source.money + EClass.curve(DangerLv, 20, 15) * 10;

	public override ZoneEventQuest CreateEvent()
	{
		return new ZoneEventSubdue();
	}

	public override ZoneInstanceRandomQuest CreateInstance()
	{
		return new ZoneInstanceSubdue();
	}

	public override string GetTextProgress()
	{
		ZoneEventSubdue zoneEventSubdue = EClass._zone.events.GetEvent<ZoneEventSubdue>();
		if (zoneEventSubdue == null)
		{
			return "";
		}
		return "progressHunt".lang((zoneEventSubdue.max - zoneEventSubdue.enemies.Count).ToString() ?? "", zoneEventSubdue.max.ToString() ?? "");
	}
}
