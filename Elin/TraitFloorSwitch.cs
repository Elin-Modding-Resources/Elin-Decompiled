public class TraitFloorSwitch : TraitSwitch
{
	public override void OnStepped(Chara c)
	{
		if (!IsNegativeEffect || (!EClass._zone.IsPCFaction && !EClass._zone.IsUserZone) || c.IsHostile())
		{
			owner.SetHidden(hide: false);
			if (IgnoreWhenLevitating() && c.IsLevitating)
			{
				owner.Say("levitating");
			}
			else if (!CanDisarmTrap || (!TryDisarmTrap(c) && EClass.pc.Evalue(1656) < 3))
			{
				ActivateTrap(c);
			}
		}
	}
}
