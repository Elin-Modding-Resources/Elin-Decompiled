using System.IO;
using UnityEngine;

public class ModItem<T> where T : Object
{
	public ModItemList<T> list;

	public string category;

	public string id;

	public string resourcePath;

	public string parent;

	public string parentParent;

	public FileInfo fileInfo;

	public T cache;

	public ModItem(FileInfo _fileInfo, string path = null, ModItemList<T> _list = null, string desiredID = null)
	{
		list = _list;
		resourcePath = path;
		Set(_fileInfo, path, desiredID);
	}

	public void Set(FileInfo _fileInfo, string path, string desiredID = null)
	{
		if (_fileInfo != null)
		{
			fileInfo = _fileInfo;
			parent = ((fileInfo != null) ? fileInfo.Directory.Name : Path.GetDirectoryName(path));
			parentParent = ((fileInfo != null) ? fileInfo.Directory.Parent.Name : Path.GetDirectoryName(path.Split('/')[0]));
			id = desiredID ?? Path.GetFileNameWithoutExtension((fileInfo != null) ? fileInfo.Name : path);
			if (list != null && list.catLength > 0)
			{
				category = id.Substring(0, list.catLength);
			}
		}
	}

	public T GetObject(object option = null)
	{
		if (fileInfo == null)
		{
			return null;
		}
		if (list == null)
		{
			if (cache == null)
			{
				cache = IO.LoadObject<T>(fileInfo, option);
			}
			return cache;
		}
		if (resourcePath != null)
		{
			return list.cache.Get(resourcePath, ResourceLoadType.Resource, option);
		}
		return list.cache.Get(fileInfo.FullName, ResourceLoadType.Streaming, option);
	}

	public void ClearCache()
	{
		if (list == null)
		{
			if ((bool)cache)
			{
				Object.DestroyImmediate(cache);
			}
		}
		else
		{
			list.cache.Clear();
		}
	}
}
