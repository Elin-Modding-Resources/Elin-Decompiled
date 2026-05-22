using System;
using System.Collections.Generic;

public class SourceGlobalTile : SourceDataInt<SourceGlobalTile.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public int id;

		public string alias;

		public string name_JP;

		public string name;

		public int[] tiles;

		public int floor;

		public string zoneProfile;

		public string[] tag;

		public int dangerLv;

		public string[] trait;

		public string idBiome;

		public int[] attribs;

		public string detail_JP;

		public string detail;

		public override bool UseAlias => true;

		public override string GetAlias => alias;
	}

	public static readonly IReadOnlyDictionary<string, int> RowMapping = new Dictionary<string, int>
	{
		["id"] = 0,
		["alias"] = 1,
		["name_JP"] = 2,
		["name"] = 3,
		["tiles"] = 4,
		["floor"] = 5,
		["zoneProfile"] = 6,
		["tag"] = 7,
		["dangerLv"] = 8,
		["trait"] = 9,
		["idBiome"] = 10,
		["attribs"] = 11,
		["detail_JP"] = 12,
		["detail"] = 13
	};

	public static readonly IReadOnlyDictionary<string, string> TypeMapping = new Dictionary<string, string>
	{
		["id"] = "int",
		["alias"] = "string",
		["name_JP"] = "string",
		["name"] = "string",
		["tiles"] = "int[]",
		["floor"] = "int",
		["zoneProfile"] = "string",
		["tag"] = "string[]",
		["dangerLv"] = "int",
		["trait"] = "string[]",
		["idBiome"] = "string",
		["attribs"] = "int[]",
		["detail_JP"] = "string",
		["detail"] = "string"
	};

	public Dictionary<int, Row> tileAlias = new Dictionary<int, Row>();

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetInt(0),
			alias = SourceData.GetString(1),
			name_JP = SourceData.GetString(2),
			name = SourceData.GetString(3),
			tiles = SourceData.GetIntArray(4),
			floor = SourceData.GetInt(5),
			zoneProfile = SourceData.GetString(6),
			tag = SourceData.GetStringArray(7),
			dangerLv = SourceData.GetInt(8),
			trait = SourceData.GetStringArray(9),
			idBiome = SourceData.GetString(10),
			attribs = SourceData.GetIntArray(11),
			detail_JP = SourceData.GetString(12),
			detail = SourceData.GetString(13)
		};
	}

	public override Row CreateRowByMapping(IReadOnlyDictionary<string, int> mapping)
	{
		return new Row
		{
			id = SourceData.GetInt(mapping["id"]),
			alias = SourceData.GetString(mapping["alias"]),
			name_JP = SourceData.GetString(mapping["name_JP"]),
			name = SourceData.GetString(mapping["name"]),
			tiles = SourceData.GetIntArray(mapping["tiles"]),
			floor = SourceData.GetInt(mapping["floor"]),
			zoneProfile = SourceData.GetString(mapping["zoneProfile"]),
			tag = SourceData.GetStringArray(mapping["tag"]),
			dangerLv = SourceData.GetInt(mapping["dangerLv"]),
			trait = SourceData.GetStringArray(mapping["trait"]),
			idBiome = SourceData.GetString(mapping["idBiome"]),
			attribs = SourceData.GetIntArray(mapping["attribs"]),
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

	public override IReadOnlyDictionary<string, string> GetTypeMapping()
	{
		return TypeMapping;
	}

	public override void OnInit()
	{
		foreach (Row row in rows)
		{
			int[] tiles = row.tiles;
			foreach (int key in tiles)
			{
				tileAlias[key] = row;
			}
		}
	}
}
