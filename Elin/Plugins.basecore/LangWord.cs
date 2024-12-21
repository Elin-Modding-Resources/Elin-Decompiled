using System;

public class LangWord : SourceDataInt<LangWord.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public int id;

		public string group;

		public string name_JP;

		public string name;

		public string name_L;

		public override bool UseAlias => false;

		public override string GetAlias => "n";
	}

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

	public override void SetRow(Row r)
	{
		map[r.id] = r;
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
