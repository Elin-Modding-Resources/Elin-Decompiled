using System;
using System.Collections.Generic;

public class SourceBacker : SourceDataInt<SourceBacker.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public int id;

		public bool valid;

		public string lang;

		public string name;

		public string text;

		public int type;

		public int skin;

		public int gender;

		public string tree;

		public string deity;

		public string chara;

		public string loot;

		public int isStatic;

		public bool done;

		public string loc;

		public string original;

		[NonSerialized]
		public string name_L;

		[NonSerialized]
		public string text_L;

		public override bool UseAlias => false;

		public override string GetAlias => "n";

		public string Name => BackerContent.ConvertName((Lang.langCode == "CN") ? GetText() : name);

		public string Text
		{
			get
			{
				if (!(Lang.langCode == "CN"))
				{
					return text;
				}
				return GetText("text");
			}
		}
	}

	public static readonly IReadOnlyDictionary<string, int> RowMapping = new Dictionary<string, int>
	{
		["id"] = 0,
		["valid"] = 3,
		["lang"] = 7,
		["name"] = 8,
		["text"] = 9,
		["type"] = 10,
		["skin"] = 11,
		["gender"] = 12,
		["tree"] = 13,
		["deity"] = 14,
		["chara"] = 15,
		["loot"] = 16,
		["isStatic"] = 17,
		["done"] = 18,
		["loc"] = 19,
		["original"] = 20
	};

	[NonSerialized]
	public List<Row> listRemain = new List<Row>();

	[NonSerialized]
	public List<Row> listLantern = new List<Row>();

	[NonSerialized]
	public List<Row> listTree = new List<Row>();

	[NonSerialized]
	public List<Row> listPet = new List<Row>();

	[NonSerialized]
	public List<Row> listSister = new List<Row>();

	[NonSerialized]
	public List<Row> listFollower = new List<Row>();

	[NonSerialized]
	public List<Row> listSnail = new List<Row>();

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetInt(0),
			valid = SourceData.GetBool(3),
			lang = SourceData.GetString(7),
			name = SourceData.GetString(8),
			text = SourceData.GetString(9),
			type = SourceData.GetInt(10),
			skin = SourceData.GetInt(11),
			gender = SourceData.GetInt(12),
			tree = SourceData.GetString(13),
			deity = SourceData.GetString(14),
			chara = SourceData.GetString(15),
			loot = SourceData.GetString(16),
			isStatic = SourceData.GetInt(17),
			done = SourceData.GetBool(18),
			loc = SourceData.GetString(19),
			original = SourceData.GetString(20)
		};
	}

	public override Row CreateRowByMapping(IReadOnlyDictionary<string, int> mapping)
	{
		return new Row
		{
			id = SourceData.GetInt(mapping["id"]),
			valid = SourceData.GetBool(mapping["valid"]),
			lang = SourceData.GetString(mapping["lang"]),
			name = SourceData.GetString(mapping["name"]),
			text = SourceData.GetString(mapping["text"]),
			type = SourceData.GetInt(mapping["type"]),
			skin = SourceData.GetInt(mapping["skin"]),
			gender = SourceData.GetInt(mapping["gender"]),
			tree = SourceData.GetString(mapping["tree"]),
			deity = SourceData.GetString(mapping["deity"]),
			chara = SourceData.GetString(mapping["chara"]),
			loot = SourceData.GetString(mapping["loot"]),
			isStatic = SourceData.GetInt(mapping["isStatic"]),
			done = SourceData.GetBool(mapping["done"]),
			loc = SourceData.GetString(mapping["loc"]),
			original = SourceData.GetString(mapping["original"])
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
			if (row.isStatic == 0 && row.valid)
			{
				switch (row.type)
				{
				case 1:
					listRemain.Add(row);
					break;
				case 2:
					listLantern.Add(row);
					break;
				case 3:
					listTree.Add(row);
					break;
				case 4:
					listPet.Add(row);
					break;
				case 5:
					listSister.Add(row);
					break;
				case 6:
					listFollower.Add(row);
					break;
				case 7:
					listSnail.Add(row);
					break;
				}
			}
		}
		listRemain.Shuffle();
		listLantern.Shuffle();
		listTree.Shuffle();
		listPet.Shuffle();
		listSister.Shuffle();
		listFollower.Shuffle();
		listSnail.Shuffle();
	}
}
