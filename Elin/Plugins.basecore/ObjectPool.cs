using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
	public interface IItem
	{
		void Update();
	}

	public class Item<T> : IItem where T : new()
	{
		public Stack<T> pools;

		public int max;

		public int addsPerUpdate;

		public Action<T> onCreate;

		public Component mold;

		public void Create(int _max, int _addsPerUpdate = 10)
		{
			max = _max;
			addsPerUpdate = _addsPerUpdate;
			pools = new Stack<T>(_max);
		}

		public T Get(Component parent)
		{
			T obj = ((pools.Count > 0) ? pools.Pop() : Create());
			Component obj2 = obj as Component;
			obj2.transform.SetParent(parent.transform, worldPositionStays: false);
			obj2.SetActive(enable: true);
			return obj;
		}

		public T Get()
		{
			if ((bool)mold)
			{
				Debug.LogError("tried to Get component");
			}
			if (pools.Count > 0)
			{
				return pools.Pop();
			}
			return Create();
		}

		public void Update()
		{
			for (int i = 0; i < addsPerUpdate; i++)
			{
				if (pools.Count >= max)
				{
					break;
				}
				pools.Push(Create());
			}
		}

		public T Create()
		{
			T val;
			if ((bool)mold)
			{
				val = Util.Instantiate(mold, Instance).GetComponent<T>();
				(val as Component).SetActive(enable: false);
			}
			else
			{
				val = new T();
			}
			if (onCreate != null)
			{
				onCreate(val);
			}
			return val;
		}

		public Item<T> SetMold(Component _mold)
		{
			mold = _mold;
			return this;
		}

		public override string ToString()
		{
			return typeof(T).Name + ":" + pools.Count;
		}
	}

	public static ObjectPool Instance;

	public List<IItem> items = new List<IItem>();

	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
		foreach (IItem item in items)
		{
			item.Update();
		}
	}

	public static Item<T> Create<T>(int max = 100, int addsPerUpdate = 10, Action<T> _onCreate = null) where T : new()
	{
		Item<T> item = new Item<T>();
		item.Create(max, addsPerUpdate);
		item.onCreate = _onCreate;
		Instance.items.Add(item);
		return item;
	}
}
