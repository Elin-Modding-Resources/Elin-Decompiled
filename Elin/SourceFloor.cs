using System;
using System.Collections.Generic;

public class SourceFloor : SourceDataInt<SourceFloor.Row>
{
	[Serializable]
	public class Row : TileRow
	{
		public string idBiome;

		public string[] reqHarvest;

		public int[] anime;

		public string defBlock;

		public string bridgeBlock;

		public int edge;

		public int autotile;

		public int autotilePriority;

		public float autotileBrightness;

		public bool nonGradient;

		public bool isBeach;

		public bool snowtile;

		public bool ignoreTransition;

		[NonSerialized]
		public SourceBlock.Row _defBlock;

		[NonSerialized]
		public SourceBlock.Row _bridgeBlock;

		[NonSerialized]
		public BiomeProfile biome;

		[NonSerialized]
		public string name_L;

		[NonSerialized]
		public string detail_L;

		public override bool UseAlias => true;

		public override string GetAlias => alias;

		public override string RecipeID => "f" + id;

		public override RenderData defaultRenderData => FallbackRenderData;

		public override void OnInit()
		{
			ignoreTransition = tag.Contains("noTransition");
			ignoreSnow = tag.Contains("noSnow");
			isBeach = tag.Contains("beach");
			snowtile = tag.Contains("snowtile");
			if (!idBiome.IsEmpty())
			{
				biome = EClass.core.refs.biomes.dict[idBiome];
			}
		}

		public override int GetTile(SourceMaterial.Row mat, int dir = 0)
		{
			return _tiles[dir % _tiles.Length];
		}
	}

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
			defBlock = SourceData.GetString(19),
			bridgeBlock = SourceData.GetString(20),
			category = SourceData.GetString(21),
			edge = SourceData.GetInt(22),
			autotile = SourceData.GetInt(23),
			autotilePriority = SourceData.GetInt(24),
			autotileBrightness = SourceData.GetFloat(25),
			soundFoot = SourceData.GetString(26),
			tag = SourceData.GetStringArray(27),
			detail_JP = SourceData.GetString(28),
			detail = SourceData.GetString(29)
		};
	}

	public override void SetRow(Row r)
	{
		map[r.id] = r;
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
		FallbackRenderData = ResourceCache.Load<RenderData>("Scene/Render/Data/floor");
		Cell.floorList = rows;
		foreach (Row row in rows)
		{
			row.Init();
		}
	}

	public void OnAfterInit()
	{
		foreach (Row row in rows)
		{
			row._defBlock = EClass.sources.blocks.alias[row.defBlock];
			row._bridgeBlock = EClass.sources.blocks.alias[row.bridgeBlock];
			row.nonGradient = row.ContainsTag("nonGradient");
		}
	}
}
