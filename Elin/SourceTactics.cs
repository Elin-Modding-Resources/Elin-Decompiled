using System;
using System.Collections.Generic;

public class SourceTactics : SourceDataString<SourceTactics.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public string id;

		public string name_JP;

		public string name;

		public int dist;

		public int move;

		public int movePC;

		public int party;

		public int taunt;

		public int melee;

		public int range;

		public int spell;

		public int heal;

		public int summon;

		public int buff;

		public int debuff;

		public string[] tag;

		public string detail_JP;

		public string detail;

		[NonSerialized]
		public string name_L;

		[NonSerialized]
		public string detail_L;

		public override bool UseAlias => false;

		public override string GetAlias => "n";
	}

	public static readonly IReadOnlyDictionary<string, int> RowMapping = new Dictionary<string, int>
	{
		["id"] = 0,
		["name_JP"] = 1,
		["name"] = 2,
		["dist"] = 4,
		["move"] = 5,
		["movePC"] = 6,
		["party"] = 7,
		["taunt"] = 8,
		["melee"] = 9,
		["range"] = 10,
		["spell"] = 11,
		["heal"] = 12,
		["summon"] = 13,
		["buff"] = 14,
		["debuff"] = 15,
		["tag"] = 16,
		["detail_JP"] = 17,
		["detail"] = 18
	};

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetString(0),
			name_JP = SourceData.GetString(1),
			name = SourceData.GetString(2),
			dist = SourceData.GetInt(4),
			move = SourceData.GetInt(5),
			movePC = SourceData.GetInt(6),
			party = SourceData.GetInt(7),
			taunt = SourceData.GetInt(8),
			melee = SourceData.GetInt(9),
			range = SourceData.GetInt(10),
			spell = SourceData.GetInt(11),
			heal = SourceData.GetInt(12),
			summon = SourceData.GetInt(13),
			buff = SourceData.GetInt(14),
			debuff = SourceData.GetInt(15),
			tag = SourceData.GetStringArray(16),
			detail_JP = SourceData.GetString(17),
			detail = SourceData.GetString(18)
		};
	}

	public override Row CreateRowByMapping(IReadOnlyDictionary<string, int> mapping)
	{
		return new Row
		{
			id = SourceData.GetString(mapping["id"]),
			name_JP = SourceData.GetString(mapping["name_JP"]),
			name = SourceData.GetString(mapping["name"]),
			dist = SourceData.GetInt(mapping["dist"]),
			move = SourceData.GetInt(mapping["move"]),
			movePC = SourceData.GetInt(mapping["movePC"]),
			party = SourceData.GetInt(mapping["party"]),
			taunt = SourceData.GetInt(mapping["taunt"]),
			melee = SourceData.GetInt(mapping["melee"]),
			range = SourceData.GetInt(mapping["range"]),
			spell = SourceData.GetInt(mapping["spell"]),
			heal = SourceData.GetInt(mapping["heal"]),
			summon = SourceData.GetInt(mapping["summon"]),
			buff = SourceData.GetInt(mapping["buff"]),
			debuff = SourceData.GetInt(mapping["debuff"]),
			tag = SourceData.GetStringArray(mapping["tag"]),
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
