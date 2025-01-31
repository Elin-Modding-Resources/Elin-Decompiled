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
			snowTile = SourceData.GetInt(16),
			colorMod = SourceData.GetInt(17),
			colorType = SourceData.GetString(18),
			value = SourceData.GetInt(19),
			LV = SourceData.GetInt(20),
			chance = SourceData.GetInt(21),
			recipeKey = SourceData.GetStringArray(22),
			factory = SourceData.GetStringArray(23),
			components = SourceData.GetStringArray(24),
			defMat = SourceData.GetString(25),
			matCategory = SourceData.GetString(26),
			category = SourceData.GetString(27),
			idRoof = SourceData.GetInt(28),
			detail_JP = SourceData.GetString(29),
			detail = SourceData.GetString(30)
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
