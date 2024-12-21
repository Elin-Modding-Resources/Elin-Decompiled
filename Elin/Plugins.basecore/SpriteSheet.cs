using System.Collections.Generic;
using UnityEngine;

public class SpriteSheet : MonoBehaviour
{
	public static Dictionary<string, Sprite> dict = new Dictionary<string, Sprite>();

	public static HashSet<string> loadedPath = new HashSet<string>();

	public static void Add(Sprite sprite)
	{
		dict[sprite.name] = sprite;
	}

	public static void Add(string path)
	{
		if (!loadedPath.Contains(path))
		{
			Sprite[] array = Resources.LoadAll<Sprite>(path);
			foreach (Sprite sprite in array)
			{
				dict[sprite.name] = sprite;
			}
			loadedPath.Add(path);
		}
	}

	public static Sprite Get(string id)
	{
		return dict.TryGetValue(id);
	}

	public static Sprite Get(string path, string id)
	{
		if (!loadedPath.Contains(path))
		{
			Add(path);
		}
		dict.TryGetValue(id, out var value);
		return value;
	}
}
