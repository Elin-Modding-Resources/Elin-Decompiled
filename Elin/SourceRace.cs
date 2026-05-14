using System;
using System.Collections.Generic;

public class SourceRace : SourceDataString<SourceRace.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public string id;

		public string name_JP;

		public string name;

		public int playable;

		public string[] tag;

		public int life;

		public int mana;

		public int vigor;

		public int DV;

		public int PV;

		public int PDR;

		public int EDR;

		public int EP;

		public int STR;

		public int END;

		public int DEX;

		public int PER;

		public int LER;

		public int WIL;

		public int MAG;

		public int CHA;

		public int SPD;

		public int INT;

		public int martial;

		public int pen;

		public int[] elements;

		public string skill;

		public string figure;

		public int geneCap;

		public string material;

		public string[] corpse;

		public string[] loot;

		public int blood;

		public string meleeStyle;

		public string castStyle;

		public string[] EQ;

		public int sex;

		public int[] age;

		public int height;

		public int breeder;

		public string[] food;

		public string fur;

		public string detail_JP;

		public string detail;

		public Dictionary<int, int> elementMap;

		[NonSerialized]
		public string name_L;

		[NonSerialized]
		public string detail_L;

		public override bool UseAlias => false;

		public override string GetAlias => "n";

		public bool IsAnimal => tag.Contains("animal");

		public bool IsHuman => tag.Contains("human");

		public bool IsUndead => tag.Contains("undead");

		public bool IsMachine => tag.Contains("machine");

		public bool IsHorror => tag.Contains("horror");

		public bool IsFish => tag.Contains("fish");

		public bool IsFairy => tag.Contains("fairy");

		public bool IsGod => tag.Contains("god");

		public bool IsDragon => tag.Contains("dragon");

		public bool IsPlant => tag.Contains("plant");
	}

	public static readonly IReadOnlyDictionary<string, int> RowMapping = new Dictionary<string, int>
	{
		["id"] = 0,
		["name_JP"] = 1,
		["name"] = 2,
		["playable"] = 3,
		["tag"] = 4,
		["life"] = 5,
		["mana"] = 6,
		["vigor"] = 7,
		["DV"] = 8,
		["PV"] = 9,
		["PDR"] = 10,
		["EDR"] = 11,
		["EP"] = 12,
		["STR"] = 13,
		["END"] = 14,
		["DEX"] = 15,
		["PER"] = 16,
		["LER"] = 17,
		["WIL"] = 18,
		["MAG"] = 19,
		["CHA"] = 20,
		["SPD"] = 21,
		["INT"] = 23,
		["martial"] = 24,
		["pen"] = 25,
		["elements"] = 26,
		["skill"] = 27,
		["figure"] = 28,
		["geneCap"] = 29,
		["material"] = 30,
		["corpse"] = 31,
		["loot"] = 32,
		["blood"] = 33,
		["meleeStyle"] = 34,
		["castStyle"] = 35,
		["EQ"] = 36,
		["sex"] = 37,
		["age"] = 38,
		["height"] = 39,
		["breeder"] = 40,
		["food"] = 41,
		["fur"] = 42,
		["detail_JP"] = 43,
		["detail"] = 44
	};

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetString(0),
			name_JP = SourceData.GetString(1),
			name = SourceData.GetString(2),
			playable = SourceData.GetInt(3),
			tag = SourceData.GetStringArray(4),
			life = SourceData.GetInt(5),
			mana = SourceData.GetInt(6),
			vigor = SourceData.GetInt(7),
			DV = SourceData.GetInt(8),
			PV = SourceData.GetInt(9),
			PDR = SourceData.GetInt(10),
			EDR = SourceData.GetInt(11),
			EP = SourceData.GetInt(12),
			STR = SourceData.GetInt(13),
			END = SourceData.GetInt(14),
			DEX = SourceData.GetInt(15),
			PER = SourceData.GetInt(16),
			LER = SourceData.GetInt(17),
			WIL = SourceData.GetInt(18),
			MAG = SourceData.GetInt(19),
			CHA = SourceData.GetInt(20),
			SPD = SourceData.GetInt(21),
			INT = SourceData.GetInt(23),
			martial = SourceData.GetInt(24),
			pen = SourceData.GetInt(25),
			elements = Core.ParseElements(SourceData.GetStr(26)),
			skill = SourceData.GetString(27),
			figure = SourceData.GetString(28),
			geneCap = SourceData.GetInt(29),
			material = SourceData.GetString(30),
			corpse = SourceData.GetStringArray(31),
			loot = SourceData.GetStringArray(32),
			blood = SourceData.GetInt(33),
			meleeStyle = SourceData.GetString(34),
			castStyle = SourceData.GetString(35),
			EQ = SourceData.GetStringArray(36),
			sex = SourceData.GetInt(37),
			age = SourceData.GetIntArray(38),
			height = SourceData.GetInt(39),
			breeder = SourceData.GetInt(40),
			food = SourceData.GetStringArray(41),
			fur = SourceData.GetString(42),
			detail_JP = SourceData.GetString(43),
			detail = SourceData.GetString(44)
		};
	}

	public override Row CreateRowByMapping(IReadOnlyDictionary<string, int> mapping)
	{
		return new Row
		{
			id = SourceData.GetString(mapping["id"]),
			name_JP = SourceData.GetString(mapping["name_JP"]),
			name = SourceData.GetString(mapping["name"]),
			playable = SourceData.GetInt(mapping["playable"]),
			tag = SourceData.GetStringArray(mapping["tag"]),
			life = SourceData.GetInt(mapping["life"]),
			mana = SourceData.GetInt(mapping["mana"]),
			vigor = SourceData.GetInt(mapping["vigor"]),
			DV = SourceData.GetInt(mapping["DV"]),
			PV = SourceData.GetInt(mapping["PV"]),
			PDR = SourceData.GetInt(mapping["PDR"]),
			EDR = SourceData.GetInt(mapping["EDR"]),
			EP = SourceData.GetInt(mapping["EP"]),
			STR = SourceData.GetInt(mapping["STR"]),
			END = SourceData.GetInt(mapping["END"]),
			DEX = SourceData.GetInt(mapping["DEX"]),
			PER = SourceData.GetInt(mapping["PER"]),
			LER = SourceData.GetInt(mapping["LER"]),
			WIL = SourceData.GetInt(mapping["WIL"]),
			MAG = SourceData.GetInt(mapping["MAG"]),
			CHA = SourceData.GetInt(mapping["CHA"]),
			SPD = SourceData.GetInt(mapping["SPD"]),
			INT = SourceData.GetInt(mapping["INT"]),
			martial = SourceData.GetInt(mapping["martial"]),
			pen = SourceData.GetInt(mapping["pen"]),
			elements = Core.ParseElements(SourceData.GetStr(mapping["elements"])),
			skill = SourceData.GetString(mapping["skill"]),
			figure = SourceData.GetString(mapping["figure"]),
			geneCap = SourceData.GetInt(mapping["geneCap"]),
			material = SourceData.GetString(mapping["material"]),
			corpse = SourceData.GetStringArray(mapping["corpse"]),
			loot = SourceData.GetStringArray(mapping["loot"]),
			blood = SourceData.GetInt(mapping["blood"]),
			meleeStyle = SourceData.GetString(mapping["meleeStyle"]),
			castStyle = SourceData.GetString(mapping["castStyle"]),
			EQ = SourceData.GetStringArray(mapping["EQ"]),
			sex = SourceData.GetInt(mapping["sex"]),
			age = SourceData.GetIntArray(mapping["age"]),
			height = SourceData.GetInt(mapping["height"]),
			breeder = SourceData.GetInt(mapping["breeder"]),
			food = SourceData.GetStringArray(mapping["food"]),
			fur = SourceData.GetString(mapping["fur"]),
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

	public override void OnInit()
	{
		foreach (Row row in rows)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			dictionary[70] = row.STR;
			dictionary[71] = row.END;
			dictionary[72] = row.DEX;
			dictionary[73] = row.PER;
			dictionary[74] = row.LER;
			dictionary[75] = row.WIL;
			dictionary[76] = row.MAG;
			dictionary[77] = row.CHA;
			dictionary[79] = row.SPD;
			dictionary[80] = row.INT;
			dictionary[100] = row.martial;
			dictionary[60] = row.life;
			dictionary[61] = row.mana;
			dictionary[62] = row.vigor;
			dictionary[65] = row.PV;
			dictionary[64] = row.DV;
			dictionary[55] = row.PDR;
			dictionary[56] = row.EDR;
			dictionary[57] = row.EP;
			dictionary[261] = 1;
			dictionary[225] = 1;
			dictionary[255] = 1;
			dictionary[220] = 1;
			dictionary[250] = 1;
			dictionary[101] = 1;
			dictionary[102] = 1;
			dictionary[103] = 1;
			dictionary[107] = 1;
			dictionary[106] = 1;
			dictionary[110] = 1;
			dictionary[111] = 1;
			dictionary[104] = 1;
			dictionary[109] = 1;
			dictionary[108] = 1;
			dictionary[123] = 1;
			dictionary[122] = 1;
			dictionary[120] = 1;
			dictionary[150] = 1;
			dictionary[301] = 1;
			dictionary[306] = 1;
			row.elementMap = Element.GetElementMap(row.elements, dictionary);
		}
	}
}
