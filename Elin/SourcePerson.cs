using System;

public class SourcePerson : SourceDataString<SourcePerson.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public string id;

		public string idActor;

		public string name_JP;

		public string name;

		public string aka_JP;

		public string aka;

		public string portrait;

		public string faction;

		public int LV;

		public string job;

		public string race;

		public string material;

		public string bio;

		public string detail_JP;

		public string detail;

		[NonSerialized]
		public string name_L;

		[NonSerialized]
		public string detail_L;

		[NonSerialized]
		public string aka_L;

		public override bool UseAlias => false;

		public override string GetAlias => "n";
	}

	public override string[] ImportFields => new string[1] { "aka" };

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetString(0),
			idActor = SourceData.GetString(1),
			name_JP = SourceData.GetString(2),
			name = SourceData.GetString(3),
			aka_JP = SourceData.GetString(4),
			aka = SourceData.GetString(5),
			portrait = SourceData.GetString(6),
			faction = SourceData.GetString(7),
			LV = SourceData.GetInt(8),
			job = SourceData.GetString(9),
			race = SourceData.GetString(10),
			material = SourceData.GetString(11),
			bio = SourceData.GetString(12),
			detail_JP = SourceData.GetString(13),
			detail = SourceData.GetString(14)
		};
	}

	public override void SetRow(Row r)
	{
		map[r.id] = r;
	}
}
