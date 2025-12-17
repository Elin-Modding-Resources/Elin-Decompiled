public class TaskBaseBuild : TaskDesignation
{
	public bool CanPlaceCard(Point pos, Card c)
	{
		if (!InstallCheck<TraitBed>())
		{
			return false;
		}
		return true;
		bool InstallCheck<T>() where T : Trait
		{
			if (!(c.trait is T))
			{
				return true;
			}
			foreach (Thing thing in pos.Things)
			{
				if (thing != c && thing.trait is T)
				{
					return false;
				}
			}
			return true;
		}
	}
}
