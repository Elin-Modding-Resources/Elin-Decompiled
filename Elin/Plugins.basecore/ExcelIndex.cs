using System.Collections.Generic;
using NPOI.SS.UserModel;

public class ExcelIndex
{
	public string type;

	public string name;

	public string enumName = "";

	public bool serializable = true;

	public static List<ExcelIndex> GetIndex(ISheet sheet)
	{
		List<ExcelIndex> list = new List<ExcelIndex>();
		IRow row = sheet.GetRow(0);
		IRow row2 = sheet.GetRow(1);
		for (int i = 0; i < row.LastCellNum; i++)
		{
			ICell cell = row2.GetCell(i);
			ExcelIndex item = new ExcelIndex(row.GetCell(i), cell);
			list.Add(item);
		}
		return list;
	}

	public ExcelIndex(ICell titleCell, ICell dataCell)
	{
		name = titleCell.StringCellValue;
		if (dataCell != null)
		{
			type = dataCell.StringCellValue;
		}
		if (type == "Sprite[]")
		{
			serializable = false;
		}
	}

	public string GetTypeName()
	{
		if (type == null)
		{
			return null;
		}
		if (type.StartsWith("#"))
		{
			enumName = type.Replace("#", "");
			return enumName;
		}
		if (type == "str")
		{
			return "string";
		}
		if (type == "element_id")
		{
			return "int";
		}
		if (type == "elements")
		{
			return "int[]";
		}
		return type;
	}
}
