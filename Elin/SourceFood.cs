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
			tiles = SourceData.GetIntArray(8),
			parse = SourceData.GetStringArray(9),
			vals = SourceData.GetStringArray(10),
			trait = SourceData.GetStringArray(11),
			elements = Core.ParseElements(SourceData.GetStr(12)),
			idTaste = SourceData.GetString(14),
			TST = SourceData.GetInt(15),
			NUT = SourceData.GetInt(16),
			STR = SourceData.GetInt(17),
			STR2 = SourceData.GetInt(18),
			END = SourceData.GetInt(19),
			END2 = SourceData.GetInt(20),
			DEX = SourceData.GetInt(21),
			DEX2 = SourceData.GetInt(22),
			PER = SourceData.GetInt(23),
			PER2 = SourceData.GetInt(24),
			LER = SourceData.GetInt(25),
			LER2 = SourceData.GetInt(26),
			WIL = SourceData.GetInt(27),
			WIL2 = SourceData.GetInt(28),
			MAG = SourceData.GetInt(29),
			MAG2 = SourceData.GetInt(30),
			CHA = SourceData.GetInt(31),
			CHA2 = SourceData.GetInt(32),
			LV = SourceData.GetInt(34),
			chance = SourceData.GetInt(35),
			value = SourceData.GetInt(36),
			weight = SourceData.GetInt(37),
			recipeKey = SourceData.GetStringArray(38),
			factory = SourceData.GetStringArray(39),
			components = SourceData.GetStringArray(40),
			defMat = SourceData.GetString(41),
			category = SourceData.GetString(42),
			tag = SourceData.GetStringArray(43),
			detail_JP = SourceData.GetString(44),
			detail = SourceData.GetString(45)
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
