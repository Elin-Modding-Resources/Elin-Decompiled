public class ConDark : Condition
{
	public override bool ShouldRefresh => true;

	public override bool ShouldOverride(Condition c)
	{
		return true;
	}

	public override int GetPhase()
	{
		return 0;
	}
}
