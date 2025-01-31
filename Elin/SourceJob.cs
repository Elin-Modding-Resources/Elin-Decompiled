using System;
using System.Collections.Generic;

public class SourceJob : SourceDataString<SourceJob.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public string id;

		public string name_JP;

		public string name;

		public int playable;

		public int STR;

		public int END;

		public int DEX;

		public int PER;

		public int LER;

		public int WIL;

		public int MAG;

		public int CHA;

		public int SPD;

		public int[] elements;

		public string[] weapon;

		public string equip;

		public int[] domain;

		public string detail_JP;

		public string detail;

		public Dictionary<int, int> elementMap;

		[NonSerialized]
		public string name_L;

		[NonSerialized]
		public string detail_L;

		public override bool UseAlias => false;

		public override string GetAlias => "n";

		public void WriteNote(UINote n)
		{
			n.Clear();
			n.AddHeader(GetName().ToTitleCase());
			n.AddText(GetDetail()).SetWidth(400);
			n.Build();
		}
	}

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetString(0),
			name_JP = SourceData.GetString(1),
			name = SourceData.GetString(2),
			playable = SourceData.GetInt(3),
			STR = SourceData.GetInt(4),
			END = SourceData.GetInt(5),
			DEX = SourceData.GetInt(6),
			PER = SourceData.GetInt(7),
			LER = SourceData.GetInt(8),
			WIL = SourceData.GetInt(9),
			MAG = SourceData.GetInt(10),
			CHA = SourceData.GetInt(11),
			SPD = SourceData.GetInt(12),
			elements = Core.ParseElements(SourceData.GetStr(14)),
			weapon = SourceData.GetStringArray(15),
			equip = SourceData.GetString(16),
			domain = Core.ParseElements(SourceData.GetStr(17)),
			detail_JP = SourceData.GetString(18),
			detail = SourceData.GetString(19)
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
			Dictionary<int, int> dictionary = (row.elementMap = Element.GetElementMap(row.elements));
			dictionary[70] = row.STR;
			dictionary[71] = row.END;
			dictionary[72] = row.DEX;
			dictionary[73] = row.PER;
			dictionary[74] = row.LER;
			dictionary[75] = row.WIL;
			dictionary[76] = row.MAG;
			dictionary[77] = row.CHA;
			dictionary[79] = row.SPD;
		}
	}
}
