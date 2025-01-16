using System.Collections.Generic;
using System.Linq;

public class TraitMap : HashSet<Card>
{
	public Card GetRandomInstalled()
	{
		if (base.Count == 0)
		{
			return null;
		}
		return this.Where((Card a) => a.placeState == PlaceState.installed).RandomItem();
	}
}
