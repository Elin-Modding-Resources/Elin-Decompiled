using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
[ComVisible(false)]
[DebuggerDisplay("Count = {Count}")]
public class UDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IDictionary, ICollection, ISerializable, IDeserializationCallback, ISerializationCallbackReceiver
{
	[SerializeField]
	private List<TKey> keys = new List<TKey>();

	[SerializeField]
	private List<TValue> values = new List<TValue>();

	[NonSerialized]
	private Dictionary<TKey, TValue> dictionary;

	public bool IsFixedSize => false;

	public ICollection<TKey> Keys => dictionary.Keys;

	ICollection IDictionary.Keys => dictionary.Keys;

	public ICollection<TValue> Values => dictionary.Values;

	ICollection IDictionary.Values => dictionary.Values;

	public TValue this[TKey key]
	{
		get
		{
			return dictionary[key];
		}
		set
		{
			dictionary[key] = value;
		}
	}

	object IDictionary.this[object key]
	{
		get
		{
			if (!(key is TKey))
			{
				return null;
			}
			return dictionary[(TKey)key];
		}
		set
		{
			if (key is TKey && (value is TValue || value == null))
			{
				dictionary[(TKey)key] = (TValue)value;
			}
		}
	}

	public int Count => dictionary.Count;

	public bool IsReadOnly => false;

	public bool IsSynchronized => false;

	public object SyncRoot => null;

	public UDictionary()
	{
		dictionary = new Dictionary<TKey, TValue>();
	}

	public UDictionary(IEqualityComparer<TKey> comparer)
	{
		dictionary = new Dictionary<TKey, TValue>(comparer);
	}

	public UDictionary(IDictionary<TKey, TValue> dictionary)
	{
		this.dictionary = new Dictionary<TKey, TValue>(dictionary);
	}

	public UDictionary(int capacity)
	{
		dictionary = new Dictionary<TKey, TValue>(capacity);
	}

	public UDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
	{
		this.dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
	}

	public UDictionary(int capacity, IEqualityComparer<TKey> comparer)
	{
		dictionary = new Dictionary<TKey, TValue>(capacity, comparer);
	}

	public void OnAfterDeserialize()
	{
		dictionary.Clear();
		for (int i = 0; i < keys.Count; i++)
		{
			if (keys[i] != null && (!(keys[i] is UnityEngine.Object) || (bool)(UnityEngine.Object)(object)keys[i]))
			{
				dictionary.Add(keys[i], values[i]);
			}
		}
	}

	public void OnBeforeSerialize()
	{
		keys.Clear();
		values.Clear();
		foreach (KeyValuePair<TKey, TValue> item in dictionary)
		{
			if (item.Key != null && (!(item.Key is UnityEngine.Object) || (bool)(UnityEngine.Object)(object)item.Key))
			{
				keys.Add(item.Key);
				values.Add(item.Value);
			}
		}
	}

	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		dictionary.GetObjectData(info, context);
	}

	public void OnDeserialization(object sender)
	{
		dictionary.OnDeserialization(sender);
	}

	public void Add(TKey key, TValue value)
	{
		dictionary.Add(key, value);
	}

	void IDictionary.Add(object key, object value)
	{
		if (key is TKey && (value is TValue || value == null))
		{
			dictionary.Add((TKey)key, (TValue)value);
		}
	}

	public bool ContainsKey(TKey key)
	{
		return dictionary.ContainsKey(key);
	}

	bool IDictionary.Contains(object key)
	{
		if (!(key is TKey))
		{
			return false;
		}
		return dictionary.ContainsKey((TKey)key);
	}

	public bool Remove(TKey key)
	{
		return dictionary.Remove(key);
	}

	void IDictionary.Remove(object key)
	{
		if (key is TKey)
		{
			dictionary.Remove((TKey)key);
		}
	}

	public bool TryGetValue(TKey key, out TValue value)
	{
		return dictionary.TryGetValue(key, out value);
	}

	IDictionaryEnumerator IDictionary.GetEnumerator()
	{
		return ((IDictionary)dictionary).GetEnumerator();
	}

	public void Add(KeyValuePair<TKey, TValue> item)
	{
		dictionary.Add(item.Key, item.Value);
	}

	public void Clear()
	{
		dictionary.Clear();
	}

	public bool Contains(KeyValuePair<TKey, TValue> item)
	{
		if (dictionary.ContainsKey(item.Key))
		{
			return dictionary[item.Key].Equals(item.Value);
		}
		return false;
	}

	void ICollection.CopyTo(Array array, int index)
	{
	}

	void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
	}

	public bool Remove(KeyValuePair<TKey, TValue> item)
	{
		return dictionary.Remove(item.Key);
	}

	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		return dictionary.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return dictionary.GetEnumerator();
	}
}
