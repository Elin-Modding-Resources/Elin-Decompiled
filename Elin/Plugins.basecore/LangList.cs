using System;

public class LangList : SourceDataString<LangList.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public string id;

		public string filter;

		public string[] text_JP;

		public string[] text;

		public string[] text_L;

		public override bool UseAlias => false;

		public override string GetAlias => "n";
	}

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

	public override void SetRow(Row r)
	{
		map[r.id] = r;
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
