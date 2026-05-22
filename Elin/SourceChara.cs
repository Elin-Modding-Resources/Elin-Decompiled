using System;
using System.Collections.Generic;

public class SourceChara : SourceDataString<SourceChara.Row>
{
	[Serializable]
	public class Row : CardRow
	{
		public int _id;

		public string aka_JP;

		public string aka;

		public int[] tiles_snow;

		public string hostility;

		public string biome;

		public string race;

		public string job;

		public string tactics;

		public string aiIdle;

		public int[] aiParam;

		public string[] actCombat;

		public string[] mainElement;

		public string equip;

		public string[] gachaFilter;

		public string tone;

		public string[] actIdle;

		public string bio;

		public string faith;

		public string[] works;

		public string[] hobbies;

		public string idText;

		public string moveAnime;

		public string[] recruitItems;

		public bool staticSkin;

		public int[] _tiles_snow;

		public int skinAntiSpider;

		[NonSerialized]
		public SourceRace.Row _race_row;

		[NonSerialized]
		public string name_L;

		[NonSerialized]
		public string detail_L;

		[NonSerialized]
		public string aka_L;

		public override bool UseAlias => false;

		public override string GetAlias => "n";

		public SourceRace.Row race_row
		{
			get
			{
				SourceRace.Row row = _race_row;
				if (row == null)
				{
					SourceRace.Row obj = base.sources.races.map.TryGetValue(race) ?? base.sources.races.map["norland"];
					SourceRace.Row row2 = obj;
					_race_row = obj;
					row = row2;
				}
				return row;
			}
		}

		public override string RecipeID => id;

		public override string GetSearchName(bool jp)
		{
			object obj;
			if (!jp)
			{
				obj = _nameSearch;
				if (obj == null)
				{
					return _nameSearch = (name.StartsWith("*") ? aka : name).ToLower();
				}
			}
			else
			{
				obj = _nameSearchJP ?? (_nameSearchJP = (name_JP.StartsWith("*") ? GetText("aka") : GetText()).ToLower());
			}
			return (string)obj;
		}

		public override void SetTiles()
		{
			base.SetTiles();
			staticSkin = HasTag(CTAG.staticSkin);
			if (!renderData || !renderData.pass)
			{
				return;
			}
			if (_tiles_snow == null || _tiles_snow.Length != tiles_snow.Length)
			{
				_tiles_snow = new int[tiles_snow.Length];
				for (int i = 0; i < tiles_snow.Length; i++)
				{
					_tiles_snow[i] = tiles_snow[i] / 100 * (int)renderData.pass.pmesh.tiling.x + tiles_snow[i] % 100;
				}
			}
			skinAntiSpider = ((staticSkin && HasTag(CTAG.antiSpider)) ? 1 : 0);
		}

		public override string GetName()
		{
			string text = GetText();
			text = text.Replace("#ele5", "").Replace("#ele4", "").Replace("#ele3", "")
				.Replace("#ele2", "")
				.Replace("#ele", "");
			if (text == "*r")
			{
				text = GetText("aka");
			}
			if (text[0] == ' ')
			{
				text = text.TrimStart(' ');
			}
			return text.Replace("  ", " ");
		}
	}

	public static readonly IReadOnlyDictionary<string, int> RowMapping;

	public static readonly IReadOnlyDictionary<string, string> TypeMapping;

	public Dictionary<string, Row> _rows = new Dictionary<string, Row>();

	public static Row rowDefaultPCC;

	public override string[] ImportFields => new string[1] { "aka" };

