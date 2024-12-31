public class HotItemActionSleep : HotAction
{
	public override string Id => "Sleep";

	public override bool CanName => false;

	public override void Perform()
	{
		if (!EClass.pc.CanSleep())
		{
			Msg.Say((EClass._zone.events.GetEvent<ZoneEventQuest>() != null) ? "badidea" : "notSleepy");
			return;
		}
		if (EClass.player.returnInfo != null || EClass.pc.isRestrained)
		{
			Msg.Say("badidea");
			return;
		}
		Thing thing = EClass.pc.things.Find<TraitBed>();
		if (thing == null)
		{
			Msg.Say("noBedFound".langGame());
			SE.Beep();
			return;
		}
		Thing thing2 = EClass.pc.things.Find<TraitPillow>();
		ItemPosition posBed = ItemPosition.Get(thing);
		ItemPosition posPillow = ItemPosition.Get(thing2);
		EClass._zone.AddCard(thing, EClass.pc.pos).Install();
		if (thing2 != null)
		{
			EClass._zone.AddCard(thing2, EClass.pc.pos).Install();
		}
		EClass.pc.Sleep(thing, thing2, pickup: true, posBed, posPillow);
	}
}
