using System;
using System.Collections.Generic;
using System.Linq;

public class TraitSet : HashSet<Card>
{
	public Trait GetRandom()
	{
		return this.RandomItem()?.trait;
	}

	public Trait GetRandom(Chara accessChara)
	{
		return GetRandom((Card t) => accessChara == null || accessChara.HasAccess(t));
	}

	public Trait GetRandom(Func<Card, bool> func)
	{
		if (func == null)
		{
			return GetRandom();
		}
		return this.Where(func).RandomItem()?.trait;
	}
}
