public class TraitContainerCompost : TraitBrewery
{
	public override Type type => Type.Fertilizer;

	public override string idMsg => "agedFood";

	public override bool CanChildDecay(Card c)
	{
		return c.id == "lunch_dystopia";
	}

	public override string GetProductID(Card c)
	{
		return "fertilizer";
	}
}
