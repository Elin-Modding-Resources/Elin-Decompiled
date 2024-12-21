using System.Collections.Generic;

public static class ExcelDataListExtension
{
	public static int IndexOf(this List<Dictionary<string, string>> list, string id)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i]["id"] == id)
			{
				return i;
			}
		}
		return -1;
	}

	public static string GetNextID(this List<Dictionary<string, string>> list, string currentId, int a)
	{
		int num = list.IndexOf(currentId) + a;
		if (num >= list.Count)
		{
			num = 0;
		}
		if (num < 0)
		{
			num = list.Count - 1;
		}
		return list[num]["id"];
	}
}
