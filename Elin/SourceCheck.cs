using System;
using System.Collections.Generic;

public class SourceCheck : SourceDataString<SourceCheck.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public string id;

		public int element;

		public int targetElement;

		public float subFactor;

		public float targetSubFactor;

		public int baseDC;

		public int critRange;

		public int fumbleRange;

		public int dice;

		public float lvMod;

		public override bool UseAlias => false;

		public override string GetAlias => "n";
	}

	public static readonly IReadOnlyDictionary<string, int> RowMapping = new Dictionary<string, int>
	{
		["id"] = 0,
		["element"] = 1,
		["targetElement"] = 2,
		["subFactor"] = 3,
		["targetSubFactor"] = 4,
		["baseDC"] = 5,
		["critRange"] = 6,
		["fumbleRange"] = 7,
		["dice"] = 8,
		["lvMod"] = 9
	};

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetString(0),
			element = Core.GetElement(SourceData.GetStr(1)),
			targetElement = Core.GetElement(SourceData.GetStr(2)),
			subFactor = SourceData.GetFloat(3),
			targetSubFactor = SourceData.GetFloat(4),
			baseDC = SourceData.GetInt(5),
			critRange = SourceData.GetInt(6),
			fumbleRange = SourceData.GetInt(7),
			dice = SourceData.GetInt(8),
			lvMod = SourceData.GetFloat(9)
		};
	}

	public override Row CreateRowByMapping(IReadOnlyDictionary<string, int> mapping)
	{
		return new Row
		{
			id = SourceData.GetString(mapping["id"]),
			element = Core.GetElement(SourceData.GetStr(mapping["element"])),
			targetElement = Core.GetElement(SourceData.GetStr(mapping["targetElement"])),
			subFactor = SourceData.GetFloat(mapping["subFactor"]),
			targetSubFactor = SourceData.GetFloat(mapping["targetSubFactor"]),
			baseDC = SourceData.GetInt(mapping["baseDC"]),
			critRange = SourceData.GetInt(mapping["critRange"]),
			fumbleRange = SourceData.GetInt(mapping["fumbleRange"]),
			dice = SourceData.GetInt(mapping["dice"]),
			lvMod = SourceData.GetFloat(mapping["lvMod"])
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
