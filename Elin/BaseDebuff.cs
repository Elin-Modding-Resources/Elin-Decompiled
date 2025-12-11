public class BaseDebuff : Condition
{
	public override bool ShouldOverride(Condition c)
	{
		return true;
	}

	public override int GetPhase()
	{
		return 0;
	}

	public override BaseNotification CreateNotification()
	{
		return new NotificationBuff
		{
			condition = this
		};
	}
}
