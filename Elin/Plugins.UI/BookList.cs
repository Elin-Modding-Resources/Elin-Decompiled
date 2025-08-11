using System.Collections.Generic;
using System.IO;
using System.Linq;

public class BookList
{
	public class Item
	{
		public string title;

		public string author;

		public string id;

		public int chance = 100;

		public int skin;
	}

	public static Dictionary<string, Dictionary<string, Item>> dict;

	public static void Init()
	{
		dict = new Dictionary<string, Dictionary<string, Item>>();
		AddDir("Book", CorePath.CorePackage.Book);
		AddDir("Scroll", CorePath.CorePackage.Scroll);
		static void AddDir(string id, string path)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(path);
			Dictionary<string, Item> dictionary = new Dictionary<string, Item>();
			dict.Add(id, dictionary);
			FileInfo[] files = directoryInfo.GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				if (!(fileInfo.Extension != ".txt"))
				{
					StreamReader streamReader = new StreamReader(fileInfo.FullName);
					string[] array = streamReader.ReadLine().Split(',');
					Item item = new Item
					{
						title = array[0],
						author = ((array.Length >= 2 && !array[1].IsEmpty()) ? "nameAuthor".lang(array[1]) : "unknownAuthor".lang()),
						chance = ((array.Length >= 3) ? array[2].ToInt() : 100),
						skin = ((array.Length >= 4) ? array[3].ToInt() : 0),
						id = fileInfo.Name.Replace(fileInfo.Extension, "")
					};
					dictionary.Add(item.id, item);
					streamReader.Close();
				}
			}
		}
	}

	public static Item GetRandomItem(string idCat = "Book")
	{
		return dict[idCat].Where((KeyValuePair<string, Item> p) => p.Value.chance != 0).ToList().RandomItem()
			.Value;
	}

	public static Item GetItem(string id, string idCat = "Book")
	{
		return dict[idCat].TryGetValue(id) ?? dict["Book"]["_default"];
	}
}
