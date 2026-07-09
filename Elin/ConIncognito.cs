public class ConIncognito : BaseBuff
{
	public override void OnStart()
	{
		if (!EClass._zone.HasField(10002))
		{
			EClass._zone.ResetHostility();
			EClass._zone.RefreshCriminal();
			EClass._zone.SetFieldEffect(10002, 1, 20);
		}
	}

	public override void OnRemoved()
	{
		if (!EClass._zone.HasField(10002))
		{
			EClass._zone.RefreshCriminal();
		}
	}
}
