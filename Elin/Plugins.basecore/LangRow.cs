using System;

public class LangRow : SourceData.BaseRow
{
	public string id;

	public string text_JP;

	public string text;

	[NonSerialized]
	public string text_L;
}
