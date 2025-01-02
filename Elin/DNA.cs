using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

public class DNA : EClass
{
	public enum Type
	{
		Inferior = 0,
		Default = 3,
		Superior = 5,
		Brain = 8
	}

	[JsonProperty]
	public string id;

	[JsonProperty]
	public int[] ints = new int[6];

	[JsonProperty]
	public List<int> vals = new List<int>();

	public BitArray32 bits;

	public Type type
	{
		get
		{
			return ints[0].ToEnum<Type>();
		}
		set
		{
			ints[0] = (int)value;
		}
	}

	public int cost
	{
		get
		{
			return ints[1];
		}
		set
		{
			ints[1] = value;
		}
	}

	public int lv
	{
		get
		{
			return ints[2];
		}
		set
		{
			ints[2] = value;
		}
	}

	public int seed
	{
		get
		{
			return ints[3];
		}
		set
		{
			ints[3] = value;
		}
	}

	public int slot
	{
		get
		{
			return ints[5];
		}
		set
		{
			ints[5] = value;
		}
	}

	[OnSerializing]
	private void _OnSerializing(StreamingContext context)
	{
		ints[4] = bits.ToInt();
	}

	[OnDeserialized]
	private void _OnDeserialized(StreamingContext context)
	{
		if (ints.Length < 6)
		{
			Array.Resize(ref ints, 6);
			slot = 1;
		}
	}

	public static Thing GenerateGene(Chara c, Type? type = null)
	{
		Thing thing = ThingGen.Create((type == Type.Brain) ? "gene_brain" : "gene");
		DNA dNA = new DNA();
		thing.MakeRefFrom(c);
		dNA.id = c.id;
		dNA.lv = c.LV;
		dNA.seed = EClass.rnd(20000);
		thing.c_DNA = dNA;
		dNA.GenerateWithGene(type ?? dNA.GetRandomType(), thing, c);
		return thing;
	}

	public static Thing GenerateGene(CardRow r, Type? type = null, int lv = -1, int seed = -1)
	{
		Thing thing = ThingGen.Create((type == Type.Brain) ? "gene_brain" : "gene");
		DNA dNA = new DNA();
		thing.MakeRefFrom(r.id);
		dNA.id = r.id;
		dNA.lv = ((lv == -1) ? r.LV : lv);
		dNA.seed = ((seed == -1) ? EClass.rnd(20000) : seed);
		thing.c_DNA = dNA;
		dNA.GenerateWithGene(type ?? dNA.GetRandomType(), thing);
		return thing;
	}

	public static Thing CopyDNA(Thing from, Thing to)
	{
		to.c_DNA = from.c_DNA;
		to.c_idRefCard = from.c_idRefCard;
		to.ChangeMaterial(from.material);
		return to;
	}

	public static Thing GenerateRandomGene(int lv = -1, int seed = -1)
	{
		if (lv == -1)
		{
			lv = EClass._zone.ContentLv;
		}
		Rand.SetSeed(seed);
		CardRow r = SpawnList.Get("chara").Select(100);
		Rand.SetSeed();
		return GenerateGene(r, Type.Superior, lv, seed);
	}

	public static Thing GenerateManiGene(Card owner)
	{
		owner.things.DestroyAll();
		Debug.Log("Mani:" + owner.c_seed);
		Rand.SetSeed(owner.c_seed);
		CardRow r = SpawnList.Get("chara").Select(100);
		Rand.SetSeed(owner.c_seed);
		Thing thing = GenerateGene(r, Type.Superior, owner.LV, owner.c_seed);
		thing.c_DNA.cost = thing.c_DNA.cost / 2;
		thing.MakeRefFrom("mani");
		Rand.SetSeed();
		owner.c_seed++;
		return thing;
	}

	public void Apply(Chara c)
	{
		if (c.c_genes == null)
		{
			c.c_genes = new CharaGenes();
		}
		CharaGenes c_genes = c.c_genes;
		if (type == Type.Inferior)
		{
			c.c_genes.inferior++;
			return;
		}
		c.feat -= cost;
		Apply(c, reverse: false);
		c_genes.items.Add(this);
	}

