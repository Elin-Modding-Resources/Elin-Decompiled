using System;

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

	public override void SetRow(Row r)
	{
		map[r.id] = r;
	}
}
