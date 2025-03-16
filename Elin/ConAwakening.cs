public class ConAwakening : Condition
{
	public override int GetPhase()
	{
		return 0;
	}

	public override void OnStart()
	{
		base.OnStart();
		owner.sleepiness.Mod(-20);
	}
}
