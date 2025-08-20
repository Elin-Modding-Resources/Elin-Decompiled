public class TraitMixedFood : TraitFoodMeal
{
	public override bool CanSearchContent => false;

	public override bool CanStack => false;

	public override int DefaultStock => 1;
}
