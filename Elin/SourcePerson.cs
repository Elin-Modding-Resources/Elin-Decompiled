using System;
using System.Collections.Generic;

public class SourcePerson : SourceDataString<SourcePerson.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public string id;

		public string idActor;

		public string name_JP;

		public string name;

		public string aka_JP;

		public string aka;

		public string portrait;

		public string faction;

		public int LV;

		public string job;

		public string race;

		public string material;

		public string bio;

		public string detail_JP;

		public string detail;

		[NonSerialized]
		public string name_L;

		[NonSerialized]
		public string detail_L;

		[NonSerialized]
		public string aka_L;

		public override bool UseAlias => false;

		public override string GetAlias => "n";
	}

	public static readonly IReadOnlyDictionary<string, int> RowMapping = new Dictionary<string, int>
	{
		["id"] = 0,
		["idActor"] = 1,
		["name_JP"] = 2,
		["name"] = 3,
		["aka_JP"] = 4,
		["aka"] = 5,
		["portrait"] = 6,
		["faction"] = 7,
		["LV"] = 8,
		["job"] = 9,
		["race"] = 10,
		["material"] = 11,
		["bio"] = 12,
		["detail_JP"] = 13,
		["detail"] = 14
	};

	public override string[] ImportFields => new string[1] { "aka" };

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetString(0),
			idActor = SourceData.GetString(1),
			name_JP = SourceData.GetString(2),
			name = SourceData.GetString(3),
			aka_JP = SourceData.GetString(4),
			aka = SourceData.GetString(5),
			portrait = SourceData.GetString(6),
			faction = SourceData.GetString(7),
			LV = SourceData.GetInt(8),
			job = SourceData.GetString(9),
			race = SourceData.GetString(10),
			material = SourceData.GetString(11),
			bio = SourceData.GetString(12),
			detail_JP = SourceData.GetString(13),
			detail = SourceData.GetString(14)
		};
	}

	public override Row CreateRowByMapping(IReadOnlyDictionary<string, int> mapping)
	{
		return new Row
		{
			id = SourceData.GetString(mapping["id"]),
			idActor = SourceData.GetString(mapping["idActor"]),
			name_JP = SourceData.GetString(mapping["name_JP"]),
			name = SourceData.GetString(mapping["name"]),
			aka_JP = SourceData.GetString(mapping["aka_JP"]),
			aka = SourceData.GetString(mapping["aka"]),
			portrait = SourceData.GetString(mapping["portrait"]),
			faction = SourceData.GetString(mapping["faction"]),
			LV = SourceData.GetInt(mapping["LV"]),
			job = SourceData.GetString(mapping["job"]),
			race = SourceData.GetString(mapping["race"]),
			material = SourceData.GetString(mapping["material"]),
			bio = SourceData.GetString(mapping["bio"]),
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
}
