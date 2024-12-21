using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SpriteReplacer
{
	public static Dictionary<string, string> dictModItems = new Dictionary<string, string>();

	public bool hasChacked;

	public SpriteData data;

	public bool HasSprite(string id)
	{
		if (!hasChacked)
		{
			try
			{
				string text = CorePath.packageCore + "Texture/Item/" + id;
				if (dictModItems.ContainsKey(id))
				{
					Debug.Log(id + ":" + dictModItems[id]);
					data = new SpriteData
					{
						path = dictModItems[id]
					};
					data.Init();
				}
				else if (File.Exists(text + ".png"))
				{
					data = new SpriteData
					{
						path = text
					};
					data.Init();
				}
				hasChacked = true;
			}
			catch (Exception ex)
			{
				Debug.Log("Error during fetching spirte:" + ex);
			}
		}
		return data != null;
	}
}
