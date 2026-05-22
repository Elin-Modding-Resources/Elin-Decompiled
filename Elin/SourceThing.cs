using System;
using System.Collections.Generic;

public class SourceThing : SourceDataString<SourceThing.Row>
{
	[Serializable]
	public class Row : CardRow
	{
		public string unknown_JP;

		public string unit_JP;

		public string naming;

		public string unit;

		public string unknown;

		public int[] altTiles;

		public int[] anime;

		public string[] disassemble;

		public int HP;

		public int weight;

		public int electricity;

		public int range;

		public string attackType;

		public int[] offense;

		public int[] substats;

		public int[] defense;

		public string idToggleExtra;

		public string idActorEx;

		public string workTag;

		public string[] roomName_JP;

		public string[] roomName;

		public int[] _altTiles;

		public bool ignoreAltFix;

		public bool animeNoSync;

		[NonSerialized]
		public string name_L;

		[NonSerialized]
		public string detail_L;

		[NonSerialized]
		public string unit_L;

		[NonSerialized]
		public string unknown_L;

		[NonSerialized]
		public string[] name2_L;

		[NonSerialized]
		public string[] roomName_L;

		public override bool UseAlias => false;

		public override string GetAlias => "n";

		public override string RecipeID => id;

		public override void OnImportData(SourceData data)
		{
			base.OnImportData(data);
			_altTiles = new int[0];
		}

		public override void SetTiles()
		{
			if (!renderData || !renderData.pass)
			{
				return;
			}
			base.SetTiles();
			if (_altTiles.Length != altTiles.Length)
			{
				_altTiles = new int[altTiles.Length];
				int num = 0;
				if (origin != null && !ignoreAltFix)
				{
					num = _tiles[0] - origin._tiles[0];
				}
				for (int i = 0; i < altTiles.Length; i++)
				{
					_altTiles[i] = altTiles[i] / 100 * (int)renderData.pass.pmesh.tiling.x + altTiles[i] % 100 + num;
				}
			}
			animeNoSync = tag.Contains("animeNoSync");
		}

		public override string GetName(SourceMaterial.Row mat, int sum)
		{
			if (naming == "m")
			{
				return base.GetName(mat, sum);
			}
			if (naming == "ma")
			{
				return mat.GetName() + " (" + sum + ")";
			}
			return GetName() + " (" + sum + ")";
		}

		public override string GetName()
		{
			string text = GetText();
			if (Lang.setting.nameStyle == 0)
			{
				return text;
			}
			if (!unit.IsEmpty())
			{
				return "_of".lang(text, unit);
			}
			return text;
		}

		public override string GetSearchName(bool jp)
		{
			if (jp)
			{
				return _nameSearchJP ?? (_nameSearchJP = GetText().ToLower());
			}
			return _nameSearch ?? (_nameSearch = (unit.IsEmpty() ? name : (unit + " of " + name)).ToLower());
		}
	}

	public static readonly IReadOnlyDictionary<string, int> RowMapping = new Dictionary<string, int>
	{
		["id"] = 0,
		["name_JP"] = 1,
		["unknown_JP"] = 2,
		["unit_JP"] = 3,
		["naming"] = 4,
		["name"] = 5,
		["unit"] = 6,
		["unknown"] = 7,
		["category"] = 8,
		["sort"] = 10,
		["_tileType"] = 11,
		["_idRenderData"] = 12,
		["tiles"] = 13,
		["altTiles"] = 14,
		["anime"] = 15,
		["skins"] = 16,
		["size"] = 17,
		["colorMod"] = 18,
		["colorType"] = 19,
		["recipeKey"] = 20,
		["factory"] = 21,
		["components"] = 22,
		["disassemble"] = 23,
		["defMat"] = 24,
		["tierGroup"] = 25,
		["value"] = 26,
		["LV"] = 27,
		["chance"] = 28,
		["quality"] = 29,
		["HP"] = 30,
		["weight"] = 31,
		["electricity"] = 32,
		["trait"] = 33,
		["elements"] = 34,
		["range"] = 35,
		["attackType"] = 36,
		["offense"] = 37,
		["substats"] = 38,
		["defense"] = 39,
		["lightData"] = 40,
		["idExtra"] = 41,
		["idToggleExtra"] = 42,
		["idActorEx"] = 43,
		["idSound"] = 44,
		["tag"] = 45,
		["workTag"] = 46,
		["filter"] = 47,
		["roomName_JP"] = 48,
		["roomName"] = 49,
		["detail_JP"] = 50,
		["detail"] = 51
	};

