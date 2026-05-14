using System;
using System.Collections.Generic;

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

	public static readonly IReadOnlyDictionary<string, int> RowMapping = new Dictionary<string, int>
	{
		["id"] = 0,
		["parent"] = 1,
		["name_JP"] = 2,
		["name"] = 3,
		["type"] = 4,
		["LV"] = 5,
		["chance"] = 6,
		["faction"] = 7,
		["value"] = 8,
		["idProfile"] = 9,
		["idFile"] = 10,
		["idBiome"] = 11,
		["idGen"] = 12,
		["idPlaylist"] = 13,
		["tag"] = 14,
		["cost"] = 15,
		["dev"] = 16,
		["image"] = 17,
		["pos"] = 18,
		["questTag"] = 19,
		["textFlavor_JP"] = 20,
		["textFlavor"] = 21,
		["detail_JP"] = 22,
		["detail"] = 23
	};

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

	public override Row CreateRowByMapping(IReadOnlyDictionary<string, int> mapping)
	{
		return new Row
		{
			id = SourceData.GetString(mapping["id"]),
			parent = SourceData.GetString(mapping["parent"]),
			name_JP = SourceData.GetString(mapping["name_JP"]),
			name = SourceData.GetString(mapping["name"]),
			type = SourceData.GetString(mapping["type"]),
			LV = SourceData.GetInt(mapping["LV"]),
			chance = SourceData.GetInt(mapping["chance"]),
			faction = SourceData.GetString(mapping["faction"]),
			value = SourceData.GetInt(mapping["value"]),
			idProfile = SourceData.GetString(mapping["idProfile"]),
			idFile = SourceData.GetStringArray(mapping["idFile"]),
			idBiome = SourceData.GetString(mapping["idBiome"]),
			idGen = SourceData.GetString(mapping["idGen"]),
			idPlaylist = SourceData.GetString(mapping["idPlaylist"]),
			tag = SourceData.GetStringArray(mapping["tag"]),
			cost = SourceData.GetInt(mapping["cost"]),
			dev = SourceData.GetInt(mapping["dev"]),
			image = SourceData.GetString(mapping["image"]),
			pos = SourceData.GetIntArray(mapping["pos"]),
			questTag = SourceData.GetStringArray(mapping["questTag"]),
			textFlavor_JP = SourceData.GetString(mapping["textFlavor_JP"]),
			textFlavor = SourceData.GetString(mapping["textFlavor"]),
			detail_JP = SourceData.GetString(mapping["detail_JP"]),
			detail = SourceData.GetString(mapping["detail"])
		};
	}

	public override void SetRow(Row r)
	{
		map[r.id] = r;
	}

	public override IReadOnlyDictionary<string, int> GetRowMapping()
	{
		return RowMapping;
	}
}
