using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class BookList
{
	public class Item
	{
		public string[] lines;

		public string title;

		public string author;

		public string id;

		public string cat;

		public string path;

		public int chance = 100;

		public int skin;
	}

	public static List<Func<string[]>> booklistLoaders = new List<Func<string[]>>();

	public static Dictionary<string, Dictionary<string, Item>> dict;

	public static void Init()
	{
		if (dict != null)
		{
			return;
		}
		dict = new Dictionary<string, Dictionary<string, Item>>();
		List<string> list = new List<string>();
		list.AddRange(LoadDefaultBookList());
		foreach (Func<string[]> booklistLoader in booklistLoaders)
		{
			try
			{
				list.AddRange(booklistLoader());
			}
			catch (Exception arg)
			{
				Debug.LogWarning($"#book external booklist loader failed\n{arg}");
			}
		}
		foreach (string item in list)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(item);
			if (!directoryInfo.Exists)
			{
				Debug.LogWarning("#book invalid booklist path/" + item);
				continue;
			}
			string name = directoryInfo.Name;
			dict.TryAdd(name, new Dictionary<string, Item>());
			FileInfo[] files = directoryInfo.GetFiles("*.txt", SearchOption.TopDirectoryOnly);
			foreach (FileInfo fileInfo in files)
			{
				using StreamReader streamReader = new StreamReader(fileInfo.FullName);
				string text = Path.ChangeExtension(fileInfo.Name, null);
				string text2 = streamReader.ReadLine();
				int? num = text2?.Length;
				if (!num.HasValue || num.GetValueOrDefault() <= 0)
				{
					Debug.LogWarning("#book invalid first line/" + item);
					continue;
				}
				string[] array = text2.Split(',');
				dict[name][text] = new Item
				{
					cat = name,
					title = array[0],
					author = ((array.Length > 1) ? "nameAuthor".lang(array[1]) : "unknownAuthor".lang()),
					chance = ((array.Length > 2) ? array[2].ToInt() : 100),
					skin = ((array.Length > 3) ? array[3].ToInt() : 0),
					id = text,
					path = fileInfo.FullName
				};
			}
		}
	}

	public static Item GetRandomItem(string idCat = "Book")
	{
		Init();
		return dict[idCat].Where((KeyValuePair<string, Item> p) => p.Value.chance != 0).ToList().RandomItemWeighted((KeyValuePair<string, Item> p) => p.Value.chance)
			.Value;
	}

	public static Item GetItem(string id, string idCat = "Book")
	{
		Init();
		return dict[idCat].TryGetValue(id) ?? dict["Book"]["_default"];
	}

	public static string[] LoadDefaultBookList()
	{
		return new string[2]
		{
			CorePath.CorePackage.Book,
			CorePath.CorePackage.Scroll
		};
	}
}
