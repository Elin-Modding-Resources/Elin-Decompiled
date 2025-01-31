using System;

public class SourceCollectible : SourceDataString<SourceCollectible.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public string id;

		public string name_JP;

		public string name;

		public int rarity;

		public string prefab;

		public int num;

		public string filter;

		public string[] tag;

		public string sound;

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
			rarity = SourceData.GetInt(3),
			prefab = SourceData.GetString(4),
			num = SourceData.GetInt(5),
			filter = SourceData.GetString(6),
			tag = SourceData.GetStringArray(7),
			sound = SourceData.GetString(8),
			detail_JP = SourceData.GetString(9),
			detail = SourceData.GetString(10)
		};
	}

	public override void SetRow(Row r)
	{
		map[r.id] = r;
	}
}
