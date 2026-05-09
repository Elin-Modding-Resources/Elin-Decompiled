using System;
using System.Collections.Generic;

public class SourceStat : SourceDataInt<SourceStat.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public int id;

		public string alias;

		public string name_JP;

		public string name;

		public string type;

		public string group;

		public string curse;

		public string duration;

		public int durationMax;

		public int hexPower;

		public string[] negate;

		public string[] defenseAttb;

		public string[] resistance;

		public int gainRes;

		public string[] elements;

		public string[] nullify;

		public string[] tag;

		public int[] phase;

		public string colors;

		public string element;

		public string[] effect;

		public string[] strPhase_JP;

		public string[] strPhase;

		public string textPhase_JP;

		public string textPhase;

		public string textEnd_JP;

		public string textEnd;

		public string textPhase2_JP;

		public string textPhase2;

		public string gradient;

		public bool invert;

		public string detail_JP;

		public string detail;

		[NonSerialized]
		public string name_L;

		[NonSerialized]
		public string detail_L;

		[NonSerialized]
		public string textPhase_L;

		[NonSerialized]
		public string textPhase2_L;

		[NonSerialized]
		public string textEnd_L;

		[NonSerialized]
		public string[] strPhase_L;

		public override bool UseAlias => true;

		public override string GetAlias => alias;
	}

	[NonSerialized]
	public Dictionary<string, List<Row>> groups = new Dictionary<string, List<Row>>();

	public override string[] ImportFields => new string[4] { "strPhase", "textPhase", "textPhase2", "textEnd" };

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetInt(0),
			alias = SourceData.GetString(1),
			name_JP = SourceData.GetString(2),
			name = SourceData.GetString(3),
			type = SourceData.GetString(4),
			group = SourceData.GetString(5),
			curse = SourceData.GetString(6),
			duration = SourceData.GetString(7),
			durationMax = SourceData.GetInt(8),
			hexPower = SourceData.GetInt(9),
			negate = SourceData.GetStringArray(10),
			defenseAttb = SourceData.GetStringArray(11),
			resistance = SourceData.GetStringArray(12),
			gainRes = SourceData.GetInt(13),
			elements = SourceData.GetStringArray(14),
			nullify = SourceData.GetStringArray(15),
			tag = SourceData.GetStringArray(16),
			phase = SourceData.GetIntArray(17),
			colors = SourceData.GetString(18),
			element = SourceData.GetString(19),
			effect = SourceData.GetStringArray(20),
			strPhase_JP = SourceData.GetStringArray(21),
			strPhase = SourceData.GetStringArray(22),
			textPhase_JP = SourceData.GetString(23),
			textPhase = SourceData.GetString(24),
			textEnd_JP = SourceData.GetString(25),
			textEnd = SourceData.GetString(26),
			textPhase2_JP = SourceData.GetString(27),
			textPhase2 = SourceData.GetString(28),
			gradient = SourceData.GetString(29),
			invert = SourceData.GetBool(30),
			detail_JP = SourceData.GetString(31),
			detail = SourceData.GetString(32)
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
			if (!row.group.IsEmpty())
			{
				groups.GetOrCreate(row.group).Add(row);
			}
		}
	}
}
