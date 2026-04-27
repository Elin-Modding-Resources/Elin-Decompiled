using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SpriteReplacer
{
	public static Dictionary<string, SpriteReplacer> dictSkins = new Dictionary<string, SpriteReplacer>();

	public static Dictionary<string, string> dictModItems = new Dictionary<string, string>();

	public static Dictionary<string, string> dictTextureItems = new Dictionary<string, string>();

	public SpriteData data;

	public Dictionary<string, SpriteData> suffixes = new Dictionary<string, SpriteData>();

	public Dictionary<string, bool> isChecked = new Dictionary<string, bool>();

	public static Dictionary<string, SpriteReplacer> ListSkins()
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, SpriteReplacer> dictSkin in dictSkins)
		{
			if (!File.Exists(dictSkin.Value.data.path + ".png"))
			{
				list.Add(dictSkin.Key);
			}
		}
		foreach (string item in list)
		{
			dictSkins.Remove(item);
		}
		Dictionary<string, string> dictionary = new DirectoryInfo(CorePath.custom + "Skin").GetFiles("*.png").ToDictionary((FileInfo f) => Path.GetFileNameWithoutExtension(f.Name), (FileInfo f) => Path.ChangeExtension(f.FullName, null));
		string key2;
		foreach (KeyValuePair<string, string> item2 in dictionary)
		{
			item2.Deconstruct(out var key, out key2);
			string key3 = key;
			string path = key2;
			if (!dictSkins.ContainsKey(key3))
			{
				dictSkins[key3] = new SpriteReplacer
				{
					data = new SpriteData
					{
						path = path
					}
				};
			}
		}
		foreach (KeyValuePair<string, SpriteReplacer> dictSkin2 in dictSkins)
		{
			dictSkin2.Deconstruct(out key2, out var value);
			string id = key2;
			value.BuildSuffixData(id, dictionary);
		}
		return dictSkins;
	}

	public Sprite GetSprite(string suffix = "")
	{
		if (!suffixes.TryGetValue(suffix, out var value))
		{
			return null;
		}
		return value.GetSprite();
	}

	public Sprite GetSprite(int dir, int skin, bool snow)
	{
		foreach (string item in new List<string>
		{
			$"_skin{skin}_dir{dir}",
			$"_skin{skin}",
			$"_dir{dir}",
			""
		})
		{
			Sprite sprite = null;
			if (snow)
			{
				sprite = GetSprite(item + "_snow");
			}
			if ((object)sprite == null)
			{
				sprite = GetSprite(item);
			}
			if ((bool)sprite)
			{
				return sprite;
			}
		}
		return data?.GetSprite();
	}

	public void Validate()
	{
		data?.Validate();
		foreach (SpriteData value in suffixes.Values)
		{
			value?.Validate();
		}
	}

	public void ReloadBuiltInTextures()
	{
		dictTextureItems = new DirectoryInfo(CorePath.packageCore + "Texture/Item").GetFiles("*.png").ToDictionary((FileInfo f) => Path.GetFileNameWithoutExtension(f.Name), (FileInfo f) => Path.ChangeExtension(f.FullName, null));
	}

	public void BuildSuffixData(string id, Dictionary<string, string> dictTexItems)
	{
		foreach (var (text3, path) in dictTexItems)
		{
			if (text3.StartsWith(id))
			{
				string text4 = text3[id.Length..];
				SpriteData spriteData = new SpriteData
				{
					path = path
				};
				spriteData.Init();
				suffixes[text4] = spriteData;
				Debug.Log("#sprite replacer " + text4.IsEmpty("<base>") + "/" + path.ShortPath());
			}
		}
	}

	public void Reload(string id, RenderData renderData = null)
	{
		data = null;
		suffixes.Clear();
		try
		{
			if (dictModItems.ContainsKey(id))
			{
				BuildSuffixData(id, dictModItems);
			}
			else
			{
				if (dictTextureItems.Count == 0)
				{
					ReloadBuiltInTextures();
				}
				string text = dictTextureItems.TryGetValue(id);
				if (text == null && renderData != null)
				{
					text = dictTextureItems.TryGetValue(renderData.name);
				}
				if (text != null)
				{
					BuildSuffixData(id, dictTextureItems);
				}
			}
			data = suffixes.TryGetValue("");
			if (data != null)
			{
				Debug.Log(id + ":" + data.path);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("#sprite error fetching sprite replacer:" + ex);
		}
		isChecked[id] = true;
	}

	public bool HasSprite(string id, RenderData renderData = null)
	{
		if (!isChecked.GetValueOrDefault(id) || (data != null && data.id != id))
		{
			Reload(id, renderData);
		}
		return data != null;
	}
}
