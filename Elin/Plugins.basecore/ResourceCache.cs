using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResourceCache
{
	public static ResourceCache<Object> caches = new ResourceCache<Object>();

	public static T Load<T>(string path) where T : Object
	{
		return caches.Get<T>(path);
	}

	public static T LoadBundle<T>(string path) where T : Object
	{
		return caches.Get<T>(path, ResourceLoadType.AssetBundle);
	}
}
public class ResourceCache<T> where T : Object
{
	public Dictionary<string, T> dict = new Dictionary<string, T>();

	public string path;

	public ResourceCache(string _path = "")
	{
		path = _path;
	}

	public void Clear()
	{
		foreach (T value in dict.Values)
		{
			Object.DestroyImmediate(value);
		}
		dict.Clear();
	}

	public T Get(string id, ResourceLoadType loadType = ResourceLoadType.Resource, object option = null)
	{
		T value = null;
		string key = typeof(T).Name + id;
		if (!dict.TryGetValue(key, out value))
		{
			value = ((loadType == ResourceLoadType.Streaming) ? IO.LoadObject<T>(path + id, option) : Resources.Load<T>(path + id));
			dict.Add(key, value);
		}
		return value;
	}

	public T2 Get<T2>(string id, ResourceLoadType loadType = ResourceLoadType.Resource) where T2 : Object
	{
		T value = null;
		string key = typeof(T2).Name + id;
		if (!dict.TryGetValue(key, out value))
		{
			T2 val = null;
			val = ((loadType != 0) ? AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/Bundle/" + id).LoadAsset<GameObject>(new FileInfo(id).Name).GetComponent<T2>() : Resources.Load<T2>(path + id));
			dict.Add(key, val as T);
			return val;
		}
		return value as T2;
	}
}
