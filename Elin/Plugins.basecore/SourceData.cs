using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using UnityEngine;

public class SourceData<T, T2> : SourceData where T : SourceData.BaseRow
{
	public class FieldInfo
	{
		public string id;

		public string name;

		public bool isStatic;

		public int width;

		public int column;

		public bool IsIndex => id == "id";

		public string GetValue(T r)
		{
			if (r.HasField<string>(id))
			{
				return r.GetField<string>(id);
			}
			if (r.HasField<int>(id))
			{
				return r.GetField<int>(id).ToString();
			}
			if (r.HasField<string[]>(id))
			{
				string text = "";
				string[] field = r.GetField<string[]>(id);
				if (field != null)
				{
					string[] array = field;
					foreach (string text2 in array)
					{
						text = text + text2 + ",";
					}
				}
				return text.TrimEnd(',');
			}
			return "";
		}
	}

	public class FieldMap
	{
		public FieldInfo field;

		public int column;
	}

	public List<T> rows = new List<T>();

	public Dictionary<T2, T> map = new Dictionary<T2, T>();

	public Dictionary<string, T> alias = new Dictionary<string, T>();

	[NonSerialized]
	public bool initialized;

	[NonSerialized]
	private List<T> _backupRows = new List<T>();

	[NonSerialized]
	public List<string> editorListString = new List<string>();

	public static ISheet currentSheet;

	public virtual bool AllowHotInitialization => false;

	public virtual string[] ImportFields => new string[1] { "" };

	public override void Init()
	{
		if (initialized)
		{
			Debug.Log("#init Skipping sourceData.Init:" + base.name);
			return;
		}
		initialized = true;
		editorListString.Clear();
		int num = 0;
		foreach (T row in rows)
		{
			SetRow(row);
			if (row.UseAlias)
			{
				alias[row.GetAlias] = row;
			}
			row._index = num;
			num++;
		}
		OnInit();
	}

	public virtual void OnInit()
	{
	}

	public virtual void SetRow(T row)
	{
	}

	public override void Reset()
	{
		initialized = false;
		if (!Application.isPlaying)
		{
			BaseCore.resetRuntime = true;
		}
		if (map != null)
		{
			map.Clear();
		}
		if (alias != null)
		{
			alias.Clear();
		}
		if (Application.isPlaying && AllowHotInitialization)
		{
			Init();
		}
	}

	public override bool ImportData(ISheet sheet, string bookname, bool overwrite = false)
	{
		if (!overwrite)
		{
			rows = new List<T>();
		}
		isNew = true;
		nameSheet = sheet.SheetName;
		nameBook = bookname;
		SourceData.rowDefault = sheet.GetRow(2);
		int num = 0;
		for (int i = 3; i <= sheet.LastRowNum; i++)
		{
			SourceData.row = sheet.GetRow(i);
			if (SourceData.row == null || SourceData.row.GetCell(0) == null || SourceData.row.GetCell(0).ToString().IsEmpty())
			{
				break;
			}
			T val = CreateRow();
			val.OnImportData(this);
			rows.Add(val);
			num++;
		}
		string text = sheet.SheetName + "/" + sheet.LastRowNum + "/" + num;
		Debug.Log(text);
		ERROR.msg = text;
		OnAfterImportData();
		initialized = false;
		return true;
	}

	public virtual void OnAfterImportData()
	{
	}

	public virtual T CreateRow()
	{
		return null;
	}

	public override void BackupSource()
	{
		_backupRows = rows;
	}

	public override void RollbackSource()
	{
		rows = _backupRows;
	}

	public List<string> GetListString()
	{
		return BuildEditorList().editorListString;
	}

	public SourceData<T, T2> BuildEditorList()
	{
		if (editorListString.Count == 0)
		{
			foreach (T row in rows)
			{
				editorListString.Add(row.GetEditorListName());
			}
		}
		return this;
	}

