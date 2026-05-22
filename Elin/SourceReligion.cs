using System;
using System.Collections.Generic;

public class SourceReligion : SourceDataString<SourceReligion.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public string id;

		public string name_JP;

		public string name;

		public string[] name2_JP;

		public string[] name2;

		public string type;

		public string idMaterial;

		public string faith;

		public string domain;

		public int tax;

		public int relation;

		public int[] elements;

		public string[] cat_offer;

		public string[] rewards;

		public string textType_JP;

		public string textType;

		public string textAvatar;

		public string detail_JP;

		public string detail;

		public string textBenefit_JP;

		public string textBenefit;

		public string textPet_JP;

		public string textPet;

		[NonSerialized]
		public string name_L;

		[NonSerialized]
		public string detail_L;

		[NonSerialized]
		public string textType_L;

		[NonSerialized]
		public string textBenefit_L;

		[NonSerialized]
		public string[] name2_L;

		public override bool UseAlias => false;

		public override string GetAlias => "n";
	}

	public static readonly IReadOnlyDictionary<string, int> RowMapping = new Dictionary<string, int>
	{
		["id"] = 0,
		["name_JP"] = 1,
		["name"] = 2,
		["name2_JP"] = 3,
		["name2"] = 4,
		["type"] = 5,
		["idMaterial"] = 6,
		["faith"] = 7,
		["domain"] = 8,
		["tax"] = 9,
		["relation"] = 10,
		["elements"] = 11,
		["cat_offer"] = 12,
		["rewards"] = 13,
		["textType_JP"] = 14,
		["textType"] = 15,
		["textAvatar"] = 16,
		["detail_JP"] = 17,
		["detail"] = 18,
		["textBenefit_JP"] = 19,
		["textBenefit"] = 20,
		["textPet_JP"] = 21,
		["textPet"] = 22
	};

	public static readonly IReadOnlyDictionary<string, string> TypeMapping = new Dictionary<string, string>
	{
		["id"] = "string",
		["name_JP"] = "string",
		["name"] = "string",
		["name2_JP"] = "string[]",
		["name2"] = "string[]",
		["type"] = "string",
		["idMaterial"] = "string",
		["faith"] = "string",
		["domain"] = "string",
		["tax"] = "int",
		["relation"] = "int",
		["elements"] = "elements",
		["cat_offer"] = "string[]",
		["rewards"] = "string[]",
		["textType_JP"] = "string",
		["textType"] = "string",
		["textAvatar"] = "string",
		["detail_JP"] = "string",
		["detail"] = "string",
		["textBenefit_JP"] = "string",
		["textBenefit"] = "string",
		["textPet_JP"] = "string",
		["textPet"] = "string"
	};

	public override string[] ImportFields => new string[3] { "textBenefit", "textType", "name2" };

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetString(0),
			name_JP = SourceData.GetString(1),
			name = SourceData.GetString(2),
			name2_JP = SourceData.GetStringArray(3),
			name2 = SourceData.GetStringArray(4),
			type = SourceData.GetString(5),
			idMaterial = SourceData.GetString(6),
			faith = SourceData.GetString(7),
			domain = SourceData.GetString(8),
			tax = SourceData.GetInt(9),
			relation = SourceData.GetInt(10),
			elements = Core.ParseElements(SourceData.GetStr(11)),
			cat_offer = SourceData.GetStringArray(12),
			rewards = SourceData.GetStringArray(13),
			textType_JP = SourceData.GetString(14),
			textType = SourceData.GetString(15),
			textAvatar = SourceData.GetString(16),
			detail_JP = SourceData.GetString(17),
			detail = SourceData.GetString(18),
			textBenefit_JP = SourceData.GetString(19),
			textBenefit = SourceData.GetString(20),
			textPet_JP = SourceData.GetString(21),
			textPet = SourceData.GetString(22)
		};
	}

	public override Row CreateRowByMapping(IReadOnlyDictionary<string, int> mapping)
	{
		return new Row
		{
			id = SourceData.GetString(mapping["id"]),
			name_JP = SourceData.GetString(mapping["name_JP"]),
			name = SourceData.GetString(mapping["name"]),
			name2_JP = SourceData.GetStringArray(mapping["name2_JP"]),
			name2 = SourceData.GetStringArray(mapping["name2"]),
			type = SourceData.GetString(mapping["type"]),
			idMaterial = SourceData.GetString(mapping["idMaterial"]),
			faith = SourceData.GetString(mapping["faith"]),
			domain = SourceData.GetString(mapping["domain"]),
			tax = SourceData.GetInt(mapping["tax"]),
			relation = SourceData.GetInt(mapping["relation"]),
			elements = Core.ParseElements(SourceData.GetStr(mapping["elements"])),
			cat_offer = SourceData.GetStringArray(mapping["cat_offer"]),
			rewards = SourceData.GetStringArray(mapping["rewards"]),
			textType_JP = SourceData.GetString(mapping["textType_JP"]),
			textType = SourceData.GetString(mapping["textType"]),
			textAvatar = SourceData.GetString(mapping["textAvatar"]),
			detail_JP = SourceData.GetString(mapping["detail_JP"]),
			detail = SourceData.GetString(mapping["detail"]),
			textBenefit_JP = SourceData.GetString(mapping["textBenefit_JP"]),
			textBenefit = SourceData.GetString(mapping["textBenefit"]),
			textPet_JP = SourceData.GetString(mapping["textPet_JP"]),
			textPet = SourceData.GetString(mapping["textPet"])
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
