﻿using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class SourceElement : SourceDataInt<SourceElement.Row>
{
	public override SourceElement.Row CreateRow()
	{
		return new SourceElement.Row
		{
			id = SourceData.GetInt(0),
			alias = SourceData.GetString(1),
			name_JP = SourceData.GetString(2),
			name = SourceData.GetString(3),
			altname_JP = SourceData.GetString(4),
			altname = SourceData.GetString(5),
			aliasParent = SourceData.GetString(6),
			parentFactor = SourceData.GetFloat(7),
			lvFactor = SourceData.GetInt(8),
			encFactor = SourceData.GetInt(9),
			mtp = SourceData.GetInt(10),
			aliasRef = SourceData.GetString(11),
			aliasMtp = SourceData.GetString(12),
			sort = SourceData.GetInt(13),
			target = SourceData.GetString(14),
			proc = SourceData.GetStringArray(15),
			type = SourceData.GetString(16),
			group = SourceData.GetString(17),
			category = SourceData.GetString(18),
			categorySub = SourceData.GetString(19),
			abilityType = SourceData.GetStringArray(20),
			tag = SourceData.GetStringArray(21),
			thing = SourceData.GetString(22),
			eleP = SourceData.GetInt(23),
			cooldown = SourceData.GetInt(24),
			LV = SourceData.GetInt(25),
			chance = SourceData.GetInt(26),
			value = SourceData.GetInt(27),
			cost = SourceData.GetIntArray(28),
			charge = SourceData.GetInt(29),
			radius = SourceData.GetFloat(30),
			max = SourceData.GetInt(31),
			req = SourceData.GetStringArray(32),
			idTrainer = SourceData.GetString(33),
			encSlot = SourceData.GetString(34),
			partySkill = SourceData.GetInt(35),
			tagTrainer = SourceData.GetString(36),
			levelBonus_JP = SourceData.GetString(37),
			levelBonus = SourceData.GetString(38),
			foodEffect = SourceData.GetStringArray(39),
			langAct = SourceData.GetStringArray(41),
			detail_JP = SourceData.GetString(42),
			detail = SourceData.GetString(43),
			textPhase_JP = SourceData.GetString(44),
			textPhase = SourceData.GetString(45),
			textExtra_JP = SourceData.GetString(46),
			textExtra = SourceData.GetString(47),
			textInc_JP = SourceData.GetString(48),
			textInc = SourceData.GetString(49),
			textDec_JP = SourceData.GetString(50),
			textDec = SourceData.GetString(51),
			textAlt_JP = SourceData.GetStringArray(52),
			textAlt = SourceData.GetStringArray(53),
			adjective_JP = SourceData.GetStringArray(54),
			adjective = SourceData.GetStringArray(55)
		};
	}

	public override void SetRow(SourceElement.Row r)
	{
		this.map[r.id] = r;
	}

	public override void OnInit()
	{
		foreach (SourceElement.Row row in this.rows)
		{
			if (row.id >= 100 && row.id < 400)
			{
				this.hobbies.Add(row);
			}
			row.isAttribute = (row.category == "attribute");
			row.isPrimaryAttribute = (row.isAttribute && row.tag.Contains("primary"));
			row.isSkill = (row.category == "skill");
			row.isSpell = (row.categorySub == "spell");
		}
	}

	public override void OnAfterImportData()
	{
		Core.SetCurrent(null);
		foreach (SourceElement.Row row in this.rows)
		{
			this.map[row.id] = row;
			this.alias[row.GetAlias] = row;
		}
		int num = 50000;
		int num2 = 0;
		for (int i = 910; i < 925; i++)
		{
			SourceElement.Row ele = EClass.sources.elements.map[i];
			this.AddRow(ele, num + num2 + 100, "ball_");
			this.AddRow(ele, num + num2 + 200, "breathe_");
			this.AddRow(ele, num + num2 + 300, "bolt_");
			this.AddRow(ele, num + num2 + 400, "hand_");
			this.AddRow(ele, num + num2 + 500, "arrow_");
			this.AddRow(ele, num + num2 + 600, "funnel_");
			this.AddRow(ele, num + num2 + 700, "miasma_");
			this.AddRow(ele, num + num2 + 800, "weapon_");
			this.AddRow(ele, num + num2 + 900, "puddle_");
			num2++;
		}
	}

	public void AddRow(SourceElement.Row ele, int id, string idOrg)
	{
		SourceElement.Row row = EClass.sources.elements.alias[idOrg];
		System.Reflection.FieldInfo[] fields = row.GetType().GetFields();
		SourceElement.Row row2 = new SourceElement.Row();
		foreach (System.Reflection.FieldInfo fieldInfo in fields)
		{
			row2.SetField(fieldInfo.Name, row.GetField(fieldInfo.Name));
		}
		row2.id = id;
		row2.idMold = row.id;
		row2.alias = row.alias + ele.alias.Remove(0, 3);
		row2.aliasRef = ele.alias;
		row2.aliasParent = ele.aliasParent;
		row2.chance = row.chance;
		row2.LV = row.LV;
		row2.OnImportData(EClass.sources.elements);
		this.rows.Add(row2);
	}

	public override string[] ImportFields
	{
		get
		{
			return new string[]
			{
				"altname",
				"textPhase",
				"textExtra",
				"textInc",
				"textDec",
				"textAlt",
				"adjective",
				"levelBonus"
			};
		}
	}

	[NonSerialized]
	public List<SourceElement.Row> hobbies = new List<SourceElement.Row>();

	[Serializable]
	public class Row : SourceData.BaseRow
	{
		public override bool UseAlias
		{
			get
			{
				return true;
			}
		}

		public override string GetAlias
		{
			get
			{
				return this.alias;
			}
		}

		public override string GetName()
		{
			if (this.idMold != 0)
			{
				return EClass.sources.elements.map[this.idMold].alias.lang(EClass.sources.elements.alias[this.aliasRef].GetAltname(0), null, null, null, null);
			}
			return base.GetName();
		}

		public Sprite GetSprite()
		{
			Sprite result;
			if ((result = SpriteSheet.Get("Media/Graphics/Icon/Element/icon_ability", this.alias)) == null)
			{
				result = (SpriteSheet.Get("Media/Graphics/Icon/Element/icon_ability", this.type) ?? EClass.core.refs.icons.defaultAbility);
			}
			return result;
		}

		public void SetImage(Image image)
		{
			image.sprite = (this.GetSprite() ?? EClass.core.refs.icons.defaultAbility);
			if (!this.aliasRef.IsEmpty())
			{
				image.color = EClass.setting.elements[this.aliasRef].colorSprite;
				return;
			}
			image.color = Color.white;
		}

		public string GetAltname(int i)
		{
			return base.GetText("altname", false).Split(',', StringSplitOptions.None).TryGet(i, -1);
		}

		public bool IsEncAppliable(Thing t, bool isMaterial)
		{
			if (isMaterial && t.IsEquipmentOrRanged)
			{
				return true;
			}
			if (this.tag.Contains("trait"))
			{
				return !t.IsEquipmentOrRanged;
			}
			return this.IsEncAppliable(t.category);
		}

		public bool IsEncAppliable(Thing t)
		{
			if (this.id == 10)
			{
				return true;
			}
			if (this.tag.Contains("trait"))
			{
				return !t.IsEquipmentOrRanged && !t.IsAmmo;
			}
			if (!t.IsEquipmentOrRanged)
			{
				return false;
			}
			if (t.IsAmmo)
			{
				this.IsEncAppliable(t.category);
			}
			return true;
		}

		public bool IsEncAppliable(SourceCategory.Row cat)
		{
			if (this.encSlot.IsEmpty())
			{
				return false;
			}
			if (this.encSlot == "global")
			{
				return true;
			}
			int slot = cat.slot;
			if (slot == 0)
			{
				return false;
			}
			string a = this.encSlot;
			if (a == "all")
			{
				return true;
			}
			if (!(a == "weapon"))
			{
				return this.encSlot.Contains(EClass.sources.elements.map[slot].alias);
			}
			return cat.IsChildOf("weapon");
		}

		public int id;

		public string alias;

		public string name_JP;

		public string name;

		public string altname_JP;

		public string altname;

		public string aliasParent;

		public float parentFactor;

		public int lvFactor;

		public int encFactor;

		public int mtp;

		public string aliasRef;

		public string aliasMtp;

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

		public int LV;

		public int chance;

		public int value;

		public int[] cost;

		public int charge;

		public float radius;

		public int max;

		public string[] req;

		public string idTrainer;

		public string encSlot;

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

		public int idMold;

		public string name_L;

		public string altname_L;

		public string detail_L;

		public string textPhase_L;

		public string textExtra_L;

		public string textInc_L;

		public string textDec_L;

		public string levelBonus_L;

		public string[] textAlt_L;

		public string[] adjective_L;
	}
}