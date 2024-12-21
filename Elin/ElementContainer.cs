using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class ElementContainer : EClass
{
	public enum NoteMode
	{
		Default,
		CharaMake,
		CharaMakeAttributes,
		Domain,
		Trait
	}

	public Dictionary<int, Element> dict = new Dictionary<int, Element>();

	public ElementContainer parent;

	public const int sizeElement = 5;

	[JsonProperty(PropertyName = "A")]
	public List<int> list;

	public virtual Card Card => null;

	public virtual Chara Chara => null;

	public virtual bool IsMeleeWeapon => false;

	[OnSerializing]
	private void OnSerializing(StreamingContext context)
	{
		list = new List<int>();
		foreach (Element value in dict.Values)
		{
			if (value.vBase != 0 || value.vExp != 0 || value.vPotential != 0 || value.vTempPotential != 0)
			{
				list.AddRange(new int[5] { value.id, value.vBase, value.vExp, value.vPotential, value.vTempPotential });
			}
		}
		if (list.Count == 0)
		{
			list = null;
		}
	}

	[OnDeserialized]
	private void OnDeserialized(StreamingContext context)
	{
		if (list == null)
		{
			return;
		}
		for (int i = 0; i < list.Count; i += 5)
		{
			Element orCreateElement = GetOrCreateElement(list[i]);
			if (orCreateElement != null)
			{
				orCreateElement.vBase += list[i + 1];
				orCreateElement.vExp += list[i + 2];
				orCreateElement.vPotential += list[i + 3];
				orCreateElement.vTempPotential = list[i + 4];
				orCreateElement.owner = this;
			}
		}
	}

	public void ApplyElementMap(int uid, SourceValueType type, Dictionary<int, int> map, int lv, bool invert = false, bool applyFeat = false)
	{
		int num = ((!invert) ? 1 : (-1));
		Rand.SetSeed(uid);
		foreach (KeyValuePair<int, int> item in map)
		{
			Element orCreateElement = GetOrCreateElement(item.Key);
			int value = item.Value;
			if (value != 0)
			{
				if (orCreateElement.source.category == "skill")
				{
					orCreateElement.vSourcePotential += orCreateElement.GetSourcePotential(value) * num;
				}
				int num2 = orCreateElement.GetSourceValue(value, lv, type) * num;
				orCreateElement.vSource += num2;
				if (applyFeat && orCreateElement is Feat)
				{
					(orCreateElement as Feat).Apply(num2, this);
				}
			}
		}
		Rand.SetSeed();
	}

	public void ApplyMaterialElementMap(Thing t, bool invert = false)
	{
		int num = ((!invert) ? 1 : (-1));
		SourceMaterial.Row material = t.material;
		Rand.SetSeed(t.uid);
		foreach (KeyValuePair<int, int> item in material.elementMap)
		{
			int value = item.Value;
			if (value == 0)
			{
				continue;
			}
			Element orCreateElement = GetOrCreateElement(item.Key);
			if (!orCreateElement.source.IsEncAppliable(t))
			{
				if (orCreateElement.vBase == 0 && orCreateElement.vSource == 0 && orCreateElement.vLink == 0 && orCreateElement.vExp == 0 && orCreateElement.vPotential == 0)
				{
					Remove(orCreateElement.id);
				}
				continue;
			}
			int num2 = orCreateElement.GetMaterialSourceValue(t, value) * num;
			orCreateElement.vSource += num2;
			if (orCreateElement.vBase == 0 && orCreateElement.vSource == 0 && orCreateElement.vLink == 0 && orCreateElement.vExp == 0 && orCreateElement.vPotential == 0)
			{
				Remove(orCreateElement.id);
			}
		}
		Rand.SetSeed();
	}

	public void ImportElementMap(Dictionary<int, int> map)
	{
		foreach (KeyValuePair<int, int> item in map)
		{
			GetOrCreateElement(item.Key).vSource += item.Value;
		}
	}

	public ElementContainer ImportElementMap(int[] ints)
	{
		for (int i = 0; i < ints.Length; i += 2)
		{
			GetOrCreateElement(ints[i]).vSource += ints[i + 1];
		}
		return this;
	}

	public void ApplyPotential(int mode = 0)
	{
		foreach (Element value in dict.Values)
		{
			if (value.HasTag("primary"))
			{
				value.vTempPotential = (value.ValueWithoutLink - ((mode != 2) ? 7 : 0)) * 5;
			}
		}
	}

	public int Value(int ele)
	{
		Element element = GetElement(ele);
		if (element == null)
		{
			if (EClass.core.game == null)
			{
				return 0;
			}
			if (Card == null || !Card.IsPCFactionOrMinion)
			{
				return 0;
			}
			if (ele != 78)
			{
				return EClass.pc.faction.charaElements.Value(ele);
			}
			return GetOrCreateElement(ele).Value;
		}
		return element.Value;
	}

	public virtual int ValueBonus(Element e)
	{
		return 0;
	}

	public int ValueWithoutLink(int ele)
	{
		return GetElement(ele)?.ValueWithoutLink ?? 0;
	}

	public int ValueWithoutLink(string alias)
	{
		return GetElement(alias)?.ValueWithoutLink ?? 0;
	}

	public int GetFeatRef(int ele, int idx = 0)
	{
		if (!(GetElement(ele) is Feat feat))
		{
			return 0;
		}
		feat.Apply(feat.Value, this);
		return Feat.featRef[idx].ToInt();
	}

	public int Exp(int ele)
	{
		return GetElement(ele)?.vExp ?? 0;
	}

	public bool Has(int ele)
	{
		Element element = GetElement(ele);
		if (element == null)
		{
			return false;
		}
		return element.Value > 0;
	}

	public bool Has(SourceElement.Row row)
	{
		return Has(row.id);
	}

	public bool Has(string alias)
	{
		return Has(EClass.sources.elements.alias[alias].id);
	}

	public bool HasBase(int ele)
	{
		Element element = GetElement(ele);
		if (element == null)
		{
			return false;
		}
		int num = element.ValueWithoutLink;
		switch (ele)
		{
		case 300:
			num += Value(1516) * -4;
			num += Value(1517) * 4;
			break;
		case 307:
			num += Value(1524) * -4;
			num += Value(1525) * 4;
			break;
		}
		return num != 0;
	}

	public int Base(int ele)
	{
		return GetElement(ele)?.ValueWithoutLink ?? 0;
	}

	public void Learn(int ele, int v = 1)
	{
		ModBase(ele, v);
		OnLearn(ele);
	}

	public void Train(int ele, int a = 10)
	{
		OnTrain(ele);
		ModTempPotential(ele, a);
	}

	public void ModExp(int ele, int a, bool chain = false)
	{
		if ((Card != null && Card.isChara && Card.Chara.isDead) || a == 0)
		{
			return;
		}
		Element element = GetElement(ele);
		if (element == null || !element.CanGainExp)
		{
			return;
		}
		int value = (element.UsePotential ? element.Potential : 100);
		if (element.UseExpMod)
		{
			a = a * Mathf.Clamp(value, 10, 1000) / (100 + Mathf.Max(0, element.ValueWithoutLink) * 25);
			if (a >= 0 && EClass.rnd(element.ValueWithoutLink + 1) < 10)
			{
				a++;
			}
		}
		element.vExp += a;
		if (!chain && element.source.parentFactor > 0f && Card != null && !element.source.aliasParent.IsEmpty())
		{
			Element element2 = element.GetParent(Card);
			if (element2.CanGainExp)
			{
				ModExp(element2.id, (int)Math.Max(1f, (float)a * element.source.parentFactor / 100f), chain: true);
			}
		}
		if (element.vExp >= element.ExpToNext)
		{
			int num = element.vExp - element.ExpToNext;
			int vBase = element.vBase;
			ModBase(ele, 1);
			OnLevelUp(element, vBase);
			element.vExp = Mathf.Clamp(num / 2, 0, element.ExpToNext / 2);
			if (element.vTempPotential > 0)
			{
				element.vTempPotential -= element.vTempPotential / 4 + EClass.rnd(5) + 5;
				if (element.vTempPotential < 0)
				{
					element.vTempPotential = 0;
				}
			}
			else if (element.vTempPotential < 0)
			{
				element.vTempPotential += -element.vTempPotential / 4 + EClass.rnd(5) + 5;
				if (element.vTempPotential > 0)
				{
					element.vTempPotential = 0;
				}
			}
		}
		else if (element.vExp < 0)
		{
			if (element.ValueWithoutLink <= 1)
			{
				element.vExp = 0;
				return;
			}
			int vBase2 = element.vBase;
			ModBase(ele, -1);
			OnLevelDown(element, vBase2);
			element.vExp = Mathf.Max(element.ExpToNext / 2, element.ExpToNext + element.vExp);
		}
	}

	public virtual void OnLearn(int ele)
	{
	}

	public virtual void OnTrain(int ele)
	{
	}

	public virtual void OnLevelUp(Element e, int lastValue)
	{
	}

	public virtual void OnLevelDown(Element e, int lastValue)
	{
	}

	public Element SetBase(string alias, int v, int potential = 0)
	{
		return SetBase(EClass.sources.elements.alias[alias].id, v, potential);
	}

	public Element SetBase(int id, int v, int potential = 0)
	{
		Element orCreateElement = GetOrCreateElement(id);
		if (parent != null && orCreateElement.CanLink(this))
		{
			parent.ModLink(id, -orCreateElement.vBase + v);
		}
		orCreateElement.vBase = v;
		orCreateElement.vExp = 0;
		orCreateElement.vPotential = potential;
		orCreateElement.OnChangeValue();
		if (orCreateElement.vBase == 0 && orCreateElement.vSource == 0 && orCreateElement.vLink == 0 && orCreateElement.vPotential == 0 && orCreateElement.vExp == 0)
		{
			Remove(orCreateElement.id);
		}
		return orCreateElement;
	}

	public void SetTo(int id, int v)
	{
		Element orCreateElement = GetOrCreateElement(id);
		int num = v - (orCreateElement.vBase + orCreateElement.vSource);
		if (num != 0)
		{
			ModBase(id, num);
		}
		if (orCreateElement.vBase == 0 && orCreateElement.vSource == 0 && orCreateElement.vLink == 0 && orCreateElement.vPotential == 0 && orCreateElement.vExp == 0)
		{
			Remove(orCreateElement.id);
		}
	}

	public void Remove(int id)
	{
		Element element = GetElement(id);
		if (element != null)
		{
			if (parent != null && element.CanLink(this))
			{
				parent.ModLink(id, -element.Value);
			}
			dict.Remove(id);
		}
	}

	public Element ModBase(int ele, int v)
	{
		Element orCreateElement = GetOrCreateElement(ele);
		orCreateElement.vBase += v;
		if (parent != null && orCreateElement.CanLink(this))
		{
			parent.ModLink(ele, v);
		}
		orCreateElement.CheckLevelBonus(this);
		orCreateElement.OnChangeValue();
		if (orCreateElement.vBase == 0 && orCreateElement.vSource == 0 && orCreateElement.vLink == 0 && orCreateElement.vPotential == 0 && orCreateElement.vExp == 0)
		{
			Remove(orCreateElement.id);
		}
		return orCreateElement;
	}

	public virtual void OnChangeValue()
	{
	}

	public Element ModPotential(int ele, int v)
	{
		Element orCreateElement = GetOrCreateElement(ele);
		orCreateElement.vPotential += v;
		if (orCreateElement.vPotential > 1000)
		{
			orCreateElement.vPotential = 1000;
		}
		return orCreateElement;
	}

	public Element ModTempPotential(int ele, int v, int threshMsg = 0)
	{
		Element orCreateElement = GetOrCreateElement(ele);
		orCreateElement.vTempPotential += v;
		if (orCreateElement.vTempPotential > 1000)
		{
			orCreateElement.vTempPotential = 1000;
		}
		OnModTempPotential(orCreateElement, v, threshMsg);
		return orCreateElement;
	}

	public virtual void OnModTempPotential(Element e, int v, int threshMsg)
	{
	}

	private Element ModLink(int id, int v)
	{
		Element orCreateElement = GetOrCreateElement(id);
		orCreateElement.vLink += v;
		orCreateElement.OnChangeValue();
		if (parent != null && orCreateElement.CanLink(this))
		{
			parent.ModLink(id, v);
		}
		return orCreateElement;
	}

	public int GetSpellExp(Chara c, Element e, int costMod = 100)
	{
		Act.Cost cost = e.GetCost(c);
		int num = cost.cost * ((cost.type == Act.CostType.SP) ? 20 : 5) * (100 + c.Evalue(1208) * 30) / 100 + 10;
		num = num * costMod / 100;
		if (!e.source.aliasParent.IsEmpty())
		{
			int num2 = ValueWithoutLink(e.source.aliasParent) - ValueWithoutLink(e.source.id);
			num = ((num2 < 0) ? (num * 100 / (100 - num2 * 25)) : (num * (100 + num2 * 5) / 100));
		}
		if (num < 0)
		{
			num = 0;
		}
		return num;
	}

	public Element GetElement(string alias)
	{
		return GetElement(EClass.sources.elements.alias.TryGetValue(alias)?.id ?? 0);
	}

	public Element GetElement(int id)
	{
		return dict.TryGetValue(id);
	}

	public Element CreateElement(int id)
	{
		Element element = Element.Create(id);
		if (element == null)
		{
			return null;
		}
		element.owner = this;
		dict.Add(id, element);
		return element;
	}

	public Element GetOrCreateElement(Element ele)
	{
		return GetOrCreateElement(ele.id);
	}

	public Element GetOrCreateElement(string alias)
	{
		return GetOrCreateElement(EClass.sources.elements.alias[alias].id);
	}

	public Element GetOrCreateElement(int id)
	{
		Element value = null;
		if (!dict.TryGetValue(id, out value))
		{
			value = CreateElement(id);
		}
		return value;
	}

	public void SetParent(Card c)
	{
		SetParent(c?.elements);
	}

	public void SetParent(ElementContainer newParent = null)
	{
		if (parent != null)
		{
			foreach (Element value in dict.Values)
			{
				if (value.CanLink(this))
				{
					parent.ModLink(value.id, -(value.vBase + value.vSource));
				}
			}
		}
		if (newParent != null)
		{
			foreach (Element value2 in dict.Values)
			{
				if (value2.CanLink(this))
				{
					newParent.ModLink(value2.id, value2.vBase + value2.vSource);
				}
			}
		}
		parent = newParent;
	}

	public List<Element> ListElements(Func<Element, bool> shoudList = null, Comparison<Element> comparison = null)
	{
		List<Element> list = new List<Element>();
		List<Element> eles = dict.Values.ToList();
		if (Card != null && Card.Chara != null)
		{
			if (Card.Chara.IsPCFaction)
			{
				AddElements(EClass.pc.faction.charaElements, isGlobal: true);
			}
			AddElements(Card.Chara.faithElements, isGlobal: false);
		}
		foreach (Element item2 in eles)
		{
			if (shoudList == null || shoudList(item2))
			{
				list.Add(item2);
			}
		}
		if (comparison != null)
		{
			list.Sort(comparison);
		}
		return list;
		void AddElements(ElementContainer container, bool isGlobal)
		{
			if (container == null)
			{
				return;
			}
			foreach (Element value in container.dict.Values)
			{
				bool flag = true;
				foreach (Element item3 in eles)
				{
					if (value.id == item3.id)
					{
						flag = false;
						break;
					}
				}
				if (flag && value.Value != 0)
				{
					if (isGlobal)
					{
						Element item = Card.Chara.elements.CreateElement(value.id);
						eles.Add(item);
					}
					else
					{
						eles.Add(value);
					}
				}
			}
		}
	}

	public List<Element> ListBestAttributes()
	{
		List<Element> obj = ListElements((Element a) => a.HasTag("primary"));
		obj.Sort((Element a, Element b) => (b.ValueWithoutLink - a.ValueWithoutLink) * 100000 + a.id - b.id);
		return obj;
	}

	public List<Element> ListBestSkills()
	{
		List<Element> obj = ListElements((Element a) => a.source.category == "skill");
		obj.Sort((Element a, Element b) => (b.ValueWithoutLink - a.ValueWithoutLink) * 100000 + a.id - b.id);
		return obj;
	}

	public List<Element> ListGeneFeats()
	{
		return ListElements((Element a) => a.Value > 0 && a.source.category == "feat" && a.source.cost.Length != 0 && a.source.cost[0] > 0);
	}

	public List<Element> ListLearnable(Chara c)
	{
		List<Element> list = new List<Element>();
		foreach (KeyValuePair<int, Element> item in c.elements.dict)
		{
			if (!dict.ContainsKey(item.Key))
			{
				list.Add(item.Value);
			}
		}
		return list;
	}

	public void CopyTo(ElementContainer container)
	{
		container.dict.Clear();
		foreach (KeyValuePair<int, Element> item in dict)
		{
			Element element = container.CreateElement(item.Key);
			element.vBase = item.Value.vBase;
			element.vExp = item.Value.vExp;
			element.vSource = item.Value.vSource;
		}
	}

	public static int GetSortVal(Element a)
	{
		int num = a.Value;
		if (a.source.textAlt.Length <= 2 || a.Value < 0)
		{
			num -= 100000;
		}
		if (a.id == 2)
		{
			num += 20000;
		}
		if (a.IsFoodTraitMain)
		{
			num += 10000;
		}
		return num;
	}

	public void AddNote(UINote n, Func<Element, bool> isValid = null, Action onAdd = null, NoteMode mode = NoteMode.Default, bool addRaceFeat = false, Func<Element, string, string> funcText = null, Action<UINote, Element> onAddNote = null)
	{
		List<Element> list = new List<Element>();
		foreach (Element value2 in dict.Values)
		{
			if ((isValid == null || isValid(value2)) && (mode != NoteMode.CharaMake || value2.ValueWithoutLink != 0) && (value2.Value != 0 || mode == NoteMode.CharaMakeAttributes) && (!value2.HasTag("hidden") || EClass.debug.showExtra))
			{
				list.Add(value2);
			}
		}
		if (addRaceFeat)
		{
			Element element = Element.Create(29, 1);
			element.owner = this;
			list.Add(element);
		}
		if (list.Count == 0)
		{
			return;
		}
		onAdd?.Invoke();
		switch (mode)
		{
		case NoteMode.CharaMake:
		case NoteMode.CharaMakeAttributes:
			list.Sort((Element a, Element b) => a.GetSortVal(UIList.SortMode.ByElementParent) - b.GetSortVal(UIList.SortMode.ByElementParent));
			break;
		case NoteMode.Trait:
			list.Sort((Element a, Element b) => GetSortVal(b) - GetSortVal(a));
			break;
		default:
			list.Sort((Element a, Element b) => a.SortVal() - b.SortVal());
			break;
		}
		string text = "";
		foreach (Element e in list)
		{
			switch (mode)
			{
			case NoteMode.Domain:
				n.AddText(e.Name, FontColor.Default);
				continue;
			case NoteMode.Default:
			case NoteMode.Trait:
			{
				bool flag = e.source.tag.Contains("common");
				string categorySub = e.source.categorySub;
				bool flag2 = false;
				bool flag3 = (e.source.tag.Contains("neg") ? (e.Value > 0) : (e.Value < 0));
				int num = Mathf.Abs(e.Value);
				bool flag4 = Card != null && Card.ShowFoodEnc;
				bool flag5 = Card != null && Card.IsWeapon && e is Ability;
				if (e.IsTrait || (flag4 && e.IsFoodTrait))
				{
					string[] textArray = e.source.GetTextArray("textAlt");
					int num2 = Mathf.Clamp(e.Value / 10 + 1, (e.Value < 0 || textArray.Length <= 2) ? 1 : 2, textArray.Length - 1);
					text = "altEnc".lang(textArray[0].IsEmpty(e.Name), textArray[num2], EClass.debug.showExtra ? (e.Value + " " + e.Name) : "");
					flag3 = num2 <= 1 || textArray.Length <= 2;
					flag2 = true;
				}
				else if (flag5)
				{
					text = "isProc".lang(e.Name);
					flag3 = false;
				}
				else if (categorySub == "resist")
				{
					text = ("isResist" + (flag3 ? "Neg" : "")).lang(e.Name);
				}
				else if (categorySub == "eleAttack")
				{
					text = "isEleAttack".lang(e.Name);
				}
				else if (!e.source.textPhase.IsEmpty() && e.Value > 0)
				{
					text = e.source.GetText("textPhase");
				}
				else
				{
					string name = e.Name;
					bool flag6 = e.source.category == "skill" || (e.source.category == "attribute" && !e.source.textPhase.IsEmpty());
					bool flag7 = e.source.category == "enchant";
					if (e.source.tag.Contains("multiplier"))
					{
						flag6 = (flag7 = false);
						name = EClass.sources.elements.alias[e.source.aliasRef].GetName();
					}
					flag2 = !(flag6 || flag7);
					text = (flag6 ? "textEncSkill" : (flag7 ? "textEncEnc" : "textEnc")).lang(name, num + (e.source.tag.Contains("ratio") ? "%" : ""), ((e.Value > 0) ? "encIncrease" : "encDecrease").lang());
				}
				int num3 = ((!(e is Resistance)) ? 1 : 0);
				if (!flag && !flag2 && !e.source.tag.Contains("flag"))
				{
					text = text + " [" + "*".Repeat(Mathf.Clamp(num * e.source.mtp / 5 + num3, 1, 5)) + ((num * e.source.mtp / 5 + num3 > 5) ? "+" : "") + "]";
				}
				if (e.HasTag("hidden"))
				{
					text = "(debug)" + text;
				}
				FontColor color = (flag ? FontColor.Default : (flag3 ? FontColor.Bad : FontColor.Good));
				if (e.IsGlobalElement)
				{
					text = text + " " + (e.IsFactionWideElement ? "_factionWide" : "_partyWide").lang();
					if (Card != null && !Card.c_idDeity.IsEmpty() && Card.c_idDeity != EClass.pc.idFaith)
					{
						continue;
					}
					color = FontColor.Myth;
				}
				if (flag4 && e.IsFoodTrait && !e.IsFoodTraitMain)
				{
					color = FontColor.FoodMisc;
				}
				if (e.id == 2 && e.Value >= 0)
				{
					color = FontColor.FoodQuality;
				}
				if (funcText != null)
				{
					text = funcText(e, text);
				}
				n.AddText("NoteText_prefwidth", text, color);
				onAddNote?.Invoke(n, e);
				continue;
			}
			}
			UIItem uIItem = n.AddTopic("TopicAttribute", e.Name, "".TagColor((e.ValueWithoutLink > 0) ? SkinManager.CurrentColors.textGood : SkinManager.CurrentColors.textBad, e.ValueWithoutLink.ToString() ?? ""));
			if ((bool)uIItem.button1)
			{
				uIItem.button1.tooltip.onShowTooltip = delegate(UITooltip t)
				{
					e.WriteNote(t.note, EClass.pc.elements);
				};
			}
			e.SetImage(uIItem.image1);
			Image image = uIItem.image2;
			int value = (e.Potential - 80) / 20;
			image.enabled = e.Potential != 80;
			image.sprite = EClass.core.refs.spritesPotential[Mathf.Clamp(Mathf.Abs(value), 0, EClass.core.refs.spritesPotential.Count - 1)];
			image.color = ((e.Potential - 80 >= 0) ? Color.white : new Color(1f, 0.7f, 0.7f));
		}
	}

	public void AddNoteAll(UINote n)
	{
		Transform transform = n.AddExtra<Transform>("noteRace");
		UINote n2 = transform.Find("note1").GetComponent<UINote>();
		UINote n3 = transform.Find("note2").GetComponent<UINote>();
		AddNote(n3, (Element e) => e.HasTag("primary"), delegate
		{
			n3.AddHeader("HeaderNoteSmall", "attributes");
		}, NoteMode.CharaMakeAttributes);
		AddNote(n2, (Element e) => e.source.category == "skill" && !e.HasTag("hidden") && e.ValueWithoutLink > 1 && e.source.categorySub != "weapon", delegate
		{
			n2.AddHeader("HeaderNoteSmall", "skills");
		}, NoteMode.CharaMake);
		AddNote(n2, (Element e) => e is Feat, delegate
		{
			n2.AddHeader("HeaderNoteSmall", "feats");
		}, NoteMode.CharaMake);
	}
}