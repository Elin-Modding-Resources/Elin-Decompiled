using System;
using System.Collections.Generic;

public class LangList : SourceDataString<LangList.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public string id;

		public string filter;

		public string[] text_JP;

		public string[] text;

		[NonSerialized]
		public string[] text_L;

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

	public override bool AllowHotInitialization => true;

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetString(0),
			filter = SourceData.GetString(1),
			text_JP = SourceData.GetStringArray(2),
			text = SourceData.GetStringArray(3)
		};
	}

	public override Row CreateRowByMapping(IReadOnlyDictionary<string, int> mapping)
	{
		return new Row
		{
			id = SourceData.GetString(mapping["id"]),
			filter = SourceData.GetString(mapping["filter"]),
			text_JP = SourceData.GetStringArray(mapping["text_JP"]),
			text = SourceData.GetStringArray(mapping["text"])
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

	public override string[] GetList(string id)
	{
		Row row = map.TryGetValue(id);
		if (row == null)
		{
			return null;
		}
		if (!Lang.isBuiltin)
		{
			if (row.text_L == null || row.text_L.Length == 0)
			{
				return row.text;
			}
			return row.text_L;
		}
		if (!Lang.isJP)
		{
			return row.text;
		}
		return row.text_JP;
	}
}
