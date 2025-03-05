public class ConSupress : BaseDebuff
{
	public override BaseNotification CreateNotification()
	{
		return new NotificationCondition
		{
			condition = this
		};
	}
}
