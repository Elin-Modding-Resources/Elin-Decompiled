using System;

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

	public override void SetRow(Row r)
	{
		map[r.id] = r;
	}
}
