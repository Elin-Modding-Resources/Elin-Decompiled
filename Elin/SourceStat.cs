using System;
using System.Collections.Generic;

public class SourceStat : SourceDataInt<SourceStat.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public int id;

		public string alias;

		public string name_JP;

		public string name;

		public string type;

		public string group;

		public string curse;

		public string duration;

		public int durationMax;

		public int hexPower;

		public string[] negate;

		public string[] defenseAttb;

		public string[] resistance;

		public int gainRes;

		public string[] elements;

		public string[] nullify;

		public string[] tag;

		public int[] phase;

		public string colors;

		public string element;

		public string[] effect;

		public string[] strPhase_JP;

		public string[] strPhase;

		public string textPhase_JP;

		public string textPhase;

		public string textEnd_JP;

		public string textEnd;

		public string textPhase2_JP;

		public string textPhase2;

		public string gradient;

		public bool invert;

		public string detail_JP;

		public string detail;

		[NonSerialized]
		public string name_L;

		[NonSerialized]
		public string detail_L;

		[NonSerialized]
		public string textPhase_L;

		[NonSerialized]
		public string textPhase2_L;

		[NonSerialized]
		public string textEnd_L;

		[NonSerialized]
		public string[] strPhase_L;

		public override bool UseAlias => true;

		public override string GetAlias => alias;
	}

	public static readonly IReadOnlyDictionary<string, int> RowMapping = new Dictionary<string, int>
	{
		["id"] = 0,
		["alias"] = 1,
		["name_JP"] = 2,
		["name"] = 3,
		["type"] = 4,
		["group"] = 5,
		["curse"] = 6,
		["duration"] = 7,
		["durationMax"] = 8,
		["hexPower"] = 9,
		["negate"] = 10,
		["defenseAttb"] = 11,
		["resistance"] = 12,
		["gainRes"] = 13,
		["elements"] = 14,
		["nullify"] = 15,
		["tag"] = 16,
		["phase"] = 17,
		["colors"] = 18,
		["element"] = 19,
		["effect"] = 20,
		["strPhase_JP"] = 21,
		["strPhase"] = 22,
		["textPhase_JP"] = 23,
		["textPhase"] = 24,
		["textEnd_JP"] = 25,
		["textEnd"] = 26,
		["textPhase2_JP"] = 27,
		["textPhase2"] = 28,
		["gradient"] = 29,
		["invert"] = 30,
		["detail_JP"] = 31,
		["detail"] = 32
	};

	public static readonly IReadOnlyDictionary<string, string> TypeMapping = new Dictionary<string, string>
	{
		["id"] = "int",
		["alias"] = "string",
		["name_JP"] = "string",
		["name"] = "string",
		["type"] = "string",
		["group"] = "string",
		["curse"] = "string",
		["duration"] = "string",
		["durationMax"] = "int",
		["hexPower"] = "int",
		["negate"] = "string[]",
		["defenseAttb"] = "string[]",
		["resistance"] = "string[]",
		["gainRes"] = "int",
		["elements"] = "string[]",
		["nullify"] = "string[]",
		["tag"] = "string[]",
		["phase"] = "int[]",
		["colors"] = "string",
		["element"] = "string",
		["effect"] = "string[]",
		["strPhase_JP"] = "string[]",
		["strPhase"] = "string[]",
		["textPhase_JP"] = "string",
		["textPhase"] = "string",
		["textEnd_JP"] = "string",
		["textEnd"] = "string",
		["textPhase2_JP"] = "string",
		["textPhase2"] = "string",
		["gradient"] = "string",
		["invert"] = "bool",
		["detail_JP"] = "string",
		["detail"] = "string"
	};

	[NonSerialized]
	public Dictionary<string, List<Row>> groups = new Dictionary<string, List<Row>>();

	public override string[] ImportFields => new string[4] { "strPhase", "textPhase", "textPhase2", "textEnd" };

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetInt(0),
			alias = SourceData.GetString(1),
			name_JP = SourceData.GetString(2),
			name = SourceData.GetString(3),
			type = SourceData.GetString(4),
			group = SourceData.GetString(5),
			curse = SourceData.GetString(6),
			duration = SourceData.GetString(7),
			durationMax = SourceData.GetInt(8),
			hexPower = SourceData.GetInt(9),
			negate = SourceData.GetStringArray(10),
			defenseAttb = SourceData.GetStringArray(11),
			resistance = SourceData.GetStringArray(12),
			gainRes = SourceData.GetInt(13),
			elements = SourceData.GetStringArray(14),
			nullify = SourceData.GetStringArray(15),
			tag = SourceData.GetStringArray(16),
			phase = SourceData.GetIntArray(17),
			colors = SourceData.GetString(18),
			element = SourceData.GetString(19),
			effect = SourceData.GetStringArray(20),
			strPhase_JP = SourceData.GetStringArray(21),
			strPhase = SourceData.GetStringArray(22),
			textPhase_JP = SourceData.GetString(23),
			textPhase = SourceData.GetString(24),
			textEnd_JP = SourceData.GetString(25),
			textEnd = SourceData.GetString(26),
			textPhase2_JP = SourceData.GetString(27),
			textPhase2 = SourceData.GetString(28),
			gradient = SourceData.GetString(29),
			invert = SourceData.GetBool(30),
			detail_JP = SourceData.GetString(31),
			detail = SourceData.GetString(32)
		};
	}

	public override Row CreateRowByMapping(IReadOnlyDictionary<string, int> mapping)
	{
		return new Row
		{
			id = SourceData.GetInt(mapping["id"]),
			alias = SourceData.GetString(mapping["alias"]),
			name_JP = SourceData.GetString(mapping["name_JP"]),
			name = SourceData.GetString(mapping["name"]),
			type = SourceData.GetString(mapping["type"]),
			group = SourceData.GetString(mapping["group"]),
			curse = SourceData.GetString(mapping["curse"]),
			duration = SourceData.GetString(mapping["duration"]),
			durationMax = SourceData.GetInt(mapping["durationMax"]),
			hexPower = SourceData.GetInt(mapping["hexPower"]),
			negate = SourceData.GetStringArray(mapping["negate"]),
			defenseAttb = SourceData.GetStringArray(mapping["defenseAttb"]),
			resistance = SourceData.GetStringArray(mapping["resistance"]),
			gainRes = SourceData.GetInt(mapping["gainRes"]),
			elements = SourceData.GetStringArray(mapping["elements"]),
			nullify = SourceData.GetStringArray(mapping["nullify"]),
			tag = SourceData.GetStringArray(mapping["tag"]),
			phase = SourceData.GetIntArray(mapping["phase"]),
			colors = SourceData.GetString(mapping["colors"]),
			element = SourceData.GetString(mapping["element"]),
			effect = SourceData.GetStringArray(mapping["effect"]),
			strPhase_JP = SourceData.GetStringArray(mapping["strPhase_JP"]),
			strPhase = SourceData.GetStringArray(mapping["strPhase"]),
			textPhase_JP = SourceData.GetString(mapping["textPhase_JP"]),
			textPhase = SourceData.GetString(mapping["textPhase"]),
			textEnd_JP = SourceData.GetString(mapping["textEnd_JP"]),
			textEnd = SourceData.GetString(mapping["textEnd"]),
			textPhase2_JP = SourceData.GetString(mapping["textPhase2_JP"]),
			textPhase2 = SourceData.GetString(mapping["textPhase2"]),
			gradient = SourceData.GetString(mapping["gradient"]),
			invert = SourceData.GetBool(mapping["invert"]),
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
			if (!row.group.IsEmpty())
			{
				groups.GetOrCreate(row.group).Add(row);
			}
		}
	}
}
