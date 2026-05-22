using System;
using System.Collections.Generic;

public class SourceJob : SourceDataString<SourceJob.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public string id;

		public string name_JP;

		public string name;

		public int playable;

		public int STR;

		public int END;

		public int DEX;

		public int PER;

		public int LER;

		public int WIL;

		public int MAG;

		public int CHA;

		public int SPD;

		public int[] elements;

		public string[] weapon;

		public string equip;

		public int[] domain;

		public string detail_JP;

		public string detail;

		public Dictionary<int, int> elementMap;

		[NonSerialized]
		public string name_L;

		[NonSerialized]
		public string detail_L;

		public override bool UseAlias => false;

		public override string GetAlias => "n";

		public void WriteNote(UINote n)
		{
			n.Clear();
			n.AddHeader(GetName().ToTitleCase());
			n.AddText(GetDetail()).SetWidth(400);
			n.Build();
		}
	}

	public static readonly IReadOnlyDictionary<string, int> RowMapping = new Dictionary<string, int>
	{
		["id"] = 0,
		["name_JP"] = 1,
		["name"] = 2,
		["playable"] = 3,
		["STR"] = 4,
		["END"] = 5,
		["DEX"] = 6,
		["PER"] = 7,
		["LER"] = 8,
		["WIL"] = 9,
		["MAG"] = 10,
		["CHA"] = 11,
		["SPD"] = 12,
		["elements"] = 14,
		["weapon"] = 15,
		["equip"] = 16,
		["domain"] = 17,
		["detail_JP"] = 18,
		["detail"] = 19
	};

	public static readonly IReadOnlyDictionary<string, string> TypeMapping = new Dictionary<string, string>
	{
		["id"] = "string",
		["name_JP"] = "string",
		["name"] = "string",
		["playable"] = "int",
		["STR"] = "int",
		["END"] = "int",
		["DEX"] = "int",
		["PER"] = "int",
		["LER"] = "int",
		["WIL"] = "int",
		["MAG"] = "int",
		["CHA"] = "int",
		["SPD"] = "int",
		["elements"] = "elements",
		["weapon"] = "string[]",
		["equip"] = "string",
		["domain"] = "elements",
		["detail_JP"] = "string",
		["detail"] = "string"
	};

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetString(0),
			name_JP = SourceData.GetString(1),
			name = SourceData.GetString(2),
			playable = SourceData.GetInt(3),
			STR = SourceData.GetInt(4),
			END = SourceData.GetInt(5),
			DEX = SourceData.GetInt(6),
			PER = SourceData.GetInt(7),
			LER = SourceData.GetInt(8),
			WIL = SourceData.GetInt(9),
			MAG = SourceData.GetInt(10),
			CHA = SourceData.GetInt(11),
			SPD = SourceData.GetInt(12),
			elements = Core.ParseElements(SourceData.GetStr(14)),
			weapon = SourceData.GetStringArray(15),
			equip = SourceData.GetString(16),
			domain = Core.ParseElements(SourceData.GetStr(17)),
			detail_JP = SourceData.GetString(18),
			detail = SourceData.GetString(19)
		};
	}

	public override Row CreateRowByMapping(IReadOnlyDictionary<string, int> mapping)
	{
		return new Row
		{
			id = SourceData.GetString(mapping["id"]),
			name_JP = SourceData.GetString(mapping["name_JP"]),
			name = SourceData.GetString(mapping["name"]),
			playable = SourceData.GetInt(mapping["playable"]),
			STR = SourceData.GetInt(mapping["STR"]),
			END = SourceData.GetInt(mapping["END"]),
			DEX = SourceData.GetInt(mapping["DEX"]),
			PER = SourceData.GetInt(mapping["PER"]),
			LER = SourceData.GetInt(mapping["LER"]),
			WIL = SourceData.GetInt(mapping["WIL"]),
			MAG = SourceData.GetInt(mapping["MAG"]),
			CHA = SourceData.GetInt(mapping["CHA"]),
			SPD = SourceData.GetInt(mapping["SPD"]),
			elements = Core.ParseElements(SourceData.GetStr(mapping["elements"])),
			weapon = SourceData.GetStringArray(mapping["weapon"]),
			equip = SourceData.GetString(mapping["equip"]),
			domain = Core.ParseElements(SourceData.GetStr(mapping["domain"])),
			detail_JP = SourceData.GetString(mapping["detail_JP"]),
			detail = SourceData.GetString(mapping["detail"])
		};
	}

	public override void SetRow(Row r)
	{
		map[r.id] = r;
	}

	public override IReadOnlyDictionary<string, int> GetRowMapping()
	{
		return RowMapping;
	}

	public override IReadOnlyDictionary<string, string> GetTypeMapping()
	{
		return TypeMapping;
	}

	public override void OnInit()
	{
		foreach (Row row in rows)
		{
			Dictionary<int, int> dictionary = (row.elementMap = Element.GetElementMap(row.elements));
			dictionary[70] = row.STR;
			dictionary[71] = row.END;
			dictionary[72] = row.DEX;
			dictionary[73] = row.PER;
			dictionary[74] = row.LER;
			dictionary[75] = row.WIL;
			dictionary[76] = row.MAG;
			dictionary[77] = row.CHA;
			dictionary[79] = row.SPD;
		}
	}
}