	public static readonly IReadOnlyDictionary<string, string> TypeMapping = new Dictionary<string, string>
	{
		["id"] = "string",
		["name_JP"] = "string",
		["unknown_JP"] = "string",
		["unit_JP"] = "string",
		["naming"] = "string",
		["name"] = "string",
		["unit"] = "string",
		["unknown"] = "string",
		["category"] = "string",
		["sort"] = "int",
		["_tileType"] = "string",
		["_idRenderData"] = "string",
		["tiles"] = "int[]",
		["altTiles"] = "int[]",
		["anime"] = "int[]",
		["skins"] = "int[]",
		["size"] = "int[]",
		["colorMod"] = "int",
		["colorType"] = "string",
		["recipeKey"] = "string[]",
		["factory"] = "string[]",
		["components"] = "string[]",
		["disassemble"] = "string[]",
		["defMat"] = "string",
		["tierGroup"] = "string",
		["value"] = "int",
		["LV"] = "int",
		["chance"] = "int",
		["quality"] = "int",
		["HP"] = "int",
		["weight"] = "int",
		["electricity"] = "int",
		["trait"] = "string[]",
		["elements"] = "elements",
		["range"] = "int",
		["attackType"] = "string",
		["offense"] = "int[]",
		["substats"] = "int[]",
		["defense"] = "int[]",
		["lightData"] = "string",
		["idExtra"] = "string",
		["idToggleExtra"] = "string",
		["idActorEx"] = "string",
		["idSound"] = "string",
		["tag"] = "string[]",
		["workTag"] = "string",
		["filter"] = "string[]",
		["roomName_JP"] = "string[]",
		["roomName"] = "string[]",
		["detail_JP"] = "string",
		["detail"] = "string"
	};

	public Dictionary<string, Row> _rows = new Dictionary<string, Row>();