	public override Row CreateRow()
	{
		Row obj = new Row();
		obj.id = SourceData.GetString(0);
		obj._id = SourceData.GetInt(1);
		obj.name_JP = SourceData.GetString(2);
		obj.name = SourceData.GetString(3);
		obj.aka_JP = SourceData.GetString(4);
		obj.aka = SourceData.GetString(5);
		obj.idActor = SourceData.GetStringArray(6);
		obj.sort = SourceData.GetInt(7);
		obj.size = SourceData.GetIntArray(8);
		obj._idRenderData = SourceData.GetString(9);
		obj.tiles = SourceData.GetIntArray(10);
		obj.tiles_snow = SourceData.GetIntArray(11);
		obj.colorMod = SourceData.GetInt(12);
		obj.components = SourceData.GetStringArray(13);
		obj.defMat = SourceData.GetString(14);
		obj.LV = SourceData.GetInt(15);
		obj.chance = SourceData.GetInt(16);
		obj.quality = SourceData.GetInt(17);
		obj.hostility = SourceData.GetString(18);
		obj.biome = SourceData.GetString(19);
		obj.tag = SourceData.GetStringArray(20);
		obj.trait = SourceData.GetStringArray(21);
		obj.race = SourceData.GetString(22);
		obj.job = SourceData.GetString(23);
		obj.tactics = SourceData.GetString(24);
		obj.aiIdle = SourceData.GetString(25);
		obj.aiParam = SourceData.GetIntArray(26);
		obj.actCombat = SourceData.GetStringArray(27);
		obj.mainElement = SourceData.GetStringArray(28);
		obj.elements = Core.ParseElements(SourceData.GetStr(29));
		obj.equip = SourceData.GetString(30);
		obj.loot = SourceData.GetStringArray(31);
		obj.category = SourceData.GetString(32);
		obj.filter = SourceData.GetStringArray(33);
		obj.gachaFilter = SourceData.GetStringArray(34);
		obj.tone = SourceData.GetString(35);
		obj.actIdle = SourceData.GetStringArray(36);
		obj.lightData = SourceData.GetString(37);
		obj.idExtra = SourceData.GetString(38);
		obj.bio = SourceData.GetString(39);
		obj.faith = SourceData.GetString(40);
		obj.works = SourceData.GetStringArray(41);
		obj.hobbies = SourceData.GetStringArray(42);
		obj.idText = SourceData.GetString(43);
		obj.moveAnime = SourceData.GetString(44);
		obj.factory = SourceData.GetStringArray(45);
		obj.components = SourceData.GetStringArray(46);
		obj.recruitItems = SourceData.GetStringArray(47);
		obj.detail_JP = SourceData.GetString(48);
		obj.detail = SourceData.GetString(49);
		return obj;
	}

