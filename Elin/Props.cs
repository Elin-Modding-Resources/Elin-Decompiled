using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class Props : EClass
{
	public PropSet all = new PropSet();

	public Dictionary<string, PropSet> cardMap = new Dictionary<string, PropSet>();

	public Dictionary<string, PropSetCategory> categoryMap = new Dictionary<string, PropSetCategory>();

	public Dictionary<string, PropSet> raceMap = new Dictionary<string, PropSet>();

	public Dictionary<string, PropSet> workMap = new Dictionary<string, PropSet>();

	public TraitManager traits = new TraitManager();

	public List<Thing> things = new List<Thing>();

	public List<Thing> containers = new List<Thing>();

	[JsonProperty]
	public int maxWeight = 100;

	public int weight;

	public virtual bool IsStocked => false;

	public virtual bool IsRoaming => false;

	public virtual bool IsInstalled => false;

	public virtual PlaceState state => PlaceState.roaming;

	public List<Thing> Things => things;

	public int Count => all.Count;

	public void Init()
	{
		if (categoryMap.Count > 0)
		{
			return;
		}
		foreach (SourceCategory.Row row in EClass.sources.categories.rows)
		{
			categoryMap.Add(row.id, new PropSetCategory
			{
				source = row
			});
		}
		foreach (PropSetCategory value in categoryMap.Values)
		{
			if (value.source.parent != null)
			{
				value.parent = categoryMap[value.source.parent.id];
			}
		}
	}

	public void Add(Card t)
	{
		if (all.Contains(t))
		{
			Debug.Log(t.props);
			Debug.Log(t.parent);
			Debug.Log(things.Contains(t) + "/" + cardMap.ContainsKey(t.id));
			Debug.LogError(t?.ToString() + " alreadin in " + this);
			return;
		}
		if (t.props != null)
		{
			t.props.Remove(t);
		}
		t.props = this;
		t.placeState = state;
		if (t.isChara)
		{
			raceMap.GetOrCreate(t.Chara.race.id).Add(t);
			return;
		}
		foreach (Thing thing in t.things)
		{
			if (t.placeState != 0)
			{
				EClass._map.Stocked.Add(thing);
			}
		}
		weight += t.Num;
		all.Add(t);
		things.Add(t.Thing);
		cardMap.GetOrCreate(t.id).Add(t);
		if (t.sourceCard.origin != null)
		{
			cardMap.GetOrCreate(t.sourceCard.origin.id).Add(t);
		}
		categoryMap[t.category.id].Add(t);
		if (!t.Thing.source.workTag.IsEmpty())
		{
			workMap.GetOrCreate(t.Thing.source.workTag).Add(t);
		}
		if (t.IsContainer)
		{
			containers.Add(t.Thing);
		}
		traits.OnAddCard(t);
		if (t.isDeconstructing)
		{
			EClass._map.props.deconstructing.Add(t);
		}
	}

	public void Remove(Card t)
	{
		t.props = null;
		t.placeState = PlaceState.roaming;
		if (t.isChara)
		{
			raceMap[t.Chara.race.id].Remove(t);
			return;
		}
		foreach (Thing thing in t.things)
		{
			if (thing.props != null)
			{
				thing.props.Remove(thing);
			}
		}
		if (!all.Contains(t))
		{
			Debug.LogError(t?.ToString() + " isn't in " + this);
			return;
		}
		weight -= t.Num;
		all.Remove(t);
		things.Remove(t.Thing);
		cardMap[t.id].Remove(t);
		if (t.sourceCard.origin != null)
		{
			cardMap[t.sourceCard.origin.id].Remove(t);
		}
		categoryMap[t.category.id].Remove(t);
		if (!t.Thing.source.workTag.IsEmpty())
		{
			workMap[t.Thing.source.workTag].Remove(t);
		}
		if (t.IsContainer)
		{
			containers.Remove(t.Thing);
		}
		traits.OnRemoveCard(t);
		if (t.isDeconstructing)
		{
			EClass._map.props.deconstructing.Remove(t);
		}
	}

	public void OnNumChange(Card c, int a)
	{
		if (!c.isChara)
		{
			weight += a;
			categoryMap[c.category.id].OnChangeNum(a);
			cardMap[c.id].OnChangeNum(a);
			if (!c.Thing.source.workTag.IsEmpty())
			{
				workMap[c.Thing.source.workTag].OnChangeNum(a);
			}
		}
	}

	public bool ShouldListAsResource(Thing t)
	{
		if (!(t.parent is Card card))
		{
			return false;
		}
		if (card.isSale || !card.trait.CanUseContent)
		{
			return false;
		}
		return !t.c_isImportant;
	}

	public Thing GetAvailableThing(string id, int idMat)
	{
		PropSet propSet = cardMap.TryGetValue(id);
		if (propSet == null)
		{
			return null;
		}
		foreach (Card item in propSet)
		{
			if (item.idMaterial == idMat)
			{
				return item as Thing;
			}
		}
		return null;
	}

	public ThingStack ListThingStack(Recipe.Ingredient ing, StockSearchMode searchMode)
	{
		string id2 = ing.id;
		int idMat = -1;
		string tag = (ing.tag.IsEmpty() ? null : ing.tag);
		ThingStack stack = new ThingStack
		{
			val = idMat
		};
		_ = EClass.pc.pos.cell.room;
		if (ing.useCat)
		{
			FindCat(id2);
			foreach (string item in ing.idOther)
			{
				FindCat(item);
			}
			stack.list.Sort(UIList.SortMode.ByCategory);
			return stack;
		}
		Find(id2);
		foreach (string item2 in ing.idOther)
		{
			Find(item2);
		}
		stack.list.Sort(UIList.SortMode.ByCategory);
		return stack;
		void Find(string id)
		{
			bool isOrigin = EClass.sources.cards.map[id].isOrigin;
			EClass.pc.things.Foreach(delegate(Thing t)
			{
				if (!t.isEquipped && (!(t.id != id) || (isOrigin && t.source._origin == id)))
				{
					TryAdd(t);
				}
			});
			if (EClass._zone.IsPCFaction || EClass._zone is Zone_Tent || EClass.debug.enable)
			{
				foreach (Card item3 in cardMap.GetOrCreate(id))
				{
					if (!(item3.parent is Thing thing) || (thing.c_lockLv == 0 && thing.trait.CanUseContent))
					{
						TryAdd(item3.Thing);
					}
				}
			}
		}
		void FindCat(string id)
		{
			SourceCategory.Row cat = EClass.sources.categories.map[id];
			EClass.pc.things.Foreach(delegate(Thing t)
			{
				if (!t.isEquipped && t.category.IsChildOf(cat.id) && !t.IsExcludeFromCraft(ing))
				{
					stack.Add(t);
				}
			});
			if (EClass._zone.IsPCFaction || EClass._zone is Zone_Tent || EClass.debug.enable)
			{
				foreach (Thing thing2 in things)
				{
					Card obj = thing2.parent as Card;
					if (obj != null && obj.c_lockLv == 0 && thing2.category.IsChildOf(cat.id) && !thing2.IsExcludeFromCraft(ing))
					{
						stack.Add(thing2);
					}
				}
			}
		}
		void TryAdd(Thing t)
		{
			if ((tag == null || t.Thing.material.tag.Contains(tag)) && (idMat == -1 || t.material.id == idMat) && !t.IsExcludeFromCraft(ing))
			{
				stack.Add(t.Thing);
			}
		}
	}

	public List<Thing> ListThingsInCategory(SourceCategory.Row cat)
	{
		List<Thing> list = new List<Thing>();
		foreach (Thing thing in Things)
		{
			if (thing.category.IsChildOf(cat))
			{
				list.Add(thing);
			}
		}
		return list;
	}

	public Dictionary<string, ThingStack> ListThingStacksInCategory(SourceCategory.Row cat)
	{
		Dictionary<string, ThingStack> dictionary = new Dictionary<string, ThingStack>();
		foreach (Thing thing in Things)
		{
			ListThingStacksInCategory(cat, dictionary, thing);
		}
		return dictionary;
	}

	private void ListThingStacksInCategory(SourceCategory.Row cat, Dictionary<string, ThingStack> stacks, Thing t)
	{
		if (EClass.sources.categories.map[t.source.category].IsChildOf(cat))
		{
			ThingStack thingStack = stacks.TryGetValue(t.id);
			if (thingStack == null)
			{
				thingStack = new ThingStack();
				stacks.Add(t.id, thingStack);
			}
			thingStack.count += t.Num;
			thingStack.list.Add(t);
		}
	}

	public Thing Find<T>() where T : Trait
	{
		foreach (Thing thing in Things)
		{
			if (thing.trait is T)
			{
				return thing;
			}
		}
		return null;
	}

	public Thing FindEmptyContainer<T>() where T : Trait
	{
		foreach (Thing thing in Things)
		{
			if (thing.trait is T && !thing.things.IsFull())
			{
				return thing;
			}
		}
		return null;
	}

	public Thing FindEmptyContainer<T>(Thing target) where T : Trait
	{
		foreach (Thing thing in Things)
		{
			if (thing.trait is T && !thing.things.IsFull(target))
			{
				return thing;
			}
		}
		return null;
	}

	public Thing Find(int uid)
	{
		foreach (Thing thing in Things)
		{
			if (thing.uid == uid)
			{
				return thing;
			}
		}
		return null;
	}

	public Thing FindShared(string id)
	{
		return Find(id, -1, -1, shared: true);
	}

	public Thing Find(string id, string idMat)
	{
		return Find(id, idMat.IsEmpty() ? (-1) : EClass.sources.materials.alias[idMat].id);
	}

	public Thing Find(string id, int idMat = -1, int refVal = -1, bool shared = false)
	{
		PropSet propSet = cardMap.TryGetValue(id);
		if (propSet != null)
		{
			foreach (Card item in propSet)
			{
				if ((!shared || item.parent is Thing { IsSharedContainer: not false }) && (idMat == -1 || item.material.id == idMat) && (refVal == -1 || item.refVal == refVal))
				{
					return item as Thing;
				}
			}
		}
		return null;
	}

	public int GetNum(string id, bool onlyShared = false)
	{
		int num = 0;
		foreach (Card item in cardMap.GetOrCreate(id))
		{
			if (!onlyShared || (item.parentThing != null && item.parentThing.IsSharedContainer))
			{
				num += item.Num;
			}
		}
		return num;
	}

	public static int GetNumStockedAndRoaming(string id)
	{
		return EClass._map.Stocked.cardMap.GetOrCreate(id).num + EClass._map.Roaming.GetNum(id);
	}

	public void Validate()
	{
		foreach (KeyValuePair<string, PropSet> item in cardMap)
		{
			int num = 0;
			foreach (Card item2 in item.Value)
			{
				num += item2.Num;
			}
			if (num != item.Value.num)
			{
				Debug.LogError("prop num:" + item.Key + " " + item.Value.num + "/" + num);
			}
		}
	}
}
