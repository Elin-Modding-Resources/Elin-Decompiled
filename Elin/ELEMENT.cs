using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ELEMENT
{
	public const int hotspring = 756;

	public const int blood = 755;

	public const int nerve = 754;

	public const int antidote = 753;

	public const int cute = 752;

	public const int rare = 751;

	public const int comfort = 750;

	public const int _void = 0;

	public const int d = 3;

	public const int lv = 1;

	public const int quality = 2;

	public const int socket = 5;

	public const int nutrition = 10;

	public const int weight = 11;

	public const int size = 12;

	public const int hardness = 13;

	public const int growth = 14;

	public const int heat = 16;

	public const int decay = 17;

	public const int taste = 18;

	public const int water = 15;

	public const int fire = 21;

	public const int cut = 22;

	public const int old_detox = 23;

	public const int old_heal = 24;

	public const int old_antidote = 25;

	public const int cure = 26;

	public const int race = 29;

	public const int piety = 85;

	public const int poison = 20;

	public static readonly int[] IDS = new int[30]
	{
		756, 755, 754, 753, 752, 751, 750, 0, 3, 1,
		2, 5, 10, 11, 12, 13, 14, 16, 17, 18,
		15, 21, 22, 23, 24, 25, 26, 29, 85, 20
	};
}
public class Element : EClass
{
	public class BonusInfo
	{
		public Element ele;

		public UINote n;

		public Chara c;

		public bool first = true;

		public int total;

		public void CheckFirst()
		{
			if (first)
			{
				first = false;
				n.Space(8);
			}
		}

		public void AddText(string text)
		{
			CheckFirst();
			n.AddText("_bullet".lang() + text, FontColor.Warning);
		}

		public void AddText(int v, string text, string textBad = null)
		{
			if (v != 0)
			{
				string text2 = text;
				if (!textBad.IsEmpty() && v < 0)
				{
					text2 = textBad;
				}
				CheckFirst();
				total += v;
				n.AddText("_bullet".lang() + text2 + " " + ((v > 0) ? "+" : "") + v, (v > 0) ? FontColor.Good : FontColor.Bad);
			}
		}

		public void AddFix(int v, string text)
		{
			if (v != 0)
			{
				CheckFirst();
				n.AddText("_bullet".lang() + text + " " + ((v > 0) ? "+" : "") + v + "%", (v > 0) ? FontColor.Good : FontColor.Bad);
			}
		}

		public void WriteNote()
		{
			int id = ele.id;
			int num = 0;
			foreach (BodySlot slot in c.body.slots)
			{
				if (slot.elementId != 44 && slot.thing != null && ((id != 67 && id != 66) || slot.elementId != 35))
				{
					Element orCreateElement = slot.thing.elements.GetOrCreateElement(id);
					if (orCreateElement != null && !orCreateElement.IsGlobalElement)
					{
						num += orCreateElement.Value;
					}
				}
			}
			AddText(num, "equipment".lang());
			if (c.IsPCFaction)
			{
				Element element = EClass.pc.faction.charaElements.GetElement(id);
				if (element != null)
				{
					AddText(element.Value, "sub_faction".lang());
				}
			}
			foreach (Condition condition in c.conditions)
			{
				if (condition.elements != null)
				{
					AddText(condition.elements.Value(id), condition.Name);
				}
			}
			if (c.tempElements != null)
			{
				AddText(c.tempElements.Value(id), "tempStrengthen".lang(), "tempWeaken".lang());
			}
			try
			{
				if (c.faithElements != null)
				{
					Element element2 = c.elements.GetElement("featGod_" + c.faith.id + "1");
					if (element2 != null)
					{
						AddText(c.faithElements.Value(id), element2.Name);
					}
				}
			}
			catch
			{
			}
			_ = ele.Value;
			_ = ele.ValueWithoutLink + total;
			foreach (Element value in c.elements.dict.Values)
			{
				if (value.HasTag("multiplier") && value.source.aliasRef == ele.source.alias)
				{
					AddFix(value.Value, value.Name);
				}
			}
			if (id == 79)
			{
				c.RefreshSpeed(this);
			}
			if (!c.race.IsMachine && !(c.id == "android"))
			{
				return;
			}
			int num2 = c.Evalue(664);
			if (num2 > 0)
			{
				switch (id)
				{
				case 64:
				case 65:
					AddFix(num2 / 2, EClass.sources.elements.map[664].GetName());
					break;
				case 79:
					AddFix(num2, EClass.sources.elements.map[664].GetName());
					break;
				}
			}
		}
	}

