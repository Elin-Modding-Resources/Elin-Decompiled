public class ConPoison : BadCondition
{
	public override Emo2 EmoIcon => Emo2.poison;

	public override bool UseElements => true;

	public override bool PreventRegen => true;

	public override void OnChangePhase(int lastPhase, int newPhase)
	{
		switch (newPhase)
		{
		case 1:
			elements.SetBase(70, -10);
			break;
		case 2:
			elements.SetBase(70, -10);
			break;
		case 3:
			elements.SetBase(70, -15);
			break;
		default:
			elements.SetBase(70, -5);
			break;
		}
	}

	public override void Tick()
	{
		if (EClass.rnd(5) == 0)
		{
			owner.DamageHP(EClass.rnd(owner.END / 10 + 2) + 1, AttackSource.Condition);
		}
		Mod(-1);
	}
}
