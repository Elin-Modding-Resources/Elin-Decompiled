public class TraitCookingStove : TraitStove
{
	public override bool Contains(RecipeSource r)
	{
		if (!base.Contains(r) && !(r.idFactory == "camppot") && !(r.idFactory == "mixer") && !(r.idFactory == "bbq") && !(r.idFactory == "bonfire") && !(r.idFactory == "chopper"))
		{
			return r.idFactory == "oven";
		}
		return true;
	}
}
