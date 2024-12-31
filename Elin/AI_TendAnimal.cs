public class AI_TendAnimal : AI_Fuck
{
	public override FuckType Type => FuckType.tame;

	public override bool ShouldAllyAttack(Chara tg)
	{
		return tg != target;
	}
}