	public void Apply(Chara c, bool reverse)
	{
		if (type == Type.Brain)
		{
			c._tactics = null;
			return;
		}
		for (int i = 0; i < vals.Count; i += 2)
		{
			int num = vals[i];
			int num2 = vals[i + 1];
			SourceElement.Row row = EClass.sources.elements.map[num];
			switch (row.category)
			{
			case "slot":
				if (reverse)
				{
					c.body.RemoveBodyPart(row.id);
				}
				else
				{
					c.body.AddBodyPart(row.id);
				}
				c.body.RefreshBodyParts();
				break;
			case "feat":
				c.SetFeat(num, c.elements.ValueWithoutLink(num) + ((!reverse) ? 1 : (-1)), !reverse);
				break;
			case "ability":
				if (reverse)
				{
					c.ability.Remove(num * ((num2 >= 0) ? 1 : (-1)));
				}
				else
				{
					c.ability.Add(num, Mathf.Abs(num2), num2 < 0);
				}
				break;
			default:
				if (!reverse && c.elements.ValueWithoutLink(row.id) == 0)
				{
					c.elements.Learn(row.id, num2);
				}
				else
				{
					c.elements.ModBase(num, reverse ? (-num2) : num2);
				}
				break;
			}
		}
	}

	public bool CanRemove()
	{
		for (int i = 0; i < vals.Count; i += 2)
		{
			int num = vals[i];
			if (num == 1237 || num == 1415)
			{
				return false;
			}
		}
		return true;
	}

	public void GenerateWithGene(Type _type, Card gene, Chara model = null)
	{
		Generate(_type, model);
		gene.ChangeMaterial(GetMaterialId(type));
		gene.elements.SetTo(10, 0);
	}

