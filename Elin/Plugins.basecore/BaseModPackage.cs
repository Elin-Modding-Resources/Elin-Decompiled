using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class BaseModPackage
{
	public static XmlReader xmlReader;

	public static XmlReaderSettings readerSetting;

	public string title;

	public string author;

	public string version;

	public string id;

	public string description;

	public string visibility;

	public string[] tags;

	public bool builtin;

	public bool activated;

	public bool willActivate = true;

	public bool installed;

	public bool banned;

	public bool isInPackages;

	public bool hasPublishedPackage;

	public bool downloadStarted;

	public int loadPriority;

	public object item;

	public DirectoryInfo dirInfo;

	public Text progressText;

	public bool IsValidVersion()
	{
		return !Version.Get(version).IsBelow(BaseCore.Instance.versionMod);
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
		XmlReader xmlReader = XmlReader.Create(text, new XmlReaderSettings
		{
			IgnoreComments = true,
			IgnoreWhitespace = true
		});
		while (xmlReader.Read())
		{
			if (xmlReader.NodeType != XmlNodeType.Element)
			{
				continue;
			}
			switch (xmlReader.Name)
			{
			case "title":
				if (xmlReader.Read())
				{
					title = xmlReader.Value;
				}
				break;
			case "author":
				if (xmlReader.Read())
				{
					author = xmlReader.Value;
				}
				break;
			case "id":
				if (xmlReader.Read())
				{
					id = xmlReader.Value;
				}
				break;
			case "description":
				if (xmlReader.Read())
				{
					description = xmlReader.Value;
				}
				break;
			case "version":
				if (xmlReader.Read())
				{
					version = xmlReader.Value;
				}
				break;
			case "builtin":
				if (xmlReader.Read())
				{
					bool.TryParse(xmlReader.Value, out builtin);
				}
				break;
			case "tag":
			case "tags":
				if (xmlReader.Read())
				{
					tags = xmlReader.Value.Split(',');
				}
				break;
			case "loadPriority":
				if (!updateOnly && xmlReader.Read())
				{
					int.TryParse(xmlReader.Value, out loadPriority);
				}
				break;
			case "visibility":
				if (xmlReader.Read())
				{
					visibility = xmlReader.Value;
				}
				break;
			}
		}
		xmlReader.Close();
	}

	public void Activate()
	{
		if (!hasPublishedPackage && installed && dirInfo.Exists && willActivate)
		{
			Debug.Log("Activating(" + loadPriority + ") :" + title + "/" + id);
			activated = true;
			Parse();
		}
	}

	public void Parse()
	{
		DirectoryInfo[] directories = dirInfo.GetDirectories();
		foreach (DirectoryInfo directoryInfo in directories)
		{
			if (directoryInfo.Name == "Actor")
			{
				FileInfo[] files = directoryInfo.GetFiles();
				foreach (FileInfo fileInfo in files)
				{
					if (fileInfo.Name.EndsWith(".xlsx"))
					{
						MOD.actorSources.items.Add(new ExcelData(fileInfo.FullName));
					}
				}
				DirectoryInfo[] directories2 = directoryInfo.GetDirectories();
				foreach (DirectoryInfo directoryInfo2 in directories2)
				{
					Log.App(directoryInfo2.FullName);
					string name = directoryInfo2.Name;
					if (!(name == "PCC"))
					{
						if (!(name == "Sprite"))
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