	public override string[] ImportFields => new string[4] { "unit", "unknown", "roomName", "name2" };

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetString(0),
			name_JP = SourceData.GetString(1),
			unknown_JP = SourceData.GetString(2),
			unit_JP = SourceData.GetString(3),
			naming = SourceData.GetString(4),
			name = SourceData.GetString(5),
			unit = SourceData.GetString(6),
			unknown = SourceData.GetString(7),
			category = SourceData.GetString(8),
			sort = SourceData.GetInt(10),
			_tileType = SourceData.GetString(11),
			_idRenderData = SourceData.GetString(12),
			tiles = SourceData.GetIntArray(13),
			altTiles = SourceData.GetIntArray(14),
			anime = SourceData.GetIntArray(15),
			skins = SourceData.GetIntArray(16),
			size = SourceData.GetIntArray(17),
			colorMod = SourceData.GetInt(18),
			colorType = SourceData.GetString(19),
			recipeKey = SourceData.GetStringArray(20),
			factory = SourceData.GetStringArray(21),
			components = SourceData.GetStringArray(22),
			disassemble = SourceData.GetStringArray(23),
			defMat = SourceData.GetString(24),
			tierGroup = SourceData.GetString(25),
			value = SourceData.GetInt(26),
			LV = SourceData.GetInt(27),
			chance = SourceData.GetInt(28),
			quality = SourceData.GetInt(29),
			HP = SourceData.GetInt(30),
			weight = SourceData.GetInt(31),
			electricity = SourceData.GetInt(32),
			trait = SourceData.GetStringArray(33),
			elements = Core.ParseElements(SourceData.GetStr(34)),
			range = SourceData.GetInt(35),
			attackType = SourceData.GetString(36),
			offense = SourceData.GetIntArray(37),
			substats = SourceData.GetIntArray(38),
			defense = SourceData.GetIntArray(39),
			lightData = SourceData.GetString(40),
			idExtra = SourceData.GetString(41),
			idToggleExtra = SourceData.GetString(42),
			idActorEx = SourceData.GetString(43),
			idSound = SourceData.GetString(44),
			tag = SourceData.GetStringArray(45),
			workTag = SourceData.GetString(46),
			filter = SourceData.GetStringArray(47),
			roomName_JP = SourceData.GetStringArray(48),
			roomName = SourceData.GetStringArray(49),
			detail_JP = SourceData.GetString(50),
			detail = SourceData.GetString(51)
		};
	}

	public override Row CreateRowByMapping(IReadOnlyDictionary<string, int> mapping)
	{
		return new Row
		{
			id = SourceData.GetString(mapping["id"]),
			name_JP = SourceData.GetString(mapping["name_JP"]),
			unknown_JP = SourceData.GetString(mapping["unknown_JP"]),
			unit_JP = SourceData.GetString(mapping["unit_JP"]),
			naming = SourceData.GetString(mapping["naming"]),
			name = SourceData.GetString(mapping["name"]),
			unit = SourceData.GetString(mapping["unit"]),
			unknown = SourceData.GetString(mapping["unknown"]),
			category = SourceData.GetString(mapping["category"]),
			sort = SourceData.GetInt(mapping["sort"]),
			_tileType = SourceData.GetString(mapping["_tileType"]),
			_idRenderData = SourceData.GetString(mapping["_idRenderData"]),
			tiles = SourceData.GetIntArray(mapping["tiles"]),
			altTiles = SourceData.GetIntArray(mapping["altTiles"]),
			anime = SourceData.GetIntArray(mapping["anime"]),
			skins = SourceData.GetIntArray(mapping["skins"]),
			size = SourceData.GetIntArray(mapping["size"]),
			colorMod = SourceData.GetInt(mapping["colorMod"]),
			colorType = SourceData.GetString(mapping["colorType"]),
			recipeKey = SourceData.GetStringArray(mapping["recipeKey"]),
			factory = SourceData.GetStringArray(mapping["factory"]),
			components = SourceData.GetStringArray(mapping["components"]),
			disassemble = SourceData.GetStringArray(mapping["disassemble"]),
			defMat = SourceData.GetString(mapping["defMat"]),
			tierGroup = SourceData.GetString(mapping["tierGroup"]),
			value = SourceData.GetInt(mapping["value"]),
			LV = SourceData.GetInt(mapping["LV"]),
			chance = SourceData.GetInt(mapping["chance"]),
			quality = SourceData.GetInt(mapping["quality"]),
			HP = SourceData.GetInt(mapping["HP"]),
			weight = SourceData.GetInt(mapping["weight"]),
			electricity = SourceData.GetInt(mapping["electricity"]),
			trait = SourceData.GetStringArray(mapping["trait"]),
			elements = Core.ParseElements(SourceData.GetStr(mapping["elements"])),
			range = SourceData.GetInt(mapping["range"]),
			attackType = SourceData.GetString(mapping["attackType"]),
			offense = SourceData.GetIntArray(mapping["offense"]),
			substats = SourceData.GetIntArray(mapping["substats"]),
			defense = SourceData.GetIntArray(mapping["defense"]),
			lightData = SourceData.GetString(mapping["lightData"]),
			idExtra = SourceData.GetString(mapping["idExtra"]),
			idToggleExtra = SourceData.GetString(mapping["idToggleExtra"]),
			idActorEx = SourceData.GetString(mapping["idActorEx"]),
			idSound = SourceData.GetString(mapping["idSound"]),
			tag = SourceData.GetStringArray(mapping["tag"]),
			workTag = SourceData.GetString(mapping["workTag"]),
			filter = SourceData.GetStringArray(mapping["filter"]),
			roomName_JP = SourceData.GetStringArray(mapping["roomName_JP"]),
			roomName = SourceData.GetStringArray(mapping["roomName"]),
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
		SourceAsset._SavePrefs("prefs_auto");
		_rows.Clear();
		foreach (Row row in rows)
		{
			_rows[row.id] = row;
		}
	}

	public override void RestorePref()
	{
	}
}
