using System;
using System.Collections.Generic;

public class SourceBlock : SourceDataInt<SourceBlock.Row>
{
	[Serializable]
	public class Row : TileRow
	{
		public string[] reqHarvest;

		public string idThing;

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
			snowTile = SourceData.GetInt(11),
			colorMod = SourceData.GetInt(12),
			colorType = SourceData.GetString(13),
			value = SourceData.GetInt(14),
			LV = SourceData.GetInt(15),
			recipeKey = SourceData.GetStringArray(16),
			factory = SourceData.GetStringArray(17),
			components = SourceData.GetStringArray(18),
			defMat = SourceData.GetString(19),
			category = SourceData.GetString(20),
			roof = SourceData.GetInt(21),
			autoFloor = SourceData.GetString(22),
			concrete = SourceData.GetBool(23),
			transparent = SourceData.GetBool(24),
			transition = SourceData.GetIntArray(25),
			tag = SourceData.GetStringArray(26),
			soundFoot = SourceData.GetString(27),
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
