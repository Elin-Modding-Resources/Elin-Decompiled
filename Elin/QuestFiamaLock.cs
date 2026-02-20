public class QuestFiamaLock : QuestProgression
{
	public override bool CanUpdateOnTalk(Chara c)
	{
		return phase == 0;
	}

	public override void OnStart()
	{
		EClass.player.DropReward(ThingGen.Create("lockpick")).c_charges = 12;
		Thing thing = EClass.player.DropReward(ThingGen.Create("1171"));
		thing.ChangeMaterial("willow");
		thing.c_lockLv = 50;
		thing.c_lockedHard = true;
		thing.things.DestroyAll();
		thing.AddCard(ThingGen.Create("1172"));
		thing.AddCard(ThingGen.Create("amulet_begger"));
		thing.SetBool(127, enable: true);
		Card card = thing.AddCard(ThingGen.Create("_meat").MakeFoodFrom("begger"));
		card.decay = card.MaxDecay + 1;
		thing.Install();
		EClass._zone.events.Add(new ZoneEventBeggerChest
		{
			uidChest = thing.uid
		});
	}
}
