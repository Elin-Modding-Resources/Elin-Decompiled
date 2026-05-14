using System;
using System.Collections.Generic;

public class LangNote : SourceDataString<LangNote.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public string id;

		public string text_JP;

		public string text;

		[NonSerialized]
		public string text_L;

		public override bool UseAlias => false;

		public override string GetAlias => "n";
	}

	public static readonly IReadOnlyDictionary<string, int> RowMapping = new Dictionary<string, int>
	{
		["id"] = 0,
		["text_JP"] = 1,
		["text"] = 2
	};

	public override bool AllowHotInitialization => true;

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetString(0),
			text_JP = SourceData.GetString(1),
			text = SourceData.GetString(2)
		};
	}

	public override Row CreateRowByMapping(IReadOnlyDictionary<string, int> mapping)
	{
		return new Row
		{
			id = SourceData.GetString(mapping["id"]),
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
