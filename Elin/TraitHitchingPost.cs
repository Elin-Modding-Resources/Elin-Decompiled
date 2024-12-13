public class TraitHitchingPost : TraitFloorSwitch
{
	public override bool IsHomeItem => true;

	public override void OnActivateTrap(Chara c)
	{
		if (!c.IsPC || !EClass._zone.IsPCFaction)
		{
			return;
		}
		if (EClass.pc.ride == null)
		{
			foreach (Chara chara in owner.pos.Charas)
			{
				if (chara.IsPCFaction && !chara.IsPC && !chara.IsPCParty && chara.trait.CanJoinParty)
				{
					ActRide.Ride(EClass.pc, chara);
					return;
				}
			}
			Msg.Say("noRide");
			TraitSwitch.haltMove = false;
		}
		else
		{
			Chara ride = EClass.pc.ride;
			ActRide.Unride(EClass.pc);
			EClass.pc.party.RemoveMember(ride);
			ride.noMove = true;
			ride.orgPos = new Point(owner.pos);
		}
	}
}
