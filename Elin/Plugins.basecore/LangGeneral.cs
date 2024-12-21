using System;

public class LangGeneral : SourceLang<LangGeneral.Row>
{
	[Serializable]
	public class Row : LangRow
	{
		public string filter;

		public override bool UseAlias => false;

		public override string GetAlias => "n";
	}

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

	public override void SetRow(Row r)
	{
		map[r.id] = r;
	}
}
