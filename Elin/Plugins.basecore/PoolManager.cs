using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
	public class PoolGroup
	{
		public string id;

		public Transform original;

		public Transform holder;

		public List<Transform> pooledList = new List<Transform>();

		public PoolGroup(Transform original, PoolManager manager, string _id)
		{
			id = _id;
			this.original = original;
			holder = new GameObject().transform;
			holder.SetParent(manager.transform);
			holder.name = id;
		}

		public Transform Spawn(Transform parent)
		{
			Transform transform = null;
			if (ignorePool)
			{
				transform = Object.Instantiate(original);
			}
			else if (pooledList.Count != 0)
			{
				transform = pooledList[pooledList.Count - 1];
				pooledList.RemoveAt(pooledList.Count - 1);
			}
			else
			{
				transform = Object.Instantiate(original);
				current.spawnedList[transform.GetHashCode()] = this;
			}
			if (Application.isEditor)
			{
				transform.SetAsLastSibling();
			}
			transform.SetParent(parent, worldPositionStays: false);
			return transform;
		}

		public void Despawn(Transform trans)
		{
			trans.SetParent(holder, worldPositionStays: false);
			pooledList.Add(trans);
		}
	}

	public static PoolManager current;

	public static Transform _trans;

	public static bool ignorePool;

	public Dictionary<string, PoolGroup> groups;

	public Dictionary<int, PoolGroup> spawnedList;

	private void Awake()
	{
		current = this;
		_trans = base.transform;
		spawnedList = new Dictionary<int, PoolGroup>();
		groups = new Dictionary<string, PoolGroup>();
		base.gameObject.SetActive(value: false);
	}

	public static T Spawn<T>(T original, Component parent = null) where T : Component
	{
		return current._Spawn(original.name, original.transform, parent?.transform).GetComponent<T>();
	}

	public static T Spawn<T>(string id, string path, Transform parent = null)
	{
		return current._Spawn(id, path, parent).GetComponent<T>();
	}

	public static Transform Spawn(string id, string path, Transform parent)
	{
		return current._Spawn(id, path, parent);
	}

	public static PoolGroup GetGroup(string id)
	{
		return current.groups[id];
	}

	private Transform _Spawn(string id, string path, Transform parent)
	{
		PoolGroup poolGroup = groups.TryGetValue(id);
		if (poolGroup == null)
		{
			poolGroup = CreateGroup(id, path);
		}
		return poolGroup.Spawn(parent);
	}

	private Transform _Spawn(string id, Transform original, Transform parent)
	{
		PoolGroup poolGroup = groups.TryGetValue(id);
		if (poolGroup == null)
		{
			poolGroup = CreateGroup(id, original);
		}
		return poolGroup.Spawn(parent);
	}

	public PoolGroup CreateGroup(string id, Transform original)
	{
		PoolGroup poolGroup = new PoolGroup(original, this, id);
		groups.Add(id, poolGroup);
		return poolGroup;
	}

	public PoolGroup CreateGroup(string id, string path)
	{
		return CreateGroup(id, Resources.Load<Transform>(path));
	}

	public static void Despawn(Component c)
	{
		if (Application.isPlaying)
		{
			if (ignorePool)
			{
				Object.Destroy(c.gameObject);
			}
			else
			{
				current.spawnedList[c.transform.GetHashCode()].Despawn(c.transform);
			}
		}
	}

	public static bool TryDespawn(Component c)
	{
		if (ignorePool)
		{
			return false;
		}
		PoolGroup poolGroup = current.spawnedList.TryGetValue(c.transform.GetHashCode());
		if (poolGroup == null)
		{
			return false;
		}
		poolGroup.Despawn(c.transform);
		return true;
	}

	public static void DespawnOrDestroy(Component c)
	{
		if (ignorePool || !TryDespawn(c))
		{
			Object.Destroy(c.gameObject);
		}
	}
}
