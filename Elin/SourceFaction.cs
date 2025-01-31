using System;

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

	public override void SetRow(Row r)
	{
		map[r.id] = r;
	}
}
