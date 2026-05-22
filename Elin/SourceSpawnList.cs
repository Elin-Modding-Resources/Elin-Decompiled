using System;
using System.Collections.Generic;

public class SourceSpawnList : SourceDataString<SourceSpawnList.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public string id;

		public string parent;

		public string type;

		public string[] category;

		public string[] idCard;

		public string[] tag;

		public string[] filter;

		public override bool UseAlias => false;

		public override string GetAlias => "n";

		public override string GetEditorListName()
		{
			return this.GetField<string>("id") ?? "";
		}
	}

	public static readonly IReadOnlyDictionary<string, int> RowMapping = new Dictionary<string, int>
	{
		["id"] = 0,
		["parent"] = 2,
		["type"] = 3,
		["category"] = 4,
		["idCard"] = 5,
		["tag"] = 6,
		["filter"] = 7
	};

	public static readonly IReadOnlyDictionary<string, string> TypeMapping = new Dictionary<string, string>
	{
		["id"] = "string",
		["parent"] = "string",
		["type"] = "string",
		["category"] = "string[]",
		["idCard"] = "string[]",
		["tag"] = "string[]",
		["filter"] = "string[]"
	};

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetString(0),
			parent = SourceData.GetString(2),
			type = SourceData.GetString(3),
			category = SourceData.GetStringArray(4),
			idCard = SourceData.GetStringArray(5),
			tag = SourceData.GetStringArray(6),
			filter = SourceData.GetStringArray(7)
		};
	}

	public override Row CreateRowByMapping(IReadOnlyDictionary<string, int> mapping)
	{
		return new Row
		{
			id = SourceData.GetString(mapping["id"]),
			parent = SourceData.GetString(mapping["parent"]),
			type = SourceData.GetString(mapping["type"]),
			category = SourceData.GetStringArray(mapping["category"]),
			idCard = SourceData.GetStringArray(mapping["idCard"]),
			tag = SourceData.GetStringArray(mapping["tag"]),
			filter = SourceData.GetStringArray(mapping["filter"])
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
