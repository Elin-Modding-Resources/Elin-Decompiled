public class TraitLandingPortal : TraitItem
{
	public override bool CanBeDestroyed => false;

	public override bool CanOnlyCarry => true;

	public override bool CanPutAway => false;

	public override bool IsAnimeOn => IsOn;

	public override bool UseAltTiles => IsOn;

	public override bool CanUseFromInventory => false;

	public override bool IsOn => EClass.game.survival?.gateZone != null;

	public override bool CanUse(Chara c)
	{
		return IsOn;
	}

	public override bool OnUse(Chara c)
	{
		Zone gateZone = EClass.game.survival.gateZone;
		if (gateZone == null || gateZone.destryoed || gateZone == EClass.game.activeZone || EClass.game.activeZone.IsRegion)
		{
			Msg.SayNothingHappen();
			return false;
		}
		Msg.Say("returnComplete");
		EClass.player.uidLastTravelZone = 0;
		EClass.pc.MoveZone(gateZone, ZoneTransition.EnterState.Return);
		EClass.player.lastZonePos = null;
		EClass.player.returnInfo = null;
		return false;
	}
}
