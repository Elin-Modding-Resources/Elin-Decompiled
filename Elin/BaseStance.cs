public class BaseStance : Condition
{
	public override bool CanManualRemove => true;

	public override bool HasDuration => false;

	public override int GetPhase()
	{
		return 0;
	}

	public override BaseNotification CreateNotification()
	{
		return new NotificationStance
		{
			condition = this
		};
	}

	public override void Tick()
	{
	}
}
