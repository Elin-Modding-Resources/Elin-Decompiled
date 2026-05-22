using System;
using System.Collections.Generic;

public class SourceHomeResource : SourceDataString<SourceHomeResource.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public string id;

		public string name_JP;

		public string name;

		public int expMod;

		public int maxLv;

		public string[] reward;

		public string detail_JP;

		public string detail;

		[NonSerialized]
		public string name_L;

		[NonSerialized]
		public string detail_L;

		public override bool UseAlias => false;

		public override string GetAlias => "n";
	}

	public static readonly IReadOnlyDictionary<string, int> RowMapping = new Dictionary<string, int>
	{
		["id"] = 0,
		["name_JP"] = 1,
		["name"] = 2,
		["expMod"] = 3,
		["maxLv"] = 4,
		["reward"] = 5,
		["detail_JP"] = 6,
		["detail"] = 7
	};

	public static readonly IReadOnlyDictionary<string, string> TypeMapping = new Dictionary<string, string>
	{
		["id"] = "string",
		["name_JP"] = "string",
		["name"] = "string",
		["expMod"] = "int",
		["maxLv"] = "int",
		["reward"] = "string[]",
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
			expMod = SourceData.GetInt(3),
			maxLv = SourceData.GetInt(4),
			reward = SourceData.GetStringArray(5),
			detail_JP = SourceData.GetString(6),
			detail = SourceData.GetString(7)
		};
	}

	public override Row CreateRowByMapping(IReadOnlyDictionary<string, int> mapping)
	{
		return new Row
		{
			id = SourceData.GetString(mapping["id"]),
			name_JP = SourceData.GetString(mapping["name_JP"]),
			name = SourceData.GetString(mapping["name"]),
			expMod = SourceData.GetInt(mapping["expMod"]),
			maxLv = SourceData.GetInt(mapping["maxLv"]),
			reward = SourceData.GetStringArray(mapping["reward"]),
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
}
