using System;

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

	public override void SetRow(Row r)
	{
		map[r.id] = r;
	}
}
