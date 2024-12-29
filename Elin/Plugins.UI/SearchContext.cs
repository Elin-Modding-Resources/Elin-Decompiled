using System;
using System.Collections.Generic;
using System.IO;

public class SearchContext
{
	public class Item
	{
		public string idFile;

		public string idTopic;

		public string text;

		public string textSearch;

		public bool system;
	}

	public static int MaxItem = 50;

	public List<Item> items = new List<Item>();

	public List<Item> result = new List<Item>();

	public void Init()
	{
		FileInfo[] files = new DirectoryInfo(CorePath.CorePackage.Help).GetFiles();
		foreach (FileInfo fileInfo in files)
		{
			if (fileInfo.Extension != ".txt" || fileInfo.Name == "_topics.txt" || fileInfo.Name == "include.txt")
			{
				continue;
			}
			string[] array = IO.LoadTextArray(fileInfo.FullName);
			string idFile = fileInfo.Name.Replace(".txt", "");
			string idTopic = "";
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (text.Length == 0 || text.IsEmpty() || text == Environment.NewLine)
				{
					continue;
				}
				if (text[0] == '$')
				{
					idTopic = text.Replace("$", "");
					continue;
				}
				bool flag = false;
				if (text[0] == '{')
				{
					if (text.Length < 6)
					{
						continue;
					}
					switch (text.Substring(0, 3))
					{
					case "{A|":
					case "{Q|":
						flag = true;
						break;
					case "{pa":
						break;
					default:
						continue;
					}
				}
				Item item = new Item();
				if (flag)
				{
					item.textSearch = (item.text = text.Split('|')[1].Replace("}", "")).ToLower();
				}
				else
				{
					item.text = text;
					item.textSearch = text.ToLower();
				}
				item.idFile = idFile;
				item.idTopic = idTopic;
				items.Add(item);
			}
		}
	}

	public List<Item> Search(string s)
	{
		result.Clear();
		int num = 0;
		string value = s.ToLower();
		foreach (Item item in items)
		{
			if (item.textSearch.Contains(value))
			{
				result.Add(item);
				num++;
				if (num >= MaxItem)
				{
					result.Add(new Item
					{
						system = true,
						text = "maxResult".lang()
					});
					break;
				}
			}
		}
		if (result.Count == 0)
		{
			result.Add(new Item
			{
				system = true,
				text = "noResult".lang()
			});
		}
		return result;
	}
}
