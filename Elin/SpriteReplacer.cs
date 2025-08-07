using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SpriteReplacer
{
	public static Dictionary<string, SpriteReplacer> dictSkins = new Dictionary<string, SpriteReplacer>();

	public static Dictionary<string, string> dictModItems = new Dictionary<string, string>();

	public bool isChecked;

	public SpriteData data;

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
		FileInfo[] files = new DirectoryInfo(CorePath.custom + "Skin").GetFiles();
		foreach (FileInfo fileInfo in files)
		{
			if (fileInfo.Extension == ".png")
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.FullName);
				if (!dictSkins.ContainsKey(fileNameWithoutExtension))
				{
					SpriteReplacer spriteReplacer = new SpriteReplacer();
					spriteReplacer.data = new SpriteData
					{
						path = fileInfo.GetFullFileNameWithoutExtension()
					};
					spriteReplacer.data.Init();
					dictSkins.Add(fileNameWithoutExtension, spriteReplacer);
				}
			}
		}
		return dictSkins;
	}

	public bool HasSprite(string id)
	{
		if (!isChecked)
		{
			try
			{
				if (dictModItems.ContainsKey(id))
				{
					Debug.Log(id + ":" + dictModItems[id]);
					data = new SpriteData
					{
						path = dictModItems[id]
					};
					data.Init();
				}
				else
				{
					string text = CorePath.packageCore + "Texture/Item/" + id;
					if (File.Exists(text + ".png"))
					{
						data = new SpriteData
						{
							path = text
						};
						data.Init();
					}
				}
				isChecked = true;
			}
			catch (Exception ex)
			{
				Debug.Log("Error during fetching spirte:" + ex);
			}
		}
		return data != null;
	}
}
