public class ConWait : Condition
{
	public override bool ConsumeTurn => true;

	public override int GetPhase()
	{
		return 0;
	}

	public override bool ShouldOverride(Condition c)
	{
		return true;
	}
}
