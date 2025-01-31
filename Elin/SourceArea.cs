using System;

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

	public override void SetRow(Row r)
	{
		map[r.id] = r;
	}
}
