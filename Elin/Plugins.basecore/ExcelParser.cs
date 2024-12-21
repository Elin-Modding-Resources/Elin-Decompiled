using System;
using NPOI.SS.UserModel;

public class ExcelParser
{
	public static IRow row;

	public static IRow rowDefault;

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
		if (int.TryParse(GetStr(id), out var result))
		{
			return result;
		}
		return 0;
	}

	public static int GetInt(int col, IRow _row)
	{
		row = _row;
		return GetInt(col);
	}

	public static bool GetBool(int id)
	{
		string str = GetStr(id);
		return str switch
		{
			"0" => false, 
			"1" => true, 
			null => false, 
			_ => bool.Parse(str), 
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
		if (str != null)
		{
			return double.Parse(str);
		}
		return 0.0;
	}

	public static float GetFloat(int id)
	{
		string str = GetStr(id);
		if (str != null)
		{
			return float.Parse(str);
		}
		return 0f;
	}

	public static float[] GetFloatArray(int id)
	{
		string str = GetStr(id);
		if (str != null)
		{
			return Array.ConvertAll(str.Split(','), float.Parse);
		}
		return new float[0];
	}

	public static int[] GetIntArray(int id)
	{
		string str = GetStr(id);
		if (str != null)
		{
			return Array.ConvertAll(str.Split(','), int.Parse);
		}
		return new int[0];
	}

	public static string[] GetStringArray(int id)
	{
		string str = GetStr(id);
		if (str != null)
		{
			return str.Split(',');
		}
		return new string[0];
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
}
