using System;
using System.Collections.Generic;

public class LangGame : SourceLang<LangGame.Row>
{
	[Serializable]
	public class Row : LangRow
	{
		public string filter;

		public string group;

		public string color;

		public string logColor;

		public string sound;

		public string effect;

		public override bool UseAlias => false;

		public override string GetAlias => "n";
	}

	public static readonly IReadOnlyDictionary<string, int> RowMapping = new Dictionary<string, int>
	{
		["id"] = 0,
		["filter"] = 1,
		["group"] = 2,
		["color"] = 3,
		["logColor"] = 4,
		["sound"] = 5,
		["effect"] = 6,
		["text_JP"] = 7,
		["text"] = 8
	};

	public static readonly IReadOnlyDictionary<string, string> TypeMapping = new Dictionary<string, string>
	{
		["id"] = "string",
		["filter"] = "string",
		["group"] = "string",
		["color"] = "string",
		["logColor"] = "string",
		["sound"] = "string",
		["effect"] = "string",
		["text_JP"] = "string",
		["text"] = "string"
	};

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetString(0),
			filter = SourceData.GetString(1),
			group = SourceData.GetString(2),
			color = SourceData.GetString(3),
			logColor = SourceData.GetString(4),
			sound = SourceData.GetString(5),
			effect = SourceData.GetString(6),
			text_JP = SourceData.GetString(7),
			text = SourceData.GetString(8)
		};
	}

	public override Row CreateRowByMapping(IReadOnlyDictionary<string, int> mapping)
	{
		return new Row
		{
			id = SourceData.GetString(mapping["id"]),
			filter = SourceData.GetString(mapping["filter"]),
			group = SourceData.GetString(mapping["group"]),
			color = SourceData.GetString(mapping["color"]),
			logColor = SourceData.GetString(mapping["logColor"]),
			sound = SourceData.GetString(mapping["sound"]),
			effect = SourceData.GetString(mapping["effect"]),
			text_JP = SourceData.GetString(mapping["text_JP"]),
			text = SourceData.GetString(mapping["text"])
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

	public override IReadOnlyDictionary<string, string> GetTypeMapping()
	{
		return TypeMapping;
	}

	public static bool Has(string id)
	{
		return Lang.Game.map.ContainsKey(id);
	}
}
