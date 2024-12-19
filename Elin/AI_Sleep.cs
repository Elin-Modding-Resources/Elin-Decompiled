public class AI_Sleep : AI_TargetThing
{
	public override bool GotoTarget => true;

	public override void OnProgressComplete()
	{
		if (!owner.CanSleep())
		{
			Msg.Say((EClass._zone.events.GetEvent<ZoneEventQuest>() != null) ? "badidea" : "notSleepy");
			return;
		}
		if (base.target != null && !owner.pos.Equals(base.target.pos))
		{
			owner._Move(base.target.pos);
		}
		owner.Sleep(base.target);
	}
}
