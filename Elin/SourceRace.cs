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

		public bool visibleWithTelepathy;

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

	public override void SetRow(Row r)
	{
		map[r.id] = r;
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
			row.visibleWithTelepathy = !row.IsUndead && !row.IsMachine && !row.IsHorror;
		}
	}
}
