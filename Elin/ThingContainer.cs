using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class ThingContainer : List<Thing>
{
	public struct DestData
	{
		public Card container;

		public Thing stack;

		public bool IsValid
		{
			get
			{
				if (stack == null)
				{
					return container != null;
				}
				return true;
			}
		}
	}

	[JsonIgnore]
	public static List<Thing> listUnassigned = new List<Thing>();

	[JsonIgnore]
	public const int InvYHotbar = 1;

	[JsonIgnore]
	public int width;

	[JsonIgnore]
	public int height;

	[JsonIgnore]
	public Card owner;

	[JsonIgnore]
	public List<Thing> grid;

	private static List<ThingContainer> _listContainers = new List<ThingContainer>();

	private static List<Thing> tempList = new List<Thing>();

	[JsonIgnore]
	public int GridSize => width * height;

	[JsonIgnore]
	public bool HasGrid => grid != null;

	[JsonIgnore]
	public bool IsMagicChest => owner.trait is TraitMagicChest;

	[JsonIgnore]
	public int MaxCapacity
	{
		get
		{
			if (!IsMagicChest)
			{
				return GridSize;
			}
			return 100 + owner.c_containerUpgrade.cap;
		}
	}

	public void SetOwner(Card owner)
	{
		this.owner = owner;
		width = owner.c_containerSize / 100;
		height = owner.c_containerSize % 100;
		if (width == 0)
		{
			width = 8;
			height = 5;
		}
	}

	public void ChangeSize(int w, int h)
	{
		width = w;
		height = h;
		owner.c_containerSize = w * 100 + h;
		RefreshGrid();
		Debug.Log(base.Count + "/" + width + "/" + height + "/" + GridSize);
	}

	public void RefreshGridRecursive()
	{
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Thing current = enumerator.Current;
				if (current.IsContainer)
				{
					current.things.RefreshGridRecursive();
				}
			}
		}
		RefreshGrid();
	}

	public void RefreshGrid()
	{
		if (GridSize == 0)
		{
			return;
		}
		grid = new List<Thing>(new Thing[GridSize]);
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Thing current = enumerator.Current;
				if (ShouldShowOnGrid(current))
				{
					if (current.invX >= GridSize || current.invX < 0 || grid[current.invX] != null)
					{
						listUnassigned.Add(current);
					}
					else
					{
						grid[current.invX] = current;
					}
				}
			}
		}
		foreach (Thing item in listUnassigned)
		{
			int freeGridIndex = GetFreeGridIndex();
			if (freeGridIndex == -1)
			{
				break;
			}
			grid[freeGridIndex] = item;
			item.invX = freeGridIndex;
		}
		listUnassigned.Clear();
	}

	public void RefreshGrid(UIMagicChest magic, Window.SaveData data)
	{
		magic.filteredList.Clear();
		magic.cats.Clear();
		magic.catCount.Clear();
		grid = new List<Thing>(new Thing[GridSize]);
		string text = magic.lastSearch;
		bool flag = !text.IsEmpty();
		bool flag2 = !magic.idCat.IsEmpty();
		Window.SaveData.CategoryType category = data.category;
		bool flag3 = category != Window.SaveData.CategoryType.None;
		string text2 = "";
		bool flag4 = text != null && text.Length >= 2 && (text[0] == '@' || text[0] == '＠');
		if (flag4)
		{
			text = text.Substring(1);
		}
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Thing current = enumerator.Current;
				if (flag3)
				{
					switch (category)
					{
					case Window.SaveData.CategoryType.Main:
						text2 = current.category.GetRoot().id;
						break;
					case Window.SaveData.CategoryType.Sub:
						text2 = current.category.GetSecondRoot().id;
						break;
					case Window.SaveData.CategoryType.Exact:
						text2 = current.category.id;
						break;
					}
					magic.cats.Add(text2);
					if (magic.catCount.ContainsKey(text2))
					{
						magic.catCount[text2]++;
					}
					else
					{
						magic.catCount.Add(text2, 1);
					}
				}
				if (flag)
				{
					if (flag4)
					{
						if (!current.MatchEncSearch(text))
						{
							continue;
						}
					}
					else
					{
						if (current.tempName == null)
						{
							current.tempName = current.GetName(NameStyle.Full, 1).ToLower();
						}
						if (!current.tempName.Contains(text) && !current.source.GetSearchName(jp: false).Contains(text) && !current.source.GetSearchName(jp: true).Contains(text))
						{
							continue;
						}
					}
				}
				if (!flag2 || !(text2 != magic.idCat))
				{
					magic.filteredList.Add(current);
				}
			}
		}
		if (flag2 && !magic.cats.Contains(magic.idCat))
		{
			magic.idCat = "";
			RefreshGrid(magic, data);
			return;
		}
		magic.pageMax = (magic.filteredList.Count - 1) / GridSize;
		if (magic.page > magic.pageMax)
		{
			magic.page = magic.pageMax;
		}
		for (int i = 0; i < GridSize; i++)
		{
			int num = magic.page * GridSize + i;
			if (num >= magic.filteredList.Count)
			{
				break;
			}
			Thing thing = magic.filteredList[num];
			grid[i] = thing;
			thing.invX = i;
		}
		magic.RefreshCats();
	}

	public bool IsOccupied(int x, int y)
	{
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Thing current = enumerator.Current;
				if (current.invY == y && current.invX == x)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool ShouldShowOnGrid(Thing t)
	{
		if (!owner.IsPC)
		{
			if (t.trait is TraitChestMerchant)
			{
				return false;
			}
			return true;
		}
		if (!t.isEquipped)
		{
			return !t.IsHotItem;
		}
		return false;
	}

	public void OnAdd(Thing t)
	{
		if (HasGrid && ShouldShowOnGrid(t))
		{
			int freeGridIndex = GetFreeGridIndex();
			if (freeGridIndex != -1)
			{
				grid[freeGridIndex] = t;
			}
			t.pos.x = freeGridIndex;
		}
	}

	public bool IsFull(int y = 0)
	{
		if (IsMagicChest)
		{
			if (base.Count < MaxCapacity)
			{
				if (owner.trait.IsFridge)
				{
					return !owner.isOn;
				}
				return false;
			}
			return true;
		}
		if (y != 0)
		{
			return false;
		}
		if (owner.trait.IsSpecialContainer && owner.parent != null)
		{
			return true;
		}
		if (!HasGrid)
		{
			return base.Count >= GridSize;
		}
		return GetFreeGridIndex() == -1;
	}

	public bool IsOverflowing()
	{
		if (!HasGrid || IsMagicChest)
		{
			return false;
		}
		int num = 0;
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Thing current = enumerator.Current;
				if (current.invY != 1 && !current.isEquipped)
				{
					num++;
				}
			}
		}
		if (num > grid.Count)
		{
			return true;
		}
		return false;
	}

	public int GetFreeGridIndex()
	{
		for (int i = 0; i < grid.Count; i++)
		{
			if (grid[i] == null)
			{
				return i;
			}
		}
		return -1;
	}

	public void OnRemove(Thing t)
	{
		if (HasGrid && t.invY == 0)
		{
			int num = grid.IndexOf(t);
			if (num != -1)
			{
				grid[num] = null;
			}
		}
	}

	public void SetSize(int w, int h)
	{
		owner.c_containerSize = w * 100 + h;
		SetOwner(owner);
	}

	public Thing TryStack(Thing target, int destInvX = -1, int destInvY = -1)
	{
		using Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			Thing current = enumerator.Current;
			if (destInvX == -1 && current.CanSearchContents && owner.GetRootCard().IsPC)
			{
				Thing thing = current.things.TryStack(target, destInvX, destInvY);
				if (thing != target)
				{
					return thing;
				}
			}
			if ((destInvX == -1 || (current.invX == destInvX && current.invY == destInvY)) && current != target && target.TryStackTo(current))
			{
				return current;
			}
		}
		return target;
	}

	public Thing CanStack(Thing target, int destInvX = -1, int destInvY = -1)
	{
		using Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			Thing current = enumerator.Current;
			if (current != target && target.CanStackTo(current))
			{
				return current;
			}
		}
		return target;
	}

	public DestData GetDest(Thing t, bool tryStack = true)
	{
		DestData d = default(DestData);
		if (!owner.IsPC)
		{
			SearchDest(this, searchEmpty: true, searchStack: true);
			return d;
		}
		if (t.trait.CanOnlyCarry && IsFull())
		{
			return d;
		}
		ContainerFlag flag = t.category.GetRoot().id.ToEnum<ContainerFlag>();
		if (flag == ContainerFlag.none)
		{
			flag = ContainerFlag.other;
		}
		_listContainers.Clear();
		_listContainers.Add(this);
		TrySearchContainer(owner);
		_listContainers.Sort((ThingContainer a, ThingContainer b) => (b.owner.GetWindowSaveData()?.priority ?? 0) * 10 + (b.owner.IsPC ? 1 : 0) - ((a.owner.GetWindowSaveData()?.priority ?? 0) * 10 + (a.owner.IsPC ? 1 : 0)));
		if (tryStack)
		{
			foreach (ThingContainer listContainer in _listContainers)
			{
				SearchDest(listContainer, searchEmpty: false, searchStack: true);
				if (d.IsValid)
				{
					return d;
				}
			}
		}
		foreach (ThingContainer listContainer2 in _listContainers)
		{
			SearchDest(listContainer2, searchEmpty: true, searchStack: false);
			if (d.IsValid)
			{
				return d;
			}
		}
		return d;
		void SearchDest(ThingContainer things, bool searchEmpty, bool searchStack)
		{
			if (!t.IsContainer || t.things.Count <= 0 || !things.owner.IsContainer || things.owner.trait is TraitToolBelt)
			{
				if (searchStack && tryStack)
				{
					Thing thing = things.CanStack(t);
					if (thing != t)
					{
						d.stack = thing;
						return;
					}
				}
				if (searchEmpty && !things.IsFull() && !(things.owner.trait is TraitToolBelt) && (things.owner.isChara || things.owner.parent is Chara || (things.owner.parent as Thing)?.trait is TraitToolBelt))
				{
					d.container = things.owner;
				}
			}
		}
		void TrySearchContainer(Card c)
		{
			foreach (Thing thing2 in c.things)
			{
				if (thing2.CanSearchContents)
				{
					TrySearchContainer(thing2);
				}
			}
			if (c.things != this)
			{
				Window.SaveData windowSaveData = c.GetWindowSaveData();
				if (windowSaveData != null && (!windowSaveData.noRotten || !t.IsDecayed) && (!windowSaveData.onlyRottable || t.trait.Decay != 0))
				{
					if (windowSaveData.userFilter)
					{
						switch (windowSaveData.IsFilterPass(t.GetName(NameStyle.Full, 1)))
						{
						case Window.SaveData.FilterResult.Block:
							return;
						case Window.SaveData.FilterResult.PassWithoutFurtherTest:
							_listContainers.Add(c.things);
							return;
						}
					}
					if (windowSaveData.advDistribution)
					{
						foreach (int cat in windowSaveData.cats)
						{
							if (t.category.uid == cat)
							{
								_listContainers.Add(c.things);
								break;
							}
						}
						return;
					}
					if (!windowSaveData.flag.HasFlag(flag))
					{
						_listContainers.Add(c.things);
					}
				}
			}
		}
	}

	public bool IsFull(Thing t, bool recursive = true, bool tryStack = true)
	{
		if (!IsFull() || (tryStack && CanStack(t) != t))
		{
			return false;
		}
		if (!recursive)
		{
			return true;
		}
		return !GetDest(t, tryStack).IsValid;
	}

	public void AddCurrency(Card owner, string id, int a, SourceMaterial.Row mat = null)
	{
		int num = a;
		ListCurrency(id);
		foreach (Thing temp in tempList)
		{
			if (!(temp.id != id) && (mat == null || temp.material == mat))
			{
				if (num > 0)
				{
					temp.ModNum(num);
					return;
				}
				if (temp.Num + num >= 0)
				{
					temp.ModNum(num);
					return;
				}
				num += temp.Num;
				temp.ModNum(-temp.Num);
			}
		}
		if (num != 0 && num > 0)
		{
			Thing thing = ThingGen.Create(id);
			if (mat != null)
			{
				thing.ChangeMaterial(mat);
			}
			owner.AddThing(thing).SetNum(num);
		}
	}

	public void DestroyAll(Func<Thing, bool> funcExclude = null)
	{
		this.ForeachReverse(delegate(Thing t)
		{
			if (funcExclude == null || !funcExclude(t))
			{
				t.Destroy();
				Remove(t);
			}
		});
		if (grid != null)
		{
			for (int i = 0; i < grid.Count; i++)
			{
				grid[i] = null;
			}
		}
	}

	public Thing Find(int uid)
	{
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Thing current = enumerator.Current;
				if (current.CanSearchContents)
				{
					Thing thing = current.things.Find(uid);
					if (thing != null)
					{
						return thing;
					}
				}
				if (current.uid == uid)
				{
					return current;
				}
			}
		}
		return null;
	}

	public Thing Find<T>() where T : Trait
	{
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Thing current = enumerator.Current;
				if (current.CanSearchContents)
				{
					Thing thing = current.things.Find<T>();
					if (thing != null)
					{
						return thing;
					}
				}
				if (current.trait is T)
				{
					return current;
				}
			}
		}
		return null;
	}

	public Thing FindBest<T>(Func<Thing, int> func) where T : Trait
	{
		List((Thing t) => t.trait is T, onlyAccessible: true);
		if (tempList.Count == 0)
		{
			return null;
		}
		tempList.Sort((Thing a, Thing b) => func(b) - func(a));
		return tempList[0];
	}

	public Thing Find(Func<Thing, bool> func, bool recursive = true)
	{
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Thing current = enumerator.Current;
				if (recursive && current.CanSearchContents)
				{
					Thing thing = current.things.Find(func);
					if (thing != null)
					{
						return thing;
					}
				}
				if (func(current))
				{
					return current;
				}
			}
		}
		return null;
	}

	public Thing Find(string id, string idMat)
	{
		return Find(id, idMat.IsEmpty() ? (-1) : EClass.sources.materials.alias[idMat].id);
	}

	public Thing Find(string id, int idMat = -1, int refVal = -1)
	{
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Thing current = enumerator.Current;
				if (current.CanSearchContents)
				{
					Thing thing = current.things.Find(id, idMat, refVal);
					if (thing != null)
					{
						return thing;
					}
				}
				if (current.id == id && (idMat == -1 || current.material.id == idMat) && (refVal == -1 || current.refVal == refVal))
				{
					return current;
				}
			}
		}
		return null;
	}

	public Thing FindStealable()
	{
		List<Thing> list = new List<Thing>();
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Thing current = enumerator.Current;
				if (!current.IsContainer && current.trait.CanBeStolen && !current.HasTag(CTAG.gift))
				{
					list.Add(current);
				}
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		list.Sort((Thing a, Thing b) => Compare(a) - Compare(b));
		return list[0];
		static int Compare(Thing a)
		{
			return a.SelfWeight + (a.isEquipped ? 10000 : 0);
		}
	}

	public ThingStack GetThingStack(string id, int refVal = -1)
	{
		ThingStack s = new ThingStack();
		bool isOrigin = EClass.sources.cards.map[id].isOrigin;
		return GetThingStack(id, s, isOrigin, refVal);
	}

	public ThingStack GetThingStack(string id, ThingStack s, bool isOrigin, int refVal = -1)
	{
		using Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			Thing current = enumerator.Current;
			if (current.CanSearchContents)
			{
				current.things.GetThingStack(id, s, isOrigin, refVal);
			}
			if ((refVal == -1 || current.refVal == refVal) && current.IsIdentified && (current.id == id || (isOrigin && current.source._origin == id)))
			{
				s.Add(current);
			}
		}
		return s;
	}

	public ThingStack GetThingStack(string id)
	{
		ThingStack s = new ThingStack();
		bool isOrigin = EClass.sources.cards.map[id].isOrigin;
		return GetThingStack(id, s, isOrigin);
	}

	public ThingStack GetThingStack(string id, ThingStack s, bool isOrigin)
	{
		using Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			Thing current = enumerator.Current;
			if (current.CanSearchContents)
			{
				current.things.GetThingStack(id, s, isOrigin);
			}
			if (current.id == id || (isOrigin && current.source._origin == id))
			{
				s.Add(current);
			}
		}
		return s;
	}

	public int GetCurrency(string id, ref int sum, SourceMaterial.Row mat = null)
	{
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Thing current = enumerator.Current;
				if (current.CanSearchContents)
				{
					current.things.GetCurrency(id, ref sum, mat);
				}
				if (current.id == id && (mat == null || current.material == mat))
				{
					sum += current.Num;
				}
			}
		}
		return sum;
	}

	public List<Thing> ListCurrency(string id)
	{
		tempList.Clear();
		_ListCurrency(id);
		return tempList;
	}

	private void _ListCurrency(string id)
	{
		using Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			Thing current = enumerator.Current;
			if (current.CanSearchContents)
			{
				current.things._ListCurrency(id);
			}
			if (current.id == id)
			{
				tempList.Add(current);
			}
		}
	}

	public List<Thing> List(Func<Thing, bool> func, bool onlyAccessible = false)
	{
		tempList.Clear();
		_List(func, onlyAccessible);
		return tempList;
	}

	public void _List(Func<Thing, bool> func, bool onlyAccessible = false)
	{
		if (onlyAccessible && !owner.trait.CanSearchContent)
		{
			return;
		}
		using Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			Thing current = enumerator.Current;
			current.things._List(func, onlyAccessible);
			if (func(current))
			{
				tempList.Add(current);
			}
		}
	}

	public void AddFactory(HashSet<string> hash)
	{
		using Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			Thing current = enumerator.Current;
			if (current.CanSearchContents)
			{
				current.things.AddFactory(hash);
			}
			if (current.trait.IsFactory)
			{
				hash.Add(current.id);
			}
			if (current.trait.ToggleType == ToggleType.Fire && current.isOn)
			{
				hash.Add("fire");
			}
		}
	}

	public void Foreach(Action<Thing> action, bool onlyAccessible = true)
	{
		using Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			Thing current = enumerator.Current;
			if (!onlyAccessible || current.CanSearchContents)
			{
				current.things.Foreach(action);
			}
			action(current);
		}
	}

	public void Foreach(Func<Thing, bool> action, bool onlyAccessible = true)
	{
		using Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			Thing current = enumerator.Current;
			if (!onlyAccessible || current.CanSearchContents)
			{
				current.things.Foreach(action);
			}
			if (action(current))
			{
				break;
			}
		}
	}
}
