using System.Collections.Generic;
using System.IO;

public class ExcelDataList
{
	public List<ExcelData> items = new List<ExcelData>(0);

	public Dictionary<string, Dictionary<string, string>> all = new Dictionary<string, Dictionary<string, string>>();

	public List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();

	protected bool initialized;

	public virtual int StartIndex => 5;

	public void Reload()
	{
		all.Clear();
		list.Clear();
		initialized = false;
	}

	public virtual void Initialize()
	{
		if (initialized)
		{
			return;
		}
		foreach (ExcelData item in items)
		{
			item.startIndex = StartIndex;
			List<Dictionary<string, string>> obj = item.BuildList();
			string directoryName = new FileInfo(item.path).DirectoryName;
			foreach (Dictionary<string, string> item2 in obj)
			{
				item2["path"] = directoryName;
				all[item2["id"]] = item2;
				list.Add(item2);
			}
		}
		OnInitialize();
		initialized = true;
	}

	public virtual void OnInitialize()
	{
	}

	public Dictionary<string, string> GetRow(string id)
	{
		if (!initialized)
		{
			Initialize();
		}
		return all.TryGetValue(id) ?? list[0];
	}
}
