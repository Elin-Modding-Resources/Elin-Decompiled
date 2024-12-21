using System;
using System.Collections.Generic;

[Serializable]
public class ExcelBookImportSetting
{
	public string name;

	public string exportPath;

	public string _prefix;

	public List<ExcelSheetImportSetting> sheets = new List<ExcelSheetImportSetting>();

	public string prefix
	{
		get
		{
			if (!string.IsNullOrEmpty(_prefix))
			{
				return _prefix;
			}
			return "Source";
		}
	}

	public string path => exportPath.IsEmpty(CustomAssetManager.Instance.exportPath) + "Export/";

	public ExcelSheetImportSetting GetSheet(string id)
	{
		foreach (ExcelSheetImportSetting sheet in sheets)
		{
			if (sheet.name == id)
			{
				return sheet;
			}
		}
		return null;
	}

	public ExcelSheetImportSetting GetOrCreateSheet(string id)
	{
		ExcelSheetImportSetting sheet = GetSheet(id);
		if (sheet != null)
		{
			return sheet;
		}
		sheet = new ExcelSheetImportSetting
		{
			name = id
		};
		sheets.Add(sheet);
		return sheet;
	}

	public override string ToString()
	{
		return name;
	}
}
