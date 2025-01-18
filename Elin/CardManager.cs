using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

public class CardManager : EClass
{
	public class GlobalCharaList : Dictionary<int, Chara>
	{
		public void Add(Chara c)
		{
			base[c.uid] = c;
		}

		public void Remove(Chara c)
		{
			Remove(c.uid);
		}

		public Chara Find(string id)
		{
			foreach (Chara value in base.Values)
			{
				if (value.id == id)
				{
					return value;
				}
			}
			return null;
		}

		public Chara Find(int uid)
		{
			foreach (Chara value in base.Values)
			{
				if (value.uid == uid)
				{
					return value;
				}
			}
			return null;
		}
	}

	[JsonProperty]
	public GlobalCharaList globalCharas = new GlobalCharaList();

	[JsonProperty]
	public int uidNext = 1;

	[JsonProperty]
	public Thing container_shipping;

	[JsonProperty]
	public Thing container_deliver;

	[JsonProperty]
	public Thing container_deposit;

	[JsonProperty]
	public List<Thing> listPackage = new List<Thing>();

	[JsonProperty]
	public List<Chara> listAdv = new List<Chara>();

	[OnDeserializing]
	private void OnDeserializing(StreamingContext context)
	{
		Debug.Log("#io CardManager OnDeserializing:");
	}

	[OnDeserialized]
	private void OnDeserialized(StreamingContext context)
	{
		Debug.Log("#io CardManager OnDeserialized:" + globalCharas.Count);
	}

	public void AssignUID(Card c)
	{
		if (globalCharas.ContainsKey(uidNext))
		{
			while (globalCharas.ContainsKey(uidNext))
			{
				uidNext++;
			}
		}
		c.uid = uidNext;
		uidNext++;
	}

	public void AssignUIDRecursive(Card c)
	{
		if (c.things.Count > 0)
		{
			foreach (Thing thing in c.things)
			{
				AssignUIDRecursive(thing);
			}
		}
		AssignUID(c);
	}

	public List<Chara> ListGlobalChara(Zone z)
	{
		List<Chara> list = new List<Chara>();
		foreach (Chara value in globalCharas.Values)
		{
			if (value.currentZone == z)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public Chara Find(int uid)
	{
		foreach (Chara value in globalCharas.Values)
		{
			if (value.uid == uid)
			{
				return value;
			}
		}
		return null;
	}
}
