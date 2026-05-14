using System;
using System.Collections.Generic;
using UnityEngine;

public class SourceMaterial : SourceDataInt<SourceMaterial.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public int id;

		public string alias;

		public string name_JP;

		public string name;

		public string category;

		public string[] tag;

		public string thing;

		public string[] goods;

		public string[] minerals;

		public int decal;

		public int decay;

		public int grass;

		public int defFloor;

		public int defBlock;

		public int edge;

		public int ramp;

		public string idSound;

		public string soundFoot;

		public int hardness;

		public string[] groups;

		public int tier;

		public int chance;

		public int weight;

		public int value;

		public int quality;

		public int atk;

		public int dmg;

		public int dv;

		public int pv;

		public int dice;

		public string[] bits;

		public int[] elements;

		public string[] altName;

		public string[] altName_JP;

		[NonSerialized]
		public Color matColor;

		[NonSerialized]
		public Color altColor;

		public Dictionary<int, int> elementMap;

		public static string[] IDPick = new string[5] { "rock", "ore", "gem", "crystal", "bone" };

		public static string[] IDAxe = new string[1] { "wood" };

		[NonSerialized]
		public string name_L;

		[NonSerialized]
		public string detail_L;

		[NonSerialized]
		public string[] altName_L;

		public override bool UseAlias => true;

		public override string GetAlias => alias;

		public bool UsePick => IDPick.Contains(category);

		public bool UseAxe => IDAxe.Contains(category);

		public override void OnImportData(SourceData data)
		{
			base.OnImportData(data);
			SetTiles();
		}

		public void Init()
		{
			MatColors matColors = Core.Instance.Colors.matColors.TryGetValue(alias);
			if (matColors == null)
			{
				matColors = new MatColors();
				string[] array = tag;
				foreach (string text in array)
				{
					if (text.StartsWith("addCol"))
					{
						if (text.Contains("_Main"))
						{
							matColors.main = text.ExtractInBetween('(', ')').ToColor();
						}
						if (text.Contains("_Alt"))
						{
							matColors.alt = text.ExtractInBetween('(', ')').ToColor();
						}
					}
				}
				Core.Instance.Colors.matColors[alias] = matColors;
			}
			matColor = matColors.main;
			altColor = matColors.alt;
			SetTiles();
		}

		public void SetTiles()
		{
		}

		public bool HasEnc(int id)
		{
			foreach (int key in elementMap.Keys)
			{
				if (key == id)
				{
					return true;
				}
			}
			return false;
		}

		public void AddBlood(Point p, int a = 1)
		{
			if (decal == 0)
			{
				return;
			}
			if (p.cell.IsSnowTile && this != MATERIAL.sourceSnow)
			{
				MATERIAL.sourceSnow.AddBlood(p, a);
				return;
			}
			for (int i = 0; i < a; i++)
			{
				EClass._map.AddDecal(p.x + ((EClass.rnd(2) != 0) ? (EClass.rnd(3) - 1) : 0), p.z + ((EClass.rnd(2) != 0) ? (EClass.rnd(3) - 1) : 0), decal);
			}
		}

		public void PlayHitEffect(Point p, int emit = 2)
		{
			Effect.Get("mine2").Play(p).SetParticleColor(GetColor())
				.Emit(emit + EClass.rnd(4));
		}

		public Color GetColor()
		{
			return Core.Instance.Colors.matColors[alias].main;
		}

		public string TryGetSound(string suffx, RenderRow c = null)
		{
			string soundImpact = GetSoundImpact(c);
			if (EClass.Sound.Exist(soundImpact + "_" + suffx))
			{
				return soundImpact + "_" + suffx;
			}
			return soundImpact;
		}

		public string GetSoundDead(RenderRow c = null)
		{
			return TryGetSound("dead", c);
		}

		public string GetSoundDrop(RenderRow c = null)
		{
			return TryGetSound("drop", c);
		}

		public string GetSoundCraft(RenderRow c = null)
		{
			if (category == "wood")
			{
				return "build_progress";
			}
			return TryGetSound("craft", c);
		}

		public string GetSoundImpact(RenderRow c = null)
		{
			if (c != null && !c.idSound.IsEmpty())
			{
				return "Material/" + c.idSound;
			}
			return "Material/" + idSound;
		}

		public void CreateByProduct(Thing container, int num)
		{
			Thing thing = ThingGen.CreateRawMaterial(this);
			thing.ModNum(num);
			container.AddThing(thing);
			C("dye");
			switch (category)
			{
			case "bone":
				C("glass");
				C("stick");
				break;
			case "crystal":
				C("glass");
				C("gem");
				break;
			case "ore":
				C("scrap");
				C("cutstone");
				C("ingot");
				break;
			case "rock":
				C("rock");
				C("pebble");
				C("cutstone");
				break;
			case "soil":
				C("glass");
				C("clay");
				C("brick");
				break;
			case "gem":
				C("cutstone");
				C("ingot");
				C("gem");
				break;
			case "wood":
				C("plank");
				C("stick");
				C("bark");
				break;
			case "fiber":
				C("thread");
				C("texture");
				C("string");
				break;
			case "skin":
				C("texture");
				break;
			}
			string[] array = goods;
			foreach (string text in array)
			{
				C(text);
			}
			void C(string _id)
			{
				Thing thing2 = ThingGen.Create(_id);
				thing2.ChangeMaterial(id);
				thing2.ModNum(num);
				container.AddThing(thing2);
			}
		}
	}

	public class TierList
	{
		public Tier[] tiers;

		public TierList()
		{
			tiers = new Tier[8];
			for (int i = 0; i < 8; i++)
			{
				tiers[i] = new Tier();
			}
		}
	}

	public class Tier
	{
		public int sum;

		public List<Row> list = new List<Row>();

		public Row Select()
		{
			int num = 0;
			int num2 = EClass.rnd(sum);
			foreach (Row item in list)
			{
				num += item.chance;
				if (num2 < num)
				{
					return item;
				}
			}
			return list.RandomItem();
		}
	}

	public static readonly IReadOnlyDictionary<string, int> RowMapping = new Dictionary<string, int>
	{
		["id"] = 0,
		["alias"] = 1,
		["name_JP"] = 2,
		["name"] = 3,
		["category"] = 4,
		["tag"] = 5,
		["thing"] = 6,
		["goods"] = 7,
		["minerals"] = 8,
		["decal"] = 9,
		["decay"] = 10,
		["grass"] = 11,
		["defFloor"] = 12,
		["defBlock"] = 13,
		["edge"] = 14,
		["ramp"] = 15,
		["idSound"] = 16,
		["soundFoot"] = 17,
		["hardness"] = 18,
		["groups"] = 19,
		["tier"] = 20,
		["chance"] = 21,
		["weight"] = 22,
		["value"] = 23,
		["quality"] = 24,
		["atk"] = 25,
		["dmg"] = 26,
		["dv"] = 27,
		["pv"] = 28,
		["dice"] = 29,
		["bits"] = 30,
		["elements"] = 31,
		["altName"] = 32,
		["altName_JP"] = 33
	};

	public static Dictionary<string, TierList> tierMap = new Dictionary<string, TierList>();

	public override string[] ImportFields => new string[1] { "altName" };

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetInt(0),
			alias = SourceData.GetString(1),
			name_JP = SourceData.GetString(2),
			name = SourceData.GetString(3),
			category = SourceData.GetString(4),
			tag = SourceData.GetStringArray(5),
			thing = SourceData.GetString(6),
			goods = SourceData.GetStringArray(7),
			minerals = SourceData.GetStringArray(8),
			decal = SourceData.GetInt(9),
			decay = SourceData.GetInt(10),
			grass = SourceData.GetInt(11),
			defFloor = SourceData.GetInt(12),
			defBlock = SourceData.GetInt(13),
			edge = SourceData.GetInt(14),
			ramp = SourceData.GetInt(15),
			idSound = SourceData.GetString(16),
			soundFoot = SourceData.GetString(17),
			hardness = SourceData.GetInt(18),
			groups = SourceData.GetStringArray(19),
			tier = SourceData.GetInt(20),
			chance = SourceData.GetInt(21),
			weight = SourceData.GetInt(22),
			value = SourceData.GetInt(23),
			quality = SourceData.GetInt(24),
			atk = SourceData.GetInt(25),
			dmg = SourceData.GetInt(26),
			dv = SourceData.GetInt(27),
			pv = SourceData.GetInt(28),
			dice = SourceData.GetInt(29),
			bits = SourceData.GetStringArray(30),
			elements = Core.ParseElements(SourceData.GetStr(31)),
			altName = SourceData.GetStringArray(32),
			altName_JP = SourceData.GetStringArray(33)
		};
	}

	public override Row CreateRowByMapping(IReadOnlyDictionary<string, int> mapping)
	{
		return new Row
		{
			id = SourceData.GetInt(mapping["id"]),
			alias = SourceData.GetString(mapping["alias"]),
			name_JP = SourceData.GetString(mapping["name_JP"]),
			name = SourceData.GetString(mapping["name"]),
			category = SourceData.GetString(mapping["category"]),
			tag = SourceData.GetStringArray(mapping["tag"]),
			thing = SourceData.GetString(mapping["thing"]),
			goods = SourceData.GetStringArray(mapping["goods"]),
			minerals = SourceData.GetStringArray(mapping["minerals"]),
			decal = SourceData.GetInt(mapping["decal"]),
			decay = SourceData.GetInt(mapping["decay"]),
			grass = SourceData.GetInt(mapping["grass"]),
			defFloor = SourceData.GetInt(mapping["defFloor"]),
			defBlock = SourceData.GetInt(mapping["defBlock"]),
			edge = SourceData.GetInt(mapping["edge"]),
			ramp = SourceData.GetInt(mapping["ramp"]),
			idSound = SourceData.GetString(mapping["idSound"]),
			soundFoot = SourceData.GetString(mapping["soundFoot"]),
			hardness = SourceData.GetInt(mapping["hardness"]),
			groups = SourceData.GetStringArray(mapping["groups"]),
			tier = SourceData.GetInt(mapping["tier"]),
			chance = SourceData.GetInt(mapping["chance"]),
			weight = SourceData.GetInt(mapping["weight"]),
			value = SourceData.GetInt(mapping["value"]),
			quality = SourceData.GetInt(mapping["quality"]),
			atk = SourceData.GetInt(mapping["atk"]),
			dmg = SourceData.GetInt(mapping["dmg"]),
			dv = SourceData.GetInt(mapping["dv"]),
			pv = SourceData.GetInt(mapping["pv"]),
			dice = SourceData.GetInt(mapping["dice"]),
			bits = SourceData.GetStringArray(mapping["bits"]),
			elements = Core.ParseElements(SourceData.GetStr(mapping["elements"])),
			altName = SourceData.GetStringArray(mapping["altName"]),
			altName_JP = SourceData.GetStringArray(mapping["altName_JP"])
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
		Cell.matList = rows;
		tierMap.Clear();
		tierMap.Add("gem", new TierList());
		tierMap.Add("ore", new TierList());
		foreach (Row row in rows)
		{
			row.Init();
			row.elementMap = Element.GetElementMap(row.elements);
			if (!row.groups.IsEmpty())
			{
				string[] groups = row.groups;
				foreach (string key in groups)
				{
					TierList tierList = tierMap.TryGetValue(key);
					if (tierList == null)
					{
						tierList = new TierList();
						tierMap[key] = tierList;
					}
					Tier obj = tierList.tiers[row.tier];
					obj.list.Add(row);
					obj.sum += row.chance;
				}
			}
			string category = row.category;
			if (!(category == "gem"))
			{
				if (category == "ore")
				{
					Tier obj2 = tierMap["ore"].tiers[row.tier];
					obj2.list.Add(row);
					obj2.sum += row.chance;
				}
			}
			else
			{
				Tier obj3 = tierMap["gem"].tiers[row.tier];
				obj3.list.Add(row);
				obj3.sum += row.chance;
			}
		}
	}

	public override void OnAfterImportData()
	{
		rows.Sort((Row a, Row b) => a.id - b.id);
	}

	public void OnImportRow(Row r)
	{
		List<int> list = new List<int>(r.elements);
		for (int i = 0; i < r.elements.Length; i += 2)
		{
			Add(r.elements[i], r.elements[i + 1]);
		}
		Add(13, r.hardness);
		r.elements = list.ToArray();
		void Add(int ele, int a)
		{
			list.Add(ele);
			list.Add(a);
		}
	}
}
