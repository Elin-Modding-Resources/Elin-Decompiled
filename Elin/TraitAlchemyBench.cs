public class TraitAlchemyBench : TraitWorkbench
{
	public override int WitchDoubleCraftChance(Thing t)
	{
		return 100;
	}

	public override bool Contains(RecipeSource r)
	{
		return r.idFactory == "tool_alchemy";
	}
}
