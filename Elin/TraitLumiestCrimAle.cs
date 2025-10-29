public class TraitLumiestCrimAle : TraitFoodMeal
{
	public override bool CanDrink(Chara c)
	{
		return true;
	}

	public override void OnDrink(Chara c)
	{
		ActEffect.Proc(EffectId.Booze, 500, owner.blessedState, c);
		FoodEffect.ProcTrait(c, owner);
	}
}
