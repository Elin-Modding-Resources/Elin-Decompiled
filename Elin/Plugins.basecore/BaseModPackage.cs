using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BaseModPackage
{
	public const int defaultLoadPriority = 100;

	public const int minLoadPriority = -999;

	public const int maxLoadPriority = 999;

	public static XmlReader xmlReader;

	public static XmlReaderSettings readerSetting;

	public string title;

	public string author;

	public string version;

	public string id;

	public string description;

	public string visibility;

	public string[] tags;

	public string[][] dependency;

	public string[][] incompatible;

	public string[][] loadAfter;

	public string[][] loadBefore;

	public bool builtin;

	public bool activated;

	public bool willActivate = true;

	public bool installed;

	public bool banned;

	public bool isInPackages;

	public bool hasPublishedPackage;

	public bool downloadStarted;

	public int loadPriority = 100;

	public int orderIndex = -1;

	public BaseModPackage blockedBy;

	public string langDepError;

	public BaseModPackage duplicateOf;

	public string parseError;

	public object item;

	public DirectoryInfo dirInfo;

	public Text progressText;

	private static readonly Dictionary<string, string> builtinPackages = new Dictionary<string, string>
	{
		{ "_Elona", "elin_core1" },
		{ "_Lang_Chinese", "elin_language_chinese" },
		{ "_ModdingKit", "elin.plugins.modding" },
		{ "Mod_Slot", "elin_minigame_slot" }
	};

	public bool CanActivate
	{
		get
		{
			if (!hasPublishedPackage && installed)
			{
				DirectoryInfo directoryInfo = dirInfo;
				if (directoryInfo != null && directoryInfo.Exists)
				{
					return willActivate;
				}
			}
			return false;
		}
	}

	public static string NormalizeId(string s)
	{
		if (s.IsEmpty())
		{
			return null;
		}
		string text = s.Trim().ToLowerInvariant();
		if (!text.IsEmpty())
		{
			return text;
		}
		return null;
	}

	public static bool IsBuiltinDir(string dirName)
	{
		if (!dirName.IsEmpty())
		{
			return builtinPackages.ContainsKey(dirName);
		}
		return false;
	}

	public static bool IsBuiltinPackage(DirectoryInfo dir, string packageId)
	{
		if (dir != null && builtinPackages.TryGetValue(dir.Name, out var value))
		{
			return NormalizeId(packageId) == value;
		}
		return false;
	}

	public bool IsValidVersion()
	{
		if (!builtin && !version.IsEmpty() && version.Split('.').Length >= 3)
		{
			return !Version.Get(version).IsBelow(BaseCore.Instance.versionMod);
		}
		return true;
	}

	public bool Init()
	{
		if (!File.Exists(dirInfo.FullName + "/package.xml"))
		{
			return false;
		}
		UpdateMeta();
		return true;
	}

	public void UpdateMeta(bool updateOnly = false)
	{
		string text = dirInfo.FullName + "/package.xml";
		if (!File.Exists(text))
		{
			return;
		}
		byte[] bytes;
		try
		{
			bytes = File.ReadAllBytes(text);
		}
		catch (Exception ex)
		{
			parseError = ex.Message;
			Debug.LogWarning($"#mod package.xml can't be read: {text}\n{ex}");
			return;
		}
		string error;
		XDocument xDocument = ModXml.Load(bytes, out error);
		if (xDocument?.Root == null)
		{
			parseError = error.IsEmpty("no root element");
			Debug.LogWarning("#mod package.xml failed to parse: " + text + "\n" + parseError);
			return;
		}
		string text2 = null;
		string text3 = null;
		string packageId = null;
		string text4 = null;
		string text5 = null;
		string text6 = null;
		string[] array = null;
		int num = 100;
		List<string[]> rows = null;
		List<string[]> rows2 = null;
		List<string[]> rows3 = null;
		List<string[]> rows4 = null;
		try
		{
			foreach (XElement item in xDocument.Root.Descendants())
			{
				string text7 = ModXml.ValidXml(item.Name.LocalName);
				if (text7 == null)
				{
					Debug.LogWarning("#mod package.xml unknown element <" + item.Name.LocalName + ">, ignored: " + text);
					continue;
				}
				if (text7 != item.Name.LocalName)
				{
					Debug.LogWarning("#mod package.xml <" + item.Name.LocalName + "> should be written as <" + text7 + ">, parsed: " + text);
				}
				switch (text7)
				{
				case "title":
					text2 = ModXml.Text(item);
					break;
				case "author":
					text3 = ModXml.Text(item);
					break;
				case "id":
					packageId = ModXml.Text(item);
					break;
				case "description":
					text4 = item.Value;
					break;
				case "version":
					text5 = ModXml.Text(item);
					break;
				case "tag":
				case "tags":
					array = ModXml.SplitTags(ModXml.Text(item));
					break;
				case "loadPriority":
				{
					if (int.TryParse(ModXml.Text(item), out var result))
					{
						num = Mathf.Clamp(result, -999, 999);
					}
					break;
				}
				case "visibility":
					text6 = ModXml.Text(item);
					break;
				case "dependency":
					ModXml.ReadIdRow(item, ref rows, text);
					break;
				case "incompatible":
					ModXml.ReadIdRow(item, ref rows2, text);
					break;
				case "loadAfter":
					ModXml.ReadIdRow(item, ref rows3, text);
					break;
				case "loadBefore":
					ModXml.ReadIdRow(item, ref rows4, text);
					break;
				}
			}
		}
		catch (Exception ex2)
		{
			parseError = ex2.Message;
			Debug.LogWarning($"#mod package.xml failed to parse: {text}\n{ex2}");
			return;
		}
		title = text2;
		author = text3;
		id = packageId;
		description = text4;
		version = text5;
		visibility = text6;
		tags = array;
		loadPriority = num;
		dependency = rows?.ToArray();
		incompatible = rows2?.ToArray();
		loadAfter = rows3?.ToArray();
		loadBefore = rows4?.ToArray();
		builtin = IsBuiltinPackage(dirInfo, packageId);
		parseError = null;
	}

	public void Activate()
	{
		if (CanActivate)
		{
			int num = ((BaseModManager.Instance == null) ? (-1) : BaseModManager.Instance.packages.IndexOf(this));
			Debug.Log("Activating(" + (num + 1) + ") : " + title + "/" + id);
			activated = true;
			Parse();
		}
	}

	public void Parse()
	{
		DirectoryInfo[] directories = dirInfo.GetDirectories();
		foreach (DirectoryInfo directoryInfo in directories)
		{
			if (directoryInfo.Name.ToLower() == "actor")
			{
				FileInfo[] files = directoryInfo.GetFiles("*.xlsx", SearchOption.TopDirectoryOnly);
				foreach (FileInfo fileInfo in files)
				{
					MOD.actorSources.Add(new ExcelData(fileInfo.FullName));
				}
				DirectoryInfo[] directories2 = directoryInfo.GetDirectories();
				foreach (DirectoryInfo directoryInfo2 in directories2)
				{
					Log.App(directoryInfo2.FullName);
					string text = directoryInfo2.Name.ToLower();
					if (!(text == "pcc"))
					{
						if (!(text == "sprite"))
						{
							continue;
						}
						files = directoryInfo2.GetFiles();
						foreach (FileInfo fileInfo2 in files)
						{
							if (fileInfo2.Name.EndsWith(".png"))
							{
								MOD.sprites.Add(fileInfo2);
							}
						}
					}
					else
					{
						DirectoryInfo[] directories3 = directoryInfo2.GetDirectories();
						foreach (DirectoryInfo obj in directories3)
						{
							MOD.OnAddPcc(obj);
						}
					}
				}
			}
			else
			{
				BaseModManager.Instance.ParseExtra(directoryInfo, this);
			}
		}
	}
}