	public void Generate(Type _type, Chara model = null)
	{
		type = _type;
		cost = 0;
		slot = 0;
		vals.Clear();
		Debug.Log(seed);
		Rand.SetSeed(seed);
		if (model == null)
		{
			model = CharaGen.Create(id);
			if (lv > model.LV)
			{
				model.SetLv(lv);
			}
		}
		if (type == Type.Inferior || model == null)
		{
			type = Type.Inferior;
			return;
		}
		if (type == Type.Brain)
		{
			cost = 4;
			return;
		}
		int body = 0;
		int action = 0;
		int feat = 0;
		int maxSlot = 1;
		List<Element> listAttb = model.elements.ListBestAttributes();
		List<Element> listSkill = model.elements.ListBestSkills();
		List<Element> listFeat = model.elements.ListGeneFeats();
		Rand.SetSeed(seed);
		if (listFeat.Count == 0)
		{
			listFeat = model.ListAvailabeFeats(pet: true);
		}
		Rand.SetSeed(seed);
		switch (type)
		{
		case Type.Default:
			AddRandom(EClass.rnd(EClass.rnd(4)) + 1);
			if (EClass.rnd(3) == 0 || vals.Count == 0)
			{
				AddSpecial();
			}
			break;
		case Type.Superior:
			AddRandom(EClass.rnd(EClass.rnd(4)) + 3);
			if (EClass.rnd(3) == 0)
			{
				AddSpecial();
			}
			AddSpecial();
			break;
		}
		if (vals.Count == 0)
		{
			for (int i = 0; i < 10; i++)
			{
				if (EClass.rnd(4) == 0)
				{
					AddSpecial();
				}
				else
				{
					AddRandom(1);
				}
				if (vals.Count > 0)
				{
					break;
				}
			}
		}
		Rand.SetSeed();
		CalcCost();
		CalcSlot();
		void AddAction()
		{
			if (model.ability.list.items.Count != 0)
			{
				ActList.Item a = model.ability.list.items.RandomItem();
				if (!(a.act.source.category != "ability"))
				{
					AddVal(a.act.source.id, a.chance * ((!a.pt) ? 1 : (-1)), allowStack: false, (int v) => 8 + a.act.source.cost[0] / 10 * 2);
					action++;
				}
			}
		}
		void AddAttribute()
		{
			Element element = listAttb[EClass.rnd(3)];
			AddVal(element.id, EClass.rndHalf(element.ValueWithoutLink / 2), allowStack: true, (int v) => v / 5 + 1);
		}
		void AddBody()
		{
			if (body == 0)
			{
				BodySlot bodySlot = null;
				for (int j = 0; j < 100; j++)
				{
					BodySlot bodySlot2 = model.body.slots.RandomItem();
					if (bodySlot2 != null && bodySlot2.elementId != 40)
					{
						bodySlot = bodySlot2;
						break;
					}
				}
				if (bodySlot != null)
				{
					AddVal(bodySlot.elementId, 1, allowStack: false, (int v) => 20);
					body++;
				}
			}
		}
		void AddFeat()
		{
			if (listFeat.Count != 0)
			{
				feat++;
				Element e = listFeat.RandomItem();
				if (maxSlot <= 1 || e.source.geneSlot <= 1)
				{
					if (e.source.geneSlot > maxSlot)
					{
						maxSlot = e.source.geneSlot;
					}
					AddVal(e.id, 1, allowStack: false, (int v) => e.source.cost[0] * 5);
				}
			}
		}
		void AddRandom(int n)
		{
			for (int l = 0; l < n; l++)
			{
				if (EClass.debug.enable && EClass.rnd(2) == 0)
				{
					AddSpecial();
				}
				else if (EClass.rnd(2) == 0)
				{
					AddSkill();
				}
				else
				{
					AddAttribute();
				}
			}
		}
		void AddSkill()
		{
			Element element2 = listSkill[Mathf.Clamp(EClass.rnd(6), 0, listSkill.Count - 1)];
			AddVal(element2.id, EClass.rndHalf(element2.ValueWithoutLink / 2), allowStack: true, (int v) => v / 5 + 1);
		}
		void AddSpecial()
		{
			if (EClass.rnd(3) == 0)
			{
				AddAction();
			}
			else if (EClass.rnd(5) == 0)
			{
				AddBody();
			}
			else if (listFeat.Count > 0)
			{
				AddFeat();
			}
			else if (EClass.rnd(2) == 0)
			{
				AddSkill();
			}
			else
			{
				AddAttribute();
			}
		}
		void AddVal(int id, int v, bool allowStack, Func<int, int> funcCost)
		{
			bool flag = false;
			int num = EClass.curve(v, 20, 10, 90);
			v = EClass.curve(v, 20, 10, 80);
			for (int k = 0; k < vals.Count; k += 2)
			{
				if (vals[k] == id)
				{
					if (allowStack)
					{
						v /= 2;
						num /= 2;
						vals[k + 1] += v;
						Debug.Log(vals[k + 1] + ": " + v + "/" + num);
						flag = true;
						break;
					}
					return;
				}
			}
			if (v != 0)
			{
				if (!flag)
				{
					vals.Add(id);
					vals.Add(v);
				}
				cost += funcCost(num);
			}
		}
	}

	public void CalcCost()
	{
		for (int i = 0; i < vals.Count; i += 2)
		{
			Element.Create(vals[i], vals[i + 1]);
		}
	}

	public void CalcSlot()
	{
		slot = 0;
		for (int i = 0; i < vals.Count; i += 2)
		{
			Element element = Element.Create(vals[i], vals[i + 1]);
			if (element.source.geneSlot > slot)
			{
				slot = element.source.geneSlot;
			}
		}
	}

	public static Type GetType(string idMat)
	{
		return idMat switch
		{
			"jelly" => Type.Default, 
			"gold" => Type.Superior, 
			"amethyst" => Type.Brain, 
			_ => Type.Inferior, 
		};
	}

