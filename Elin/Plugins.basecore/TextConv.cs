using System.Collections.Generic;
using System.Text;
using NPOI.SS.UserModel;
using UnityEngine;

public class TextConv : ExcelData
{
	public class Item
	{
		public static string[] DefaultSuffix = "、,。,！,？,…,ー".Split(',');

		public string[] strs;

		public string[] suffix;

		public string[] keys;

		public int probab;

		public int gender;

		public bool additive;

		public Item(IRow row, string c0, string c1, string c2, int c4)
		{
			ICell cell = row.GetCell(3);
			keys = c0.Split(',');
			strs = c1.Split(',');
			if (cell == null)
			{
				suffix = DefaultSuffix;
			}
			else
			{
				string text = cell.ToString();
				if (text == "*")
				{
					suffix = null;
				}
				else
				{
					suffix = text.Split(',');
				}
			}
			probab = c4;
			additive = ExcelParser.GetBool(5, row);
			gender = ((c2 == "F") ? 1 : ((c2 == "M") ? 2 : 0));
		}
	}

	public string[] I_M;

	public string[] I_F;

	public string[] you_M;

	public string[] you_F;

	public string[] tags;

	public int p_I_M;

	public int p_I_F;

	public int p_you_M;

	public int p_you_F;

	public List<Item> items = new List<Item>();

	public StringBuilder Apply(ref string text, int gender)
	{
		BuildMap(null);
		StringBuilder stringBuilder = new StringBuilder();
		string text2 = null;
		Item item = null;
		int num = 0;
		if (gender == 0)
		{
			gender = ((Rand.rnd(3) != 0) ? 1 : 2);
		}
		while (num < text.Length)
		{
			bool flag = false;
			foreach (Item item2 in items)
			{
				if ((item2.probab != 0 && Rand.Range(0, 100) >= item2.probab) || (item2.gender != 0 && gender != item2.gender))
				{
					continue;
				}
				string[] keys = item2.keys;
				foreach (string text3 in keys)
				{
					flag = true;
					for (int j = 0; j < text3.Length; j++)
					{
						if (num + j >= text.Length || text[num + j] != text3[j])
						{
							flag = false;
							break;
						}
					}
					if (flag && item2.suffix != null)
					{
						flag = false;
						if (num + text3.Length >= text.Length)
						{
							continue;
						}
						string[] suffix = item2.suffix;
						foreach (string text4 in suffix)
						{
							if (text[num + text3.Length] == text4[0])
							{
								flag = true;
								break;
							}
						}
					}
					if (flag)
					{
						item = item2;
						text2 = text3;
						break;
					}
				}
				if (flag)
				{
					break;
				}
			}
			if (flag && text2.Length > 0)
			{
				stringBuilder.Append((item.additive ? text2 : "") + item.strs[Rand.Range(0, item.strs.Length)]);
				num += text2.Length;
			}
			else
			{
				stringBuilder.Append(text[num]);
				num++;
			}
		}
		stringBuilder.Replace("…ー", "ー…");
		return stringBuilder;
	}

	public override void BuildMap(string sheetName = null)
	{
		LoadBook();
		ISheet sheetAt = book.GetSheetAt(0);
		if (sheetAt.LastRowNum <= 3)
		{
			return;
		}
		for (int i = 3; i <= sheetAt.LastRowNum; i++)
		{
			IRow row = sheetAt.GetRow(i);
			if (row.Cells.Count == 0)
			{
				Debug.LogWarning(path + "/" + book?.ToString() + "/" + sheetName + "/" + sheetAt.LastRowNum + "/" + i);
				continue;
			}
			string text = row.GetCell(0).ToString();
			if (text.IsEmpty())
			{
				Debug.LogWarning(path + "/" + book?.ToString() + "/" + sheetName + "/" + sheetAt.LastRowNum + "/" + i);
				continue;
			}
			string text2 = row.GetCell(1).ToString();
			string text3 = row.GetCell(2)?.ToString() ?? "";
			int @int = ExcelParser.GetInt(4, row);
			if (text[0] == '$')
			{
				switch (text + "_" + text3)
				{
				case "$I_":
					I_M = (I_F = text2.Split(','));
					p_I_M = (p_I_F = @int);
					break;
				case "$you_":
					you_M = (you_F = text2.Split(','));
					p_you_M = (p_you_F = @int);
					break;
				case "$I_M":
					I_M = text2.Split(',');
					p_I_M = @int;
					break;
				case "$you_M":
					you_M = text2.Split(',');
					p_you_M = @int;
					break;
				case "$I_F":
					I_F = text2.Split(',');
					p_I_F = @int;
					break;
				case "$you_F":
					you_F = text2.Split(',');
					p_you_F = @int;
					break;
				case "$tag":
					tags = text2.Split(',');
					break;
				}
			}
			items.Add(new Item(row, text, text2, text3, @int));
		}
	}
}
