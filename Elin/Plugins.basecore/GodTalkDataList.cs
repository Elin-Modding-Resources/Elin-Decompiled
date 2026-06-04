using System.Collections.Generic;

public class GodTalkDataList : ExcelDataList
{
	public override int StartIndex => 3;

	public override void Initialize()
	{
		if (initialized)
		{
			return;
		}
		foreach (ExcelData item in items)
		{
			item.startIndex = StartIndex;
			item.BuildMap();
			foreach (KeyValuePair<string, Dictionary<string, string>> item2 in item.sheets["_default"].map)
			{
				item2.Deconstruct(out var key, out var value);
				string key2 = key;
				Dictionary<string, string> dictionary = value;
				if (all.TryAdd(key2, dictionary))
				{
					list.Add(dictionary);
					continue;
				}
				foreach (KeyValuePair<string, string> item3 in dictionary)
				{
					item3.Deconstruct(out key, out var value2);
					string key3 = key;
					string value3 = value2;
					all[key2][key3] = value3;
				}
			}
		}
		initialized = true;
	}

	public string GetTalk(string id, string idTopic)
	{
		Dictionary<string, string> row = GetRow(idTopic);
		return ((row == null) ? null : row.TryGetValue(id)?.SplitByNewline().RandomItem()) ?? "";
	}
}
