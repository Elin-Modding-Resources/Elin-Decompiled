public class TraitAgingShelf : TraitBrewery
{
	public override Type type => Type.Food;

	public override string idMsg => "agedFood";

	public override string GetProductID(Card c)
	{
		return "cheese";
	}

	public override void OnProduce(Card c)
	{
		if (c.Evalue(73) >= -30)
		{
			c.elements.SetTo(73, -10);
		}
	}
}
