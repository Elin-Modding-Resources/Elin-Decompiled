public class ConSmoking : Condition
{
	public override int GetPhase()
	{
		return 0;
	}

	public override void Tick()
	{
		base.Tick();
		if (EClass.rnd(2) == 0)
		{
			owner.sleepiness.Mod(-1);
		}
	}
}
