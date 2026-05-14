using System;
using System.Collections.Generic;

public class SourceCalc : SourceDataString<SourceCalc.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public string id;

		public string num;

		public string sides;

		public string bonus;

		public override bool UseAlias => false;

		public override string GetAlias => "n";
	}

	public static readonly IReadOnlyDictionary<string, int> RowMapping = new Dictionary<string, int>
	{
		["id"] = 0,
		["num"] = 2,
		["sides"] = 3,
		["bonus"] = 4
	};

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetString(0),
			num = SourceData.GetString(2),
			sides = SourceData.GetString(3),
			bonus = SourceData.GetString(4)
		};
	}

	public override Row CreateRowByMapping(IReadOnlyDictionary<string, int> mapping)
	{
		return new Row
		{
			id = SourceData.GetString(mapping["id"]),
			num = SourceData.GetString(mapping["num"]),
			sides = SourceData.GetString(mapping["sides"]),
			bonus = SourceData.GetString(mapping["bonus"])
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
