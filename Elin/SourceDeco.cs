using System;
using System.Collections.Generic;

public class SourceDeco : SourceDataInt<SourceDeco.Row>
{
	[Serializable]
	public class Row : TileRow
	{
		public string idBiome;

		public string[] reqHarvest;

		public int[] anime;

		public int edge;

		public int autotile;

		public int autotilePriority;

		public float autotileBrightness;

		public override bool UseAlias => true;

		public override string GetAlias => alias;

		public override string RecipeID => "d" + id;

		public override int GetTile(SourceMaterial.Row mat, int dir = 0)
		{
			return _tiles[dir % _tiles.Length];
		}
	}

	public static readonly IReadOnlyDictionary<string, int> RowMapping = new Dictionary<string, int>
	{
		["id"] = 0,
		["alias"] = 1,
		["name_JP"] = 2,
		["name"] = 3,
		["sort"] = 4,
		["idBiome"] = 5,
		["reqHarvest"] = 6,
		["hp"] = 7,
		["_tileType"] = 8,
		["_idRenderData"] = 9,
		["tiles"] = 10,
		["anime"] = 11,
		["colorMod"] = 12,
		["value"] = 13,
		["LV"] = 14,
		["recipeKey"] = 15,
		["factory"] = 16,
		["components"] = 17,
		["defMat"] = 18,
		["category"] = 19,
		["edge"] = 20,
		["autotile"] = 21,
		["autotilePriority"] = 22,
		["autotileBrightness"] = 23,
		["soundFoot"] = 24,
		["tag"] = 25,
		["detail_JP"] = 26,
		["detail"] = 27
	};

	public Dictionary<int, Row> _rows = new Dictionary<int, Row>();

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetInt(0),
			alias = SourceData.GetString(1),
			name_JP = SourceData.GetString(2),
			name = SourceData.GetString(3),
			sort = SourceData.GetInt(4),
			idBiome = SourceData.GetString(5),
			reqHarvest = SourceData.GetStringArray(6),
			hp = SourceData.GetInt(7),
			_tileType = SourceData.GetString(8),
			_idRenderData = SourceData.GetString(9),
			tiles = SourceData.GetIntArray(10),
			anime = SourceData.GetIntArray(11),
			colorMod = SourceData.GetInt(12),
			value = SourceData.GetInt(13),
			LV = SourceData.GetInt(14),
			recipeKey = SourceData.GetStringArray(15),
			factory = SourceData.GetStringArray(16),
			components = SourceData.GetStringArray(17),
			defMat = SourceData.GetString(18),
			category = SourceData.GetString(19),
			edge = SourceData.GetInt(20),
			autotile = SourceData.GetInt(21),
			autotilePriority = SourceData.GetInt(22),
			autotileBrightness = SourceData.GetFloat(23),
			soundFoot = SourceData.GetString(24),
			tag = SourceData.GetStringArray(25),
			detail_JP = SourceData.GetString(26),
			detail = SourceData.GetString(27)
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
			sort = SourceData.GetInt(mapping["sort"]),
			idBiome = SourceData.GetString(mapping["idBiome"]),
			reqHarvest = SourceData.GetStringArray(mapping["reqHarvest"]),
			hp = SourceData.GetInt(mapping["hp"]),
			_tileType = SourceData.GetString(mapping["_tileType"]),
			_idRenderData = SourceData.GetString(mapping["_idRenderData"]),
			tiles = SourceData.GetIntArray(mapping["tiles"]),
			anime = SourceData.GetIntArray(mapping["anime"]),
			colorMod = SourceData.GetInt(mapping["colorMod"]),
			value = SourceData.GetInt(mapping["value"]),
			LV = SourceData.GetInt(mapping["LV"]),
			recipeKey = SourceData.GetStringArray(mapping["recipeKey"]),
			factory = SourceData.GetStringArray(mapping["factory"]),
			components = SourceData.GetStringArray(mapping["components"]),
			defMat = SourceData.GetString(mapping["defMat"]),
			category = SourceData.GetString(mapping["category"]),
			edge = SourceData.GetInt(mapping["edge"]),
			autotile = SourceData.GetInt(mapping["autotile"]),
			autotilePriority = SourceData.GetInt(mapping["autotilePriority"]),
			autotileBrightness = SourceData.GetFloat(mapping["autotileBrightness"]),
			soundFoot = SourceData.GetString(mapping["soundFoot"]),
			tag = SourceData.GetStringArray(mapping["tag"]),
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

	public override void BackupPref()
	{
		_rows.Clear();
		foreach (Row row in rows)
		{
			_rows[row.id] = row;
		}
	}

	public override void RestorePref()
	{
		foreach (Row row in rows)
		{
			row.pref = _rows.TryGetValue(row.id)?.pref ?? new SourcePref();
		}
	}

	public override void ValidatePref()
	{
		foreach (Row row in rows)
		{
			row.pref.Validate();
		}
	}

	public override void OnAfterImportData()
	{
		int num = 0;
		foreach (Row row in rows)
		{
			if (row.sort != 0)
			{
				num = row.sort;
			}
			row.sort = num;
			num++;
		}
		rows.Sort((Row a, Row b) => a.id - b.id);
	}

	public override void OnInit()
	{
		Cell.decoList = rows;
		foreach (Row row in rows)
		{
			row.Init();
		}
	}
}
