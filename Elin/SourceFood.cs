using System;
using System.Collections.Generic;

public class SourceFood : SourceThingV
{
	[Serializable]
	public class Row2 : Row
	{
		public string idTaste;

		public int TST;

		public int NUT;

		public int STR;

		public int STR2;

		public int END;

		public int END2;

		public int DEX;

		public int DEX2;

		public int PER;

		public int PER2;

		public int LER;

		public int LER2;

		public int WIL;

		public int WIL2;

		public int MAG;

		public int MAG2;

		public int CHA;

		public int CHA2;

		public override bool UseAlias => false;

		public override string GetAlias => "n";
	}

	public override Row CreateRow()
	{
		return new Row2
		{
			id = SourceData.GetString(0),
			_origin = SourceData.GetString(1),
			name_JP = SourceData.GetString(2),
			unit_JP = SourceData.GetString(3),
			name = SourceData.GetString(4),
			unit = SourceData.GetString(5),
			name2_JP = SourceData.GetStringArray(6),
			name2 = SourceData.GetStringArray(7),
			unknown_JP = SourceData.GetString(8),
			unknown = SourceData.GetString(9),
			tiles = SourceData.GetIntArray(10),
			parse = SourceData.GetStringArray(11),
			vals = SourceData.GetStringArray(12),
			trait = SourceData.GetStringArray(13),
			elements = Core.ParseElements(SourceData.GetStr(14)),
			idTaste = SourceData.GetString(16),
			TST = SourceData.GetInt(17),
			NUT = SourceData.GetInt(18),
			STR = SourceData.GetInt(19),
			STR2 = SourceData.GetInt(20),
			END = SourceData.GetInt(21),
			END2 = SourceData.GetInt(22),
			DEX = SourceData.GetInt(23),
			DEX2 = SourceData.GetInt(24),
			PER = SourceData.GetInt(25),
			PER2 = SourceData.GetInt(26),
			LER = SourceData.GetInt(27),
			LER2 = SourceData.GetInt(28),
			WIL = SourceData.GetInt(29),
			WIL2 = SourceData.GetInt(30),
			MAG = SourceData.GetInt(31),
			MAG2 = SourceData.GetInt(32),
			CHA = SourceData.GetInt(33),
			CHA2 = SourceData.GetInt(34),
			LV = SourceData.GetInt(36),
			chance = SourceData.GetInt(37),
			value = SourceData.GetInt(38),
			weight = SourceData.GetInt(39),
			recipeKey = SourceData.GetStringArray(40),
			factory = SourceData.GetStringArray(41),
			components = SourceData.GetStringArray(42),
			defMat = SourceData.GetString(43),
			category = SourceData.GetString(44),
			tag = SourceData.GetStringArray(45),
			detail_JP = SourceData.GetString(46),
			detail = SourceData.GetString(47)
		};
	}

	public override void SetRow(Row r)
	{
		map[r.id] = r;
	}

	public override void Reset()
	{
		base.Reset();
		EClass.sources.things.Reset();
	}

	public override void OnImportRow(Row _r, SourceThing.Row c)
	{
		List<int> list = new List<int>(c.elements);
		Row2 row = _r as Row2;
		Add(10, row.NUT);
		Parse(row.STR, 70, row.STR2, 440);
		Parse(row.END, 71, row.END2, 441);
		Parse(row.DEX, 72, row.DEX2, 442);
		Parse(row.PER, 73, row.PER2, 443);
		Parse(row.LER, 74, row.LER2, 444);
		Parse(row.WIL, 75, row.WIL2, 445);
		Parse(row.MAG, 76, row.MAG2, 446);
		Parse(row.CHA, 77, row.CHA2, 447);
		for (int i = 0; i < row.elements.Length; i += 2)
		{
			Add(_r.elements[i], row.elements[i + 1]);
		}
		c.elements = list.ToArray();
		c.name2 = row.name2;
		c.name2_JP = row.name2_JP;
		c.unknown = row.unknown;
		c.unknown_JP = row.unknown_JP;
		if (!row.unit_JP.IsEmpty())
		{
			c.unit_JP = row.unit_JP;
		}
		void Add(int ele, int a)
		{
			list.Add(ele);
			list.Add(a);
		}
		void Parse(int raw, int ele, int raw2, int ele2)
		{
			if (raw != 0)
			{
				Add(ele, raw);
			}
			if (raw2 != 0)
			{
				Add(ele2, raw2);
			}
		}
	}
}
