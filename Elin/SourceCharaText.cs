using System;

public class SourceCharaText : SourceDataString<SourceCharaText.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public string id;

		public string calm_JP;

		public string fov_JP;

		public string aggro_JP;

		public string dead_JP;

		public string kill_JP;

		public string calm;

		public string fov;

		public string aggro;

		public string dead;

		public string kill;

		[NonSerialized]
		public string calm_L;

		[NonSerialized]
		public string fov_L;

		[NonSerialized]
		public string aggro_L;

		[NonSerialized]
		public string dead_L;

		[NonSerialized]
		public string kill_L;

		public override bool UseAlias => false;

		public override string GetAlias => "n";
	}

	public override string[] ImportFields => new string[5] { "calm", "fov", "aggro", "dead", "kill" };

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetString(0),
			calm_JP = SourceData.GetString(2),
			fov_JP = SourceData.GetString(3),
			aggro_JP = SourceData.GetString(4),
			dead_JP = SourceData.GetString(5),
			kill_JP = SourceData.GetString(6),
			calm = SourceData.GetString(7),
			fov = SourceData.GetString(8),
			aggro = SourceData.GetString(9),
			dead = SourceData.GetString(10),
			kill = SourceData.GetString(11)
		};
	}

	public override void SetRow(Row r)
	{
		map[r.id] = r;
	}
}
