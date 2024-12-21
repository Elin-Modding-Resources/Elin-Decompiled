using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BaseModManager
{
	[Serializable]
	public class BaseResource
	{
		public string resourcePath;

		public List<string> files;

		public void Parse<T>(ModItemList<T> list) where T : UnityEngine.Object
		{
			foreach (string file in files)
			{
				list.Add(null, resourcePath + "/" + file);
			}
		}
	}

	public static BaseModManager Instance;

	public static string rootMod;

	public static string rootDefaultPacakge;

	public static bool isInitialized;

	public static List<string> listChainLoad = new List<string>();

	public DirectoryInfo dirWorkshop;

	public int priorityIndex;

	[NonSerialized]
	public List<BaseModPackage> packages = new List<BaseModPackage>();

	public virtual void Init(string path, string defaultPackage = "_Elona")
	{
		Debug.Log("Initializing ModManager:" + defaultPackage + "/" + path);
		Instance = this;
		rootMod = (rootDefaultPacakge = path);
		if (!defaultPackage.IsEmpty())
		{
			rootDefaultPacakge = rootMod + defaultPackage + "/";
		}
	}

	public void InitLang()
	{
		Debug.Log("Initializing Langs");
		foreach (LangSetting value in MOD.langs.Values)
		{
			if (value.id != Lang.langCode)
			{
				continue;
			}
			new DirectoryInfo(value.dir);
			FileInfo[] files = new DirectoryInfo(value.dir + "Data").GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				if (fileInfo.Name == "Alias.xlsx")
				{
					Lang.alias = new ExcelData(fileInfo.FullName);
				}
				if (fileInfo.Name == "Name.xlsx")
				{
					Lang.names = new ExcelData(fileInfo.FullName);
				}
				if (fileInfo.Name == "chara_talk.xlsx")
				{
					MOD.listTalk.items.Add(new ExcelData(fileInfo.FullName));
				}
				if (fileInfo.Name == "chara_tone.xlsx")
				{
					MOD.tones.items.Add(new ExcelData(fileInfo.FullName));
				}
			}
		}
	}

	public virtual void ParseExtra(DirectoryInfo dir, BaseModPackage package)
	{
	}
}
