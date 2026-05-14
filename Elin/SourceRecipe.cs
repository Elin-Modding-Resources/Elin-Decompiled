using System;
using System.Collections.Generic;

public class SourceRecipe : SourceDataInt<SourceRecipe.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public int id;

		public string factory;

		public string type;

		public string thing;

		public string num;

		public int sp;

		public int time;

		public string[] ing1;

		public string[] ing2;

		public string[] ing3;

		public string[] tag;

		[NonSerialized]
		public string name_L;

		[NonSerialized]
		public string detail_L;

		public override bool UseAlias => false;

		public override string GetAlias => "n";
	}

	public static readonly IReadOnlyDictionary<string, int> RowMapping = new Dictionary<string, int>
	{
		["id"] = 0,
		["factory"] = 1,
		["type"] = 2,
		["thing"] = 3,
		["num"] = 4,
		["sp"] = 5,
		["time"] = 6,
		["ing1"] = 7,
		["ing2"] = 8,
		["ing3"] = 9,
		["tag"] = 10
	};

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetInt(0),
			factory = SourceData.GetString(1),
			type = SourceData.GetString(2),
			thing = SourceData.GetString(3),
			num = SourceData.GetString(4),
			sp = SourceData.GetInt(5),
			time = SourceData.GetInt(6),
			ing1 = SourceData.GetStringArray(7),
			ing2 = SourceData.GetStringArray(8),
			ing3 = SourceData.GetStringArray(9),
			tag = SourceData.GetStringArray(10)
		};
	}

	public override Row CreateRowByMapping(IReadOnlyDictionary<string, int> mapping)
	{
		return new Row
		{
			id = SourceData.GetInt(mapping["id"]),
			factory = SourceData.GetString(mapping["factory"]),
			type = SourceData.GetString(mapping["type"]),
			thing = SourceData.GetString(mapping["thing"]),
			num = SourceData.GetString(mapping["num"]),
			sp = SourceData.GetInt(mapping["sp"]),
			time = SourceData.GetInt(mapping["time"]),
			ing1 = SourceData.GetStringArray(mapping["ing1"]),
			ing2 = SourceData.GetStringArray(mapping["ing2"]),
			ing3 = SourceData.GetStringArray(mapping["ing3"]),
			tag = SourceData.GetStringArray(mapping["tag"])
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
}
