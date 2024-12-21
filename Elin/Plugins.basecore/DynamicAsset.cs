using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DynamicAsset<T> where T : MonoBehaviour
{
	public string groupId = "";

	public List<T> list = new List<T>();

	public Dictionary<string, T> map;

	public bool usePool;

	public bool instantiate = true;

	private bool initialized;

	public DynamicAsset()
	{
	}

	public DynamicAsset(string groupId, bool instantiate)
	{
		this.groupId = groupId;
		this.instantiate = instantiate;
	}

	private void Init()
	{
		map = new Dictionary<string, T>();
		foreach (T item in list)
		{
			map.Add(item.name, item);
		}
		initialized = true;
	}

	public T GetNew(string id, Transform parent = null)
	{
		T original = GetOriginal(id);
		if (!instantiate || original == null)
		{
			return original;
		}
		T val = (usePool ? PoolManager.Spawn<T>(groupId + "/" + id, groupId + "/" + id) : UnityEngine.Object.Instantiate(original).GetComponent<T>());
		if ((bool)parent)
		{
			val.transform.SetParent(parent, worldPositionStays: false);
		}
		return val;
	}

	public T GetOriginal(string id)
	{
		if (!initialized)
		{
			Init();
		}
		if (map.TryGetValue(id, out var value))
		{
			return value;
		}
		value = Resources.Load<T>(groupId + "/" + id);
		map[id] = value;
		list.Add(value);
		return value;
	}
}
