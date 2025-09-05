public class AI_Sleep : AI_TargetThing
{
	public Chara lover;

	public override bool GotoTarget => lover == null;

	public override void OnProgressComplete()
	{
		if (!owner.CanSleep())
		{
			Msg.Say((EClass._zone.events.GetEvent<ZoneEventQuest>() != null) ? "badidea" : "notSleepy");
			return;
		}
		if (lover != null && lover.ExistsOnMap && lover.Dist(owner) <= 1)
		{
			owner.Say("sleep_beside", owner, lover);
			owner.MoveImmediate(lover.pos, focus: false, cancelAI: false);
		}
		else if (base.target != null && !owner.pos.Equals(base.target.pos))
		{
			owner._Move(base.target.pos);
		}
		owner.Sleep(base.target);
	}
}
