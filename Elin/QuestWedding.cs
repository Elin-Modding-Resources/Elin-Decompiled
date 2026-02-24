public class QuestWedding : QuestInstance
{
	public override DifficultyType difficultyType => DifficultyType.Music;

	public override string IdZone => "instance_wedding";

	public override int KarmaOnFail => 0;

	public override ZoneEventQuest CreateEvent()
	{
		return new ZoneEventWedding();
	}

	public override ZoneInstanceRandomQuest CreateInstance()
	{
		return new ZoneInstanceWedding();
	}

	public override string GetTextProgress()
	{
		return "progressWedding".lang();
	}

	public override void OnDropReward()
	{
		Thing thing = ThingGen.Create("milkcan");
		thing.MakeRefFrom(person.chara);
		EClass.player.DropReward(thing);
		thing = ThingGen.Create("musicbox_memory");
		thing.MakeRefFrom(person.chara, EClass.pc, simple: true);
		DropReward(thing);
		if (!EClass.pc.elements.Has(6628))
		{
			EClass.pc.elements.Learn(6628);
		}
	}
}
