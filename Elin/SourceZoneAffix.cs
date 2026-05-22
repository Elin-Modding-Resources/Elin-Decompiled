using System;
using System.Collections.Generic;

public class SourceZoneAffix : SourceDataInt<SourceZoneAffix.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public int id;

		public string zone;

		public string name_JP;

		public string name;

		public string textAssign_JP;

		public string textAssign;

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
		["zone"] = 1,
		["name_JP"] = 2,
		["name"] = 3,
		["textAssign_JP"] = 4,
		["textAssign"] = 5,
		["detail_JP"] = 6,
		["detail"] = 7
	};

	public static readonly IReadOnlyDictionary<string, string> TypeMapping = new Dictionary<string, string>
	{
		["id"] = "int",
		["zone"] = "string",
		["name_JP"] = "string",
		["name"] = "string",
		["textAssign_JP"] = "string",
		["textAssign"] = "string",
		["detail_JP"] = "string",
		["detail"] = "string"
	};

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetInt(0),
			zone = SourceData.GetString(1),
			name_JP = SourceData.GetString(2),
			name = SourceData.GetString(3),
			textAssign_JP = SourceData.GetString(4),
			textAssign = SourceData.GetString(5),
			detail_JP = SourceData.GetString(6),
			detail = SourceData.GetString(7)
		};
	}

	public override Row CreateRowByMapping(IReadOnlyDictionary<string, int> mapping)
	{
		return new Row
		{
			id = SourceData.GetInt(mapping["id"]),
			zone = SourceData.GetString(mapping["zone"]),
			name_JP = SourceData.GetString(mapping["name_JP"]),
			name = SourceData.GetString(mapping["name"]),
			textAssign_JP = SourceData.GetString(mapping["textAssign_JP"]),
			textAssign = SourceData.GetString(mapping["textAssign"]),
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
