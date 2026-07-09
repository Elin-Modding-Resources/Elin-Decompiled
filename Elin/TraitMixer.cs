public class TraitMixer : TraitChopper
{
	public override bool Contains(RecipeSource r)
	{
		if (!base.Contains(r))
		{
			return r.idFactory == "mixer";
		}
		return true;
	}
}