	public string GetMaterialId(Type type)
	{
		return type switch
		{
			Type.Default => "jelly", 
			Type.Superior => "gold", 
			Type.Brain => "amethyst", 
			_ => "process", 
		};
	}

	public int GetDurationHour()
	{
		return cost * cost / 2;
	}

	public Type GetRandomType()
	{
		if (EClass.rnd(5) == 0)
		{
			return Type.Superior;
		}
		if (EClass.rnd(10) == 0)
		{
			return Type.Inferior;
		}
		return Type.Default;
	}

	public string GetText()
	{
		return "gene".lang((EClass.sources.cards.map.TryGetValue(id)?.GetName() ?? "???").ToTitleCase(), cost.ToString() ?? "");
	}

	public void WriteNote(UINote n)
	{
		if (slot >= 1)
		{
			n.AddText("isGeneReqSlots".lang(slot.ToString() ?? ""), FontColor.Warning);
		}
		if (!CanRemove())
		{
			n.AddText("isPermaGene".lang(), FontColor.Warning);
		}
		n.Space(4);
		if (type == Type.Brain)
		{
			SourceChara.Row row = EClass.sources.charas.map.TryGetValue(id);
			if (row != null)
			{
				string key = row.tactics.IsEmpty(EClass.sources.tactics.map.TryGetValue(row.id)?.id ?? EClass.sources.tactics.map.TryGetValue(row.job)?.id ?? "predator");
				n.AddText("gene_info".lang(EClass.sources.tactics.map[key].GetName().ToTitleCase(), ""), FontColor.ButtonGeneral);
			}
			for (int i = 0; i < vals.Count; i += 2)
			{
				int num = vals[i];
				int num2 = vals[i + 1];
				FontColor color = ((num2 >= 0) ? FontColor.Good : FontColor.Bad);
				string @ref = (num + 1).ToString() ?? "";
				string text = "";
				num2 = Mathf.Abs(num2 / 20) + 1;
				text = text + "[" + "*".Repeat(Mathf.Clamp(num2, 1, 5)) + ((num2 > 5) ? "+" : "") + "]";
				n.AddText("gene_info_brain".lang(@ref, text), color);
			}
			return;
		}
		for (int j = 0; j < vals.Count; j += 2)
		{
			Element element = Element.Create(vals[j], vals[j + 1]);
			string text2 = "";
			int num3 = element.Value / 10;
			FontColor color2 = FontColor.Good;
			switch (element.source.category)
			{
			case "slot":
				color2 = FontColor.Myth;
				num3 = -1;
				break;
			case "feat":
				color2 = FontColor.FoodMisc;
				num3 = -1;
				break;
			case "ability":
				color2 = FontColor.Topic2;
				num3 = -1;
				break;
			}
			if (num3 >= 0)
			{
				text2 = text2 + "[" + "*".Repeat(Mathf.Clamp(num3, 1, 5)) + ((num3 > 5) ? "+" : "") + "]";
			}
			if (EClass.debug.showExtra)
			{
				text2 = text2 + " " + element.Value;
			}
			n.AddText("gene_info".lang(element.Name.ToTitleCase(wholeText: true), text2), color2);
		}
	}

	public Element GetInvalidFeat(Chara c)
	{
		for (int i = 0; i < vals.Count; i += 2)
		{
			Element element = Element.Create(vals[i], vals[i + 1]);
			if (element.source.category == "feat" && c.Evalue(element.id) >= element.source.max)
			{
				return element;
			}
		}
		return null;
	}

	public Element GetInvalidAction(Chara c)
	{
		for (int i = 0; i < vals.Count; i += 2)
		{
			Element element = Element.Create(vals[i], vals[i + 1]);
			if (!(element.source.category == "ability"))
			{
				continue;
			}
			foreach (ActList.Item item in c.ability.list.items)
			{
				if (item.act.source.id == element.id)
				{
					return element;
				}
			}
		}
		return null;
	}
}
