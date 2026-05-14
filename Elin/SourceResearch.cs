using System;
using System.Collections.Generic;

public class SourceResearch : SourceDataString<SourceResearch.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public string id;

		public string name_JP;

		public string name;

		public string[] resource;

		public int money;

		public int tech;

		public string req;

		public string type;

		public int expMod;

		public int maxLv;

		public string reward;

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
		["resource"] = 3,
		["money"] = 4,
		["tech"] = 5,
		["req"] = 6,
		["type"] = 7,
		["expMod"] = 8,
		["maxLv"] = 9,
		["reward"] = 10,
		["detail_JP"] = 11,
		["detail"] = 12
	};

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetString(0),
			name_JP = SourceData.GetString(1),
			name = SourceData.GetString(2),
			resource = SourceData.GetStringArray(3),
			money = SourceData.GetInt(4),
			tech = SourceData.GetInt(5),
			req = SourceData.GetString(6),
			type = SourceData.GetString(7),
			expMod = SourceData.GetInt(8),
			maxLv = SourceData.GetInt(9),
			reward = SourceData.GetString(10),
			detail_JP = SourceData.GetString(11),
			detail = SourceData.GetString(12)
		};
	}

	public override Row CreateRowByMapping(IReadOnlyDictionary<string, int> mapping)
	{
		return new Row
		{
			id = SourceData.GetString(mapping["id"]),
			name_JP = SourceData.GetString(mapping["name_JP"]),
			name = SourceData.GetString(mapping["name"]),
			resource = SourceData.GetStringArray(mapping["resource"]),
			money = SourceData.GetInt(mapping["money"]),
			tech = SourceData.GetInt(mapping["tech"]),
			req = SourceData.GetString(mapping["req"]),
			type = SourceData.GetString(mapping["type"]),
			expMod = SourceData.GetInt(mapping["expMod"]),
			maxLv = SourceData.GetInt(mapping["maxLv"]),
			reward = SourceData.GetString(mapping["reward"]),
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
}
