public class TraitCoreDefense : Trait
{
	public override bool CanBeDestroyed => false;

	public override bool CanOnlyCarry => true;

	public override bool CanPutAway => false;

	public override bool IsLightOn => true;

	public override void TrySetAct(ActPlan p)
	{
		if (EClass.game.IsSurvival && EClass._zone is Zone_StartSiteSky)
		{
			if (!EClass.game.survival.flags.raid)
			{
				p.TrySetAct("actWarhorn", delegate
				{
					SE.Play("warhorn");
					Msg.Say("warhorn");
					EClass.game.survival.StartRaid();
					return true;
				});
			}
			else if (!EClass.game.survival.IsInRaid && EClass._map.FindThing<TraitVoidgate>() != null)
			{
				p.TrySetAct("actWarhornRaid", delegate
				{
					SE.Play("warhorn");
					Msg.Say("warhorn");
					EClass._zone.events.Add(new ZoneEventRaid());
					return true;
				});
			}
			if (EClass.debug.enable)
			{
				p.TrySetAct("50", delegate
				{
					SE.Play("warhorn");
					Msg.Say("warhorn");
					EClass.game.survival.flags.raidLv += 50;
					return true;
				});
			}
		}
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
