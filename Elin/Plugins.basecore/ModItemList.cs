using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ModItemList<T> where T : Object
{
	public ResourceCache<T> cache = new ResourceCache<T>();

	public List<ModItem<T>> list = new List<ModItem<T>>();

	public Dictionary<string, ModItem<T>> dict = new Dictionary<string, ModItem<T>>();

	public int catLength;

	public ModItemList(int _catLength = 0)
	{
		catLength = _catLength;
	}

	public string GetRandomID(string category = null)
	{
		if (list.Count == 0)
		{
			return null;
		}
		ModItem<T> modItem;
		do
		{
			modItem = list[Rand.rnd(list.Count)];
		}
		while (category != null && !(modItem.category == category));
		return modItem.id;
	}

	public string GetNextID(string currentId, int a, bool ignoreCategory = true)
	{
		int num = IndexOf(currentId) + a;
		if (num >= list.Count)
		{
			num = 0;
		}
		if (num < 0)
		{
			num = list.Count - 1;
		}
		return list[num].id;
	}

	public int IndexOf(string id)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].id == id)
			{
				return i;
			}
		}
		return -1;
	}

	public ModItem<T> GetItem(string id, bool returnNull = false)
	{
		ModItem<T> value = null;
		if (dict.TryGetValue(id, out value))
		{
			return value;
		}
		if (!returnNull)
		{
			return list[0];
		}
		return null;
	}

	public T GetObject(string id, object option = null)
	{
		ModItem<T> item = GetItem(id, returnNull: true);
		if (item != null)
		{
			return item.GetObject();
		}
		return null;
	}

	public void Add(FileInfo fi, string path = null, string prefix = "")
	{
		Add(Path.GetFileNameWithoutExtension(prefix + ((fi != null) ? fi.Name : path)), fi, path);
	}

	public void Add(string id, FileInfo fi, string path = null)
	{
		if (dict.ContainsKey(id))
		{
			dict[id].Set(fi, path);
			return;
		}
		list.Add(new ModItem<T>(fi, path, this, id));
		dict.Add(id, list[list.Count - 1]);
	}
}
