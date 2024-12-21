using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using UnityEngine;

public class ExcelData
{
	public class Sheet
	{
		public Dictionary<string, Dictionary<string, string>> map;

		public List<Dictionary<string, string>> list;
	}

	public int startIndex = 5;

	public XSSFWorkbook book;

	public IRow rowIndex;

	public Dictionary<string, Sheet> sheets = new Dictionary<string, Sheet>();

	public string path;

	public int maxEmptyRows;

	public DateTime lastModified;

	public ExcelData()
	{
	}

	public ExcelData(string _path, int _index)
	{
		path = _path;
		startIndex = _index;
		BuildMap();
	}

	public ExcelData(string _path)
	{
		path = _path;
	}

	public void LoadBook()
	{
		if (book != null)
		{
			if (lastModified.Equals(File.GetLastWriteTime(path)))
			{
				return;
			}
			Debug.Log("Excel file modified:" + path);
			sheets.Clear();
		}
		lastModified = File.GetLastWriteTime(path);
		TextAsset textAsset = Resources.Load(path) as TextAsset;
		if ((bool)textAsset)
		{
			Stream @is = new MemoryStream(textAsset.bytes);
			book = new XSSFWorkbook(@is);
			return;
		}
		using FileStream is2 = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		book = new XSSFWorkbook((Stream)is2);
	}

	public bool IsModified()
	{
		return !lastModified.Equals(File.GetLastWriteTime(path));
	}

	public virtual void BuildMap(string sheetName = "_default")
	{
		BuildList(sheetName);
		Sheet sheet = sheets[sheetName];
		if (sheet.map != null)
		{
			return;
		}
		sheet.map = new Dictionary<string, Dictionary<string, string>>();
		foreach (Dictionary<string, string> item in sheet.list)
		{
			sheet.map[item["id"]] = item;
		}
	}

	public List<Dictionary<string, string>> BuildList(string sheetName = "_default")
	{
		LoadBook();
		if (sheetName.IsEmpty())
		{
			sheetName = "_default";
		}
		Sheet sheet = sheets.TryGetValue(sheetName);
		if (sheet == null)
		{
			sheet = new Sheet();
			sheets[sheetName] = sheet;
		}
		if (sheet.list != null)
		{
			return sheet.list;
		}
		sheet.list = new List<Dictionary<string, string>>();
		ISheet sheet2 = (string.IsNullOrEmpty(sheetName) ? book.GetSheetAt(0) : (book.GetSheet(sheetName) ?? book.GetSheetAt(0)));
		rowIndex = sheet2.GetRow(0);
		int num = 0;
		for (int i = startIndex - 1; i <= sheet2.LastRowNum; i++)
		{
			IRow row = sheet2.GetRow(i);
			if (row == null)
			{
				if (i >= 5)
				{
					num++;
					if (num > maxEmptyRows)
					{
						break;
					}
				}
				continue;
			}
			num = 0;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			for (int j = 0; j < rowIndex.LastCellNum; j++)
			{
				ICell cell = row.GetCell(j);
				string text = rowIndex.GetCell(j).ToString();
				if (text.IsEmpty())
				{
					break;
				}
				try
				{
					dictionary.Add(text, (cell == null) ? "" : cell.ToString());
				}
				catch
				{
				}
			}
			sheet.list.Add(dictionary);
		}
		return sheet.list;
	}

	public void Override(Dictionary<string, SourceData> sources, bool canAddData = true)
	{
		using FileStream @is = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		book = new XSSFWorkbook((Stream)@is);
		for (int i = 0; i < book.NumberOfSheets; i++)
		{
			ISheet sheetAt = book.GetSheetAt(i);
			ExcelParser.rowDefault = sheetAt.GetRow(2);
			SourceData value = null;
			if (!sources.TryGetValue(sheetAt.SheetName, out value))
			{
				Debug.Log(sheetAt.SheetName + " does not exist in sourceMap.");
				continue;
			}
			Type type = value.GetType();
			IList list = type.GetField("rows").GetValue(value) as IList;
			IDictionary dictionary = type.GetField("map").GetValue(value) as IDictionary;
			Type type2 = list.GetType().GetGenericArguments()[0];
			List<FieldInfo> list2 = new List<FieldInfo>();
			List<ExcelIndex> index = ExcelIndex.GetIndex(sheetAt);
			for (int j = 0; j < index.Count; j++)
			{
				list2.Add(type2.GetField(index[j].name));
			}
			for (int k = startIndex - 1; k <= sheetAt.LastRowNum; k++)
			{
				string stringCellValue = (ExcelParser.row = sheetAt.GetRow(k)).GetCell(0).StringCellValue;
				bool flag = true;
				object obj;
				if (dictionary.Contains(stringCellValue))
				{
					obj = dictionary[stringCellValue];
					flag = false;
				}
				else
				{
					if (!canAddData)
					{
						continue;
					}
					obj = Activator.CreateInstance(type2);
				}
				for (int l = 0; l < index.Count; l++)
				{
					string type3 = index[l].type;
					if (type3 == "str" || type3 == "string")
					{
						list2[l].SetValue(obj, ExcelParser.GetString(l));
					}
				}
				if (flag)
				{
					list.Add(obj);
				}
			}
		}
	}

	public string GetText(string id, string topic = "text")
	{
		BuildMap();
		return sheets["_default"].map.TryGetValue(topic)?[id];
	}
}
