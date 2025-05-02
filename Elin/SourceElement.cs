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
			if (!t.IsEquipmentOrRangedOrAmmo && !t.IsThrownWeapon)
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
			string text = encSlot;
			if (!(text == "all"))
			{
				if (text == "weapon")
				{
					return cat.IsChildOf("weapon");
				}
				return encSlot.Contains(EClass.sources.elements.map[slot].alias);
			}
			return true;
		}
	}

	[NonSerialized]
	public List<Row> hobbies = new List<Row>();

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

	public override void SetRow(Row r)
	{
		map[r.id] = r;
	}

	public override void OnInit()
	{
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
	}

	public override void OnAfterImportData()
	{
		Core.SetCurrent();
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
			num2++;
		}
	}

	public void AddRow(Row ele, int id, string idOrg)
	{
		if (Application.isEditor || !map.ContainsKey(id))
		{
			Row row = EClass.sources.elements.alias[idOrg];
			System.Reflection.FieldInfo[] fields = row.GetType().GetFields();
			Row row2 = new Row();
			System.Reflection.FieldInfo[] array = fields;
			foreach (System.Reflection.FieldInfo fieldInfo in array)
			{
				row2.SetField(fieldInfo.Name, row.GetField<object>(fieldInfo.Name));
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
}
