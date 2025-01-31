using System;

public class SourceHomeResource : SourceDataString<SourceHomeResource.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public string id;

		public string name_JP;

		public string name;

		public int expMod;

		public int maxLv;

		public string[] reward;

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
			expMod = SourceData.GetInt(3),
			maxLv = SourceData.GetInt(4),
			reward = SourceData.GetStringArray(5),
			detail_JP = SourceData.GetString(6),
			detail = SourceData.GetString(7)
		};
	}

	public override void SetRow(Row r)
	{
		map[r.id] = r;
	}
}
