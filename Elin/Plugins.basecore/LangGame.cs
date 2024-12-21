using System;

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

	public override void SetRow(Row r)
	{
		map[r.id] = r;
	}

	public static bool Has(string id)
	{
		return Lang.Game.map.ContainsKey(id);
	}
}
