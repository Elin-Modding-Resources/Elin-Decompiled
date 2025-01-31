using System;

public class SourceZone : SourceDataString<SourceZone.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public string id;

		public string parent;

		public string name_JP;

		public string name;

		public string type;

		public int LV;

		public int chance;

		public string faction;

		public int value;

		public string idProfile;

		public string[] idFile;

		public string idBiome;

		public string idGen;

		public string idPlaylist;

		public string[] tag;

		public int cost;

		public int dev;

		public string image;

		public int[] pos;

		public string[] questTag;

		public string textFlavor_JP;

		public string textFlavor;

		public string detail_JP;

		public string detail;

		[NonSerialized]
		public string name_L;

		[NonSerialized]
		public string detail_L;

		[NonSerialized]
		public string textFlavor_L;

		public override bool UseAlias => false;

		public override string GetAlias => "n";
	}

	public override string[] ImportFields => new string[1] { "textFlavor" };

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetString(0),
			parent = SourceData.GetString(1),
			name_JP = SourceData.GetString(2),
			name = SourceData.GetString(3),
			type = SourceData.GetString(4),
			LV = SourceData.GetInt(5),
			chance = SourceData.GetInt(6),
			faction = SourceData.GetString(7),
			value = SourceData.GetInt(8),
			idProfile = SourceData.GetString(9),
			idFile = SourceData.GetStringArray(10),
			idBiome = SourceData.GetString(11),
			idGen = SourceData.GetString(12),
			idPlaylist = SourceData.GetString(13),
			tag = SourceData.GetStringArray(14),
			cost = SourceData.GetInt(15),
			dev = SourceData.GetInt(16),
			image = SourceData.GetString(17),
			pos = SourceData.GetIntArray(18),
			questTag = SourceData.GetStringArray(19),
			textFlavor_JP = SourceData.GetString(20),
			textFlavor = SourceData.GetString(21),
			detail_JP = SourceData.GetString(22),
			detail = SourceData.GetString(23)
		};
	}

	public override void SetRow(Row r)
	{
		map[r.id] = r;
	}
}
