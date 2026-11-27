using UnityEngine;

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
			ModSTR(10, 0.2f);
			break;
		case 2:
			ModSTR(15, 0.3f);
			break;
		case 3:
			ModSTR(20, 0.4f);
			break;
		default:
			ModSTR(5, 0.1f);
			break;
		}
		void ModSTR(int min, float ratio)
		{
			elements.SetBase(70, -Mathf.Max(min, (int)(ratio * (float)owner.elements.ValueWithoutLink(70))));
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
