using System;
using System.Collections.Generic;

public class SourceCollectible : SourceDataString<SourceCollectible.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public string id;

		public string name_JP;

		public string name;

		public int rarity;

		public string prefab;

		public int num;

		public string filter;

		public string[] tag;

		public string sound;

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
		["rarity"] = 3,
		["prefab"] = 4,
		["num"] = 5,
		["filter"] = 6,
		["tag"] = 7,
		["sound"] = 8,
		["detail_JP"] = 9,
		["detail"] = 10
	};

	public static readonly IReadOnlyDictionary<string, string> TypeMapping = new Dictionary<string, string>
	{
		["id"] = "string",
		["name_JP"] = "string",
		["name"] = "string",
		["rarity"] = "int",
		["prefab"] = "string",
		["num"] = "int",
		["filter"] = "string",
		["tag"] = "string[]",
		["sound"] = "string",
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
			rarity = SourceData.GetInt(3),
			prefab = SourceData.GetString(4),
			num = SourceData.GetInt(5),
			filter = SourceData.GetString(6),
			tag = SourceData.GetStringArray(7),
			sound = SourceData.GetString(8),
			detail_JP = SourceData.GetString(9),
			detail = SourceData.GetString(10)
		};
	}

	public override Row CreateRowByMapping(IReadOnlyDictionary<string, int> mapping)
	{
		return new Row
		{
			id = SourceData.GetString(mapping["id"]),
			name_JP = SourceData.GetString(mapping["name_JP"]),
			name = SourceData.GetString(mapping["name"]),
			rarity = SourceData.GetInt(mapping["rarity"]),
			prefab = SourceData.GetString(mapping["prefab"]),
			num = SourceData.GetInt(mapping["num"]),
			filter = SourceData.GetString(mapping["filter"]),
			tag = SourceData.GetStringArray(mapping["tag"]),
			sound = SourceData.GetString(mapping["sound"]),
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
