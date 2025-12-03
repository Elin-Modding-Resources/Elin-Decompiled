public class ConSupress : BaseDebuff
{
	public override bool WillOverride => true;

	public override BaseNotification CreateNotification()
	{
		return new NotificationCondition
		{
			condition = this
		};
	}
}
