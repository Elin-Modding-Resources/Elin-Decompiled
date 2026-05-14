using System;
using System.Collections.Generic;

public class SourceBlock : SourceDataInt<SourceBlock.Row>
{
	[Serializable]
	public class Row : TileRow
	{
		public string[] reqHarvest;

		public string idThing;

		public int[] anime;

		public int roof;

		public string autoFloor;

		public bool concrete;

		public bool transparent;

		public int[] transition;

		[NonSerialized]
		public bool isBlockOrRamp;

		[NonSerialized]
		public SourceFloor.Row sourceAutoFloor;

		[NonSerialized]
		public string name_L;

		[NonSerialized]
		public string detail_L;

		public override bool UseAlias => true;

		public override string GetAlias => alias;

		public override string RecipeID => "b" + id;

		public override RenderData defaultRenderData => FallbackRenderData;

		public override void OnInit()
		{
			isBlockOrRamp = tileType == TileType.Block || tileType.IsRamp;
		}

		public override int GetTile(SourceMaterial.Row mat, int dir = 0)
		{
			return _tiles[dir % _tiles.Length];
		}

		public override RenderParam GetRenderParam(SourceMaterial.Row mat, int dir, Point point = null, int bridgeHeight = -1)
		{
			RenderParam renderParam = base.GetRenderParam(mat, dir, point, bridgeHeight);
			if (tileType == TileType.HalfBlock)
			{
				int num = 104025;
				Row row = ((id == 5) ? base.sources.blocks.rows[mat.defBlock] : this);
				renderParam.tile = row._tiles[0];
				renderParam.matColor = ((row.colorMod == 0) ? num : BaseTileMap.GetColorInt(ref mat.matColor, row.colorMod));
				renderParam.tile2 = row.sourceAutoFloor._tiles[0];
				renderParam.halfBlockColor = ((row.sourceAutoFloor.colorMod == 0) ? num : BaseTileMap.GetColorInt(ref mat.matColor, row.sourceAutoFloor.colorMod));
			}
			return renderParam;
		}
	}

	public static readonly IReadOnlyDictionary<string, int> RowMapping = new Dictionary<string, int>
	{
		["id"] = 0,
		["alias"] = 1,
		["name_JP"] = 2,
		["name"] = 3,
		["sort"] = 4,
		["reqHarvest"] = 5,
		["hp"] = 6,
		["idThing"] = 7,
		["_tileType"] = 8,
		["_idRenderData"] = 9,
		["tiles"] = 10,
		["anime"] = 11,
		["snowTile"] = 12,
		["colorMod"] = 13,
		["colorType"] = 14,
		["value"] = 15,
		["LV"] = 16,
		["recipeKey"] = 17,
		["factory"] = 18,
		["components"] = 19,
		["defMat"] = 20,
		["category"] = 21,
		["roof"] = 22,
		["autoFloor"] = 23,
		["concrete"] = 24,
		["transparent"] = 25,
		["transition"] = 26,
		["tag"] = 27,
		["soundFoot"] = 28,
		["detail_JP"] = 29,
		["detail"] = 30
	};

	public Dictionary<int, Row> _rows = new Dictionary<int, Row>();

	public static RenderData FallbackRenderData;

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetInt(0),
			alias = SourceData.GetString(1),
			name_JP = SourceData.GetString(2),
			name = SourceData.GetString(3),
			sort = SourceData.GetInt(4),
			reqHarvest = SourceData.GetStringArray(5),
			hp = SourceData.GetInt(6),
			idThing = SourceData.GetString(7),
			_tileType = SourceData.GetString(8),
			_idRenderData = SourceData.GetString(9),
			tiles = SourceData.GetIntArray(10),
			anime = SourceData.GetIntArray(11),
			snowTile = SourceData.GetInt(12),
			colorMod = SourceData.GetInt(13),
			colorType = SourceData.GetString(14),
			value = SourceData.GetInt(15),
			LV = SourceData.GetInt(16),
			recipeKey = SourceData.GetStringArray(17),
			factory = SourceData.GetStringArray(18),
			components = SourceData.GetStringArray(19),
			defMat = SourceData.GetString(20),
			category = SourceData.GetString(21),
			roof = SourceData.GetInt(22),
			autoFloor = SourceData.GetString(23),
			concrete = SourceData.GetBool(24),
			transparent = SourceData.GetBool(25),
			transition = SourceData.GetIntArray(26),
			tag = SourceData.GetStringArray(27),
			soundFoot = SourceData.GetString(28),
			detail_JP = SourceData.GetString(29),
			detail = SourceData.GetString(30)
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
			reqHarvest = SourceData.GetStringArray(mapping["reqHarvest"]),
			hp = SourceData.GetInt(mapping["hp"]),
			idThing = SourceData.GetString(mapping["idThing"]),
			_tileType = SourceData.GetString(mapping["_tileType"]),
			_idRenderData = SourceData.GetString(mapping["_idRenderData"]),
			tiles = SourceData.GetIntArray(mapping["tiles"]),
			anime = SourceData.GetIntArray(mapping["anime"]),
			snowTile = SourceData.GetInt(mapping["snowTile"]),
			colorMod = SourceData.GetInt(mapping["colorMod"]),
			colorType = SourceData.GetString(mapping["colorType"]),
			value = SourceData.GetInt(mapping["value"]),
			LV = SourceData.GetInt(mapping["LV"]),
			recipeKey = SourceData.GetStringArray(mapping["recipeKey"]),
			factory = SourceData.GetStringArray(mapping["factory"]),
			components = SourceData.GetStringArray(mapping["components"]),
			defMat = SourceData.GetString(mapping["defMat"]),
			category = SourceData.GetString(mapping["category"]),
			roof = SourceData.GetInt(mapping["roof"]),
			autoFloor = SourceData.GetString(mapping["autoFloor"]),
			concrete = SourceData.GetBool(mapping["concrete"]),
			transparent = SourceData.GetBool(mapping["transparent"]),
			transition = SourceData.GetIntArray(mapping["transition"]),
			tag = SourceData.GetStringArray(mapping["tag"]),
			soundFoot = SourceData.GetString(mapping["soundFoot"]),
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
		FallbackRenderData = ResourceCache.Load<RenderData>("Scene/Render/Data/block");
		Cell.blockList = rows;
		SourceFloor floors = Core.Instance.sources.floors;
		foreach (Row row in rows)
		{
			row.Init();
			row.sourceAutoFloor = (row.autoFloor.IsEmpty() ? floors.rows[40] : floors.alias[row.autoFloor]);
		}
	}
}