	public const int Div = 5;

	public static Element Void = new Element();

	public static int[] List_MainAttributes = new int[9] { 70, 72, 71, 77, 74, 75, 76, 73, 79 };

	public static int[] List_MainAttributesMajor = new int[8] { 70, 72, 71, 77, 74, 75, 76, 73 };

	public static int[] List_Body = new int[4] { 70, 72, 71, 77 };

	public static int[] List_Mind = new int[4] { 74, 75, 76, 73 };

	public SourceElement.Row _source;

	public int id;

	public int vBase;

	public int vExp;

	public int vPotential;

	public int vTempPotential;

	public int vLink;

	public int vSource;

	public int vSourcePotential;

	public ElementContainer owner;

	public static List<SourceElement.Row> ListElements = new List<SourceElement.Row>();

	public static List<SourceElement.Row> ListAttackElements = new List<SourceElement.Row>();

	public SourceElement.Row source
	{
		get
		{
			SourceElement.Row row = _source;
			if (row == null)
			{
				SourceElement.Row obj = ((id == 0) ? GetSource() : Get(id)) ?? EClass.sources.elements.rows[0];
				SourceElement.Row row2 = obj;
				_source = obj;
				row = row2;
			}
			return row;
		}
	}

	public virtual int DisplayValue => Value;

	public virtual int MinValue => -100;

	public int Value => ValueWithoutLink + vLink + ((owner != null) ? owner.ValueBonus(this) : 0);

	public int ValueWithoutLink => vBase + vSource;

	public virtual int MinPotential => 100;

	public int Potential => vPotential + vTempPotential + vSourcePotential + MinPotential;

	public virtual bool CanGainExp => ValueWithoutLink > 0;

	public bool IsFlag => source.tag.Contains("flag");

	public virtual string Name => source.GetName();

	public virtual string FullName => Name;

	public virtual int ExpToNext => 1000;

	public virtual bool UseExpMod => true;

	public virtual int CostTrain => Mathf.Max((ValueWithoutLink / 10 + 5) * (100 + vTempPotential) / 500, 1);

	public virtual int CostLearn => 5;

	public virtual bool ShowXP
	{
		get
		{
			if (!EClass.debug.showExtra)
			{
				return source.category != "attribute";
			}
			return true;
		}
	}

	public virtual bool ShowMsgOnValueChanged => true;

	public virtual bool ShowValue => true;

	public virtual bool ShowPotential => true;

	public virtual bool UsePotential => true;

	public virtual bool PotentialAsStock => false;

	public virtual bool ShowRelativeAttribute => false;

	public virtual bool ShowBonuses => true;

	public virtual string ShortName => Name;

	public bool IsGlobalElement
	{
		get
		{
			if (vExp != -1)
			{
				return vExp == -2;
			}
			return true;
		}
	}

	public bool IsFactionWideElement => vExp == -1;

	public bool IsPartyWideElement => vExp == -2;

	public virtual bool ShowEncNumber => true;

	public bool IsTrait => source.tag.Contains("trait");

	public bool IsFoodTrait => !source.foodEffect.IsEmpty();

	public bool IsFoodTraitMain
	{
		get
		{
			if (IsFoodTrait)
			{
				if (!source.tag.Contains("primary"))
				{
					return source.tag.Contains("foodpot");
				}
				return true;
			}
			return false;
		}
	}

	public bool IsMainAttribute
	{
		get
		{
			if (source.category == "attribute")
			{
				return source.tag.Contains("primary");
			}
			return false;
		}
	}

	public Act act => (this as Act) ?? ACT.Create(id);

	public static string GetName(string alias)
	{
		return EClass.sources.elements.alias[alias].GetName();
	}

