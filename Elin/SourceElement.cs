using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class SourceElement : SourceDataInt<SourceElement.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public int id;

		public string alias;

		public string name_JP;

		public string name;

		public string altname_JP;

		public string altname;

		public string aliasParent;

		public string aliasRef;

		public string aliasMtp;

		public float parentFactor;

		public int lvFactor;

		public int encFactor;

		public string encSlot;

		public int mtp;

		public int LV;

		public int chance;

		public int value;

		public int[] cost;

		public int geneSlot;

		public int sort;

		public string target;

		public string[] proc;

		public string type;

		public string group;

		public string category;

		public string categorySub;

		public string[] abilityType;

		public string[] tag;

		public string thing;

		public int eleP;

		public int cooldown;

		public int charge;

		public float radius;

		public int max;

		public string[] req;

		public string idTrainer;

		public int partySkill;

		public string tagTrainer;

		public string levelBonus_JP;

		public string levelBonus;

		public string[] foodEffect;

		public string[] langAct;

		public string detail_JP;

		public string detail;

		public string textPhase_JP;

		public string textPhase;

		public string textExtra_JP;

		public string textExtra;

		public string textInc_JP;

		public string textInc;

		public string textDec_JP;

		public string textDec;

		public string[] textAlt_JP;

		public string[] textAlt;

		public string[] adjective_JP;

		public string[] adjective;

		[NonSerialized]
		public bool isAttribute;

		[NonSerialized]
		public bool isPrimaryAttribute;

		[NonSerialized]
		public bool isSkill;

		[NonSerialized]
		public bool isSpell;

		[NonSerialized]
		public bool isTrait;

		public int idMold;

		[NonSerialized]
		public string name_L;

		[NonSerialized]
		public string altname_L;

		[NonSerialized]
		public string detail_L;

		[NonSerialized]
		public string textPhase_L;

		[NonSerialized]
		public string textExtra_L;

		[NonSerialized]
		public string textInc_L;

		[NonSerialized]
		public string textDec_L;

		[NonSerialized]
		public string levelBonus_L;

		[NonSerialized]
		public string[] textAlt_L;

		[NonSerialized]
		public string[] adjective_L;

		public override bool UseAlias => true;

		public override string GetAlias => alias;

		public bool IsWeaponEnc
		{
			get
			{
				if (!tag.Contains("weaponEnc") && !(categorySub == "eleConvert") && !(categorySub == "eleAttack"))
				{
					return category == "ability";
				}
				return true;
			}
		}

		public bool IsShieldEnc => encSlot == "shield";

		public override string GetName()
		{
			if (idMold != 0)
			{
				return EClass.sources.elements.map[idMold].alias.lang(EClass.sources.elements.alias[aliasRef].GetAltname(0));
			}
			return base.GetName();
		}

		public Sprite GetSprite()
		{
			return SpriteSheet.Get("Media/Graphics/Icon/Element/icon_ability", alias) ?? SpriteSheet.Get("Media/Graphics/Icon/Element/icon_ability", type) ?? EClass.core.refs.icons.defaultAbility;
		}

		public void SetImage(Image image)
		{
			image.sprite = GetSprite() ?? EClass.core.refs.icons.defaultAbility;
			if (!aliasRef.IsEmpty())
			{
				image.color = EClass.setting.elements[aliasRef].colorSprite;
			}
			else
			{
				image.color = Color.white;
			}
		}

		public string GetAltname(int i)
		{
			return GetText("altname").Split(',').TryGet(i);
		}

		public bool IsMaterialEncAppliable(Thing t)
		{
			if (id == 10)
			{
				return true;
			}
			if (isTrait)
			{
				if (t.IsEquipmentOrRangedOrAmmo)
				{
					return false;
				}
				return true;
			}
			if (!t.IsEquipmentOrRangedOrAmmo && !t.IsThrownWeapon && !(t.trait is TraitToolMusic))
			{
				return false;
			}
			if (!t.IsEquipment)
			{
				return IsWeaponEnc;
			}
			return true;
		}

		public bool IsEncAppliable(SourceCategory.Row cat)
		{
			if (encSlot.IsEmpty())
			{
				return false;
			}
			if (encSlot == "global")
			{
				return true;
			}
			int slot = cat.slot;
			if (slot == 0)
			{
				return IsWeaponEnc;
			}
			switch (encSlot)
			{
			case "all":
				return true;
			case "weapon":
				return cat.IsChildOf("weapon");
			case "shield":
				if (!cat.IsChildOf("shield"))
				{
					return cat.IsChildOf("martial");
				}
				return true;
			default:
				return encSlot.Contains(EClass.sources.elements.map[slot].alias);
			}
		}
	}

	public static readonly IReadOnlyDictionary<string, int> RowMapping = new Dictionary<string, int>
	{
		["id"] = 0,
		["alias"] = 1,
		["name_JP"] = 2,
		["name"] = 3,
		["altname_JP"] = 4,
		["altname"] = 5,
		["aliasParent"] = 6,
		["aliasRef"] = 7,
		["aliasMtp"] = 8,
		["parentFactor"] = 9,
		["lvFactor"] = 10,
		["encFactor"] = 11,
		["encSlot"] = 12,
		["mtp"] = 13,
		["LV"] = 14,
		["chance"] = 15,
		["value"] = 16,
		["cost"] = 17,
		["geneSlot"] = 18,
		["sort"] = 19,
		["target"] = 20,
		["proc"] = 21,
		["type"] = 22,
		["group"] = 23,
		["category"] = 24,
		["categorySub"] = 25,
		["abilityType"] = 26,
		["tag"] = 27,
		["thing"] = 28,
		["eleP"] = 29,
		["cooldown"] = 30,
		["charge"] = 31,
		["radius"] = 32,
		["max"] = 33,
		["req"] = 34,
		["idTrainer"] = 35,
		["partySkill"] = 36,
		["tagTrainer"] = 37,
		["levelBonus_JP"] = 38,
		["levelBonus"] = 39,
		["foodEffect"] = 40,
		["langAct"] = 42,
		["detail_JP"] = 43,
		["detail"] = 44,
		["textPhase_JP"] = 45,
		["textPhase"] = 46,
		["textExtra_JP"] = 47,
		["textExtra"] = 48,
		["textInc_JP"] = 49,
		["textInc"] = 50,
		["textDec_JP"] = 51,
		["textDec"] = 52,
		["textAlt_JP"] = 53,
		["textAlt"] = 54,
		["adjective_JP"] = 55,
		["adjective"] = 56
	};

	public static readonly IReadOnlyDictionary<string, string> TypeMapping = new Dictionary<string, string>
	{
		["id"] = "int",
		["alias"] = "string",
		["name_JP"] = "string",
		["name"] = "string",
		["altname_JP"] = "string",
		["altname"] = "string",
		["aliasParent"] = "string",
		["aliasRef"] = "string",
		["aliasMtp"] = "string",
		["parentFactor"] = "float",
		["lvFactor"] = "int",
		["encFactor"] = "int",
		["encSlot"] = "string",
		["mtp"] = "int",
		["LV"] = "int",
		["chance"] = "int",
		["value"] = "int",
		["cost"] = "int[]",
		["geneSlot"] = "int",
		["sort"] = "int",
		["target"] = "string",
		["proc"] = "string[]",
		["type"] = "string",
		["group"] = "string",
		["category"] = "string",
		["categorySub"] = "string",
		["abilityType"] = "string[]",
		["tag"] = "string[]",
		["thing"] = "string",
		["eleP"] = "int",
		["cooldown"] = "int",
		["charge"] = "int",
		["radius"] = "float",
		["max"] = "int",
		["req"] = "string[]",
		["idTrainer"] = "string",
		["partySkill"] = "int",
		["tagTrainer"] = "string",
		["levelBonus_JP"] = "string",
		["levelBonus"] = "string",
		["foodEffect"] = "string[]",
		["langAct"] = "string[]",
		["detail_JP"] = "string",
		["detail"] = "string",
		["textPhase_JP"] = "string",
		["textPhase"] = "string",
		["textExtra_JP"] = "string",
		["textExtra"] = "string",
		["textInc_JP"] = "string",
		["textInc"] = "string",
		["textDec_JP"] = "string",
		["textDec"] = "string",
		["textAlt_JP"] = "string[]",
		["textAlt"] = "string[]",
		["adjective_JP"] = "string[]",
		["adjective"] = "string[]"
	};

	[NonSerialized]
	public List<Row> hobbies = new List<Row>();

	[NonSerialized]
	public Dictionary<string, string> fuzzyAlias = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

	public override string[] ImportFields => new string[8] { "altname", "textPhase", "textExtra", "textInc", "textDec", "textAlt", "adjective", "levelBonus" };

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetInt(0),
			alias = SourceData.GetString(1),
			name_JP = SourceData.GetString(2),
			name = SourceData.GetString(3),
			altname_JP = SourceData.GetString(4),
			altname = SourceData.GetString(5),
			aliasParent = SourceData.GetString(6),
			aliasRef = SourceData.GetString(7),
			aliasMtp = SourceData.GetString(8),
			parentFactor = SourceData.GetFloat(9),
			lvFactor = SourceData.GetInt(10),
			encFactor = SourceData.GetInt(11),
			encSlot = SourceData.GetString(12),
			mtp = SourceData.GetInt(13),
			LV = SourceData.GetInt(14),
			chance = SourceData.GetInt(15),
			value = SourceData.GetInt(16),
			cost = SourceData.GetIntArray(17),
			geneSlot = SourceData.GetInt(18),
			sort = SourceData.GetInt(19),
			target = SourceData.GetString(20),
			proc = SourceData.GetStringArray(21),
			type = SourceData.GetString(22),
			group = SourceData.GetString(23),
			category = SourceData.GetString(24),
			categorySub = SourceData.GetString(25),
			abilityType = SourceData.GetStringArray(26),
			tag = SourceData.GetStringArray(27),
			thing = SourceData.GetString(28),
			eleP = SourceData.GetInt(29),
			cooldown = SourceData.GetInt(30),
			charge = SourceData.GetInt(31),
			radius = SourceData.GetFloat(32),
			max = SourceData.GetInt(33),
			req = SourceData.GetStringArray(34),
			idTrainer = SourceData.GetString(35),
			partySkill = SourceData.GetInt(36),
			tagTrainer = SourceData.GetString(37),
			levelBonus_JP = SourceData.GetString(38),
			levelBonus = SourceData.GetString(39),
			foodEffect = SourceData.GetStringArray(40),
			langAct = SourceData.GetStringArray(42),
			detail_JP = SourceData.GetString(43),
			detail = SourceData.GetString(44),
			textPhase_JP = SourceData.GetString(45),
			textPhase = SourceData.GetString(46),
			textExtra_JP = SourceData.GetString(47),
			textExtra = SourceData.GetString(48),
			textInc_JP = SourceData.GetString(49),
			textInc = SourceData.GetString(50),
			textDec_JP = SourceData.GetString(51),
			textDec = SourceData.GetString(52),
			textAlt_JP = SourceData.GetStringArray(53),
			textAlt = SourceData.GetStringArray(54),
			adjective_JP = SourceData.GetStringArray(55),
			adjective = SourceData.GetStringArray(56)
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
			altname_JP = SourceData.GetString(mapping["altname_JP"]),
			altname = SourceData.GetString(mapping["altname"]),
			aliasParent = SourceData.GetString(mapping["aliasParent"]),
			aliasRef = SourceData.GetString(mapping["aliasRef"]),
			aliasMtp = SourceData.GetString(mapping["aliasMtp"]),
			parentFactor = SourceData.GetFloat(mapping["parentFactor"]),
			lvFactor = SourceData.GetInt(mapping["lvFactor"]),
			encFactor = SourceData.GetInt(mapping["encFactor"]),
			encSlot = SourceData.GetString(mapping["encSlot"]),
			mtp = SourceData.GetInt(mapping["mtp"]),
			LV = SourceData.GetInt(mapping["LV"]),
			chance = SourceData.GetInt(mapping["chance"]),
			value = SourceData.GetInt(mapping["value"]),
			cost = SourceData.GetIntArray(mapping["cost"]),
			geneSlot = SourceData.GetInt(mapping["geneSlot"]),
			sort = SourceData.GetInt(mapping["sort"]),
			target = SourceData.GetString(mapping["target"]),
			proc = SourceData.GetStringArray(mapping["proc"]),
			type = SourceData.GetString(mapping["type"]),
			group = SourceData.GetString(mapping["group"]),
			category = SourceData.GetString(mapping["category"]),
			categorySub = SourceData.GetString(mapping["categorySub"]),
			abilityType = SourceData.GetStringArray(mapping["abilityType"]),
			tag = SourceData.GetStringArray(mapping["tag"]),
			thing = SourceData.GetString(mapping["thing"]),
			eleP = SourceData.GetInt(mapping["eleP"]),
			cooldown = SourceData.GetInt(mapping["cooldown"]),
			charge = SourceData.GetInt(mapping["charge"]),
			radius = SourceData.GetFloat(mapping["radius"]),
			max = SourceData.GetInt(mapping["max"]),
			req = SourceData.GetStringArray(mapping["req"]),
			idTrainer = SourceData.GetString(mapping["idTrainer"]),
			partySkill = SourceData.GetInt(mapping["partySkill"]),
			tagTrainer = SourceData.GetString(mapping["tagTrainer"]),
			levelBonus_JP = SourceData.GetString(mapping["levelBonus_JP"]),
			levelBonus = SourceData.GetString(mapping["levelBonus"]),
			foodEffect = SourceData.GetStringArray(mapping["foodEffect"]),
			langAct = SourceData.GetStringArray(mapping["langAct"]),
			detail_JP = SourceData.GetString(mapping["detail_JP"]),
			detail = SourceData.GetString(mapping["detail"]),
			textPhase_JP = SourceData.GetString(mapping["textPhase_JP"]),
			textPhase = SourceData.GetString(mapping["textPhase"]),
			textExtra_JP = SourceData.GetString(mapping["textExtra_JP"]),
			textExtra = SourceData.GetString(mapping["textExtra"]),
			textInc_JP = SourceData.GetString(mapping["textInc_JP"]),
			textInc = SourceData.GetString(mapping["textInc"]),
			textDec_JP = SourceData.GetString(mapping["textDec_JP"]),
			textDec = SourceData.GetString(mapping["textDec"]),
			textAlt_JP = SourceData.GetStringArray(mapping["textAlt_JP"]),
			textAlt = SourceData.GetStringArray(mapping["textAlt"]),
			adjective_JP = SourceData.GetStringArray(mapping["adjective_JP"]),
			adjective = SourceData.GetStringArray(mapping["adjective"])
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

	public override IReadOnlyDictionary<string, string> GetTypeMapping()
	{
		return TypeMapping;
	}

	public override void OnInit()
	{
		hobbies.Clear();
		foreach (Row row in rows)
		{
			if (row.id >= 100 && row.id < 400)
			{
				hobbies.Add(row);
			}
			row.isAttribute = row.category == "attribute";
			row.isPrimaryAttribute = row.isAttribute && row.tag.Contains("primary");
			row.isSkill = row.category == "skill";
			row.isSpell = row.categorySub == "spell";
			row.isTrait = row.tag.Contains("trait");
		}
		fuzzyAlias.Clear();
		foreach (string key in alias.Keys)
		{
			fuzzyAlias[key] = key;
		}
	}

	public override void OnAfterImportData()
	{
		Core.SetCurrent();
		map.Clear();
		alias.Clear();
		foreach (Row row in rows)
		{
			map[row.id] = row;
			alias[row.GetAlias] = row;
		}
		int num = 50000;
		int num2 = 0;
		for (int i = 910; i < 927; i++)
		{
			Row ele = EClass.sources.elements.map[i];
			AddRow(ele, num + num2 + 100, "ball_");
			AddRow(ele, num + num2 + 200, "breathe_");
			AddRow(ele, num + num2 + 300, "bolt_");
			AddRow(ele, num + num2 + 400, "hand_");
			AddRow(ele, num + num2 + 500, "arrow_");
			AddRow(ele, num + num2 + 600, "funnel_");
			AddRow(ele, num + num2 + 700, "miasma_");
			AddRow(ele, num + num2 + 800, "weapon_");
			AddRow(ele, num + num2 + 900, "puddle_");
			AddRow(ele, num + num2 + 1000, "sword_");
			AddRow(ele, num + num2 + 1100, "bit_");
			AddRow(ele, num + num2 + 1200, "flare_");
			num2++;
		}
		initialized = false;
	}

	public void AddRow(Row ele, int id, string idOrg)
	{
		if (map.ContainsKey(id))
		{
			return;
		}
		Row row = EClass.sources.elements.alias[idOrg];
		Dictionary<string, System.Reflection.FieldInfo> rowFields = row.GetRowFields();
		Row row2 = new Row();
		foreach (System.Reflection.FieldInfo value in rowFields.Values)
		{
			value.SetValue(row2, value.GetValue(row));
		}
		row2.id = id;
		row2.idMold = row.id;
		row2.alias = row.alias + ele.alias.Remove(0, 3);
		row2.aliasRef = ele.alias;
		row2.aliasParent = ele.aliasParent;
		row2.chance = row.chance * ele.chance / 100;
		row2.LV = row.LV;
		row2.OnImportData(EClass.sources.elements);
		rows.Add(row2);
	}
}
