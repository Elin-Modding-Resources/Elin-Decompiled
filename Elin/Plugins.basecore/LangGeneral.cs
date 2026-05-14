using System;
using System.Collections.Generic;

public class LangGeneral : SourceLang<LangGeneral.Row>
{
	[Serializable]
	public class Row : LangRow
	{
		public string filter;

		public override bool UseAlias => false;

		public override string GetAlias => "n";
	}

	public static readonly IReadOnlyDictionary<string, int> RowMapping = new Dictionary<string, int>
	{
		["id"] = 0,
		["filter"] = 1,
		["text_JP"] = 2,
		["text"] = 3
	};

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetString(0),
			filter = SourceData.GetString(1),
			text_JP = SourceData.GetString(2),
			text = SourceData.GetString(3)
		};
	}

	public override Row CreateRowByMapping(IReadOnlyDictionary<string, int> mapping)
	{
		return new Row
		{
			id = SourceData.GetString(mapping["id"]),
			filter = SourceData.GetString(mapping["filter"]),
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
}