	public List<FieldInfo> GetFields()
	{
		List<FieldInfo> list = new List<FieldInfo>();
		int x = 0;
		AddField("id", 4096);
		AddField("version", 4096);
		AddField("filter", 4096);
		AddField("name", 4096);
		AddField("text", 4096);
		AddField("detail", 4096);
		AddField("description", 4096);
		string[] importFields = ImportFields;
		foreach (string text in importFields)
		{
			if (!text.IsEmpty())
			{
				AddField(text, 4096);
			}
		}
		return list;
		void AddField(string id, int width)
		{
			bool flag = id == "id" || id == "filter";
			bool flag2 = id == "version";
			if (!(typeof(T).GetField(id) == null) || flag2)
			{
				list.Add(new FieldInfo
				{
					id = id,
					name = id,
					isStatic = flag,
					width = width,
					column = x
				});
				x++;
				if (!(flag || flag2))
				{
					list.Add(new FieldInfo
					{
						id = id,
						name = id + "_EN",
						isStatic = true,
						width = width,
						column = x
					});
					x++;
					list.Add(new FieldInfo
					{
						id = id + "_JP",
						name = id + "_JP",
						isStatic = true,
						width = width,
						column = x
					});
					x++;
				}
			}
		}
	}

	public virtual T GetRow(string id)
	{
		return null;
	}

