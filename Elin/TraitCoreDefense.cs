public class TraitCoreDefense : Trait
{
	public override bool CanBeDestroyed => false;

	public override bool CanOnlyCarry => true;

	public override bool CanPutAway => false;

	public override bool IsLightOn => true;

	public override void TrySetAct(ActPlan p)
	{
		ZoneEventDefenseGame ev = EClass._zone.events.GetEvent<ZoneEventDefenseGame>();
		if (ev == null)
		{
			return;
		}
		if (ev.wave % 5 != 0 && !ev.retreated)
		{
			p.TrySetAct("actWarhorn", delegate
			{
				ev.Horn_Next();
				return true;
			});
		}
		if (ev.CanRetreat && EClass.player.returnInfo == null)
		{
			p.TrySetAct("actEvacDefense", delegate
			{
				ev.Horn_Retreat();
				return true;
			});
		}
		if (ev.CanCallAlly)
		{
			p.TrySetAct("actCallAlly", delegate
			{
				ev.Horn_Ally();
				return true;
			});
		}
	}
}
