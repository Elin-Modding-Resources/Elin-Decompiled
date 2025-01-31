using System;

public class SourceKeyItem : SourceDataInt<SourceKeyItem.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public int id;

		public string alias;

		public string name_JP;

		public string name;

		public string detail_JP;

		public string detail;

		[NonSerialized]
		public string name_L;

		[NonSerialized]
		public string detail_L;

		public override bool UseAlias => true;

		public override string GetAlias => alias;
	}

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetInt(0),
			alias = SourceData.GetString(1),
			name_JP = SourceData.GetString(2),
			name = SourceData.GetString(3),
			detail_JP = SourceData.GetString(4),
			detail = SourceData.GetString(5)
		};
	}

	public override void SetRow(Row r)
	{
		map[r.id] = r;
	}
}
