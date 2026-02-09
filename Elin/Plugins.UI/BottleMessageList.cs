using System.Collections.Generic;
using System.Linq;

public class BottleMessageList
{
	public static List<BookList.Item> list;

	public static void Init()
	{
		if (list == null)
		{
			list = new List<BookList.Item>();
			string[] dialog = Lang.GetDialog("rumor", "bottle");
			int num = 0;
			string[] array = dialog;
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split('|');
				list.Add(new BookList.Item
				{
					id = (num.ToString() ?? ""),
					author = array2[0],
					title = array2[1],
					lines = new string[4]
					{
						"",
						"",
						"{center}",
						array2[2]
					}
				});
				num++;
			}
		}
	}

	public static BookList.Item GetRandomItem()
	{
		Init();
		return list.RandomItemWeighted((BookList.Item p) => p.chance);
	}

	public static BookList.Item GetItem(string id)
	{
		Init();
		return list.First((BookList.Item p) => p.id == id) ?? list[0];
	}
}
