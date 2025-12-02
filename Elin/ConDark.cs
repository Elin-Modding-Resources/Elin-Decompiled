public class ConDark : Condition
{
	public override bool ShouldRefresh => true;

	public override int GetPhase()
	{
		return 0;
	}
}
