using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileLookup<T> where T : SourceData.BaseRow
{
	private const int DefaultRange = 1024;

	private readonly Dictionary<int, T> _map;

	private T[] _builtin = Array.Empty<T>();

	private T _fallback;

	private HashSet<int> _missing;

	public T this[int id]
	{
		get
		{
			T[] builtin = _builtin;
			if ((uint)id < (uint)builtin.Length)
			{
				T val = builtin[id];
				if (val != null)
				{
					return val;
				}
			}
			return Resolve(id);
		}
	}

	public TileLookup(Dictionary<int, T> rowsById)
	{
		_map = rowsById;
	}

	public void Build()
	{
		_missing = null;
		_fallback = _map.TryGetValue(0);
		int maxId = -1;
		foreach (int item in _map.Keys.Where((int id) => id > maxId && id < 1024))
		{
			maxId = item;
		}
		_builtin = ((maxId < 0) ? Array.Empty<T>() : new T[maxId + 1]);
		foreach (KeyValuePair<int, T> item2 in _map.Where((KeyValuePair<int, T> kv) => kv.Key <= maxId))
		{
			_builtin[item2.Key] = item2.Value;
		}
	}

	private T Resolve(int id)
	{
		if (_map.TryGetValue(id, out var value))
		{
			return value;
		}
		if (_missing == null)
		{
			_missing = new HashSet<int>();
		}
		if (_missing.Add(id))
		{
			Debug.LogWarning($"#source missing tile row: {typeof(T).DeclaringType?.Name}#{id}");
		}
		return _fallback ?? _map.TryGetValue(0);
	}
}
