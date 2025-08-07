using System;
using System.Globalization;
using System.IO;
using System.Text;
using IniParser;
using IniParser.Model;
using UnityEngine;

public class SpriteData
{
	public DateTime date;

	public Texture2D tex;

	public Texture2D texSnow;

	public string path;

	public Sprite[] sprites;

	public Sprite[] spritesSnow;

	public SourcePref pref;

	public int frame = 1;

	public int scale = 50;

	public float time = 0.2f;

	public void Init()
	{
		try
		{
			if (File.Exists(path + ".ini"))
			{
				IniData iniData = new FileIniDataParser().ReadFile(path + ".ini", Encoding.UTF8);
				frame = int.Parse(iniData.GetKey("frame") ?? "1");
				scale = int.Parse(iniData.GetKey("scale") ?? "50");
				time = float.Parse(iniData.GetKey("time") ?? "0.2", CultureInfo.InvariantCulture);
			}
		}
		catch (Exception message)
		{
			Debug.Log(message);
			Debug.Log("exception: Failed to parse:" + path + ".ini");
			time = 0.2f;
			scale = 50;
		}
	}

	public Sprite[] GetSprites()
	{
		Validate();
		return sprites;
	}

	public void LoadPref()
	{
		pref = null;
		if (File.Exists(path + ".pref"))
		{
			pref = SourcePref.ReadFromIni(path + ".pref");
		}
	}

	public Sprite GetSprite(bool snow = false)
	{
		Validate();
		if (!snow || spritesSnow == null)
		{
			return sprites[0];
		}
		return spritesSnow[0];
	}

	public void Validate()
	{
		DateTime lastWriteTime = File.GetLastWriteTime(path + ".png");
		bool flag = date.Year != 1 && date == lastWriteTime;
		if (!flag)
		{
			Load(flag, ref tex, ref sprites, "");
			if (File.Exists(path + "_snow.png"))
			{
				Load(flag, ref texSnow, ref spritesSnow, "_snow");
			}
			date = lastWriteTime;
		}
	}

	public void Load(bool dateMatched, ref Texture2D tex, ref Sprite[] sprites, string suffix)
	{
		if (dateMatched && (bool)tex)
		{
			return;
		}
		if ((bool)tex)
		{
			Texture2D texture2D = IO.LoadPNG(path + suffix + ".png");
			if (texture2D.width != tex.width || texture2D.height != tex.height)
			{
				Debug.Log(texture2D.width + "/" + texture2D.height + "/" + path);
				tex.Reinitialize(texture2D.width, texture2D.height);
			}
			tex.SetPixels32(texture2D.GetPixels32());
			tex.Apply();
			UnityEngine.Object.Destroy(texture2D);
		}
		else
		{
			tex = IO.LoadPNG(path + suffix + ".png");
			int num = tex.width / frame;
			int height = tex.height;
			sprites = new Sprite[frame];
			for (int i = 0; i < frame; i++)
			{
				sprites[i] = Sprite.Create(tex, new Rect(i * num, 0f, num, height), new Vector2(0.5f, 0.5f), 100f, 0u, SpriteMeshType.FullRect);
			}
		}
		LoadPref();
	}
}
