using System;
using System.Collections.Generic;

public class SourceFaction : SourceDataString<SourceFaction.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public string id;

		public string name_JP;

		public string name;

		public string type;

		public string faith;

		public string domain;

		public int relation;

		public string textType_JP;

		public string textType;

		public string textAvatar;

		public string detail_JP;

		public string detail;

		[NonSerialized]
		public string name_L;

		[NonSerialized]
		public string detail_L;

		[NonSerialized]
		public string textType_L;

		[NonSerialized]
		public string textBenefit_L;

		[NonSerialized]
		public string textPet_L;

		public override bool UseAlias => false;

		public override string GetAlias => "n";
	}

	public static readonly IReadOnlyDictionary<string, int> RowMapping = new Dictionary<string, int>
	{
		["id"] = 0,
		["name_JP"] = 1,
		["name"] = 2,
		["type"] = 3,
		["faith"] = 4,
		["domain"] = 5,
		["relation"] = 6,
		["textType_JP"] = 7,
		["textType"] = 8,
		["textAvatar"] = 9,
		["detail_JP"] = 10,
		["detail"] = 11
	};

	public static readonly IReadOnlyDictionary<string, string> TypeMapping = new Dictionary<string, string>
	{
		["id"] = "string",
		["name_JP"] = "string",
		["name"] = "string",
		["type"] = "string",
		["faith"] = "string",
		["domain"] = "string",
		["relation"] = "int",
		["textType_JP"] = "string",
		["textType"] = "string",
		["textAvatar"] = "string",
		["detail_JP"] = "string",
		["detail"] = "string"
	};

	public override string[] ImportFields => new string[3] { "textType", "textBenefit", "textPet" };

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetString(0),
			name_JP = SourceData.GetString(1),
			name = SourceData.GetString(2),
			type = SourceData.GetString(3),
			faith = SourceData.GetString(4),
			domain = SourceData.GetString(5),
			relation = SourceData.GetInt(6),
			textType_JP = SourceData.GetString(7),
			textType = SourceData.GetString(8),
			textAvatar = SourceData.GetString(9),
			detail_JP = SourceData.GetString(10),
			detail = SourceData.GetString(11)
		};
	}

	public override Row CreateRowByMapping(IReadOnlyDictionary<string, int> mapping)
	{
		return new Row
		{
			id = SourceData.GetString(mapping["id"]),
			name_JP = SourceData.GetString(mapping["name_JP"]),
			name = SourceData.GetString(mapping["name"]),
			type = SourceData.GetString(mapping["type"]),
			faith = SourceData.GetString(mapping["faith"]),
			domain = SourceData.GetString(mapping["domain"]),
			relation = SourceData.GetInt(mapping["relation"]),
			textType_JP = SourceData.GetString(mapping["textType_JP"]),
			textType = SourceData.GetString(mapping["textType"]),
			textAvatar = SourceData.GetString(mapping["textAvatar"]),
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
