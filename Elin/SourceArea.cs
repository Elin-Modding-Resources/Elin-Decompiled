using System;
using System.Collections.Generic;

public class SourceArea : SourceDataString<SourceArea.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public string id;

		public string name_JP;

		public string name;

		public string textAssign_JP;

		public string textAssign;

		public string detail_JP;

		public string tag;

		public string detail;

		[NonSerialized]
		public string name_L;

		[NonSerialized]
		public string detail_L;

		[NonSerialized]
		public string textAssign_L;

		public override bool UseAlias => false;

		public override string GetAlias => "n";
	}

	public static readonly IReadOnlyDictionary<string, int> RowMapping = new Dictionary<string, int>
	{
		["id"] = 0,
		["name_JP"] = 1,
		["name"] = 2,
		["textAssign_JP"] = 3,
		["textAssign"] = 4,
		["detail_JP"] = 5,
		["tag"] = 6,
		["detail"] = 7
	};

	public static readonly IReadOnlyDictionary<string, string> TypeMapping = new Dictionary<string, string>
	{
		["id"] = "string",
		["name_JP"] = "string",
		["name"] = "string",
		["textAssign_JP"] = "string",
		["textAssign"] = "string",
		["detail_JP"] = "string",
		["tag"] = "string",
		["detail"] = "string"
	};

	public override string[] ImportFields => new string[1] { "textAssign" };

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetString(0),
			name_JP = SourceData.GetString(1),
			name = SourceData.GetString(2),
			textAssign_JP = SourceData.GetString(3),
			textAssign = SourceData.GetString(4),
			detail_JP = SourceData.GetString(5),
			tag = SourceData.GetString(6),
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
			textAssign_JP = SourceData.GetString(mapping["textAssign_JP"]),
			textAssign = SourceData.GetString(mapping["textAssign"]),
			detail_JP = SourceData.GetString(mapping["detail_JP"]),
			tag = SourceData.GetString(mapping["tag"]),
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
