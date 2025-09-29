using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

public class SpatialManager : EClass
{
	public class GlobalSpatialList : Dictionary<int, Spatial>
	{
		public void Add(Spatial s)
		{
			Add(s.uid, s);
		}
	}

	[JsonProperty]
	public GlobalSpatialList map = new GlobalSpatialList();

	[JsonProperty]
	public int uidNext = 1;

	[JsonProperty]
	public RankedZoneManager ranks = new RankedZoneManager();

	public List<Spatial> listDestryoed = new List<Spatial>();

	public Zone Somewhere => Find("somewhere");

	public List<Zone> Zones => map.Values.Where((Spatial a) => a is Zone).Cast<Zone>().ToList();

	public void AssignUID(Spatial s)
	{
		s.uid = uidNext;
		uidNext++;
		map.Add(s);
	}

	public void Remove(Spatial s)
	{
		if (s.parent != null)
		{
			s.parent.children.Remove(s);
		}
		map.Remove(s.uid);
		listDestryoed.Add(s);
	}

	public T Find<T>(Func<T, bool> func = null) where T : Zone
	{
		foreach (Spatial value in map.Values)
		{
			if (value is T val && (func == null || func(val)))
			{
				return val;
			}
		}
		return null;
	}

	public Zone Find(string id)
	{
		foreach (Spatial value in map.Values)
		{
			if (value is Zone && value.id == id)
			{
				return value as Zone;
			}
		}
		return null;
	}

	public Zone Find(int uid)
	{
		return map.TryGetValue(uid) as Zone;
	}

	public List<Zone> ListReturnLocations()
	{
		if (EClass.debug.returnAnywhere)
		{
			List<Zone> list = (from Zone a in map.Values.Where((Spatial a) => a is Zone)
				where a != EClass._zone && (a.IsReturnLocation || a.IsPCFaction || (!(a is Zone_Field) && !a.IsInstance && !a.isRandomSite)) && a.parent == EClass.world.region && !a.source.tag.Contains("closed")
				select a).ToList();
			list.Sort((Zone a, Zone b) => a.GetSortVal() - b.GetSortVal());
			return list;
		}
		List<Zone> list2 = (from Zone a in map.Values.Where((Spatial a) => a is Zone)
			where a != EClass._zone && a.IsReturnLocation && a.GetTopZone().visitCount > 0 && (a.GetTopZone().FindDeepestZone() == a || EClass.pc.homeZone == a)
			select a).ToList();
		list2.Sort((Zone a, Zone b) => a.GetSortVal() - b.GetSortVal());
		return list2;
	}
}
