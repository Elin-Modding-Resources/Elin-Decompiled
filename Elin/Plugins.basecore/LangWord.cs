using System;
using System.Collections.Generic;

public class LangWord : SourceDataInt<LangWord.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public int id;

		public string group;

		public string name_JP;

		public string name;

		[NonSerialized]
		public string name_L;

		public override bool UseAlias => false;

		public override string GetAlias => "n";
	}

	public static readonly IReadOnlyDictionary<string, int> RowMapping = new Dictionary<string, int>
	{
		["id"] = 0,
		["group"] = 1,
		["name_JP"] = 2,
		["name"] = 3
	};

	public static readonly IReadOnlyDictionary<string, string> TypeMapping = new Dictionary<string, string>
	{
		["id"] = "int",
		["group"] = "string",
		["name_JP"] = "string",
		["name"] = "string"
	};

	public override bool AllowHotInitialization => true;

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetInt(0),
			group = SourceData.GetString(1),
			name_JP = SourceData.GetString(2),
			name = SourceData.GetString(3)
		};
	}

	public override Row CreateRowByMapping(IReadOnlyDictionary<string, int> mapping)
	{
		return new Row
		{
			id = SourceData.GetInt(mapping["id"]),
			group = SourceData.GetString(mapping["group"]),
			name_JP = SourceData.GetString(mapping["name_JP"]),
			name = SourceData.GetString(mapping["name"])
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

	public override void OnAfterImportData()
	{
		int num = 0;
		foreach (Row row in rows)
		{
			if (row.id != 0)
			{
				num = row.id;
			}
			row.id = num;
			num++;
		}
	}
}
