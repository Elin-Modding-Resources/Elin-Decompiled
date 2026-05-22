using System;
using System.Collections.Generic;

public class SourceObj : SourceDataInt<SourceObj.Row>
{
	[Serializable]
	public class Row : TileRow
	{
		public string[] _growth;

		public int costSoil;

		public string objType;

		public string[] vals;

		public string[] reqHarvest;

		public string valType;

		public int[] anime;

		public string matCategory;

		public int idRoof;

		[NonSerialized]
		public bool HasGrowth;

		[NonSerialized]
		public bool autoTile;

		public GrowSystem growth;

		public ObjValType objValType;

		[NonSerialized]
		public string name_L;

		[NonSerialized]
		public string detail_L;

		public override bool UseAlias => true;

		public override string GetAlias => alias;

		public override string RecipeID => "o" + id;

		public override RenderData defaultRenderData => FallbackRenderData;

		public override void OnInit()
		{
			objValType = ((!valType.IsEmpty()) ? valType.ToEnum<ObjValType>() : ObjValType.None);
			autoTile = tag.Contains("autotile");
			if (!_growth.IsEmpty())
			{
				growth = ClassCache.Create<GrowSystem>("GrowSystem" + _growth[0], "Elin");
				growth.Init(this);
				HasGrowth = true;
			}
			else
			{
				HasGrowth = false;
			}
		}
	}

	public class Stage
	{
		public int step;

		public int[] tiles;

		public string idThing;

		public bool harvest;
	}

	public static readonly IReadOnlyDictionary<string, int> RowMapping = new Dictionary<string, int>
	{
		["id"] = 0,
		["alias"] = 1,
		["name_JP"] = 2,
		["name"] = 3,
		["_growth"] = 4,
		["costSoil"] = 5,
		["objType"] = 6,
		["vals"] = 7,
		["tag"] = 8,
		["sort"] = 9,
		["reqHarvest"] = 10,
		["hp"] = 11,
		["_tileType"] = 12,
		["valType"] = 13,
		["_idRenderData"] = 14,
		["tiles"] = 15,
		["anime"] = 16,
		["snowTile"] = 17,
		["colorMod"] = 18,
		["colorType"] = 19,
		["value"] = 20,
		["LV"] = 21,
		["chance"] = 22,
		["recipeKey"] = 23,
		["factory"] = 24,
		["components"] = 25,
		["defMat"] = 26,
		["matCategory"] = 27,
		["category"] = 28,
		["idRoof"] = 29,
		["detail_JP"] = 30,
		["detail"] = 31
	};

	public static readonly IReadOnlyDictionary<string, string> TypeMapping = new Dictionary<string, string>
	{
		["id"] = "int",
		["alias"] = "string",
		["name_JP"] = "string",
		["name"] = "string",
		["_growth"] = "string[]",
		["costSoil"] = "int",
		["objType"] = "string",
		["vals"] = "string[]",
		["tag"] = "string[]",
		["sort"] = "int",
		["reqHarvest"] = "string[]",
		["hp"] = "int",
		["_tileType"] = "string",
		["valType"] = "string",
		["_idRenderData"] = "string",
		["tiles"] = "int[]",
		["anime"] = "int[]",
		["snowTile"] = "int",
		["colorMod"] = "int",
		["colorType"] = "string",
		["value"] = "int",
		["LV"] = "int",
		["chance"] = "int",
		["recipeKey"] = "string[]",
		["factory"] = "string[]",
		["components"] = "string[]",
		["defMat"] = "string",
		["matCategory"] = "string",
		["category"] = "string",
		["idRoof"] = "int",
		["detail_JP"] = "string",
		["detail"] = "string"
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
			_growth = SourceData.GetStringArray(4),
			costSoil = SourceData.GetInt(5),
			objType = SourceData.GetString(6),
			vals = SourceData.GetStringArray(7),
			tag = SourceData.GetStringArray(8),
			sort = SourceData.GetInt(9),
			reqHarvest = SourceData.GetStringArray(10),
			hp = SourceData.GetInt(11),
			_tileType = SourceData.GetString(12),
			valType = SourceData.GetString(13),
			_idRenderData = SourceData.GetString(14),
			tiles = SourceData.GetIntArray(15),
			anime = SourceData.GetIntArray(16),
			snowTile = SourceData.GetInt(17),
			colorMod = SourceData.GetInt(18),
			colorType = SourceData.GetString(19),
			value = SourceData.GetInt(20),
			LV = SourceData.GetInt(21),
			chance = SourceData.GetInt(22),
			recipeKey = SourceData.GetStringArray(23),
			factory = SourceData.GetStringArray(24),
			components = SourceData.GetStringArray(25),
			defMat = SourceData.GetString(26),
			matCategory = SourceData.GetString(27),
			category = SourceData.GetString(28),
			idRoof = SourceData.GetInt(29),
			detail_JP = SourceData.GetString(30),
			detail = SourceData.GetString(31)
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
			_growth = SourceData.GetStringArray(mapping["_growth"]),
			costSoil = SourceData.GetInt(mapping["costSoil"]),
			objType = SourceData.GetString(mapping["objType"]),
			vals = SourceData.GetStringArray(mapping["vals"]),
			tag = SourceData.GetStringArray(mapping["tag"]),
			sort = SourceData.GetInt(mapping["sort"]),
			reqHarvest = SourceData.GetStringArray(mapping["reqHarvest"]),
			hp = SourceData.GetInt(mapping["hp"]),
			_tileType = SourceData.GetString(mapping["_tileType"]),
			valType = SourceData.GetString(mapping["valType"]),
			_idRenderData = SourceData.GetString(mapping["_idRenderData"]),
			tiles = SourceData.GetIntArray(mapping["tiles"]),
			anime = SourceData.GetIntArray(mapping["anime"]),
			snowTile = SourceData.GetInt(mapping["snowTile"]),
			colorMod = SourceData.GetInt(mapping["colorMod"]),
			colorType = SourceData.GetString(mapping["colorType"]),
			value = SourceData.GetInt(mapping["value"]),
			LV = SourceData.GetInt(mapping["LV"]),
			chance = SourceData.GetInt(mapping["chance"]),
			recipeKey = SourceData.GetStringArray(mapping["recipeKey"]),
			factory = SourceData.GetStringArray(mapping["factory"]),
			components = SourceData.GetStringArray(mapping["components"]),
			defMat = SourceData.GetString(mapping["defMat"]),
			matCategory = SourceData.GetString(mapping["matCategory"]),
			category = SourceData.GetString(mapping["category"]),
			idRoof = SourceData.GetInt(mapping["idRoof"]),
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

	public string GetName(int id)
	{
		return map[id].GetName().ToTitleCase();
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
		FallbackRenderData = ResourceCache.Load<RenderData>("Scene/Render/Data/obj");
		Cell.objList = rows;
		foreach (Row row in rows)
		{
			row.Init();
		}
	}
}
