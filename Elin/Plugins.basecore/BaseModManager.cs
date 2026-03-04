using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

	private static readonly Dictionary<string, HashSet<Action<object>>> _eventHandlers = new Dictionary<string, HashSet<Action<object>>>();

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
		Debug.Log("Initializing Langs: " + Lang.langCode);
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

	public static void SubscribeEvent(string eventId, Action<object> handler)
	{
		if (!_eventHandlers.TryGetValue(eventId, out var value))
		{
			value = new HashSet<Action<object>>();
			_eventHandlers[eventId] = value;
		}
		value.Add(handler);
	}

	public static void UnsubscribeEvent(string eventId, Action<object> handler)
	{
		if (_eventHandlers.TryGetValue(eventId, out var value))
		{
			value.Remove(handler);
			if (value.Count == 0)
			{
				_eventHandlers.Remove(eventId);
			}
		}
	}

	public static void PublishEvent(string eventId, object data = null)
	{
		if (!_eventHandlers.TryGetValue(eventId, out var value))
		{
			return;
		}
		foreach (Action<object> item in value.ToList())
		{
			try
			{
				item(data);
			}
			catch (Exception arg)
			{
				Debug.LogError($"#event '{eventId}' throws error in one of the handlers\n{arg}");
			}
		}
	}

	public static bool HasEventSubscriber(string eventId)
	{
		int? num = _eventHandlers.GetValueOrDefault(eventId)?.Count;
		if (num.HasValue)
		{
			return num.GetValueOrDefault() > 0;
		}
		return false;
	}
}
