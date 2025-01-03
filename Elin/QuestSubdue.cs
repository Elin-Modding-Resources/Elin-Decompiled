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
		ZoneEventSubdue @event = EClass._zone.events.GetEvent<ZoneEventSubdue>();
		if (@event == null)
		{
			return "";
		}
		return "progressHunt".lang((@event.max - @event.enemies.Count).ToString() ?? "", @event.max.ToString() ?? "");
	}
}
