using System;
using System.IO;
using System.Text;
using IniParser;
using IniParser.Model;
using UnityEngine;

public class SpriteData
{
	public string id;

	public string path;

	public Texture2D tex;

	public Sprite[] sprites;

	public DateTime dateTex;

	public DateTime dateIni;

	public DateTime datePref;

	public int frame = 1;

	public int scale = 50;

	public float time = 0.2f;

	public SourcePref pref;

	public bool tryFixPrefNotLoadedAtStart;

	public void Init()
	{
		try
		{
			id = Path.GetFileNameWithoutExtension(path);
			LoadAnimationIni();
			LoadPref();
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			Debug.LogError("#sprite failed " + id + "/" + path);
		}
	}

	public void LoadPref()
	{
		pref = null;
		if (File.Exists(path + ".pref"))
		{
			pref = SourcePref.ReadFromIni(path + ".pref");
		}
	}

	public void LoadAnimationIni()
	{
		if (!File.Exists(path + ".ini"))
		{
			frame = 1;
			scale = 50;
			time = 0.2f;
			return;
		}
		try
		{
			IniData iniData = new FileIniDataParser().ReadFile(path + ".ini", Encoding.UTF8);
			frame = Mathf.Max(1, (!int.TryParse(iniData.GetKey("frame"), out var result)) ? 1 : result);
			scale = (int.TryParse(iniData.GetKey("scale"), out var result2) ? result2 : 50);
			time = (float.TryParse(iniData.GetKey("time"), out var result3) ? Mathf.Max(0.016f, result3) : 0.2f);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	public void LoadSprites()
	{
		Texture2D texture2D = IO.LoadPNG(path + ".png");
		Debug.Log(texture2D.width + "/" + texture2D.height + "/" + path);
		if ((bool)tex)
		{
			if (texture2D.width != tex.width || texture2D.height != tex.height)
			{
				tex.Reinitialize(texture2D.width, texture2D.height);
			}
			tex.SetPixels32(texture2D.GetPixels32());
			tex.Apply();
			UnityEngine.Object.Destroy(texture2D);
		}
		else
		{
			tex = texture2D;
		}
		int num = tex.width / frame;
		int height = tex.height;
		if (sprites == null || sprites.Length != frame)
		{
			sprites = new Sprite[frame];
		}
		Vector2 pivot = new Vector2(0.5f, 0.5f);
		for (int i = 0; i < frame; i++)
		{
			sprites[i] = Sprite.Create(tex, new Rect(i * num, 0f, num, height), pivot, 100f, 0u, SpriteMeshType.FullRect);
		}
	}

	public Sprite[] GetSprites()
	{
		if (sprites == null)
		{
			Load();
		}
		return sprites;
	}

	public Sprite GetSprite()
	{
		if (sprites == null)
		{
			Load();
		}
		return sprites.TryGet(0, returnNull: true);
	}

	public void Validate()
	{
		if (IsDirty(".png", ref dateTex) || IsDirty(".ini", ref dateIni) || IsDirty(".pref", ref datePref))
		{
			Load();
		}
		bool IsDirty(string p, ref DateTime lastChecked)
		{
			string text = path + p;
			if (!File.Exists(text))
			{
				return false;
			}
			DateTime lastWriteTimeUtc = File.GetLastWriteTimeUtc(text);
			if (lastWriteTimeUtc == lastChecked)
			{
				return false;
			}
			lastChecked = lastWriteTimeUtc;
			return true;
		}
	}

	public void Load()
	{
		LoadAnimationIni();
		LoadPref();
		LoadSprites();
	}
}
