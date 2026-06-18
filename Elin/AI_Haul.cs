using System.Collections.Generic;

public class AI_Haul : AIAct
{
	public Thing dest;

	public static List<Thing> _list = new List<Thing>();

	public override IEnumerable<Status> Run()
	{
		if (dest.ExistsOnMap)
		{
			yield return DoGoto(dest.pos, 1);
		}
		if (!dest.ExistsOnMap)
		{
			yield return Success();
		}
		if (EClass._zone.TryAddThingInSharedContainer(dest))
		{
			owner.Say("haul", owner, dest);
			if (dest.id == "731")
			{
				owner.Talk("clean");
			}
			else
			{
				owner.Talk("haul");
			}
		}
		yield return Success();
	}

	public static Thing GetThingToClean(Chara c = null)
	{
		_list.Clear();
		foreach (Thing thing in EClass._map.things)
		{
			if (thing.placeState == PlaceState.roaming && !thing.isMasked && (thing.id == "731" || thing.id == "_egg" || thing.id == "egg_fertilized" || thing.id == "_milk"))
			{
				_list.Add(thing);
			}
		}
		if (_list.Count == 0)
		{
			return null;
		}
		if (c != null)
		{
			Thing result = null;
			int num = 9999;
			{
				foreach (Thing item in _list)
				{
					int num2 = c.pos.Distance(item.pos);
					if (num2 < num && EClass._zone.TryAddThingInSharedContainer(item, null, add: false))
					{
						num = num2;
						result = item;
					}
				}
				return result;
			}
		}
		_list.Shuffle();
		foreach (Thing item2 in _list)
		{
			if (EClass._zone.TryAddThingInSharedContainer(item2, null, add: false))
			{
				return item2;
			}
		}
		return null;
	}

	public static AI_Haul TryGetAI(Chara c)
	{
		Thing thingToClean = GetThingToClean(c);
		if (thingToClean != null)
		{
			return new AI_Haul
			{
				dest = thingToClean
			};
		}
		return null;
	}
}