	public override void ExportTexts(string path, bool update = false)
	{
		Debug.Log("Exporting:" + nameSheet + " path:" + path);
		string text = nameSheet + ".xlsx";
		XSSFWorkbook xSSFWorkbook = new XSSFWorkbook();
		List<FieldInfo> fields = GetFields();
		currentSheet = xSSFWorkbook.CreateSheet(nameSheet);
		int num = 0;
		int y = 0;
		foreach (FieldInfo item in fields)
		{
			GetCell(num, y).SetCellValue(item.name);
			currentSheet.SetColumnWidth(num, item.width);
			ICellStyle cellStyle = xSSFWorkbook.CreateCellStyle();
			cellStyle.FillForegroundColor = 44;
			cellStyle.FillPattern = FillPattern.SolidForeground;
			if (!item.isStatic && item.id != "version")
			{
				GetCell(num, y).CellStyle = cellStyle;
			}
			num++;
		}
		string cellValue = BaseCore.Instance.version.GetText() ?? "";
		y = 2;
		foreach (T row4 in rows)
		{
			foreach (FieldInfo item2 in fields)
			{
				if (item2.isStatic)
				{
					GetCell(item2.column, y).SetCellValue(item2.GetValue(row4));
				}
				else if (item2.id == "version")
				{
					GetCell(item2.column, y).SetCellValue(cellValue);
				}
			}
			y++;
		}
		currentSheet.CreateFreezePane(0, 2);
		currentSheet.SetAutoFilter(new CellRangeAddress(1, 1, 0, fields.Count - 1));
		if (update)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			if (!File.Exists(path + "_temp/" + text))
			{
				return;
			}
			XSSFWorkbook xSSFWorkbook2;
			using (FileStream @is = File.Open(path + "_temp/" + text, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				xSSFWorkbook2 = new XSSFWorkbook((Stream)@is);
			}
			ISheet sheetAt = xSSFWorkbook2.GetSheetAt(0);
			IRow row = sheetAt.GetRow(0);
			int num2 = 0;
			foreach (FieldInfo item3 in fields)
			{
				if (item3.isStatic)
				{
					continue;
				}
				int num3 = -1;
				for (int i = 0; i < row.LastCellNum; i++)
				{
					string stringCellValue = row.GetCell(i).StringCellValue;
					if (stringCellValue.IsEmpty())
					{
						break;
					}
					if (stringCellValue == item3.id)
					{
						num3 = i;
						break;
					}
				}
				if (num3 == -1)
				{
					continue;
				}
				dictionary.Add(item3.id, 0);
				for (y = 2; y <= sheetAt.LastRowNum; y++)
				{
					IRow row2 = sheetAt.GetRow(y);
					if (row2 == null)
					{
						if (y > 5)
						{
							break;
						}
						continue;
					}
					ICell cell = row2.GetCell(0);
					if (cell == null)
					{
						continue;
					}
					string text2 = ((cell.CellType == CellType.Numeric) ? cell.NumericCellValue.ToString() : cell.StringCellValue);
					if (text2.IsEmpty())
					{
						continue;
					}
					for (int j = 0; j <= currentSheet.LastRowNum; j++)
					{
						IRow row3 = currentSheet.GetRow(j);
						if (row3 == null)
						{
							if (j > 5)
							{
								break;
							}
							continue;
						}
						ICell cell2 = row3.GetCell(0);
						if (cell2 == null || cell2.StringCellValue != text2)
						{
							continue;
						}
						string text3 = row2.GetCell(num3)?.StringCellValue ?? "";
						if (text3.IsEmpty())
						{
							continue;
						}
						(row3.GetCell(item3.column) ?? row3.CreateCell(item3.column)).SetCellValue(text3);
						if (item3.id != "version")
						{
							ICell cell3 = row3.GetCell(item3.column + 2);
							ICell cell4 = row2.GetCell(num3 + 2);
							if (cell3 == null)
							{
								continue;
							}
							if (cell4 == null || cell3.StringCellValue != cell4.StringCellValue)
							{
								row3.GetCell(1).SetCellValue(cellValue);
							}
							num2++;
						}
						dictionary[item3.id]++;
					}
				}
			}
			Log.system = Log.system + ((num2 == 0) ? "(No Changes) " : "(Updated) ") + path + "/" + text + Environment.NewLine;
			if (num2 != 0)
			{
				foreach (KeyValuePair<string, int> item4 in dictionary)
				{
					Log.system = Log.system + item4.Key + ":" + item4.Value + "  ";
				}
				Log.system += Environment.NewLine;
			}
			Log.system += Environment.NewLine;
		}
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		using FileStream stream = new FileStream(path + "/" + text, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
		xSSFWorkbook.Write(stream);
	}

	public override void ValidateLang()
	{
		string langImportMod = CorePath.CorePackage.LangImportMod;
		string text = nameSheet + ".xlsx";
		Log.system = Log.system + langImportMod + text + Environment.NewLine;
		Log.system += Environment.NewLine;
	}

	public override void ImportTexts(string _nameSheet = null)
	{
		string langImportMod = CorePath.CorePackage.LangImportMod;
		string text = _nameSheet.IsEmpty(nameSheet) + ".xlsx";
		if (!File.Exists(langImportMod + text))
		{
			return;
		}
		XSSFWorkbook xSSFWorkbook;
		using (FileStream @is = File.Open(langImportMod + text, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		{
			xSSFWorkbook = new XSSFWorkbook((Stream)@is);
		}
		ISheet sheetAt = xSSFWorkbook.GetSheetAt(0);
		List<FieldInfo> fields = GetFields();
		IRow row = sheetAt.GetRow(0);
		List<FieldMap> list = new List<FieldMap>();
		foreach (FieldInfo item in fields)
		{
			if (item.isStatic)
			{
				continue;
			}
			for (int i = 0; i < row.LastCellNum; i++)
			{
				string stringCellValue = row.GetCell(i).StringCellValue;
				if (stringCellValue.IsEmpty())
				{
					break;
				}
				if (stringCellValue == item.id)
				{
					list.Add(new FieldMap
					{
						field = item,
						column = i
					});
					break;
				}
			}
		}
		for (int j = 2; j <= sheetAt.LastRowNum; j++)
		{
			IRow row2 = sheetAt.GetRow(j);
			T val = GetRow(row2.GetCell(0).StringCellValue);
			if (val == null)
			{
				Debug.Log(sheetAt.SheetName + ": id to import no longer exist: " + row2.GetCell(0).StringCellValue);
				break;
			}
			foreach (FieldMap item2 in list)
			{
				FieldInfo field = item2.field;
				System.Reflection.FieldInfo field2 = val.GetType().GetField(field.id + "_L");
				if (field2 == null)
				{
					continue;
				}
				if (typeof(string[]).IsAssignableFrom(field2.FieldType))
				{
					ICell cell = row2.GetCell(item2.column);
					string[] array = ((cell == null) ? new string[0] : cell.StringCellValue.Split(','));
					string[] field3 = val.GetField<string[]>(field.id);
					if (field3 != null)
					{
						field2.SetValue(val, (array.Length >= field3.Length) ? array : field3);
					}
				}
				else
				{
					ICell cell2 = row2.GetCell(item2.column);
					if (cell2 != null)
					{
						field2.SetValue(val, cell2.StringCellValue.IsEmpty(val.GetField<string>(field.id)));
					}
				}
			}
		}
	}

	public static ICell GetCell(int x, int y)
	{
		IRow row = currentSheet.GetRow(y) ?? currentSheet.CreateRow(y);
		return row.GetCell(x) ?? row.CreateCell(x);
	}
}
public class SourceData : ScriptableObject
{
	public enum AutoID
	{
		None,
		Auto
	}

	[Serializable]
	public class BaseRow
	{
		public int _index;

		public virtual bool UseAlias => false;

		public virtual string GetAlias => "";

		public virtual string GetName()
		{
			return GetText();
		}

		public string GetDetail()
		{
			return GetText("detail");
		}

		public virtual string GetEditorListName()
		{
			return this.GetField<int>("id") + "-" + this.GetField<string>("alias") + "(" + this.GetField<string>("name_JP") + ")";
		}

		public string GetText(string id = "name", bool returnNull = false)
		{
			FieldInfo field = GetType().GetField(id + LangSuffix);
			if (!Lang.isBuiltin && (field == null || (field.GetValue(this) as string).IsEmpty()))
			{
				FieldInfo field2 = GetType().GetField(id);
				if (field2 != null && !(field2.GetValue(this) as string).IsEmpty())
				{
					return field2.GetValue(this) as string;
				}
			}
			if (field != null)
			{
				return (field.GetValue(this) as string).IsEmpty(returnNull ? null : "");
			}
			return "";
		}

		public string[] GetTextArray(string id)
		{
			if (!Lang.isBuiltin)
			{
				FieldInfo field = GetType().GetField(id + Lang.suffix);
				if (field != null && field.GetValue(this) is string[] array && array.Length != 0)
				{
					return array;
				}
				return GetType().GetField(id).GetValue(this) as string[];
			}
			return GetType().GetField(id + Lang.suffix).GetValue(this) as string[];
		}

		public virtual void SetID(ref int count)
		{
			this.SetField("id", count);
			count++;
		}

		public virtual void OnImportData(SourceData data)
		{
		}
	}

	public static string LangSuffix;

	public static string dataPath;

	public AutoID autoID;

	public bool isNew = true;

	public string nameSheet;

	public string nameBook;

	public virtual string sheetName => "";

	public virtual string sourcePath => "";

	public static IRow row
	{
		get
		{
			return ExcelParser.row;
		}
		set
		{
			ExcelParser.row = value;
		}
	}

	public static IRow rowDefault
	{
		get
		{
			return ExcelParser.rowDefault;
		}
		set
		{
			ExcelParser.rowDefault = value;
		}
	}

	public void BuildFlags(string rawText, Dictionary<string, bool> map)
	{
		if (!string.IsNullOrEmpty(rawText))
		{
			string[] array = rawText.Split(',');
			foreach (string key in array)
			{
				map.Add(key, value: true);
			}
		}
	}

	public virtual void Reset()
	{
	}

	public virtual void InsertData(IRow row)
	{
	}

	public virtual void Init()
	{
	}

	public virtual string[] GetList(string id)
	{
		return null;
	}

	public virtual bool ImportData(ISheet sheet, string bookname, bool overwrite = false)
	{
		return false;
	}

	public virtual void BackupSource()
	{
	}

	public virtual void RollbackSource()
	{
	}

	public virtual void BackupPref()
	{
	}

	public virtual void RestorePref()
	{
	}

	public virtual void ValidatePref()
	{
	}

	public virtual void ExportTexts(string path, bool update = false)
	{
	}

	public virtual void ImportTexts(string _nameSheet = null)
	{
	}

	public virtual void ValidateLang()
	{
	}

	public static bool IsNull(ICell cell)
	{
		if (cell != null && cell.CellType != CellType.Blank)
		{
			return cell.CellType == CellType.Unknown;
		}
		return true;
	}

	public static int GetInt(int id)
	{
		return ExcelParser.GetInt(id);
	}

	public static bool GetBool(int id)
	{
		return ExcelParser.GetBool(id);
	}

	public static double GetDouble(int id)
	{
		return ExcelParser.GetDouble(id);
	}

	public static float GetFloat(int id)
	{
		return ExcelParser.GetFloat(id);
	}

	public static float[] GetFloatArray(int id)
	{
		return ExcelParser.GetFloatArray(id);
	}

	public static int[] GetIntArray(int id)
	{
		return ExcelParser.GetIntArray(id);
	}

	public static string[] GetStringArray(int id)
	{
		return ExcelParser.GetStringArray(id);
	}

	public static string GetString(int id)
	{
		return ExcelParser.GetString(id);
	}

	public static string GetStr(int id, bool useDefault = false)
	{
		return ExcelParser.GetStr(id, useDefault);
	}
}
