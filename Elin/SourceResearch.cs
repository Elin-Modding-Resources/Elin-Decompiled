using System;

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

	public override void SetRow(Row r)
	{
		map[r.id] = r;
	}
}
