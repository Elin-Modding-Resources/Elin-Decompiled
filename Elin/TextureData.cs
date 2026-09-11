using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TextureData : EScriptable
{
	[Serializable]
	public class Date
	{
		public long ticks;

		public DateTime DateTime
		{
			get
			{
				return new DateTime(ticks);
			}
			set
			{
				ticks = value.Ticks;
			}
		}
	}

	public enum Type
	{
		Pass,
		World,
		DestTex
	}

	public string id;

	public Type type;

	public Date date;

	public Texture2D tex;

	public bool forceRefresh;

	public int tileW;

	public int tileH;

	[NonSerialized]
	public int extraRows;

	[NonSerialized]
	public string path;

	[NonSerialized]
	public string texName;

	[NonSerialized]
	public List<MeshPass> listPass = new List<MeshPass>();

	[NonSerialized]
	public Dictionary<int, TextureReplace> dictReplace = new Dictionary<int, TextureReplace>();

	[NonSerialized]
	public List<TextureReplace> listReplaceLocal = new List<TextureReplace>();

	public void TryRefresh()
	{
		DateTime lastWriteTime = File.GetLastWriteTime(path);
		bool flag = date.DateTime.Year != 1 && date.DateTime.Equals(lastWriteTime);
		if (dictReplace.Count > 0)
		{
			List<int> list = new List<int>();
			foreach (TextureReplace value in dictReplace.Values)
			{
				if (!File.Exists(value.file.FullName))
				{
					forceRefresh = true;
					list.Add(value.index);
				}
			}
			foreach (int item in list)
			{
				RemoveDeletedReplace(item);
			}
		}
		if (forceRefresh)
		{
			forceRefresh = false;
			flag = false;
		}
		if (!flag || !IsValid())
		{
			Load(flag);
		}
		foreach (TextureReplace value2 in dictReplace.Values)
		{
			value2.TryRefresh(!flag);
		}
		date.DateTime = lastWriteTime;
	}

	public void ForceRefresh()
	{
		forceRefresh = true;
		TryRefresh();
	}

	public static void RefreshAll()
	{
		if (!EClass.core.IsGameStarted)
		{
			return;
		}
		foreach (TextureData value in EClass.core.textures.texMap.Values)
		{
			value.ForceRefresh();
		}
	}

	public virtual bool IsValid()
	{
		switch (type)
		{
		case Type.Pass:
			foreach (MeshPass item in listPass)
			{
				if (!item.mat.GetTexture(texName))
				{
					return false;
				}
			}
			return true;
		case Type.World:
			return EClass.scene.tileset.AtlasTexture;
		default:
			return true;
		}
	}

	public void Load(bool dateMatched)
	{
		if (!dateMatched)
		{
			Texture2D texture2D = IO.LoadPNG(path);
			if (!texture2D)
			{
				Debug.Log(path);
			}
			int num = extraRows * tileH;
			if (tex.width != texture2D.width || tex.height != texture2D.height + num)
			{
				Debug.Log(id + "/" + texture2D.width + "/" + texture2D.height + "/" + path);
				tex.Reinitialize(texture2D.width, texture2D.height + num, tex.format, tex.mipmapCount > 1);
			}
			if (num > 0)
			{
				tex.SetPixels32(0, 0, tex.width, num, new Color32[tex.width * num]);
			}
			tex.SetPixels32(0, num, texture2D.width, texture2D.height, texture2D.GetPixels32());
			tex.Apply();
			UnityEngine.Object.Destroy(texture2D);
			Debug.Log("Reloaded Texture:" + path);
		}
		switch (type)
		{
		case Type.Pass:
		{
			foreach (MeshPass item in listPass)
			{
				item.mat.SetTexture(texName, tex);
				if (item.haveShadowPass)
				{
					item.shadowPass.mat.SetTexture(texName, tex);
				}
			}
			break;
		}
		case Type.World:
			EClass.scene.tileset.AtlasTexture = tex;
			break;
		}
	}

	public void CreateReplace(int index, string path, TextureReplace.Source source, int sizeX, int sizeY)
	{
		string text = id + "_" + index + ".png";
		Texture2D texture2D = new Texture2D(tileW * sizeX, tileH * sizeY);
		int srcX = index % 100 * tileW;
		int srcY = tex.height - index / 100 * tileH - texture2D.height;
		Graphics.CopyTexture(tex, 0, 0, srcX, srcY, tileW * sizeX, tileH * sizeY, texture2D, 0, 0, 0, 0);
		byte[] bytes = texture2D.EncodeToPNG();
		try
		{
			File.WriteAllBytes(path + text, bytes);
			EClass.core.textures.TryAddReplace(new FileInfo(path + text), source, add: true, refresh: true);
		}
		catch
		{
		}
		UnityEngine.Object.Destroy(texture2D);
	}

	public void AddReplace(TextureReplace r)
	{
		TextureReplace textureReplace = dictReplace.TryGetValue(r.index);
		if (textureReplace != null)
		{
			if (r.source == TextureReplace.Source.Local && textureReplace.source != TextureReplace.Source.Local)
			{
				r.original = textureReplace;
			}
			else
			{
				textureReplace.DestoryTex();
			}
		}
		if (r.source == TextureReplace.Source.Local)
		{
			listReplaceLocal.Add(r);
		}
		dictReplace[r.index] = r;
	}

	public void DeleteReplace(TextureReplace r)
	{
		r.file.Delete();
		RemoveDeletedReplace(r.index);
	}

	public void RemoveDeletedReplace(int index)
	{
		TextureReplace textureReplace = dictReplace.TryGetValue(index);
		if (textureReplace != null)
		{
			textureReplace.DestoryTex();
			if (textureReplace.original != null && textureReplace.original.file.Exists)
			{
				dictReplace[index] = textureReplace.original;
			}
			else
			{
				dictReplace.Remove(index);
			}
		}
	}
}