	public override Row CreateRowByMapping(IReadOnlyDictionary<string, int> mapping)
	{
		Row obj = new Row();
		obj.id = SourceData.GetString(mapping["id"]);
		obj._id = SourceData.GetInt(mapping["_id"]);
		obj.name_JP = SourceData.GetString(mapping["name_JP"]);
		obj.name = SourceData.GetString(mapping["name"]);
		obj.aka_JP = SourceData.GetString(mapping["aka_JP"]);
		obj.aka = SourceData.GetString(mapping["aka"]);
		obj.idActor = SourceData.GetStringArray(mapping["idActor"]);
		obj.sort = SourceData.GetInt(mapping["sort"]);
		obj.size = SourceData.GetIntArray(mapping["size"]);
		obj._idRenderData = SourceData.GetString(mapping["_idRenderData"]);
		obj.tiles = SourceData.GetIntArray(mapping["tiles"]);
		obj.tiles_snow = SourceData.GetIntArray(mapping["tiles_snow"]);
		obj.colorMod = SourceData.GetInt(mapping["colorMod"]);
		obj.components = SourceData.GetStringArray(mapping["components"]);
		obj.defMat = SourceData.GetString(mapping["defMat"]);
		obj.LV = SourceData.GetInt(mapping["LV"]);
		obj.chance = SourceData.GetInt(mapping["chance"]);
		obj.quality = SourceData.GetInt(mapping["quality"]);
		obj.hostility = SourceData.GetString(mapping["hostility"]);
		obj.biome = SourceData.GetString(mapping["biome"]);
		obj.tag = SourceData.GetStringArray(mapping["tag"]);
		obj.trait = SourceData.GetStringArray(mapping["trait"]);
		obj.race = SourceData.GetString(mapping["race"]);
		obj.job = SourceData.GetString(mapping["job"]);
		obj.tactics = SourceData.GetString(mapping["tactics"]);
		obj.aiIdle = SourceData.GetString(mapping["aiIdle"]);
		obj.aiParam = SourceData.GetIntArray(mapping["aiParam"]);
		obj.actCombat = SourceData.GetStringArray(mapping["actCombat"]);
		obj.mainElement = SourceData.GetStringArray(mapping["mainElement"]);
		obj.elements = Core.ParseElements(SourceData.GetStr(mapping["elements"]));
		obj.equip = SourceData.GetString(mapping["equip"]);
		obj.loot = SourceData.GetStringArray(mapping["loot"]);
		obj.category = SourceData.GetString(mapping["category"]);
		obj.filter = SourceData.GetStringArray(mapping["filter"]);
		obj.gachaFilter = SourceData.GetStringArray(mapping["gachaFilter"]);
		obj.tone = SourceData.GetString(mapping["tone"]);
		obj.actIdle = SourceData.GetStringArray(mapping["actIdle"]);
		obj.lightData = SourceData.GetString(mapping["lightData"]);
		obj.idExtra = SourceData.GetString(mapping["idExtra"]);
		obj.bio = SourceData.GetString(mapping["bio"]);
		obj.faith = SourceData.GetString(mapping["faith"]);
		obj.works = SourceData.GetStringArray(mapping["works"]);
		obj.hobbies = SourceData.GetStringArray(mapping["hobbies"]);
		obj.idText = SourceData.GetString(mapping["idText"]);
		obj.moveAnime = SourceData.GetString(mapping["moveAnime"]);
		obj.factory = SourceData.GetStringArray(mapping["factory"]);
		obj.components = SourceData.GetStringArray(mapping["components"]);
		obj.recruitItems = SourceData.GetStringArray(mapping["recruitItems"]);
		obj.detail_JP = SourceData.GetString(mapping["detail_JP"]);
		obj.detail = SourceData.GetString(mapping["detail"]);
		return obj;
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

	static SourceChara()
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		dictionary["id"] = 0;
		dictionary["_id"] = 1;
		dictionary["name_JP"] = 2;
		dictionary["name"] = 3;
		dictionary["aka_JP"] = 4;
		dictionary["aka"] = 5;
		dictionary["idActor"] = 6;
		dictionary["sort"] = 7;
		dictionary["size"] = 8;
		dictionary["_idRenderData"] = 9;
		dictionary["tiles"] = 10;
		dictionary["tiles_snow"] = 11;
		dictionary["colorMod"] = 12;
		dictionary["components"] = 13;
		dictionary["defMat"] = 14;
		dictionary["LV"] = 15;
		dictionary["chance"] = 16;
		dictionary["quality"] = 17;
		dictionary["hostility"] = 18;
		dictionary["biome"] = 19;
		dictionary["tag"] = 20;
		dictionary["trait"] = 21;
		dictionary["race"] = 22;
		dictionary["job"] = 23;
		dictionary["tactics"] = 24;
		dictionary["aiIdle"] = 25;
		dictionary["aiParam"] = 26;
		dictionary["actCombat"] = 27;
		dictionary["mainElement"] = 28;
		dictionary["elements"] = 29;
		dictionary["equip"] = 30;
		dictionary["loot"] = 31;
		dictionary["category"] = 32;
		dictionary["filter"] = 33;
		dictionary["gachaFilter"] = 34;
		dictionary["tone"] = 35;
		dictionary["actIdle"] = 36;
		dictionary["lightData"] = 37;
		dictionary["idExtra"] = 38;
		dictionary["bio"] = 39;
		dictionary["faith"] = 40;
		dictionary["works"] = 41;
		dictionary["hobbies"] = 42;
		dictionary["idText"] = 43;
		dictionary["moveAnime"] = 44;
		dictionary["factory"] = 45;
		dictionary["components"] = 46;
		dictionary["recruitItems"] = 47;
		dictionary["detail_JP"] = 48;
		dictionary["detail"] = 49;
		RowMapping = dictionary;
		Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
		dictionary2["id"] = "string";
		dictionary2["_id"] = "int";
		dictionary2["name_JP"] = "string";
		dictionary2["name"] = "string";
		dictionary2["aka_JP"] = "string";
		dictionary2["aka"] = "string";
		dictionary2["idActor"] = "string[]";
		dictionary2["sort"] = "int";
		dictionary2["size"] = "int[]";
		dictionary2["_idRenderData"] = "string";
		dictionary2["tiles"] = "int[]";
		dictionary2["tiles_snow"] = "int[]";
		dictionary2["colorMod"] = "int";
		dictionary2["components"] = "string[]";
		dictionary2["defMat"] = "string";
		dictionary2["LV"] = "int";
		dictionary2["chance"] = "int";
		dictionary2["quality"] = "int";
		dictionary2["hostility"] = "string";
		dictionary2["biome"] = "string";
		dictionary2["tag"] = "string[]";
		dictionary2["trait"] = "string[]";
		dictionary2["race"] = "string";
		dictionary2["job"] = "string";
		dictionary2["tactics"] = "string";
		dictionary2["aiIdle"] = "string";
		dictionary2["aiParam"] = "int[]";
		dictionary2["actCombat"] = "string[]";
		dictionary2["mainElement"] = "string[]";
		dictionary2["elements"] = "elements";
		dictionary2["equip"] = "string";
		dictionary2["loot"] = "string[]";
		dictionary2["category"] = "string";
		dictionary2["filter"] = "string[]";
		dictionary2["gachaFilter"] = "string[]";
		dictionary2["tone"] = "string";
		dictionary2["actIdle"] = "string[]";
		dictionary2["lightData"] = "string";
		dictionary2["idExtra"] = "string";
		dictionary2["bio"] = "string";
		dictionary2["faith"] = "string";
		dictionary2["works"] = "string[]";
		dictionary2["hobbies"] = "string[]";
		dictionary2["idText"] = "string";
		dictionary2["moveAnime"] = "string";
		dictionary2["factory"] = "string[]";
		dictionary2["components"] = "string[]";
		dictionary2["recruitItems"] = "string[]";
		dictionary2["detail_JP"] = "string";
		dictionary2["detail"] = "string";
		TypeMapping = dictionary2;
	}
}
