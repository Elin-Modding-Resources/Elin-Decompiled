using System;
using System.Collections.Generic;

public class SourceHobby : SourceDataInt<SourceHobby.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public int id;

		public string alias;

		public string type;

		public string name_JP;

		public string name;

		public string ai;

		public string talk;

		public string area;

		public string destTrait;

		public string workTag;

		public string expedition;

		public int[] resources;

		public int randomRange;

		public string[] modifiers;

		public int tax;

		public string[] things;

		public int[] elements;

		public string skill;

		public string detail_JP;

		public string detail;

		[NonSerialized]
		public string name_L;

		[NonSerialized]
		public string detail_L;

		public override bool UseAlias => true;

		public override string GetAlias => alias;
	}

	[NonSerialized]
	public List<Row> listHobbies = new List<Row>();

	[NonSerialized]
	public List<Row> listWorks = new List<Row>();

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetInt(0),
			alias = SourceData.GetString(1),
			type = SourceData.GetString(2),
			name_JP = SourceData.GetString(3),
			name = SourceData.GetString(4),
			ai = SourceData.GetString(5),
			talk = SourceData.GetString(6),
			area = SourceData.GetString(7),
			destTrait = SourceData.GetString(8),
			workTag = SourceData.GetString(9),
			expedition = SourceData.GetString(10),
			resources = SourceData.GetIntArray(11),
			randomRange = SourceData.GetInt(12),
			modifiers = SourceData.GetStringArray(13),
			tax = SourceData.GetInt(14),
			things = SourceData.GetStringArray(15),
			elements = Core.ParseElements(SourceData.GetStr(16)),
			skill = SourceData.GetString(17),
			detail_JP = SourceData.GetString(18),
			detail = SourceData.GetString(19)
		};
	}

	public override void SetRow(Row r)
	{
		map[r.id] = r;
	}

	public override void OnInit()
	{
		foreach (Row row in rows)
		{
			switch (row.type)
			{
			case "Hobby":
				listHobbies.Add(row);
				break;
			case "Work":
				listWorks.Add(row);
				break;
			case "Both":
				listHobbies.Add(row);
				listWorks.Add(row);
				break;
			}
		}
	}
}
