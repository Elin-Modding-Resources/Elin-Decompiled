using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class SpriteReplacer
{
	public static Dictionary<string, SpriteReplacer> dictSkins = new Dictionary<string, SpriteReplacer>();

	public static Dictionary<string, SpriteReplacer> dictOverrides = new Dictionary<string, SpriteReplacer>();

	public static List<(string suffix, Func<Card, bool> when)> states = new List<(string, Func<Card, bool>)>();

	public static Dictionary<string, string> dictModItems = new Dictionary<string, string>();

	private static List<string> sortedModIds = new List<string>();

	private static bool modIdsDirty = true;

	private static Dictionary<(int, int, bool, string), List<string>> suffixKeys = new Dictionary<(int, int, bool, string), List<string>>();

	private static HashSet<string> failedStates = new HashSet<string>();

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
		Dictionary<string, string> dictionary = ListSkinItems();
		List<string> sortedIds = dictionary.Keys.OrderBy((string k) => k).ToList();
		foreach (KeyValuePair<string, string> item2 in dictionary)
		{
			item2.Deconstruct(out var key, out var value);
			string text = key;
			string path = value;
			SpriteReplacer spriteReplacer = new SpriteReplacer
			{
				data = new SpriteData
				{
					path = path
				}
			};
			spriteReplacer.BuildSuffixData(text, dictionary, sortedIds);
			if (spriteReplacer.suffixes.TryGetValue("", out var value2))
			{
				spriteReplacer.data = value2;
			}
			else
			{
				spriteReplacer.data.Init();
			}
			dictSkins[text] = spriteReplacer;
		}
		return dictSkins;
	}

	public static Dictionary<string, string> ListSkinItems()
	{
		List<DirectoryInfo> list = new List<DirectoryInfo>();
		list.Add(new DirectoryInfo(CorePath.custom + "Skin"));
		list.AddRange(PackageIterator.GetDirectories("Skin", useCache: false));
		IEnumerable<FileInfo> enumerable = list.SelectMany((DirectoryInfo d) => from f in d.GetFiles("*.png", SearchOption.TopDirectoryOnly)
			orderby f.Name
			select f);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (FileInfo item in enumerable)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(item.Name);
			string value = Path.ChangeExtension(item.FullName, null);
			dictionary[fileNameWithoutExtension] = value;
		}
		return dictionary;
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
		return GetData(dir, skin, snow)?.GetSprite();
	}

	public SpriteData GetData(int dir, int skin, bool snow, Card card = null)
	{
		SpriteData spriteData = FindData(dir, skin, snow, "");
		if (card == null)
		{
			return spriteData;
		}
		foreach (var state in states)
		{
			string item = state.suffix;
			Func<Card, bool> item2 = state.when;
			SpriteData spriteData2 = FindData(dir, skin, snow, item);
			if (spriteData2 == spriteData)
			{
				continue;
			}
			try
			{
				if (item2(card))
				{
					return spriteData2;
				}
			}
			catch (Exception arg)
			{
				if (failedStates.Add(item))
				{
					Debug.LogError($"#sprite failed to load state {item} on {card.id}\n{arg}");
				}
			}
		}
		return spriteData;
	}

	public SpriteData FindData(int dir, int skin, bool snow, string state)
	{
		if (!suffixKeys.TryGetValue((dir, skin, snow, state), out var value))
		{
			value = new List<string>
			{
				$"_skin{skin}_dir{dir}",
				$"_skin{skin}",
				$"_dir{dir}",
				""
			};
			if (snow)
			{
				value = value.SelectMany((string k) => new string[2]
				{
					k + "_snow",
					k
				}).ToList();
			}
			if (state != "")
			{
				value = value.Select((string k) => state + k).Concat(value).ToList();
			}
			suffixKeys[(dir, skin, snow, state)] = value;
		}
		foreach (string item in value)
		{
			if (suffixes.TryGetValue(item, out var value2))
			{
				Sprite[] sprites = value2.sprites;
				if (sprites == null || sprites.Length != 0)
				{
					return value2;
				}
			}
		}
		return data;
	}

	public void Validate()
	{
		data?.Validate();
		foreach (SpriteData value in suffixes.Values)
		{
			value?.Validate();
		}
	}

	public void BuildSuffixData(string id, IReadOnlyDictionary<string, string> dictTexItems, List<string> sortedIds = null)
	{
		List<string> list = sortedIds ?? dictTexItems.Keys.OrderBy((string k) => k).ToList();
		int num = list.BinarySearch(id);
		if (num < 0)
		{
			num = ~num;
		}
		for (int num2 = num; num2 < list.Count; num2++)
		{
			string text = list[num2];
			if (!text.StartsWith(id))
			{
				break;
			}
			string text2 = text[id.Length..];
			if (text2.StartsWith("_"))
			{
				text2 = string.Concat(from t in text2.Split('_').Skip(1)
					orderby (!(t == "snow")) ? ((!Regex.IsMatch(t, "^dir\\d+$")) ? (Regex.IsMatch(t, "^skin\\d+$") ? 1 : 0) : 2) : 3
					select "_" + t);
			}
			SpriteData spriteData = new SpriteData
			{
				path = dictTexItems[text]
			};
			spriteData.Init();
			suffixes[text2] = spriteData;
			Debug.Log("#sprite replacer " + text2.IsEmpty("<base>") + "/" + dictTexItems[text].ShortPath());
		}
	}

	public void SortModItemIds()
	{
		if (sortedModIds.Count != dictModItems.Count || modIdsDirty)
		{
			sortedModIds = dictModItems.Keys.OrderBy((string k) => k).ToList();
			modIdsDirty = false;
		}
	}

	public void Reload(string id, RenderData renderData = null)
	{
		data = null;
		suffixes.Clear();
		try
		{
			SortModItemIds();
			if (dictModItems.ContainsKey(id))
			{
				BuildSuffixData(id, dictModItems, sortedModIds);
			}
			else if (renderData != null && dictModItems.ContainsKey("Item/" + id))
			{
				BuildSuffixData("Item/" + id, dictModItems, sortedModIds);
			}
			suffixes.TryGetValue("", out data);
			if (data != null)
			{
				Debug.Log(id + ":" + data.path);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("#sprite error fetching sprite replacer '" + id + "' : " + ex);
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
