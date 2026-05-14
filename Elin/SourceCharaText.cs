using System;
using System.Collections.Generic;

public class SourceCharaText : SourceDataString<SourceCharaText.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public string id;

		public string calm_JP;

		public string fov_JP;

		public string aggro_JP;

		public string dead_JP;

		public string kill_JP;

		public string calm;

		public string fov;

		public string aggro;

		public string dead;

		public string kill;

		[NonSerialized]
		public string calm_L;

		[NonSerialized]
		public string fov_L;

		[NonSerialized]
		public string aggro_L;

		[NonSerialized]
		public string dead_L;

		[NonSerialized]
		public string kill_L;

		public override bool UseAlias => false;

		public override string GetAlias => "n";
	}

	public static readonly IReadOnlyDictionary<string, int> RowMapping = new Dictionary<string, int>
	{
		["id"] = 0,
		["calm_JP"] = 2,
		["fov_JP"] = 3,
		["aggro_JP"] = 4,
		["dead_JP"] = 5,
		["kill_JP"] = 6,
		["calm"] = 7,
		["fov"] = 8,
		["aggro"] = 9,
		["dead"] = 10,
		["kill"] = 11
	};

	public override string[] ImportFields => new string[5] { "calm", "fov", "aggro", "dead", "kill" };

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetString(0),
			calm_JP = SourceData.GetString(2),
			fov_JP = SourceData.GetString(3),
			aggro_JP = SourceData.GetString(4),
			dead_JP = SourceData.GetString(5),
			kill_JP = SourceData.GetString(6),
			calm = SourceData.GetString(7),
			fov = SourceData.GetString(8),
			aggro = SourceData.GetString(9),
			dead = SourceData.GetString(10),
			kill = SourceData.GetString(11)
		};
	}

	public override Row CreateRowByMapping(IReadOnlyDictionary<string, int> mapping)
	{
		return new Row
		{
			id = SourceData.GetString(mapping["id"]),
			calm_JP = SourceData.GetString(mapping["calm_JP"]),
			fov_JP = SourceData.GetString(mapping["fov_JP"]),
			aggro_JP = SourceData.GetString(mapping["aggro_JP"]),
			dead_JP = SourceData.GetString(mapping["dead_JP"]),
			kill_JP = SourceData.GetString(mapping["kill_JP"]),
			calm = SourceData.GetString(mapping["calm"]),
			fov = SourceData.GetString(mapping["fov"]),
			aggro = SourceData.GetString(mapping["aggro"]),
			dead = SourceData.GetString(mapping["dead"]),
			kill = SourceData.GetString(mapping["kill"])
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