	public static SourceElement.Row Get(int id)
	{
		return EClass.sources.elements.map[id];
	}

	public virtual SourceElement.Row GetSource()
	{
		return EClass.sources.elements.alias.TryGetValue(GetType().ToString());
	}

	public virtual int GetSourcePotential(int v)
	{
		return 0;
	}

	public virtual Sprite GetSprite()
	{
		return null;
	}

	public int GetMaterialSourceValue(Thing t, int v)
	{
		if (id == 2)
		{
			return v;
		}
		if (IsTrait)
		{
			if (t.IsFurniture)
			{
				return v;
			}
			return Mathf.Min(v + t.encLV * 10, 60);
		}
		return v * (100 + t.encLV * 10) / 100;
	}

	public virtual int GetSourceValue(int v, int lv, SourceValueType type)
	{
		return type switch
		{
			SourceValueType.Chara => v * (100 + (lv - 1 + EClass.rnd(lv / 2 + 1)) * source.lvFactor / 10) / 100 + EClass.rnd(lv / 3) * source.lvFactor / 100, 
			SourceValueType.Fixed => v, 
			_ => v * ((source.encFactor == 0) ? 100 : (50 + EClass.rnd(100) + EClass.rnd((int)Mathf.Sqrt(lv * 100)) * source.encFactor / 100)) / 100, 
		};
	}

	public virtual Sprite GetIcon(string suffix = "")
	{
		return SpriteSheet.Get("Media/Graphics/Icon/Element/icon_elements", "ele_" + source.alias + suffix) ?? SpriteSheet.Get("Media/Graphics/Icon/Element/icon_elements", "ele_" + source.aliasParent + suffix) ?? SpriteSheet.Get("Media/Graphics/Icon/Element/icon_elements", "cat_" + source.category);
	}

	public int SortVal(bool charaSheet = false)
	{
		int num = ((source.sort != 0) ? source.sort : id);
		return (IsFlag ? 100000 : 0) + ((!charaSheet && IsGlobalElement) ? (-1000000) : 0) + num;
	}

	public virtual bool CanLink(ElementContainer owner)
	{
		return false;
	}

	public bool HasTag(string tag)
	{
		return source.tag.Contains(tag);
	}

	public void SetImage(Image i)
	{
		Sprite icon = GetIcon();
		if ((bool)icon)
		{
			i.sprite = icon;
			i.SetNativeSize();
		}
	}

	public virtual string GetDetail()
	{
		return source.GetDetail();
	}

