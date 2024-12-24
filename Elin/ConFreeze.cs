public class ConFreeze : Condition
{
	public override bool UseElements => true;

	public override void OnChangePhase(int lastPhase, int newPhase)
	{
		switch (newPhase)
		{
		case 0:
			elements.SetBase(79, -25);
			break;
		case 1:
			elements.SetBase(79, -50);
			break;
		}
	}
}
