using System;
using System.Collections.Generic;

public class SourceCellEffect : SourceDataInt<SourceCellEffect.Row>
{
	[Serializable]
	public class Row : TileRow
	{
		public int[] anime;

		public override bool UseAlias => true;

		public override string GetAlias => alias;

		public override string RecipeID => "l" + id;

		public override RenderData defaultRenderData => FallbackRenderData;

		public override int GetTile(SourceMaterial.Row mat, int dir = 0)
		{
			return _tiles[0] + 3;
		}
	}

	public static readonly IReadOnlyDictionary<string, int> RowMapping = new Dictionary<string, int>
	{
		["id"] = 0,
		["alias"] = 1,
		["name_JP"] = 2,
		["name"] = 3,
		["sort"] = 4,
		["_tileType"] = 5,
		["_idRenderData"] = 6,
		["tiles"] = 7,
		["anime"] = 8,
		["colorMod"] = 9,
		["value"] = 10,
		["recipeKey"] = 11,
		["factory"] = 12,
		["components"] = 13,
		["defMat"] = 14,
		["category"] = 15,
		["tag"] = 16,
		["detail_JP"] = 17,
		["detail"] = 18
	};

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
			_tileType = SourceData.GetString(5),
			_idRenderData = SourceData.GetString(6),
			tiles = SourceData.GetIntArray(7),
			anime = SourceData.GetIntArray(8),
			colorMod = SourceData.GetInt(9),
			value = SourceData.GetInt(10),
			recipeKey = SourceData.GetStringArray(11),
			factory = SourceData.GetStringArray(12),
			components = SourceData.GetStringArray(13),
			defMat = SourceData.GetString(14),
			category = SourceData.GetString(15),
			tag = SourceData.GetStringArray(16),
			detail_JP = SourceData.GetString(17),
			detail = SourceData.GetString(18)
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
			_tileType = SourceData.GetString(mapping["_tileType"]),
			_idRenderData = SourceData.GetString(mapping["_idRenderData"]),
			tiles = SourceData.GetIntArray(mapping["tiles"]),
			anime = SourceData.GetIntArray(mapping["anime"]),
			colorMod = SourceData.GetInt(mapping["colorMod"]),
			value = SourceData.GetInt(mapping["value"]),
			recipeKey = SourceData.GetStringArray(mapping["recipeKey"]),
			factory = SourceData.GetStringArray(mapping["factory"]),
			components = SourceData.GetStringArray(mapping["components"]),
			defMat = SourceData.GetString(mapping["defMat"]),
			category = SourceData.GetString(mapping["category"]),
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
		FallbackRenderData = ResourceCache.Load<RenderData>("Scene/Render/Data/liquid");
		Cell.effectList = rows;
		foreach (Row row in rows)
		{
			row.Init();
		}
	}
}
