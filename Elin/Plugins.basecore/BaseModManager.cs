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

	private static readonly Dictionary<(string, Delegate), Action<object>> _typedEventHandlers = new Dictionary<(string, Delegate), Action<object>>();

	public DirectoryInfo dirWorkshop;

	public int priorityIndex;

	[NonSerialized]
	public List<BaseModPackage> packages = new List<BaseModPackage>();

	public virtual void Init(string path, string defaultPackage = "_Elona")
	{
		Debug.Log("Initializing ModManager:" + defaultPackage + "/" + path);
		Instance = this;
		rootMod = (rootDefaultPacakge = path);
		_eventHandlers.Clear();
		_typedEventHandlers.Clear();
		if (!defaultPackage.IsEmpty())
		{
			rootDefaultPacakge = rootMod + defaultPackage + "/";
		}
	}

	public void InitLang()
	{
		Debug.Log("Initializing Langs: " + Lang.langCode);
		MOD.listTalk.Clear();
		MOD.listGodTalk.Clear();
		MOD.tones.Clear();
		LangSetting langSetting = MOD.langs.TryGetValue(Lang.langCode);
		if (langSetting == null)
		{
			Debug.LogError("Lang: " + Lang.langCode + " is not set");
			langSetting = MOD.langs.FirstItem();
		}
		FileInfo[] files = new DirectoryInfo(langSetting.dir + "Data").GetFiles();
		foreach (FileInfo fileInfo in files)
		{
			switch (fileInfo.Name)
			{
			case "Alias.xlsx":
				Lang.alias = new ExcelData(fileInfo.FullName);
				break;
			case "Name.xlsx":
				Lang.names = new ExcelData(fileInfo.FullName);
				break;
			case "chara_talk.xlsx":
				MOD.listTalk.Add(new ExcelData(fileInfo.FullName));
				break;
			case "god_talk.xlsx":
				MOD.listGodTalk.Add(new ExcelData(fileInfo.FullName));
				break;
			case "chara_tone.xlsx":
				MOD.tones.Add(new ExcelData(fileInfo.FullName));
				break;
			}
		}
	}

	public virtual void ParseExtra(DirectoryInfo dir, BaseModPackage package)
	{
	}

	public static void SubscribeEvent(string eventId, Action<object> handler)
	{
		if (handler != null)
		{
			if (!_eventHandlers.TryGetValue(eventId, out var value))
			{
				value = new HashSet<Action<object>>();
				_eventHandlers[eventId] = value;
			}
			value.Add(handler);
		}
	}

	public static void SubscribeEvent(string eventId, Action handler)
	{
		if (handler != null)
		{
			Action<object> action = delegate
			{
				handler();
			};
			(string, Delegate) key = (eventId, handler);
			_typedEventHandlers[key] = action;
			SubscribeEvent(eventId, action);
		}
	}

	public static void SubscribeEvent<T>(string eventId, Action<T> handler)
	{
		if (handler == null)
		{
			return;
		}
		Action<object> action = delegate(object data)
		{
			if (!(data is T obj))
			{
				if (data != null)
				{
					throw new InvalidCastException($"{handler.Method.DeclaringType?.Assembly.GetName()} " + "expects " + typeof(T).Name + ", got " + data.GetType().Name);
				}
				handler(default(T));
			}
			else
			{
				handler(obj);
			}
		};
		(string, Delegate) key = (eventId, handler);
		_typedEventHandlers[key] = action;
		SubscribeEvent(eventId, action);
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

	public static void UnsubscribeEvent(string eventId, Action handler)
	{
		if (handler != null)
		{
			(string, Delegate) key = (eventId, handler);
			if (_typedEventHandlers.TryGetValue(key, out var value))
			{
				UnsubscribeEvent(eventId, value);
				_typedEventHandlers.Remove(key);
			}
		}
	}

	public static void UnsubscribeEvent<T>(string eventId, Action<T> handler)
	{
		if (handler != null)
		{
			(string, Delegate) key = (eventId, handler);
			if (_typedEventHandlers.TryGetValue(key, out var value))
			{
				UnsubscribeEvent(eventId, value);
				_typedEventHandlers.Remove(key);
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

	public static void PublishEvent<T>(string eventId, T data = default(T))
	{
		PublishEvent(eventId, (object)data);
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
