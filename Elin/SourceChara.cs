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

		public bool staticSkin;

		public int[] _tiles_snow;

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

		public SourceRace.Row race_row => _race_row ?? (_race_row = base.sources.races.map[race]);

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
			if ((bool)renderData && (bool)renderData.pass && (_tiles_snow == null || _tiles_snow.Length != tiles_snow.Length))
			{
				_tiles_snow = new int[tiles_snow.Length];
				for (int i = 0; i < tiles_snow.Length; i++)
				{
					_tiles_snow[i] = tiles_snow[i] / 100 * (int)renderData.pass.pmesh.tiling.x + tiles_snow[i] % 100;
				}
			}
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
		obj.detail_JP = SourceData.GetString(47);
		obj.detail = SourceData.GetString(48);
		return obj;
	}

	public override void SetRow(Row r)
	{
		map[r.id] = r;
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
}
