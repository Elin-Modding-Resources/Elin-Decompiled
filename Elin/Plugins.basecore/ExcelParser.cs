using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.SS.UserModel;
using UnityEngine;

public class ExcelParser
{
	public static string path;

	public static IRow row;

	public static IRow rowDefault;

	public static IRow rowHeader;

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
		string str = GetStr(id);
		if (str.IsEmpty())
		{
			return 0;
		}
		if (!int.TryParse(str, out var result))
		{
			if (float.TryParse(str, out var result2))
			{
				return (int)result2;
			}
			ReportIllFormat<int>(id);
		}
		return result;
	}

	public static int GetInt(int col, IRow _row)
	{
		row = _row;
		return GetInt(col);
	}

	public static bool GetBool(int id)
	{
		string str = GetStr(id);
		bool result;
		return str switch
		{
			null => false, 
			"1" => true, 
			"0" => false, 
			_ => bool.TryParse(str, out result) && result, 
		};
	}

	public static bool GetBool(int col, IRow _row)
	{
		row = _row;
		return GetBool(col);
	}

	public static double GetDouble(int id)
	{
		string str = GetStr(id);
		if (str.IsEmpty())
		{
			return 0.0;
		}
		if (!double.TryParse(str, out var result))
		{
			ReportIllFormat<double>(id);
		}
		return result;
	}

	public static float GetFloat(int id)
	{
		string str = GetStr(id);
		if (str.IsEmpty())
		{
			return 0f;
		}
		if (!float.TryParse(str, out var result))
		{
			ReportIllFormat<float>(id);
		}
		return result;
	}

	public static float[] GetFloatArray(int id)
	{
		string str = GetStr(id);
		if (str.IsEmpty())
		{
			return Array.Empty<float>();
		}
		string[] array = str.Split(',');
		float[] array2 = new float[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			if (!float.TryParse(array[i], out array2[i]))
			{
				ReportIllFormat<float>(id);
				array2[i] = 0f;
			}
		}
		return array2;
	}

	public static int[] GetIntArray(int id)
	{
		string str = GetStr(id);
		if (str.IsEmpty())
		{
			return Array.Empty<int>();
		}
		string[] array = str.Split(',');
		int[] array2 = new int[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			if (!int.TryParse(array[i], out array2[i]))
			{
				if (float.TryParse(array[i], out var result))
				{
					array2[i] = (int)result;
					continue;
				}
				ReportIllFormat<int>(id);
				array2[i] = 0;
			}
		}
		return array2;
	}

	public static string[] GetStringArray(int id)
	{
		string str = GetStr(id);
		if (str != null)
		{
			return str.Split(',').ToArray();
		}
		return Array.Empty<string>();
	}

	public static string GetString(int id)
	{
		return GetStr(id) ?? "";
	}

	public static string GetString(int col, IRow _row)
	{
		row = _row;
		return GetStr(col);
	}

	public static string GetStr(int id, bool useDefault = false)
	{
		IRow row = (useDefault ? rowDefault : ExcelParser.row);
		if (row == null)
		{
			if (!useDefault)
			{
				return GetStr(id, useDefault: true);
			}
			return null;
		}
		ICell cell = row.GetCell(id);
		if (IsNull(cell))
		{
			if (!useDefault)
			{
				return GetStr(id, useDefault: true);
			}
			return null;
		}
		cell.SetCellType(CellType.String);
		if (cell.StringCellValue == "")
		{
			if (!useDefault)
			{
				return GetStr(id, useDefault: true);
			}
			return null;
		}
		return cell.StringCellValue;
	}

	public static string ToLetterId(int id)
	{
		string text = "";
		while (id >= 0)
		{
			text = (char)(id % 26 + 65) + text;
			id /= 26;
			id--;
		}
		return text;
	}

	public static void ReportIllFormat<T>(int id)
	{
		StringBuilder stringBuilder = new StringBuilder();
		string name = typeof(T).Name;
		ICell cell = row?.Cells.TryGet(id, returnNull: true);
		IRow obj = row;
		string value = ((obj != null && obj.RowNum >= 3) ? $", default:'{rowDefault?.Cells.TryGet(id, returnNull: true)}'" : ", SourceData begins at the 4th row. 3rd row is the default value row.");
		stringBuilder.AppendLine("#source ill-format file: " + path);
		object[] array = new object[5];
		IRow obj2 = row;
		array[0] = ((obj2 != null) ? new int?(obj2.RowNum + 1) : null);
		array[1] = id + 1;
		array[2] = ToLetterId(id);
		array[3] = name;
		array[4] = cell;
		stringBuilder.Append(string.Format("row#{0}, cell#{1}/#{2}, expected:'{3}', read:'{4}'", array));
		stringBuilder.AppendLine(value);
		Debug.LogWarning(stringBuilder);
	}

	public static string GetRowHeaderDiff(IReadOnlyDictionary<string, int> mapping, IReadOnlyDictionary<string, int> header)
	{
		StringBuilder stringBuilder = new StringBuilder();
		List<KeyValuePair<string, int>> list = mapping.OrderBy((KeyValuePair<string, int> c) => c.Value).ToList();
		int num = list.Max((KeyValuePair<string, int> c) => c.Key.Length);
		foreach (KeyValuePair<string, int> item in list)
		{
			item.Deconstruct(out var key, out var value);
			string text = key;
			int index = value;
			int value2;
			bool num2 = header.TryGetValue(text, out value2);
			string text2 = ((num2 && value2 != index) ? $"maybe {value2 + 1,2}/{ToLetterId(value2)} {text}" : "");
			if (!num2)
			{
				text2 = "<missing>";
			}
			string text3 = header.FirstOrDefault((KeyValuePair<string, int> c) => c.Value == index).Key ?? "<missing>";
			text3 = text3.PadRight(num + 3);
			string text4 = text.PadRight(num);
			stringBuilder.AppendLine($"{index + 1,2}/{ToLetterId(index),2}: {text4} -> {text3} {text2}");
		}
		return stringBuilder.ToString();
	}

	public static void ClearStaticRows()
	{
		row = null;
		rowDefault = null;
		rowHeader = null;
	}
}
