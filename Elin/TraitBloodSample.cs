public class TraitBloodSample : TraitFoodMeal
{
	public override void SetName(ref string s)
	{
		if (!owner.c_idRefCard.IsEmpty())
		{
			s = s + Lang.space + "_flavor".lang(EClass.sources.cards.map.TryGetValue(owner.c_idRefCard, "fail_dish").GetName().ToTitleCase(wholeText: true));
		}
	}
}
