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

	public static readonly IReadOnlyDictionary<string, int> RowMapping = new Dictionary<string, int>
	{
		["id"] = 0,
		["alias"] = 1,
		["type"] = 2,
		["name_JP"] = 3,
		["name"] = 4,
		["ai"] = 5,
		["talk"] = 6,
		["area"] = 7,
		["destTrait"] = 8,
		["workTag"] = 9,
		["expedition"] = 10,
		["resources"] = 11,
		["randomRange"] = 12,
		["modifiers"] = 13,
		["tax"] = 14,
		["things"] = 15,
		["elements"] = 16,
		["skill"] = 17,
		["detail_JP"] = 18,
		["detail"] = 19
	};

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

	public override Row CreateRowByMapping(IReadOnlyDictionary<string, int> mapping)
	{
		return new Row
		{
			id = SourceData.GetInt(mapping["id"]),
			alias = SourceData.GetString(mapping["alias"]),
			type = SourceData.GetString(mapping["type"]),
			name_JP = SourceData.GetString(mapping["name_JP"]),
			name = SourceData.GetString(mapping["name"]),
			ai = SourceData.GetString(mapping["ai"]),
			talk = SourceData.GetString(mapping["talk"]),
			area = SourceData.GetString(mapping["area"]),
			destTrait = SourceData.GetString(mapping["destTrait"]),
			workTag = SourceData.GetString(mapping["workTag"]),
			expedition = SourceData.GetString(mapping["expedition"]),
			resources = SourceData.GetIntArray(mapping["resources"]),
			randomRange = SourceData.GetInt(mapping["randomRange"]),
			modifiers = SourceData.GetStringArray(mapping["modifiers"]),
			tax = SourceData.GetInt(mapping["tax"]),
			things = SourceData.GetStringArray(mapping["things"]),
			elements = Core.ParseElements(SourceData.GetStr(mapping["elements"])),
			skill = SourceData.GetString(mapping["skill"]),
			detail_JP = SourceData.GetString(mapping["detail_JP"]),
			detail = SourceData.GetString(mapping["detail"])
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