	public bool IsFactionElement(Chara c)
	{
		if (c == null)
		{
			return false;
		}
		if (c.IsPCFaction)
		{
			foreach (Element value in EClass.pc.faction.charaElements.dict.Values)
			{
				if (value.id == id && value.Value > 0)
				{
					return true;
				}
			}
		}
		if (c.faithElements != null)
		{
			foreach (Element value2 in c.faithElements.dict.Values)
			{
				if (value2.id == id && value2.Value > 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	public Element GetParent(Card c)
	{
		if (!source.aliasParent.IsEmpty())
		{
			return c.elements.GetOrCreateElement(source.aliasParent);
		}
		return null;
	}

	public static Dictionary<int, int> GetElementMap(int[] list)
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		if (list != null)
		{
			for (int i = 0; i < list.Length / 2; i++)
			{
				dictionary[list[i * 2]] = list[i * 2 + 1];
			}
		}
		return dictionary;
	}

	public static Dictionary<int, int> GetElementMap(int[] list, Dictionary<int, int> map)
	{
		if (list != null)
		{
			for (int i = 0; i < list.Length / 2; i++)
			{
				map[list[i * 2]] = list[i * 2 + 1];
			}
		}
		return map;
	}

	public static SourceElement.Row GetRandomElement(int lv = 1)
	{
		if (ListElements.Count == 0)
		{
			foreach (SourceElement.Row row in EClass.sources.elements.rows)
			{
				if (row.categorySub == "eleAttack" && row.chance > 0)
				{
					ListElements.Add(row);
				}
			}
		}
		List<Tuple<SourceElement.Row, int>> list = new List<Tuple<SourceElement.Row, int>>();
		foreach (SourceElement.Row listElement in ListElements)
		{
			int num = 40 * (listElement.eleP - 100) / 100;
			if (list.Count == 0 || num < lv)
			{
				list.Add(new Tuple<SourceElement.Row, int>(listElement, num));
			}
		}
		return list.RandomItemWeighted((Tuple<SourceElement.Row, int> a) => 10000 / (100 + (lv - a.Item2) * 25)).Item1;
	}

	public void WriteNote(UINote n, ElementContainer owner = null, Action<UINote> onWriteNote = null)
	{
		n.Clear();
		_WriteNote(n, owner, onWriteNote, isRef: false);
		n.Build();
	}

	public void WriteNoteWithRef(UINote n, ElementContainer owner, Action<UINote> onWriteNote, Element refEle)
	{
		n.Clear();
		_WriteNote(n, owner, onWriteNote, isRef: false);
		refEle._WriteNote(n, owner, onWriteNote, isRef: true);
		n.Build();
	}

	public void _WriteNote(UINote n, Chara c, Act act)
	{
		Element e = c.elements.GetOrCreateElement(act.source.id);
		Act.Cost cost = e.GetCost(c);
		int p = e.GetPower(c);
		n.Space(6);
		string text = source.GetText("textExtra");
		if (!text.IsEmpty())
		{
			string[] array = text.Split(',');
			foreach (string text2 in array)
			{
				if (text2.StartsWith("@"))
				{
					Condition condition = Condition.Create(text2.Replace("@", ""), p);
					condition.owner = c;
					if (!source.aliasRef.IsEmpty())
					{
						condition.SetElement(EClass.sources.elements.alias[source.aliasRef].id);
					}
					switch (act.id)
					{
					case 6902:
						condition.SetRefVal(79, 266);
						break;
					case 8510:
					case 8710:
						condition.SetRefVal(79, (act.id == 8710) ? 221 : 220);
						break;
					}
					n.AddText("_bullet".lang() + "hintCon".lang(condition.Name, condition.EvaluateTurn(p).ToString() ?? ""));
					condition._WriteNote(n, asChild: true);
				}
				else
				{
					string text3 = text2.Replace("#calc", Calc());
					if (!source.aliasRef.IsEmpty())
					{
						text3 = text3.Replace("#ele", EClass.sources.elements.alias[source.aliasRef].GetName().ToLower());
					}
					n.AddText("_bullet".lang() + text3);
				}
			}
		}
		if (source.tag.Contains("syncRide"))
		{
			n.AddText("_bullet".lang() + "hintSyncRide".lang());
		}
		if (act.HaveLongPressAction)
		{
			int i = id;
			if (i != 8230 && i != 8232)
			{
				n.AddText("_bullet".lang() + "hintPartyAbility".lang());
			}
		}
		if (!act.LocalAct)
		{
			n.Space();
			n.AddText("isGlobalAct".lang());
		}
		if (cost.type != 0 && cost.cost != 0)
		{
			n.Space(4);
			UIItem uIItem = n.AddExtra<UIItem>("costPrice");
			uIItem.text1.SetText(cost.cost.ToString() ?? "", (((cost.type == Act.CostType.MP) ? c.mana.value : c.stamina.value) >= cost.cost) ? FontColor.Good : FontColor.Bad);
			uIItem.image1.sprite = ((cost.type == Act.CostType.MP) ? EClass.core.refs.icons.mana : EClass.core.refs.icons.stamina);
			uIItem.image1.SetNativeSize();
		}
		string Calc()
		{
			Dice dice = Dice.Create(e, c);
			if (dice == null)
			{
				return p.ToString() ?? "";
			}
			return dice.ToString();
		}
	}

	public void _WriteNote(UINote n, ElementContainer owner, Action<UINote> onWriteNote, bool isRef, bool addHeader = true)
	{
		if (addHeader)
		{
			if (isRef)
			{
				UIText.globalSizeMod = -2;
				n.AddHeader("prevElement".lang(FullName));
			}
			else
			{
				n.AddHeader(FullName.ToTitleCase(wholeText: true));
			}
		}
		string detail = GetDetail();
		if (!detail.IsEmpty())
		{
			n.AddText("NoteText_flavor_element", detail);
			n.Space(6);
		}
		int num = vLink;
		if (owner.Chara != null && owner.Chara.IsPCFaction)
		{
			num += EClass.pc.faction.charaElements.Value(id);
		}
		bool flag = ShowValue;
		bool flag2 = ShowRelativeAttribute;
		if (source.category == "landfeat")
		{
			flag = false;
			flag2 = false;
		}
		if (this is Act)
		{
			Act act = ACT.Create(source.id);
			UIItem uIItem = n.AddItem("ItemAbility");
			uIItem.text1.text = "vValue".lang(DisplayValue.ToString() ?? "", ValueWithoutLink + ((num == 0) ? "" : ((num > 0) ? (" + " + num) : (" - " + -num))));
			uIItem.text2.text = act.TargetType.ToString().lang();
			uIItem.text3.text = ((this is Spell) ? (owner.Chara.CalcCastingChance(owner.GetOrCreateElement(act.source.id)) + "%") : "-") ?? "";
		}
		else if (flag)
		{
			n.AddTopic("TopicLeft", "vCurrent".lang(), "vValue".lang(DisplayValue.ToString() ?? "", ValueWithoutLink + ((num == 0) ? "" : ((num > 0) ? (" + " + num) : (" - " + -num)))));
			if (ShowPotential)
			{
				num = vTempPotential;
				n.AddTopic("TopicLeft", "vPotential".lang(), "vValue".lang(Potential.ToString() ?? "", vPotential + vSourcePotential + MinPotential + ((num == 0) ? "" : ((num > 0) ? (" + " + num) : (" - " + -num)))));
			}
			_ = PotentialAsStock;
		}
		if (flag2 && !source.aliasParent.IsEmpty())
		{
			Element element = Create(source.aliasParent);
			UIItem uIItem2 = n.AddItem("ItemRelativeAttribute");
			uIItem2.text1.SetText(element.Name);
			element.SetImage(uIItem2.image1);
			bool flag3 = source.lvFactor > 0 && this is Act;
			uIItem2.text2.SetActive(flag3);
			uIItem2.text3.SetActive(flag3);
			if (flag3)
			{
				uIItem2.text2.SetText(GetPower(EClass.pc).ToString() ?? "");
			}
		}
		OnWriteNote(n, owner);
		if (EClass.debug.showExtra)
		{
			n.AddTopic("TopicLeft", "Class:", GetType()?.ToString() ?? "");
			n.AddTopic("TopicLeft", "vExp".lang(), vExp.ToString() ?? "");
			n.AddTopic("TopicLeft", "vSource", vSource.ToString() ?? "");
			n.AddTopic("TopicLeft", "vSourcePotential", vSourcePotential.ToString() ?? "");
			n.AddTopic("TopicLeft", "vPotential", vPotential.ToString() ?? "");
			n.AddTopic("TopicLeft", "Potential", Potential.ToString() ?? "");
		}
		CheckLevelBonus(owner, n);
		onWriteNote?.Invoke(n);
		if (ShowBonuses && owner.Chara != null)
		{
			BonusInfo bonusInfo = new BonusInfo();
			bonusInfo.ele = this;
			bonusInfo.n = n;
			bonusInfo.c = owner.Chara;
			bonusInfo.WriteNote();
		}
		UIText.globalSizeMod = 0;
	}

	public virtual void OnWriteNote(UINote n, ElementContainer owner)
	{
	}

	public virtual void OnChangeValue()
	{
	}

	public void CheckLevelBonus(ElementContainer owner, UINote n = null)
	{
		if (owner == null || source.levelBonus.IsEmpty())
		{
			return;
		}
		bool flag = n == null;
		string[] array = (source.GetText("levelBonus", returnNull: true) ?? source.levelBonus).Split(Environment.NewLine.ToCharArray());
		if (!flag)
		{
			n.Space(10);
		}
		string[] array2 = array;
		foreach (string obj in array2)
		{
			string[] array3 = obj.Split(',');
			int lv = array3[0].ToInt();
			SourceElement.Row row = (EClass.sources.elements.alias.ContainsKey(array3[1]) ? EClass.sources.elements.alias[array3[1]] : null);
			if (flag)
			{
				if (lv > ValueWithoutLink)
				{
					break;
				}
				if (row != null && !owner.Has(row.id) && owner is ElementContainerZone)
				{
					owner.Learn(row.id);
				}
			}
			else
			{
				string s = ((row != null) ? row.GetName() : array3[1]);
				n.AddText(("  Lv " + lv).TagColor(FontColor.Topic) + "  " + s.TagColorGoodBad(() => (row != null) ? owner.Has(row.id) : (lv <= ValueWithoutLink), () => false));
			}
		}
		if (!flag)
		{
			n.Space(4);
		}
	}

	public int GetSortVal(UIList.SortMode m)
	{
		switch (m)
		{
		case UIList.SortMode.ByCategory:
			return source.id + ((!source.aliasParent.IsEmpty()) ? (EClass.sources.elements.alias[source.aliasParent].id * 10000) : 0);
		case UIList.SortMode.ByNumber:
			return -vPotential * 10000 + source.id;
		case UIList.SortMode.ByElementParent:
			if (!source.aliasParent.IsEmpty())
			{
				return EClass.sources.elements.alias[source.aliasParent].sort;
			}
			return id;
		default:
			return source.id;
		}
	}

	public virtual Act.Cost GetCost(Chara c)
	{
		if (source.cost[0] == 0)
		{
			Act.Cost result = default(Act.Cost);
			result.type = Act.CostType.None;
			return result;
		}
		Act.Cost result2 = default(Act.Cost);
		if (this is Spell)
		{
			result2.type = Act.CostType.MP;
			int num = EClass.curve(Value, 50, 10);
			result2.cost = source.cost[0] * (100 + ((!source.tag.Contains("noCostInc")) ? (num * 3) : 0)) / 100;
		}
		else
		{
			result2.type = Act.CostType.SP;
			result2.cost = source.cost[0];
			if (source.id == 6020)
			{
				result2.cost = c.stamina.max / 3 + 10;
			}
		}
		if (!c.IsPC && result2.cost > 2)
		{
			result2.cost /= 2;
		}
		return result2;
	}

	public virtual int GetPower(Card c)
	{
		return 100;
	}

	public virtual void SetTextValue(UIText text)
	{
		string text2 = DisplayValue.ToString() ?? "";
		if (ShowXP)
		{
			text2 += ".".TagSize((vExp / 10).ToString("D2") ?? "", 11);
		}
		if (vLink != 0)
		{
			string text3 = ((vLink > 0) ? "+" : "") + vLink;
			text2 = "<color=" + ((DisplayValue > ValueWithoutLink) ? SkinManager.CurrentColors.textGood : SkinManager.CurrentColors.textBad).ToHex() + ">" + text2 + (" (" + text3 + ")").TagSize(13) + "</color>";
		}
		text.text = text2;
	}

	public static Element Create(int id, int v = 0)
	{
		SourceElement.Row row = EClass.sources.elements.map.TryGetValue(id);
		if (row == null)
		{
			return null;
		}
		Element element = ClassCache.Create<Element>(row.type.IsEmpty("Element"), "Elin");
		element.id = id;
		element.vBase = v;
		element._source = row;
		return element;
	}

	public static Element Create(string id, int v = 1)
	{
		return Create(EClass.sources.elements.alias[id].id, v);
	}

	public static int GetId(string alias)
	{
		return EClass.sources.elements.alias[alias].id;
	}

	public static int GetResistLv(int v)
	{
		int num = v / 5;
		if (num < -2)
		{
			num = -2;
		}
		return num;
	}

	public static int GetResistDamage(int dmg, int v)
	{
		int resistLv = GetResistLv(v);
		if (resistLv >= 4)
		{
			return 0;
		}
		return resistLv switch
		{
			3 => dmg / 4, 
			2 => dmg / 3, 
			1 => dmg / 2, 
			-1 => dmg * 3 / 2, 
			-2 => dmg * 2, 
			_ => dmg, 
		};
	}
}
